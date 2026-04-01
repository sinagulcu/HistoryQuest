import axios from "axios";
import { format, isValid, parseISO } from "date-fns";
import { BarChart3, FileQuestion, FolderPlus, Plus, Users2 } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import {
  Bar,
  BarChart,
  CartesianGrid,
  Cell,
  Legend,
  Pie,
  PieChart,
  ResponsiveContainer,
  Tooltip,
  XAxis,
  YAxis,
} from "recharts";
import { useNavigate } from "react-router-dom";
import { categoryApi } from "@/api/category.api";
import { questionApi } from "@/api/question.api";
import { quizApi } from "@/api/quiz.api";
import { quizAttemptApi } from "@/api/quizAttempt.api";
import { statsApi, type DashboardStats } from "@/api/stats.api";
import { userApi } from "@/api/user.api";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/hooks/useAuth";
import type { Category } from "@/types/category.types";
import type { Question } from "@/types/question.types";
import type { Quiz } from "@/types/quiz.types";
import type { User } from "@/types/user.types";

interface KpiStats {
  totalQuizzes: number;
  totalQuestions: number;
  totalAttempts: number;
  totalUsers: number;
}

const pieColors = ["#a16207", "#b45309", "#92400e", "#ca8a04", "#713f12", "#854d0e"];

function getApiErrorMessage(error: unknown, fallbackMessage: string) {
  if (axios.isAxiosError(error)) {
    const responseData = error.response?.data as { message?: unknown } | undefined;
    if (typeof responseData?.message === "string") {
      return responseData.message;
    }
  }
  return fallbackMessage;
}

function getTimestamp(value: string | undefined, fallback: number | string): number {
  if (!value) {
    if (typeof fallback === "number") {
      return fallback;
    }

    return Number.parseInt(fallback.replace(/[^0-9]/g, "").slice(0, 12) || "0", 10);
  }

  const parsedDate = parseISO(value);
  if (!isValid(parsedDate)) {
    if (typeof fallback === "number") {
      return fallback;
    }

    return Number.parseInt(fallback.replace(/[^0-9]/g, "").slice(0, 12) || "0", 10);
  }

  return parsedDate.getTime();
}

function formatDateTime(value: string | undefined) {
  if (!value) {
    return "-";
  }
  const parsedDate = parseISO(value);
  if (!isValid(parsedDate)) {
    return "-";
  }
  return format(parsedDate, "dd.MM.yyyy HH:mm");
}

function extractRecentActivityText(item: Record<string, unknown>) {
  const candidates = [item.title, item.description, item.message, item.action];
  const textValue = candidates.find((candidate) => typeof candidate === "string");
  return typeof textValue === "string" ? textValue : "Sistem aktivitesi";
}

export default function DashboardPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const isAdmin = user?.role === "Admin";

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [kpiStats, setKpiStats] = useState<KpiStats>({ totalQuizzes: 0, totalQuestions: 0, totalAttempts: 0, totalUsers: 0 });
  const [latestQuizzes, setLatestQuizzes] = useState<Quiz[]>([]);
  const [latestQuestions, setLatestQuestions] = useState<Question[]>([]);
  const [categoryDistribution, setCategoryDistribution] = useState<Array<{ name: string; value: number }>>([]);
  const [difficultyDistribution, setDifficultyDistribution] = useState<Array<{ name: string; value: number }>>([]);
  const [roleDistribution, setRoleDistribution] = useState<Array<{ name: string; value: number }>>([]);
  const [recentActivity, setRecentActivity] = useState<Array<{ text: string; dateLabel: string }>>([]);

  const fetchDashboardData = async () => {
    setLoading(true);
    setError(null);

    try {
      const [quizResponse, questionResponse, categoryResponse] = await Promise.all([
        quizApi.getAll(),
        questionApi.getAll(),
        categoryApi.getAll(),
      ]);

      const quizzes = quizResponse.data;
      const questions = questionResponse.data;
      const categories = categoryResponse.data;

      let statsPayload: DashboardStats | null = null;
      try {
        const statsResponse = await statsApi.getDashboard();
        statsPayload = statsResponse.data;
      } catch {
        statsPayload = null;
      }

      let totalAttempts = statsPayload?.totalAttempts ?? 0;
      try {
        const attemptsResponse = await quizAttemptApi.getAll();
        totalAttempts = attemptsResponse.data.length;
      } catch {
        totalAttempts = statsPayload?.totalAttempts ?? 0;
      }

      let users: User[] = [];
      if (isAdmin) {
        try {
          const usersResponse = await userApi.getAll();
          users = usersResponse.data;
        } catch {
          users = [];
        }
      }

      setKpiStats({
        totalQuizzes: statsPayload?.totalQuizzes ?? quizzes.length,
        totalQuestions: statsPayload?.totalQuestions ?? questions.length,
        totalAttempts,
        totalUsers: statsPayload?.totalUsers ?? users.length,
      });

      const sortedQuizzes = [...quizzes].sort(
        (a, b) => getTimestamp(b.createdAt, b.id) - getTimestamp(a.createdAt, a.id)
      );
      const sortedQuestions = [...questions].sort(
        (a, b) => getTimestamp(b.createdAt, b.id) - getTimestamp(a.createdAt, a.id)
      );
      setLatestQuizzes(sortedQuizzes.slice(0, 5));
      setLatestQuestions(sortedQuestions.slice(0, 5));

      const categoryNameById = new Map<number, string>(categories.map((category: Category) => [category.id, category.name]));
      const categoryCounter = new Map<number, number>();
      questions.forEach((question) => {
        categoryCounter.set(question.categoryId, (categoryCounter.get(question.categoryId) ?? 0) + 1);
      });
      const categoryChartData = Array.from(categoryCounter.entries()).map(([categoryId, count]) => ({
        name: categoryNameById.get(categoryId) ?? `Kategori #${categoryId}`,
        value: count,
      }));
      setCategoryDistribution(categoryChartData);

      const difficultyCounter = { 1: 0, 2: 0, 3: 0 };
      questions.forEach((question) => {
        if (question.difficultyLevel === 1 || question.difficultyLevel === 2 || question.difficultyLevel === 3) {
          difficultyCounter[question.difficultyLevel] += 1;
        }
      });
      setDifficultyDistribution([
        { name: "Kolay", value: difficultyCounter[1] },
        { name: "Orta", value: difficultyCounter[2] },
        { name: "Zor", value: difficultyCounter[3] },
      ]);

      if (isAdmin) {
        const roleCounter = { Admin: 0, Teacher: 0, Student: 0 };
        users.forEach((currentUser) => {
          roleCounter[currentUser.role] += 1;
        });
        setRoleDistribution([
          { name: "Admin", value: roleCounter.Admin },
          { name: "Teacher", value: roleCounter.Teacher },
          { name: "Student", value: roleCounter.Student },
        ]);
      } else {
        setRoleDistribution([]);
      }

      const activityList: Array<{ text: string; dateLabel: string }> = [];
      if (Array.isArray(statsPayload?.recentActivity)) {
        statsPayload.recentActivity.slice(0, 5).forEach((entry) => {
          if (typeof entry === "string") {
            activityList.push({ text: entry, dateLabel: "" });
            return;
          }

          if (entry && typeof entry === "object") {
            const entryRecord = entry as Record<string, unknown>;
            const createdValue =
              typeof entryRecord.createdAt === "string"
                ? entryRecord.createdAt
                : typeof entryRecord.date === "string"
                  ? entryRecord.date
                  : undefined;

            activityList.push({
              text: extractRecentActivityText(entryRecord),
              dateLabel: formatDateTime(createdValue),
            });
          }
        });
      }
      setRecentActivity(activityList);
    } catch (requestError) {
      setError(getApiErrorMessage(requestError, "Dashboard verileri yuklenirken hata olustu."));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchDashboardData();
  }, [isAdmin]);

  const kpiCards = useMemo(() => {
    const cards = [
      { title: "Toplam Quiz", value: kpiStats.totalQuizzes, icon: BarChart3 },
      { title: "Toplam Soru", value: kpiStats.totalQuestions, icon: FileQuestion },
      { title: "Toplam Deneme", value: kpiStats.totalAttempts, icon: BarChart3 },
    ];

    if (isAdmin) {
      cards.push({ title: "Toplam Kullanici", value: kpiStats.totalUsers, icon: Users2 });
    }

    return cards;
  }, [isAdmin, kpiStats.totalAttempts, kpiStats.totalQuestions, kpiStats.totalQuizzes, kpiStats.totalUsers]);

  return (
    <div className="space-y-6">
      <PageSection
        title="Dashboard"
        description="Platform ozet metrikleri, dagilimlar ve son aktiviteler"
        actions={
          <div className="flex flex-wrap gap-2">
            <Button className="gap-2" onClick={() => navigate("/quizzes/create")}>
              <Plus className="h-4 w-4" />
              Yeni Quiz
            </Button>
            <Button variant="outline" className="gap-2" onClick={() => navigate("/questions/create")}>
              <Plus className="h-4 w-4" />
              Yeni Soru
            </Button>
            <Button variant="outline" className="gap-2" onClick={() => navigate("/categories")}>
              <FolderPlus className="h-4 w-4" />
              Yeni Kategori
            </Button>
          </div>
        }
      />

      {loading ? <LoadingState message="Dashboard verileri yukleniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchDashboardData} /> : null}

      {!loading && !error ? (
        <>
          <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
            {kpiCards.map((card) => {
              const Icon = card.icon;
              return (
                <div
                  key={card.title}
                  className="rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur transition-all hover:-translate-y-0.5 hover:shadow-md dark:border-stone-800 dark:bg-stone-900/65"
                >
                  <div className="mb-2 flex items-center justify-between">
                    <p className="text-sm text-stone-500 dark:text-stone-400">{card.title}</p>
                    <Icon className="h-4 w-4 text-amber-700 dark:text-amber-400" />
                  </div>
                  <p className="text-2xl font-semibold text-stone-900 dark:text-stone-100">{card.value.toLocaleString("tr-TR")}</p>
                </div>
              );
            })}
          </section>

          <section className="grid gap-4 xl:grid-cols-2">
            <div className="rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
              <h2 className="mb-4 text-sm font-semibold text-stone-900 dark:text-stone-100">Son Eklenen Quizler</h2>
              {latestQuizzes.length === 0 ? (
                <p className="text-sm text-stone-500 dark:text-stone-400">Quiz verisi bulunamadi.</p>
              ) : (
                <div className="overflow-x-auto">
                  <table className="min-w-full text-sm">
                    <thead>
                      <tr className="text-left text-xs uppercase tracking-wide text-stone-500 dark:text-stone-400">
                        <th className="py-2">Baslik</th>
                        <th className="py-2">Durum</th>
                        <th className="py-2">Tarih</th>
                      </tr>
                    </thead>
                    <tbody>
                      {latestQuizzes.map((quiz) => (
                        <tr className="border-t border-stone-100 text-stone-700 dark:border-stone-800 dark:text-stone-200" key={quiz.id}>
                          <td className="py-2">{quiz.title}</td>
                          <td className="py-2">{quiz.isPublished ? "Yayinda" : "Taslak"}</td>
                          <td className="py-2">{formatDateTime(quiz.createdAt)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>

            <div className="rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
              <h2 className="mb-4 text-sm font-semibold text-stone-900 dark:text-stone-100">Son Eklenen Sorular</h2>
              {latestQuestions.length === 0 ? (
                <p className="text-sm text-stone-500 dark:text-stone-400">Soru verisi bulunamadi.</p>
              ) : (
                <div className="overflow-x-auto">
                  <table className="min-w-full text-sm">
                    <thead>
                      <tr className="text-left text-xs uppercase tracking-wide text-stone-500 dark:text-stone-400">
                        <th className="py-2">Soru</th>
                        <th className="py-2">Zorluk</th>
                        <th className="py-2">Tarih</th>
                      </tr>
                    </thead>
                    <tbody>
                      {latestQuestions.map((question) => (
                        <tr key={question.id} className="border-t border-stone-100 text-stone-700 dark:border-stone-800 dark:text-stone-200">
                          <td className="max-w-sm py-2">{question.text}</td>
                          <td className="py-2">{question.difficultyLevel === 1 ? "Kolay" : question.difficultyLevel === 2 ? "Orta" : "Zor"}</td>
                          <td className="py-2">{formatDateTime(question.createdAt)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>
          </section>

          <section className="grid gap-4 xl:grid-cols-2">
            <div className="rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
              <h2 className="mb-3 text-sm font-semibold text-stone-900 dark:text-stone-100">Kategorilere Gore Soru Dagilimi</h2>
              {categoryDistribution.length === 0 ? (
                <p className="text-sm text-stone-500 dark:text-stone-400">Dagilim icin yeterli soru verisi yok.</p>
              ) : (
                <div className="h-72">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie data={categoryDistribution} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={100} label>
                        {categoryDistribution.map((entry, index) => (
                          <Cell key={entry.name} fill={pieColors[index % pieColors.length]} />
                        ))}
                      </Pie>
                      <Tooltip />
                      <Legend />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              )}
            </div>

            <div className="rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
              <h2 className="mb-3 text-sm font-semibold text-stone-900 dark:text-stone-100">Zorluk Seviyesi Dagilimi</h2>
              {difficultyDistribution.every((entry) => entry.value === 0) ? (
                <p className="text-sm text-stone-500 dark:text-stone-400">Dagilim icin yeterli soru verisi yok.</p>
              ) : (
                <div className="h-72">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={difficultyDistribution}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis allowDecimals={false} />
                      <Tooltip />
                      <Bar dataKey="value" fill="#a16207" radius={[8, 8, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              )}
            </div>
          </section>

          {isAdmin ? (
            <section className="grid gap-4 xl:grid-cols-2">
              <div className="rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
                <h2 className="mb-3 text-sm font-semibold text-stone-900 dark:text-stone-100">Rol Dagilimi</h2>
                {roleDistribution.length === 0 ? (
                  <p className="text-sm text-stone-500 dark:text-stone-400">Rol dagilimi verisi alinamadi.</p>
                ) : (
                  <div className="h-72">
                    <ResponsiveContainer width="100%" height="100%">
                      <PieChart>
                        <Pie data={roleDistribution} dataKey="value" nameKey="name" cx="50%" cy="50%" outerRadius={100} label>
                          {roleDistribution.map((entry, index) => (
                            <Cell key={entry.name} fill={pieColors[index % pieColors.length]} />
                          ))}
                        </Pie>
                        <Tooltip />
                        <Legend />
                      </PieChart>
                    </ResponsiveContainer>
                  </div>
                )}
              </div>

              <div className="rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
                <h2 className="mb-3 text-sm font-semibold text-stone-900 dark:text-stone-100">Son Aktiviteler</h2>
                {recentActivity.length === 0 ? (
                  <p className="text-sm text-stone-500 dark:text-stone-400">Son aktivite kaydi bulunamadi.</p>
                ) : (
                  <ul className="space-y-3 text-sm text-stone-700 dark:text-stone-200">
                    {recentActivity.map((activity, index) => (
                      <li
                        key={`${activity.text}-${index}`}
                        className="rounded-lg border border-stone-200 bg-white/70 p-3 dark:border-stone-800 dark:bg-stone-950/30"
                      >
                        <p>{activity.text}</p>
                        {activity.dateLabel ? <p className="mt-1 text-xs text-stone-500 dark:text-stone-400">{activity.dateLabel}</p> : null}
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            </section>
          ) : null}
        </>
      ) : null}
    </div>
  );
}

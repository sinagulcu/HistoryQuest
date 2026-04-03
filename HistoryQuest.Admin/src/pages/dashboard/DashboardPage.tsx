import axios from "axios";
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
import { userApi } from "@/api/user.api";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/hooks/useAuth";
import type { Category } from "@/types/category.types";
import type { Question } from "@/types/question.types";
import type { Quiz } from "@/types/quiz.types";
import { formatServerDateTime, getServerTimestamp } from "@/utils/dateTime";

interface KpiStats {
  totalQuizzes: number;
  totalQuestions: number;
  totalAttempts: number;
  totalUsers: number;
}

const pieColors = ["#0ea5a4", "#2563eb", "#7c3aed", "#16a34a", "#d97706", "#dc2626", "#0891b2"];

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
  const fallbackValue =
    typeof fallback === "number"
      ? fallback
      : Number.parseInt(fallback.replace(/[^0-9]/g, "").slice(0, 12) || "0", 10);
  return getServerTimestamp(value, fallbackValue);
}

function formatDateTime(value: string | undefined) {
  const formatted = formatServerDateTime(value);
  return formatted === "-" ? "Tarih bilgisi yok" : formatted;
}

export default function DashboardPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const isAdmin = String(user?.role ?? "").toLowerCase() === "admin";

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

      setKpiStats({
        totalQuizzes: quizzes.length,
        totalQuestions: questions.length,
        totalAttempts: 0,
        totalUsers: 0,
      });

      let totalUsers = 0;
      try {
        const userCountResponse = await userApi.getCount();
        totalUsers = userCountResponse.data.totalUsers;
      } catch {
        totalUsers = 0;
      }

      try {
        const usersResponse = await userApi.getAll();
        const roleCounter: Record<string, number> = { Admin: 0, Teacher: 0, Student: 0 };
        usersResponse.data.forEach((u) => {
          if (u.role in roleCounter) {
            roleCounter[u.role] += 1;
          }
        });

        setRoleDistribution([
          { name: "Admin", value: roleCounter.Admin },
          { name: "Teacher", value: roleCounter.Teacher },
          { name: "Student", value: roleCounter.Student },
        ]);

        if (totalUsers <= 0) {
          totalUsers = usersResponse.data.length;
        }
      } catch {
        setRoleDistribution([]);
      }

      setKpiStats((prev) => ({ ...prev, totalUsers }));

      const sortedQuizzes = [...quizzes].sort(
        (a, b) => getTimestamp(b.createdAt, b.id) - getTimestamp(a.createdAt, a.id)
      );
      const sortedQuestions = [...questions].sort(
        (a, b) => getTimestamp(b.createdAt, b.id) - getTimestamp(a.createdAt, a.id)
      );

      const latestQuizCandidates = sortedQuizzes.slice(0, 5);
      const missingQuizDateCandidates = latestQuizCandidates.filter((quiz) => !quiz.createdAt);
      let latestQuizData = latestQuizCandidates;
      if (missingQuizDateCandidates.length > 0) {
        const quizDetailResults = await Promise.allSettled(
          missingQuizDateCandidates.map((quiz) => quizApi.getById(quiz.id))
        );
        const quizDetailMap = new Map(
          quizDetailResults
            .filter((result): result is PromiseFulfilledResult<Awaited<ReturnType<typeof quizApi.getById>>> => result.status === "fulfilled")
            .map((result) => [result.value.data.id, result.value.data])
        );

        latestQuizData = latestQuizCandidates.map((quiz) => ({
          ...quiz,
          ...quizDetailMap.get(quiz.id),
        }));
      }

      const latestQuestionCandidates = sortedQuestions.slice(0, 5);
      const missingDateCandidates = latestQuestionCandidates.filter((question) => !question.createdAt);
      if (missingDateCandidates.length > 0) {
        const detailResults = await Promise.allSettled(
          missingDateCandidates.map((question) => questionApi.getById(question.id))
        );
        const detailMap = new Map(
          detailResults
            .filter((result): result is PromiseFulfilledResult<Awaited<ReturnType<typeof questionApi.getById>>> => result.status === "fulfilled")
            .map((result) => [result.value.data.id, result.value.data])
        );

        setLatestQuestions(
          latestQuestionCandidates.map((question) => detailMap.get(question.id) || question)
        );
      } else {
        setLatestQuestions(latestQuestionCandidates);
      }
      setLatestQuizzes(latestQuizData);

      const categoryNameById = new Map<string, string>(categories.map((category: Category) => [category.id, category.name]));
      const categoryCounter = new Map<string, number>();
      questions.forEach((question) => {
        categoryCounter.set(question.categoryId, (categoryCounter.get(question.categoryId) ?? 0) + 1);
      });
      const categoryChartData = Array.from(categoryCounter.entries()).map(([categoryId, count]) => ({
        name: categoryNameById.get(categoryId) ?? "Kategori belirtilmemiş",
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

      if (!isAdmin) {
        setRoleDistribution([]);
      }

      const activityList = [
        ...latestQuizData.slice(0, 3).map((quiz) => ({
          text: `Yeni quiz oluşturuldu: ${quiz.title}`,
          dateLabel: formatDateTime(quiz.createdAt),
          timestamp: getTimestamp(quiz.createdAt, 0),
        })),
        ...latestQuestionCandidates.slice(0, 3).map((question) => ({
          text: `Yeni soru eklendi: ${question.text}`,
          dateLabel: formatDateTime(question.createdAt),
          timestamp: getTimestamp(question.createdAt, 0),
        })),
      ];

      activityList.sort((a, b) => b.timestamp - a.timestamp);

      setRecentActivity(activityList.map(({ text, dateLabel }) => ({ text, dateLabel })));
    } catch (requestError) {
      setError(getApiErrorMessage(requestError, "Dashboard verileri yüklenirken hata oluştu."));
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
      { title: "Toplam Kullanıcı", value: kpiStats.totalUsers, icon: Users2 },
    ];

    return cards;
  }, [kpiStats.totalAttempts, kpiStats.totalQuestions, kpiStats.totalQuizzes, kpiStats.totalUsers]);

  return (
    <div className="space-y-6">
      <PageSection
        title="Dashboard"
        description="Platform özet metrikleri, dağılımlar ve son aktiviteler"
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

      {loading ? <LoadingState message="Dashboard verileri yükleniyor..." /> : null}

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
            <div className="min-w-0 rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
              <h2 className="mb-4 text-sm font-semibold text-stone-900 dark:text-stone-100">Son Eklenen Quizler</h2>
              {latestQuizzes.length === 0 ? (
                <p className="text-sm text-stone-500 dark:text-stone-400">Quiz verisi bulunamadı.</p>
              ) : (
                <div className="overflow-x-auto">
                  <table className="min-w-full text-sm">
                    <thead>
                      <tr className="text-left text-xs uppercase tracking-wide text-stone-500 dark:text-stone-400">
                          <th className="py-2">Başlık</th>
                        <th className="py-2">Durum</th>
                        <th className="py-2">Tarih</th>
                      </tr>
                    </thead>
                    <tbody>
                      {latestQuizzes.map((quiz) => (
                        <tr className="border-t border-stone-100 text-stone-700 dark:border-stone-800 dark:text-stone-200" key={quiz.id}>
                          <td className="py-2">{quiz.title}</td>
                          <td className="py-2">{quiz.isPublished ? "Yayında" : "Taslak"}</td>
                          <td className="py-2">{formatDateTime(quiz.createdAt)}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </div>

            <div className="min-w-0 rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
              <h2 className="mb-4 text-sm font-semibold text-stone-900 dark:text-stone-100">Son Eklenen Sorular</h2>
              {latestQuestions.length === 0 ? (
                <p className="text-sm text-stone-500 dark:text-stone-400">Soru verisi bulunamadı.</p>
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
              <h2 className="mb-3 text-sm font-semibold text-stone-900 dark:text-stone-100">Kategorilere Göre Soru Dağılımı</h2>
              {categoryDistribution.length === 0 ? (
                <p className="text-sm text-stone-500 dark:text-stone-400">Dağılım için yeterli soru verisi yok.</p>
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
              <h2 className="mb-3 text-sm font-semibold text-stone-900 dark:text-stone-100">Zorluk Seviyesi Dağılımı</h2>
              {difficultyDistribution.every((entry) => entry.value === 0) ? (
                <p className="text-sm text-stone-500 dark:text-stone-400">Dağılım için yeterli soru verisi yok.</p>
              ) : (
                <div className="h-72">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={difficultyDistribution}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis allowDecimals={false} />
                      <Tooltip />
                      <Bar dataKey="value" fill="#0ea5a4" radius={[8, 8, 0, 0]} />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              )}
            </div>
          </section>

          {isAdmin ? (
            <section className="grid gap-4 xl:grid-cols-2">
              <div className="min-w-0 rounded-2xl border border-stone-200/80 bg-white/85 p-4 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
                <h2 className="mb-3 text-sm font-semibold text-stone-900 dark:text-stone-100">Rol Dağılımı</h2>
                {roleDistribution.length === 0 ? (
                  <p className="text-sm text-stone-500 dark:text-stone-400">Rol dağılımı verisi alınamadı.</p>
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
                  <p className="text-sm text-stone-500 dark:text-stone-400">Son aktivite kaydı bulunamadı.</p>
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

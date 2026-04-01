import axios from "axios";
import { Eye, Pencil, Plus, Trash2, Upload, Download } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import { quizApi } from "@/api/quiz.api";
import ConfirmDialog from "@/components/shared/ConfirmDialog";
import EmptyState from "@/components/shared/EmptyState";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useAuth } from "@/hooks/useAuth";
import type { Category } from "@/types/category.types";
import type { Quiz } from "@/types/quiz.types";

const difficultyText: Record<number, string> = {
  1: "Kolay",
  2: "Orta",
  3: "Zor",
};

export default function QuizListPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [quizzes, setQuizzes] = useState<Quiz[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [categoryFilter, setCategoryFilter] = useState<number | "all">("all");
  const [difficultyFilter, setDifficultyFilter] = useState<number | "all">("all");
  const [statusFilter, setStatusFilter] = useState<"all" | "published" | "draft">("all");
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [pendingDeleteQuiz, setPendingDeleteQuiz] = useState<Quiz | null>(null);
  const [publishingId, setPublishingId] = useState<string | null>(null);

  const fetchData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [quizResponse, categoryResponse] = await Promise.all([quizApi.getAll(), categoryApi.getAll()]);
      setQuizzes(quizResponse.data);
      setCategories(categoryResponse.data);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Quizler yuklenirken hata olustu.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const canManageQuiz = (quiz: Quiz) => {
    if (!user) {
      return false;
    }
    if (user.role === "Admin") {
      return true;
    }
    return user.id === quiz.createdByUserId;
  };

  const getCategoryName = (quiz: Quiz) => {
    if (quiz.categoryName) {
      return quiz.categoryName;
    }
    return categories.find((category) => category.id === quiz.categoryId)?.name || `#${quiz.categoryId}`;
  };

  const filteredQuizzes = useMemo(() => {
    return quizzes.filter((quiz) => {
      const searchMatch = quiz.title.toLowerCase().includes(search.toLowerCase());
      const categoryMatch = categoryFilter === "all" || quiz.categoryId === categoryFilter;
      const difficultyMatch = difficultyFilter === "all" || quiz.difficultyLevel === difficultyFilter;
      const statusMatch =
        statusFilter === "all" || (statusFilter === "published" ? quiz.isPublished : !quiz.isPublished);

      return searchMatch && categoryMatch && difficultyMatch && statusMatch;
    });
  }, [categoryFilter, difficultyFilter, quizzes, search, statusFilter]);

  const handleDelete = async (quiz: Quiz) => {
    if (!canManageQuiz(quiz)) {
      toast.error("Bu quizi silme yetkiniz yok");
      return;
    }

    setDeletingId(quiz.id);
    try {
      await quizApi.delete(quiz.id);
      toast.success("Quiz silindi");
      setPendingDeleteQuiz(null);
      await fetchData();
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Quiz silinirken hata olustu.";
      toast.error(message);
    } finally {
      setDeletingId(null);
    }
  };

  const handlePublishToggle = async (quiz: Quiz) => {
    if (!canManageQuiz(quiz)) {
      toast.error("Bu quizin yayin durumunu degistirme yetkiniz yok");
      return;
    }

    setPublishingId(quiz.id);
    try {
      if (quiz.isPublished) {
        await quizApi.unpublish(quiz.id);
        toast.success("Quiz taslak durumuna alindi");
      } else {
        await quizApi.publish(quiz.id);
        toast.success("Quiz yayinlandi");
      }
      await fetchData();
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Yayin durumu degistirilemedi.";
      toast.error(message);
    } finally {
      setPublishingId(null);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection
        title="Quizler"
        description="Quizleri arayin, filtreleyin ve yetkinize gore yonetin."
        actions={
          <Button onClick={() => navigate("/quizzes/create")} className="gap-2">
            <Plus className="h-4 w-4" />
            Yeni Quiz Olustur
          </Button>
        }
      />

      <div className="grid gap-3 md:grid-cols-4">
        <Input placeholder="Basliga gore ara" value={search} onChange={(event) => setSearch(event.target.value)} />
        <select
          className="h-10 rounded-lg border border-stone-300 bg-white/90 px-3 text-sm dark:border-stone-700 dark:bg-stone-900/80"
          value={categoryFilter}
          onChange={(event) => {
            const value = event.target.value;
            setCategoryFilter(value === "all" ? "all" : Number(value));
          }}
        >
          <option value="all">Tum kategoriler</option>
          {categories.map((category) => (
            <option key={category.id} value={category.id}>
              {category.name}
            </option>
          ))}
        </select>
        <select
          className="h-10 rounded-lg border border-stone-300 bg-white/90 px-3 text-sm dark:border-stone-700 dark:bg-stone-900/80"
          value={difficultyFilter}
          onChange={(event) => {
            const value = event.target.value;
            setDifficultyFilter(value === "all" ? "all" : Number(value));
          }}
        >
          <option value="all">Tum zorluklar</option>
          <option value={1}>Kolay</option>
          <option value={2}>Orta</option>
          <option value={3}>Zor</option>
        </select>
        <select
          className="h-10 rounded-lg border border-stone-300 bg-white/90 px-3 text-sm dark:border-stone-700 dark:bg-stone-900/80"
          value={statusFilter}
          onChange={(event) => setStatusFilter(event.target.value as "all" | "published" | "draft")}
        >
          <option value="all">Tum durumlar</option>
          <option value="published">Yayinda</option>
          <option value="draft">Taslak</option>
        </select>
      </div>

      {loading ? <LoadingState message="Quizler yukleniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchData} /> : null}

      {!loading && !error && filteredQuizzes.length === 0 ? (
        <EmptyState message="Henuz quiz olusturulmamis. Ilk quizinizi olusturun." />
      ) : null}

      {!loading && !error && filteredQuizzes.length > 0 ? (
        <div className="overflow-x-auto rounded-2xl border border-stone-200/80 bg-white/85 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
          <table className="min-w-full divide-y divide-stone-200 dark:divide-stone-800">
            <thead>
              <tr className="text-left text-xs uppercase tracking-wide text-stone-500 dark:text-stone-400">
                <th className="px-4 py-3">Baslik</th>
                <th className="px-4 py-3">Kategori</th>
                <th className="px-4 py-3">Zorluk</th>
                <th className="px-4 py-3">Soru Sayisi</th>
                <th className="px-4 py-3">Sure</th>
                <th className="px-4 py-3">Durum</th>
                <th className="px-4 py-3">Olusturan</th>
                <th className="px-4 py-3 text-right">Islemler</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-stone-200 dark:divide-stone-800">
              {filteredQuizzes.map((quiz) => {
                const canManage = canManageQuiz(quiz);
                return (
                  <tr key={quiz.id} className="text-sm text-stone-700 transition-colors hover:bg-stone-50/80 dark:text-stone-200 dark:hover:bg-stone-800/40">
                    <td className="px-4 py-3 font-medium">{quiz.title}</td>
                    <td className="px-4 py-3">{getCategoryName(quiz)}</td>
                    <td className="px-4 py-3">{difficultyText[quiz.difficultyLevel] || "-"}</td>
                    <td className="px-4 py-3">{quiz.questions?.length ?? "-"}</td>
                    <td className="px-4 py-3">{quiz.timeLimitMinutes} dk</td>
                    <td className="px-4 py-3">
                      <span
                        className={`inline-flex rounded-full px-2 py-1 text-xs font-semibold ${
                          quiz.isPublished
                            ? "bg-emerald-100 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-300"
                            : "bg-[#f7efdc] text-[#876822] dark:bg-[#2f2615] dark:text-[#d7bd7e]"
                        }`}
                      >
                        {quiz.isPublished ? "Yayinda" : "Taslak"}
                      </span>
                    </td>
                    <td className="px-4 py-3">{quiz.createdByUserName || quiz.createdByUserId}</td>
                    <td className="px-4 py-3">
                      <div className="flex justify-end gap-2">
                        <Button variant="outline" onClick={() => navigate(`/quizzes/${quiz.id}`)} className="gap-2">
                          <Eye className="h-4 w-4" />
                          Detay
                        </Button>
                        {canManage ? (
                          <>
                            <Button variant="outline" onClick={() => navigate(`/quizzes/${quiz.id}/edit`)} className="gap-2">
                              <Pencil className="h-4 w-4" />
                              Duzenle
                            </Button>
                            <Button
                              variant="outline"
                              onClick={() => handlePublishToggle(quiz)}
                              disabled={publishingId === quiz.id}
                              className="gap-2"
                            >
                              {quiz.isPublished ? <Download className="h-4 w-4" /> : <Upload className="h-4 w-4" />}
                              {quiz.isPublished ? "Kaldir" : "Yayinla"}
                            </Button>
                            <Button
                              variant="danger"
                              onClick={() => setPendingDeleteQuiz(quiz)}
                              disabled={deletingId === quiz.id}
                              className="gap-2"
                            >
                              <Trash2 className="h-4 w-4" />
                              Sil
                            </Button>
                          </>
                        ) : null}
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      ) : null}

      <ConfirmDialog
        open={Boolean(pendingDeleteQuiz)}
        title="Quiz Sil"
        description={
          pendingDeleteQuiz
            ? `${pendingDeleteQuiz.title} baslikli quizi silmek istediginize emin misiniz? Bu islem geri alinamaz.`
            : ""
        }
        confirmLabel="Evet, Sil"
        loading={deletingId !== null}
        onCancel={() => setPendingDeleteQuiz(null)}
        onConfirm={() => {
          if (pendingDeleteQuiz) {
            handleDelete(pendingDeleteQuiz);
          }
        }}
      />
    </div>
  );
}

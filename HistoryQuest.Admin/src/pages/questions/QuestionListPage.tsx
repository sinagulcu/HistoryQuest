import axios from "axios";
import { Eye, Pencil, Plus, Trash2, X } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import { challengeApi } from "@/api/challenge.api";
import { questionApi } from "@/api/question.api";
import { quizApi } from "@/api/quiz.api";
import ConfirmDialog from "@/components/shared/ConfirmDialog";
import EmptyState from "@/components/shared/EmptyState";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import PageSection from "@/components/shared/PageSection";
import { useAuth } from "@/hooks/useAuth";
import type { Category } from "@/types/category.types";
import type { Question } from "@/types/question.types";

const difficultyText: Record<number, string> = {
  1: "Kolay",
  2: "Orta",
  3: "Zor",
};

export default function QuestionListPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [questions, setQuestions] = useState<Question[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [categoryFilter, setCategoryFilter] = useState<string | "all">("all");
  const [difficultyFilter, setDifficultyFilter] = useState<number | "all">("all");
  const [creatorFilter, setCreatorFilter] = useState<"all" | "mine" | "others">("all");
  const [previewQuestion, setPreviewQuestion] = useState<Question | null>(null);
  const [previewLoading, setPreviewLoading] = useState(false);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [pendingDeleteQuestion, setPendingDeleteQuestion] = useState<Question | null>(null);
  const [deleteGuardMessage, setDeleteGuardMessage] = useState<string | null>(null);
  const [deleteGuardLoading, setDeleteGuardLoading] = useState(false);

  const fetchQuestions = async () => {
    setLoading(true);
    setError(null);
    try {
      const [questionsResponse, categoriesResponse] = await Promise.all([questionApi.getAll(), categoryApi.getAll()]);

      const baseQuestions = questionsResponse.data;
      const detailTargets = baseQuestions.filter((question) => !question.categoryId);

      let enrichedQuestions = baseQuestions;
      if (detailTargets.length > 0) {
        const detailResults = await Promise.allSettled(detailTargets.map((question) => questionApi.getById(question.id)));
        const detailMap = new Map(
          detailResults
            .filter((result): result is PromiseFulfilledResult<Awaited<ReturnType<typeof questionApi.getById>>> => result.status === "fulfilled")
            .map((result) => [result.value.data.id, result.value.data])
        );

        enrichedQuestions = baseQuestions.map((question) => detailMap.get(question.id) || question);
      }

      setQuestions(enrichedQuestions);
      setCategories(categoriesResponse.data);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Sorular yuklenirken hata olustu.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchQuestions();
  }, []);

  const filteredQuestions = useMemo(() => {
    return questions.filter((question) => {
      const searchMatch = question.text.toLowerCase().includes(search.toLowerCase());
      const categoryMatch = categoryFilter === "all" || question.categoryId === categoryFilter;
      const difficultyMatch = difficultyFilter === "all" || question.difficultyLevel === difficultyFilter;

      let creatorMatch = true;
      if (creatorFilter === "mine") {
        creatorMatch = question.createdByUserId === user?.id;
      }
      if (creatorFilter === "others") {
        creatorMatch = question.createdByUserId !== user?.id;
      }

      return searchMatch && categoryMatch && difficultyMatch && creatorMatch;
    });
  }, [categoryFilter, creatorFilter, difficultyFilter, questions, search, user?.id]);

  const getCategoryName = (question: Question) => {
    if (question.categoryName) {
      return question.categoryName;
    }
    return categories.find((category) => category.id === question.categoryId)?.name || "Belirtilmedi";
  };

  const canManageQuestion = (question: Question) => {
    if (!user) {
      return false;
    }
    if (user.role === "Admin") {
      return true;
    }
    return user.id === question.createdByUserId;
  };

  const handlePreview = async (questionId: string) => {
    setPreviewLoading(true);
    try {
      const { data } = await questionApi.getById(questionId);
      setPreviewQuestion(data);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru detayi yuklenemedi.";
      toast.error(message);
    } finally {
      setPreviewLoading(false);
    }
  };

  const handleDelete = async (question: Question) => {
    if (!canManageQuestion(question)) {
      toast.error("Bu soruyu silme yetkiniz yok");
      return;
    }

    setDeletingId(question.id);
    try {
      await questionApi.delete(question.id);
      toast.success("Soru silindi");
      setPendingDeleteQuestion(null);
      await fetchQuestions();
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru silinirken hata olustu.";
      toast.error(message);
    } finally {
      setDeletingId(null);
    }
  };

  const checkQuestionUsageBeforeDelete = async (question: Question) => {
    setDeleteGuardLoading(true);
    setDeleteGuardMessage(null);

    try {
      const [challengeResponse, quizResponse] = await Promise.all([challengeApi.getAll(), quizApi.getAll()]);

      const challengeUsages = challengeResponse.data.filter((challenge) => challenge.questionId === question.id);

      const quizDetails = await Promise.allSettled(quizResponse.data.map((quiz) => quizApi.getById(quiz.id)));
      const quizUsages = quizDetails
        .filter((result): result is PromiseFulfilledResult<Awaited<ReturnType<typeof quizApi.getById>>> => result.status === "fulfilled")
        .map((result) => result.value.data)
        .filter((quiz) => quiz.questions?.some((quizQuestion) => quizQuestion.id === question.id));

      if (challengeUsages.length === 0 && quizUsages.length === 0) {
        setPendingDeleteQuestion(question);
        return;
      }

      const quizNames = quizUsages.map((quiz) => quiz.title).filter(Boolean);
      const challengeNames = challengeUsages.map((challenge) => challenge.title).filter(Boolean);

      const parts: string[] = [];
      if (quizNames.length > 0) {
        parts.push(`Quizlerde kullaniliyor: ${quizNames.join(", ")}`);
      }
      if (challengeNames.length > 0) {
        parts.push(`Meydan okumalarda kullaniliyor: ${challengeNames.join(", ")}`);
      }

      setDeleteGuardMessage(
        `Bu soru diger kayitlarda kullaniliyor. Once ilgili kayitlardan soruyu kaldirip veya o kayitlari silip tekrar deneyin.\n\n${parts.join("\n")}`
      );
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru bagimliliklari kontrol edilirken hata olustu.";
      toast.error(message);
    } finally {
      setDeleteGuardLoading(false);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection
        title="Sorular"
        description="Tum sorulari arayin, filtreleyin ve yetkinize gore yonetin."
        actions={
          <Button onClick={() => navigate("/questions/create")} className="gap-2">
            <Plus className="h-4 w-4" />
            Yeni Soru Olustur
          </Button>
        }
      />

      <div className="grid gap-3 md:grid-cols-4">
        <Input placeholder="Soru metnine gore ara" value={search} onChange={(event) => setSearch(event.target.value)} />
        <select
          className="h-10 rounded-lg border border-stone-300 bg-white/90 px-3 text-sm dark:border-stone-700 dark:bg-stone-900/80"
          value={categoryFilter}
          onChange={(event) => {
            const value = event.target.value;
            setCategoryFilter(value === "all" ? "all" : value);
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
          value={creatorFilter}
          onChange={(event) => setCreatorFilter(event.target.value as "all" | "mine" | "others")}
        >
          <option value="all">Tum olusturanlar</option>
          <option value="mine">Sadece benim sorularim</option>
          <option value="others">Sadece digerlerinin sorulari</option>
        </select>
      </div>

      {loading ? <LoadingState message="Sorular yukleniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchQuestions} /> : null}

      {!loading && !error && filteredQuestions.length === 0 ? <EmptyState message="Filtrelere uygun soru bulunamadi." /> : null}

      {!loading && !error && filteredQuestions.length > 0 ? (
        <div className="overflow-x-auto rounded-2xl border border-stone-200/80 bg-white/85 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
          <table className="min-w-full divide-y divide-stone-200 dark:divide-stone-800">
            <thead>
              <tr className="text-left text-xs uppercase tracking-wide text-stone-500 dark:text-stone-400">
                <th className="px-4 py-3">Soru Metni</th>
                <th className="px-4 py-3">Kategori</th>
                <th className="px-4 py-3">Zorluk</th>
                <th className="px-4 py-3">Secenek</th>
                <th className="px-4 py-3">Olusturan</th>
                <th className="px-4 py-3 text-right">Islemler</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-stone-200 dark:divide-stone-800">
              {filteredQuestions.map((question) => {
                const canManage = canManageQuestion(question);
                return (
                  <tr
                    key={question.id}
                    className="text-sm text-stone-700 transition-colors hover:bg-stone-50/80 dark:text-stone-200 dark:hover:bg-stone-800/40"
                  >
                    <td className="max-w-sm px-4 py-3">{question.text}</td>
                    <td className="px-4 py-3">{getCategoryName(question)}</td>
                    <td className="px-4 py-3">{difficultyText[question.difficultyLevel] || "-"}</td>
                    <td className="px-4 py-3">{question.options?.length || "-"}</td>
                    <td className="px-4 py-3">{question.createdByUserFullName || question.createdByUserName || "-"}</td>
                    <td className="px-4 py-3">
                      <div className="flex justify-end gap-2">
                         <Button variant="outline" onClick={() => handlePreview(question.id)} className="gap-2">
                          <Eye className="h-4 w-4" />
                          Onizle
                        </Button>
                        {canManage ? (
                          <>
                            <Button variant="outline" onClick={() => navigate(`/questions/${question.id}/edit`)} className="gap-2">
                              <Pencil className="h-4 w-4" />
                              Duzenle
                            </Button>
                            <Button
                              variant="danger"
                              onClick={() => checkQuestionUsageBeforeDelete(question)}
                              disabled={deletingId === question.id}
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

      {previewQuestion ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4">
          <div className="hq-fade-in w-full max-w-2xl rounded-2xl border border-stone-200 bg-white/95 p-6 shadow-2xl dark:border-stone-700 dark:bg-stone-900/90">
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-lg font-semibold text-stone-900 dark:text-stone-100">Soru Onizleme</h2>
              <Button variant="ghost" onClick={() => setPreviewQuestion(null)} aria-label="Kapat">
                <X className="h-4 w-4" />
              </Button>
            </div>

            {previewLoading ? <p className="text-sm text-stone-600 dark:text-stone-300">Detay yukleniyor...</p> : null}

            {!previewLoading ? (
              <div className="space-y-4">
                <p className="font-medium text-stone-900 dark:text-stone-100">{previewQuestion.text}</p>
                <div className="space-y-2">
                  {previewQuestion.options?.map((option, index) => (
                    <div
                      key={`${option.text}-${index}`}
                      className={`rounded-md border px-3 py-2 text-sm ${
                        option.isCorrect
                          ? "border-emerald-300 bg-emerald-50 text-emerald-800 dark:border-emerald-800 dark:bg-emerald-950/40 dark:text-emerald-300"
                          : "border-stone-200 text-stone-700 dark:border-stone-700 dark:text-stone-200"
                      }`}
                    >
                      {String.fromCharCode(65 + index)}) {option.text}
                      {option.isCorrect ? " (Dogru cevap)" : ""}
                    </div>
                  ))}
                </div>
                <p className="text-sm text-stone-600 dark:text-stone-300">
                  Kategori: {getCategoryName(previewQuestion)} | Zorluk: {difficultyText[previewQuestion.difficultyLevel] || "-"}
                </p>
              </div>
            ) : null}
          </div>
        </div>
      ) : null}

      <ConfirmDialog
        open={Boolean(pendingDeleteQuestion)}
        title="Soru Sil"
        description={
          pendingDeleteQuestion
            ? `Bu soruyu silmek istediginize emin misiniz?\n\n${pendingDeleteQuestion.text}`
            : ""
        }
        confirmLabel="Evet, Sil"
        loading={deletingId !== null}
        onCancel={() => setPendingDeleteQuestion(null)}
        onConfirm={() => {
          if (pendingDeleteQuestion) {
            handleDelete(pendingDeleteQuestion);
          }
        }}
      />

      <ConfirmDialog
        open={Boolean(deleteGuardMessage) || deleteGuardLoading}
        title="Soru Silme Kontrolu"
        description={
          deleteGuardLoading
            ? "Soru bagimliliklari kontrol ediliyor..."
            : deleteGuardMessage || ""
        }
        cancelLabel="Tamam"
        showConfirmButton={false}
        onCancel={() => {
          setDeleteGuardLoading(false);
          setDeleteGuardMessage(null);
        }}
        onConfirm={() => {
          setDeleteGuardLoading(false);
          setDeleteGuardMessage(null);
        }}
      />
    </div>
  );
}

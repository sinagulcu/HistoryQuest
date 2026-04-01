import axios from "axios";
import { ArrowLeft, Plus, Trash2 } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import { questionApi } from "@/api/question.api";
import { quizApi } from "@/api/quiz.api";
import ConfirmDialog from "@/components/shared/ConfirmDialog";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useAuth } from "@/hooks/useAuth";
import type { Category } from "@/types/category.types";
import type { Question } from "@/types/question.types";
import type { Quiz } from "@/types/quiz.types";

const difficultyText: Record<number, string> = {
  1: "Kolay",
  2: "Orta",
  3: "Zor",
};

export default function QuizDetailPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const quizId = id || "";
  const { user } = useAuth();
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [questions, setQuestions] = useState<Question[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [categoryFilter, setCategoryFilter] = useState<number | "all">("all");
  const [difficultyFilter, setDifficultyFilter] = useState<number | "all">("all");
  const [processingQuestionId, setProcessingQuestionId] = useState<string | null>(null);
  const [pendingRemoveQuestionId, setPendingRemoveQuestionId] = useState<string | null>(null);

  const fetchData = async () => {
    if (!quizId) {
      setError("Gecersiz quiz ID");
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const [quizResponse, questionResponse, categoryResponse] = await Promise.all([
        quizApi.getById(quizId),
        questionApi.getAll(),
        categoryApi.getAll(),
      ]);
      setQuiz(quizResponse.data);
      setQuestions(questionResponse.data);
      setCategories(categoryResponse.data);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Quiz detayi yuklenirken hata olustu.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [quizId]);

  const canManageQuiz = useMemo(() => {
    if (!quiz || !user) {
      return false;
    }
    if (user.role === "Admin") {
      return true;
    }
    return user.id === quiz.createdByUserId;
  }, [quiz, user]);

  const quizQuestionIds = useMemo(() => new Set(quiz?.questions?.map((question) => question.id) || []), [quiz?.questions]);

  const getCategoryNameById = (categoryId: number) => {
    return categories.find((category) => category.id === categoryId)?.name || `#${categoryId}`;
  };

  const filteredPoolQuestions = useMemo(() => {
    return questions.filter((question) => {
      const textMatch = question.text.toLowerCase().includes(search.toLowerCase());
      const categoryMatch = categoryFilter === "all" || question.categoryId === categoryFilter;
      const difficultyMatch = difficultyFilter === "all" || question.difficultyLevel === difficultyFilter;
      return textMatch && categoryMatch && difficultyMatch;
    });
  }, [categoryFilter, difficultyFilter, questions, search]);

  const handleAddQuestion = async (questionId: string) => {
    if (!canManageQuiz) {
      toast.error("Bu quize soru ekleme yetkiniz yok");
      return;
    }

    setProcessingQuestionId(questionId);
    try {
      await quizApi.addQuestion(quizId, questionId);
      toast.success("Soru quize eklendi");
      await fetchData();
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru quiz'e eklenemedi.";
      toast.error(message);
    } finally {
      setProcessingQuestionId(null);
    }
  };

  const handleRemoveQuestion = async (questionId: string) => {
    if (!canManageQuiz) {
      toast.error("Bu quizden soru cikarma yetkiniz yok");
      return;
    }

    setProcessingQuestionId(questionId);
    try {
      await quizApi.removeQuestion(quizId, questionId);
      toast.success("Soru quizden cikarildi");
      setPendingRemoveQuestionId(null);
      await fetchData();
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru quizden cikarilamadi.";
      toast.error(message);
    } finally {
      setProcessingQuestionId(null);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection
        title={`Quiz Detay #${id}`}
        description="Quiz icindeki sorulari yonetin ve soru havuzundan yeni sorular ekleyin."
        actions={
          <Button variant="outline" onClick={() => navigate("/quizzes")} className="gap-2">
            <ArrowLeft className="h-4 w-4" />
            Quiz Listesine Don
          </Button>
        }
      />

      {loading ? <LoadingState message="Quiz detayi yukleniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchData} /> : null}

      {!loading && !error && quiz ? (
        <>
          <div className="grid gap-3 rounded-lg border border-stone-200 bg-white p-4 text-sm dark:border-stone-800 dark:bg-stone-900 md:grid-cols-3">
            <div>
              <p className="text-xs uppercase text-stone-500">Baslik</p>
              <p className="font-semibold text-stone-900 dark:text-stone-100">{quiz.title}</p>
            </div>
            <div>
              <p className="text-xs uppercase text-stone-500">Kategori</p>
              <p className="font-semibold text-stone-900 dark:text-stone-100">{quiz.categoryName || getCategoryNameById(quiz.categoryId)}</p>
            </div>
            <div>
              <p className="text-xs uppercase text-stone-500">Durum</p>
              <p className="font-semibold text-stone-900 dark:text-stone-100">{quiz.isPublished ? "Yayinda" : "Taslak"}</p>
            </div>
            <div>
              <p className="text-xs uppercase text-stone-500">Zorluk</p>
              <p className="font-semibold text-stone-900 dark:text-stone-100">{difficultyText[quiz.difficultyLevel] || "-"}</p>
            </div>
            <div>
              <p className="text-xs uppercase text-stone-500">Sure Limiti</p>
              <p className="font-semibold text-stone-900 dark:text-stone-100">{quiz.timeLimitMinutes} dakika</p>
            </div>
            <div>
              <p className="text-xs uppercase text-stone-500">Olusturan</p>
              <p className="font-semibold text-stone-900 dark:text-stone-100">{quiz.createdByUserName || quiz.createdByUserId}</p>
            </div>
            <div className="md:col-span-3">
              <p className="text-xs uppercase text-stone-500">Aciklama</p>
              <p className="text-stone-700 dark:text-stone-200">{quiz.description}</p>
            </div>
          </div>

          <div className="grid gap-4 lg:grid-cols-2">
            <section className="space-y-3 rounded-lg border border-stone-200 bg-white p-4 dark:border-stone-800 dark:bg-stone-900">
              <h2 className="text-sm font-semibold uppercase tracking-wide text-stone-600 dark:text-stone-300">Mevcut Sorular</h2>
              <p className="text-xs text-stone-500 dark:text-stone-400">Toplam: {quiz.questions?.length || 0} soru</p>

              {(quiz.questions?.length || 0) === 0 ? (
                <p className="rounded-md border border-dashed border-stone-300 p-4 text-sm text-stone-600 dark:border-stone-700 dark:text-stone-300">
                  Bu quizde henuz soru yok.
                </p>
              ) : null}

              <div className="space-y-2">
                {quiz.questions?.map((question) => (
                  <div
                    key={question.id}
                    className="flex items-start justify-between gap-3 rounded-md border border-stone-200 p-3 text-sm dark:border-stone-700"
                  >
                    <div>
                      <p className="font-medium text-stone-900 dark:text-stone-100">{question.text}</p>
                      <p className="text-xs text-stone-500 dark:text-stone-400">
                        {question.categoryName || getCategoryNameById(question.categoryId)} - {difficultyText[question.difficultyLevel] || "-"}
                      </p>
                    </div>
                    {canManageQuiz ? (
                      <Button
                        variant="danger"
                        className="gap-2"
                        onClick={() => setPendingRemoveQuestionId(question.id)}
                        disabled={processingQuestionId === question.id}
                      >
                        <Trash2 className="h-4 w-4" />
                        Cikar
                      </Button>
                    ) : null}
                  </div>
                ))}
              </div>
            </section>

            <section className="space-y-3 rounded-lg border border-stone-200 bg-white p-4 dark:border-stone-800 dark:bg-stone-900">
              <h2 className="text-sm font-semibold uppercase tracking-wide text-stone-600 dark:text-stone-300">Tum Sorular</h2>

              <div className="grid gap-2 md:grid-cols-3">
                <Input placeholder="Soru ara" value={search} onChange={(event) => setSearch(event.target.value)} className="md:col-span-3" />
                <select
                  className="h-10 rounded-md border border-stone-300 bg-white px-3 text-sm dark:border-stone-700 dark:bg-stone-900"
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
                  className="h-10 rounded-md border border-stone-300 bg-white px-3 text-sm dark:border-stone-700 dark:bg-stone-900"
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
              </div>

              {filteredPoolQuestions.length === 0 ? (
                <p className="rounded-md border border-dashed border-stone-300 p-4 text-sm text-stone-600 dark:border-stone-700 dark:text-stone-300">
                  Filtrelere uygun soru bulunamadi.
                </p>
              ) : null}

              <div className="space-y-2">
                {filteredPoolQuestions.map((question) => {
                  const alreadyAdded = quizQuestionIds.has(question.id);
                  return (
                    <div
                      key={question.id}
                      className="flex items-start justify-between gap-3 rounded-md border border-stone-200 p-3 text-sm dark:border-stone-700"
                    >
                      <div>
                        <p className="font-medium text-stone-900 dark:text-stone-100">{question.text}</p>
                        <p className="text-xs text-stone-500 dark:text-stone-400">
                          {question.categoryName || getCategoryNameById(question.categoryId)} - {difficultyText[question.difficultyLevel] || "-"}
                        </p>
                      </div>
                      {alreadyAdded ? (
                        <span className="rounded-full bg-emerald-100 px-2 py-1 text-xs font-semibold text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-300">
                          Eklendi
                        </span>
                      ) : (
                        <Button
                          className="gap-2"
                          onClick={() => handleAddQuestion(question.id)}
                          disabled={!canManageQuiz || processingQuestionId === question.id}
                        >
                          <Plus className="h-4 w-4" />
                          Ekle
                        </Button>
                      )}
                    </div>
                  );
                })}
              </div>
            </section>
          </div>
        </>
      ) : null}

      <ConfirmDialog
        open={pendingRemoveQuestionId !== null}
        title="Soru Cikar"
        description="Bu soruyu quizden cikarmak istediginize emin misiniz?"
        confirmLabel="Evet, Cikar"
        loading={processingQuestionId !== null}
        onCancel={() => setPendingRemoveQuestionId(null)}
        onConfirm={() => {
          if (pendingRemoveQuestionId !== null) {
            handleRemoveQuestion(pendingRemoveQuestionId);
          }
        }}
      />
    </div>
  );
}

import axios from "axios";
import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import { quizApi } from "@/api/quiz.api";
import QuizForm from "@/components/quiz/QuizForm";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { useAuth } from "@/hooks/useAuth";
import type { Category } from "@/types/category.types";
import type { Quiz } from "@/types/quiz.types";
import type { QuizFormValues } from "@/utils/validators";

export default function QuizEditPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const quizId = id || "";
  const { user } = useAuth();
  const [categories, setCategories] = useState<Category[]>([]);
  const [quiz, setQuiz] = useState<Quiz | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canEdit = useMemo(() => {
    if (!user || !quiz) {
      return false;
    }
    if (user.role === "Admin") {
      return true;
    }
    return user.id === quiz.createdByUserId;
  }, [quiz, user]);

  const fetchData = async () => {
    if (!quizId) {
      setError("Gecersiz quiz ID");
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const [categoriesResponse, quizResponse] = await Promise.all([categoryApi.getAll(), quizApi.getById(quizId)]);
      setCategories(categoriesResponse.data);
      setQuiz(quizResponse.data);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Quiz bilgileri yuklenemedi.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [quizId]);

  useEffect(() => {
    if (!loading && quiz && !canEdit) {
      toast.error("Bu quizi duzenleme yetkiniz yok");
      navigate("/quizzes", { replace: true });
    }
  }, [canEdit, loading, navigate, quiz]);

  const initialValues = useMemo<QuizFormValues | undefined>(() => {
    if (!quiz) {
      return undefined;
    }

    return {
      title: quiz.title,
      description: quiz.description,
      categoryId: quiz.categoryId,
      difficultyLevel: quiz.difficultyLevel,
      timeLimitMinutes: quiz.timeLimitMinutes,
      isPublished: quiz.isPublished,
    };
  }, [quiz]);

  const handleSubmit = async (values: QuizFormValues) => {
    setSaving(true);
    try {
      await quizApi.update(quizId, values);
      toast.success("Quiz guncellendi");
      navigate("/quizzes");
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Quiz guncellenirken hata olustu.";
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection title={quiz ? `${quiz.title} Duzenle` : "Quiz Duzenle"} description="Quiz bilgilerini guncelleyin." />

      {loading ? <LoadingState message="Quiz yukleniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchData} /> : null}

      {!loading && !error && quiz && canEdit && initialValues ? (
        <QuizForm
          categories={categories}
          initialValues={initialValues}
          isSubmitting={saving}
          submitLabel="Degisiklikleri Kaydet"
          onSubmit={handleSubmit}
          onCancel={() => navigate("/quizzes")}
        />
      ) : null}
    </div>
  );
}

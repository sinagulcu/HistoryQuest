import axios from "axios";
import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import { questionApi } from "@/api/question.api";
import QuestionForm from "@/components/question/QuestionForm";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { useAuth } from "@/hooks/useAuth";
import type { Category } from "@/types/category.types";
import type { Question } from "@/types/question.types";
import type { QuestionFormValues } from "@/utils/validators";

export default function QuestionEditPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const questionId = id || "";
  const { user } = useAuth();
  const [categories, setCategories] = useState<Category[]>([]);
  const [question, setQuestion] = useState<Question | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canEdit = useMemo(() => {
    if (!user || !question) {
      return false;
    }
    if (user.role === "Admin") {
      return true;
    }
    return user.id === question.createdByUserId;
  }, [question, user]);

  const fetchInitialData = async () => {
    if (!questionId) {
      setError("Gecersiz soru ID");
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);
    try {
      const [categoriesResponse, questionResponse] = await Promise.all([
        categoryApi.getAll(),
        questionApi.getById(questionId),
      ]);
      setCategories(categoriesResponse.data);
      setQuestion(questionResponse.data);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru bilgileri yuklenemedi.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchInitialData();
  }, [questionId]);

  useEffect(() => {
    if (!loading && question && !canEdit) {
      toast.error("Bu soruyu duzenleme yetkiniz yok");
      navigate("/questions", { replace: true });
    }
  }, [canEdit, loading, navigate, question]);

  const initialValues = useMemo<QuestionFormValues | undefined>(() => {
    if (!question) {
      return undefined;
    }

    return {
      text: question.text,
      categoryId: question.categoryId,
      difficultyLevel: question.difficultyLevel,
      options:
        question.options?.map((option) => ({
          text: option.text,
          isCorrect: option.isCorrect,
        })) || [
          { text: "", isCorrect: false },
          { text: "", isCorrect: true },
        ],
    };
  }, [question]);

  const handleSubmit = async (values: QuestionFormValues) => {
    setSaving(true);
    try {
      await questionApi.update(questionId, values);
      toast.success("Soru guncellendi");
      navigate("/questions");
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru guncellenirken hata olustu.";
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection title={`Soru Duzenle #${id}`} description="Soru metni ve seceneklerini guncelleyin." />

      {loading ? <LoadingState message="Soru yukleniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchInitialData} /> : null}

      {!loading && !error && question && canEdit && initialValues ? (
        <QuestionForm
          categories={categories}
          initialValues={initialValues}
          isSubmitting={saving}
          submitLabel="Degisiklikleri Kaydet"
          onSubmit={handleSubmit}
          onCancel={() => navigate("/questions")}
        />
      ) : null}
    </div>
  );
}

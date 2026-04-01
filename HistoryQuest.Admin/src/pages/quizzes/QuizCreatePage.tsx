import axios from "axios";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import { quizApi } from "@/api/quiz.api";
import QuizForm from "@/components/quiz/QuizForm";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import type { Category } from "@/types/category.types";
import type { QuizFormValues } from "@/utils/validators";

export default function QuizCreatePage() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchCategories = async () => {
    setLoading(true);
    setError(null);
    try {
      const { data } = await categoryApi.getAll();
      setCategories(data);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Kategoriler yuklenemedi.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const handleSubmit = async (values: QuizFormValues) => {
    setSaving(true);
    try {
      const { data } = await quizApi.create(values);
      toast.success("Quiz olusturuldu");
      navigate(`/quizzes/${data.id}`);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Quiz olusturulurken hata olustu.";
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection title="Quiz Olustur" description="Yeni bir quiz ekleyin ve yayin durumunu belirleyin." />

      {loading ? <LoadingState message="Kategoriler yukleniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchCategories} /> : null}

      {!loading && !error ? (
        <QuizForm
          categories={categories}
          isSubmitting={saving}
          submitLabel="Kaydet"
          onSubmit={handleSubmit}
          onCancel={() => navigate("/quizzes")}
        />
      ) : null}
    </div>
  );
}

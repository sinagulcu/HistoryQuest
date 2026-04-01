import axios from "axios";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import { questionApi } from "@/api/question.api";
import QuestionForm from "@/components/question/QuestionForm";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import type { Category } from "@/types/category.types";
import type { QuestionFormValues } from "@/utils/validators";

export default function QuestionCreatePage() {
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
          : "Kategori verileri yuklenemedi.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const handleSubmit = async (values: QuestionFormValues) => {
    setSaving(true);
    try {
      await questionApi.create(values);
      toast.success("Soru basariyla olusturuldu");
      navigate("/questions");
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru olusturulurken hata olustu.";
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection title="Soru Olustur" description="Yeni soru ekleyin ve dogru secenegi belirleyin." />

      {loading ? <LoadingState message="Form hazirlaniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchCategories} /> : null}

      {!loading && !error ? (
        <QuestionForm
          categories={categories}
          isSubmitting={saving}
          submitLabel="Soruyu Kaydet"
          onSubmit={handleSubmit}
          onCancel={() => navigate("/questions")}
        />
      ) : null}
    </div>
  );
}

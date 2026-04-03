import axios from "axios";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import { challengeApi } from "@/api/challenge.api";
import { questionApi } from "@/api/question.api";
import ChallengeForm from "@/components/challenge/ChallengeForm";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import type { Category } from "@/types/category.types";
import type { Question } from "@/types/question.types";
import type { ChallengeFormValues, QuestionFormValues } from "@/utils/validators";

export default function ChallengeCreatePage() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState<Category[]>([]);
  const [questions, setQuestions] = useState<Question[]>([]);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [creatingQuestion, setCreatingQuestion] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchInitialData = async () => {
    setLoading(true);
    setError(null);
    try {
      const [categoriesResponse, questionsResponse] = await Promise.all([categoryApi.getAll(), questionApi.getAll()]);
      setCategories(categoriesResponse.data);
      setQuestions(questionsResponse.data);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Form verileri yüklenemedi.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchInitialData();
  }, []);

  const createQuestion = async (values: QuestionFormValues) => {
    setCreatingQuestion(true);
    try {
      const { data } = await questionApi.create(values);
      setQuestions((prev) => [data, ...prev]);
      toast.success("Yeni soru oluşturuldu ve havuza eklendi.");
      return data;
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru oluşturulamadı.";
      toast.error(message);
      return null;
    } finally {
      setCreatingQuestion(false);
    }
  };

  const handleSubmit = async (values: ChallengeFormValues) => {
    setSaving(true);
    try {
      await challengeApi.create({
        ...values,
      });
      toast.success("Süreli meydan okuma planlandı.");
      navigate("/challenges");
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Meydan okuma kaydedilemedi.";
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection
        title="Süreli Meydan Okuma Oluştur"
        description="Hazır soru seçin veya yeni soru yazın, tarih ve saat belirleyip tüm öğrencilere planlayın."
      />

      {loading ? <LoadingState message="Meydan okuma formu hazırlanıyor..." /> : null}
      {error ? <ErrorState message={error} onRetry={fetchInitialData} /> : null}

      {!loading && !error ? (
        <ChallengeForm
          categories={categories}
          questions={questions}
          isSubmitting={saving}
          isCreatingQuestion={creatingQuestion}
          onSubmit={handleSubmit}
          onCancel={() => navigate("/challenges")}
          onCreateQuestion={createQuestion}
        />
      ) : null}
    </div>
  );
}

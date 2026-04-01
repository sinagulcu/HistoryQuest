import axios from "axios";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import { challengeApi } from "@/api/challenge.api";
import { questionApi } from "@/api/question.api";
import ChallengeForm, { toDateTimeLocalValue } from "@/components/challenge/ChallengeForm";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { useAuth } from "@/hooks/useAuth";
import type { Category } from "@/types/category.types";
import type { Challenge } from "@/types/challenge.types";
import type { Question } from "@/types/question.types";
import type { ChallengeFormValues, QuestionFormValues } from "@/utils/validators";

export default function ChallengeEditPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const [categories, setCategories] = useState<Category[]>([]);
  const [questions, setQuestions] = useState<Question[]>([]);
  const [challenge, setChallenge] = useState<Challenge | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [creatingQuestion, setCreatingQuestion] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const challengeId = id || "";

  const fetchInitialData = async () => {
    if (!challengeId) {
      setError("Gecersiz kayit kimligi.");
      setLoading(false);
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const [categoriesResponse, questionsResponse, challengeResponse] = await Promise.all([
        categoryApi.getAll(),
        questionApi.getAll(),
        challengeApi.getById(challengeId),
      ]);

      const challengeData = challengeResponse.data;
      if (user?.role === "Teacher" && challengeData.createdByUserId !== user.id) {
        toast.error("Bu kaydi duzenleme yetkiniz yok");
        navigate("/challenges", { replace: true });
        return;
      }

      setCategories(categoriesResponse.data);
      setQuestions(questionsResponse.data);
      setChallenge(challengeData);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Kayit verileri yuklenemedi.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchInitialData();
  }, [challengeId, navigate, user?.id, user?.role]);

  const createQuestion = async (values: QuestionFormValues) => {
    setCreatingQuestion(true);
    try {
      const { data } = await questionApi.create(values);
      setQuestions((prev) => [data, ...prev]);
      toast.success("Yeni soru olusturuldu ve havuza eklendi");
      return data;
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Soru olusturulamadi.";
      toast.error(message);
      return null;
    } finally {
      setCreatingQuestion(false);
    }
  };

  const handleSubmit = async (values: ChallengeFormValues) => {
    if (!challengeId) {
      toast.error("Gecersiz kayit kimligi.");
      return;
    }

    setSaving(true);
    try {
      await challengeApi.update(challengeId, {
        ...values,
        scheduledAt: new Date(values.scheduledAt).toISOString(),
      });
      toast.success("Meydan okuma guncellendi");
      navigate("/challenges");
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Meydan okuma guncellenemedi.";
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  const initialValues: ChallengeFormValues | undefined = challenge
    ? {
        title: challenge.title,
        questionId: challenge.questionId,
        scheduledAt: toDateTimeLocalValue(challenge.scheduledAt),
        scoringDurationMinutes: challenge.scoringDurationMinutes,
        lateDurationMinutes: challenge.lateDurationMinutes,
        maxScore: challenge.maxScore,
        showCorrectAnswerOnWrong: challenge.showCorrectAnswerOnWrong,
        showExplanationOnWrong: challenge.showExplanationOnWrong,
        explanation: challenge.explanation || "",
        notifyAllStudents: challenge.notifyAllStudents,
      }
    : undefined;

  return (
    <div className="space-y-6">
      <PageSection
        title="Sureli Meydan Okuma Duzenle"
        description="Planlanan tarih, sure ve puanlama kurallarini guncelleyebilirsiniz."
      />

      {loading ? <LoadingState message="Kayit verileri yukleniyor..." /> : null}
      {error ? <ErrorState message={error} onRetry={fetchInitialData} /> : null}

      {!loading && !error && challenge && initialValues ? (
        <ChallengeForm
          categories={categories}
          questions={questions}
          initialValues={initialValues}
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

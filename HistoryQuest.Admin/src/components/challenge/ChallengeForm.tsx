import { zodResolver } from "@hookform/resolvers/zod";
import { Plus } from "lucide-react";
import { useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import QuestionForm from "@/components/question/QuestionForm";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Label from "@/components/ui/label";
import type { Category } from "@/types/category.types";
import type { Question } from "@/types/question.types";
import { toLocalDateTimeInputValue } from "@/utils/dateTime";
import type { ChallengeFormValues, QuestionFormValues } from "@/utils/validators";
import { challengeSchema } from "@/utils/validators";

interface ChallengeFormProps {
  categories: Category[];
  questions: Question[];
  initialValues?: ChallengeFormValues;
  isSubmitting: boolean;
  isCreatingQuestion: boolean;
  onSubmit: (values: ChallengeFormValues) => Promise<void>;
  onCancel: () => void;
  onCreateQuestion: (values: QuestionFormValues) => Promise<Question | null>;
}

function toDateTimeLocalValue(isoDate: string) {
  return toLocalDateTimeInputValue(isoDate);
}

export default function ChallengeForm({
  categories,
  questions,
  initialValues,
  isSubmitting,
  isCreatingQuestion,
  onSubmit,
  onCancel,
  onCreateQuestion,
}: ChallengeFormProps) {
  const [questionMode, setQuestionMode] = useState<"existing" | "new">("existing");
  const [questionModalOpen, setQuestionModalOpen] = useState(false);

  const defaultValues = useMemo<ChallengeFormValues>(
    () =>
      initialValues || {
        title: "",
        questionId: "",
        scheduledAt: "",
        scoringDurationMinutes: 10,
        lateDurationMinutes: 10,
        maxScore: 100,
        showCorrectAnswerOnWrong: true,
        showExplanationOnWrong: true,
        explanation: "",
        notifyAllStudents: true,
      },
    [initialValues]
  );

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<ChallengeFormValues>({
    resolver: zodResolver(challengeSchema),
    defaultValues,
  });

  const selectedQuestionId = watch("questionId");
  const selectedQuestion = questions.find((item) => item.id === selectedQuestionId);

  const handleFormSubmit = async (values: ChallengeFormValues) => {
    await onSubmit(values);
  };

  const handleCreateQuestion = async (values: QuestionFormValues) => {
    const createdQuestion = await onCreateQuestion(values);
    if (createdQuestion) {
      setValue("questionId", createdQuestion.id, { shouldValidate: true });
      setQuestionMode("existing");
      setQuestionModalOpen(false);
    }
  };

  return (
    <>
      <form onSubmit={handleSubmit(handleFormSubmit)} className="space-y-6">
        <div className="space-y-2">
          <Label htmlFor="title">Meydan Okuma Basligi</Label>
          <Input id="title" placeholder="Ornek: Haftanin hiz sorusu" {...register("title")} />
          {errors.title ? <p className="text-xs text-red-600">{errors.title.message}</p> : null}
        </div>

        <section className="space-y-4 rounded-md border border-stone-200 p-4 dark:border-stone-800">
          <div className="flex items-center justify-between gap-2">
            <h2 className="text-sm font-semibold text-stone-900 dark:text-stone-100">Soru Secimi</h2>
            <Button type="button" variant="outline" onClick={() => setQuestionModalOpen(true)} className="gap-2">
              <Plus className="h-4 w-4" />
              Soru Ekle
            </Button>
          </div>

          <div className="flex flex-wrap gap-4 text-sm">
            <label className="inline-flex items-center gap-2 text-stone-700 dark:text-stone-200">
              <input
                type="radio"
                checked={questionMode === "existing"}
                onChange={() => setQuestionMode("existing")}
              />
              Hazir sorularimdan sec
            </label>
            <label className="inline-flex items-center gap-2 text-stone-700 dark:text-stone-200">
              <input type="radio" checked={questionMode === "new"} onChange={() => setQuestionMode("new")} />
              Yeni soru yaz ve ekle
            </label>
          </div>

          {questionMode === "existing" ? (
            <div className="space-y-2">
              <Label htmlFor="questionId">Soru</Label>
              <select
                id="questionId"
                className="h-10 w-full rounded-md border border-stone-300 bg-white px-3 text-sm outline-none ring-[#b99647] focus:ring-2 dark:border-stone-700 dark:bg-stone-900"
                {...register("questionId")}
              >
                <option value="">Soru seciniz</option>
                {questions.map((question) => (
                  <option key={question.id} value={question.id}>
                    #{question.id} - {question.text}
                  </option>
                ))}
              </select>
              {errors.questionId ? <p className="text-xs text-red-600">{errors.questionId.message}</p> : null}
            </div>
          ) : (
            <div className="rounded-md border border-dashed border-stone-300 p-3 text-sm text-stone-600 dark:border-stone-700 dark:text-stone-300">
              Yeni soru eklemek icin "Soru Ekle" butonunu kullanin. Kaydedilen soru otomatik olarak sorulariniza da eklenir.
            </div>
          )}

          {selectedQuestion ? (
            <div className="rounded-md border border-[#d8c08a]/50 bg-[#f8f2e5] p-3 text-sm text-stone-700 dark:border-[#786436] dark:bg-[#2b2315] dark:text-stone-200">
              Secili soru: {selectedQuestion.text}
            </div>
          ) : null}
        </section>

        <div className="grid gap-4 md:grid-cols-2">
          <div className="space-y-2">
            <Label htmlFor="scheduledAt">Planlanan Baslangic</Label>
            <Input id="scheduledAt" type="datetime-local" {...register("scheduledAt")} />
            {errors.scheduledAt ? <p className="text-xs text-red-600">{errors.scheduledAt.message}</p> : null}
          </div>
          <div className="space-y-2">
            <Label htmlFor="maxScore">Maksimum Puan</Label>
            <Input id="maxScore" type="number" min={1} {...register("maxScore", { valueAsNumber: true })} />
            {errors.maxScore ? <p className="text-xs text-red-600">{errors.maxScore.message}</p> : null}
          </div>
          <div className="space-y-2">
            <Label htmlFor="scoringDurationMinutes">Puanli Sure (dakika)</Label>
            <Input
              id="scoringDurationMinutes"
              type="number"
              min={1}
              {...register("scoringDurationMinutes", { valueAsNumber: true })}
            />
            {errors.scoringDurationMinutes ? (
              <p className="text-xs text-red-600">{errors.scoringDurationMinutes.message}</p>
            ) : null}
          </div>
          <div className="space-y-2">
            <Label htmlFor="lateDurationMinutes">Puan Disi Ek Sure (dakika)</Label>
            <Input
              id="lateDurationMinutes"
              type="number"
              min={1}
              {...register("lateDurationMinutes", { valueAsNumber: true })}
            />
            {errors.lateDurationMinutes ? <p className="text-xs text-red-600">{errors.lateDurationMinutes.message}</p> : null}
          </div>
        </div>

        <div className="space-y-2">
          <Label htmlFor="explanation">Soru Aciklamasi (yanlis cevapta gosterilir)</Label>
          <textarea
            id="explanation"
            rows={4}
            className="w-full rounded-md border border-stone-300 bg-white px-3 py-2 text-sm outline-none ring-[#b99647] focus:ring-2 dark:border-stone-700 dark:bg-stone-900"
            placeholder="Ogrencinin yanlis cevap sonrasi gorecegi aciklama"
            {...register("explanation")}
          />
          {errors.explanation ? <p className="text-xs text-red-600">{errors.explanation.message}</p> : null}
        </div>

        <div className="grid gap-3 md:grid-cols-3">
          <label className="inline-flex items-center gap-2 rounded-md border border-stone-200 p-3 text-sm dark:border-stone-800">
            <input type="checkbox" {...register("showCorrectAnswerOnWrong")} />
            Yanlis cevapta dogru sikki goster
          </label>
          <label className="inline-flex items-center gap-2 rounded-md border border-stone-200 p-3 text-sm dark:border-stone-800">
            <input type="checkbox" {...register("showExplanationOnWrong")} />
            Yanlis cevapta aciklamayi goster
          </label>
          <label className="inline-flex items-center gap-2 rounded-md border border-stone-200 p-3 text-sm dark:border-stone-800">
            <input type="checkbox" {...register("notifyAllStudents")} />
            Planlanan saatte tum ogrencilere bildirim gonder
          </label>
        </div>

        <div className="flex gap-2">
          <Button type="submit" disabled={isSubmitting}>
            {isSubmitting ? "Kaydediliyor..." : "Meydan Okumayi Kaydet"}
          </Button>
          <Button type="button" variant="outline" onClick={onCancel}>
            Iptal
          </Button>
        </div>
      </form>

      {questionModalOpen ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4 py-6">
          <div className="max-h-full w-full max-w-3xl overflow-y-auto rounded-lg bg-white p-5 dark:bg-stone-900">
            <h3 className="mb-4 text-lg font-semibold text-stone-900 dark:text-stone-100">Yeni Soru Ekle</h3>
            <QuestionForm
              categories={categories}
              isSubmitting={isCreatingQuestion}
              submitLabel="Soruyu Olustur"
              onSubmit={handleCreateQuestion}
              onCancel={() => setQuestionModalOpen(false)}
            />
          </div>
        </div>
      ) : null}
    </>
  );
}

export { toDateTimeLocalValue };

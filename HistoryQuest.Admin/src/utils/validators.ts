import { z } from "zod";

export const loginSchema = z.object({
  identifier: z.string().min(3, "E-posta veya kullanici adi giriniz"),
  password: z.string().min(6, "Şifre en az 6 karakter olmalı"),
});

export type LoginFormValues = z.infer<typeof loginSchema>;

export const categorySchema = z.object({
  name: z.string().min(2, "Kategori adı en az 2 karakter olmalı"),
  description: z.string().min(3, "Açıklama en az 3 karakter olmalı"),
});

export type CategoryFormValues = z.infer<typeof categorySchema>;

export const questionOptionSchema = z.object({
  text: z.string().min(1, "Seçenek metni boş bırakılamaz"),
  isCorrect: z.boolean(),
});

export const questionSchema = z
  .object({
    text: z.string().min(10, "Soru metni en az 10 karakter olmalı"),
    categoryId: z.number().positive("Kategori seçiniz"),
    difficultyLevel: z.number().min(1).max(3),
    options: z.array(questionOptionSchema).min(2, "En az 2 seçenek olmalı").max(6, "En fazla 6 seçenek olabilir"),
  })
  .refine((value) => value.options.filter((option) => option.isCorrect).length === 1, {
    message: "Tam olarak 1 doğru cevap seçilmelidir",
    path: ["options"],
  });

export type QuestionFormValues = z.infer<typeof questionSchema>;

export const quizSchema = z.object({
  title: z.string().min(3, "Baslik en az 3 karakter olmali"),
  description: z.string().min(10, "Aciklama en az 10 karakter olmali"),
  categoryId: z.number().positive("Kategori seciniz"),
  difficultyLevel: z.number().min(1).max(3),
  timeLimitMinutes: z.number().min(1, "Sure en az 1 dakika olmali"),
  isPublished: z.boolean(),
});

export type QuizFormValues = z.infer<typeof quizSchema>;

export const challengeSchema = z.object({
  title: z.string().min(3, "Baslik en az 3 karakter olmali"),
  questionId: z.string().min(1, "Bir soru seciniz"),
  scheduledAt: z.string().min(1, "Planlanan tarih ve saat zorunludur"),
  scoringDurationMinutes: z.number().min(1, "Puanli sure en az 1 dakika olmali"),
  lateDurationMinutes: z.number().min(1, "Ek sure en az 1 dakika olmali"),
  maxScore: z.number().min(1, "Maksimum puan en az 1 olmali"),
  showCorrectAnswerOnWrong: z.boolean(),
  showExplanationOnWrong: z.boolean(),
  explanation: z.string().max(1000, "Aciklama 1000 karakterden uzun olamaz").optional(),
  notifyAllStudents: z.boolean(),
});

export type ChallengeFormValues = z.infer<typeof challengeSchema>;
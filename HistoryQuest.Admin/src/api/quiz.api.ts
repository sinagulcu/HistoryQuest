import api from "./axios";
import type { Quiz, QuizCreateDto, QuizUpdateDto } from "@/types/quiz.types";
import { normalizeServerDateString } from "@/utils/dateTime";
import { toDifficultyLevel, unwrapApiData } from "./apiUtils";

const normalizeQuiz = (raw: unknown): Quiz => {
  const item = unwrapApiData<Record<string, unknown>>(raw);
  const questionsRaw = Array.isArray(item.questions) ? item.questions : [];

  return {
    id: String(item.id ?? item.quizId ?? ""),
    title: String(item.title ?? ""),
    description: String(item.description ?? ""),
    categoryId: typeof item.categoryId === "number" ? item.categoryId : 0,
    categoryName: typeof item.categoryName === "string" ? item.categoryName : undefined,
    difficultyLevel: toDifficultyLevel(item.difficulty ?? item.difficultyLevel),
    timeLimitMinutes: typeof item.timeLimitMinutes === "number" ? item.timeLimitMinutes : 0,
    isPublished: Boolean(item.isPublished),
    createdByUserId: String(item.createdByUserId ?? item.createdByTeacherId ?? ""),
    createdByUserName:
      typeof item.createdByUserName === "string"
        ? item.createdByUserName
        : typeof item.createdByTeacherName === "string"
          ? item.createdByTeacherName
          : undefined,
    createdAt: normalizeServerDateString(typeof item.createdAt === "string" ? item.createdAt : undefined),
    questions: questionsRaw.map((question) => {
      const q = unwrapApiData<Record<string, unknown>>(question);
      return {
        id: String(q.id ?? q.questionId ?? ""),
        text: String(q.text ?? ""),
        categoryId: typeof q.categoryId === "number" ? q.categoryId : 0,
        categoryName: typeof q.categoryName === "string" ? q.categoryName : undefined,
        difficultyLevel: toDifficultyLevel(q.difficulty ?? q.difficultyLevel),
        createdByUserId: String(q.createdByUserId ?? q.createdByTeacherId ?? ""),
        createdByUserName:
          typeof q.createdByUserName === "string"
            ? q.createdByUserName
            : typeof q.createdByTeacherName === "string"
              ? q.createdByTeacherName
              : undefined,
      };
    }),
  };
};

const normalizeQuizList = (raw: unknown): Quiz[] => {
  const list = unwrapApiData<unknown[]>(raw);
  if (!Array.isArray(list)) {
    return [];
  }

  return list.map((item) => normalizeQuiz(item));
};

const toQuizPayload = (data: QuizCreateDto | QuizUpdateDto) => ({
  title: data.title,
  description: data.description,
});

export const quizApi = {
  getAll: async () => {
    const response = await api.get("/Quiz/GetMyQuizzes");
    return {
      ...response,
      data: normalizeQuizList(response.data),
    };
  },
  getById: async (id: string) => {
    const response = await api.get(`/Quiz/QuizDetail/${id}`);
    return {
      ...response,
      data: normalizeQuiz(response.data),
    };
  },
  create: async (data: QuizCreateDto) => {
    const response = await api.post("/Quiz", toQuizPayload(data));
    return {
      ...response,
      data: normalizeQuiz(response.data),
    };
  },
  update: async (id: string, data: QuizUpdateDto) => {
    const response = await api.put(`/Quiz/${id}`, toQuizPayload(data));
    return {
      ...response,
      data: normalizeQuiz(response.data),
    };
  },
  delete: (id: string) => api.post(`/Quiz/${id}/delete`),
  publish: (id: string) => api.post(`/Quiz/${id}/publish`),
  unpublish: (id: string) => api.post(`/Quiz/${id}/revert`),
  addQuestion: (quizId: string, questionId: string) => api.post(`/Quiz/AddQuestionToQuiz/${quizId}/questions/${questionId}`),
  removeQuestion: (quizId: string, questionId: string) => api.delete(`/Quiz/${quizId}/questions/${questionId}`),
};
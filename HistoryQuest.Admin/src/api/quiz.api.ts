import axios from "axios";
import api from "./axios";
import type { Quiz, QuizCreateDto, QuizUpdateDto } from "@/types/quiz.types";
import { normalizeServerDateString } from "@/utils/dateTime";
import { toDifficultyLevel, unwrapApiData } from "./apiUtils";

const normalizeQuiz = (raw: unknown): Quiz => {
  const item = unwrapApiData<Record<string, unknown>>(raw);
  const questionsRaw = Array.isArray(item.questions)
    ? item.questions
    : Array.isArray(item.quizQuestions)
      ? item.quizQuestions
      : [];

  const difficultySource = item.difficulty ?? item.difficultyLevel ?? item.level;
  const statusSource = String(item.status ?? "").toLowerCase();
  const categoryValue =
    item.categoryId ?? item.CategoryId ?? item.categoryID ?? item.quizCategoryId ?? item.QuizCategoryId;
  const categoryObject =
    item.category && typeof item.category === "object"
      ? unwrapApiData<Record<string, unknown>>(item.category)
      : null;
  const createdByValue = item.createdByUserId ?? item.createdByTeacherId ?? item.teacherId ?? item.TeacherId;
  const timeLimitSource =
    item.timeLimitMinutes ?? item.TimeLimitMinutes ?? item.durationInMinutes ?? item.DurationInMinutes ?? item.timeLimit;

  return {
    id: String(item.id ?? item.quizId ?? item.Id ?? ""),
    title: String(item.title ?? item.Title ?? ""),
    description: String(item.description ?? item.Description ?? ""),
    categoryId: categoryValue != null ? String(categoryValue) : "",
    categoryName:
      typeof item.categoryName === "string"
        ? item.categoryName
        : categoryObject && typeof categoryObject.name === "string"
          ? categoryObject.name
        : typeof item.category === "string"
          ? item.category
          : typeof item.Category === "string"
            ? item.Category
        : typeof item.CategoryName === "string"
          ? item.CategoryName
          : typeof item.categoryTitle === "string"
            ? item.categoryTitle
          : undefined,
    difficultyLevel: toDifficultyLevel(difficultySource),
    timeLimitMinutes: typeof timeLimitSource === "number" ? timeLimitSource : Number(timeLimitSource ?? 0) || 0,
    isPublished:
      typeof item.isPublished === "boolean"
        ? item.isPublished
        : typeof item.IsPublished === "boolean"
          ? item.IsPublished
          : statusSource === "published",
    createdByUserId: createdByValue != null ? String(createdByValue) : "",
    createdByUserName:
      typeof item.createdByUserName === "string"
        ? item.createdByUserName
        : typeof item.createdByTeacherFullName === "string"
          ? item.createdByTeacherFullName
          : typeof item.createdByTeacherUserName === "string"
            ? item.createdByTeacherUserName
        : typeof item.createdByTeacherName === "string"
          ? item.createdByTeacherName
            : typeof item.createdByName === "string"
              ? item.createdByName
              : typeof item.teacherName === "string"
                ? item.teacherName
                : typeof item.userName === "string"
                  ? item.userName
                  : typeof item.createdBy === "string"
                    ? item.createdBy
          : undefined,
    createdAt: normalizeServerDateString(
      typeof item.createdAt === "string"
        ? item.createdAt
        : typeof item.CreatedAt === "string"
          ? item.CreatedAt
          : undefined
    ),
    questionCount:
      typeof item.questionCount === "number"
        ? item.questionCount
        : typeof item.QuestionCount === "number"
          ? item.QuestionCount
          : questionsRaw.length,
    questions: questionsRaw.map((question) => {
      const q = unwrapApiData<Record<string, unknown>>(question);
      return {
        id: String(q.id ?? q.questionId ?? q.QuestionId ?? ""),
        text: String(q.text ?? q.questionText ?? ""),
        categoryId: String(q.categoryId ?? ""),
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
  categoryId: data.categoryId,
  difficultyLevel: data.difficultyLevel,
  timeLimitMinutes: data.timeLimitMinutes,
  isPublished: data.isPublished,
});

export const quizApi = {
  getAll: async () => {
    // Backend contract uses GetMyQuizzes as the primary list endpoint.
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
    let response;
    try {
      response = await api.put(`/Quiz/update/${id}`, toQuizPayload(data));
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 404) {
        response = await api.put(`/Quiz/${id}`, toQuizPayload(data));
      } else {
        throw error;
      }
    }
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
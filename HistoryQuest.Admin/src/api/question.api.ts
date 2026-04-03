import api from "./axios";
import type { Question, QuestionCreateDto, QuestionUpdateDto } from "@/types/question.types";
import { normalizeServerDateString } from "@/utils/dateTime";
import { toDifficultyLevel, toDifficultyName, unwrapApiData } from "./apiUtils";

const normalizeQuestion = (raw: unknown): Question => {
  const item = unwrapApiData<Record<string, unknown>>(raw);

  const optionsRaw = Array.isArray(item.options) ? item.options : [];

  return {
    id: String(item.id ?? item.questionId ?? item.Id ?? ""),
    text: String(item.text ?? item.questionText ?? item.Text ?? ""),
    categoryId: String(item.categoryId ?? item.CategoryId ?? item.categoryID ?? ""),
    categoryName:
      typeof item.categoryName === "string"
        ? item.categoryName
        : typeof item.CategoryName === "string"
          ? item.CategoryName
          : item.category && typeof item.category === "object"
            ? String(unwrapApiData<Record<string, unknown>>(item.category).name ?? "")
            : typeof item.category === "string"
              ? item.category
          : undefined,
    difficultyLevel: toDifficultyLevel(item.difficulty ?? item.difficultyLevel),
    createdByUserId: String(item.createdByUserId ?? item.createdByTeacherId ?? ""),
    createdByUserName:
      typeof item.createdByUserName === "string"
        ? item.createdByUserName
        : typeof item.createdByTeacherUserName === "string"
          ? item.createdByTeacherUserName
        : typeof item.createdByTeacherName === "string"
          ? item.createdByTeacherName
          : undefined,
    createdByUserFullName:
      typeof item.createdByUserFullName === "string"
        ? item.createdByUserFullName
        : typeof item.createdByTeacherFullName === "string"
          ? item.createdByTeacherFullName
          : undefined,
    createdAt: normalizeServerDateString(
      typeof item.createdAt === "string"
        ? item.createdAt
        : typeof item.CreatedAt === "string"
          ? item.CreatedAt
          : typeof item.createdAtUtc === "string"
            ? item.createdAtUtc
            : typeof item.CreatedAtUtc === "string"
              ? item.CreatedAtUtc
              : typeof item.createdDate === "string"
                ? item.createdDate
                : typeof item.createdOn === "string"
                  ? item.createdOn
                  : typeof item.created === "string"
                    ? item.created
                    : undefined
    ),
    options: optionsRaw.map((option) => {
      const optionRecord = unwrapApiData<Record<string, unknown>>(option);
      return {
        id: optionRecord.id ? String(optionRecord.id) : undefined,
        text: String(optionRecord.text ?? ""),
        isCorrect: Boolean(optionRecord.isCorrect),
      };
    }),
  };
};

const normalizeQuestionList = (raw: unknown): Question[] => {
  const list = unwrapApiData<unknown[]>(raw);
  if (!Array.isArray(list)) {
    return [];
  }
  return list.map((item) => normalizeQuestion(item));
};

const toQuestionCreatePayload = (data: QuestionCreateDto) => ({
  text: data.text,
  categoryId: data.categoryId,
  // Backend farklı sözleşmelerde enum adı bekleyebiliyor.
  difficulty: toDifficultyName(data.difficultyLevel),
  difficultyLevel: data.difficultyLevel,
  options: data.options.map((option) => ({
    text: option.text,
    isCorrect: option.isCorrect,
  })),
});

const toQuestionUpdatePayload = (data: QuestionUpdateDto) => ({
  text: data.text,
  categoryId: data.categoryId,
  difficulty: toDifficultyName(data.difficultyLevel),
  difficultyLevel: data.difficultyLevel,
  explanation: "",
  options: data.options.map((option) => ({
    id: option.id,
    text: option.text,
    isCorrect: option.isCorrect,
  })),
});

export const questionApi = {
  getAll: async () => {
    // Backend now supports listing all questions for Teacher/Admin.
    const response = await api.get("/Questions");
    return {
      ...response,
      data: normalizeQuestionList(response.data),
    };
  },
  getMine: async () => {
    const response = await api.get("/Questions/getmyquestions");
    return {
      ...response,
      data: normalizeQuestionList(response.data),
    };
  },
  getById: async (id: string) => {
    const response = await api.get(`/Questions/get/${id}`);
    return {
      ...response,
      data: normalizeQuestion(response.data),
    };
  },
  create: async (data: QuestionCreateDto) => {
    const response = await api.post("/Questions", toQuestionCreatePayload(data));
    return {
      ...response,
      data: normalizeQuestion(response.data),
    };
  },
  update: async (id: string, data: QuestionUpdateDto) => {
    const response = await api.put(`/Questions/update/${id}`, toQuestionUpdatePayload(data));
    return {
      ...response,
      data: normalizeQuestion(response.data),
    };
  },
  delete: (id: string) => api.delete(`/Questions/delete/${id}`),
};
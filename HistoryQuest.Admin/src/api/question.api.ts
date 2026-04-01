import api from "./axios";
import type { Question, QuestionCreateDto, QuestionUpdateDto } from "@/types/question.types";
import { normalizeServerDateString } from "@/utils/dateTime";
import { toDifficultyLevel, toDifficultyName, unwrapApiData } from "./apiUtils";

const normalizeQuestion = (raw: unknown): Question => {
  const item = unwrapApiData<Record<string, unknown>>(raw);

  const optionsRaw = Array.isArray(item.options) ? item.options : [];

  return {
    id: String(item.id ?? item.questionId ?? ""),
    text: String(item.text ?? ""),
    categoryId: typeof item.categoryId === "number" ? item.categoryId : 0,
    categoryName: typeof item.categoryName === "string" ? item.categoryName : undefined,
    difficultyLevel: toDifficultyLevel(item.difficulty ?? item.difficultyLevel),
    createdByUserId: String(item.createdByUserId ?? item.createdByTeacherId ?? ""),
    createdByUserName:
      typeof item.createdByUserName === "string"
        ? item.createdByUserName
        : typeof item.createdByTeacherName === "string"
          ? item.createdByTeacherName
          : undefined,
    createdAt: normalizeServerDateString(typeof item.createdAt === "string" ? item.createdAt : undefined),
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
  options: data.options.map((option) => ({
    text: option.text,
    isCorrect: option.isCorrect,
  })),
});

const toQuestionUpdatePayload = (data: QuestionUpdateDto) => ({
  text: data.text,
  difficulty: toDifficultyName(data.difficultyLevel),
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
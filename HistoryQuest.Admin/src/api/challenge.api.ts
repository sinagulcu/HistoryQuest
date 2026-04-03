import api from "@/api/axios";
import type { Challenge, ChallengeCreateDto, ChallengeUpdateDto } from "@/types/challenge.types";
import { localDateTimeInputToUtcIso, normalizeServerDateString } from "@/utils/dateTime";
import { unwrapApiData } from "./apiUtils";

const toStatus = (value: unknown): Challenge["status"] => {
  const normalized = typeof value === "string" ? value.toLowerCase() : "";
  if (normalized === "active") return "Active";
  if (normalized === "expired" || normalized === "completed" || normalized === "cancelled") return "Expired";
  return "Scheduled";
};

const normalizeChallenge = (raw: unknown): Challenge => {
  const item = unwrapApiData<Record<string, unknown>>(raw);
  const scheduledAtRaw = String(item.scheduledAtUtc ?? item.scheduledAt ?? new Date().toISOString());
  const questionObject =
    item.question && typeof item.question === "object"
      ? unwrapApiData<Record<string, unknown>>(item.question)
      : null;
  const creatorObject =
    item.createdByTeacher && typeof item.createdByTeacher === "object"
      ? unwrapApiData<Record<string, unknown>>(item.createdByTeacher)
      : null;

  return {
    id: String(item.id ?? ""),
    title: String(item.title ?? ""),
    questionId: String(item.questionId ?? ""),
    questionText:
      typeof item.questionText === "string"
        ? item.questionText
        : typeof item.QuestionText === "string"
          ? item.QuestionText
          : questionObject && typeof questionObject.text === "string"
            ? questionObject.text
            : undefined,
    scheduledAtUtc: normalizeServerDateString(scheduledAtRaw) ?? new Date().toISOString(),
    answerWindowSeconds:
      typeof item.answerWindowSeconds === "number"
        ? item.answerWindowSeconds
        : Math.max(30, (typeof item.scoringDurationMinutes === "number" ? item.scoringDurationMinutes : 1) * 60),
    visibilityWindowSeconds:
      typeof item.visibilityWindowSeconds === "number"
        ? item.visibilityWindowSeconds
        : Math.max(30, (typeof item.lateDurationMinutes === "number" ? item.lateDurationMinutes : 1) * 60),
    maxScore: typeof item.maxScore === "number" ? item.maxScore : 100,
    showCorrectAnswerOnWrong: Boolean(item.showCorrectAnswerOnWrong),
    showExplanationOnWrong: Boolean(item.showExplanationOnWrong),
    notifyAllStudents: Boolean(item.notifyAllStudents),
    status: toStatus(item.status),
    createdByTeacherId: String(item.createdByTeacherId ?? item.createdByUserId ?? ""),
    createdByTeacherName:
      typeof item.createdByTeacherName === "string"
        ? item.createdByTeacherName
        : typeof item.createdByTeacherFullName === "string"
          ? item.createdByTeacherFullName
          : typeof item.createdByUserName === "string"
            ? item.createdByUserName
            : creatorObject && typeof creatorObject.fullName === "string"
              ? creatorObject.fullName
              : creatorObject && typeof creatorObject.userName === "string"
                ? creatorObject.userName
                : undefined,
    createdAt: normalizeServerDateString(typeof item.createdAt === "string" ? item.createdAt : undefined),
    updatedAt: normalizeServerDateString(typeof item.updatedAt === "string" ? item.updatedAt : undefined),
  };
};

const toChallengePayload = (data: ChallengeCreateDto | ChallengeUpdateDto) => ({
  title: data.title,
  questionId: data.questionId,
  scheduledAtUtc: localDateTimeInputToUtcIso(data.scheduledAt),
  answerWindowSeconds: Math.max(30, data.scoringDurationMinutes * 60),
  // Backend visibility window is total lifetime after publish, not just extra duration.
  visibilityWindowSeconds: Math.max(30, (data.scoringDurationMinutes + data.lateDurationMinutes) * 60),
  maxScore: data.maxScore,
  showCorrectAnswerOnWrong: data.showCorrectAnswerOnWrong,
  showExplanationOnWrong: data.showExplanationOnWrong,
  notifyAllStudents: data.notifyAllStudents,
});

export const challengeApi = {
  getAll: async () => {
    const response = await api.get("/Challenge");
    const list = unwrapApiData<unknown[]>(response.data);
    return {
      ...response,
      data: Array.isArray(list) ? list.map((item) => normalizeChallenge(item)) : [],
    };
  },
  getById: async (id: string) => {
    const response = await api.get(`/Challenge/${id}`);
    return {
      ...response,
      data: normalizeChallenge(response.data),
    };
  },
  create: async (data: ChallengeCreateDto) => {
    const response = await api.post("/Challenge", toChallengePayload(data));
    return {
      ...response,
      data: normalizeChallenge(response.data),
    };
  },
  update: async (id: string, data: ChallengeUpdateDto) => {
    const response = await api.put(`/Challenge/${id}`, toChallengePayload(data));
    return {
      ...response,
      data: normalizeChallenge(response.data),
    };
  },
  delete: (id: string) => api.delete(`/Challenge/${id}`),
};

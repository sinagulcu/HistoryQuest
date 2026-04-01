import api from "@/api/axios";
import type { Challenge, ChallengeCreateDto, ChallengeUpdateDto } from "@/types/challenge.types";

export const challengeApi = {
  getAll: () => api.get<Challenge[]>("/Challenge"),
  getById: (id: string) => api.get<Challenge>(`/Challenge/${id}`),
  create: (data: ChallengeCreateDto) => api.post<Challenge>("/Challenge", data),
  update: (id: string, data: ChallengeUpdateDto) => api.put<Challenge>(`/Challenge/${id}`, data),
  delete: (id: string) => api.delete(`/Challenge/${id}`),
};

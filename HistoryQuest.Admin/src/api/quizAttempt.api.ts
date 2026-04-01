import api from "./axios";

export const quizAttemptApi = {
  getAll: () => api.get<unknown[]>("/QuizAttempt"),
};

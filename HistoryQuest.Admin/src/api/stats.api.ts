import api from "./axios";

export interface DashboardStats {
  totalUsers: number;
  totalQuizzes: number;
  totalQuestions: number;
  totalAttempts: number;
  recentActivity: Array<Record<string, unknown>>;
}

export const statsApi = {
  getDashboard: () => api.get<DashboardStats>("/Stats/dashboard"),
};
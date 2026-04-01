import api from "./axios";
import type { User } from "@/types/user.types";
import type { UserRole } from "@/types/auth.types";

export const userApi = {
  getAll: () => api.get<User[]>("/User"),
  getById: (id: string) => api.get<User>(`/User/${id}`),
  updateRole: (id: string, role: UserRole) => api.put(`/User/${id}/role`, { role }),
  delete: (id: string) => api.delete(`/User/${id}`),
};
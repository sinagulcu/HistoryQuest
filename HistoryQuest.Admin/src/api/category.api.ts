import api from "./axios";
import type { Category, CategoryCreateDto, CategoryUpdateDto } from "@/types/category.types";

export const categoryApi = {
  getAll: () => api.get<Category[]>("/Category"),
  getById: (id: string) => api.get<Category>(`/Category/${id}`),
  create: (data: CategoryCreateDto) => api.post<Category>("/Category", data),
  update: (id: string, data: CategoryUpdateDto) => api.put<Category>(`/Category/${id}`, data),
  delete: (id: string) => api.delete(`/Category/${id}`),
};
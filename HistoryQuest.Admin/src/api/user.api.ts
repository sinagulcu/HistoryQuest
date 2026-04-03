import api from "./axios";
import type { User } from "@/types/user.types";
import type { UserRole } from "@/types/auth.types";
import type { UserCountResponse } from "@/types/user.types";
import { unwrapApiData } from "./apiUtils";

const normalizeUser = (raw: unknown): User => {
  const item = unwrapApiData<Record<string, unknown>>(raw);
  return {
    id: String(item.id ?? item.Id ?? ""),
    userName: String(item.userName ?? item.username ?? item.UserName ?? ""),
    email: String(item.email ?? item.Email ?? ""),
    firstName:
      typeof item.firstName === "string"
        ? item.firstName
        : typeof item.FirstName === "string"
          ? item.FirstName
          : undefined,
    lastName:
      typeof item.lastName === "string"
        ? item.lastName
        : typeof item.LastName === "string"
          ? item.LastName
          : undefined,
    fullName:
      typeof item.fullName === "string"
        ? item.fullName
        : typeof item.FullName === "string"
          ? item.FullName
          : undefined,
    score:
      typeof item.score === "number"
        ? item.score
        : typeof item.totalScore === "number"
          ? item.totalScore
          : typeof item.points === "number"
            ? item.points
            : undefined,
    role: String(item.role ?? item.Role ?? "Student") as UserRole,
    createdAt:
      typeof item.createdAt === "string"
        ? item.createdAt
        : typeof item.CreatedAt === "string"
          ? item.CreatedAt
          : undefined,
  };
};

const normalizeUsers = (raw: unknown): User[] => {
  const list = unwrapApiData<unknown[]>(raw);
  if (!Array.isArray(list)) {
    return [];
  }
  return list.map((item) => normalizeUser(item));
};

const normalizeUserCount = (raw: unknown): UserCountResponse => {
  if (typeof raw === "number" && Number.isFinite(raw)) {
    return { totalUsers: raw };
  }

  const item = unwrapApiData<Record<string, unknown>>(raw);
  const totalUsers =
    typeof item.totalUsers === "number"
      ? item.totalUsers
      : typeof item.totalUserCount === "number"
        ? item.totalUserCount
        : typeof item.count === "number"
          ? item.count
          : typeof item.value === "number"
            ? item.value
            : 0;
  return { totalUsers };
};

export const userApi = {
  getAll: async () => {
    let response;
    try {
      response = await api.get("/Users");
    } catch {
      response = await api.get("/User");
    }
    return {
      ...response,
      data: normalizeUsers(response.data),
    };
  },
  getById: async (id: string) => {
    const response = await api.get(`/Users/${id}`);
    return {
      ...response,
      data: normalizeUser(response.data),
    };
  },
  getCount: async () => {
    let response;
    try {
      response = await api.get("/Users/count");
    } catch {
      response = await api.get("/User/count");
    }
    return {
      ...response,
      data: normalizeUserCount(response.data),
    };
  },
  updateRole: (id: string, role: UserRole) => api.post("/Auth/change-role", { userId: id, newRole: role }),
  delete: (id: string) => api.delete(`/Users/${id}`),
};
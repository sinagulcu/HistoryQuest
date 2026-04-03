import type { UserRole } from "./auth.types";

export interface User {
  id: string;
  userName: string;
  email: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
  score?: number;
  role: UserRole;
  createdAt?: string;
}

export interface UserCountResponse {
  totalUsers: number;
}
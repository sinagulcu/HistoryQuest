import type { UserRole } from "./auth.types";

export interface User {
  id: string;
  userName: string;
  email: string;
  role: UserRole;
  createdAt?: string;
}
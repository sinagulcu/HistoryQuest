export type UserRole = "Admin" | "Teacher" | "Student";

export interface AuthUser {
  id: string;
  userName: string;
  email: string;
  role: UserRole;
}

export interface LoginRequestDto {
  identifier: string;
  password: string;
}

export interface LoginResponseDto {
  token?: string;
  accessToken?: string;
  refreshToken?: string;
  expiration?: string;
  user: AuthUser;
}

export interface RegisterRequestDto {
  username: string;
  email: string;
  password: string;
}
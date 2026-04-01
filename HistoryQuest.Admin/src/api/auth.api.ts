import axios from "axios";
import api from "./axios";
import type { LoginRequestDto, LoginResponseDto, RegisterRequestDto } from "@/types/auth.types";
import { unwrapApiData } from "./apiUtils";

const tryLogin = (payload: Record<string, string>) =>
  api.post<LoginResponseDto>("/Auth/login", payload, {
    skipAuth: true,
  });

const decodeJwtPayload = (token?: string) => {
  if (!token) {
    return null;
  }

  try {
    const part = token.split(".")[1];
    if (!part) {
      return null;
    }

    const normalized = part.replace(/-/g, "+").replace(/_/g, "/");
    const padded = normalized + "=".repeat((4 - (normalized.length % 4)) % 4);
    const decoded = atob(padded);

    return JSON.parse(decoded) as Record<string, unknown>;
  } catch {
    return null;
  }
};

const mapRole = (value: unknown): "Admin" | "Teacher" | "Student" | null => {
  if (Array.isArray(value) && value.length > 0) {
    const normalizedRoles = value
      .map((item) => (typeof item === "string" ? item.toLowerCase() : String(item).toLowerCase()))
      .filter(Boolean);

    if (normalizedRoles.includes("admin")) {
      return "Admin";
    }
    if (normalizedRoles.includes("teacher")) {
      return "Teacher";
    }
    if (normalizedRoles.includes("student")) {
      return "Student";
    }

    return mapRole(value[0]);
  }

  if (typeof value === "number") {
    if (value === 2) {
      return "Admin";
    }
    if (value === 1) {
      return "Teacher";
    }
    return "Student";
  }

  const role = typeof value === "string" ? value.toLowerCase() : "";

  if (role === "admin") {
    return "Admin";
  }
  if (role === "teacher") {
    return "Teacher";
  }

  if (role === "student") {
    return "Student";
  }

  return null;
};

const normalizeLoginResponse = (raw: unknown): LoginResponseDto => {
  const unwrapped = unwrapApiData<Record<string, unknown>>(raw) ?? {};
  const nestedUserRaw = unwrapped.user ? unwrapApiData<Record<string, unknown>>(unwrapped.user) : null;
  const userRaw = nestedUserRaw && typeof nestedUserRaw === "object" ? nestedUserRaw : ({} as Record<string, unknown>);

  const tokenFromResponse =
    (typeof unwrapped.token === "string" ? unwrapped.token : undefined) ||
    (typeof unwrapped.accessToken === "string" ? unwrapped.accessToken : undefined);

  const jwtPayload = decodeJwtPayload(tokenFromResponse);

  const roleFromClaims =
    jwtPayload?.role ??
    jwtPayload?.roles ??
    jwtPayload?.roleType ??
    jwtPayload?.RoleType ??
    jwtPayload?.Role ??
    jwtPayload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ??
    jwtPayload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role"];

  const idFromClaims =
    jwtPayload?.nameid ??
    jwtPayload?.sub ??
    jwtPayload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

  const userNameFromClaims =
    jwtPayload?.unique_name ?? jwtPayload?.name ?? jwtPayload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];

  const emailFromClaims = jwtPayload?.email ?? jwtPayload?.["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];

  const roleCandidate =
    mapRole(userRaw.role) ??
    mapRole(userRaw.roleType) ??
    mapRole(userRaw.userRole) ??
    mapRole(unwrapped.role) ??
    mapRole(unwrapped.roleType) ??
    mapRole(roleFromClaims);

  return {
    token: tokenFromResponse,
    accessToken: typeof unwrapped.accessToken === "string" ? unwrapped.accessToken : undefined,
    refreshToken: typeof unwrapped.refreshToken === "string" ? unwrapped.refreshToken : undefined,
    expiration: typeof unwrapped.expiration === "string" ? unwrapped.expiration : undefined,
    user: {
      id: String(userRaw.id ?? unwrapped.userId ?? unwrapped.id ?? idFromClaims ?? ""),
      userName: String(userRaw.userName ?? userRaw.username ?? unwrapped.userName ?? unwrapped.username ?? userNameFromClaims ?? ""),
      email: String(userRaw.email ?? unwrapped.email ?? emailFromClaims ?? ""),
      role: roleCandidate ?? "Teacher",
    },
  };
};

export const authApi = {
  login: async (data: LoginRequestDto) => {
    const identifier = data.identifier.trim();
    const password = data.password;

    const response = await tryLogin({
      userNameOrEmail: identifier,
      password,
    });

    return {
      ...response,
      data: normalizeLoginResponse(response.data),
    };
  },
  register: (data: RegisterRequestDto) =>
    api.post<LoginResponseDto>("/Auth/register", data, {
      skipAuth: true,
    }),
};

export const getApiErrorMessage = (error: unknown, fallback: string) => {
  if (!axios.isAxiosError(error)) {
    return fallback;
  }

  if (typeof error.response?.data === "string" && error.response.data.trim()) {
    return error.response.data;
  }

  const responseData = error.response?.data as Record<string, unknown> | undefined;
  const directMessage = responseData?.message;

  if (typeof directMessage === "string" && directMessage.trim()) {
    return directMessage;
  }

  const errors = responseData?.errors;
  if (errors && typeof errors === "object") {
    const first = Object.values(errors as Record<string, unknown>).find((value) => {
      if (typeof value === "string") {
        return value.trim().length > 0;
      }
      if (Array.isArray(value)) {
        return value.length > 0;
      }
      return false;
    });

    if (typeof first === "string") {
      return first;
    }
    if (Array.isArray(first) && typeof first[0] === "string") {
      return first[0];
    }
  }

  if (error.response?.status) {
    return `${fallback} (HTTP ${error.response.status})`;
  }

  return fallback;
};
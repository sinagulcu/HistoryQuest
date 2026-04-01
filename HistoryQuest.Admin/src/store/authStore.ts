import { create } from "zustand";
import { persist } from "zustand/middleware";
import type { AuthUser, UserRole } from "@/types/auth.types";

interface AuthState {
  user: AuthUser | null;
  token: string | null;
  isAuthenticated: boolean;
  hydrated: boolean;
  login: (token: string, user: AuthUser) => void;
  logout: () => void;
  isAdmin: () => boolean;
  isTeacher: () => boolean;
  canEditQuiz: (quizCreatorId: string) => boolean;
  canEditQuestion: (questionCreatorId: string) => boolean;
  hasRole: (roles: UserRole[]) => boolean;
  setHydrated: (value: boolean) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      isAuthenticated: false,
      hydrated: false,

      login: (token, user) => {
        localStorage.setItem("historyquest_token", token);
        localStorage.setItem("historyquest_user", JSON.stringify(user));
        set({ user, token, isAuthenticated: true });
      },

      logout: () => {
        localStorage.removeItem("historyquest_token");
        localStorage.removeItem("historyquest_user");
        set({ user: null, token: null, isAuthenticated: false });
      },

      isAdmin: () => get().user?.role === "Admin",

      isTeacher: () => get().user?.role === "Teacher",

      hasRole: (roles) => {
        const role = get().user?.role;
        return role ? roles.includes(role) : false;
      },

      canEditQuiz: (quizCreatorId) => {
        const user = get().user;
        if (!user) {
          return false;
        }
        if (user.role === "Admin") {
          return true;
        }
        return user.id === quizCreatorId;
      },

      canEditQuestion: (questionCreatorId) => {
        const user = get().user;
        if (!user) {
          return false;
        }
        if (user.role === "Admin") {
          return true;
        }
        return user.id === questionCreatorId;
      },

      setHydrated: (value) => set({ hydrated: value }),
    }),
    {
      name: "historyquest-auth",
      onRehydrateStorage: () => (state) => {
        state?.setHydrated(true);
      },
    }
  )
);
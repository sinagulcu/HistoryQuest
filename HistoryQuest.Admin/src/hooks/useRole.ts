import { useAuthStore } from "@/store/authStore";
import type { UserRole } from "@/types/auth.types";

export const useRole = () => {
  const user = useAuthStore((state) => state.user);
  const isAdmin = useAuthStore((state) => state.isAdmin);
  const isTeacher = useAuthStore((state) => state.isTeacher);
  const hasRole = useAuthStore((state) => state.hasRole);

  const canAccess = (roles: UserRole[]) => hasRole(roles);

  return {
    role: user?.role,
    isAdmin: isAdmin(),
    isTeacher: isTeacher(),
    canAccess,
  };
};
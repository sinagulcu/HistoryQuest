import { useEffect } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { toast } from "sonner";
import type { UserRole } from "@/types/auth.types";
import { useAuth } from "@/hooks/useAuth";

interface RoleGuardProps {
  allowedRoles: UserRole[];
}

export default function RoleGuard({ allowedRoles }: RoleGuardProps) {
  const { user } = useAuth();
  const unauthorized = Boolean(user && !allowedRoles.includes(user.role));

  useEffect(() => {
    if (unauthorized) {
      toast.error("Bu sayfaya erisim yetkiniz yok");
    }
  }, [unauthorized]);

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  if (unauthorized) {
    return <Navigate to="/dashboard" replace />;
  }

  return <Outlet />;
}

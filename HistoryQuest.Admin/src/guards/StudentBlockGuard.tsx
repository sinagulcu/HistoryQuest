import { useEffect } from "react";
import { Navigate, Outlet } from "react-router-dom";
import { toast } from "sonner";
import { useAuth } from "@/hooks/useAuth";

export default function StudentBlockGuard() {
  const { user, logout } = useAuth();
  const isBlocked = user?.role === "Student";

  useEffect(() => {
    if (isBlocked) {
      logout();
      toast.error("Bu panel sadece ogretmenler ve yoneticiler icindir");
    }
  }, [isBlocked, logout]);

  if (isBlocked) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}

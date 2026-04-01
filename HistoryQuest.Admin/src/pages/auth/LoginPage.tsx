import { zodResolver } from "@hookform/resolvers/zod";
import axios from "axios";
import { Loader2 } from "lucide-react";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { Navigate, useLocation, useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { authApi, getApiErrorMessage } from "@/api/auth.api";
import api from "@/api/axios";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Label from "@/components/ui/label";
import { useAuth } from "@/hooks/useAuth";
import type { AuthUser } from "@/types/auth.types";
import { loginSchema, type LoginFormValues } from "@/utils/validators";

interface LocationState {
  from?: {
    pathname: string;
  };
}

interface LoginDebugState {
  endpoint: string;
  payload: {
    userNameOrEmail: string;
    passwordLength: number;
  };
  requestTime: string;
  status?: number;
  ok: boolean;
  message: string;
  responseData?: unknown;
}

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { user, isAuthenticated, login, logout } = useAuth();
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [showDebug, setShowDebug] = useState(false);
  const [debugState, setDebugState] = useState<LoginDebugState | null>(null);
  const [testingConnection, setTestingConnection] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      identifier: "",
      password: "",
    },
  });

  useEffect(() => {
    if (user?.role === "Student") {
      logout();
      setSubmitError("Bu panel sadece öğretmenler ve yöneticiler içindir");
      toast.error("Bu panel sadece öğretmenler ve yöneticiler içindir");
    }
  }, [logout, user?.role]);

  if (isAuthenticated && user && user.role !== "Student") {
    const state = location.state as LocationState | null;
    const redirectPath = state?.from?.pathname || "/dashboard";
    return <Navigate to={redirectPath} replace />;
  }

  const onSubmit = async (values: LoginFormValues) => {
    setSubmitError(null);

    const debugPayload = {
      userNameOrEmail: values.identifier.trim(),
      passwordLength: values.password.length,
    };

    setDebugState({
      endpoint: `${import.meta.env.VITE_API_BASE_URL || "https://localhost:7086/api"}/Auth/login`,
      payload: debugPayload,
      requestTime: new Date().toISOString(),
      ok: false,
      message: "Istek gonderildi",
    });

    try {
      const { data } = await authApi.login({
        identifier: values.identifier,
        password: values.password,
      });

      setDebugState((prev) =>
        prev
          ? {
              ...prev,
              ok: true,
              status: 200,
              message: "Login basarili",
              responseData: {
                userId: data.user.id,
                userName: data.user.userName,
                role: data.user.role,
                hasToken: Boolean(data.token || data.accessToken),
              },
            }
          : prev
      );

      if (data.user.role === "Student") {
        setSubmitError("Bu panel sadece öğretmenler ve yöneticiler içindir");
        toast.error("Bu panel sadece öğretmenler ve yöneticiler içindir");
        return;
      }

      const authToken = data.token || data.accessToken;
      if (!authToken) {
        throw new Error("Token bulunamadi");
      }

      login(authToken, data.user);
      toast.success("Giriş başarılı");
      navigate("/dashboard", { replace: true });
    } catch (error) {
      const errorMessage = getApiErrorMessage(error, "Giris basarisiz. Bilgilerinizi kontrol ediniz.");
      setSubmitError(errorMessage);
      toast.error(errorMessage);

      if (axios.isAxiosError(error)) {
        setDebugState((prev) =>
          prev
            ? {
                ...prev,
                status: error.response?.status,
                ok: false,
                message: errorMessage,
                responseData: error.response?.data,
              }
            : prev
        );
      } else {
        setDebugState((prev) =>
          prev
            ? {
                ...prev,
                ok: false,
                message: errorMessage,
              }
            : prev
        );
      }
    }
  };

  const handleConnectionTest = async () => {
    setTestingConnection(true);
    try {
      const response = await api.get("/Test/public", { skipAuth: true });
      toast.success("API baglanti testi basarili");
      setDebugState((prev) => ({
        endpoint: `${import.meta.env.VITE_API_BASE_URL || "https://localhost:7086/api"}/Test/public`,
        payload: prev?.payload || { userNameOrEmail: "-", passwordLength: 0 },
        requestTime: new Date().toISOString(),
        status: response.status,
        ok: true,
        message: "API erisilebilir",
        responseData: response.data,
      }));
    } catch (error) {
      const message = getApiErrorMessage(error, "API baglanti testi basarisiz");
      toast.error(message);
      setDebugState((prev) => ({
        endpoint: `${import.meta.env.VITE_API_BASE_URL || "https://localhost:7086/api"}/Test/public`,
        payload: prev?.payload || { userNameOrEmail: "-", passwordLength: 0 },
        requestTime: new Date().toISOString(),
        status: axios.isAxiosError(error) ? error.response?.status : undefined,
        ok: false,
        message,
        responseData: axios.isAxiosError(error) ? error.response?.data : undefined,
      }));
    } finally {
      setTestingConnection(false);
    }
  };

  const handleDevAdminLogin = () => {
    const devAdminUser: AuthUser = {
      id: "dev-admin-local",
      userName: "Local Admin",
      email: "local-admin@historyquest.dev",
      role: "Admin",
    };

    login("dev-local-bypass-token", devAdminUser);
    toast.success("Geliştirme girişi ile admin oturumu açıldı");
    navigate("/dashboard", { replace: true });
  };

  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden px-4 py-8">
      <div className="absolute inset-0 -z-10 bg-[radial-gradient(circle_at_20%_20%,rgba(176,137,61,0.16),transparent_40%),radial-gradient(circle_at_80%_10%,rgba(120,113,108,0.12),transparent_38%)]" />

      <div className="hq-fade-in w-full max-w-5xl overflow-hidden rounded-3xl border border-stone-200/70 bg-stone-50/90 shadow-2xl backdrop-blur-xl dark:border-stone-800 dark:bg-stone-950/70">
        <div className="grid md:grid-cols-2">
          <div className="hidden bg-[linear-gradient(145deg,#5c4618_0%,#8b6b29_45%,#c8a55a_100%)] p-10 text-white md:block">
            <p className="inline-flex rounded-full border border-white/40 bg-white/10 px-3 py-1 text-xs font-medium uppercase tracking-[0.18em]">
              HistoryQuest
            </p>
            <h1 className="mt-6 text-3xl font-semibold leading-tight">Tarih Egitimi Yonetim Paneli</h1>
            <p className="mt-4 text-sm text-white/85">
              Quiz, soru ve oyunlastirma akisini tek panelden yonetin. Ogretmen ve admin odakli hizli yonetim deneyimi.
            </p>
          </div>

          <div className="p-6 sm:p-8">
            <div className="mb-6">
              <h2 className="hq-gold-text text-2xl font-semibold tracking-tight">Panel Girisi</h2>
              <p className="mt-1 text-sm text-stone-600 dark:text-stone-400">Hesabinizla giris yaparak yonetime devam edin.</p>
            </div>

            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="identifier">E-posta veya Kullanici Adi</Label>
                <Input
                  id="identifier"
                  type="text"
                  autoComplete="username"
                  placeholder="ornek@mail.com veya kullaniciadi"
                  {...register("identifier")}
                />
                {errors.identifier ? <p className="text-xs text-red-600">{errors.identifier.message}</p> : null}
              </div>

              <div className="space-y-2">
                <Label htmlFor="password">Sifre</Label>
                <Input id="password" type="password" autoComplete="current-password" placeholder="********" {...register("password")} />
                {errors.password ? <p className="text-xs text-red-600">{errors.password.message}</p> : null}
              </div>

              {submitError ? (
                <p className="rounded-lg border border-red-200 bg-red-50/80 px-3 py-2 text-sm text-red-600 dark:border-red-900/50 dark:bg-red-950/40">
                  {submitError}
                </p>
              ) : null}

              <Button type="submit" className="w-full" disabled={isSubmitting}>
                {isSubmitting ? (
                  <span className="flex items-center gap-2">
                    <Loader2 className="h-4 w-4 animate-spin" />
                    Giris yapiliyor...
                  </span>
                ) : (
                  "Giris Yap"
                )}
              </Button>

              <Button type="button" variant="outline" className="w-full" onClick={handleDevAdminLogin}>
                Sunucu Kapaliysa Dev Admin ile Giris Yap
              </Button>

              <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
                <Button type="button" variant="outline" onClick={handleConnectionTest} disabled={testingConnection}>
                  {testingConnection ? "Baglanti test ediliyor..." : "API Baglanti Testi"}
                </Button>
                <Button type="button" variant="ghost" onClick={() => setShowDebug((prev) => !prev)}>
                  {showDebug ? "Debug Gizle" : "Debug Goster"}
                </Button>
              </div>

              {showDebug ? (
                <div className="rounded-lg border border-stone-200 bg-stone-100/70 p-3 text-xs text-stone-700 dark:border-stone-700 dark:bg-stone-900/70 dark:text-stone-300">
                  <p>
                    <strong>Endpoint:</strong> {debugState?.endpoint || "-"}
                  </p>
                  <p>
                    <strong>Istek Zamani:</strong> {debugState?.requestTime || "-"}
                  </p>
                  <p>
                    <strong>Status:</strong> {debugState?.status ?? "-"}
                  </p>
                  <p>
                    <strong>Mesaj:</strong> {debugState?.message || "-"}
                  </p>
                  <p>
                    <strong>Kullanici:</strong> {debugState?.payload.userNameOrEmail || "-"}
                  </p>
                  <p>
                    <strong>Sifre Uzunlugu:</strong> {debugState?.payload.passwordLength ?? "-"}
                  </p>
                  <pre className="mt-2 max-h-40 overflow-auto rounded border border-stone-300/70 bg-stone-50 p-2 text-[11px] dark:border-stone-700 dark:bg-stone-950/80">
                    {JSON.stringify(debugState?.responseData ?? {}, null, 2)}
                  </pre>
                </div>
              ) : null}
            </form>

            <p className="mt-3 text-center text-xs text-[#8f6d28] dark:text-[#d7be80]">
              Gecici test girisidir. Is bittiginde kaldirilmalidir.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

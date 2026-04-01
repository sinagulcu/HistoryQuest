import axios from "axios";
import { RefreshCw, Trash2, UserRoundCog, X } from "lucide-react";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { userApi } from "@/api/user.api";
import ConfirmDialog from "@/components/shared/ConfirmDialog";
import EmptyState from "@/components/shared/EmptyState";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/hooks/useAuth";
import type { UserRole } from "@/types/auth.types";
import type { User } from "@/types/user.types";
import { formatServerDateTime } from "@/utils/dateTime";

function getApiErrorMessage(error: unknown, fallbackMessage: string) {
  if (axios.isAxiosError(error)) {
    const responseData = error.response?.data as { message?: unknown } | undefined;
    if (typeof responseData?.message === "string") {
      return responseData.message;
    }
  }
  return fallbackMessage;
}

function getRoleClass(role: UserRole) {
  if (role === "Admin") {
    return "bg-red-100 text-red-700 dark:bg-red-950/40 dark:text-red-300";
  }
  if (role === "Teacher") {
    return "bg-amber-100 text-amber-800 dark:bg-amber-950/40 dark:text-amber-300";
  }
  return "bg-emerald-100 text-emerald-700 dark:bg-emerald-950/40 dark:text-emerald-300";
}

function formatCreatedAt(user: User) {
  return formatServerDateTime(user.createdAt);
}

export default function UserListPage() {
  const { user: currentUser } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [selectedRole, setSelectedRole] = useState<UserRole>("Student");
  const [updatingRole, setUpdatingRole] = useState(false);
  const [deletingUserId, setDeletingUserId] = useState<string | null>(null);
  const [pendingDeleteUser, setPendingDeleteUser] = useState<User | null>(null);

  const fetchUsers = async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await userApi.getAll();
      setUsers(response.data);
    } catch (requestError) {
      setError(getApiErrorMessage(requestError, "Kullanicilar yuklenirken hata olustu."));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUsers();
  }, []);

  const openRoleDialog = (targetUser: User) => {
    if (currentUser?.id === targetUser.id) {
      toast.error("Kendi rolunuzu degistiremezsiniz");
      return;
    }

    setSelectedUser(targetUser);
    setSelectedRole(targetUser.role);
  };

  const closeRoleDialog = () => {
    setSelectedUser(null);
  };

  const handleRoleUpdate = async () => {
    if (!selectedUser) {
      return;
    }

    if (currentUser?.id === selectedUser.id) {
      toast.error("Kendi rolunuzu degistiremezsiniz");
      return;
    }

    setUpdatingRole(true);
    try {
      await userApi.updateRole(selectedUser.id, selectedRole);
      toast.success("Kullanici rolu guncellendi");
      closeRoleDialog();
      await fetchUsers();
    } catch (requestError) {
      toast.error(getApiErrorMessage(requestError, "Rol degistirilirken hata olustu."));
    } finally {
      setUpdatingRole(false);
    }
  };

  const handleDeleteUser = async (targetUser: User) => {
    if (currentUser?.id === targetUser.id) {
      toast.error("Kendi hesabinizi silemezsiniz");
      return;
    }

    setDeletingUserId(targetUser.id);
    try {
      await userApi.delete(targetUser.id);
      toast.success("Kullanici silindi");
      setPendingDeleteUser(null);
      await fetchUsers();
    } catch (requestError) {
      toast.error(getApiErrorMessage(requestError, "Kullanici silinirken hata olustu."));
    } finally {
      setDeletingUserId(null);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection
        title="Kullanici Yonetimi"
        description="Kullanici rolleri, yetki duzenlemeleri ve silme islemleri"
        actions={
          <Button variant="outline" className="gap-2" onClick={fetchUsers}>
            <RefreshCw className="h-4 w-4" />
            Listeyi Yenile
          </Button>
        }
      />

      {loading ? <LoadingState message="Kullanicilar yukleniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchUsers} /> : null}

      {!loading && !error && users.length === 0 ? <EmptyState message="Kayitli kullanici bulunamadi." /> : null}

      {!loading && !error && users.length > 0 ? (
        <div className="overflow-x-auto rounded-lg border border-stone-200 bg-white dark:border-stone-800 dark:bg-stone-900">
          <table className="min-w-full divide-y divide-stone-200 dark:divide-stone-800">
            <thead>
              <tr className="text-left text-xs uppercase tracking-wide text-stone-500 dark:text-stone-400">
                <th className="px-4 py-3">Kullanici Adi</th>
                <th className="px-4 py-3">Email</th>
                <th className="px-4 py-3">Rol</th>
                <th className="px-4 py-3">Kayit Tarihi</th>
                <th className="px-4 py-3 text-right">Islemler</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-stone-200 dark:divide-stone-800">
              {users.map((targetUser) => {
                const isSelf = currentUser?.id === targetUser.id;

                return (
                  <tr key={targetUser.id} className="text-sm text-stone-700 dark:text-stone-200">
                    <td className="px-4 py-3">{targetUser.userName}</td>
                    <td className="px-4 py-3">{targetUser.email}</td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex rounded-full px-2.5 py-1 text-xs font-semibold ${getRoleClass(targetUser.role)}`}>
                        {targetUser.role}
                      </span>
                    </td>
                    <td className="px-4 py-3">{formatCreatedAt(targetUser)}</td>
                    <td className="px-4 py-3">
                      <div className="flex justify-end gap-2">
                        <Button
                          variant="outline"
                          className="gap-2"
                          disabled={isSelf}
                          onClick={() => openRoleDialog(targetUser)}
                        >
                          <UserRoundCog className="h-4 w-4" />
                          Rol Degistir
                        </Button>
                        <Button
                          variant="danger"
                          className="gap-2"
                          disabled={isSelf || deletingUserId === targetUser.id}
                          onClick={() => setPendingDeleteUser(targetUser)}
                        >
                          <Trash2 className="h-4 w-4" />
                          Sil
                        </Button>
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      ) : null}

      {selectedUser ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4">
          <div className="w-full max-w-md rounded-lg bg-white p-6 shadow-xl dark:bg-stone-900">
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-lg font-semibold text-stone-900 dark:text-stone-100">Rol Degistir</h2>
              <Button variant="ghost" onClick={closeRoleDialog} aria-label="Kapat">
                <X className="h-4 w-4" />
              </Button>
            </div>

            <div className="space-y-3 text-sm">
              <p className="text-stone-600 dark:text-stone-300">Kullanici: {selectedUser.email}</p>
              <p className="text-stone-600 dark:text-stone-300">Mevcut Rol: {selectedUser.role}</p>

              <label className="block text-stone-700 dark:text-stone-200" htmlFor="new-role-select">
                Yeni Rol
              </label>
              <select
                id="new-role-select"
                className="h-10 w-full rounded-md border border-stone-300 bg-white px-3 text-sm dark:border-stone-700 dark:bg-stone-900"
                value={selectedRole}
                onChange={(event) => setSelectedRole(event.target.value as UserRole)}
              >
                <option value="Admin">Admin</option>
                <option value="Teacher">Teacher</option>
                <option value="Student">Student</option>
              </select>
            </div>

            <div className="mt-6 flex justify-end gap-2">
              <Button variant="outline" onClick={closeRoleDialog}>
                Iptal
              </Button>
              <Button onClick={handleRoleUpdate} disabled={updatingRole}>
                {updatingRole ? "Degistiriliyor..." : "Degistir"}
              </Button>
            </div>
          </div>
        </div>
      ) : null}

      <ConfirmDialog
        open={Boolean(pendingDeleteUser)}
        title="Kullanici Sil"
        description={
          pendingDeleteUser
            ? `${pendingDeleteUser.email} kullanicisini silmek istediginize emin misiniz? Bu islem geri alinamaz.`
            : ""
        }
        confirmLabel="Evet, Sil"
        loading={deletingUserId !== null}
        onCancel={() => setPendingDeleteUser(null)}
        onConfirm={() => {
          if (pendingDeleteUser) {
            handleDeleteUser(pendingDeleteUser);
          }
        }}
      />
    </div>
  );
}

import axios from "axios";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { challengeApi } from "@/api/challenge.api";
import { questionApi } from "@/api/question.api";
import ConfirmDialog from "@/components/shared/ConfirmDialog";
import EmptyState from "@/components/shared/EmptyState";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useAuth } from "@/hooks/useAuth";
import type { Challenge, ChallengeStatus } from "@/types/challenge.types";
import { formatServerDateTime } from "@/utils/dateTime";

const statusLabelMap: Record<ChallengeStatus, string> = {
  Scheduled: "Planlandi",
  Active: "Yayinda",
  Expired: "Suresi Doldu",
};

const statusClassMap: Record<ChallengeStatus, string> = {
  Scheduled: "bg-[#f7efdc] text-[#876822] dark:bg-[#2f2615] dark:text-[#d7bd7e]",
  Active: "bg-emerald-100 text-emerald-700 dark:bg-emerald-950 dark:text-emerald-300",
  Expired: "bg-stone-200 text-stone-700 dark:bg-stone-800 dark:text-stone-200",
};

export default function ChallengeListPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const [challenges, setChallenges] = useState<Challenge[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState<ChallengeStatus | "all">("all");
  const [creatorFilter, setCreatorFilter] = useState<"all" | "mine" | "others">("all");
  const [pendingDelete, setPendingDelete] = useState<Challenge | null>(null);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [questionTextById, setQuestionTextById] = useState<Map<string, string>>(new Map());

  const fetchChallenges = async () => {
    setLoading(true);
    setError(null);
    try {
      const [{ data: challengeData }, { data: questionData }] = await Promise.all([challengeApi.getAll(), questionApi.getAll()]);
      setChallenges(challengeData);
      setQuestionTextById(new Map(questionData.map((question) => [question.id, question.text])));
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Meydan okumalar yuklenemedi.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchChallenges();
  }, []);

  const canManageChallenge = (challenge: Challenge) => {
    if (!user) {
      return false;
    }
    if (user.role === "Admin") {
      return true;
    }
    return challenge.createdByTeacherId === user.id;
  };

  const filteredChallenges = useMemo(() => {
    return challenges.filter((challenge) => {
      const searchMatch =
        challenge.title.toLowerCase().includes(search.toLowerCase()) ||
        challenge.questionText?.toLowerCase().includes(search.toLowerCase());

      const statusMatch = statusFilter === "all" || challenge.status === statusFilter;

      let creatorMatch = true;
      if (creatorFilter === "mine") {
        creatorMatch = challenge.createdByTeacherId === user?.id;
      }
      if (creatorFilter === "others") {
        creatorMatch = challenge.createdByTeacherId !== user?.id;
      }

      return searchMatch && statusMatch && creatorMatch;
    });
  }, [challenges, creatorFilter, search, statusFilter, user?.id]);

  const handleDelete = async (challenge: Challenge) => {
    if (!canManageChallenge(challenge)) {
      toast.error("Bu kaydi silme yetkiniz yok");
      return;
    }

    setDeletingId(challenge.id);
    try {
      await challengeApi.delete(challenge.id);
      toast.success("Meydan okuma silindi");
      setPendingDelete(null);
      await fetchChallenges();
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Meydan okuma silinemedi.";
      toast.error(message);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection
        title="Sureli Meydan Okumalar"
        description="Sorulari tarih ve saate gore planlayin, puanlama kurallarini belirleyin."
        actions={
          <Button className="gap-2" onClick={() => navigate("/challenges/create")}>
            <Plus className="h-4 w-4" />
            Meydan Okuma Ekle
          </Button>
        }
      />

      <div className="grid gap-3 md:grid-cols-3">
        <Input placeholder="Baslik veya soru ile ara" value={search} onChange={(event) => setSearch(event.target.value)} />
        <select
          className="h-10 rounded-md border border-stone-300 bg-white px-3 text-sm dark:border-stone-700 dark:bg-stone-900"
          value={statusFilter}
          onChange={(event) => setStatusFilter(event.target.value as ChallengeStatus | "all")}
        >
          <option value="all">Tum durumlar</option>
          <option value="Scheduled">Planlandi</option>
          <option value="Active">Yayinda</option>
          <option value="Expired">Suresi Doldu</option>
        </select>
        <select
          className="h-10 rounded-md border border-stone-300 bg-white px-3 text-sm dark:border-stone-700 dark:bg-stone-900"
          value={creatorFilter}
          onChange={(event) => setCreatorFilter(event.target.value as "all" | "mine" | "others")}
        >
          <option value="all">Tum olusturanlar</option>
          <option value="mine">Sadece benim kayitlarim</option>
          <option value="others">Diger ogretmenler</option>
        </select>
      </div>

      {loading ? <LoadingState message="Meydan okumalar yukleniyor..." /> : null}
      {error ? <ErrorState message={error} onRetry={fetchChallenges} /> : null}
      {!loading && !error && filteredChallenges.length === 0 ? (
        <EmptyState message="Kayit bulunamadi. Ilk sureli meydan okumayi olusturun." />
      ) : null}

      {!loading && !error && filteredChallenges.length > 0 ? (
        <div className="overflow-x-auto rounded-lg border border-stone-200 bg-white dark:border-stone-800 dark:bg-stone-900">
          <table className="min-w-full divide-y divide-stone-200 dark:divide-stone-800">
            <thead>
              <tr className="text-left text-xs uppercase tracking-wide text-stone-500 dark:text-stone-400">
                <th className="px-4 py-3">Baslik</th>
                <th className="px-4 py-3">Soru</th>
                <th className="px-4 py-3">Baslangic</th>
                <th className="px-4 py-3">Puanli/Ek Sure</th>
                <th className="px-4 py-3">Puan</th>
                <th className="px-4 py-3">Durum</th>
                <th className="px-4 py-3">Olusturan</th>
                <th className="px-4 py-3 text-right">Islemler</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-stone-200 dark:divide-stone-800">
              {filteredChallenges.map((challenge) => {
                const challengeStatus = challenge.status;
                const scoringMinutes = Math.floor(challenge.answerWindowSeconds / 60);
                const extraMinutes = Math.max(0, Math.floor((challenge.visibilityWindowSeconds - challenge.answerWindowSeconds) / 60));
                return (
                  <tr key={challenge.id} className="text-sm text-stone-700 dark:text-stone-200">
                    <td className="px-4 py-3">{challenge.title}</td>
                    <td className="max-w-sm px-4 py-3">{challenge.questionText || challenge.question?.text || questionTextById.get(challenge.questionId) || "Belirtilmedi"}</td>
                    <td className="whitespace-nowrap px-4 py-3">
                      {formatServerDateTime(challenge.scheduledAtUtc)}
                    </td>
                    <td className="px-4 py-3">
                      {scoringMinutes} dk / {extraMinutes} dk
                    </td>
                    <td className="px-4 py-3">{challenge.maxScore}</td>
                    <td className="px-4 py-3">
                      <span className={`inline-flex rounded-full px-2 py-1 text-xs font-semibold ${statusClassMap[challengeStatus]}`}>
                        {statusLabelMap[challengeStatus]}
                      </span>
                    </td>
                    <td className="px-4 py-3">
                      {challenge.createdByTeacherName || (challenge.createdByTeacherId === user?.id ? user.userName : "Belirtilmedi")}
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex justify-end gap-2">
                        {canManageChallenge(challenge) ? (
                          <>
                            <Button variant="outline" className="gap-2" onClick={() => navigate(`/challenges/${challenge.id}/edit`)}>
                              <Pencil className="h-4 w-4" />
                              Duzenle
                            </Button>
                            <Button variant="danger" className="gap-2" onClick={() => setPendingDelete(challenge)}>
                              <Trash2 className="h-4 w-4" />
                              Sil
                            </Button>
                          </>
                        ) : null}
                      </div>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      ) : null}

      <ConfirmDialog
        open={Boolean(pendingDelete)}
        title="Meydan Okuma Sil"
        description={
          pendingDelete
            ? `${pendingDelete.title} kaydini silmek istediginize emin misiniz? Bu islem geri alinamaz.`
            : ""
        }
        confirmLabel="Evet, Sil"
        loading={deletingId !== null}
        onCancel={() => setPendingDelete(null)}
        onConfirm={() => {
          if (pendingDelete) {
            handleDelete(pendingDelete);
          }
        }}
      />
    </div>
  );
}

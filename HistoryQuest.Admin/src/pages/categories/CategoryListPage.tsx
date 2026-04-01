import { zodResolver } from "@hookform/resolvers/zod";
import axios from "axios";
import { Pencil, Plus, Trash2, X } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { categoryApi } from "@/api/category.api";
import ConfirmDialog from "@/components/shared/ConfirmDialog";
import EmptyState from "@/components/shared/EmptyState";
import ErrorState from "@/components/shared/ErrorState";
import LoadingState from "@/components/shared/LoadingState";
import PageSection from "@/components/shared/PageSection";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Label from "@/components/ui/label";
import type { Category } from "@/types/category.types";
import { categorySchema, type CategoryFormValues } from "@/utils/validators";

export default function CategoryListPage() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);
  const [deletingId, setDeletingId] = useState<number | null>(null);
  const [pendingDeleteCategory, setPendingDeleteCategory] = useState<Category | null>(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CategoryFormValues>({
    resolver: zodResolver(categorySchema),
    defaultValues: {
      name: "",
      description: "",
    },
  });

  const modalTitle = useMemo(() => (editingCategory ? "Kategori Düzenle" : "Yeni Kategori Ekle"), [editingCategory]);

  const fetchCategories = async () => {
    setLoading(true);
    setError(null);
    try {
      const { data } = await categoryApi.getAll();
      setCategories(data);
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Kategoriler yüklenirken hata oluştu.";
      setError(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCategories();
  }, []);

  const openCreateModal = () => {
    setEditingCategory(null);
    reset({ name: "", description: "" });
    setIsModalOpen(true);
  };

  const openEditModal = (category: Category) => {
    setEditingCategory(category);
    reset({ name: category.name, description: category.description });
    setIsModalOpen(true);
  };

  const closeModal = () => {
    setIsModalOpen(false);
    setEditingCategory(null);
    reset({ name: "", description: "" });
  };

  const onSubmit = async (values: CategoryFormValues) => {
    setSaving(true);
    try {
      if (editingCategory) {
        await categoryApi.update(editingCategory.id, values);
        toast.success("Kategori güncellendi");
      } else {
        await categoryApi.create(values);
        toast.success("Kategori oluşturuldu");
      }
      closeModal();
      await fetchCategories();
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Kategori kaydedilirken hata oluştu.";
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async (category: Category) => {
    setDeletingId(category.id);
    try {
      await categoryApi.delete(category.id);
      toast.success("Kategori silindi");
      setPendingDeleteCategory(null);
      await fetchCategories();
    } catch (requestError) {
      const message =
        axios.isAxiosError(requestError) && typeof requestError.response?.data?.message === "string"
          ? requestError.response.data.message
          : "Kategori silinirken hata oluştu.";
      toast.error(message);
    } finally {
      setDeletingId(null);
    }
  };

  return (
    <div className="space-y-6">
      <PageSection
        title="Kategoriler"
        description="Kategori ekleyin, duzenleyin ve silin."
        actions={
          <Button onClick={openCreateModal} className="gap-2">
            <Plus className="h-4 w-4" />
            Kategori Ekle
          </Button>
        }
      />

      {loading ? <LoadingState message="Kategoriler yukleniyor..." /> : null}

      {error ? <ErrorState message={error} onRetry={fetchCategories} /> : null}

      {!loading && !error && categories.length === 0 ? <EmptyState message="Henuz kategori yok. Ilk kategoriyi ekleyebilirsiniz." /> : null}

      {!loading && !error && categories.length > 0 ? (
        <div className="overflow-x-auto rounded-2xl border border-stone-200/80 bg-white/85 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/65">
          <table className="min-w-full divide-y divide-stone-200 dark:divide-stone-800">
            <thead>
              <tr className="text-left text-xs uppercase tracking-wide text-stone-500 dark:text-stone-400">
                <th className="px-4 py-3">Ad</th>
                <th className="px-4 py-3">Aciklama</th>
                <th className="px-4 py-3 text-right">Islemler</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-stone-200 dark:divide-stone-800">
              {categories.map((category) => (
                <tr
                  key={category.id}
                  className="text-sm text-stone-700 transition-colors hover:bg-stone-50/80 dark:text-stone-200 dark:hover:bg-stone-800/40"
                >
                  <td className="px-4 py-3 font-medium">{category.name}</td>
                  <td className="px-4 py-3">{category.description}</td>
                  <td className="px-4 py-3">
                    <div className="flex items-center justify-end gap-2">
                      <Button variant="outline" onClick={() => openEditModal(category)} className="gap-2">
                        <Pencil className="h-4 w-4" />
                        Duzenle
                      </Button>
                       <Button variant="danger" onClick={() => setPendingDeleteCategory(category)} disabled={deletingId === category.id} className="gap-2">
                        <Trash2 className="h-4 w-4" />
                        Sil
                      </Button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : null}

      {isModalOpen ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 px-4">
          <div className="hq-fade-in w-full max-w-lg rounded-2xl border border-stone-200 bg-white/95 p-6 shadow-2xl dark:border-stone-700 dark:bg-stone-900/90">
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-lg font-semibold text-stone-900 dark:text-stone-100">{modalTitle}</h2>
              <Button variant="ghost" onClick={closeModal} aria-label="Kapat">
                <X className="h-4 w-4" />
              </Button>
            </div>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="name">Ad</Label>
                <Input id="name" placeholder="Kategori adi" {...register("name")} />
                {errors.name ? <p className="text-xs text-red-600">{errors.name.message}</p> : null}
              </div>

              <div className="space-y-2">
                <Label htmlFor="description">Aciklama</Label>
                <textarea
                  id="description"
                  className="min-h-24 w-full rounded-lg border border-stone-300 bg-white/95 px-3 py-2 text-sm outline-none transition-all focus:border-[#bc983f] focus:ring-4 focus:ring-[#bc983f]/15 dark:border-stone-700 dark:bg-stone-900/80 dark:focus:border-[#d1b16a] dark:focus:ring-[#d1b16a]/20"
                  placeholder="Kategori aciklamasi"
                  {...register("description")}
                />
                {errors.description ? <p className="text-xs text-red-600">{errors.description.message}</p> : null}
              </div>

              <div className="flex justify-end gap-2">
                <Button type="button" variant="outline" onClick={closeModal}>
                  Iptal
                </Button>
                <Button type="submit" disabled={saving}>
                  {saving ? "Kaydediliyor..." : "Kaydet"}
                </Button>
              </div>
            </form>
          </div>
        </div>
      ) : null}

      <ConfirmDialog
        open={Boolean(pendingDeleteCategory)}
        title="Kategori Sil"
        description={
          pendingDeleteCategory
            ? `${pendingDeleteCategory.name} kategorisini silmek istediginize emin misiniz? Bu isleme bagli sorular etkilenebilir.`
            : ""
        }
        confirmLabel="Evet, Sil"
        loading={deletingId !== null}
        onCancel={() => setPendingDeleteCategory(null)}
        onConfirm={() => {
          if (pendingDeleteCategory) {
            handleDelete(pendingDeleteCategory);
          }
        }}
      />
    </div>
  );
}

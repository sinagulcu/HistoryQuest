import { Button } from "@/components/ui/button";

interface ConfirmDialogProps {
  open: boolean;
  title: string;
  description: string;
  confirmLabel?: string;
  cancelLabel?: string;
  showConfirmButton?: boolean;
  loading?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
}

export default function ConfirmDialog({
  open,
  title,
  description,
  confirmLabel = "Onayla",
  cancelLabel = "İptal",
  showConfirmButton = true,
  loading = false,
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  if (!open) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4 backdrop-blur-sm">
      <div className="hq-fade-in w-full max-w-md rounded-2xl border border-stone-200 bg-white/95 p-6 shadow-2xl dark:border-stone-700 dark:bg-stone-900/90">
        <h2 className="text-lg font-semibold text-stone-900 dark:text-stone-100">{title}</h2>
        <p className="mt-2 text-sm leading-relaxed text-stone-600 dark:text-stone-300">{description}</p>

        <div className="mt-6 flex justify-end gap-2">
          <Button type="button" variant="outline" onClick={onCancel} disabled={loading}>
            {cancelLabel}
          </Button>
          {showConfirmButton ? (
            <Button type="button" variant="danger" onClick={onConfirm} disabled={loading}>
              {loading ? "İşleniyor..." : confirmLabel}
            </Button>
          ) : null}
        </div>
      </div>
    </div>
  );
}

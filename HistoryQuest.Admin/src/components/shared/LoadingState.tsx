import { Loader2 } from "lucide-react";

interface LoadingStateProps {
  message?: string;
}

export default function LoadingState({ message = "Veriler yukleniyor..." }: LoadingStateProps) {
  return (
    <div className="hq-fade-in rounded-xl border border-stone-200/80 bg-white/85 p-5 text-sm text-stone-600 shadow-sm backdrop-blur dark:border-stone-800 dark:bg-stone-900/70 dark:text-stone-300">
      <div className="flex items-center gap-3">
        <Loader2 className="h-4 w-4 animate-spin text-amber-600 dark:text-amber-400" />
        <span>{message}</span>
      </div>
    </div>
  );
}

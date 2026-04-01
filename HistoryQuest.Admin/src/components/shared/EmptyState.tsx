import { Inbox } from "lucide-react";

interface EmptyStateProps {
  message: string;
}

export default function EmptyState({ message }: EmptyStateProps) {
  return (
    <div className="hq-fade-in rounded-xl border border-dashed border-stone-300 bg-white/70 p-8 text-center text-sm text-stone-600 dark:border-stone-700 dark:bg-stone-900/40 dark:text-stone-300">
      <Inbox className="mx-auto mb-3 h-5 w-5 text-stone-500 dark:text-stone-400" />
      {message}
    </div>
  );
}

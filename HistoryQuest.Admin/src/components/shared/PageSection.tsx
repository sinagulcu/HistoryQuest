import type { ReactNode } from "react";

interface PageSectionProps {
  title: string;
  description: string;
  actions?: ReactNode;
}

export default function PageSection({ title, description, actions }: PageSectionProps) {
  return (
    <section className="hq-fade-in space-y-2">
      <div className="flex flex-wrap items-start justify-between gap-3">
        <div>
          <h1 className="hq-gold-text text-2xl font-semibold tracking-tight lg:text-3xl">{title}</h1>
          <div className="mt-2 h-1 w-14 rounded-full bg-[linear-gradient(135deg,#6a5018_0%,#a58233_52%,#d8bc77_100%)]" />
        </div>
        {actions}
      </div>
      <p className="max-w-3xl text-sm text-stone-600 dark:text-stone-400">{description}</p>
    </section>
  );
}

import type { LabelHTMLAttributes } from "react";
import { cn } from "@/utils/cn";

export default function Label({ className, ...props }: LabelHTMLAttributes<HTMLLabelElement>) {
  return <label className={cn("text-sm font-medium text-stone-700 dark:text-stone-200", className)} {...props} />;
}
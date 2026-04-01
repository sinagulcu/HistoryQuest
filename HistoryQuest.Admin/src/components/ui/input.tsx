import * as React from "react";
import { cn } from "@/utils/cn";

export interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {}

const Input = React.forwardRef<HTMLInputElement, InputProps>(({ className, ...props }, ref) => {
  return (
    <input
      ref={ref}
      className={cn(
        "flex h-10 w-full rounded-lg border border-stone-300 bg-white/95 px-3 py-2 text-sm text-stone-900 outline-none transition-all duration-200 placeholder:text-stone-400 focus:border-[#bc983f] focus:ring-4 focus:ring-[#bc983f]/15 dark:border-stone-700 dark:bg-stone-900/80 dark:text-stone-100 dark:focus:border-[#d1b16a] dark:focus:ring-[#d1b16a]/20",
        className
      )}
      {...props}
    />
  );
});

Input.displayName = "Input";

export { Input };
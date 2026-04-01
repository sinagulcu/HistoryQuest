import * as React from "react";
import { cn } from "@/utils/cn";

type ButtonVariant = "default" | "secondary" | "outline" | "ghost" | "danger";

interface ButtonProps extends React.ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: ButtonVariant;
}

const variantClassMap: Record<ButtonVariant, string> = {
  default:
    "hq-gold-surface shadow-[0_10px_22px_-16px_rgba(95,75,28,0.65)] hover:brightness-110",
  secondary:
    "border border-[#d6c08a]/60 bg-[#fffaf0] text-[#644b17] hover:border-[#c3a055] hover:bg-[#f9efd8] dark:border-[#7f6835] dark:bg-[#2a2113] dark:text-[#ebd9ae] dark:hover:border-[#b6934a] dark:hover:bg-[#352a18]",
  outline:
    "border border-[#cfb06a]/55 bg-white text-stone-900 hover:border-[#bc983f] hover:bg-[#fcf4df] dark:border-[#876d30] dark:bg-stone-900/75 dark:text-stone-100 dark:hover:border-[#c6a95f] dark:hover:bg-[#2f2615]",
  ghost:
    "border border-transparent bg-transparent text-stone-800 hover:border-[#d5be8a]/65 hover:bg-[#fcf7ea] dark:text-stone-100 dark:hover:border-[#7f6837] dark:hover:bg-[#2d2516]",
  danger: "border border-transparent bg-red-600 text-white shadow-sm shadow-red-700/25 hover:bg-red-700",
};

export const Button = React.forwardRef<HTMLButtonElement, ButtonProps>(
  ({ className, variant = "default", type = "button", ...props }, ref) => {
    return (
      <button
        ref={ref}
        type={type}
        className={cn(
          "inline-flex h-10 appearance-none items-center justify-center rounded-lg border border-transparent bg-clip-padding px-4 text-sm font-medium transition-all duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#c8a95f]/80 focus-visible:ring-offset-2 focus-visible:ring-offset-white active:scale-[0.98] disabled:pointer-events-none disabled:opacity-50 dark:focus-visible:ring-offset-stone-950",
          variantClassMap[variant],
          className
        )}
        {...props}
      />
    );
  }
);

Button.displayName = "Button";
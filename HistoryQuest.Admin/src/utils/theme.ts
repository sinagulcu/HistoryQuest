import { STORAGE_KEYS } from "@/utils/constants";

export type ThemeMode = "light" | "dark";

function isThemeMode(value: string | null): value is ThemeMode {
  return value === "light" || value === "dark";
}

export function getStoredTheme(): ThemeMode | null {
  const savedTheme = localStorage.getItem(STORAGE_KEYS.theme);
  if (isThemeMode(savedTheme)) {
    return savedTheme;
  }
  return null;
}

export function getSystemTheme(): ThemeMode {
  return window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
}

export function resolveInitialTheme(): ThemeMode {
  return getStoredTheme() ?? getSystemTheme();
}

export function applyThemeClass(theme: ThemeMode): void {
  document.documentElement.classList.toggle("dark", theme === "dark");
}

export function initializeTheme(): ThemeMode {
  const initialTheme = resolveInitialTheme();
  applyThemeClass(initialTheme);
  return initialTheme;
}
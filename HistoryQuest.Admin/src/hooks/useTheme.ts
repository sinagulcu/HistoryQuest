import { useCallback, useEffect, useState } from "react";
import { STORAGE_KEYS } from "@/utils/constants";
import {
  applyThemeClass,
  getStoredTheme,
  getSystemTheme,
  resolveInitialTheme,
  type ThemeMode,
} from "@/utils/theme";

export const useTheme = () => {
  const [theme, setTheme] = useState<ThemeMode>(() => resolveInitialTheme());
  const [hasManualPreference, setHasManualPreference] = useState<boolean>(() => getStoredTheme() !== null);

  useEffect(() => {
    applyThemeClass(theme);
  }, [theme]);

  useEffect(() => {
    if (!hasManualPreference) {
      localStorage.removeItem(STORAGE_KEYS.theme);
      return;
    }

    localStorage.setItem(STORAGE_KEYS.theme, theme);
  }, [hasManualPreference, theme]);

  useEffect(() => {
    if (hasManualPreference) {
      return;
    }

    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
    const handleSystemThemeChange = (event: MediaQueryListEvent) => {
      setTheme(event.matches ? "dark" : "light");
    };

    setTheme(getSystemTheme());
    mediaQuery.addEventListener("change", handleSystemThemeChange);

    return () => {
      mediaQuery.removeEventListener("change", handleSystemThemeChange);
    };
  }, [hasManualPreference]);

  const toggleTheme = useCallback(() => {
    setHasManualPreference(true);
    setTheme((prevTheme) => (prevTheme === "light" ? "dark" : "light"));
  }, []);

  return {
    theme,
    toggleTheme,
  };
};

import { Bell, Menu, Moon, PanelLeftClose, PanelLeftOpen, Sun } from "lucide-react";
import { Button } from "@/components/ui/button";
import Breadcrumb from "@/components/layout/Breadcrumb";
import { useAuth } from "@/hooks/useAuth";
import { useTheme } from "@/hooks/useTheme";
import { useNotificationStore } from "@/store/notificationStore";

interface TopbarProps {
  onOpenMobileMenu: () => void;
  onToggleDesktopSidebar: () => void;
  isDesktopSidebarCollapsed: boolean;
}

export default function Topbar({ onOpenMobileMenu, onToggleDesktopSidebar, isDesktopSidebarCollapsed }: TopbarProps) {
  const { theme, toggleTheme } = useTheme();
  const { user } = useAuth();
  const notifications = useNotificationStore((state) => state.notifications);
  const unreadCount = notifications.filter((notification) => !notification.read).length;

  return (
    <header className="sticky top-0 z-20 flex h-16 items-center justify-between border-b border-stone-200/70 bg-white/80 px-4 backdrop-blur-xl dark:border-stone-800 dark:bg-stone-950/68 lg:px-6">
      <div className="flex items-center gap-3">
        <Button variant="ghost" className="lg:hidden" onClick={onOpenMobileMenu}>
          <Menu className="h-5 w-5" />
        </Button>
        <Button
          variant="ghost"
          className="hidden lg:inline-flex"
          onClick={onToggleDesktopSidebar}
          aria-label={isDesktopSidebarCollapsed ? "Menüyü genişlet" : "Menüyü daralt"}
          title={isDesktopSidebarCollapsed ? "Menüyü genişlet" : "Menüyü daralt"}
        >
          {isDesktopSidebarCollapsed ? <PanelLeftOpen className="h-4 w-4" /> : <PanelLeftClose className="h-4 w-4" />}
        </Button>
        <Breadcrumb />
      </div>

      <div className="flex items-center gap-2">
        <Button variant="ghost" className="relative" aria-label="Bildirimler">
          <Bell className="h-4 w-4" />
          {unreadCount > 0 ? (
            <span className="absolute right-1 top-1 min-w-4 rounded-full bg-[#8f6d28] px-1 text-center text-[10px] font-semibold text-white dark:bg-[#b9974d] dark:text-stone-950">
              {unreadCount}
            </span>
          ) : null}
        </Button>
        <Button variant="ghost" onClick={toggleTheme} aria-label="Tema değiştir">
          {theme === "dark" ? <Sun className="h-4 w-4" /> : <Moon className="h-4 w-4" />}
        </Button>
        <div className="hidden rounded-lg border border-[#cfb06a]/50 bg-[#fffcf5] px-3 py-1.5 text-right shadow-sm dark:border-[#7f6835] dark:bg-[#231c12] md:block">
          <p className="text-xs font-medium text-stone-900 dark:text-stone-100">{user?.userName || "Kullanıcı"}</p>
          <p className="text-[11px] text-stone-500 dark:text-stone-400">{user?.role || "-"}</p>
        </div>
      </div>
    </header>
  );
}

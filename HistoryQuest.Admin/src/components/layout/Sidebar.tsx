import { BookOpen, CircleHelp, FolderOpen, LayoutDashboard, LogOut, Timer, UserCircle2 } from "lucide-react";
import { NavLink } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/hooks/useAuth";

interface SidebarProps {
  isMobileOpen: boolean;
  onCloseMobile: () => void;
  isDesktopCollapsed: boolean;
}

interface MenuItem {
  label: string;
  href: string;
  icon: typeof LayoutDashboard;
  adminOnly?: boolean;
}

const menuItems: MenuItem[] = [
  { label: "Dashboard", href: "/dashboard", icon: LayoutDashboard },
  { label: "Quizler", href: "/quizzes", icon: BookOpen },
  { label: "Sorular", href: "/questions", icon: CircleHelp },
  { label: "Meydan Okuma", href: "/challenges", icon: Timer },
  { label: "Kategoriler", href: "/categories", icon: FolderOpen },
  { label: "Kullanıcılar", href: "/users", icon: UserCircle2, adminOnly: true },
];

export default function Sidebar({ isMobileOpen, onCloseMobile, isDesktopCollapsed }: SidebarProps) {
  const { user, logout } = useAuth();
  const isAdmin = user?.role === "Admin";

  const onLogout = () => {
    logout();
    onCloseMobile();
  };

  return (
    <>
      <div
        onClick={onCloseMobile}
        className={`fixed inset-0 z-30 bg-black/30 transition-opacity lg:hidden ${
          isMobileOpen ? "opacity-100" : "pointer-events-none opacity-0"
        }`}
      />
        <aside
          className={`fixed left-0 top-0 z-40 flex h-screen w-72 flex-col border-r border-stone-300/70 bg-white/90 px-3 py-5 shadow-xl backdrop-blur-xl transition-[transform,width] duration-300 dark:border-stone-800 dark:bg-stone-950/82 lg:translate-x-0 ${
            isDesktopCollapsed ? "lg:w-20" : "lg:w-72"
          } ${
          isMobileOpen ? "translate-x-0" : "-translate-x-full"
        }`}
      >
        <div
          className={`mb-8 rounded-xl border border-stone-200/80 bg-white/80 p-4 shadow-sm dark:border-stone-800 dark:bg-stone-900/70 ${
            isDesktopCollapsed ? "lg:px-2 lg:text-center" : ""
          }`}
        >
          <p className="hq-gold-text text-lg font-bold tracking-tight">TP</p>
          <p className={`hq-gold-text text-sm font-semibold ${isDesktopCollapsed ? "lg:hidden" : ""}`}>Tarih Pusulası</p>
          <p className={`text-xs text-stone-600 dark:text-stone-400 ${isDesktopCollapsed ? "lg:hidden" : ""}`}>
            {isAdmin ? "Yönetici Paneli" : "Öğretmen Paneli"}
          </p>
        </div>

        <nav className="flex flex-1 flex-col gap-2">
          {menuItems
            .filter((item) => (item.adminOnly ? isAdmin : true))
            .map((item) => {
              const Icon = item.icon;
              return (
                <NavLink
                  key={item.href}
                  to={item.href}
                  onClick={onCloseMobile}
                  className={({ isActive }) =>
                    `group flex items-center rounded-md border border-transparent px-3 py-2 text-sm transition ${
                      isActive
                        ? "hq-gold-surface shadow-[0_10px_22px_-16px_rgba(120,90,24,0.9)]"
                        : "text-stone-500 hover:border-amber-300/65 hover:bg-amber-200/45 hover:text-stone-700 dark:text-stone-300 dark:hover:border-amber-400/35 dark:hover:bg-amber-300/15 dark:hover:text-stone-100"
                    } ${isDesktopCollapsed ? "lg:justify-center" : "gap-3"}`
                  }
                  title={isDesktopCollapsed ? item.label : undefined}
                >
                  {({ isActive }) => (
                    <>
                      <span
                        className={`inline-flex h-7 w-7 items-center justify-center rounded-full transition ${
                          isActive
                            ? "border border-white/25 bg-white/20 text-white"
                            : "border border-stone-300/80 text-stone-500 group-hover:border-stone-400 group-hover:text-stone-700 dark:border-stone-600 dark:text-stone-300 dark:group-hover:border-stone-500 dark:group-hover:text-stone-100"
                        }`}
                      >
                        <Icon className={`h-4 w-4 shrink-0 ${isDesktopCollapsed ? "mx-auto" : ""}`} />
                      </span>
                      <span
                        className={`${
                          isActive
                            ? "font-normal text-white"
                            : "font-light text-stone-500 group-hover:text-stone-700 dark:text-stone-300 dark:group-hover:text-stone-100"
                        } ${isDesktopCollapsed ? "lg:hidden" : ""}`}
                      >
                        {item.label}
                      </span>
                    </>
                  )}
                </NavLink>
              );
            })}
        </nav>

        <div
          className={`mt-5 rounded-xl border border-stone-200/80 bg-white/80 p-4 dark:border-stone-800 dark:bg-stone-900/70 ${
            isDesktopCollapsed ? "lg:px-2" : ""
          }`}
        >
          <p className={`text-sm font-semibold text-stone-700 dark:text-stone-200 ${isDesktopCollapsed ? "lg:hidden" : ""}`}>
            {user?.userName || "Kullanıcı"}
          </p>
          <p className={`mb-3 text-xs text-stone-500 dark:text-stone-400 ${isDesktopCollapsed ? "lg:hidden" : ""}`}>
            {user?.role || "Yetki Yok"}
          </p>
          <Button
            variant="outline"
            className={`gap-2 ${
              isDesktopCollapsed ? "mx-auto h-10 w-10 min-w-10 justify-center rounded-xl p-0" : "w-full justify-start"
            }`}
            onClick={onLogout}
            title={isDesktopCollapsed ? "Çıkış Yap" : undefined}
          >
            <LogOut className="h-5 w-5" />
            <span className={isDesktopCollapsed ? "lg:hidden" : ""}>Çıkış Yap</span>
          </Button>
        </div>
      </aside>
    </>
  );
}

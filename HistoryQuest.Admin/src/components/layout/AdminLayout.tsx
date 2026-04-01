import { useState } from "react";
import { Outlet } from "react-router-dom";
import Sidebar from "@/components/layout/Sidebar";
import Topbar from "@/components/layout/Topbar";

export default function AdminLayout() {
  const [mobileSidebarOpen, setMobileSidebarOpen] = useState(false);
  const [isDesktopSidebarCollapsed, setIsDesktopSidebarCollapsed] = useState(() => {
    const saved = localStorage.getItem("historyquest_sidebar_collapsed");
    if (saved === null) {
      return true;
    }
    return saved === "true";
  });

  const toggleDesktopSidebar = () => {
    setIsDesktopSidebarCollapsed((prev) => {
      const next = !prev;
      localStorage.setItem("historyquest_sidebar_collapsed", String(next));
      return next;
    });
  };

  return (
    <div className="min-h-screen text-stone-900 dark:text-stone-100">
      <Sidebar
        isMobileOpen={mobileSidebarOpen}
        onCloseMobile={() => setMobileSidebarOpen(false)}
        isDesktopCollapsed={isDesktopSidebarCollapsed}
      />

      <div className={isDesktopSidebarCollapsed ? "lg:pl-20" : "lg:pl-72"}>
        <Topbar
          onOpenMobileMenu={() => setMobileSidebarOpen(true)}
          onToggleDesktopSidebar={toggleDesktopSidebar}
          isDesktopSidebarCollapsed={isDesktopSidebarCollapsed}
        />
        <main className="hq-fade-in p-4 lg:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

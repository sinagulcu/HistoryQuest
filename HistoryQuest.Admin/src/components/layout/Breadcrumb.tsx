import { Link, useLocation } from "react-router-dom";

const pathLabelMap: Record<string, string> = {
  dashboard: "Dashboard",
  quizzes: "Quizler",
  questions: "Sorular",
  challenges: "Meydan Okuma",
  categories: "Kategoriler",
  users: "Kullanıcılar",
  create: "Oluştur",
  edit: "Düzenle",
  login: "Giriş",
};

function formatSegment(segment: string) {
  if (pathLabelMap[segment]) {
    return pathLabelMap[segment];
  }

  if (/^\d+$/.test(segment)) {
    return `Detay #${segment}`;
  }

  return segment.charAt(0).toUpperCase() + segment.slice(1);
}

export default function Breadcrumb() {
  const location = useLocation();
  const segments = location.pathname.split("/").filter(Boolean);

  if (segments.length === 0) {
    return <p className="text-sm text-stone-500 dark:text-stone-400">Dashboard</p>;
  }

  return (
    <nav aria-label="breadcrumb" className="flex items-center gap-2 text-sm">
      <Link to="/dashboard" className="rounded-md px-1 text-stone-500 transition hover:text-stone-700 dark:text-stone-400 dark:hover:text-stone-200">
        Dashboard
      </Link>
      {segments.map((segment, index) => {
        const path = `/${segments.slice(0, index + 1).join("/")}`;
        const isLast = index === segments.length - 1;
        return (
          <span key={path} className="flex items-center gap-2">
            <span className="text-stone-400">/</span>
            {isLast ? (
              <span className="rounded-md bg-stone-200/70 px-2 py-0.5 font-medium text-stone-800 dark:bg-stone-800 dark:text-stone-100">
                {formatSegment(segment)}
              </span>
            ) : (
              <Link
                to={path}
                className="rounded-md px-1 text-stone-500 transition hover:text-stone-700 dark:text-stone-400 dark:hover:text-stone-200"
              >
                {formatSegment(segment)}
              </Link>
            )}
          </span>
        );
      })}
    </nav>
  );
}

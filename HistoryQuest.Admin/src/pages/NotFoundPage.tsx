import { useNavigate } from "react-router-dom";
import { Button } from "@/components/ui/button";

export default function NotFoundPage() {
  const navigate = useNavigate();

  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-4 bg-white px-4 text-center dark:bg-stone-950">
      <p className="hq-gold-text text-5xl font-bold">404</p>
      <h1 className="text-xl font-semibold text-stone-900 dark:text-stone-100">Sayfa bulunamadı</h1>
      <p className="text-sm text-stone-600 dark:text-stone-400">İstediğiniz sayfa taşınmış veya silinmiş olabilir.</p>
      <Button onClick={() => navigate("/dashboard")}>Dashboard'a Dön</Button>
    </div>
  );
}

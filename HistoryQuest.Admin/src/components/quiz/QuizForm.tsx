import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Label from "@/components/ui/label";
import type { Category } from "@/types/category.types";
import { quizSchema, type QuizFormValues } from "@/utils/validators";

interface QuizFormProps {
  categories: Category[];
  initialValues?: QuizFormValues;
  isSubmitting: boolean;
  submitLabel: string;
  onSubmit: (values: QuizFormValues) => Promise<void>;
  onCancel: () => void;
}

const defaultValues: QuizFormValues = {
  title: "",
  description: "",
  categoryId: "",
  difficultyLevel: 1,
  timeLimitMinutes: 10,
  isPublished: false,
};

export default function QuizForm({
  categories,
  initialValues,
  isSubmitting,
  submitLabel,
  onSubmit,
  onCancel,
}: QuizFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<QuizFormValues>({
    resolver: zodResolver(quizSchema),
    defaultValues: initialValues || defaultValues,
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
      <div className="space-y-2">
        <Label htmlFor="title">Baslik</Label>
        <Input id="title" placeholder="Quiz basligi" {...register("title")} />
        {errors.title ? <p className="text-xs text-red-600">{errors.title.message}</p> : null}
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">Aciklama</Label>
        <textarea
          id="description"
          className="min-h-28 w-full rounded-md border border-stone-300 bg-white px-3 py-2 text-sm outline-none ring-[#b99647] focus:ring-2 dark:border-stone-700 dark:bg-stone-900"
          placeholder="Quiz aciklamasi"
          {...register("description")}
        />
        {errors.description ? <p className="text-xs text-red-600">{errors.description.message}</p> : null}
      </div>

      <div className="grid gap-4 md:grid-cols-2">
        <div className="space-y-2">
          <Label htmlFor="categoryId">Kategori</Label>
          <select
            id="categoryId"
            className="h-10 w-full rounded-md border border-stone-300 bg-white px-3 text-sm outline-none ring-[#b99647] focus:ring-2 dark:border-stone-700 dark:bg-stone-900"
            {...register("categoryId")}
          >
            <option value="">Kategori seciniz</option>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.name}
              </option>
            ))}
          </select>
          {errors.categoryId ? <p className="text-xs text-red-600">{errors.categoryId.message}</p> : null}
        </div>

        <div className="space-y-2">
          <Label htmlFor="difficultyLevel">Zorluk</Label>
          <select
            id="difficultyLevel"
            className="h-10 w-full rounded-md border border-stone-300 bg-white px-3 text-sm outline-none ring-[#b99647] focus:ring-2 dark:border-stone-700 dark:bg-stone-900"
            {...register("difficultyLevel", { valueAsNumber: true })}
          >
            <option value={1}>Kolay</option>
            <option value={2}>Orta</option>
            <option value={3}>Zor</option>
          </select>
        </div>

        <div className="space-y-2">
          <Label htmlFor="timeLimitMinutes">Sure Limiti (dakika)</Label>
          <Input id="timeLimitMinutes" type="number" min={1} {...register("timeLimitMinutes", { valueAsNumber: true })} />
          {errors.timeLimitMinutes ? <p className="text-xs text-red-600">{errors.timeLimitMinutes.message}</p> : null}
        </div>

        <div className="flex items-center gap-2 pt-7">
          <input id="isPublished" type="checkbox" className="h-4 w-4" {...register("isPublished")} />
          <Label htmlFor="isPublished">Yayin durumunda baslat</Label>
        </div>
      </div>

      <div className="flex gap-2">
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Kaydediliyor..." : submitLabel}
        </Button>
        <Button type="button" variant="outline" onClick={onCancel}>
          Iptal
        </Button>
      </div>
    </form>
  );
}

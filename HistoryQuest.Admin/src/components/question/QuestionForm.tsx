import { zodResolver } from "@hookform/resolvers/zod";
import { Plus, Trash2 } from "lucide-react";
import { useMemo } from "react";
import { useFieldArray, useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import Label from "@/components/ui/label";
import type { Category } from "@/types/category.types";
import type { QuestionFormValues } from "@/utils/validators";
import { questionSchema } from "@/utils/validators";

interface QuestionFormProps {
  categories: Category[];
  initialValues?: QuestionFormValues;
  isSubmitting: boolean;
  submitLabel: string;
  onSubmit: (values: QuestionFormValues) => Promise<void>;
  onCancel: () => void;
}

const emptyOption = { text: "", isCorrect: false };

export default function QuestionForm({
  categories,
  initialValues,
  isSubmitting,
  submitLabel,
  onSubmit,
  onCancel,
}: QuestionFormProps) {
  const fallbackValues = useMemo<QuestionFormValues>(
    () => ({
      text: "",
      categoryId: "",
      difficultyLevel: 1,
      options: [emptyOption, { ...emptyOption, isCorrect: true }],
    }),
    []
  );

  const {
    register,
    handleSubmit,
    control,
    setValue,
    watch,
    formState: { errors },
  } = useForm<QuestionFormValues>({
    resolver: zodResolver(questionSchema),
    defaultValues: initialValues || fallbackValues,
  });

  const { fields, append, remove } = useFieldArray({ control, name: "options" });
  const options = watch("options");

  const setCorrectOption = (index: number) => {
    options.forEach((_, optionIndex) => {
      setValue(`options.${optionIndex}.isCorrect`, optionIndex === index, { shouldValidate: true });
    });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="space-y-2">
        <Label htmlFor="text">Soru Metni</Label>
        <textarea
          id="text"
          className="min-h-28 w-full rounded-md border border-stone-300 bg-white px-3 py-2 text-sm outline-none ring-[#b99647] focus:ring-2 dark:border-stone-700 dark:bg-stone-900"
          placeholder="Soru metnini yaziniz"
          {...register("text")}
        />
        {errors.text ? <p className="text-xs text-red-600">{errors.text.message}</p> : null}
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
          <Label htmlFor="difficultyLevel">Zorluk Seviyesi</Label>
          <select
            id="difficultyLevel"
            className="h-10 w-full rounded-md border border-stone-300 bg-white px-3 text-sm outline-none ring-[#b99647] focus:ring-2 dark:border-stone-700 dark:bg-stone-900"
            {...register("difficultyLevel", { valueAsNumber: true })}
          >
            <option value={1}>Kolay</option>
            <option value={2}>Orta</option>
            <option value={3}>Zor</option>
          </select>
          {errors.difficultyLevel ? <p className="text-xs text-red-600">{errors.difficultyLevel.message}</p> : null}
        </div>
      </div>

      <div className="space-y-3 rounded-md border border-stone-200 p-4 dark:border-stone-800">
        <div className="flex items-center justify-between">
          <h2 className="font-semibold text-stone-900 dark:text-stone-100">Secenekler</h2>
          <Button
            type="button"
            variant="outline"
            className="gap-2"
            onClick={() => append({ ...emptyOption })}
            disabled={fields.length >= 6}
          >
            <Plus className="h-4 w-4" />
            Secenek Ekle
          </Button>
        </div>

        {fields.map((field, index) => (
          <div key={field.id} className="grid gap-2 md:grid-cols-[1fr_auto_auto] md:items-center">
            <Input placeholder={`Secenek ${index + 1}`} {...register(`options.${index}.text`)} />
            <label className="inline-flex items-center gap-2 text-sm text-stone-700 dark:text-stone-200">
              <input
                type="radio"
                name="correctOption"
                checked={options[index]?.isCorrect || false}
                onChange={() => setCorrectOption(index)}
              />
              Dogru
            </label>
            <Button type="button" variant="danger" onClick={() => remove(index)} disabled={fields.length <= 2}>
              <Trash2 className="h-4 w-4" />
            </Button>
            {errors.options?.[index]?.text ? <p className="text-xs text-red-600">{errors.options[index]?.text?.message}</p> : null}
          </div>
        ))}

        {errors.options?.message ? <p className="text-xs text-red-600">{errors.options.message}</p> : null}
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
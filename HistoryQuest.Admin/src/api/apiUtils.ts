export function unwrapApiData<T>(payload: unknown): T {
  if (payload && typeof payload === "object") {
    const record = payload as Record<string, unknown>;

    if ("data" in record) {
      return record.data as T;
    }

    if ("result" in record) {
      return record.result as T;
    }
  }

  return payload as T;
}

export function toDifficultyLevel(value: unknown): number {
  if (typeof value === "number" && [1, 2, 3].includes(value)) {
    return value;
  }

  if (typeof value === "string") {
    const normalized = value.toLowerCase();
    if (normalized === "easy") {
      return 1;
    }
    if (normalized === "medium") {
      return 2;
    }
    if (normalized === "hard") {
      return 3;
    }
  }

  return 1;
}

export function toDifficultyName(level: number): "Easy" | "Medium" | "Hard" {
  if (level === 2) {
    return "Medium";
  }
  if (level === 3) {
    return "Hard";
  }
  return "Easy";
}

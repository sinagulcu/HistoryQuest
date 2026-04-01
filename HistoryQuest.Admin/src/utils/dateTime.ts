import { format } from "date-fns";

const TIMEZONE_SUFFIX_REGEX = /([zZ]|[+-]\d{2}:\d{2})$/;

export function hasExplicitTimezone(value: string) {
  return TIMEZONE_SUFFIX_REGEX.test(value);
}

export function parseServerDate(value?: string | null): Date | null {
  if (!value) {
    return null;
  }

  const normalized = hasExplicitTimezone(value) ? value : `${value}Z`;
  const parsed = new Date(normalized);
  if (Number.isNaN(parsed.getTime())) {
    return null;
  }

  return parsed;
}

export function normalizeServerDateString(value?: string | null): string | undefined {
  const parsed = parseServerDate(value);
  return parsed ? parsed.toISOString() : undefined;
}

export function formatServerDateTime(value?: string | null, datePattern = "dd.MM.yyyy HH:mm") {
  const parsed = parseServerDate(value);
  if (!parsed) {
    return "-";
  }

  return format(parsed, datePattern);
}

export function getServerTimestamp(value?: string | null, fallback = 0) {
  const parsed = parseServerDate(value);
  return parsed ? parsed.getTime() : fallback;
}

export function toLocalDateTimeInputValue(value?: string | null) {
  const parsed = parseServerDate(value);
  if (!parsed) {
    return "";
  }

  const timezoneOffset = parsed.getTimezoneOffset() * 60000;
  const local = new Date(parsed.getTime() - timezoneOffset);
  return local.toISOString().slice(0, 16);
}

export function localDateTimeInputToUtcIso(localDateTimeValue: string) {
  const parsed = new Date(localDateTimeValue);
  if (Number.isNaN(parsed.getTime())) {
    return new Date().toISOString();
  }

  return parsed.toISOString();
}
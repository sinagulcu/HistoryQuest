import type { Question } from "@/types/question.types";

export type ChallengeStatus = "Scheduled" | "Active" | "Expired";

export interface Challenge {
  id: string;
  title: string;
  questionId: string;
  questionText?: string;
  question?: Question;
  scheduledAtUtc: string;
  answerWindowSeconds: number;
  visibilityWindowSeconds: number;
  maxScore: number;
  showCorrectAnswerOnWrong: boolean;
  showExplanationOnWrong: boolean;
  notifyAllStudents: boolean;
  status: ChallengeStatus;
  createdByTeacherId: string;
  createdByTeacherName?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface ChallengeCreateDto {
  title: string;
  questionId: string;
  scheduledAt: string;
  scoringDurationMinutes: number;
  lateDurationMinutes: number;
  maxScore: number;
  showCorrectAnswerOnWrong: boolean;
  showExplanationOnWrong: boolean;
  notifyAllStudents: boolean;
}

export interface ChallengeUpdateDto extends ChallengeCreateDto {}

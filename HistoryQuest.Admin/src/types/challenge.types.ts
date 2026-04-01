import type { Question } from "@/types/question.types";

export type ChallengeStatus = "Draft" | "Scheduled" | "Active" | "Completed" | "Cancelled";

export interface Challenge {
  id: string;
  title: string;
  questionId: string;
  questionText?: string;
  question?: Question;
  scheduledAt: string;
  scoringDurationMinutes: number;
  lateDurationMinutes: number;
  maxScore: number;
  showCorrectAnswerOnWrong: boolean;
  showExplanationOnWrong: boolean;
  explanation?: string;
  notifyAllStudents: boolean;
  status?: ChallengeStatus;
  createdByUserId: string;
  createdByUserName?: string;
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
  explanation?: string;
  notifyAllStudents: boolean;
}

export interface ChallengeUpdateDto extends ChallengeCreateDto {}

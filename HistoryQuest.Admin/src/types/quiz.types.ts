export interface Quiz {
  id: string;
  title: string;
  description: string;
  categoryId: number;
  categoryName?: string;
  difficultyLevel: number;
  timeLimitMinutes: number;
  isPublished: boolean;
  createdByUserId: string;
  createdByUserName?: string;
  createdAt?: string;
  questions?: QuizQuestion[];
}

export interface QuizQuestionOption {
  id?: string;
  text: string;
  isCorrect: boolean;
}

export interface QuizQuestion {
  id: string;
  text: string;
  categoryId: number;
  categoryName?: string;
  difficultyLevel: number;
  createdByUserId: string;
  createdByUserName?: string;
  options?: QuizQuestionOption[];
}

export interface QuizCreateDto {
  title: string;
  description: string;
  categoryId: number;
  difficultyLevel: number;
  timeLimitMinutes: number;
  isPublished: boolean;
}

export interface QuizUpdateDto {
  title: string;
  description: string;
  categoryId: number;
  difficultyLevel: number;
  timeLimitMinutes: number;
  isPublished: boolean;
}
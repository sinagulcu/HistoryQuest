export interface Question {
  id: string;
  text: string;
  categoryId: number;
  categoryName?: string;
  difficultyLevel: number;
  createdByUserId: string;
  createdByUserName?: string;
  createdAt?: string;
  options?: QuestionOption[];
}

export interface QuestionOption {
  id?: string;
  text: string;
  isCorrect: boolean;
}

export interface QuestionCreateDto {
  text: string;
  categoryId: number;
  difficultyLevel: number;
  options: QuestionOption[];
}

export interface QuestionUpdateDto {
  text: string;
  categoryId: number;
  difficultyLevel: number;
  options: QuestionOption[];
}
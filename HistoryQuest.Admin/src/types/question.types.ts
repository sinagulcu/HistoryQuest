export interface Question {
  id: string;
  text: string;
  categoryId: string;
  categoryName?: string;
  difficultyLevel: number;
  createdByUserId: string;
  createdByUserName?: string;
  createdByUserFullName?: string;
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
  categoryId: string;
  difficultyLevel: number;
  options: QuestionOption[];
}

export interface QuestionUpdateDto {
  text: string;
  categoryId: string;
  difficultyLevel: number;
  options: QuestionOption[];
}
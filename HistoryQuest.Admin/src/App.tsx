import { Navigate, Route, Routes } from "react-router-dom";
import { Toaster } from "sonner";
import AdminLayout from "@/components/layout/AdminLayout";
import AuthGuard from "@/guards/AuthGuard";
import RoleGuard from "@/guards/RoleGuard";
import StudentBlockGuard from "@/guards/StudentBlockGuard";
import LoginPage from "@/pages/auth/LoginPage";
import CategoryListPage from "@/pages/categories/CategoryListPage";
import ChallengeCreatePage from "@/pages/challenges/ChallengeCreatePage";
import ChallengeEditPage from "@/pages/challenges/ChallengeEditPage";
import ChallengeListPage from "@/pages/challenges/ChallengeListPage";
import DashboardPage from "@/pages/dashboard/DashboardPage";
import NotFoundPage from "@/pages/NotFoundPage";
import QuestionCreatePage from "@/pages/questions/QuestionCreatePage";
import QuestionEditPage from "@/pages/questions/QuestionEditPage";
import QuestionListPage from "@/pages/questions/QuestionListPage";
import QuizCreatePage from "@/pages/quizzes/QuizCreatePage";
import QuizDetailPage from "@/pages/quizzes/QuizDetailPage";
import QuizEditPage from "@/pages/quizzes/QuizEditPage";
import QuizListPage from "@/pages/quizzes/QuizListPage";
import UserListPage from "@/pages/users/UserListPage";

export default function App() {
  return (
    <>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route element={<AuthGuard />}>
          <Route element={<StudentBlockGuard />}>
            <Route element={<AdminLayout />}>
              <Route path="/" element={<Navigate to="/dashboard" replace />} />
              <Route path="/dashboard" element={<DashboardPage />} />

              <Route path="/quizzes" element={<QuizListPage />} />
              <Route path="/quizzes/create" element={<QuizCreatePage />} />
              <Route path="/quizzes/:id" element={<QuizDetailPage />} />
              <Route path="/quizzes/:id/edit" element={<QuizEditPage />} />

              <Route path="/questions" element={<QuestionListPage />} />
              <Route path="/questions/create" element={<QuestionCreatePage />} />
              <Route path="/questions/:id/edit" element={<QuestionEditPage />} />

              <Route path="/categories" element={<CategoryListPage />} />

              <Route path="/challenges" element={<ChallengeListPage />} />
              <Route path="/challenges/create" element={<ChallengeCreatePage />} />
              <Route path="/challenges/:id/edit" element={<ChallengeEditPage />} />

              <Route element={<RoleGuard allowedRoles={["Admin"]} />}>
                <Route path="/users" element={<UserListPage />} />
              </Route>
            </Route>
          </Route>
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Routes>
      <Toaster richColors position="top-right" />
    </>
  );
}

import { Route, Routes } from 'react-router-dom'
import DashboardPage from './pages/DashboardPage'
import ClaimDetailPage from './pages/ClaimDetailPage'
import NewClaimPage from './pages/NewClaimPage'
import HomePage from './pages/HomePage'
import LoginPage from './pages/LoginPage'
import NotFoundPage from './pages/NotFoundPage'
import ProtectedRoute from './routes/ProtectedRoute'

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/login" element={<LoginPage />} />

      <Route element={<ProtectedRoute />}>
        <Route
          path="/dashboard"
          element={<DashboardPage />}
        />
        <Route path="/claims/:claimId" element={<ClaimDetailPage />} />
        <Route path="/claims/new" element={<NewClaimPage />} />
      </Route>

      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}

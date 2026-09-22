import { lazy, Suspense } from 'react'
import { Route, Routes } from 'react-router-dom'
import ProtectedRoute from './routes/ProtectedRoute'

const DashboardPage = lazy(() => import('./pages/DashboardPage'))
const ClaimDetailPage = lazy(() => import('./pages/ClaimDetailPage'))
const NewClaimPage = lazy(() => import('./pages/NewClaimPage'))
const HomePage = lazy(() => import('./pages/HomePage'))
const LoginPage = lazy(() => import('./pages/LoginPage'))
const NotFoundPage = lazy(() => import('./pages/NotFoundPage'))

export default function App() {
  return (
    <Suspense fallback={<div className="route-loader" role="status"><span />Loading workspace…</div>}>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />

        <Route element={<ProtectedRoute />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/claims/:claimId" element={<ClaimDetailPage />} />
          <Route path="/claims/new" element={<NewClaimPage />} />
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </Suspense>
  )
}

import { useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/useAuth'

export default function DashboardPage() {
  const { session, signOut } = useAuth()
  const navigate = useNavigate()

  function handleSignOut() {
    signOut()
    navigate('/login', { replace: true })
  }

  return (
    <main className="page">
      <section className="card">
        <p className="eyebrow">Claims Management</p>
        <h1>Dashboard</h1>

        <p>
          Signed in as <strong>{session?.email}</strong>
        </p>

        <p>
          Role: <strong>{session?.role}</strong>
        </p>

        <button type="button" onClick={handleSignOut}>
          Sign out
        </button>
      </section>
    </main>
  )
}

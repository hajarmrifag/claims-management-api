import { useState, type ReactNode } from 'react'
import { NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../auth/useAuth'
import Brand from './Brand'
import Icon from './Icon'

interface AppShellProps {
  children: ReactNode
  title: string
  description?: string
  actions?: ReactNode
}

function initials(email?: string) {
  return email?.slice(0, 2).toUpperCase() ?? 'AC'
}

export default function AppShell({ children, title, description, actions }: AppShellProps) {
  const { session, signOut } = useAuth()
  const navigate = useNavigate()
  const [menuOpen, setMenuOpen] = useState(false)

  function handleSignOut() {
    signOut()
    navigate('/login', { replace: true })
  }

  return (
    <div className="app-shell">
      <a className="skip-link" href="#main-content">Skip to content</a>
      <aside className={`app-sidebar ${menuOpen ? 'app-sidebar--open' : ''}`}>
        <div className="sidebar-topline">
          <Brand to="/dashboard" />
          <button className="icon-button sidebar-close" type="button" onClick={() => setMenuOpen(false)} aria-label="Close navigation">
            <Icon name="x" />
          </button>
        </div>

        <nav className="app-nav" aria-label="Primary navigation">
          <p className="nav-label">Workspace</p>
          <NavLink to="/dashboard" onClick={() => setMenuOpen(false)}>
            <Icon name="grid" />
            All claims
          </NavLink>
          <NavLink to="/claims/new" onClick={() => setMenuOpen(false)}>
            <Icon name="plus" />
            New claim
          </NavLink>
        </nav>

        <div className="sidebar-assurance">
          <Icon name="shield" />
          <div>
            <strong>Protected workspace</strong>
            <span>Role-based access enabled</span>
          </div>
        </div>

        <div className="sidebar-user">
          <span className="user-avatar">{initials(session?.email)}</span>
          <div>
            <strong title={session?.email}>{session?.email}</strong>
            <span>{session?.role}</span>
          </div>
          <button className="icon-button" type="button" onClick={handleSignOut} aria-label="Sign out" title="Sign out">
            <Icon name="logout" />
          </button>
        </div>
      </aside>

      {menuOpen && <button className="sidebar-scrim" type="button" onClick={() => setMenuOpen(false)} aria-label="Close navigation" />}

      <div className="app-main">
        <header className="app-topbar">
          <button className="icon-button mobile-menu" type="button" onClick={() => setMenuOpen(true)} aria-label="Open navigation">
            <Icon name="menu" />
          </button>
          <div className="topbar-title">
            <h1>{title}</h1>
            {description && <p>{description}</p>}
          </div>
          {actions && <div className="topbar-actions">{actions}</div>}
        </header>
        <main id="main-content" className="app-content">{children}</main>
      </div>
    </div>
  )
}

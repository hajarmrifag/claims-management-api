import type { AuthSession } from './types'

const AUTH_SESSION_KEY = 'claims-auth-session'

export function saveAuthSession(session: AuthSession): void {
  sessionStorage.setItem(AUTH_SESSION_KEY, JSON.stringify(session))
}

export function loadAuthSession(): AuthSession | null {
  const storedSession = sessionStorage.getItem(AUTH_SESSION_KEY)

  if (!storedSession) {
    return null
  }

  try {
    const session = JSON.parse(storedSession) as AuthSession

    if (new Date(session.expiresAt).getTime() <= Date.now()) {
      clearAuthSession()
      return null
    }

    return session
  } catch {
    clearAuthSession()
    return null
  }
}

export function clearAuthSession(): void {
  sessionStorage.removeItem(AUTH_SESSION_KEY)
}

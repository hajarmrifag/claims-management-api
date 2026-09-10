import { useMemo, useState, type ReactNode } from 'react'
import { AuthContext } from './authContext'
import {
  clearAuthSession,
  loadAuthSession,
  saveAuthSession,
} from './session'
import type { AuthSession } from './types'

interface AuthProviderProps {
  children: ReactNode
}

export default function AuthProvider({
  children,
}: AuthProviderProps) {
  const [session, setSession] = useState<AuthSession | null>(() =>
    loadAuthSession(),
  )

  const value = useMemo(
    () => ({
      session,
      isAuthenticated: session !== null,

      signIn: (newSession: AuthSession) => {
        saveAuthSession(newSession)
        setSession(newSession)
      },

      signOut: () => {
        clearAuthSession()
        setSession(null)
      },
    }),
    [session],
  )

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  )
}

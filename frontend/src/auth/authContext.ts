import { createContext } from 'react'
import type { AuthSession } from './types'

export interface AuthContextValue {
  session: AuthSession | null
  isAuthenticated: boolean
  signIn: (session: AuthSession) => void
  signOut: () => void
}

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined,
)

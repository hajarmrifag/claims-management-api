export type UserRole = 'Adjuster' | 'Manager' | 'Admin'

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
}

export interface AuthSession {
  token: string
  expiresAt: string
  email: string
  role: UserRole
}

import { apiClient } from './client'
import type { AuthSession, LoginRequest, RegisterRequest } from '../auth/types'

export async function login(
  credentials: LoginRequest,
): Promise<AuthSession> {
  const response = await apiClient.post<AuthSession>(
    '/api/auth/login',
    credentials,
  )

  return response.data
}

export async function registerAccount(details: RegisterRequest): Promise<AuthSession> {
  const response = await apiClient.post<AuthSession>('/api/auth/register', details)
  return response.data
}

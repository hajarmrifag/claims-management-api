import { apiClient } from './client'
import type { AuthSession, LoginRequest } from '../auth/types'

export async function login(
  credentials: LoginRequest,
): Promise<AuthSession> {
  const response = await apiClient.post<AuthSession>(
    '/api/auth/login',
    credentials,
  )

  return response.data
}

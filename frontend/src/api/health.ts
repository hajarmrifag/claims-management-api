import { apiClient } from './client'

export async function getHealth(): Promise<string> {
  const response = await apiClient.get<string>('/health')
  return response.data
}

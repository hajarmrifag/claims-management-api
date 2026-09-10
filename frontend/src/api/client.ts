import axios from 'axios'
import { loadAuthSession } from '../auth/session'

const baseURL = import.meta.env.VITE_API_URL

if (!baseURL) {
  throw new Error('VITE_API_URL is not configured')
}

export const apiClient = axios.create({
  baseURL,
  headers: {
    'Content-Type': 'application/json',
  },
})

apiClient.interceptors.request.use((config) => {
  const session = loadAuthSession()

  if (session) {
    config.headers.Authorization = `Bearer ${session.token}`
  }

  return config
})

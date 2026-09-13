import react from '@vitejs/plugin-react'
import { defineConfig } from 'vitest/config'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    include: ['src/**/*.test.{ts,tsx}'],
    setupFiles: './src/test/setup.ts',
  },
  server: {
    allowedHosts: true,
    proxy: {
      '/api': 'http://localhost:5293',
      '/health': 'http://localhost:5293',
    },
  },
})

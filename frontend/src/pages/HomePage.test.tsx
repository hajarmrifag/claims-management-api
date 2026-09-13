import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { cleanup, render, screen } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { getHealth } from '../api/health'
import HomePage from './HomePage'

vi.mock('../api/health', () => ({ getHealth: vi.fn() }))

function renderPage() {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  })

  return render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter>
        <HomePage />
      </MemoryRouter>
    </QueryClientProvider>,
  )
}

describe('HomePage', () => {
  beforeEach(() => vi.mocked(getHealth).mockReset())
  afterEach(() => cleanup())

  it('shows the product entry point and healthy API state', async () => {
    vi.mocked(getHealth).mockResolvedValue('Healthy')
    renderPage()

    expect(screen.getByRole('heading', {
      name: 'Insurance claims, managed clearly.',
    })).toBeInTheDocument()
    expect(screen.getByRole('link', {
      name: 'Open claims workspace',
    })).toHaveAttribute('href', '/login')
    expect(await screen.findByText('API operational')).toBeInTheDocument()
  })

  it('announces the API connection while the request is pending', () => {
    vi.mocked(getHealth).mockImplementation(
      () => new Promise<string>((resolve) => {
        setTimeout(() => resolve('Healthy'), 50)
      }),
    )
    renderPage()

    expect(screen.getByText('Checking API connection...')).toBeInTheDocument()
  })
})

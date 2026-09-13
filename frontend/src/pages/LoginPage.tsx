import { zodResolver } from '@hookform/resolvers/zod'
import { useMutation } from '@tanstack/react-query'
import axios from 'axios'
import { useForm } from 'react-hook-form'
import { useState } from 'react'
import {
  Navigate,
  useLocation,
  useNavigate,
} from 'react-router-dom'
import { z } from 'zod'
import { login, registerAccount } from '../api/auth'
import { useAuth } from '../auth/useAuth'

const loginSchema = z.object({
  email: z
    .string()
    .min(1, 'Email is required')
    .email('Enter a valid email address'),

  password: z
    .string()
    .min(1, 'Password is required'),
})

type LoginFormValues = z.infer<typeof loginSchema>

interface ApiError {
  status: number
  error: string
}

export default function LoginPage() {
  const [isRegistering, setIsRegistering] = useState(false)
  const { signIn, isAuthenticated } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
  })

  const mutation = useMutation({
    mutationFn: (values: LoginFormValues) =>
      isRegistering ? registerAccount(values) : login(values),

    onSuccess: (session) => {
      signIn(session)

      const destination =
        (
          location.state as
            | { from?: { pathname?: string } }
            | null
        )?.from?.pathname ?? '/dashboard'

      navigate(destination, { replace: true })
    },
  })

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />
  }

  const serverError =
    axios.isAxiosError<ApiError>(mutation.error)
      ? mutation.error.response?.data?.error
      : mutation.isError
        ? 'Unable to sign in. Please try again.'
        : null

  return (
    <main className="page">
      <section className="card auth-card">
        <p className="eyebrow">Claims Management</p>

        <h1>{isRegistering ? 'Create account' : 'Sign in'}</h1>

        <p>
          {isRegistering
            ? 'Create an adjuster account to access the workspace.'
            : 'Access the claims workspace using your account.'}
        </p>

        <form
          className="auth-form"
          onSubmit={handleSubmit((values) =>
            mutation.mutate(values),
          )}
          noValidate
        >
          <div className="field">
            <label htmlFor="email">Email</label>

            <input
              id="email"
              type="email"
              autoComplete="email"
              aria-invalid={Boolean(errors.email)}
              {...register('email')}
            />

            {errors.email && (
              <p className="field-error">
                {errors.email.message}
              </p>
            )}
          </div>

          <div className="field">
            <label htmlFor="password">Password</label>

            <input
              id="password"
              type="password"
              autoComplete="current-password"
              aria-invalid={Boolean(errors.password)}
              {...register('password')}
            />

            {errors.password && (
              <p className="field-error">
                {errors.password.message}
              </p>
            )}
          </div>

          {serverError && (
            <div className="form-error" role="alert">
              {serverError}
            </div>
          )}

          <button
            className="primary-button"
            type="submit"
            disabled={mutation.isPending}
          >
            {mutation.isPending
              ? 'Please wait...'
              : isRegistering ? 'Create account' : 'Sign in'}
          </button>
        </form>
        <button
          className="text-button"
          type="button"
          onClick={() => { mutation.reset(); setIsRegistering((value) => !value) }}
        >
          {isRegistering ? 'Already have an account? Sign in' : 'Need an account? Register'}
        </button>
      </section>
    </main>
  )
}

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
import Brand from '../components/Brand'
import Icon from '../components/Icon'

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
    <main className="auth-page">
      <section className="auth-story">
        <Brand />
        <div className="auth-story-copy">
          <span className="auth-quote-mark">“</span>
          <blockquote>Every claim tells a story. Give your team the clarity to resolve it well.</blockquote>
          <p>A secure, focused workspace for modern claims operations.</p>
        </div>
        <div className="auth-assurance"><Icon name="shield" /><span><strong>Enterprise-grade protection</strong><small>Authentication, authorization, and auditability built in.</small></span></div>
      </section>

      <section className="auth-panel">
        <div className="auth-mobile-brand"><Brand /></div>
        <div className="auth-card">
          <div className="auth-heading">
            <p className="eyebrow">{isRegistering ? 'Join the workspace' : 'Welcome back'}</p>
            <h1>{isRegistering ? 'Create your account' : 'Sign in to Aegis'}</h1>
            <p>{isRegistering ? 'Create an adjuster account and start managing claims.' : 'Enter your details to access your claims workspace.'}</p>
          </div>

          <form className="auth-form" onSubmit={handleSubmit((values) => mutation.mutate(values))} noValidate>
          <div className="field">
            <label htmlFor="email">Email</label>
            <div className="input-with-icon"><Icon name="user" size={18} /><input id="email" type="email" autoComplete="email" placeholder="you@company.com" aria-invalid={Boolean(errors.email)} {...register('email')} /></div>

            {errors.email && (
              <p className="field-error">
                {errors.email.message}
              </p>
            )}
          </div>

          <div className="field">
            <label htmlFor="password">Password</label>
            <div className="input-with-icon"><Icon name="lock" size={18} /><input id="password" type="password" autoComplete={isRegistering ? 'new-password' : 'current-password'} placeholder="Enter your password" aria-invalid={Boolean(errors.password)} {...register('password')} /></div>

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
            className="primary-button auth-submit"
            type="submit"
            disabled={mutation.isPending}
          >
            <span>{mutation.isPending ? 'Please wait…' : isRegistering ? 'Create account' : 'Sign in'}</span>
            {!mutation.isPending && <Icon name="arrow-right" size={18} />}
          </button>
          </form>
          <div className="auth-switch">
            <span>{isRegistering ? 'Already have an account?' : 'New to Aegis?'}</span>
            <button className="text-button" type="button" onClick={() => { mutation.reset(); setIsRegistering((value) => !value) }}>
              {isRegistering ? 'Sign in' : 'Create an account'}
            </button>
          </div>
          <p className="auth-legal"><Icon name="lock" size={13} /> Your session is encrypted and securely managed.</p>
        </div>
      </section>
    </main>
  )
}

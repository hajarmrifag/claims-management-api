import { Link } from 'react-router-dom'

interface BrandProps {
  compact?: boolean
  to?: string
}

export default function Brand({ compact = false, to = '/' }: BrandProps) {
  return (
    <Link className="brand" to={to} aria-label="Aegis Claims home">
      <span className="brand-mark" aria-hidden="true">
        <svg viewBox="0 0 32 32">
          <path d="M16 3.5 27 8v7.2c0 6.3-4.2 11.1-11 13.3C9.2 26.3 5 21.5 5 15.2V8z" />
          <path d="m10.5 16 3.3 3.3 7.8-8" />
        </svg>
      </span>
      {!compact && (
        <span className="brand-copy">
          <strong>Aegis</strong>
          <small>Claims</small>
        </span>
      )}
    </Link>
  )
}

import { useQuery } from '@tanstack/react-query'
import { getHealth } from '../api/health'

export default function ApiStatus() {
  const { isPending, isError, refetch } = useQuery({
    queryKey: ['api-health'],
    queryFn: getHealth,
  })

  if (isPending) {
    return (
      <div className="api-status" aria-live="polite">
        <span className="status-dot status-dot--checking" />
        Checking API connection...
      </div>
    )
  }

  if (isError) {
    return (
      <div className="api-status api-status--error" aria-live="polite">
        <span className="status-dot status-dot--error" />
        <span>API unavailable</span>
        <button type="button" onClick={() => refetch()}>
          Retry
        </button>
      </div>
    )
  }

  return (
    <div className="api-status api-status--success" aria-live="polite">
      <span className="status-dot status-dot--success" />
      API operational
    </div>
  )
}

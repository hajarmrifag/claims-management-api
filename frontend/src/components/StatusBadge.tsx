import type { ClaimStatus } from '../claims/types'

const labels: Record<ClaimStatus, string> = {
  Submitted: 'Submitted',
  UnderReview: 'Under review',
  Approved: 'Approved',
  Rejected: 'Rejected',
  Paid: 'Paid',
}

export default function StatusBadge({ status }: { status: ClaimStatus }) {
  return (
    <span className={`status-badge status-${status.toLowerCase()}`}>
      <span className="status-badge-dot" />
      {labels[status]}
    </span>
  )
}

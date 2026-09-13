import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getClaim, updateClaimStatus, uploadClaimDocument } from '../api/claims'
import { useAuth } from '../auth/useAuth'
import { CLAIM_STATUSES, type ClaimStatus } from '../claims/types'

function formatAmount(amount: number) {
  return new Intl.NumberFormat('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(amount)
}

export default function ClaimDetailPage() {
  const { claimId = '' } = useParams()
  const { session } = useAuth()
  const queryClient = useQueryClient()
  const [selectedStatus, setSelectedStatus] = useState<ClaimStatus | ''>('')
  const claimQuery = useQuery({ queryKey: ['claim', claimId], queryFn: () => getClaim(claimId), enabled: Boolean(claimId) })
  const statusMutation = useMutation({
    mutationFn: (status: ClaimStatus) => updateClaimStatus(claimId, status),
    onSuccess: (claim) => {
      queryClient.setQueryData(['claim', claimId], claim)
      queryClient.invalidateQueries({ queryKey: ['claims'] })
      setSelectedStatus('')
    },
  })
  const documentMutation = useMutation({
    mutationFn: (file: File) => uploadClaimDocument(claimId, file),
  })
  const canChangeStatus = session?.role === 'Manager' || session?.role === 'Admin'

  if (claimQuery.isLoading) return <main className="detail-shell table-state">Loading claim…</main>

  if (claimQuery.isError || !claimQuery.data) {
    return (
      <main className="detail-shell">
        <Link className="back-link" to="/dashboard">← Back to claims</Link>
        <section className="detail-card table-state table-state-error">
          Unable to load this claim.
          <button type="button" onClick={() => claimQuery.refetch()}>Retry</button>
        </section>
      </main>
    )
  }

  const claim = claimQuery.data
  return (
    <main className="detail-shell">
      <Link className="back-link" to="/dashboard">← Back to claims</Link>
      <header className="detail-header">
        <div><p className="eyebrow">Claim record</p><h1>{claim.claimNumber}</h1></div>
        <span className={`status-badge status-${claim.status.toLowerCase()}`}>{claim.status}</span>
      </header>
      <section className="detail-card">
        <div className="detail-card-header"><h2>Claim details</h2><strong>{formatAmount(claim.amount)}</strong></div>
        <dl className="detail-grid">
          <div className="detail-description"><dt>Description</dt><dd>{claim.description}</dd></div>
          <div><dt>Submitted</dt><dd>{new Date(claim.submittedAt).toLocaleString()}</dd></div>
          <div><dt>Customer ID</dt><dd>{claim.customerId}</dd></div>
          <div><dt>Policy ID</dt><dd>{claim.policyId}</dd></div>
          <div><dt>Claim ID</dt><dd>{claim.id}</dd></div>
        </dl>
        {canChangeStatus && (
          <form className="status-form" onSubmit={(event) => { event.preventDefault(); if (selectedStatus) statusMutation.mutate(selectedStatus) }}>
            <label>Update status
              <select value={selectedStatus} onChange={(event) => setSelectedStatus(event.target.value as ClaimStatus)}>
                <option value="">Choose status</option>
                {CLAIM_STATUSES.filter((status) => status !== claim.status).map((status) => <option key={status} value={status}>{status}</option>)}
              </select>
            </label>
            <button className="primary-button" type="submit" disabled={!selectedStatus || statusMutation.isPending}>
              {statusMutation.isPending ? 'Saving…' : 'Save status'}
            </button>
            {statusMutation.isError && <span className="field-error" role="alert">Unable to update status.</span>}
          </form>
        )}
        <form
          className="document-form"
          onSubmit={(event) => {
            event.preventDefault()
            const input = event.currentTarget.elements.namedItem('document') as HTMLInputElement
            if (input.files?.[0]) documentMutation.mutate(input.files[0])
          }}
        >
          <div><h2>Supporting document</h2><p>Upload one PDF, JPEG, or PNG file up to 10 MB.</p></div>
          <input name="document" type="file" accept="application/pdf,image/jpeg,image/png" required />
          <button className="secondary-button" disabled={documentMutation.isPending}>
            {documentMutation.isPending ? 'Uploading…' : 'Upload document'}
          </button>
          {documentMutation.isSuccess && <span className="success-message" role="status">{documentMutation.data.fileName} uploaded.</span>}
          {documentMutation.isError && <span className="field-error" role="alert">Unable to upload this file.</span>}
        </form>
      </section>
    </main>
  )
}

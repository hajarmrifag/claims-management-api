import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { getClaim, updateClaimStatus, uploadClaimDocument } from '../api/claims'
import { useAuth } from '../auth/useAuth'
import { CLAIM_STATUSES, type ClaimStatus } from '../claims/types'
import AppShell from '../components/AppShell'
import Icon from '../components/Icon'
import StatusBadge from '../components/StatusBadge'

function formatAmount(amount: number) {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', minimumFractionDigits: 2 }).format(amount)
}

function formatDate(value: string) {
  return new Intl.DateTimeFormat('en-US', { month: 'long', day: 'numeric', year: 'numeric', hour: 'numeric', minute: '2-digit' }).format(new Date(value))
}

export default function ClaimDetailPage() {
  const { claimId = '' } = useParams()
  const { session } = useAuth()
  const queryClient = useQueryClient()
  const [selectedStatus, setSelectedStatus] = useState<ClaimStatus | ''>('')
  const [selectedFile, setSelectedFile] = useState<File | null>(null)
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
    onSuccess: () => setSelectedFile(null),
  })
  const canChangeStatus = session?.role === 'Manager' || session?.role === 'Admin'

  if (claimQuery.isLoading) {
    return <AppShell title="Claim record"><div className="detail-loading" role="status"><span className="loading-spinner" />Loading claim record…</div></AppShell>
  }

  if (claimQuery.isError || !claimQuery.data) {
    return <AppShell title="Claim record"><div className="content-narrow"><Link className="back-link" to="/dashboard"><Icon name="arrow-left" size={17} /> Back to all claims</Link><section className="workspace-card empty-state error-state"><span className="empty-icon"><Icon name="document" /></span><h2>We couldn’t load this claim</h2><p>The record may have moved, or the service is temporarily unavailable.</p><button className="secondary-button" type="button" onClick={() => claimQuery.refetch()}>Try again</button></section></div></AppShell>
  }

  const claim = claimQuery.data
  return (
    <AppShell title={`Claim ${claim.claimNumber}`} description="Review the record, update its status, and add supporting evidence.">
      <div className="content-narrow">
        <Link className="back-link" to="/dashboard"><Icon name="arrow-left" size={17} /> Back to all claims</Link>

        <section className="claim-hero-card">
          <div className="claim-hero-main">
            <div><p className="eyebrow">Claim record</p><div className="claim-title-line"><h2>{claim.claimNumber}</h2><StatusBadge status={claim.status} /></div><p>{claim.description}</p></div>
            <div className="claim-value"><span>Claim amount</span><strong>{formatAmount(claim.amount)}</strong><small>Submitted {formatDate(claim.submittedAt)}</small></div>
          </div>
          <div className="claim-meta-strip">
            <div><span>Customer reference</span><strong>{claim.customerId}</strong></div>
            <div><span>Policy reference</span><strong>{claim.policyId}</strong></div>
            <div><span>Internal claim ID</span><strong>{claim.id}</strong></div>
          </div>
        </section>

        <div className="detail-columns">
          <section className="workspace-card detail-section">
            <div className="detail-section-heading"><span className="section-icon"><Icon name="clock" /></span><div><h2>Claim status</h2><p>Current position in the review workflow.</p></div></div>
            <div className="status-timeline" aria-label={`Current status: ${claim.status}`}>
              {['Submitted', 'UnderReview', 'Approved', 'Paid'].map((step, index) => {
                const ordered = ['Submitted', 'UnderReview', 'Approved', 'Paid']
                const currentIndex = ordered.indexOf(claim.status)
                const isComplete = currentIndex >= index && claim.status !== 'Rejected'
                return <div className={isComplete ? 'complete' : ''} key={step}><span>{isComplete ? <Icon name="check" size={14} /> : index + 1}</span><small>{step === 'UnderReview' ? 'Under review' : step}</small></div>
              })}
            </div>

            {canChangeStatus ? (
              <form className="status-form" onSubmit={(event) => { event.preventDefault(); if (selectedStatus) statusMutation.mutate(selectedStatus) }}>
                <label>Move claim to<select value={selectedStatus} onChange={(event) => setSelectedStatus(event.target.value as ClaimStatus)}><option value="">Choose the next status</option>{CLAIM_STATUSES.filter((status) => status !== claim.status).map((status) => <option key={status} value={status}>{status === 'UnderReview' ? 'Under review' : status}</option>)}</select></label>
                <button className="primary-button" type="submit" disabled={!selectedStatus || statusMutation.isPending}>{statusMutation.isPending ? 'Saving…' : 'Update status'}</button>
                {statusMutation.isError && <span className="field-error" role="alert">Unable to update status. Please try again.</span>}
              </form>
            ) : <div className="permission-note"><Icon name="lock" size={17} /><span>Only managers and administrators can change claim status.</span></div>}
          </section>

          <section className="workspace-card detail-section document-section">
            <div className="detail-section-heading"><span className="section-icon"><Icon name="file" /></span><div><h2>Supporting evidence</h2><p>Add a PDF, JPEG, or PNG up to 10 MB.</p></div></div>
            <form onSubmit={(event) => { event.preventDefault(); if (selectedFile) documentMutation.mutate(selectedFile) }}>
              <label className={`upload-zone ${selectedFile ? 'has-file' : ''}`}>
                <input name="document" type="file" accept="application/pdf,image/jpeg,image/png" onChange={(event) => { setSelectedFile(event.target.files?.[0] ?? null); documentMutation.reset() }} />
                <span className="upload-icon"><Icon name={selectedFile ? 'check' : 'upload'} /></span>
                {selectedFile ? <><strong>{selectedFile.name}</strong><small>{(selectedFile.size / 1024 / 1024).toFixed(2)} MB · Ready to upload</small></> : <><strong>Choose a file to upload</strong><small>PDF, JPEG, or PNG · 10 MB maximum</small></>}
              </label>
              <button className="secondary-button upload-button" disabled={!selectedFile || documentMutation.isPending}>{documentMutation.isPending ? 'Uploading evidence…' : <><Icon name="upload" size={17} /> Upload evidence</>}</button>
              {documentMutation.isSuccess && <span className="success-message" role="status"><Icon name="check" size={16} /> {documentMutation.data.fileName} uploaded successfully.</span>}
              {documentMutation.isError && <span className="field-error" role="alert">Unable to upload this file. Check its format and size, then try again.</span>}
            </form>
          </section>
        </div>
      </div>
    </AppShell>
  )
}

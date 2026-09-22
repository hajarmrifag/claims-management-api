import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { searchClaims } from '../api/claims'
import type { ClaimStatus } from '../claims/types'
import { CLAIM_STATUSES } from '../claims/types'
import AppShell from '../components/AppShell'
import Icon from '../components/Icon'
import StatusBadge from '../components/StatusBadge'

const PAGE_SIZE = 10

function formatAmount(amount: number): string {
  return new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 0 }).format(amount)
}

function formatDate(value: string): string {
  return new Intl.DateTimeFormat('en-US', { month: 'short', day: 'numeric', year: 'numeric' }).format(new Date(value))
}

export default function DashboardPage() {
  const navigate = useNavigate()
  const [searchInput, setSearchInput] = useState('')
  const [search, setSearch] = useState('')
  const [status, setStatus] = useState<ClaimStatus | ''>('')
  const [fromDate, setFromDate] = useState('')
  const [toDate, setToDate] = useState('')
  const [page, setPage] = useState(1)

  const invalidDateRange = Boolean(fromDate) && Boolean(toDate) && fromDate > toDate
  const claimsQuery = useQuery({
    queryKey: ['claims', { search, status, fromDate, toDate, page, pageSize: PAGE_SIZE }],
    queryFn: () => searchClaims({ search: search || undefined, status: status || undefined, fromDate: fromDate || undefined, toDate: toDate || undefined, page, pageSize: PAGE_SIZE }),
    placeholderData: keepPreviousData,
    enabled: !invalidDateRange,
  })

  const items = claimsQuery.data?.items ?? []
  const totalPages = Math.max(claimsQuery.data?.totalPages ?? 1, 1)
  const pageValue = items.reduce((sum, claim) => sum + claim.amount, 0)
  const inReview = items.filter((claim) => claim.status === 'UnderReview').length
  const hasFilters = Boolean(search || status || fromDate || toDate)

  function clearFilters() {
    setSearchInput('')
    setSearch('')
    setStatus('')
    setFromDate('')
    setToDate('')
    setPage(1)
  }

  function submitSearch(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setSearch(searchInput.trim())
    setPage(1)
  }

  return (
    <AppShell
      title="Claims workspace"
      description="Monitor, review, and resolve claims with confidence."
      actions={<Link className="primary-button button-link" to="/claims/new"><Icon name="plus" size={18} /> New claim</Link>}
    >
      <section className="summary-grid" aria-label="Claims summary">
        <article className="summary-card summary-card--primary"><span className="summary-icon"><Icon name="document" /></span><div><span>Matching claims</span><strong>{claimsQuery.data?.totalCount ?? '—'}</strong><small>{hasFilters ? 'Across active filters' : 'Across the workspace'}</small></div></article>
        <article className="summary-card"><span className="summary-icon"><Icon name="clock" /></span><div><span>In review</span><strong>{claimsQuery.isSuccess ? inReview : '—'}</strong><small>On this page</small></div></article>
        <article className="summary-card"><span className="summary-icon"><Icon name="shield" /></span><div><span>Claim value</span><strong>{claimsQuery.isSuccess ? formatAmount(pageValue) : '—'}</strong><small>On this page</small></div></article>
      </section>

      <section className="workspace-card claims-panel">
        <div className="section-heading">
          <div><p className="eyebrow">Claims register</p><h2>All claims</h2><p>{claimsQuery.data ? `${claimsQuery.data.totalCount} records found` : 'Your complete claims portfolio'}</p></div>
          {hasFilters && <button className="quiet-button" type="button" onClick={clearFilters}><Icon name="x" size={16} /> Clear filters</button>}
        </div>

        <div className="filter-bar">
          <form className="claim-search" role="search" onSubmit={submitSearch}>
            <Icon name="search" size={17} />
            <input aria-label="Search claims" placeholder="Search claim number or incident" value={searchInput} onChange={(event) => setSearchInput(event.target.value)} maxLength={100} />
            <button type="submit">Search</button>
          </form>
          <div className="filter-bar-label"><Icon name="filter" size={17} /><span>Filter</span></div>
          <label className="filter-field"><span className="visually-hidden">Status</span><select value={status} onChange={(event) => { setStatus(event.target.value as ClaimStatus | ''); setPage(1) }}><option value="">All statuses</option>{CLAIM_STATUSES.map((claimStatus) => <option key={claimStatus} value={claimStatus}>{claimStatus === 'UnderReview' ? 'Under review' : claimStatus}</option>)}</select></label>
          <label className="filter-field date-filter"><span>From</span><input aria-label="From date" type="date" value={fromDate} onChange={(event) => { setFromDate(event.target.value); setPage(1) }} /></label>
          <label className="filter-field date-filter"><span>To</span><input aria-label="To date" type="date" value={toDate} onChange={(event) => { setToDate(event.target.value); setPage(1) }} /></label>
        </div>

        {invalidDateRange && <div className="form-error inline-alert" role="alert">From date cannot be later than to date.</div>}

        {claimsQuery.isLoading && <div className="table-skeleton" role="status" aria-label="Loading claims">{[1, 2, 3, 4, 5].map((row) => <div key={row}><span /><span /><span /><span /></div>)}</div>}

        {claimsQuery.isError && <div className="empty-state error-state"><span className="empty-icon"><Icon name="document" /></span><h3>We couldn’t load your claims</h3><p>Check your connection and try again.</p><button className="secondary-button" type="button" onClick={() => claimsQuery.refetch()}>Try again</button></div>}

        {claimsQuery.isSuccess && items.length === 0 && (
          <div className="empty-state"><span className="empty-icon"><Icon name="search" /></span><h3>No claims found</h3><p>{hasFilters ? 'Try widening your filters to find what you need.' : 'Create your first claim to start building the register.'}</p>{hasFilters ? <button className="secondary-button" type="button" onClick={clearFilters}>Reset filters</button> : <Link className="primary-button button-link" to="/claims/new"><Icon name="plus" size={17} /> Create claim</Link>}</div>
        )}

        {claimsQuery.isSuccess && items.length > 0 && (
          <>
            {claimsQuery.isFetching && <p className="refresh-indicator" role="status">Updating claims…</p>}
            <div className="table-wrapper">
              <table className="claims-table">
                <thead><tr><th>Claim</th><th>Incident</th><th>Claim amount</th><th>Status</th><th>Submitted</th><th><span className="visually-hidden">Action</span></th></tr></thead>
                <tbody>{items.map((claim) => (
                  <tr key={claim.id} className="claim-row" onClick={() => navigate(`/claims/${claim.id}`)}>
                    <td><Link className="claim-number" to={`/claims/${claim.id}`} onClick={(event) => event.stopPropagation()}>{claim.claimNumber}</Link><small className="mobile-cell-label">{formatDate(claim.submittedAt)}</small></td>
                    <td className="description-cell">{claim.description}</td>
                    <td className="amount-cell">{formatAmount(claim.amount)}</td>
                    <td><StatusBadge status={claim.status} /></td>
                    <td>{formatDate(claim.submittedAt)}</td>
                    <td><span className="row-action" aria-hidden="true"><Icon name="arrow-right" size={17} /></span></td>
                  </tr>
                ))}</tbody>
              </table>
            </div>
            <div className="pagination">
              <span>Showing <strong>{(claimsQuery.data.page - 1) * PAGE_SIZE + 1}–{Math.min(claimsQuery.data.page * PAGE_SIZE, claimsQuery.data.totalCount)}</strong> of <strong>{claimsQuery.data.totalCount}</strong></span>
              <div className="pagination-buttons"><button type="button" disabled={page <= 1 || claimsQuery.isFetching} onClick={() => setPage((current) => Math.max(current - 1, 1))}><Icon name="arrow-left" size={17} /> Previous</button><span>{page} / {totalPages}</span><button type="button" disabled={page >= totalPages || claimsQuery.isFetching} onClick={() => setPage((current) => current + 1)}>Next <Icon name="arrow-right" size={17} /></button></div>
            </div>
          </>
        )}
      </section>
    </AppShell>
  )
}

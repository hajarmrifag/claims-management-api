import { keepPreviousData, useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { searchClaims } from '../api/claims'
import { useAuth } from '../auth/useAuth'
import {
  CLAIM_STATUSES,
  type ClaimStatus,
} from '../claims/types'

const PAGE_SIZE = 10

function formatAmount(amount: number): string {
  return new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(amount)
}

function formatDate(value: string): string {
  return new Date(value).toLocaleDateString()
}

export default function DashboardPage() {
  const { session, signOut } = useAuth()
  const navigate = useNavigate()

  const [status, setStatus] = useState<ClaimStatus | ''>('')
  const [fromDate, setFromDate] = useState('')
  const [toDate, setToDate] = useState('')
  const [page, setPage] = useState(1)

  const invalidDateRange =
    Boolean(fromDate) &&
    Boolean(toDate) &&
    fromDate > toDate

  const claimsQuery = useQuery({
    queryKey: [
      'claims',
      {
        status,
        fromDate,
        toDate,
        page,
        pageSize: PAGE_SIZE,
      },
    ],
    queryFn: () =>
      searchClaims({
        status: status || undefined,
        fromDate: fromDate || undefined,
        toDate: toDate || undefined,
        page,
        pageSize: PAGE_SIZE,
      }),
    placeholderData: keepPreviousData,
    enabled: !invalidDateRange,
  })

  function handleSignOut() {
    signOut()
    navigate('/login', { replace: true })
  }

  function handleStatusChange(value: string) {
    setStatus(value as ClaimStatus | '')
    setPage(1)
  }

  function handleFromDateChange(value: string) {
    setFromDate(value)
    setPage(1)
  }

  function handleToDateChange(value: string) {
    setToDate(value)
    setPage(1)
  }

  function clearFilters() {
    setStatus('')
    setFromDate('')
    setToDate('')
    setPage(1)
  }

  const totalPages = Math.max(
    claimsQuery.data?.totalPages ?? 1,
    1,
  )

  return (
    <main className="workspace">
      <header className="workspace-header">
        <div>
          <p className="eyebrow">Claims Management</p>
          <h1>Claims workspace</h1>
          <p className="workspace-subtitle">
            Review and manage insurance claims.
          </p>
        </div>

        <div className="user-panel">
          <button className="primary-button" type="button" onClick={() => navigate('/claims/new')}>
            New claim
          </button>
          <div>
            <strong>{session?.email}</strong>
            <span>{session?.role}</span>
          </div>

          <button type="button" onClick={handleSignOut}>
            Sign out
          </button>
        </div>
      </header>

      <section className="summary-grid">
        <article className="summary-card">
          <span>Matching claims</span>
          <strong>
            {claimsQuery.data?.totalCount ?? 0}
          </strong>
        </article>

        <article className="summary-card">
          <span>Current page</span>
          <strong>
            {claimsQuery.data?.page ?? page}
          </strong>
        </article>

        <article className="summary-card">
          <span>Role</span>
          <strong>{session?.role}</strong>
        </article>
      </section>

      <section className="workspace-card">
        <div className="section-heading">
          <div>
            <h2>Claims</h2>
            <p>
              Filter by workflow status or submission date.
            </p>
          </div>

          <button
            className="secondary-button"
            type="button"
            onClick={clearFilters}
            disabled={!status && !fromDate && !toDate}
          >
            Clear filters
          </button>
        </div>

        <div className="filter-grid">
          <label className="filter-field">
            <span>Status</span>

            <select
              value={status}
              onChange={(event) =>
                handleStatusChange(event.target.value)
              }
            >
              <option value="">All statuses</option>

              {CLAIM_STATUSES.map((claimStatus) => (
                <option
                  key={claimStatus}
                  value={claimStatus}
                >
                  {claimStatus}
                </option>
              ))}
            </select>
          </label>

          <label className="filter-field">
            <span>From date</span>

            <input
              type="date"
              value={fromDate}
              onChange={(event) =>
                handleFromDateChange(event.target.value)
              }
            />
          </label>

          <label className="filter-field">
            <span>To date</span>

            <input
              type="date"
              value={toDate}
              onChange={(event) =>
                handleToDateChange(event.target.value)
              }
            />
          </label>
        </div>

        {invalidDateRange && (
          <div className="form-error" role="alert">
            From date cannot be later than to date.
          </div>
        )}

        {claimsQuery.isLoading && (
          <div className="table-state">
            Loading claims...
          </div>
        )}

        {claimsQuery.isError && (
          <div className="table-state table-state-error">
            Unable to load claims.
            <button
              type="button"
              onClick={() => claimsQuery.refetch()}
            >
              Retry
            </button>
          </div>
        )}

        {!claimsQuery.isLoading &&
          !claimsQuery.isError &&
          claimsQuery.data?.items.length === 0 && (
            <div className="empty-state">
              <h3>No claims found</h3>
              <p>
                No claims match the current filters.
              </p>
            </div>
          )}

        {claimsQuery.data &&
          claimsQuery.data.items.length > 0 && (
            <>
              {claimsQuery.isFetching && !claimsQuery.isLoading && (
                <p className="refresh-indicator" role="status">
                  Updating claims…
                </p>
              )}

              <div className="table-wrapper">
                <table className="claims-table">
                  <thead>
                    <tr>
                      <th>Claim</th>
                      <th>Description</th>
                      <th>Amount</th>
                      <th>Status</th>
                      <th>Submitted</th>
                      <th><span className="visually-hidden">Action</span></th>
                    </tr>
                  </thead>

                  <tbody>
                    {claimsQuery.data.items.map((claim) => (
                      <tr
                        key={claim.id}
                        className="claim-row"
                        tabIndex={0}
                        onClick={() => navigate(`/claims/${claim.id}`)}
                        onKeyDown={(event) => {
                          if (event.key === 'Enter' || event.key === ' ') {
                            event.preventDefault()
                            navigate(`/claims/${claim.id}`)
                          }
                        }}
                      >
                        <td>
                          <strong>
                            {claim.claimNumber}
                          </strong>
                        </td>

                        <td className="description-cell">
                          {claim.description}
                        </td>

                        <td>
                          {formatAmount(claim.amount)}
                        </td>

                        <td>
                          <span
                            className={`status-badge status-${claim.status.toLowerCase()}`}
                          >
                            {claim.status}
                          </span>
                        </td>

                        <td>
                          {formatDate(claim.submittedAt)}
                        </td>
                        <td className="row-action" aria-hidden="true">
                          View →
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              <div className="pagination">
                <span>
                  Page {claimsQuery.data.page} of{' '}
                  {totalPages}
                </span>

                <div className="pagination-buttons">
                  <button
                    type="button"
                    disabled={page <= 1 || claimsQuery.isFetching}
                    onClick={() =>
                      setPage((current) =>
                        Math.max(current - 1, 1),
                      )
                    }
                  >
                    Previous
                  </button>

                  <button
                    type="button"
                    disabled={
                      page >= totalPages || claimsQuery.isFetching
                    }
                    onClick={() =>
                      setPage((current) =>
                        current + 1,
                      )
                    }
                  >
                    Next
                  </button>
                </div>
              </div>
            </>
          )}
      </section>
    </main>
  )
}

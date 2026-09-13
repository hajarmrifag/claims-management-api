export const CLAIM_STATUSES = [
  'Submitted',
  'UnderReview',
  'Approved',
  'Rejected',
  'Paid',
] as const

export type ClaimStatus = (typeof CLAIM_STATUSES)[number]

export interface Claim {
  id: string
  claimNumber: string
  customerId: string
  policyId: string
  description: string
  amount: number
  status: ClaimStatus
  submittedAt: string
}

export interface ClaimSearchParams {
  status?: ClaimStatus
  fromDate?: string
  toDate?: string
  page: number
  pageSize: number
}

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
}

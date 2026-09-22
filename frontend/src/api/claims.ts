import { apiClient } from './client'
import type {
  Claim,
  ClaimSearchParams,
  PagedResult,
} from '../claims/types'

export async function searchClaims(
  params: ClaimSearchParams,
): Promise<PagedResult<Claim>> {
  const response = await apiClient.get<PagedResult<Claim>>(
    '/api/claims',
    {
      params: {
        Search: params.search,
        Status: params.status,
        FromDate: params.fromDate,
        ToDate: params.toDate,
        Page: params.page,
        PageSize: params.pageSize,
      },
    },
  )

  return response.data
}

export async function getClaim(id: string): Promise<Claim> {
  const response = await apiClient.get<Claim>(`/api/claims/${id}`)
  return response.data
}

export async function updateClaimStatus(
  id: string,
  status: Claim['status'],
): Promise<Claim> {
  const response = await apiClient.patch<Claim>(
    `/api/claims/${id}/status`,
    { status },
  )

  return response.data
}

export interface ClaimDocument {
  id: string
  claimId: string
  fileName: string
  blobUrl: string
  uploadedAt: string
}

export async function uploadClaimDocument(id: string, file: File): Promise<ClaimDocument> {
  const data = new FormData()
  data.append('file', file)
  const response = await apiClient.post<ClaimDocument>(`/api/claims/${id}/documents`, data, {
    headers: { 'Content-Type': 'multipart/form-data' },
  })
  return response.data
}

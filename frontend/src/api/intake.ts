import { apiClient } from './client'
import type { Claim } from '../claims/types'

export interface CustomerInput { firstName: string; lastName: string; email: string; phoneNumber?: string }
export interface Customer extends CustomerInput { id: string }
export interface PolicyInput { policyNumber: string; customerId: string; policyType: string; coverageAmount: number; startDate: string; endDate: string }
export interface Policy extends PolicyInput { id: string }
export interface ClaimInput { claimNumber: string; customerId: string; policyId: string; description: string; amount: number }

export async function createCustomer(input: CustomerInput) {
  return (await apiClient.post<Customer>('/api/customers', input)).data
}
export async function createPolicy(input: PolicyInput) {
  return (await apiClient.post<Policy>('/api/policies', input)).data
}
export async function createClaim(input: ClaimInput) {
  return (await apiClient.post<Claim>('/api/claims', input)).data
}

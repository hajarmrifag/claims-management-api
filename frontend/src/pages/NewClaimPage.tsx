import { useMutation, useQueryClient } from '@tanstack/react-query'
import axios from 'axios'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { createClaim, createCustomer, createPolicy } from '../api/intake'

function value(data: FormData, name: string) { return String(data.get(name) ?? '').trim() }

export default function NewClaimPage() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const mutation = useMutation({
    mutationFn: async (data: FormData) => {
      const customer = await createCustomer({
        firstName: value(data, 'firstName'), lastName: value(data, 'lastName'),
        email: value(data, 'email'), phoneNumber: value(data, 'phoneNumber') || undefined,
      })
      const policy = await createPolicy({
        customerId: customer.id, policyNumber: value(data, 'policyNumber'),
        policyType: value(data, 'policyType'), coverageAmount: Number(value(data, 'coverageAmount')),
        startDate: value(data, 'startDate'), endDate: value(data, 'endDate'),
      })
      return createClaim({
        customerId: customer.id, policyId: policy.id, claimNumber: value(data, 'claimNumber'),
        description: value(data, 'description'), amount: Number(value(data, 'amount')),
      })
    },
    onSuccess: (claim) => {
      queryClient.invalidateQueries({ queryKey: ['claims'] })
      navigate(`/claims/${claim.id}`, { replace: true })
    },
  })
  const errorMessage = axios.isAxiosError(mutation.error)
    ? String(mutation.error.response?.data?.error ?? 'Unable to create the claim. Check the details and try again.')
    : mutation.isError ? 'Unable to create the claim.' : null

  function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    const form = event.currentTarget
    if (!form.reportValidity()) return
    const data = new FormData(form)
    if (value(data, 'endDate') < value(data, 'startDate')) {
      form.querySelector<HTMLInputElement>('[name="endDate"]')?.setCustomValidity('End date must be after start date.')
      form.reportValidity()
      return
    }
    mutation.mutate(data)
  }

  return (
    <main className="detail-shell">
      <Link className="back-link" to="/dashboard">← Back to claims</Link>
      <header className="detail-header"><div><p className="eyebrow">Claim intake</p><h1>Create a new claim</h1></div></header>
      <form className="detail-card intake-form" onSubmit={submit}>
        <fieldset><legend>Customer</legend><div className="form-grid">
          <label>First name<input name="firstName" required maxLength={100} /></label>
          <label>Last name<input name="lastName" required maxLength={100} /></label>
          <label>Email<input name="email" type="email" required maxLength={255} /></label>
          <label>Phone number<input name="phoneNumber" type="tel" maxLength={30} /></label>
        </div></fieldset>
        <fieldset><legend>Policy</legend><div className="form-grid">
          <label>Policy number<input name="policyNumber" required maxLength={50} /></label>
          <label>Policy type<input name="policyType" required maxLength={100} placeholder="Home, motor, travel…" /></label>
          <label>Coverage amount<input name="coverageAmount" type="number" min="0.01" step="0.01" required /></label>
          <label>Start date<input name="startDate" type="date" required /></label>
          <label>End date<input name="endDate" type="date" required onChange={(event) => event.currentTarget.setCustomValidity('')} /></label>
        </div></fieldset>
        <fieldset><legend>Claim</legend><div className="form-grid">
          <label>Claim number<input name="claimNumber" required maxLength={50} /></label>
          <label>Claim amount<input name="amount" type="number" min="0.01" step="0.01" required /></label>
          <label className="form-wide">Description<textarea name="description" required maxLength={2000} rows={5} /></label>
        </div></fieldset>
        {errorMessage && <div className="form-error" role="alert">{errorMessage}</div>}
        <div className="form-actions"><Link className="secondary-link" to="/dashboard">Cancel</Link><button className="primary-button" disabled={mutation.isPending}>{mutation.isPending ? 'Creating…' : 'Create claim'}</button></div>
      </form>
    </main>
  )
}

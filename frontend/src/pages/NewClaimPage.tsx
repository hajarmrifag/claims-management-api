import { useMutation, useQueryClient } from '@tanstack/react-query'
import axios from 'axios'
import type { FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { createClaim, createCustomer, createPolicy } from '../api/intake'
import AppShell from '../components/AppShell'
import Icon from '../components/Icon'

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
    <AppShell title="Create a new claim" description="Add the customer, policy, and incident details to open a claim.">
      <div className="content-narrow">
        <Link className="back-link" to="/dashboard"><Icon name="arrow-left" size={17} /> Back to all claims</Link>
        <div className="intake-layout">
          <aside className="intake-guide">
            <p className="eyebrow">Claim intake</p>
            <h2>A complete record in three steps.</h2>
            <ol>
              <li className="active"><span>1</span><div><strong>Customer</strong><small>Contact information</small></div></li>
              <li><span>2</span><div><strong>Policy</strong><small>Coverage details</small></div></li>
              <li><span>3</span><div><strong>Claim</strong><small>Incident and value</small></div></li>
            </ol>
            <div className="guide-note"><Icon name="shield" /><p><strong>Secure by default</strong><span>Data is validated before it enters the claims register.</span></p></div>
          </aside>

          <form className="workspace-card intake-form" onSubmit={submit}>
            <fieldset><legend><span>1</span><div>Customer information<small>Who is making this claim?</small></div></legend><div className="form-grid">
              <label>First name<input name="firstName" autoComplete="given-name" placeholder="e.g. Maya" required maxLength={100} /></label>
              <label>Last name<input name="lastName" autoComplete="family-name" placeholder="e.g. Chen" required maxLength={100} /></label>
              <label>Email address<input name="email" type="email" autoComplete="email" placeholder="maya@company.com" required maxLength={255} /></label>
              <label>Phone number <small>Optional</small><input name="phoneNumber" type="tel" autoComplete="tel" placeholder="+1 555 000 0000" maxLength={30} /></label>
            </div></fieldset>
            <fieldset><legend><span>2</span><div>Policy details<small>Confirm the coverage in force.</small></div></legend><div className="form-grid">
              <label>Policy number<input name="policyNumber" placeholder="POL-2026-001" required maxLength={50} /></label>
              <label>Policy type<input name="policyType" placeholder="Home, motor, travel…" required maxLength={100} /></label>
              <label>Coverage amount<input name="coverageAmount" type="number" min="0.01" step="0.01" placeholder="0.00" required /></label>
              <label>Coverage start<input name="startDate" type="date" required /></label>
              <label>Coverage end<input name="endDate" type="date" required onChange={(event) => event.currentTarget.setCustomValidity('')} /></label>
            </div></fieldset>
            <fieldset><legend><span>3</span><div>Claim details<small>Describe the loss and amount requested.</small></div></legend><div className="form-grid">
              <label>Claim number<input name="claimNumber" placeholder="CLM-2026-001" required maxLength={50} /></label>
              <label>Claim amount<input name="amount" type="number" min="0.01" step="0.01" placeholder="0.00" required /></label>
              <label className="form-wide">Incident description <small>Be clear and specific.</small><textarea name="description" placeholder="Summarize what happened, when it occurred, and the resulting loss…" required maxLength={2000} rows={6} /></label>
            </div></fieldset>
            {errorMessage && <div className="form-error" role="alert">{errorMessage}</div>}
            <div className="form-actions"><Link className="secondary-link-button" to="/dashboard">Cancel</Link><button className="primary-button" disabled={mutation.isPending}>{mutation.isPending ? 'Creating claim…' : <>Create claim <Icon name="arrow-right" size={17} /></>}</button></div>
          </form>
        </div>
      </div>
    </AppShell>
  )
}

import { expect, test } from '@playwright/test'

test('an adjuster can register and enter the claims workspace', async ({
  page,
}) => {
  await page.route('**/api/auth/register', async (route) => {
    await route.fulfill({
      contentType: 'application/json',
      body: JSON.stringify({
        token: 'browser-test-token',
        expiresAt: '2099-01-01T00:00:00Z',
        email: 'candidate@example.com',
        role: 'Adjuster',
      }),
    })
  })

  await page.route(/\/api\/claims(?:\?.*)?$/, async (route) => {
    await route.fulfill({
      contentType: 'application/json',
      body: JSON.stringify({
        items: [],
        page: 1,
        pageSize: 10,
        totalCount: 0,
        totalPages: 0,
      }),
    })
  })

  await page.goto('/login')
  await page.getByRole('button', { name: 'Create an account' }).click()
  await page.getByLabel('Email').fill('candidate@example.com')
  await page.getByLabel('Password').fill('A-secure-test-password!')
  await page.getByRole('button', { name: 'Create account' }).click()

  await expect(page).toHaveURL(/\/dashboard$/)
  await expect(
    page.getByRole('heading', { name: 'Claims workspace' }),
  ).toBeVisible()
  await expect(page.getByText('candidate@example.com')).toBeVisible()
  await expect(
    page.getByRole('heading', { name: 'No claims found' }),
  ).toBeVisible()
})

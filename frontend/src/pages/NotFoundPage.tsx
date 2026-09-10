import { Link } from 'react-router-dom'

export default function NotFoundPage() {
  return (
    <main className="page">
      <section className="card">
        <p className="eyebrow">404</p>
        <h1>Page not found</h1>
        <Link to="/">Return home</Link>
      </section>
    </main>
  )
}

import ApiStatus from '../components/ApiStatus'
import { Link } from 'react-router-dom'

export default function HomePage() {
  return (
    <main className="page">
      <section className="card">
        <p className="eyebrow">Claims Management</p>

        <h1>Insurance claims, managed clearly.</h1>

        <p>
          A full-stack claims management application built with React,
          TypeScript, ASP.NET Core, and SQL Server.
        </p>

        <Link className="primary-link" to="/login">
          Open claims workspace
        </Link>

        <ApiStatus />
      </section>
    </main>
  )
}

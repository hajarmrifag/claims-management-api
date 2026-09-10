import ApiStatus from '../components/ApiStatus'

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

        <ApiStatus />
      </section>
    </main>
  )
}

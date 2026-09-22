import { Link } from 'react-router-dom'
import Brand from '../components/Brand'
import Icon from '../components/Icon'

export default function NotFoundPage() {
  return (
    <main className="not-found-page">
      <Brand />
      <section>
        <span className="not-found-code">404</span>
        <p className="eyebrow">Lost in the paperwork?</p>
        <h1>This page isn’t part of the claim.</h1>
        <p>The address may be incorrect, or the page may have moved.</p>
        <Link className="primary-link" to="/"><Icon name="arrow-left" size={18} /> Return home</Link>
      </section>
    </main>
  )
}

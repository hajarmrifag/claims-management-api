import ApiStatus from '../components/ApiStatus'
import { Link } from 'react-router-dom'
import Brand from '../components/Brand'
import Icon from '../components/Icon'

export default function HomePage() {
  return (
    <div className="marketing-page">
      <header className="marketing-header">
        <Brand />
        <nav aria-label="Homepage navigation">
          <a href="#platform">Platform</a>
          <a href="#assurance">Security</a>
          <Link className="header-login" to="/login">Sign in</Link>
        </nav>
      </header>

      <main>
        <section className="hero">
          <div className="hero-copy">
            <div className="hero-kicker"><Icon name="sparkles" size={16} /> Built for modern claims teams</div>
            <h1>Insurance claims, <span>managed clearly.</span></h1>
            <p className="hero-lede">
              Move every claim from first notice to resolution with one focused,
              auditable workspace your team can trust.
            </p>
            <div className="hero-actions">
              <Link className="primary-link" to="/login">
                Open claims workspace <Icon name="arrow-right" size={18} />
              </Link>
              <a className="secondary-link-button" href="#platform">Explore the platform</a>
            </div>
            <div className="hero-proof">
              <span><Icon name="check" size={16} /> Role-based access</span>
              <span><Icon name="check" size={16} /> Complete audit trail</span>
              <span><Icon name="check" size={16} /> Secure evidence</span>
            </div>
          </div>

          <div className="product-preview" aria-label="Claims dashboard preview">
            <div className="preview-glow" />
            <div className="preview-window">
              <div className="preview-sidebar">
                <Brand compact />
                <span className="preview-nav active"><Icon name="grid" size={15} /> Claims</span>
                <span className="preview-nav"><Icon name="plus" size={15} /> New claim</span>
              </div>
              <div className="preview-main">
                <div className="preview-heading"><div><span>Overview</span><strong>Good morning, Hajar</strong></div><i /></div>
                <div className="preview-stats">
                  <div><span>Open claims</span><strong>248</strong><small>↑ 8.2% this month</small></div>
                  <div><span>In review</span><strong>64</strong><small>12 due today</small></div>
                  <div><span>Settled value</span><strong>$1.2m</strong><small>↑ 14.6% this month</small></div>
                </div>
                <div className="preview-table">
                  <div className="preview-table-head"><strong>Recent claims</strong><span>View all</span></div>
                  {[
                    ['CLM-2048', 'Water damage', '$12,400', 'Under review'],
                    ['CLM-2047', 'Vehicle collision', '$8,750', 'Approved'],
                    ['CLM-2046', 'Travel disruption', '$2,180', 'Submitted'],
                    ['CLM-2045', 'Property damage', '$24,900', 'Paid'],
                  ].map(([id, name, amount, status]) => (
                    <div className="preview-row" key={id}>
                      <span><b>{id}</b><small>{name}</small></span>
                      <strong>{amount}</strong>
                      <em className={`mini-status mini-${status.replace(' ', '').toLowerCase()}`}>{status}</em>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </section>

        <section className="trust-bar" aria-label="Platform technologies">
          <span>Production-ready architecture</span>
          <strong>React</strong><strong>TypeScript</strong><strong>ASP.NET Core</strong><strong>PostgreSQL</strong>
        </section>

        <section className="feature-section" id="platform">
          <div className="section-intro">
            <p className="eyebrow">One connected workflow</p>
            <h2>Clarity at every stage of the claim.</h2>
            <p>Less time piecing together context. More time making confident decisions.</p>
          </div>
          <div className="feature-grid">
            <article><span className="feature-icon"><Icon name="document" /></span><h3>Structured intake</h3><p>Capture customer, policy, incident, and financial details in one guided workflow.</p></article>
            <article><span className="feature-icon"><Icon name="clock" /></span><h3>Visible progress</h3><p>See status, ownership, and the full decision trail without chasing updates.</p></article>
            <article><span className="feature-icon"><Icon name="shield" /></span><h3>Secure by design</h3><p>Role-aware actions, validated documents, and hardened APIs protect every record.</p></article>
          </div>
        </section>

        <section className="assurance-section" id="assurance">
          <div><p className="eyebrow">Operational confidence</p><h2>Built for the work behind every decision.</h2></div>
          <div className="assurance-list">
            <span><Icon name="check" /> Auditable status transitions</span>
            <span><Icon name="check" /> Encrypted authentication</span>
            <span><Icon name="check" /> Validated file handling</span>
            <span><Icon name="check" /> Automated security scanning</span>
          </div>
          <ApiStatus />
        </section>
      </main>

      <footer className="marketing-footer"><Brand /><p>Claims operations, without the operational drag.</p><span>© {new Date().getFullYear()} Aegis Claims</span></footer>
    </div>
  )
}

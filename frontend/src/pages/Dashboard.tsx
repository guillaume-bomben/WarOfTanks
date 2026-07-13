import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { matchService } from "../services/matchService";
import type { Match } from "../services/matchService";
import { Shield, Play, Trophy, Award, Percent, Calendar, ArrowRight, Activity } from "lucide-react";

export const Dashboard: React.FC = () => {
  const { user } = useAuth();
  const [matches, setMatches] = useState<Match[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchRecentMatches = async () => {
      try {
        const data = await matchService.getMatches();
        // Filter matches where current user played
        if (user) {
          const userMatches = data.filter(
            (m) => m.playerA?._id === user._id || m.playerB?._id === user._id
          );
          // Sort chronologically and take last 3
          userMatches.sort(
            (a, b) => new Date(b.playedAt).getTime() - new Date(a.playedAt).getTime()
          );
          setMatches(userMatches.slice(0, 3));
        }
      } catch (err) {
        console.error("Error fetching matches for dashboard:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchRecentMatches();
  }, [user]);

  if (!user) return null;

  // Dynamically calculate rank tier
  const getRank = (score: number) => {
    if (score >= 1200) return { name: "Diamant III", color: "#60a5fa", bg: "rgba(96, 165, 250, 0.1)" };
    if (score >= 1100) return { name: "Platine I", color: "#38bdf8", bg: "rgba(56, 189, 248, 0.1)" };
    if (score >= 1050) return { name: "Or II", color: "#fbbf24", bg: "rgba(251, 191, 36, 0.1)" };
    if (score >= 1000) return { name: "Argent I", color: "#94a3b8", bg: "rgba(148, 163, 184, 0.1)" };
    return { name: "Bronze III", color: "#b45309", bg: "rgba(180, 83, 9, 0.1)" };
  };

  const rank = getRank(user.score);
  const winRate = user.matchesPlayed > 0 ? Math.round((user.wins / user.matchesPlayed) * 100) : 0;

  return (
    <div style={styles.dashboard} className="animate-fade-in container">
      {/* Grid: Welcome CTA & Quick Stats Side-by-side */}
      <div style={styles.topSection}>
        {/* Welcome Box */}
        <div style={styles.welcomeCard} className="modern-card">
          <div style={styles.welcomeLeft}>
            <span style={styles.rankBadge(rank.color, rank.bg)}>{rank.name}</span>
            <h1 style={styles.greeting}>Bonjour, {user.username} !</h1>
            <p style={styles.welcomeSub}>
              Prêt pour le combat ? Vos chenilles trépignent d'impatience. Montez à bord et détruisez les blindés ennemis !
            </p>
            <div style={styles.ctaContainer}>
              <Link to="/play" className="btn btn-primary" style={styles.playBtn}>
                <Play size={18} fill="#ffffff" />
                <span>COMBATTRE EN WEBGL</span>
              </Link>
            </div>
          </div>
          <div style={styles.welcomeRight}>
            <div style={styles.bigShield}>🛡️</div>
          </div>
        </div>
      </div>

      {/* Metrics Row */}
      <div style={styles.metricsGrid}>
        <div style={styles.metricCard} className="modern-card">
          <div style={styles.metricIconWrap("#3b82f6", "rgba(59, 130, 246, 0.1)")}>
            <Trophy size={20} />
          </div>
          <span style={styles.metricLabel}>SCORE GENERAL</span>
          <span style={styles.metricValue}>{user.score}</span>
        </div>

        <div style={styles.metricCard} className="modern-card">
          <div style={styles.metricIconWrap("#10b981", "rgba(16, 185, 129, 0.1)")}>
            <Award size={20} />
          </div>
          <span style={styles.metricLabel}>VICTOIRES</span>
          <span style={styles.metricValue}>{user.wins}</span>
        </div>

        <div style={styles.metricCard} className="modern-card">
          <div style={styles.metricIconWrap("#ef4444", "rgba(239, 68, 68, 0.1)")}>
            <Shield size={20} />
          </div>
          <span style={styles.metricLabel}>DÉFAITES</span>
          <span style={styles.metricValue}>{user.losses}</span>
        </div>

        <div style={styles.metricCard} className="modern-card">
          <div style={styles.metricIconWrap("#f59e0b", "rgba(245, 158, 11, 0.1)")}>
            <Percent size={20} />
          </div>
          <span style={styles.metricLabel}>WIN RATE</span>
          <span style={styles.metricValue}>{winRate}%</span>
        </div>
      </div>

      {/* Bottom Grid: Left = Recent Activity, Right = Training status / Quick tips */}
      <div style={styles.bottomGrid}>
        <div style={styles.recentActivityCol} className="modern-card">
          <div style={styles.sectionHeader}>
            <div style={{ display: "flex", alignItems: "center", gap: "10px" }}>
              <Activity size={20} style={{ color: "var(--accent-primary)" }} />
              <h3 style={styles.sectionTitle}>Matchs Récents</h3>
            </div>
            <Link to="/history" style={styles.historyLink}>
              <span>Voir tout</span>
              <ArrowRight size={14} />
            </Link>
          </div>

          {loading ? (
            <div style={styles.matchesLoader}>Chargement...</div>
          ) : matches.length === 0 ? (
            <div style={styles.emptyState}>
              <p style={{ color: "var(--text-secondary)", marginBottom: "15px" }}>
                Aucune bataille enregistrée à votre actif.
              </p>
              <Link to="/play" className="btn btn-secondary" style={{ padding: "8px 16px" }}>
                Faire vos débuts
              </Link>
            </div>
          ) : (
            <div style={styles.matchList}>
              {matches.map((match) => {
                const isWinner = match.winner?._id === user._id;
                const opponent = match.playerA?._id === user._id ? match.playerB : match.playerA;
                const userScore = match.playerA?._id === user._id ? match.scoreTeamA : match.scoreTeamB;
                const oppScore = match.playerA?._id === user._id ? match.scoreTeamB : match.scoreTeamA;
                const formattedDate = new Date(match.playedAt).toLocaleDateString("fr-FR", {
                  day: "numeric",
                  month: "short",
                  hour: "2-digit",
                  minute: "2-digit",
                });

                return (
                  <div key={match._id} style={styles.matchItem}>
                    <div style={styles.matchLeft}>
                      <span className={`badge ${isWinner ? "badge-success" : "badge-danger"}`} style={{ fontSize: "11px" }}>
                        {isWinner ? "VICTOIRE" : "DÉFAITE"}
                      </span>
                      <div style={styles.matchOpponentDetails}>
                        <span style={styles.opponentName}>vs {opponent?.username || "Robot_AI"}</span>
                        <div style={styles.matchMeta}>
                          <Calendar size={12} />
                          <span>{formattedDate}</span>
                        </div>
                      </div>
                    </div>
                    <div style={styles.matchRight}>
                      <span style={{
                        ...styles.matchScore,
                        color: isWinner ? "var(--accent-emerald)" : "var(--accent-rose)"
                      }}>
                        {userScore} - {oppScore}
                      </span>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>

        {/* Tips & Guides Column */}
        <div style={styles.trainingCol} className="modern-card">
          <h3 style={{ ...styles.sectionTitle, marginBottom: "15px" }}>Guide Tactique</h3>
          <div style={styles.tipsContainer}>
            <div style={styles.tipItem}>
              <span style={styles.tipNumber}>01</span>
              <div>
                <h4 style={styles.tipHeader}>Gardez vos distances</h4>
                <p style={styles.tipText}>
                  Les obus plasma volent droit, mais les chenilles ennemies tournent vite. Utilisez le recul pour rester hors de portée !
                </p>
              </div>
            </div>

            <div style={styles.tipItem}>
              <span style={styles.tipNumber}>02</span>
              <div>
                <h4 style={styles.tipHeader}>Visez les flancs</h4>
                <p style={styles.tipText}>
                  Les collisions frontales vous infligent de lourds dégâts de bouclier. Évitez les chocs directs !
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

const styles: Record<string, any> = {
  dashboard: {
    padding: "30px 20px",
  },
  topSection: {
    marginBottom: "30px",
  },
  welcomeCard: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    backgroundImage: "linear-gradient(135deg, rgba(37, 99, 235, 0.05) 0%, rgba(255, 255, 255, 0) 100%)",
    padding: "35px 40px",
  },
  welcomeLeft: {
    flex: 1.5,
    textAlign: "left",
  },
  welcomeRight: {
    flex: 0.5,
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
  },
  bigShield: {
    fontSize: "90px",
    opacity: 0.8,
    userSelect: "none",
  },
  rankBadge: (color: string, bg: string) => ({
    display: "inline-block",
    color: color,
    backgroundColor: bg,
    padding: "6px 12px",
    borderRadius: "20px",
    fontSize: "12px",
    fontWeight: 700,
    textTransform: "uppercase",
    border: `1px solid rgba(255, 255, 255, 0.05)`,
    letterSpacing: "0.05em",
    marginBottom: "12px",
  }),
  greeting: {
    fontSize: "32px",
    fontWeight: 800,
    letterSpacing: "-0.5px",
    marginBottom: "8px",
    color: "#ffffff",
  },
  welcomeSub: {
    color: "var(--text-secondary)",
    fontSize: "15px",
    lineHeight: "1.6",
    marginBottom: "24px",
    maxWidth: "520px",
  },
  ctaContainer: {
    display: "flex",
    gap: "15px",
  },
  playBtn: {
    padding: "14px 28px",
    fontSize: "15px",
  },
  metricsGrid: {
    display: "grid",
    gridTemplateColumns: "repeat(auto-fit, minmax(220px, 1fr))",
    gap: "20px",
    marginBottom: "35px",
  },
  metricCard: {
    display: "flex",
    flexDirection: "column",
    alignItems: "flex-start",
    position: "relative",
    overflow: "hidden",
  },
  metricIconWrap: (color: string, bg: string) => ({
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    width: "40px",
    height: "40px",
    borderRadius: "10px",
    color: color,
    backgroundColor: bg,
    marginBottom: "16px",
  }),
  metricLabel: {
    fontSize: "12px",
    fontWeight: 700,
    color: "var(--text-secondary)",
    letterSpacing: "0.05em",
    marginBottom: "4px",
  },
  metricValue: {
    fontSize: "28px",
    fontWeight: 800,
    color: "#ffffff",
  },
  bottomGrid: {
    display: "grid",
    gridTemplateColumns: "1.3fr 1fr",
    gap: "25px",
  },
  recentActivityCol: {
    textAlign: "left",
  },
  sectionHeader: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    borderBottom: "1px solid var(--border-color)",
    paddingBottom: "15px",
    marginBottom: "15px",
  },
  sectionTitle: {
    fontSize: "18px",
    fontWeight: 700,
    color: "#ffffff",
  },
  historyLink: {
    display: "flex",
    alignItems: "center",
    gap: "4px",
    color: "var(--accent-primary)",
    textDecoration: "none",
    fontSize: "14px",
    fontWeight: 600,
    transition: "color 0.2s ease",
  },
  matchesLoader: {
    padding: "30px 0",
    textAlign: "center",
    color: "var(--text-secondary)",
  },
  emptyState: {
    padding: "40px 0",
    textAlign: "center",
  },
  matchList: {
    display: "flex",
    flexDirection: "column",
    gap: "12px",
  },
  matchItem: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    backgroundColor: "rgba(255, 255, 255, 0.015)",
    border: "1px solid var(--border-color)",
    borderRadius: "10px",
    padding: "16px",
    transition: "border-color 0.15s ease",
  },
  matchLeft: {
    display: "flex",
    alignItems: "center",
    gap: "15px",
  },
  matchOpponentDetails: {
    display: "flex",
    flexDirection: "column",
    gap: "2px",
  },
  opponentName: {
    fontWeight: 600,
    fontSize: "15px",
    color: "#ffffff",
  },
  matchMeta: {
    display: "flex",
    alignItems: "center",
    gap: "6px",
    fontSize: "12px",
    color: "var(--text-secondary)",
  },
  matchRight: {
    textAlign: "right",
  },
  matchScore: {
    fontWeight: 800,
    fontSize: "16px",
  },
  trainingCol: {
    textAlign: "left",
  },
  tipsContainer: {
    display: "flex",
    flexDirection: "column",
    gap: "20px",
  },
  tipItem: {
    display: "flex",
    gap: "16px",
  },
  tipNumber: {
    fontSize: "24px",
    fontWeight: 800,
    color: "rgba(59, 130, 246, 0.3)",
    lineHeight: "1",
  },
  tipHeader: {
    fontSize: "15px",
    fontWeight: 700,
    color: "#ffffff",
    marginBottom: "4px",
  },
  tipText: {
    fontSize: "13px",
    color: "var(--text-secondary)",
    lineHeight: "1.5",
  }
};

// Inject responsive grid styling
if (typeof document !== "undefined") {
  const style = document.createElement("style");
  style.textContent = `
    @media (max-width: 900px) {
      #root > div > div.container {
        padding: 15px !important;
      }
      #root > div > div.container > div:last-child {
        grid-template-columns: 1fr !important;
      }
      #root > div > div.container > div:first-child > div {
        flex-direction: column !important;
        padding: 25px !important;
        text-align: center !important;
      }
      #root > div > div.container > div:first-child > div > div:first-child {
        text-align: center !important;
      }
      #root > div > div.container > div:first-child > div > div:first-child div {
        justify-content: center !important;
      }
      #root > div > div.container > div:first-child > div > div:last-child {
        display: none !important;
      }
    }
  `;
  document.head.appendChild(style);
}
export default Dashboard;

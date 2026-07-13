import React from "react";
import { useAuth } from "../context/AuthContext";
import { BarChart2, Target, Flame, TrendingUp, ShieldAlert } from "lucide-react";

export const Stats: React.FC = () => {
  const { user } = useAuth();

  if (!user) return null;

  const totalMatches = user.matchesPlayed || 0;
  const winRate = totalMatches > 0 ? Math.round((user.wins / totalMatches) * 100) : 0;
  
  // Calculate average performance metrics
  const avgScoreGained = totalMatches > 0 ? Math.round(user.score / totalMatches) : 0;
  
  // Custom Rank progress toward the next division (e.g. increments of 100)
  const scoreDiv = user.score % 100;
  const rankProgress = scoreDiv; // Progress from 0 to 100 to next level
  const nextRankScore = user.score + (100 - scoreDiv);

  return (
    <div style={styles.page} className="animate-fade-in container">
      {/* Header */}
      <div style={styles.header}>
        <div style={styles.headerTitleGroup}>
          <BarChart2 size={28} style={{ color: "var(--accent-primary)" }} />
          <h2 style={styles.title}>Statistiques Personnelles</h2>
        </div>
        <p style={styles.subtitle}>
          Consultez l'historique complet de vos performances militaires et analysez vos points forts.
        </p>
      </div>

      {/* Main Grid: Career Overview Cards */}
      <div style={styles.grid}>
        
        {/* Rating Progress Panel */}
        <div style={{ ...styles.card, gridColumn: "span 2" }} className="modern-card">
          <div style={styles.cardHeader}>
            <TrendingUp size={20} style={{ color: "var(--accent-primary)" }} />
            <h3 style={styles.cardTitle}>Progression de Division</h3>
          </div>
          
          <div style={styles.rankSection}>
            <div style={styles.rankScoreDisplay}>
              <span style={styles.rankScoreVal}>{user.score}</span>
              <span style={styles.rankScoreLabel}>POINTS DE COMBAT</span>
            </div>
            
            <div style={styles.progressBarWrapper}>
              <div style={styles.progressBarLabels}>
                <span>Division Actuelle</span>
                <span>Prochaine Division ({nextRankScore} PTS)</span>
              </div>
              <div style={styles.progressBarBg}>
                <div style={styles.progressBarFill(rankProgress)} />
              </div>
              <span style={styles.progressBarPercentage}>{rankProgress}% requis pour franchir le prochain palier</span>
            </div>
          </div>
        </div>

        {/* Win/Loss Balance Bar */}
        <div style={styles.card} className="modern-card">
          <div style={styles.cardHeader}>
            <Flame size={20} style={{ color: "#ef4444" }} />
            <h3 style={styles.cardTitle}>Équilibre des Combats</h3>
          </div>
          
          <div style={styles.balanceSection}>
            <div style={styles.balanceNumbers}>
              <div style={styles.balanceBox}>
                <span className="text-emerald font-bold" style={styles.balanceVal}>{user.wins}</span>
                <span style={styles.balanceLabel}>VICTOIRES</span>
              </div>
              <div style={styles.balanceSeparator}>VS</div>
              <div style={styles.balanceBox}>
                <span className="text-rose font-bold" style={styles.balanceVal}>{user.losses}</span>
                <span style={styles.balanceLabel}>DÉFAITES</span>
              </div>
            </div>

            <div style={styles.ratioBarBg}>
              <div style={styles.ratioBarWins(winRate)} />
              <div style={styles.ratioBarLosses(100 - winRate)} />
            </div>
            <span style={styles.balanceSub}>{winRate}% de victoires sur {totalMatches} matchs</span>
          </div>
        </div>

        {/* Combat Efficiency */}
        <div style={styles.card} className="modern-card">
          <div style={styles.cardHeader}>
            <Target size={20} style={{ color: "#10b981" }} />
            <h3 style={styles.cardTitle}>Efficacité Stratégique</h3>
          </div>
          
          <div style={styles.efficiencyList}>
            <div style={styles.efficiencyItem}>
              <span style={styles.effLabel}>Parties Jouées</span>
              <span style={styles.effValue}>{totalMatches}</span>
            </div>
            <div style={styles.efficiencyItem}>
              <span style={styles.effLabel}>Score Moyen / Match</span>
              <span style={styles.effValue}>{avgScoreGained} PTS</span>
            </div>
            <div style={styles.efficiencyItem}>
              <span style={styles.effLabel}>Tiers de combat</span>
              <span style={styles.effValue}>
                {user.score >= 1100 ? "Elite Command" : "Standard Driver"}
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Achievement badges */}
      <div style={styles.achievementsPanel} className="modern-card">
        <h3 style={{ ...styles.cardTitle, marginBottom: "20px" }}>Médailles & Distinctions</h3>
        <div style={styles.badgesRow}>
          
          {user.wins >= 1 && (
            <div style={styles.badgeItem}>
              <div style={styles.badgeIconWrap("#fbbf24")}>🥇</div>
              <h4 style={styles.badgeName}>Premier Sang</h4>
              <p style={styles.badgeDesc}>Décrocher sa toute première victoire en combat.</p>
            </div>
          )}

          {user.score >= 1100 && (
            <div style={styles.badgeItem}>
              <div style={styles.badgeIconWrap("#60a5fa")}>💎</div>
              <h4 style={styles.badgeName}>Pilote d'Élite</h4>
              <p style={styles.badgeDesc}>Franchir la barre symbolique des 1100 PTS.</p>
            </div>
          )}

          {totalMatches >= 10 && (
            <div style={styles.badgeItem}>
              <div style={styles.badgeIconWrap("#10b981")}>🚜</div>
              <h4 style={styles.badgeName}>Vétéran de l'Acier</h4>
              <p style={styles.badgeDesc}>Disputer plus de 10 batailles acharnées.</p>
            </div>
          )}

          {totalMatches === 0 && (
            <div style={styles.noAchievements}>
              <ShieldAlert size={36} style={{ color: "var(--text-secondary)", marginBottom: "10px" }} />
              <p style={{ color: "var(--text-secondary)", fontSize: "14px" }}>
                Disputez votre premier match en WebGL pour débloquer des médailles honorifiques.
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

const styles: Record<string, any> = {
  page: {
    padding: "30px 20px",
    textAlign: "left",
  },
  header: {
    marginBottom: "30px",
  },
  headerTitleGroup: {
    display: "flex",
    alignItems: "center",
    gap: "10px",
    marginBottom: "6px",
  },
  title: {
    fontSize: "24px",
    fontWeight: 800,
    color: "#ffffff",
  },
  subtitle: {
    color: "var(--text-secondary)",
    fontSize: "15px",
    lineHeight: "1.5",
  },
  grid: {
    display: "grid",
    gridTemplateColumns: "1fr 1fr",
    gap: "25px",
    marginBottom: "30px",
  },
  card: {
    display: "flex",
    flexDirection: "column",
  },
  cardHeader: {
    display: "flex",
    alignItems: "center",
    gap: "10px",
    marginBottom: "20px",
    borderBottom: "1px solid var(--border-color)",
    paddingBottom: "12px",
  },
  cardTitle: {
    fontSize: "16px",
    fontWeight: 700,
    color: "#ffffff",
  },
  rankSection: {
    display: "flex",
    gap: "40px",
    alignItems: "center",
    flexWrap: "wrap",
  },
  rankScoreDisplay: {
    display: "flex",
    flexDirection: "column",
    alignItems: "flex-start",
  },
  rankScoreVal: {
    fontSize: "48px",
    fontWeight: 800,
    color: "var(--accent-primary)",
    lineHeight: "1",
  },
  rankScoreLabel: {
    fontSize: "11px",
    fontWeight: 700,
    color: "var(--text-secondary)",
    letterSpacing: "0.05em",
    marginTop: "6px",
  },
  progressBarWrapper: {
    flex: 1,
    display: "flex",
    flexDirection: "column",
    gap: "8px",
    minWidth: "250px",
  },
  progressBarLabels: {
    display: "flex",
    justifyContent: "space-between",
    fontSize: "12px",
    color: "var(--text-secondary)",
    fontWeight: 500,
  },
  progressBarBg: {
    width: "100%",
    height: "14px",
    backgroundColor: "var(--bg-primary)",
    borderRadius: "10px",
    overflow: "hidden",
    border: "1px solid var(--border-color)",
  },
  progressBarFill: (progress: number) => ({
    width: `${progress}%`,
    height: "100%",
    backgroundColor: "var(--accent-primary)",
    borderRadius: "10px",
    backgroundImage: "linear-gradient(90deg, var(--accent-primary) 0%, #60a5fa 100%)",
  }),
  progressBarPercentage: {
    fontSize: "12px",
    color: "var(--text-secondary)",
    fontStyle: "italic",
    textAlign: "left",
  },
  balanceSection: {
    display: "flex",
    flexDirection: "column",
    alignItems: "stretch",
    justifyContent: "center",
    height: "100%",
  },
  balanceNumbers: {
    display: "flex",
    justifyContent: "space-around",
    alignItems: "center",
    marginBottom: "20px",
  },
  balanceBox: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
  },
  balanceVal: {
    fontSize: "32px",
  },
  balanceLabel: {
    fontSize: "11px",
    fontWeight: 700,
    color: "var(--text-secondary)",
    marginTop: "4px",
    letterSpacing: "0.05em",
  },
  balanceSeparator: {
    fontSize: "14px",
    fontWeight: 800,
    color: "var(--text-secondary)",
    backgroundColor: "var(--bg-primary)",
    padding: "6px 12px",
    borderRadius: "50%",
    border: "1px solid var(--border-color)",
  },
  ratioBarBg: {
    width: "100%",
    height: "12px",
    borderRadius: "6px",
    overflow: "hidden",
    display: "flex",
    marginBottom: "10px",
  },
  ratioBarWins: (winsRate: number) => ({
    width: `${winsRate}%`,
    height: "100%",
    backgroundColor: "var(--accent-emerald)",
  }),
  ratioBarLosses: (lossesRate: number) => ({
    width: `${lossesRate}%`,
    height: "100%",
    backgroundColor: "var(--accent-rose)",
  }),
  balanceSub: {
    fontSize: "13px",
    color: "var(--text-secondary)",
    textAlign: "center",
  },
  efficiencyList: {
    display: "flex",
    flexDirection: "column",
    justifyContent: "center",
    height: "100%",
    gap: "16px",
  },
  efficiencyItem: {
    display: "flex",
    justifyContent: "space-between",
    paddingBottom: "10px",
    borderBottom: "1px solid rgba(255, 255, 255, 0.03)",
  },
  effLabel: {
    fontSize: "14px",
    color: "var(--text-secondary)",
  },
  effValue: {
    fontSize: "14px",
    fontWeight: 700,
    color: "#ffffff",
  },
  achievementsPanel: {
    textAlign: "left",
  },
  badgesRow: {
    display: "flex",
    gap: "20px",
    flexWrap: "wrap",
  },
  badgeItem: {
    flex: "1 1 200px",
    backgroundColor: "var(--bg-primary)",
    border: "1px solid var(--border-color)",
    borderRadius: "10px",
    padding: "20px",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    textAlign: "center",
  },
  badgeIconWrap: (glowColor: string) => ({
    fontSize: "36px",
    marginBottom: "12px",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    width: "60px",
    height: "60px",
    borderRadius: "50%",
    backgroundColor: "rgba(255, 255, 255, 0.02)",
    border: "1px solid var(--border-color)",
    boxShadow: `0 0 15px ${glowColor}22`,
  }),
  badgeName: {
    fontSize: "15px",
    fontWeight: 700,
    color: "#ffffff",
    marginBottom: "6px",
  },
  badgeDesc: {
    fontSize: "12px",
    color: "var(--text-secondary)",
    lineHeight: "1.4",
  },
  noAchievements: {
    width: "100%",
    textAlign: "center",
    padding: "20px 0",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
  }
};

// Add responsive grid styles for stats page
if (typeof document !== "undefined") {
  const style = document.createElement("style");
  style.textContent = `
    @media (max-width: 800px) {
      #root > div > div.container > div:nth-child(2) {
        grid-template-columns: 1fr !important;
      }
      #root > div > div.container > div:nth-child(2) > div:first-child {
        grid-column: span 1 !important;
      }
    }
  `;
  document.head.appendChild(style);
}
export default Stats;

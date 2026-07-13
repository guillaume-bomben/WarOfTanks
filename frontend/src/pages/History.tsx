import React, { useState, useEffect } from "react";
import { useAuth } from "../context/AuthContext";
import { matchService } from "../services/matchService";
import type { Match } from "../services/matchService";
import { History as HistoryIcon, Calendar, Clock, Sword, Award, AlertTriangle } from "lucide-react";

export const History: React.FC = () => {
  const { user } = useAuth();
  const [matches, setMatches] = useState<Match[]>([]);
  const [filter, setFilter] = useState<"all" | "wins" | "losses">("all");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchHistory = async () => {
      try {
        const data = await matchService.getMatches();
        if (user) {
          // Filter matches involving current user
          const userMatches = data.filter(
            (m) => m.playerA?._id === user._id || m.playerB?._id === user._id
          );
          
          // Sort newest first
          userMatches.sort(
            (a, b) => new Date(b.playedAt).getTime() - new Date(a.playedAt).getTime()
          );
          setMatches(userMatches);
        }
      } catch (err) {
        console.error("Error fetching match history:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchHistory();
  }, [user]);

  if (!user) return null;

  const filteredMatches = matches.filter((m) => {
    const isWinner = m.winner?._id === user._id;
    if (filter === "wins") return isWinner;
    if (filter === "losses") return !isWinner;
    return true;
  });

  const formatDuration = (seconds: number) => {
    if (!seconds) return "0s";
    const mins = Math.floor(seconds / 60);
    const secs = seconds % 60;
    return mins > 0 ? `${mins}m ${secs}s` : `${secs}s`;
  };

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString("fr-FR", {
      day: "numeric",
      month: "long",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  return (
    <div style={styles.page} className="animate-fade-in container">
      {/* Header */}
      <div style={styles.header}>
        <div style={styles.headerTitleGroup}>
          <HistoryIcon size={28} style={{ color: "var(--accent-primary)" }} />
          <h2 style={styles.title}>Historique des Matchs</h2>
        </div>
        <p style={styles.subtitle}>
          Retrouvez la chronologie complète de vos combats d'acier et retracez vos victoires passées.
        </p>
      </div>

      {/* Toolbar / Filters */}
      <div style={styles.toolbar}>
        <div style={styles.filterGroup}>
          <button
            onClick={() => setFilter("all")}
            style={{
              ...styles.filterBtn,
              ...(filter === "all" ? styles.filterActive : {}),
            }}
          >
            Tous les matchs ({matches.length})
          </button>
          <button
            onClick={() => setFilter("wins")}
            style={{
              ...styles.filterBtn,
              ...(filter === "wins" ? styles.filterActive : {}),
              color: filter === "wins" ? "#10b981" : "var(--text-secondary)",
              borderColor: filter === "wins" ? "rgba(16, 185, 129, 0.4)" : "var(--border-color)",
              backgroundColor: filter === "wins" ? "rgba(16, 185, 129, 0.08)" : "transparent",
            }}
          >
            Victoires ({matches.filter((m) => m.winner?._id === user._id).length})
          </button>
          <button
            onClick={() => setFilter("losses")}
            style={{
              ...styles.filterBtn,
              ...(filter === "losses" ? styles.filterActive : {}),
              color: filter === "losses" ? "#ef4444" : "var(--text-secondary)",
              borderColor: filter === "losses" ? "rgba(239, 68, 68, 0.4)" : "var(--border-color)",
              backgroundColor: filter === "losses" ? "rgba(239, 68, 68, 0.08)" : "transparent",
            }}
          >
            Défaites ({matches.filter((m) => m.winner?._id !== user._id).length})
          </button>
        </div>
      </div>

      {/* Match History List */}
      {loading ? (
        <div style={styles.loader}>Lecture des archives de combat...</div>
      ) : filteredMatches.length === 0 ? (
        <div style={styles.emptyCard} className="modern-card">
          <AlertTriangle size={36} style={{ color: "var(--text-secondary)", marginBottom: "15px" }} />
          <p style={{ color: "var(--text-secondary)", fontSize: "15px" }}>
            Aucun combat répertorié pour ce filtre. Lancez une partie pour enrichir votre historique.
          </p>
        </div>
      ) : (
        <div style={styles.matchGrid}>
          {filteredMatches.map((match) => {
            const isWinner = match.winner?._id === user._id;
            const opponent = match.playerA?._id === user._id ? match.playerB : match.playerA;
            const userScore = match.playerA?._id === user._id ? match.scoreTeamA : match.scoreTeamB;
            const oppScore = match.playerA?._id === user._id ? match.scoreTeamB : match.scoreTeamA;

            return (
              <div
                key={match._id}
                style={{
                  ...styles.matchCard,
                  borderColor: isWinner ? "rgba(16, 185, 129, 0.25)" : "rgba(239, 68, 68, 0.25)",
                }}
                className="modern-card"
              >
                {/* Result header */}
                <div style={styles.cardHeader}>
                  <div style={styles.outcomeGroup}>
                    <div style={styles.iconWrap(isWinner ? "#10b981" : "#ef4444")}>
                      {isWinner ? <Award size={18} /> : <Sword size={18} />}
                    </div>
                    <span style={{
                      ...styles.outcomeText,
                      color: isWinner ? "var(--accent-emerald)" : "var(--accent-rose)"
                    }}>
                      {isWinner ? "VICTOIRE" : "DÉFAITE"}
                    </span>
                  </div>
                  <div style={styles.scoreText(isWinner ? "var(--accent-emerald)" : "var(--accent-rose)")}>
                    {userScore} - {oppScore}
                  </div>
                </div>

                {/* Details */}
                <div style={styles.cardBody}>
                  <div style={styles.detailRow}>
                    <span style={styles.detailLabel}>OPPOSANT</span>
                    <span style={styles.opponentName}>
                      {opponent?.username || "Système_AI (Medium)"}
                    </span>
                  </div>

                  <div style={styles.metaRow}>
                    <div style={styles.metaItem}>
                      <Clock size={14} style={{ color: "var(--text-secondary)" }} />
                      <span>{formatDuration(match.durationSeconds)}</span>
                    </div>

                    <div style={styles.metaItem}>
                      <Calendar size={14} style={{ color: "var(--text-secondary)" }} />
                      <span>{formatDate(match.playedAt)}</span>
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}
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
  toolbar: {
    marginBottom: "25px",
  },
  filterGroup: {
    display: "flex",
    gap: "12px",
    flexWrap: "wrap",
  },
  filterBtn: {
    padding: "8px 16px",
    fontSize: "14px",
    fontWeight: 600,
    borderRadius: "8px",
    border: "1px solid var(--border-color)",
    backgroundColor: "transparent",
    color: "var(--text-secondary)",
    cursor: "pointer",
    transition: "all 0.15s ease",
  },
  filterActive: {
    backgroundColor: "rgba(59, 130, 246, 0.08)",
    borderColor: "rgba(59, 130, 246, 0.4)",
    color: "var(--accent-primary)",
  },
  loader: {
    textAlign: "center",
    padding: "40px 0",
    color: "var(--text-secondary)",
    fontSize: "15px",
  },
  emptyCard: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    padding: "40px",
    textAlign: "center",
  },
  matchGrid: {
    display: "grid",
    gridTemplateColumns: "repeat(auto-fill, minmax(360px, 1fr))",
    gap: "20px",
  },
  matchCard: {
    display: "flex",
    flexDirection: "column",
    padding: "20px",
    transition: "transform 0.15s ease, border-color 0.15s ease",
  },
  cardHeader: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    borderBottom: "1px solid var(--border-color)",
    paddingBottom: "14px",
    marginBottom: "14px",
  },
  outcomeGroup: {
    display: "flex",
    alignItems: "center",
    gap: "10px",
  },
  iconWrap: (color: string) => ({
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    width: "28px",
    height: "28px",
    borderRadius: "6px",
    backgroundColor: `${color}15`,
    color: color,
    border: `1px solid ${color}25`,
  }),
  outcomeText: {
    fontWeight: 700,
    fontSize: "14px",
    letterSpacing: "0.02em",
  },
  scoreText: (color: string) => ({
    fontSize: "18px",
    fontWeight: 800,
    color: color,
  }),
  cardBody: {
    display: "flex",
    flexDirection: "column",
    gap: "14px",
  },
  detailRow: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
  },
  detailLabel: {
    fontSize: "11px",
    fontWeight: 700,
    color: "var(--text-secondary)",
    letterSpacing: "0.05em",
  },
  opponentName: {
    fontWeight: 600,
    fontSize: "14px",
    color: "#ffffff",
  },
  metaRow: {
    display: "flex",
    gap: "15px",
    fontSize: "12px",
    color: "var(--text-secondary)",
    borderTop: "1px solid rgba(255, 255, 255, 0.02)",
    paddingTop: "12px",
  },
  metaItem: {
    display: "flex",
    alignItems: "center",
    gap: "6px",
  }
};
export default History;

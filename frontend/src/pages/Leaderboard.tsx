import React, { useState, useEffect } from "react";
import { useAuth } from "../context/AuthContext";
import { authService } from "../services/authService";
import type { Player } from "../services/authService";
import { Trophy, Search, Medal } from "lucide-react";

export const Leaderboard: React.FC = () => {
  const { user } = useAuth();
  const [players, setPlayers] = useState<Player[]>([]);
  const [searchQuery, setSearchQuery] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchLeaderboard = async () => {
      try {
        const data = await authService.getLeaderboard();
        setPlayers(data);
      } catch (err) {
        console.error("Error loading leaderboard:", err);
      } finally {
        setLoading(false);
      }
    };
    fetchLeaderboard();
  }, []);

  const filteredPlayers = players.filter((p) =>
    p.username.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const getRankBadge = (rank: number) => {
    if (rank === 1) return <Medal size={18} style={{ color: "#fbbf24" }} />; // Gold
    if (rank === 2) return <Medal size={18} style={{ color: "#cbd5e1" }} />; // Silver
    if (rank === 3) return <Medal size={18} style={{ color: "#b45309" }} />; // Bronze
    return <span style={styles.rankNumber}>{rank}</span>;
  };

  return (
    <div style={styles.page} className="animate-fade-in container">
      {/* Header */}
      <div style={styles.header}>
        <div style={styles.headerTitleGroup}>
          <Trophy size={28} style={{ color: "#fbbf24" }} />
          <h2 style={styles.title}>Classement Général</h2>
        </div>
        <p style={styles.subtitle}>
          Découvrez l'élite des pilotes de char et mesurez-vous aux meilleurs pour atteindre la première place.
        </p>
      </div>

      {/* Toolbar */}
      <div style={styles.toolbar}>
        <div style={styles.searchContainer}>
          <Search size={18} style={styles.searchIcon} />
          <input
            type="text"
            placeholder="Rechercher un pilote..."
            style={styles.searchInput}
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
      </div>

      {/* Leaderboard Table */}
      {loading ? (
        <div style={styles.loader}>Chargement des profils tactiques...</div>
      ) : (
        <div className="modern-table-container">
          <table className="modern-table">
            <thead>
              <tr>
                <th style={{ width: "80px", textAlign: "center" }}>Rang</th>
                <th>Pilote</th>
                <th style={{ textAlign: "right" }}>Score</th>
                <th style={{ textAlign: "right" }}>Matchs</th>
                <th style={{ textAlign: "right" }}>V / D</th>
                <th style={{ textAlign: "right" }}>Taux de Victoire</th>
              </tr>
            </thead>
            <tbody>
              {filteredPlayers.length === 0 ? (
                <tr>
                  <td colSpan={6} style={styles.emptyCell}>
                    Aucun pilote ne correspond à votre recherche.
                  </td>
                </tr>
              ) : (
                filteredPlayers.map((p, idx) => {
                  const rank = idx + 1;
                  const isCurrentUser = user?._id === p._id;
                  const total = p.matchesPlayed || 0;
                  const wr = total > 0 ? Math.round((p.wins / total) * 100) : 0;
                  
                  return (
                    <tr key={p._id} className={isCurrentUser ? "highlighted" : ""}>
                      <td style={{ textAlign: "center", fontWeight: "bold" }}>
                        <div style={styles.rankCellContent}>
                          {getRankBadge(rank)}
                        </div>
                      </td>
                      <td>
                        <div style={styles.playerCellContent}>
                          {isCurrentUser && (
                            <span className="badge badge-info" style={{ marginRight: "8px", fontSize: "10px", padding: "1px 6px" }}>
                              MOI
                            </span>
                          )}
                          <span style={{ fontWeight: isCurrentUser ? 700 : 500 }}>
                            {p.username}
                          </span>
                        </div>
                      </td>
                      <td style={{ textAlign: "right", fontWeight: "bold", color: "#ffffff" }}>
                        {p.score} PTS
                      </td>
                      <td style={{ textAlign: "right", color: "var(--text-secondary)" }}>
                        {total}
                      </td>
                      <td style={{ textAlign: "right", color: "var(--text-secondary)" }}>
                        <span className="text-emerald">{p.wins || 0}</span>
                        <span> / </span>
                        <span className="text-rose">{p.losses || 0}</span>
                      </td>
                      <td style={{ textAlign: "right", fontWeight: "600", color: wr >= 50 ? "var(--accent-emerald)" : "var(--accent-rose)" }}>
                        {wr}%
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

const styles: Record<string, React.CSSProperties> = {
  page: {
    padding: "30px 20px",
    textAlign: "left",
  },
  header: {
    marginBottom: "25px",
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
    marginBottom: "20px",
  },
  searchContainer: {
    position: "relative",
    maxWidth: "350px",
    width: "100%",
  },
  searchIcon: {
    position: "absolute",
    left: "14px",
    top: "50%",
    transform: "translateY(-50%)",
    color: "var(--text-secondary)",
    pointerEvents: "none",
  },
  searchInput: {
    width: "100%",
    padding: "10px 16px 10px 42px",
    backgroundColor: "var(--bg-secondary)",
    border: "1px solid var(--border-color)",
    borderRadius: "8px",
    color: "var(--text-primary)",
    fontSize: "14px",
    fontFamily: "var(--sans)",
    outline: "none",
    transition: "border-color 0.15s ease",
  },
  loader: {
    textAlign: "center",
    padding: "40px 0",
    color: "var(--text-secondary)",
    fontSize: "15px",
  },
  rankNumber: {
    color: "var(--text-secondary)",
  },
  rankCellContent: {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    height: "24px",
  },
  playerCellContent: {
    display: "flex",
    alignItems: "center",
  },
  emptyCell: {
    textAlign: "center",
    padding: "30px",
    color: "var(--text-secondary)",
  }
};

// Add input focus design globally
if (typeof document !== "undefined") {
  const style = document.createElement("style");
  style.textContent = `
    nav input:focus, .container input:focus {
      border-color: var(--accent-primary) !important;
    }
  `;
  document.head.appendChild(style);
}
export default Leaderboard;

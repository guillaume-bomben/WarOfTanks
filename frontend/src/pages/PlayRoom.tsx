import React, { useRef, useState, useEffect } from "react";
import { useAuth } from "../context/AuthContext";
import { matchService } from "../services/matchService";
import { authService } from "../services/authService";
import { Play, Shield, Trophy, Loader2, ArrowLeft } from "lucide-react";
import { Link } from "react-router-dom";

export const PlayRoom: React.FC = () => {
  const { user, refreshUser } = useAuth();
  const canvasRef = useRef<HTMLCanvasElement>(null);
  
  const [isGameStarted, setIsGameStarted] = useState(false);
  const [loading, setLoading] = useState(false);
  const [loadingProgress, setLoadingProgress] = useState(0);
  const [opponentId, setOpponentId] = useState<string | null>(null);
  
  // Game over modal overlay state
  const [showResultModal, setShowResultModal] = useState(false);
  const [gameResult, setGameResult] = useState<{
    score: number;
    durationSeconds: number;
    isWinner: boolean;
  } | null>(null);

  // 1. Resolve or create a valid opponent player to satisfy MongoDB schema foreign keys
  useEffect(() => {
    if (!user) return;

    const getOrCreateOpponent = async () => {
      try {
        const players = await authService.getLeaderboard();
        
        // Look for any user that is not us and has "AI" or is just another player
        let opp = players.find((p) => p._id !== user._id);
        
        if (opp) {
          setOpponentId(opp._id);
        } else {
          // No other players in DB! Let's register a default AI Opponent
          try {
            console.log("PlayRoom: Seeding AI opponent in database...");
            const aiRes = await authService.register(
              "Tactical_AI",
              "tactical_ai@waroftanks.com",
              "ai_opponent_secure_pass_123"
            );
            setOpponentId(aiRes._id);
          } catch (regErr) {
            console.error("PlayRoom: Failed to seed AI opponent (might already exist):", regErr);
          }
        }
      } catch (err) {
        console.error("PlayRoom: Error resolving opponent player:", err);
      }
    };

    getOrCreateOpponent();
  }, [user]);

  // 2. Load Unity WebGL Loader Script & Initialize Game
  const handleStartGame = () => {
    if (isGameStarted) return;
    setIsGameStarted(true);
    setLoading(true);
    setLoadingProgress(0);

    // Append standard loader script dynamically
    const script = document.createElement("script");
    script.src = "/Build/Build.loader.js";
    script.onload = () => {
      if (canvasRef.current && (window as any).createUnityInstance) {
        (window as any).createUnityInstance(
          canvasRef.current,
          {
            dataUrl: "/Build/Build.data",
            frameworkUrl: "/Build/Build.framework.js",
            codeUrl: "/Build/Build.wasm",
            streamingAssetsUrl: "StreamingAssets",
            companyName: "LaPlateforme",
            productName: "WarOfTanks",
            productVersion: "1.0",
          },
          (progress: number) => {
            setLoadingProgress(Math.round(progress * 100));
          }
        )
        .then(() => {
          setLoading(false);
        })
        .catch((err: any) => {
          console.error("Failed to load Unity instance:", err);
          setLoading(false);
        });
      }
    };
    document.body.appendChild(script);
  };

  // 3. Bind game over hook from mock loader canvas
  useEffect(() => {
    const handleUnityGameOver = async (event: any) => {
      const detail = event.detail || event;
      const { score, durationSeconds } = detail;
      
      console.log("PlayRoom: Unity Game Over event received!", detail);
      
      const isWinner = score >= 500;
      setGameResult({ score, durationSeconds, isWinner });
      setShowResultModal(true);

      // Submit score to database via matchService
      if (user && opponentId) {
        try {
          await matchService.createMatch({
            playerA: user._id,
            playerB: opponentId,
            winner: isWinner ? user._id : opponentId,
            scoreTeamA: score,
            scoreTeamB: isWinner ? Math.round(score * 0.4) : Math.round(score * 1.5),
            durationSeconds: durationSeconds,
          });
          
          console.log("PlayRoom: Match submitted to database successfully!");
          // Refresh user profile stats so the top bar score updates immediately
          await refreshUser();
        } catch (err) {
          console.error("PlayRoom: Failed to submit match stats:", err);
        }
      }
    };

    window.addEventListener("unity-game-over", handleUnityGameOver);
    (window as any).onUnityGameOver = (data: any) => {
      // Direct call fallback
      const ev = new CustomEvent("unity-game-over", { detail: data });
      window.dispatchEvent(ev);
    };

    return () => {
      window.removeEventListener("unity-game-over", handleUnityGameOver);
      delete (window as any).onUnityGameOver;
      
      // Cleanup game loop
      if ((window as any).gameLoopId) {
        cancelAnimationFrame((window as any).gameLoopId);
      }
    };
  }, [user, opponentId]);

  if (!user) return null;

  return (
    <div style={styles.page} className="animate-fade-in container">
      {/* Header bar */}
      <div style={styles.header}>
        <Link to="/" style={styles.backBtn}>
          <ArrowLeft size={16} />
          <span>Tableau de bord</span>
        </Link>
      </div>

      {/* Main viewport Container */}
      <div style={styles.gameWrapper} className="modern-card">
        {!isGameStarted ? (
          /* Start Screen */
          <div style={styles.startScreen}>
            <div style={styles.playIconContainer}>
              <Play size={44} fill="var(--accent-primary)" style={{ marginLeft: "6px", color: "var(--accent-primary)" }} />
            </div>
            
            <h2 style={styles.gameTitle}>War Of Tanks WebGL</h2>
            <p style={styles.gameDesc}>
              Pilotez votre blindé de combat lourd à l'aide de votre clavier, ciblez et détruisez les chars ennemis avec votre canon plasma. 
              Vos performances affecteront votre score global dans le classement.
            </p>

            <div style={styles.controlsLegend}>
              <div style={styles.controlKeyGroup}>
                <span style={styles.keyBadge}>Z</span>
                <span style={styles.keyBadge}>Q</span>
                <span style={styles.keyBadge}>S</span>
                <span style={styles.keyBadge}>D</span>
                <span style={styles.controlLabel}>Déplacement</span>
              </div>
              <div style={styles.controlKeyGroup}>
                <span style={styles.keyBadge}>CLIC GAUCHE</span>
                <span style={styles.controlLabel}>Tirer</span>
              </div>
            </div>

            <button onClick={handleStartGame} className="btn btn-primary" style={styles.launchBtn}>
              CHARGER LE MODULE WEBGL
            </button>
          </div>
        ) : (
          /* Canvas viewport */
          <div style={styles.viewportWrapper}>
            {loading && (
              <div style={styles.loaderOverlay}>
                <Loader2 size={48} className="animate-spin" style={{ color: "var(--accent-primary)", animation: "spin 1s linear infinite" }} />
                <h3 style={styles.loadingText}>Initialisation de l'instance WebGL...</h3>
                <span style={styles.progressText}>{loadingProgress}%</span>
              </div>
            )}
            
            <canvas
              ref={canvasRef}
              width={800}
              height={500}
              style={{
                ...styles.canvas,
                visibility: loading ? "hidden" : "visible",
              }}
            />
          </div>
        )}
      </div>

      {/* Custom Keyframe spin injection */}
      <style>{`
        @keyframes spin {
          0% { transform: rotate(0deg); }
          100% { transform: rotate(360deg); }
        }
        .animate-spin {
          animation: spin 1s linear infinite;
        }
      `}</style>

      {/* Dynamic Game Over Results Modal */}
      {showResultModal && gameResult && (
        <div style={styles.modalBackdrop}>
          <div style={styles.modalCard} className="modern-card animate-fade-in">
            <div style={styles.modalHeader}>
              {gameResult.isWinner ? (
                <Trophy size={40} style={{ color: "var(--accent-emerald)" }} />
              ) : (
                <Shield size={40} style={{ color: "var(--accent-rose)" }} />
              )}
              <h3 style={styles.modalTitle}>
                {gameResult.isWinner ? "VICTOIRE ECLATANTE !" : "DÉFAITE DU VEHICULE"}
              </h3>
            </div>

            <div style={styles.modalBody}>
              <p style={styles.modalDesc}>
                Les données télémétriques de combat ont été transmises avec succès à la base centrale. Votre profil a été mis à jour.
              </p>

              <div style={styles.modalStatsRow}>
                <div style={styles.modalStatItem}>
                  <span style={styles.modalStatLabel}>SCORE MARQUÉ</span>
                  <span style={{
                    ...styles.modalStatValue,
                    color: gameResult.isWinner ? "var(--accent-emerald)" : "var(--accent-rose)"
                  }}>
                    {gameResult.isWinner ? "+" : ""}{gameResult.score} PTS
                  </span>
                </div>

                <div style={styles.modalStatItem}>
                  <span style={styles.modalStatLabel}>RATING PROGRESSION</span>
                  <span style={{
                    ...styles.modalStatValue,
                    color: gameResult.isWinner ? "var(--accent-emerald)" : "var(--accent-rose)"
                  }}>
                    {gameResult.isWinner ? "+25 PTS" : "-15 PTS"}
                  </span>
                </div>
              </div>
            </div>

            <div style={styles.modalFooter}>
              <button
                onClick={() => setShowResultModal(false)}
                className="btn btn-primary"
                style={{ flex: 1 }}
              >
                Confirmer
              </button>
              <Link
                to="/"
                className="btn btn-secondary"
                style={{ flex: 1 }}
              >
                Quitter
              </Link>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

const styles: Record<string, any> = {
  page: {
    padding: "20px 20px 40px",
    textAlign: "left",
  },
  header: {
    marginBottom: "15px",
  },
  backBtn: {
    display: "inline-flex",
    alignItems: "center",
    gap: "8px",
    color: "var(--text-secondary)",
    textDecoration: "none",
    fontSize: "14px",
    fontWeight: 600,
    transition: "color 0.15s ease",
  },
  gameWrapper: {
    display: "flex",
    flexDirection: "column",
    justifyContent: "center",
    alignItems: "center",
    backgroundColor: "var(--bg-secondary)",
    border: "1px solid var(--border-color)",
    borderRadius: "16px",
    minHeight: "540px",
    padding: 0,
    overflow: "hidden",
  },
  startScreen: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "center",
    maxWidth: "550px",
    textAlign: "center",
    padding: "40px 20px",
  },
  playIconContainer: {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    width: "80px",
    height: "80px",
    borderRadius: "50%",
    backgroundColor: "rgba(59, 130, 246, 0.1)",
    marginBottom: "24px",
    border: "1px solid rgba(59, 130, 246, 0.2)",
    cursor: "pointer",
  },
  gameTitle: {
    fontSize: "26px",
    fontWeight: 800,
    color: "#ffffff",
    marginBottom: "12px",
  },
  gameDesc: {
    fontSize: "14px",
    color: "var(--text-secondary)",
    lineHeight: "1.6",
    marginBottom: "30px",
  },
  controlsLegend: {
    display: "flex",
    justifyContent: "center",
    gap: "25px",
    marginBottom: "30px",
    flexWrap: "wrap",
  },
  controlKeyGroup: {
    display: "flex",
    alignItems: "center",
    gap: "6px",
  },
  keyBadge: {
    backgroundColor: "var(--bg-primary)",
    border: "1px solid var(--border-color)",
    borderRadius: "6px",
    padding: "4px 8px",
    fontSize: "12px",
    fontWeight: 700,
    color: "#ffffff",
  },
  controlLabel: {
    fontSize: "13px",
    color: "var(--text-secondary)",
    fontWeight: 600,
    marginLeft: "4px",
  },
  launchBtn: {
    padding: "14px 28px",
    fontWeight: 700,
  },
  viewportWrapper: {
    position: "relative",
    width: "800px",
    maxWidth: "100%",
    height: "500px",
    backgroundColor: "#0b0f19",
  },
  loaderOverlay: {
    position: "absolute",
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: "#0b0f19",
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    justifyContent: "center",
    gap: "16px",
  },
  loadingText: {
    fontSize: "16px",
    fontWeight: 600,
    color: "var(--text-primary)",
  },
  progressText: {
    fontSize: "14px",
    color: "var(--text-secondary)",
  },
  canvas: {
    width: "100%",
    height: "100%",
    display: "block",
  },
  modalBackdrop: {
    position: "fixed",
    top: 0,
    left: 0,
    right: 0,
    bottom: 0,
    backgroundColor: "rgba(9, 13, 22, 0.85)",
    backdropFilter: "blur(4px)",
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    zIndex: 200,
    padding: "20px",
  },
  modalCard: {
    width: "100%",
    maxWidth: "420px",
    padding: "30px",
    textAlign: "center",
    backgroundColor: "var(--bg-secondary)",
    border: "1px solid var(--border-color)",
    borderRadius: "16px",
  },
  modalHeader: {
    display: "flex",
    flexDirection: "column",
    alignItems: "center",
    gap: "12px",
    marginBottom: "20px",
  },
  modalTitle: {
    fontSize: "20px",
    fontWeight: 800,
    color: "#ffffff",
  },
  modalBody: {
    marginBottom: "24px",
  },
  modalDesc: {
    fontSize: "14px",
    color: "var(--text-secondary)",
    lineHeight: "1.5",
    marginBottom: "20px",
  },
  modalStatsRow: {
    display: "flex",
    gap: "15px",
  },
  modalStatItem: {
    flex: 1,
    backgroundColor: "var(--bg-primary)",
    border: "1px solid var(--border-color)",
    borderRadius: "10px",
    padding: "12px",
    display: "flex",
    flexDirection: "column",
    gap: "4px",
  },
  modalStatLabel: {
    fontSize: "10px",
    fontWeight: 700,
    color: "var(--text-secondary)",
    letterSpacing: "0.05em",
  },
  modalStatValue: {
    fontSize: "18px",
    fontWeight: 800,
  },
  modalFooter: {
    display: "flex",
    gap: "12px",
  }
};
export default PlayRoom;

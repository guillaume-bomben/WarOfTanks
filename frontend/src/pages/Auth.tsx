import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { Shield, Mail, Lock, User, AlertCircle, ArrowRight } from "lucide-react";

export const Auth: React.FC = () => {
  const { user, login, register, error, clearError } = useAuth();
  const navigate = useNavigate();
  const [isLogin, setIsLogin] = useState(true);

  // Form states
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [localLoading, setLocalLoading] = useState(false);
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => {
    if (user) {
      navigate("/");
    }
    clearError();
    setValidationError(null);
  }, [user, isLogin, navigate]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);

    // Basic Validation
    if (!email || !password) {
      setValidationError("Veuillez remplir tous les champs obligatoires.");
      return;
    }

    if (!isLogin && !username) {
      setValidationError("Veuillez spécifier un nom d'utilisateur.");
      return;
    }

    if (password.length < 6) {
      setValidationError("Le mot de passe doit comporter au moins 6 caractères.");
      return;
    }

    setLocalLoading(true);
    try {
      if (isLogin) {
        await login(email, password);
      } else {
        await register(username, email, password);
      }
      navigate("/");
    } catch (err) {
      console.error("Auth error:", err);
    } finally {
      setLocalLoading(false);
    }
  };

  return (
    <div style={styles.pageContainer} className="animate-fade-in">
      <div style={styles.authWrapper} className="modern-card">
        {/* Left Column: Visual branding */}
        <div style={styles.infoCol}>
          <div style={styles.logoBadge}>
            <Shield size={36} style={{ color: "var(--accent-primary)" }} />
          </div>
          <h2 style={styles.infoTitle}>WarOfTanks</h2>
          <p style={styles.infoDesc}>
            Affrontez des joueurs du monde entier dans des combats de chars frénétiques. 
            Suivez vos statistiques en temps réel et montez au classement général.
          </p>
          
          <div style={styles.statsPreview}>
            <div style={styles.statMiniCard}>
              <span style={styles.statMiniVal}>1000+</span>
              <span style={styles.statMiniLabel}>Joueurs Actifs</span>
            </div>
            <div style={styles.statMiniCard}>
              <span style={styles.statMiniVal}>50k+</span>
              <span style={styles.statMiniLabel}>Matchs Joués</span>
            </div>
          </div>
        </div>

        {/* Right Column: Interactive Form */}
        <div style={styles.formCol}>
          <div style={styles.formHeader}>
            <h3 style={styles.formTitle}>
              {isLogin ? "Connexion Joueur" : "Créer un Compte"}
            </h3>
            <p style={styles.formSub}>
              {isLogin ? "Ravi de vous revoir sur le champ de bataille !" : "Rejoignez la bataille dès maintenant."}
            </p>
          </div>

          <form onSubmit={handleSubmit} style={styles.form}>
            {/* Context or Local Error Alert */}
            {(error || validationError) && (
              <div style={styles.alertError}>
                <AlertCircle size={20} style={{ flexShrink: 0 }} />
                <span>{validationError || error}</span>
              </div>
            )}

            {!isLogin && (
              <div className="form-group">
                <label className="form-label">Nom de pilote</label>
                <div style={styles.inputContainer}>
                  <User size={18} style={styles.inputIcon} />
                  <input
                    type="text"
                    className="form-input"
                    placeholder="ex: CyberCommander"
                    value={username}
                    onChange={(e) => setUsername(e.target.value)}
                    style={styles.inputPadding}
                    required={!isLogin}
                  />
                </div>
              </div>
            )}

            <div className="form-group">
              <label className="form-label">Adresse Email</label>
              <div style={styles.inputContainer}>
                <Mail size={18} style={styles.inputIcon} />
                <input
                  type="email"
                  className="form-input"
                  placeholder="pilote@plateforme.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  style={styles.inputPadding}
                  required
                />
              </div>
            </div>

            <div className="form-group">
              <label className="form-label">Mot de passe</label>
              <div style={styles.inputContainer}>
                <Lock size={18} style={styles.inputIcon} />
                <input
                  type="password"
                  className="form-input"
                  placeholder="••••••••"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  style={styles.inputPadding}
                  required
                />
              </div>
            </div>

            <button
              type="submit"
              className="btn btn-primary"
              style={styles.submitBtn}
              disabled={localLoading}
            >
              <span>{localLoading ? "Chargement..." : isLogin ? "Se Connecter" : "S'inscrire"}</span>
              {!localLoading && <ArrowRight size={18} />}
            </button>
          </form>

          <div style={styles.formFooter}>
            <span style={{ color: "var(--text-secondary)" }}>
              {isLogin ? "Nouveau sur WarOfTanks ?" : "Vous possédez déjà un compte ?"}
            </span>
            <button
              style={styles.toggleBtn}
              onClick={() => setIsLogin(!isLogin)}
            >
              {isLogin ? "Créer un profil" : "Se connecter"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

const styles: Record<string, React.CSSProperties> = {
  pageContainer: {
    display: "flex",
    justifyContent: "center",
    alignItems: "center",
    minHeight: "calc(100vh - 70px)",
    padding: "20px",
    background: "radial-gradient(circle at 50% 50%, var(--bg-secondary) 0%, var(--bg-primary) 100%)",
  },
  authWrapper: {
    display: "flex",
    width: "100%",
    maxWidth: "850px",
    padding: 0,
    overflow: "hidden",
    borderRadius: "16px",
    backgroundColor: "var(--bg-secondary)",
    border: "1px solid var(--border-color)",
  },
  infoCol: {
    flex: 1,
    padding: "40px",
    display: "flex",
    flexDirection: "column",
    justifyContent: "center",
    backgroundColor: "rgba(255, 255, 255, 0.01)",
    borderRight: "1px solid var(--border-color)",
    backgroundImage: "radial-gradient(circle at 0% 0%, rgba(59, 130, 246, 0.05) 0%, transparent 50%)",
  },
  logoBadge: {
    display: "flex",
    alignItems: "center",
    justifyContent: "center",
    width: "60px",
    height: "60px",
    borderRadius: "12px",
    backgroundColor: "rgba(59, 130, 246, 0.1)",
    marginBottom: "20px",
  },
  infoTitle: {
    fontSize: "28px",
    fontWeight: 800,
    marginBottom: "15px",
    color: "#ffffff",
    letterSpacing: "-0.5px",
  },
  infoDesc: {
    color: "var(--text-secondary)",
    fontSize: "15px",
    lineHeight: "1.6",
    marginBottom: "30px",
  },
  statsPreview: {
    display: "flex",
    gap: "15px",
  },
  statMiniCard: {
    flex: 1,
    backgroundColor: "var(--bg-primary)",
    border: "1px solid var(--border-color)",
    borderRadius: "8px",
    padding: "12px",
    display: "flex",
    flexDirection: "column",
  },
  statMiniVal: {
    fontSize: "20px",
    fontWeight: 700,
    color: "var(--accent-primary)",
  },
  statMiniLabel: {
    fontSize: "11px",
    color: "var(--text-secondary)",
    textTransform: "uppercase",
    fontWeight: 600,
    marginTop: "2px",
  },
  formCol: {
    flex: 1.2,
    padding: "40px",
    display: "flex",
    flexDirection: "column",
    justifyContent: "center",
  },
  formHeader: {
    marginBottom: "25px",
  },
  formTitle: {
    fontSize: "24px",
    fontWeight: 700,
    color: "#ffffff",
    marginBottom: "8px",
  },
  formSub: {
    color: "var(--text-secondary)",
    fontSize: "14px",
  },
  form: {
    width: "100%",
  },
  alertError: {
    display: "flex",
    alignItems: "center",
    gap: "10px",
    backgroundColor: "rgba(239, 68, 68, 0.1)",
    border: "1px solid rgba(239, 68, 68, 0.2)",
    color: "var(--accent-rose)",
    borderRadius: "8px",
    padding: "12px",
    fontSize: "14px",
    marginBottom: "20px",
    lineHeight: "1.4",
  },
  inputContainer: {
    position: "relative",
    width: "100%",
  },
  inputIcon: {
    position: "absolute",
    left: "14px",
    top: "50%",
    transform: "translateY(-50%)",
    color: "var(--text-secondary)",
    pointerEvents: "none",
  },
  inputPadding: {
    paddingLeft: "42px",
  },
  submitBtn: {
    width: "100%",
    marginTop: "10px",
  },
  formFooter: {
    display: "flex",
    justifyContent: "center",
    gap: "6px",
    marginTop: "20px",
    fontSize: "14px",
  },
  toggleBtn: {
    backgroundColor: "transparent",
    border: "none",
    color: "var(--accent-primary)",
    fontWeight: 600,
    cursor: "pointer",
    padding: 0,
    fontSize: "14px",
  },
};

// Add CSS media query for mobile in doc
if (typeof document !== "undefined") {
  const style = document.createElement("style");
  style.textContent = `
    @media (max-width: 768px) {
      #root > div > div {
        flex-direction: column !important;
        max-width: 450px !important;
      }
      #root > div > div > div:first-child {
        display: none !important; /* Hide info col on small mobile */
      }
    }
  `;
  document.head.appendChild(style);
}
export default Auth;

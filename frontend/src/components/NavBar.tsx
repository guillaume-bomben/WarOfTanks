import React, { useState } from "react";
import { Link, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { Shield, Trophy, BarChart2, History, Play, LogOut, Menu, X, User } from "lucide-react";

export const NavBar: React.FC = () => {
  const { user, logout } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const [isOpen, setIsOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate("/auth");
  };

  const navLinks = [
    { name: "Tableau de Bord", path: "/", icon: Shield },
    { name: "Classement", path: "/leaderboard", icon: Trophy },
    { name: "Statistiques", path: "/stats", icon: BarChart2 },
    { name: "Historique", path: "/history", icon: History },
    { name: "Jouer WebGL", path: "/play", icon: Play },
  ];

  const isActive = (path: string) => {
    return location.pathname === path;
  };

  if (!user) return null;

  return (
    <nav style={styles.nav}>
      <div className="container" style={styles.container}>
        {/* Brand */}
        <Link to="/" style={styles.brand}>
          <span style={styles.brandLogo}>🛡️</span>
          <span style={styles.brandText}>WarOfTanks</span>
        </Link>

        {/* Mobile Toggle */}
        <button style={styles.menuToggle} onClick={() => setIsOpen(!isOpen)}>
          {isOpen ? <X size={24} /> : <Menu size={24} />}
        </button>

        {/* Links & User HUD */}
        <div style={{
          ...styles.navContent,
          display: isOpen ? "flex" : undefined
        }} className={isOpen ? "mobile-open" : ""}>
          <div style={styles.linksGroup}>
            {navLinks.map((link) => {
              const Icon = link.icon;
              const active = isActive(link.path);
              return (
                <Link
                  key={link.path}
                  to={link.path}
                  style={{
                    ...styles.link,
                    color: active ? "var(--accent-primary)" : "var(--text-secondary)",
                    backgroundColor: active ? "rgba(59, 130, 246, 0.08)" : "transparent",
                    borderColor: active ? "rgba(59, 130, 246, 0.3)" : "transparent"
                  }}
                  onClick={() => setIsOpen(false)}
                >
                  <Icon size={18} />
                  <span>{link.name}</span>
                </Link>
              );
            })}
          </div>

          <div style={styles.userHUD}>
            <div style={styles.profileBadge}>
              <User size={16} style={{ color: "var(--text-secondary)" }} />
              <span style={styles.username}>{user.username}</span>
              <span className="badge badge-info" style={styles.scoreBadge}>
                {user.score} PTS
              </span>
            </div>

            <button onClick={handleLogout} style={styles.logoutBtn} title="Se déconnecter">
              <LogOut size={18} />
              <span style={styles.logoutText}>Déconnexion</span>
            </button>
          </div>
        </div>
      </div>
      
      {/* Mobile Styling Inject */}
      <style>{`
        @media (max-width: 960px) {
          .mobile-open {
            display: flex !important;
            flex-direction: column;
            position: absolute;
            top: 70px;
            left: 0;
            right: 0;
            background-color: var(--bg-secondary);
            border-bottom: 1px solid var(--border-color);
            padding: 20px;
            gap: 20px;
            z-index: 100;
            box-shadow: var(--shadow-lg);
          }
        }
      `}</style>
    </nav>
  );
};

const styles: Record<string, React.CSSProperties> = {
  nav: {
    backgroundColor: "var(--bg-secondary)",
    borderBottom: "1px solid var(--border-color)",
    height: "70px",
    display: "flex",
    alignItems: "center",
    position: "sticky",
    top: 0,
    zIndex: 90,
  },
  container: {
    display: "flex",
    justifyContent: "space-between",
    alignItems: "center",
    height: "100%",
  },
  brand: {
    display: "flex",
    alignItems: "center",
    gap: "10px",
    textDecoration: "none",
    color: "var(--text-primary)",
    fontWeight: 800,
    fontSize: "20px",
  },
  brandLogo: {
    fontSize: "24px",
  },
  brandText: {
    letterSpacing: "-0.5px",
    background: "linear-gradient(90deg, #ffffff 0%, #cbd5e1 100%)",
    WebkitBackgroundClip: "text",
    WebkitTextFillColor: "transparent",
  },
  menuToggle: {
    display: "none",
    background: "none",
    border: "none",
    color: "var(--text-primary)",
    cursor: "pointer",
  },
  navContent: {
    display: "flex",
    alignItems: "center",
    justifyContent: "space-between",
    flex: 1,
    marginLeft: "40px",
  },
  linksGroup: {
    display: "flex",
    gap: "10px",
  },
  link: {
    display: "flex",
    alignItems: "center",
    gap: "8px",
    textDecoration: "none",
    padding: "8px 14px",
    borderRadius: "8px",
    fontSize: "14px",
    fontWeight: 600,
    border: "1px solid transparent",
    transition: "all 0.15s ease",
  },
  userHUD: {
    display: "flex",
    alignItems: "center",
    gap: "15px",
  },
  profileBadge: {
    display: "flex",
    alignItems: "center",
    gap: "10px",
    backgroundColor: "var(--bg-primary)",
    border: "1px solid var(--border-color)",
    borderRadius: "20px",
    padding: "4px 12px 4px 8px",
    fontSize: "14px",
  },
  username: {
    fontWeight: 600,
  },
  scoreBadge: {
    fontSize: "11px",
    padding: "2px 8px",
  },
  logoutBtn: {
    display: "flex",
    alignItems: "center",
    gap: "8px",
    backgroundColor: "transparent",
    border: "1px solid var(--border-color)",
    color: "var(--accent-rose)",
    padding: "8px 14px",
    borderRadius: "8px",
    fontSize: "14px",
    fontWeight: 600,
    cursor: "pointer",
    transition: "all 0.15s ease",
  },
  logoutText: {
    display: "inline",
  }
};

// Inject toggle selector for responsive
if (typeof document !== "undefined") {
  const style = document.createElement("style");
  style.textContent = `
    @media (max-width: 960px) {
      nav button {
        display: block !important;
      }
      nav div > div {
        display: none;
      }
      nav div > div.mobile-open div {
        display: flex;
        flex-direction: column;
        width: 100%;
        gap: 8px;
      }
      nav div > div.mobile-open {
        align-items: flex-start !important;
      }
      nav div > div.mobile-open div:last-child {
        flex-direction: row;
        justify-content: space-between;
        margin-top: 10px;
        padding-top: 15px;
        border-top: 1px solid var(--border-color);
      }
    }
  `;
  document.head.appendChild(style);
}
export default NavBar;

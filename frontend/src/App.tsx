import React from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider, useAuth } from "./context/AuthContext";
import { NavBar } from "./components/NavBar";
import { Auth } from "./pages/Auth";
import { Dashboard } from "./pages/Dashboard";
import { Leaderboard } from "./pages/Leaderboard";
import { Stats } from "./pages/Stats";
import { History } from "./pages/History";
import { PlayRoom } from "./pages/PlayRoom";

// Route Guard for Protected Pages
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated, loading } = useAuth();

  if (loading) {
    return (
      <div style={styles.loaderContainer}>
        <div style={styles.loaderSpinner}></div>
        <p style={styles.loaderText}>Chargement du profil pilote...</p>
      </div>
    );
  }

  if (!isAuthenticated) {
    return <Navigate to="/auth" replace />;
  }

  return <>{children}</>;
};

function AppContent() {
  const { isAuthenticated } = useAuth();
  
  return (
    <Router>
      {isAuthenticated && <NavBar />}
      <div style={styles.contentArea}>
        <Routes>
          {/* Public Auth Page */}
          <Route path="/auth" element={<Auth />} />

          {/* Protected Main Pages */}
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/leaderboard"
            element={
              <ProtectedRoute>
                <Leaderboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/stats"
            element={
              <ProtectedRoute>
                <Stats />
              </ProtectedRoute>
            }
          />
          <Route
            path="/history"
            element={
              <ProtectedRoute>
                <History />
              </ProtectedRoute>
            }
          />
          <Route
            path="/play"
            element={
              <ProtectedRoute>
                <PlayRoom />
              </ProtectedRoute>
            }
          />

          {/* Redirect all unmatched routes to home */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </div>
    </Router>
  );
}

function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}

const styles: Record<string, React.CSSProperties> = {
  contentArea: {
    flex: 1,
    display: "flex",
    flexDirection: "column",
  },
  loaderContainer: {
    display: "flex",
    flexDirection: "column",
    justifyContent: "center",
    alignItems: "center",
    minHeight: "100vh",
    backgroundColor: "var(--bg-primary)",
    gap: "20px",
  },
  loaderSpinner: {
    width: "40px",
    height: "40px",
    border: "3px solid var(--border-color)",
    borderTop: "3px solid var(--accent-primary)",
    borderRadius: "50%",
    animation: "spin 1s linear infinite",
  },
  loaderText: {
    color: "var(--text-secondary)",
    fontSize: "15px",
    fontWeight: 600,
  }
};

export default App;

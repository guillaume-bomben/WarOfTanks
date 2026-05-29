import React, { createContext, useContext, useState, useEffect } from "react";
import { authService } from "../services/authService";
import type { Player } from "../services/authService";

interface AuthContextType {
  user: Player | null;
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;
  login: (email: string, password: string) => Promise<void>;
  register: (username: string, email: string, password: string) => Promise<void>;
  logout: () => void;
  refreshUser: () => Promise<void>;
  clearError: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<Player | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const loadUser = async () => {
    const token = localStorage.getItem("wot_token");
    if (!token) {
      setLoading(false);
      return;
    }
    try {
      const player = await authService.getMe();
      setUser(player);
    } catch (err: any) {
      console.error("Failed to load user:", err);
      logout();
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUser();
  }, []);

  const login = async (email: string, password: string) => {
    setLoading(true);
    setError(null);
    try {
      const res = await authService.login(email, password);
      localStorage.setItem("wot_token", res.token);
      const player = await authService.getMe();
      setUser(player);
    } catch (err: any) {
      setError(err.message || "Email ou mot de passe invalide.");
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const register = async (username: string, email: string, password: string) => {
    setLoading(true);
    setError(null);
    try {
      const res = await authService.register(username, email, password);
      localStorage.setItem("wot_token", res.token);
      const player = await authService.getMe();
      setUser(player);
    } catch (err: any) {
      setError(err.message || "Erreur lors de la création du compte.");
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    localStorage.removeItem("wot_token");
    setUser(null);
    setError(null);
  };

  const refreshUser = async () => {
    try {
      const player = await authService.getMe();
      setUser(player);
    } catch (err) {
      console.error("Error refreshing player stats:", err);
    }
  };

  const clearError = () => setError(null);

  const value: AuthContextType = {
    user,
    isAuthenticated: !!user,
    loading,
    error,
    login,
    register,
    logout,
    refreshUser,
    clearError,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};

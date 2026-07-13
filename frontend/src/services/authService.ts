import { apiRequest } from "./api";

export interface Player {
  _id: string;
  username: string;
  email: string;
  wins: number;
  losses: number;
  matchesPlayed: number;
  score: number;
  createdAt: string;
  updatedAt: string;
}

export interface AuthResponse {
  _id: string;
  username: string;
  email: string;
  token: string;
}

export const authService = {
  async register(username: string, email: string, password: string): Promise<AuthResponse> {
    return apiRequest<AuthResponse>("/auth/register", {
      method: "POST",
      body: JSON.stringify({ username, email, password }),
    });
  },

  async login(email: string, password: string): Promise<AuthResponse> {
    return apiRequest<AuthResponse>("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
  },

  async getMe(): Promise<Player> {
    return apiRequest<Player>("/auth/me");
  },

  async getLeaderboard(): Promise<Player[]> {
    return apiRequest<Player[]>("/auth/leaderboard");
  },
};

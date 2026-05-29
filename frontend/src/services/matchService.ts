import { apiRequest } from "./api";

export interface MatchPlayerPopulated {
  _id: string;
  username: string;
}

export interface Match {
  _id: string;
  playerA: MatchPlayerPopulated;
  playerB: MatchPlayerPopulated;
  winner?: MatchPlayerPopulated;
  scoreTeamA: number;
  scoreTeamB: number;
  durationSeconds: number;
  playedAt: string;
  createdAt: string;
}

export interface CreateMatchDTO {
  playerA: string;
  playerB: string;
  winner?: string;
  scoreTeamA: number;
  scoreTeamB: number;
  durationSeconds: number;
}

export const matchService = {
  async createMatch(dto: CreateMatchDTO): Promise<Match> {
    return apiRequest<Match>("/matches", {
      method: "POST",
      body: JSON.stringify(dto),
    });
  },

  async getMatches(): Promise<Match[]> {
    return apiRequest<Match[]>("/matches");
  },
};

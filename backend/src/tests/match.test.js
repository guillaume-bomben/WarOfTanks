import { describe, it, expect } from "@jest/globals";
import request from "supertest";
import app from "../app.js";

// Cree un joueur et renvoie son _id (les matchs referencent des joueurs)
async function createPlayer(name) {
  const res = await request(app)
    .post("/api/auth/register")
    .send({
      username: name,
      email: `${name}@test.com`,
      password: "123456"
    });
  return res.body._id;
}

describe("Match endpoints", () => {

  it("POST /api/matches - cree un match et renvoie 201", async () => {
    const playerA = await createPlayer("PlayerA");
    const playerB = await createPlayer("PlayerB");

    const res = await request(app)
      .post("/api/matches")
      .send({
        playerA,
        playerB,
        winner: playerA,
        scoreTeamA: 5,
        scoreTeamB: 2,
        durationSeconds: 180
      });

    expect(res.statusCode).toBe(201);
    expect(res.body.scoreTeamA).toBe(5);
    expect(res.body.scoreTeamB).toBe(2);
    expect(res.body._id).toBeDefined();
  });

  it("POST /api/matches - met a jour les stats des joueurs (wins/losses)", async () => {
    const playerA = await createPlayer("Winner");
    const playerB = await createPlayer("Loser");

    await request(app)
      .post("/api/matches")
      .send({
        playerA,
        playerB,
        winner: playerA,
        scoreTeamA: 10,
        scoreTeamB: 3,
        durationSeconds: 120
      });

    // Le gagnant doit avoir wins: 1
    const board = await request(app).get("/api/auth/leaderboard");
    const winner = board.body.find(p => p.username === "Winner");
    const loser = board.body.find(p => p.username === "Loser");

    expect(winner.wins).toBe(1);
    expect(loser.losses).toBe(1);
  });

  it("GET /api/matches - renvoie l'historique des matchs", async () => {
    const playerA = await createPlayer("A");
    const playerB = await createPlayer("B");

    await request(app).post("/api/matches").send({
      playerA, playerB, winner: playerA,
      scoreTeamA: 1, scoreTeamB: 0, durationSeconds: 60
    });

    const res = await request(app).get("/api/matches");

    expect(res.statusCode).toBe(200);
    expect(Array.isArray(res.body)).toBe(true);
    expect(res.body.length).toBe(1);
  });
});
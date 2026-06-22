import { describe, it, expect } from "@jest/globals";
import request from "supertest";
import app from "../app.js";

describe("Auth endpoints", () => {

  const player = {
    username: "Ali",
    email: "ali@test.com",
    password: "123456"
  };

  it("POST /api/auth/register - cree un joueur et renvoie un token", async () => {
    const res = await request(app)
      .post("/api/auth/register")
      .send(player);

    expect(res.statusCode).toBe(201);
    expect(res.body.username).toBe("Ali");
    expect(res.body.email).toBe("ali@test.com");
    expect(res.body.token).toBeDefined();
    // Le hash du mot de passe ne doit JAMAIS etre renvoye
    expect(res.body.passwordHash).toBeUndefined();
  });

  it("POST /api/auth/register - refuse un email en double (400)", async () => {
    await request(app).post("/api/auth/register").send(player);

    const res = await request(app)
      .post("/api/auth/register")
      .send(player);

    expect(res.statusCode).toBe(400);
    expect(res.body.message).toBe("Player already exists");
  });

  it("POST /api/auth/login - connecte avec les bons identifiants", async () => {
    await request(app).post("/api/auth/register").send(player);

    const res = await request(app)
      .post("/api/auth/login")
      .send({ email: player.email, password: player.password });

    expect(res.statusCode).toBe(200);
    expect(res.body.token).toBeDefined();
    expect(res.body.username).toBe("Ali");
  });

  it("POST /api/auth/login - refuse un mauvais mot de passe (401)", async () => {
    await request(app).post("/api/auth/register").send(player);

    const res = await request(app)
      .post("/api/auth/login")
      .send({ email: player.email, password: "mauvais" });

    expect(res.statusCode).toBe(401);
    expect(res.body.message).toBe("Invalid email or password");
  });

  it("GET /api/auth/leaderboard - renvoie les joueurs tries par score", async () => {
    await request(app).post("/api/auth/register").send(player);

    const res = await request(app).get("/api/auth/leaderboard");

    expect(res.statusCode).toBe(200);
    expect(Array.isArray(res.body)).toBe(true);
    expect(res.body.length).toBe(1);
  });
});
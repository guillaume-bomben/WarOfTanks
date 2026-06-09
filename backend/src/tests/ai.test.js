import { describe, it, expect } from "@jest/globals";
import request from "supertest";
import app from "../app.js";

describe("AI endpoints", () => {

  it("POST /api/ai - cree une IA et renvoie 201", async () => {
    const res = await request(app)
      .post("/api/ai")
      .send({
        name: "Aggressor",
        behaviorType: "hunter",
        difficulty: "Hard"
      });

    expect(res.statusCode).toBe(201);
    expect(res.body.name).toBe("Aggressor");
    expect(res.body.behaviorType).toBe("hunter");
    expect(res.body.difficulty).toBe("Hard");
    // Valeurs par defaut du schema
    expect(res.body.wins).toBe(0);
    expect(res.body.losses).toBe(0);
    expect(res.body._id).toBeDefined();
  });

  it("POST /api/ai - refuse un nom en double (400)", async () => {
    await request(app)
      .post("/api/ai")
      .send({ name: "Aggressor", behaviorType: "hunter" });

    const res = await request(app)
      .post("/api/ai")
      .send({ name: "Aggressor", behaviorType: "defender" });

    expect(res.statusCode).toBe(400);
    expect(res.body.message).toBe("AI already exists");
  });

  it("GET /api/ai - renvoie la liste des IA", async () => {
    await request(app).post("/api/ai").send({ name: "AI_1", behaviorType: "hunter" });
    await request(app).post("/api/ai").send({ name: "AI_2", behaviorType: "defender" });

    const res = await request(app).get("/api/ai");

    expect(res.statusCode).toBe(200);
    expect(Array.isArray(res.body)).toBe(true);
    expect(res.body.length).toBe(2);
  });

  it("GET /api/ai/:id - renvoie une IA precise", async () => {
    const created = await request(app)
      .post("/api/ai")
      .send({ name: "Scout", behaviorType: "capturer" });

    const res = await request(app).get(`/api/ai/${created.body._id}`);

    expect(res.statusCode).toBe(200);
    expect(res.body.name).toBe("Scout");
  });

  it("GET /api/ai/:id - renvoie 404 si l'id n'existe pas", async () => {
    const fakeId = "6a27425c101cfc342e7b1221";
    const res = await request(app).get(`/api/ai/${fakeId}`);

    expect(res.statusCode).toBe(404);
    expect(res.body.message).toBe("AI not found");
  });
});
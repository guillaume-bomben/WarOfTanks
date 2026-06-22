import mongoose from "mongoose";
import { MongoMemoryServer } from "mongodb-memory-server";
import { beforeAll, afterEach, afterAll } from "@jest/globals";

let mongoServer;

// Avant TOUS les tests : on demarre une base Mongo temporaire en RAM
// et on y connecte mongoose. Aucune connexion a la vraie base.
beforeAll(async () => {
  mongoServer = await MongoMemoryServer.create();
  const uri = mongoServer.getUri();
  await mongoose.connect(uri);
});

// Apres CHAQUE test : on vide toutes les collections.
// Ainsi chaque test demarre avec une base propre (tests independants).
afterEach(async () => {
  const collections = mongoose.connection.collections;
  for (const key in collections) {
    await collections[key].deleteMany({});
  }
});

// Apres TOUS les tests : on ferme mongoose et on detruit la base en RAM.
afterAll(async () => {
  await mongoose.connection.dropDatabase();
  await mongoose.connection.close();
  await mongoServer.stop();
});
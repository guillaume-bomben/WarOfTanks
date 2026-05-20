import express from "express";

import {
  createMatch,
  getMatches,
  getMatchHistory
} from "../controllers/matchController.js";

const router = express.Router();

router.post("/", createMatch);

router.get("/", getMatches);

router.get("/history/:playerId", getMatchHistory);

export default router;
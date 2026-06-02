import express from "express";

import {
    registerPlayer,
    loginPlayer,
    getMe,
    getLeaderboard
} from "../controllers/authController.js";

import protect from "../middleware/authMiddleware.js";

const router = express.Router();

// Register
router.post("/register", registerPlayer);

// Login
router.post("/login", loginPlayer);

// Profil joueur connecté
router.get("/me", protect, getMe);

// Leaderboard
router.get("/leaderboard", getLeaderboard);

export default router;
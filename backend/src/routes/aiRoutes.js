import express from "express";

import {
    createAI,
    getAIs,
    getAIById
} from "../controllers/aiController.js";

const router = express.Router();

// Stocker une IA
router.post("/", createAI);

// Récupérer toutes les IA
router.get("/", getAIs);

// Récupérer une IA par id
router.get("/:id", getAIById);

export default router;
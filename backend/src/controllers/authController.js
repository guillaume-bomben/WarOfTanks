import bcrypt from "bcrypt";
import Player from "../models/Player.js";
import generateToken from "../utils/generateToken.js";

// ================= REGISTER =================

export const registerPlayer = async (req, res) => {
  try {

    const { username, email, password } = req.body;

    // Vérifier si le joueur existe déjà
    const playerExists = await Player.findOne({ email });

    if (playerExists) {
      return res.status(400).json({
        message: "Player already exists"
      });
    }

    // Hash mot de passe
    const salt = await bcrypt.genSalt(10);

    const passwordHash = await bcrypt.hash(password, salt);

    // Création joueur
    const player = await Player.create({
      username,
      email,
      passwordHash
    });

    // Réponse + token JWT
    res.status(201).json({
      _id: player._id,
      username: player.username,
      email: player.email,
      token: generateToken(player._id)
    });

  } catch (error) {

    res.status(500).json({
      message: error.message
    });
  }
};

// ================= LOGIN =================

export const loginPlayer = async (req, res) => {

  try {

    const { email, password } = req.body;

    // Chercher joueur
    const player = await Player.findOne({ email });

    // Vérifier mot de passe
    if (
      player &&
      (await bcrypt.compare(password, player.passwordHash))
    ) {

      res.json({
        _id: player._id,
        username: player.username,
        email: player.email,
        token: generateToken(player._id)
      });

    } else {

      res.status(401).json({
        message: "Invalid email or password"
      });
    }

  } catch (error) {

    res.status(500).json({
      message: error.message
    });
  }
};

// ================= GET CURRENT PLAYER =================

export const getMe = async (req, res) => {

  res.json(req.player);
};

// ================= LEADERBOARD =================

export const getLeaderboard = async (req, res) => {

  try {

    const players = await Player.find()
      .select("username score wins losses matchesPlayed")
      .sort({ score: -1 });

    res.json(players);

  } catch (error) {

    res.status(500).json({
      message: error.message
    });
  }
};
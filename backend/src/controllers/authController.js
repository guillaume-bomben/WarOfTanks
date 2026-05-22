import bcrypt from "bcrypt";
import Player from "../models/Player.js";
import generateToken from "../utils/generateToken.js";

export const registerPlayer = async (req, res) => {
  try {
    const { username, email, password } = req.body;

    const playerExists = await Player.findOne({ email });

    if (playerExists) {
      return res.status(400).json({
        message: "Player already exists"
      });
    }

    const salt = await bcrypt.genSalt(10);

    const passwordHash = await bcrypt.hash(password, salt);

    const player = await Player.create({
      username,
      email,
      passwordHash
    });

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

export const loginPlayer = async (req, res) => {
  try {
    const { email, password } = req.body;

    const player = await Player.findOne({ email });

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
export const login = async (req, res) => {
  try {
    const { email, password } = req.body;

    const player = await Player.findOne({ email });

    if (!player) {
      return res.status(401).json({
        message: "Invalid credentials"
      });
    }

    const isMatch = await bcrypt.compare(password, player.password);

    if (!isMatch) {
      return res.status(401).json({
        message: "Invalid credentials"
      });
    }

    res.json({
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
export const getMe = async (req, res) => {
    res.json(req.player);
};
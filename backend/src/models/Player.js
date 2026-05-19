import mongoose from "mongoose";

const playerSchema = new mongoose.Schema(
  {
    username: {
      type: String,
      required: true,
      unique: true,
      trim: true
    },

    email: {
      type: String,
      required: true,
      unique: true,
      trim: true
    },

    passwordHash: {
      type: String,
      required: true
    },

    wins: {
      type: Number,
      default: 0
    },

    losses: {
      type: Number,
      default: 0
    },

    matchesPlayed: {
      type: Number,
      default: 0
    },

    score: {
      type: Number,
      default: 1000
    }
  },
  {
    timestamps: true
  }
);

const Player = mongoose.model("Player", playerSchema);

export default Player;
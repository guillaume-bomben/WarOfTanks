import mongoose from "mongoose";

const aiSchema = new mongoose.Schema(
  {
    name: {
      type: String,
      required: true,
      unique: true,
      trim: true
    },

    behaviorType: {
      type: String,
      required: true
    },

    difficulty: {
      type: String,
      enum: ["Easy", "Medium", "Hard"],
      default: "Medium"
    },

    wins: {
      type: Number,
      default: 0
    },

    losses: {
      type: Number,
      default: 0
    },

    winRate: {
      type: Number,
      default: 0
    }
  },
  {
    timestamps: true
  }
);

const AI = mongoose.model("AI", aiSchema);

export default AI;
import mongoose from "mongoose";

const matchSchema = new mongoose.Schema(
{
    playerA: {
        type: mongoose.Schema.Types.ObjectId,
        ref: "Player",
        required: true
    },

    playerB: {
        type: mongoose.Schema.Types.ObjectId,
        ref: "Player",
        required: true
    },

    winner: {
        type: mongoose.Schema.Types.ObjectId,
        ref: "Player"
    },

    scoreTeamA: {
        type: Number,
        default: 0
    },

    scoreTeamB: {
        type: Number,
        default: 0
    },

    durationSeconds: {
        type: Number,
        default: 0
    },

    playedAt: {
        type: Date,
        default: Date.now
    }
},
{
    timestamps: true
}
);

const Match = mongoose.model("Match", matchSchema);

export default Match;
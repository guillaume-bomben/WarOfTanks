import Match from "../models/Match.js";
import Player from "../models/Player.js";

export const createMatch = async (req, res) => {
    try {

        const {
            playerA,
            playerB,
            winner,
            scoreTeamA,
            scoreTeamB,
            durationSeconds
        } = req.body;

        const match = await Match.create({
            playerA,
            playerB,
            winner,
            scoreTeamA,
            scoreTeamB,
            durationSeconds
        });

        // Mise à jour stats joueurs

        if (winner) {

            await Player.findByIdAndUpdate(winner, {
                $inc: {
                    wins: 1,
                    matchesPlayed: 1,
                    score: 25
                }
            });

            const loser =
                winner === playerA ? playerB : playerA;

            await Player.findByIdAndUpdate(loser, {
                $inc: {
                    losses: 1,
                    matchesPlayed: 1,
                    score: -15
                }
            });
        }

        res.status(201).json(match);

    } catch (error) {

        res.status(500).json({
            message: error.message
        });
    }
};

export const getMatches = async (req, res) => {

    try {

        const matches = await Match.find()
            .populate("playerA", "username")
            .populate("playerB", "username")
            .populate("winner", "username");

        res.json(matches);

    } catch (error) {

        res.status(500).json({
            message: error.message
        });
    }
};
export const getMatchHistory = async (req, res) => {
  try {

    const { playerId } = req.params;

    const matches = await Match.find({
      $or: [
        { playerA: playerId },
        { playerB: playerId }
      ]
    })
      .populate("playerA", "username")
      .populate("playerB", "username")
      .populate("winner", "username")
      .sort({ playedAt: -1 });

    res.json(matches);

  } catch (error) {

    res.status(500).json({
      message: error.message
    });
  }
};
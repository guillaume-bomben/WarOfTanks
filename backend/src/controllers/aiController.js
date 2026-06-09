import AI from "../models/AI.js";

// Stocker les données d'une IA
export const createAI = async (req, res) => {
    try {

        const {
            name,
            behaviorType,
            difficulty,
            wins,
            losses,
            winRate
        } = req.body;

        const aiExists = await AI.findOne({ name });

        if (aiExists) {
            return res.status(400).json({
                message: "AI already exists"
            });
        }

        const ai = await AI.create({
            name,
            behaviorType,
            difficulty,
            wins,
            losses,
            winRate
        });

        res.status(201).json(ai);

    } catch (error) {

        res.status(500).json({
            message: error.message
        });
    }
};

// Récupérer les données de toutes les IA
export const getAIs = async (req, res) => {

    try {

        const ais = await AI.find().sort({ winRate: -1 });

        res.json(ais);

    } catch (error) {

        res.status(500).json({
            message: error.message
        });
    }
};

// Récupérer les données d'une IA par son id
export const getAIById = async (req, res) => {

    try {

        const ai = await AI.findById(req.params.id);

        if (!ai) {
            return res.status(404).json({
                message: "AI not found"
            });
        }

        res.json(ai);

    } catch (error) {

        res.status(500).json({
            message: error.message
        });
    }
};
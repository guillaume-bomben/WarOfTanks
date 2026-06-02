import jwt from "jsonwebtoken";
import Player from "../models/Player.js";

const protect = async (req, res, next) => {
  let token;

  if (
    req.headers.authorization &&
    req.headers.authorization.startsWith("Bearer")
  ) {
    try {

      token = req.headers.authorization.split(" ")[1];

      const decoded = jwt.verify(
        token,
        process.env.JWT_SECRET
      );

      req.player = await Player.findById(decoded.id)
        .select("-passwordHash");

      next();

    } catch (error) {
      res.status(401).json({
        message: "Not authorized"
      });
    }

  } else {
    res.status(401).json({
      message: "No token"
    });
  }
};

export default protect;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WarOfTanks.Nav
{
    public class GridDirection
    {
        public readonly Vector2Int vector;

        private GridDirection(int x, int y)
        {
            vector = new Vector2Int(x, y);
        }

        public static implicit operator Vector2Int(GridDirection direction)
        {
            return direction.vector;
        }

        public static GridDirection GetDirectionFromV2I(Vector2Int vector)
        {
            return CardinalAndIntercardinalDirections.DefaultIfEmpty(None).FirstOrDefault(direction => direction == vector);
        }

        public static readonly GridDirection None = new GridDirection(0, 0);
        public static readonly GridDirection North = new GridDirection(0, 1);
        public static readonly GridDirection South = new GridDirection(0, -1);
        public static readonly GridDirection East = new GridDirection(1, 0);
        public static readonly GridDirection West = new GridDirection(-1, 0);
        public static readonly GridDirection NorthEast = new GridDirection(1, 1);
        public static readonly GridDirection NorthWest = new GridDirection(-1, 1);
        public static readonly GridDirection SouthEast = new GridDirection(1, -1);
        public static readonly GridDirection SouthWest = new GridDirection(-1, -1);

        public class Directions : List<GridDirection> {}

        public static readonly Directions CardinalDirections = new Directions
        {
            North,
            East,
            South,
            West
        };

        public static readonly Directions CardinalAndIntercardinalDirections = new Directions
        {
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        };

        public static readonly Directions AllDirections = new Directions
        {
            None,
            North,
            NorthEast,
            East,
            SouthEast,
            South,
            SouthWest,
            West,
            NorthWest
        };

        public override string ToString()
        {
            if      (this == North)     return "North";
            else if (this == South)     return "South";
            else if (this == East)      return "East";
            else if (this == West)      return "West";
            else if (this == NorthEast) return "North-East";
            else if (this == NorthWest) return "North-West";
            else if (this == SouthEast) return "South-East";
            else if (this == SouthWest) return "South-West";
            else                        return "None";
        }



        // For serialization purposes
        public enum DirectionMode
        {
            CardinalDirections,
            CardinalAndIntercardinalDirections,
            AllDirections
        }
    }
}
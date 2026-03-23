using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WarOfTanks.MapGen
{
    [CreateAssetMenu(menuName = "War Of Tanks/Create new Custom Rule Tile")]
    public class TerrainRuleTile : RuleTile<RuleTile.TilingRule.Neighbor>
    {
        public bool checkSelf;
        public bool alwaysConnect;
        public TileBase[] tilesToConnect;
        public TerrainKind terrainKind;

        public enum TerrainKind
        {
            Water,
            Sand,
            Grass,
            Rock
        }


        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            public const int Any = 3;
            public const int Specified = 4;
            public const int Nothing = 5;
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            if (tile is TerrainRuleTile t)
            {
                switch (neighbor)
                {
                    case Neighbor.This:         return Check_This(tile);
                    case Neighbor.NotThis:      return Check_NotThis(tile);
                    case Neighbor.Any:          return Check_Any(tile);
                    case Neighbor.Specified:    return Check_Specified(tile);
                    case Neighbor.Nothing:      return Check_Nothing(tile);
                }
            }

            return base.RuleMatch(neighbor, tile);
        }
    
        bool Check_This(TileBase tile)
        {
            if (!alwaysConnect) return tile == this;
            else return tilesToConnect.Contains(tile) || tile == this;
        }
        
        bool Check_NotThis(TileBase tile) => tile != this;

        bool Check_Any(TileBase tile)
        {
            if (checkSelf) return tile != null;
            else return tile != null && tile != this;
        }

        bool Check_Specified(TileBase tile) => tilesToConnect.Contains(this);

        bool Check_Nothing(TileBase tile) => tile == null;
    } 
}
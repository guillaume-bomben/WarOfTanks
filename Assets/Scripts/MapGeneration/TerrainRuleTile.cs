using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WarOfTanks.MapGen
{
    [CreateAssetMenu(menuName = "War Of Tanks/Create new Custom Rule Tile")]
    public class TerrainRuleTile : RuleTile<RuleTile.TilingRule.Neighbor>
    {
        // public bool checkSelf;
        // public bool alwaysConnect;
        // public TileBase[] tilesToConnect;
        public TerrainKind terrainKind;

        public enum TerrainKind
        {
            Water,
            Sand,
            Grass,
            Rock,
            Swamp
        }


        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
        }

        public override bool RuleMatch(int neighbor, TileBase tile)
        {
            return base.RuleMatch(neighbor, tile);
        }
    } 
}
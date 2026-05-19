using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WarOfTanks.MapGen
{
    [CreateAssetMenu(menuName = "War Of Tanks/Rule Tiles/Create new Custom Rule Tile")]
    public class TerrainRuleTile : RuleTile<RuleTile.TilingRule.Neighbor>
    {
        public TerrainKind terrainKind;
        public TerrainDataModifier modifier;

        public bool isMountain;
        public bool isWalkable;
        public bool isHazard;

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
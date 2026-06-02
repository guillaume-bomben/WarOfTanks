using UnityEngine;
using UnityEngine.Tilemaps;

namespace WarOfTanks.MapGen
{
    [CreateAssetMenu(menuName = "War Of Tanks/Rule Tiles/Create new Rumble Rule Tile")]
    public class RumbleRuleTile : RuleTile<TerrainRuleTile.Neighbor>
    {
        public override bool RuleMatch(int neighbor, TileBase other)
        {
            TerrainRuleTile t = other as TerrainRuleTile;

            if (t == null)
                return false;

            if (neighbor == TilingRule.Neighbor.This)
                return t.isMountain;

            return base.RuleMatch(neighbor, other);
        }
    }
}

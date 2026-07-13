using UnityEngine;

namespace WarOfTanks.MapGen
{
    public class MapZones
    {
        public ZoneType[,] GenerateZones(int width, int height)
        {
            ZoneType[,] zones = new ZoneType[width, height];

            int zoneWidth = width / 3;
            int zoneHeight = height / 3;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int zx = x / zoneWidth;
                    int zy = y / zoneHeight;

                    zx = Mathf.Clamp(zx, 0, 2);
                    zy = Mathf.Clamp(zy, 0, 2);

                    /* MAPPING :
                     * 
                     * R : Red Spawn
                     * B : Blue Spawn
                     * @ : Center
                     * - : Neutral
                     * 
                     *      - - R
                     *      - @ -
                     *      B - -
                     * 
                     */

                    if (zx == 2 && zy == 2)
                        zones[x, y] = ZoneType.RedSpawn;
                    else if (zx == 0 && zy == 0)
                        zones[x, y] = ZoneType.BlueSpawn;
                    else if (zx == 1 && zy == 1)
                        zones[x, y] = ZoneType.Center;
                    else
                        zones[x, y] = ZoneType.Neutral;
                }
            }

            return zones;
        }
    }

    public enum ZoneType
    {
        Neutral,
        RedSpawn,
        BlueSpawn,
        Center
    }
}
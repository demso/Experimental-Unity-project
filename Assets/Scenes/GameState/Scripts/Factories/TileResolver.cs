using System.Collections.Generic;
using SuperTiled2Unity;

namespace Scenes.GameState.Scripts.Tilemap_Scripts
{
    public static class TileResolver
    {
        private static Dictionary<string, SuperTile> sTiles;
        private static Dictionary<int, SuperTile> nTiles;

        public static void setTiles(Dictionary<int, SuperTile> ts)
        {
            sTiles = new Dictionary<string, SuperTile>();
            nTiles = new Dictionary<int, SuperTile>(ts);
            foreach (var tile in ts.Values)
            {
                var name = tile.m_CustomProperties.Find(c => c.m_Name.Equals("name"));
                if (name != null) 
                    sTiles.Add(name.m_Value, tile);
            }
        }

        public static SuperTile GetTile(int id)
        {
            nTiles.TryGetValue(id, out var tile);
            return tile;
        }
        
        public static SuperTile GetTile(string id)
        {
            sTiles.TryGetValue(id, out var tile);
            return tile;
        }
    }
}
using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;

namespace DefaultNamespace
{
    public class TileResolver
    {
        private Dictionary<string, SuperTile> sTiles;
        private Dictionary<int, SuperTile> nTiles;

        public void setTiles(Dictionary<int, SuperTile> ts)
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

        public SuperTile getTile(int id)
        {
            nTiles.TryGetValue(id, out var tile);
            return tile;
        }
        
        public SuperTile getTile(string id)
        {
            sTiles.TryGetValue(id, out var tile);
            return tile;
        }
    }
}
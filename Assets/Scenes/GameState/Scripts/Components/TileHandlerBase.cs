using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scenes.GameState.Scripts.Tilemap_Scripts
{
    public class TileHandlerBase : MonoBehaviour
    {
        private Tilemap _tilemap;
        public Tilemap Tilemap
        {
            get
            {
                return _tilemap;
            }
        }

        private SuperTile _tile;

        public SuperTile Tile
        {
            get
            {
                return _tile;
            }

            set
            {
                _tile = value;
                _tilemap.SetTile(new Vector3Int(x, y), value);
            }
        }

        public int x, y;

        public void Init(int x1, int y1)
        {
            x = x1;
            y = y1;
            _tilemap = GetComponentInParent<Tilemap>();
            _tile = _tilemap.GetTile<SuperTile>(new Vector3Int(x, y));
        }

        public string GetName()
        {
            return _tile.m_CustomProperties.Find(property => property.m_Name.Equals("name")).m_Value;
        }

        public bool IsFlippedHorizontally() {
            Matrix4x4 mat = _tilemap.GetTransformMatrix(new Vector3Int(x, y));
            return mat.m00 == -1 || IsFlippedDiagonally() && mat.m01 == 1;
        }

        public bool IsFlippedVertically()
        {
            Matrix4x4 mat = _tilemap.GetTransformMatrix(new Vector3Int(x, y));
            return mat.m11 == -1 || IsFlippedDiagonally() && mat.m10 == 1;
        }
        
        public bool IsFlippedDiagonally() {
            Matrix4x4 mat = _tilemap.GetTransformMatrix(new Vector3Int(x, y));
            return mat.m01 == -1 && mat.m10 == 1 || mat.m10 == -1 && mat.m01 == 1 || mat.m01 == 1 && mat.m10 == 1;
        }

        public Rigidbody2D GetBody()
        {
            return GetComponent<Rigidbody2D>();
        }
    }
}
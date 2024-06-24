using DefaultNamespace.Tilemap_Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DefaultNamespace
{
    public class BodyFactory
    {
        public enum Direction {
            North,
            South,
            West,
            East
        }
        public Rigidbody2D GetTileBody(string bodyType, Direction direction, float x, float y)
        {
            Rigidbody2D body = null;
            switch (bodyType)
            {
                case "FULL_BODY":
                    GameObject go = new GameObject();
                    
                    body = go.AddComponent<Rigidbody2D>();
                    body.bodyType = RigidbodyType2D.Static;
                    BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                    collider.size = new Vector2(1,1);
                    Vector2 colliderSize = collider.size;
                    go.transform.position = new Vector3(x + colliderSize.x / 2f, y - colliderSize.y / 2f);
                    go.name = bodyType;
                    break;
                case "WINDOW":
                    body = window(bodyType, direction, x, y);
                    break;
            }
            
            return body;
        }

        public Rigidbody2D window(string bodyType, Direction direction, float x, float y) {
            GameObject go = new GameObject();
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Static;
            BoxCollider2D collider = go.AddComponent<BoxCollider2D>();

            switch (direction) {
                case Direction.North:
                    collider.size = new Vector2(1f, 0.1f);
                    Vector2 colliderSize = collider.size;
                    go.transform.position = new Vector3(x + colliderSize.x / 2f, y - colliderSize.y / 2f);
                    break;
                case Direction.South:
                    collider.size = new Vector2(1f, 0.1f);
                    colliderSize = collider.size;
                    go.transform.position = new Vector3(x + colliderSize.x / 2f, y - (1 - colliderSize.y / 2f));
                    break;
                case Direction.West:
                    collider.size = new Vector2(0.1f, 1f);
                    colliderSize = collider.size;
                    go.transform.position = new Vector3(x, y - colliderSize.y / 2f);
                    break;
                case Direction.East:
                    collider.size = new Vector2(0.1f, 1f);
                    colliderSize = collider.size;
                    go.transform.position = new Vector3(x +  (1 - colliderSize.x / 2f), y - colliderSize.y / 2f);
                    break;
            }
            
            
            go.name = bodyType;
            return body;
        }

        public static Direction GetDirection(Tilemap tilemap, int x, int y) {
            Vector3Int pos = new Vector3Int(x, y);
            Matrix4x4 transform = tilemap.GetTransformMatrix(pos);
            Direction direction = Direction.North;
            if (IsFlippedVertically(transform) && IsFlippedHorizontally(transform)) {
                direction = Direction.South;
            } else if (IsFlippedVertically(transform) && IsFlippedDiagonally(transform)) {
                direction = Direction.West;
            } else if (IsFlippedHorizontally(transform) && IsFlippedDiagonally(transform)) {
                direction = Direction.East;
            }

            return direction;
        }
        
        private static bool IsFlippedHorizontally(Matrix4x4 mat)
        {
            return mat.m00 == -1 || IsFlippedDiagonally(mat) && mat.m01 == 1;
        }

        private static bool IsFlippedVertically(Matrix4x4 mat)
        {
            return mat.m11 == -1 || IsFlippedDiagonally(mat) && mat.m10 == 1;
        }

        private static bool IsFlippedDiagonally(Matrix4x4 mat) {
            return mat.m01 == -1 && mat.m10 == 1 || mat.m10 == -1 && mat.m01 == 1 || mat.m01 == 1 && mat.m10 == 1;
        }
    }
}
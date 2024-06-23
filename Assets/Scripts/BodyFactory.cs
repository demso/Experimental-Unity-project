using UnityEngine;

namespace DefaultNamespace
{
    public class BodyFactory
    {
        public Rigidbody2D getBody(string bodyType, float x, float y)
        {
            GameObject go = null;
            Rigidbody2D body = null;
            switch (bodyType)
            {
                case "FULL_BODY":
                    go = new GameObject();
                    
                    body = go.AddComponent<Rigidbody2D>();
                    body.bodyType = RigidbodyType2D.Static;
                    BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                    collider.size = new Vector2(1,1);
                    Vector2 colliderSize = collider.size;
                    go.transform.position = new Vector3(x + colliderSize.x / 2f, y - colliderSize.y / 2f);
                    //body.lay
                    break;
            }

            if (go != null)
            {
                go.name = bodyType;
            }
            
            return body;
        }
    }
}
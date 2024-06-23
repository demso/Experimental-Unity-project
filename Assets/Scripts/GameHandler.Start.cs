using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.Tilemaps;

public partial class GameHandler : MonoBehaviour
{
    [HideInInspector] public GameObject map;
    [HideInInspector] public GameObject obstaclesGO;
    [HideInInspector] public Tilemap tilemap;
    [HideInInspector] public TileResolver TileResolver;
    public Dictionary<int, SuperTile> tls;
    void Start()
    {
        Globals.camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        Globals.player = GameObject.FindWithTag("Player");
        bodyFactory = new BodyFactory();
        TileResolver = new TileResolver();
        
        string filePath = "Tilemap/firstmap/worldmap";
        map = GameObject.FindWithTag("Level map");
        // if (map == null)
        // {
        
            map = Instantiate(Resources.Load<GameObject>(filePath), new Vector3(0, 0), Quaternion.identity);
            map.tag = "Level map";
        //}
        
        TileResolver.setTiles(map.GetComponent<SuperMap>().m_Tiles);
        
        obstaclesGO = map.transform.GetChild(0).Find("obstacles").gameObject;
        tilemap = obstaclesGO.GetComponent<Tilemap>();
        
        for (int y = 0; y < tilemap.size.y; y++)
        {
            for (int x = 0; x < tilemap.size.x; x++)
            {
                SuperTile tile = tilemap.GetTile<SuperTile>(new Vector3Int(x, -y));
                if (tile == null)
                    continue;
                var properties = tile.m_CustomProperties;
                var bodytype = properties.Find(property => property.m_Name.Equals("body type"));
                Rigidbody2D body = null;
                if (bodytype != null)
                {
                    body = bodyFactory.getBody(bodytype.m_Value, x, -y);
                    if (body != null)
                    {
                        body.gameObject.transform.parent = tilemap.gameObject.transform;
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Setup : MonoBehaviour
{
    public GameObject map;
    public GameObject obstaclesGO;
    public Tilemap tilemap;
    void Start()
    {
        Globals.camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        Globals.player = GameObject.FindWithTag("Player");
        string filePath = "Tilemap/firstmap/worldmap";
        map = GameObject.FindWithTag("Level map");
        if (map == null)
        {
            map = Instantiate(Resources.Load<GameObject>(filePath), new Vector3(0, 0), Quaternion.identity);
            map.tag = "Level map";
        }
        
        obstaclesGO = map.transform.GetChild(0).Find("obstacles").gameObject;
        tilemap = obstaclesGO.GetComponent<Tilemap>();
        
        for (int i = 0; i < tilemap.size.y; i++)
        {
            for (int j = 0; j < tilemap.size.x; j++)
            {
                SuperTile tile = tilemap.GetTile<SuperTile>(new Vector3Int(j, i));
                var properties = tile.m_CustomProperties;
                var bodytype = properties.Find(property => property.m_Name.Equals("body type"));
                if (bodytype != null)
                {
                   // Rigidbody2D body
                }
            }
        }
    }
}

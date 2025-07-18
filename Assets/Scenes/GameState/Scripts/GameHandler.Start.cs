using System;
using System.Collections.Generic;
using Scenes.GameState.Scripts.Entities;
using Scenes.GameState.Scripts.Factories;
using Scenes.GameState.Scripts.Items;
using Scenes.GameState.Scripts.Tilemap_Scripts;
using SuperTiled2Unity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Scenes.GameState.Scripts {
    public partial class GameHandler : MonoBehaviour {
        private void Start() {
            Instance = this;
            Globals.camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            Globals.player = GameObject.FindWithTag("Player");
            Items = new Dictionary<int, Item>();
            BodyFactory = new BodyFactory();
            ItemsFactory = new ItemsFactory(Items, BodyFactory);
            MobsFactory = new MobsFactory();

            InitTilemap();

            Item item = ItemsFactory.GetItem("deagle_44");
            item.Allocate(Globals.player.transform.position);
            Zombie zombie = MobsFactory.SpawnZombie(Globals.player.transform.position + new Vector3(1, 1, 0 ));
        }

        private void InitTilemap() {
            string filePath = "Tilemap/firstmap/worldmap";
            
            GameObject map;
            GameObject obstaclesGO;
            Tilemap tilemap;
            
            map = GameObject.FindWithTag("Level map");
            if (map == null) {
                map = Instantiate(Resources.Load<GameObject>(filePath), new Vector3(0, 0), Quaternion.identity);
                map.tag = "Level map";
            }

            TileResolver.setTiles(map.GetComponent<SuperMap>().m_Tiles);

            obstaclesGO = map.transform.GetChild(0).Find("obstacles").gameObject;
            tilemap = obstaclesGO.GetComponent<Tilemap>();

            for (int y = 0; y > -tilemap.size.y; y--) {
                for (int x = 0; x < tilemap.size.x; x++) {
                    SuperTile tile = tilemap.GetTile<SuperTile>(new Vector3Int(x, y));
                    if (tile == null)
                        continue;
                    var properties = tile.m_CustomProperties;
                    string bodytype = tile.GetPropertyOrDefault("body type", (string)null);
                    string tileName = tile.GetPropertyOrDefault("name", (string)null);
                    Rigidbody2D body = null;
                    if (bodytype != null) {
                        body = BodyFactory.GetTileBody(bodytype, BodyFactory.GetDirection(tilemap, x, y), x, y);
                    }

                    if (body == null) {
                        //Debug.Log("Tile without body on obstacles layer: " + " x: " + x + " y: " + y);
                        continue;
                    }

                    GameObject tileGO = body.gameObject;

                    tileGO.transform.parent = tilemap.gameObject.transform;

                    List<String> nameEntries = new List<String>(tileName.Split("_"));

                    if (nameEntries[0].Equals("t"))
                        switch (nameEntries[1]) {
                            case "door":
                                Door door = tileGO.AddComponent<Door>();
                                door.Init(x, y);
                                tileGO.name = "DOOR";
                                if (nameEntries.Contains("o")) {
                                    door.openTile = door.Tile;
                                    int index = nameEntries.IndexOf("o");
                                    nameEntries[index] = "c";
                                    door.closedTile = TileResolver.GetTile(string.Join("_", nameEntries));
                                    door.Open();
                                } else if (nameEntries.Contains("c")) {
                                    door.closedTile = door.Tile;
                                    int index = nameEntries.IndexOf("c");
                                    nameEntries[index] = "o";
                                    door.openTile = TileResolver.GetTile(string.Join("_", nameEntries));
                                    door.Close();
                                }
                                break;
                            
                            case "window":
                                var window = tileGO.AddComponent<Window>();
                                window.Init(x, y);
                                if (nameEntries.Contains("o")) {
                                    window.openTile = window.Tile;
                                    var index = nameEntries.IndexOf("o");
                                    nameEntries[index] = "c";
                                    window.closedTile = TileResolver.GetTile(string.Join("_", nameEntries));
                                    window.Open();
                                } else if (nameEntries.Contains("c")) {
                                    window.closedTile = window.Tile;
                                    var index = nameEntries.IndexOf("c");
                                    nameEntries[index] = "o";
                                    window.openTile = TileResolver.GetTile(string.Join("_", nameEntries));
                                    window.Close();
                                }
                                break;
                            
                            default:
                                TileHandlerBase tileHandler = tileGO.AddComponent<TileHandlerBase>();
                                tileHandler.Init(x, y);
                                break;
                        }
                }
            }
        }
    }
}
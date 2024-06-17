using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public static GameObject player;
    public static Camera camera;
    
    public const float
        DEFAULT_RENDER_ORDER =              0,
        ZOMBIE_RENDER_ORDER =               0.5f,
        PLAYER_RENDER_ORDER =               2,
        ITEMS_RENDER_ORDER =                0.4f;

    public const float
        PLAYER_HEALTH =                     20f,
        ZOMBIE_HEALTH =                     8f,
        ZOMBIE_DAMAGE =                     1f;

    public const int
        DEFAULT_CONTACT_FILTER =            0x0001,                 //00000000 00000001
        PLAYER_CONTACT_FILTER =             0x0008,                 //00000000 00001000
        PLAYER_INTERACT_CONTACT_FILTER =    0x0002,                 //00000000 00000010
        IGNORE_LIGHTS_LAYER =        0b_1000_0000_0000_0000,        //10000000 00000000 Ignore Lights layer
        BULLET_CONTACT_FILTER =             0x0004,                 //00000000 00000100
        ZOMBIE_CONTACT_FILTER =             0x0010,                 //00000000 00010000
        ALL_CONTACT_FILTER =                -1,                     //11111111 11111111
        NONE_CONTACT_FILTER =               0X0000,
        GRENADE_CONTACT_FILTER =            0X0020;              //00000000 0010000

        //PLAYER_CONTACT_GROUP =              -42,
        //LIGHT_CONTACT_GROUP =               -10;
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ContactPoint2D = UnityEngine.ContactPoint2D;

public class ObstaclesScript : MonoBehaviour
{
    private Tilemap tilemap;
    void Start()
    {
        tilemap = gameObject.GetComponent<Tilemap>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        print("Colided");
        Vector3 hitPosition = Vector3.zero; 
        ContactPoint2D[] contacts = new ContactPoint2D[other.contactCount];
        other.GetContacts(contacts);
        foreach (ContactPoint2D hit in contacts)
        {
            hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
            hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
            tilemap.SetTile(tilemap.WorldToCell(hitPosition), null);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        throw new NotImplementedException();
    }
}

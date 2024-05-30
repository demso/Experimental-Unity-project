using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    Vector2 moveImpulse = Vector2.zero;
    private Rigidbody2D body;
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        moveImpulse.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        moveImpulse = ((moveImpulse * moveSpeed * body.mass) / (1 - Time.fixedDeltaTime * body.drag)) * Time.fixedDeltaTime * 10f;
        body.AddForce(moveImpulse, ForceMode2D.Impulse);
    }
}

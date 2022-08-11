using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushMovementController : MonoBehaviour
{
    public float speed = 1.0f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private Animator animator;

    private string lastState = "MushIdleRight";

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.CrossFade("MushIdleRight", 0, 0);
    }

    public void MovementControl(MushController mushController)
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        var state = "MushIdleRight";

        if (lastState != state)
            state = lastState;

        if (movement.x > 0)
        {
            state = "MushRunRight";
            lastState = "MushIdleRight";
        }
        else if (movement.x < 0)
        {
            state = "MushRunLeft";
            lastState = "MushIdleLeft";
        }

        if (movement.y != 0)
        {
            if (lastState == "MushIdleRight")
            {
                state = "MushRunRight";
            }
            else if (lastState == "MushIdleLeft")
            {
                state = "MushRunLeft";
            }
        }

        animator.CrossFade(state, 0, 0);

        rb.MovePosition(rb.position + movement * mushController.GetStatValueByName("Speed") * speed * Time.deltaTime);
    }

}

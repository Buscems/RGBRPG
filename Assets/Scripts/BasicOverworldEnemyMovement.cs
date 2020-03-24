using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicOverworldEnemyMovement : MonoBehaviour
{

    Rigidbody2D rb;

    [Header("Movement")]
    public float speed;

    public enum Direction { Up, Down, Left, Right}
    public Direction thisDirection;

    [HideInInspector]
    public Vector2 direction;

    public enum MovementStyle { MoveUntilWall, RandomlyChangeDirection, MoveInOneDirectionUntilTime }
    public MovementStyle thisMovement;

    // Start is called before the first frame update
    void Start()
    {

        if(thisDirection == Direction.Up)
        {
            direction = new Vector2(0, 1);
        }
        if (thisDirection == Direction.Down)
        {
            direction = new Vector2(0, -1);
        }
        if (thisDirection == Direction.Left)
        {
            direction = new Vector2(-1, 0);
        }
        if (thisDirection == Direction.Right)
        {
            direction = new Vector2(1, 0);
        }

        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.currentState == GameControl.GameState.Overworld)
        {

            switch (thisMovement)
            {
                case MovementStyle.MoveUntilWall:

                    MovementUntilWall();

                    break;
            }
        }
    }

    void MovementUntilWall()
    {
        rb.MovePosition(rb.position + direction * Time.deltaTime * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (thisMovement)
        {
            case MovementStyle.MoveUntilWall:
                if (collision.gameObject.layer == 8)
                {
                    direction = Vector2.Reflect(direction, collision.contacts[0].normal);
                }
                break;
        }
    }

}

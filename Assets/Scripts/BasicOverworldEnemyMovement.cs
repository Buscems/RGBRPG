using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicOverworldEnemyMovement : MonoBehaviour
{

    Rigidbody2D rb;

    [Header("Movement")]
    public float speed;

    bool isMoving;
    Vector3 startPos, endPos;
    public GameObject endPosCollider;
    float timeToMove;
    public float walkSpeed;

    public enum Direction { Up, Down, Left, Right}
    public Direction thisDirection;

    //[HideInInspector]
    public Vector2 direction;

    public enum MovementStyle { MoveUntilWall, RandomlyChangeDirection, MoveInOneDirectionUntilTime }
    public MovementStyle thisMovement;

    // Start is called before the first frame update
    void Start()
    {
        if (this.transform.GetChild(0))
        {
            this.transform.GetChild(0).transform.SetParent(null);
        }
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

                    if (!isMoving)
                    {
                        StartCoroutine(Movement(transform));
                    }

                    break;
            }
        }
    }

    IEnumerator Movement(Transform entity)
    {
        isMoving = true;

        startPos = entity.position;
        timeToMove = 0;

        endPos = new Vector3(startPos.x + System.Math.Sign(direction.x), startPos.y + System.Math.Sign(direction.y), startPos.z);
        endPosCollider.transform.position = endPos;

        while (timeToMove < 1f)
        {
            timeToMove += Time.deltaTime * walkSpeed;
            entity.GetComponent<Rigidbody2D>().MovePosition(Vector3.Lerp(startPos, endPos, timeToMove));
            yield return null;
        }

        isMoving = false;
        yield return 0;

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
                    direction *= -1;
                }
                break;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicOverworldEnemyMovement : MonoBehaviour
{

    Rigidbody2D rb;

    [Header("Movement")]
    public float speed;
    public Vector2 direction;

    public enum MovementStyle { MoveUntilWall, RandomlyChangeDirection, MoveInOneDirectionUntilTime }
    public MovementStyle thisMovement;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {

        switch (thisMovement)
        {
            case MovementStyle.MoveUntilWall:

                break;
        }

    }
}

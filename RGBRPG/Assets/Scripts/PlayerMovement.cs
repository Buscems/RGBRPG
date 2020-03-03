﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

public class PlayerMovement : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player myPlayer;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public enum Direction { North, South, East, West}
    public Direction currentDirection;
    public Vector2 direction;
    bool isMoving;
    Vector3 startPos, endPos;
    float timeToMove;

    Animator anim;

    PlayerAttacks pa;
    StateManager sm;

    public float walkSpeed;

    private void Awake()
    {
        //Rewired Code
        myPlayer = ReInput.players.GetPlayer(playerNum - 1);
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        CheckController(myPlayer);
    }

    // Start is called before the first frame update
    void Start()
    {

        pa = GetComponent<PlayerAttacks>();
        sm = GetComponent<StateManager>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.currentState == GameControl.GameState.Overworld)
        {
            sm.enabled = false;
            if (!isMoving)
            {
                direction = new Vector2(myPlayer.GetAxis("MoveHorizontal"), myPlayer.GetAxis("MoveVertical"));

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    direction.y = 0;
                }
                else
                {
                    direction.x = 0;
                }

                if (direction != Vector2.zero)
                {

                    if (direction.x < 0)
                    {
                        currentDirection = Direction.West;
                    }
                    if (direction.x > 0)
                    {
                        currentDirection = Direction.East;
                    }
                    if (direction.y < 0)
                    {
                        currentDirection = Direction.South;
                    }
                    if (direction.y > 0)
                    {
                        currentDirection = Direction.North;
                    }

                    switch (currentDirection)
                    {
                        case Direction.North:
                            anim.SetFloat("Blend", 1);
                            break;

                        case Direction.East:
                            anim.SetFloat("Blend", 3);
                            break;

                        case Direction.South:
                            anim.SetFloat("Blend", 0);
                            break;

                        case Direction.West:
                            anim.SetFloat("Blend", 2);
                            break;
                    }

                    StartCoroutine(Movement(transform));
                }
            }
        }

        //animation stuff
        anim.SetBool("isMoving", isMoving);

    }

    IEnumerator Movement(Transform entity)
    {
        isMoving = true;

        startPos = entity.position;
        timeToMove = 0;

        endPos = new Vector3(startPos.x + System.Math.Sign(direction.x), startPos.y + System.Math.Sign(direction.y), startPos.z);

        while(timeToMove < 1f)
        {
            timeToMove += Time.deltaTime * walkSpeed;
            entity.GetComponent<Rigidbody2D>().MovePosition(Vector3.Lerp(startPos, endPos, timeToMove));
            yield return null;
        }

        isMoving = false;
        yield return 0;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            GameControl.currentState = GameControl.GameState.Combat;
        }
    }

    //[REWIRED METHODS]
    //these two methods are for ReWired, if any of you guys have any questions about it I can answer them, but you don't need to worry about this for working on the game - Buscemi
    void OnControllerConnected(ControllerStatusChangedEventArgs arg)
    {
        CheckController(myPlayer);
    }

    void CheckController(Player player)
    {
        foreach (Joystick joyStick in player.controllers.Joysticks)
        {
            var ds4 = joyStick.GetExtension<DualShock4Extension>();
            if (ds4 == null) continue;//skip this if not DualShock4
            switch (playerNum)
            {
                case 4:
                    ds4.SetLightColor(Color.yellow);
                    break;
                case 3:
                    ds4.SetLightColor(Color.green);
                    break;
                case 2:
                    ds4.SetLightColor(Color.blue);
                    break;
                case 1:
                    ds4.SetLightColor(Color.red);
                    break;
                default:
                    ds4.SetLightColor(Color.white);
                    Debug.LogError("Player Num is 0, please change to a number > 0");
                    break;
            }
        }
    }

}

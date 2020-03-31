using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;
using UnityEngine.UI;

public class CombatPlayerMovement : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player myPlayer;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    [Header("MovementVariables")]

    Vector2 direction;

    Vector3 moveFromPos;

    public enum Direction { North, South, East, West }
    public Direction currentDirection;

    Vector2 lastDirection;
    Vector2 lastPressedDirection;

    public Color movementIndicatorColor;

    Stack<GameObject> movementIndicators = new Stack<GameObject>();
    public Stack<GameObject> currentMovementIndicator = new Stack<GameObject>();
    Stack<Vector2> lastPressedDirections = new Stack<Vector2>();
    public static int maxIndicators;

    public bool isFinished;

    bool movedDirection;

    bool canSelectPosition;

    public int moveSpeed;
    int moveCounter;

    bool hasAddedToQueue;

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
        lastPressedDirection = Vector2.right;
        lastPressedDirections.Push(Vector2.right);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAddedToQueue)
        {
            AddToQueue();
            hasAddedToQueue = true;
        }

        SelectMovementSpot();

        //resetMovement
        if (myPlayer.GetButtonDown("Back"))// && moveCounter >= 1)
        {
            foreach(GameObject g in currentMovementIndicator)
            {
                g.transform.position = new Vector3(100000, 0, 0);
            }
            currentMovementIndicator.Clear();
            movedDirection = false;
            moveCounter = 0;
        }

    }

    public void AddToQueue()
    {
        for (int i = 0; i < maxIndicators; i++)
        {
            movementIndicators.Push(GameObject.Find("MovementIndicator (" + i + ")"));
        }
    }

    void SelectMovementSpot()
    {
        direction = new Vector2(myPlayer.GetAxis("MoveHorizontal"), myPlayer.GetAxis("MoveVertical"));

        if (!movedDirection)
        {
            var thisIndicator = movementIndicators.Peek();
            thisIndicator.transform.position = this.transform.position;
            thisIndicator.GetComponent<SpriteRenderer>().color = movementIndicatorColor;
            currentMovementIndicator.Push(thisIndicator);
            movementIndicators.Pop();
            lastDirection = thisIndicator.transform.position;
            movedDirection = true;
        }

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            direction.y = 0;
            if (direction.x > 0)
            {
                direction.x = 1;
            }
            if (direction.x < 0)
            {
                direction.x = -1;
            }
        }
        else
        {
            direction.x = 0;
            if (direction.y > 0)
            {
                direction.y = 1;
            }
            if (direction.y < 0)
            {
                direction.y = -1;
            }
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
            var thisIndicator = currentMovementIndicator.Peek();
            thisIndicator.transform.position = lastDirection + direction;
            thisIndicator.GetComponent<SpriteRenderer>().color = movementIndicatorColor;
            thisIndicator.GetComponent<SpriteRenderer>().enabled = false;
            /*
            foreach (GameObject g in currentMovementIndicator)
            {
                if(g.name != thisIndicator.name && g.transform.position == thisIndicator.transform.position)
                {
                    //canSelectPosition = false;
                    thisIndicator.GetComponent<SpriteRenderer>().color = Color.red;
                }
                else
                {
                    //canSelectPosition = true;
                    thisIndicator.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
            */
        }

        if (myPlayer.GetButtonDown("Select"))
        {
            isFinished = true;
        }

        if (direction == lastPressedDirections.Peek() && moveCounter >= 1 && canSelectPosition)
        //if(myPlayer.GetButtonDown("BackOne") && moveCounter >= 1)
        {
            var thisIndicator = currentMovementIndicator.Peek();
            movementIndicators.Push(thisIndicator);
            currentMovementIndicator.Pop();
            thisIndicator.transform.position = new Vector3(100000, 0, 0);
            if (moveCounter >= 2)
            {
                var newerIndicator = currentMovementIndicator.Peek();
                newerIndicator.transform.position = new Vector3(100000, 0, 0);
                currentMovementIndicator.Pop();
                lastDirection = currentMovementIndicator.Peek().transform.position;
                currentMovementIndicator.Push(newerIndicator);
                moveCounter--;
                lastPressedDirections.Pop();
            }
            else if(moveCounter == 1)
            {/*
                foreach (GameObject g in currentMovementIndicator)
                {
                    g.transform.position = new Vector3(100000, 0, 0);
                }
                */
                currentMovementIndicator.Peek().transform.position = this.transform.position;
                movementIndicators.Push(currentMovementIndicator.Peek());
                currentMovementIndicator.Clear();
                moveCounter = 0;
                var firstIndicator = movementIndicators.Peek();
                firstIndicator.transform.position = this.transform.position;
                currentMovementIndicator.Push(firstIndicator);
                movementIndicators.Pop();
                lastDirection = firstIndicator.transform.position;
                movedDirection = true;
                currentMovementIndicator.Peek().GetComponent<SpriteRenderer>().enabled = true;
                currentMovementIndicator.Peek().transform.position = this.transform.position;
                /*lastDirection = this.transform.position;
                var currentIndicator = currentMovementIndicator.Peek();
                currentIndicator.transform.position = this.transform.position;
                currentMovementIndicator.Clear();
                movedDirection = false;
                moveCounter = 0;*/
                canSelectPosition = false;
                
            }
            //lastPressedDirections.Clear();
            //lastPressedDirections.Push(Vector2.right);
            canSelectPosition = false;
        }
        else if (canSelectPosition && direction != Vector2.zero && moveCounter < moveSpeed)
        {
            canSelectPosition = false;
            lastDirection = currentMovementIndicator.Peek().transform.position;
            currentMovementIndicator.Peek().GetComponent<SpriteRenderer>().enabled = true;
            var nextIndicator = movementIndicators.Peek();
            currentMovementIndicator.Push(nextIndicator);
            movementIndicators.Pop();
            moveCounter++;
            //lastPressedDirection = direction;
            lastPressedDirections.Push(direction * -1);
            canSelectPosition = false;
        }

        if (direction == Vector2.zero)
        {
            canSelectPosition = true;    
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

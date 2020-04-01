using System.Collections;
using System.Collections.Generic;
using Rewired;
using Rewired.ControllerExtensions;
using UnityEngine;

public class RealCombatPlayerMovement : MonoBehaviour
{//the following is in order to use rewired
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

    public Color movementIndicatorColor;

    Vector2 lastDirection;
    Stack<Vector2> lastPressedDirections = new Stack<Vector2>();

    public bool isFinished;

    bool movedDirection;

    bool canSelectPosition;

    public int moveSpeed;
    public int moveCounter;

    bool hasAddedToQueue;

    [SerializeField]
    GameObject[] movementIndicators;

    GameObject movementIndicatorHolder;

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
        lastPressedDirections.Push(Vector2.right);
        if (this.gameObject.name == "RedGoops")
        {
            movementIndicatorHolder = GameObject.Find("RedMovementIndicators");
        }
        if (this.gameObject.name == "GreenGoops")
        {
            movementIndicatorHolder = GameObject.Find("GreenMovementIndicators");
        }
        if (this.gameObject.name == "BlueGoops")
        {
            movementIndicatorHolder = GameObject.Find("BlueMovementIndicators");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAddedToQueue)
        {
            PopulateArray();
            hasAddedToQueue = true;
        }

        SelectMovementSpot();

        //resetMovement
        if (myPlayer.GetButtonDown("Back"))
        {
            for (int i = 0; i < moveSpeed; i++)
            {
                movementIndicators[i].transform.position = new Vector3(100000, 0, 0);
            }
            movedDirection = false;
        }
    }

    public void PopulateArray()
    {
        movementIndicators = new GameObject[moveSpeed];
        for (int i = 0; i < moveSpeed; i++)
        {
            movementIndicators[i] = movementIndicatorHolder.transform.GetChild(i).gameObject;
        }
    }
    
    void SelectMovementSpot()
    {
        direction = new Vector2(myPlayer.GetAxis("MoveHorizontal"), myPlayer.GetAxis("MoveVertical"));

        if (!movedDirection)
        {
            movementIndicators[0].transform.position = this.transform.position;
            movementIndicators[0].GetComponent<SpriteRenderer>().color = movementIndicatorColor;
            lastDirection = movementIndicators[0].transform.position;
            moveCounter = 0;
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

        if (direction != Vector2.zero && moveCounter < moveSpeed)
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
        }

        if (myPlayer.GetButtonDown("Select"))
        {
            isFinished = true;
        }

        if (direction == lastPressedDirections.Peek() && moveCounter >= 1 && canSelectPosition)
        {
            var thisIndicator = movementIndicators[moveCounter-1];
            thisIndicator.transform.position = new Vector3(100000, 0, 0);
            if (moveCounter >= 2)
            {
                var newerIndicator = movementIndicators[moveCounter-1];
                newerIndicator.transform.position = new Vector3(100000, 0, 0);
                lastDirection = movementIndicators[moveCounter-2].transform.position;
                moveCounter--;
                lastPressedDirections.Pop();
            }
            else if (moveCounter == 1)
            {
                movedDirection = false;
            }
            canSelectPosition = false;
        }
        else if (canSelectPosition && direction != Vector2.zero && moveCounter < moveSpeed)
        {
            canSelectPosition = false;

            var thisIndicator = movementIndicators[moveCounter];
            thisIndicator.transform.position = lastDirection + direction;
            thisIndicator.GetComponent<SpriteRenderer>().color = movementIndicatorColor;
            thisIndicator.GetComponent<SpriteRenderer>().enabled = false;

            lastDirection = movementIndicators[moveCounter].transform.position;
            movementIndicators[moveCounter].GetComponent<SpriteRenderer>().enabled = true;
            moveCounter++;
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

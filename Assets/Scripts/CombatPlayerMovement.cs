using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

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
    public enum Direction { North, South, East, West }
    public Direction currentDirection;
    
    

    Queue<GameObject> movementIndicators = new Queue<GameObject>();
    public int maxIndicators;

    public bool isFinished;

    bool movedDirection;

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
        for(int i = 0; i < maxIndicators; i++)
        {
            movementIndicators.Enqueue(GameObject.Find("MovementIndicator (" + i + ")"));
        }
    }

    // Update is called once per frame
    void Update()
    {

        SelectMovementSpot();

    }

    void SelectMovementSpot()
    {
        direction = new Vector2(myPlayer.GetAxis("MoveHorizontal"), myPlayer.GetAxis("MoveVertical"));

        if (!movedDirection)
        {
            var thisIndicator = movementIndicators.Peek();
            thisIndicator.transform.position = this.transform.position;
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
            var thisIndicator = movementIndicators.Peek();
            thisIndicator.transform.position = this.transform.position + (Vector3)direction;
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

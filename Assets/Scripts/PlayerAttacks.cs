using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

public class PlayerAttacks : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player myPlayer;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public enum AttackType { None, ConeAttack }
    public AttackType currentAttack;

    public GameObject[] attackIndicator;

    PlayerMovement pm;

    Vector3 attackDirection;

    public int coneDistance;

    [HideInInspector]
    public bool hasChangedDirection;

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

        for(int i = 0; i < attackIndicator.Length; i++)
        {
            attackIndicator[i].SetActive(false);
        }

        pm = this.GetComponent<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {

        attackDirection = new Vector2(myPlayer.GetAxis("MoveHorizontal"), myPlayer.GetAxis("MoveVertical"));

        if (Mathf.Abs(attackDirection.x) > Mathf.Abs(attackDirection.y))
        {
            attackDirection.y = 0;
        }
        else
        {
            attackDirection.x = 0;
        }

        if (currentAttack == AttackType.ConeAttack)
        {
            attackIndicator[0].SetActive(true);
            if (!hasChangedDirection)
            {
                if (pm.currentDirection == PlayerMovement.Direction.East)
                {
                    attackIndicator[0].transform.position = new Vector3(transform.position.x + 1, transform.position.y, 0);
                }
                if (pm.currentDirection == PlayerMovement.Direction.West)
                {
                    attackIndicator[0].transform.position = new Vector3(transform.position.x - 1, transform.position.y, 0);
                }
                if (pm.currentDirection == PlayerMovement.Direction.North)
                {
                    attackIndicator[0].transform.position = new Vector3(transform.position.x, transform.position.y + 1, 0);
                }
                if (pm.currentDirection == PlayerMovement.Direction.South)
                {
                    attackIndicator[0].transform.position = new Vector3(transform.position.x, transform.position.y - 1, 0);
                }
            }

            if (attackDirection.x > 0)
            {
                if (!hasChangedDirection)
                {
                    hasChangedDirection = true;
                }
                attackIndicator[0].transform.position = new Vector3(transform.position.x + 1, transform.position.y, 0);
            }
            if (attackDirection.x < 0)
            {
                if (!hasChangedDirection)
                {
                    hasChangedDirection = true;
                }
                attackIndicator[0].transform.position = new Vector3(transform.position.x - 1, transform.position.y, 0);
            }
            if (attackDirection.y > 0)
            {
                if (!hasChangedDirection)
                {
                    hasChangedDirection = true;
                }
                attackIndicator[0].transform.position = new Vector3(this.transform.position.x, transform.position.y + 1, 0);
            }
            if (attackDirection.y < 0)
            {
                if (!hasChangedDirection)
                {
                    hasChangedDirection = true;
                }
                attackIndicator[0].transform.position = new Vector3(this.transform.position.x, transform.position.y - 1, 0);
            }
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

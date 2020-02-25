using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

public class StateManager : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player myPlayer;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public enum GameState { MovementSelection, SelectAttack, SelectAttackTarget }
    public GameState currentState;

    PlayerAttacks pa;
    PlayerMovement pm;

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
        pm = GetComponent<PlayerMovement>();

        pa.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(currentState == GameState.SelectAttack && myPlayer.GetButtonDown("Select"))
        {
            currentState = GameState.SelectAttackTarget;
            pm.enabled = false;
            pa.enabled = true;
        }

        else if(currentState == GameState.SelectAttackTarget & myPlayer.GetButtonDown("Select"))
        {
            currentState = GameState.SelectAttack;
            for (int i = 0; i < pa.attackIndicator.Length; i++)
            {
                pa.attackIndicator[i].SetActive(false);
            }
            pa.hasChangedDirection = false;
            pm.enabled = true;
            pa.enabled = false;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using Rewired.ControllerExtensions;

public class GameControl : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player myPlayer;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public GameObject playerCamera;

    public enum GameState { Overworld, Combat }
    public static GameState currentState;

    public enum CombatState { NotInCombat, PickMovement, PickAttack, TurnPlay}
    public static CombatState currentCombatState;

    bool enterCombatState;
    bool enterMovementState;
    bool endMovementState;

    bool enterAttackState;

    bool hasDoneCombatMovement;

    public GameObject transition;

    public int currentMaxMovementIndicators;

    Queue<GameObject> goopQueue = new Queue<GameObject>();
    GameObject[] goops;

    public int currentGoop;

    public Image finishedRed;
    public Image finishedGreen;
    public Image finishedBlue;

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
        currentState = GameState.Combat;
        transition.SetActive(false);

        currentCombatState = CombatState.NotInCombat;
        finishedRed.enabled = false;
        finishedGreen.enabled = false;
        finishedBlue.enabled = false;

        currentGoop = 0;

    }

    // Update is called once per frame
    void Update()
    {
        
        if(currentState == GameState.Combat)
        {

            if (!enterCombatState)
            {
                Time.timeScale = 0;
                transition.SetActive(true);
                enterCombatState = true;
            }

            #region SelectingMovement

            if (currentCombatState == CombatState.PickMovement)
            {
                if (!enterMovementState)
                {
                    if(GameObject.Find("RedGoops") != null)
                    {
                        goopQueue.Enqueue(GameObject.Find("RedGoops"));
                    }
                    if (GameObject.Find("GreenGoops") != null)
                    {
                        goopQueue.Enqueue(GameObject.Find("GreenGoops"));
                    }
                    if (GameObject.Find("BlueGoops") != null)
                    {
                        goopQueue.Enqueue(GameObject.Find("BlueGoops"));
                    }
                    goops = goopQueue.ToArray();
                    goopQueue.Clear();
                    for (int i = 0; i < goops.Length; i++)
                    {
                        goops[i].GetComponent<RealCombatPlayerMovement>().enabled = false;
                        goops[i].GetComponent<PlayerAttacks>().enabled = false;
                    }
                    enterMovementState = true;
                }

                if (goops.Length > 0)
                {

                    goops[currentGoop].GetComponent<RealCombatPlayerMovement>().enabled = true;

                    if (myPlayer.GetButtonDown("RB"))
                    {
                        if (currentGoop < goops.Length - 1)
                        {
                            currentGoop++;
                        }
                        else
                        {
                            currentGoop = 0;
                        }
                        for(int i = 0; i < goops.Length; i++)
                        {
                            goops[i].GetComponent<RealCombatPlayerMovement>().enabled = false;
                        }
                    }
                    if (myPlayer.GetButtonDown("LB"))
                    {
                        if (currentGoop > 0)
                        {
                            currentGoop--;
                        }
                        else
                        {
                            currentGoop = goops.Length - 1;
                        }
                        for (int i = 0; i < goops.Length; i++)
                        {
                            goops[i].GetComponent<RealCombatPlayerMovement>().enabled = false;
                        }
                    }
                    if (AllGoopsLockedIn())
                    {
                        Debug.Log("All goops are locked in");
                        for(int i = 0; i < goops.Length; i++)
                        {
                            currentCombatState = CombatState.PickAttack;
                            /*
                            var goopMovement = goops[i].GetComponent<RealCombatPlayerMovement>();
                            if (!goopMovement.hasStartedMoving)
                            {
                                goopMovement.movementCoroutines[0] = StartCoroutine(goopMovement.DoCombatMovement());
                            }
                            */
                        }
                    }

                    if (MovementHasFinished())
                    {
                        //currentCombatState = CombatState.PickAttack;
                    }

                }

            }
            #endregion

            if (currentCombatState == CombatState.PickAttack)
            {
                if (!enterAttackState)
                {
                    for (int i = 0; i < goops.Length; i++)
                    {
                        goops[i].GetComponent<RealCombatPlayerMovement>().enabled = false;
                        goops[i].GetComponent<PlayerAttacks>().enabled = false;
                    }
                    enterAttackState = true;
                }
                if (goops.Length > 0)
                {

                    goops[currentGoop].GetComponent<PlayerAttacks>().enabled = true;

                    if (myPlayer.GetButtonDown("RB"))
                    {
                        if (currentGoop < goops.Length - 1)
                        {
                            currentGoop++;
                        }
                        else
                        {
                            currentGoop = 0;
                        }
                        for (int i = 0; i < goops.Length; i++)
                        {
                            goops[i].GetComponent<RealCombatPlayerMovement>().enabled = false;
                        }
                    }
                    if (myPlayer.GetButtonDown("LB"))
                    {
                        if (currentGoop > 0)
                        {
                            currentGoop--;
                        }
                        else
                        {
                            currentGoop = goops.Length - 1;
                        }
                        for (int i = 0; i < goops.Length; i++)
                        {
                            goops[i].GetComponent<RealCombatPlayerMovement>().enabled = false;
                        }
                    }
                    if (AllGoopsLockedIn())
                    {
                        Debug.Log("All goops are locked in");
                        for (int i = 0; i < goops.Length; i++)
                        {
                            currentCombatState = CombatState.TurnPlay;
                        }
                    }
                }
            }
        }
    }

    private bool AllGoopsLockedIn()
    {
        if (currentCombatState == CombatState.PickMovement)
        {
            for (int i = 0; i < goops.Length; i++)
            {
                if (!goops[i].GetComponent<RealCombatPlayerMovement>().isFinished)
                {
                    return false;
                }
            }
        }
        if(currentCombatState == CombatState.PickAttack)
        {
            for (int i = 0; i < goops.Length; i++)
            {
                if (!goops[i].GetComponent<PlayerAttacks>().isFinished)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool MovementHasFinished()
    {
        for (int i = 0; i < goops.Length; i++)
        {
            if (!goops[i].GetComponent<RealCombatPlayerMovement>().movementHasFinished)
            {
                return false;
            }
        }

        return true;
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

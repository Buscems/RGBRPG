using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{

    public enum GameState { Overworld, Combat }
    public static GameState currentState;

    public enum CombatState { NotInCombat, PickMovement, PickAttack, PickAttackDirection, TurnPlay}
    public static CombatState currentCombatState;

    bool enterCombatState;
    bool enterMovementState;

    public GameObject transition;

    public int currentMaxMovementIndicators;

    Queue<GameObject> goopQueue = new Queue<GameObject>();
    //Queue<GameObject> 

    public Image finishedRed;
    public Image finishedGreen;
    public Image finishedBlue;

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.Combat;
        transition.SetActive(false);

        currentCombatState = CombatState.NotInCombat;
        finishedRed.enabled = false;
        finishedGreen.enabled = false;
        finishedBlue.enabled = false;
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

            if(currentCombatState == CombatState.PickMovement)
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
                }

                if (goopQueue.Count > 0)
                {
                    goopQueue.Peek().GetComponent<CombatPlayerMovement>().enabled = true;
                    CombatPlayerMovement.maxIndicators = currentMaxMovementIndicators;
                    //goopQueue.Peek().GetComponent<CombatPlayerMovement>().AddToQueue();
                    if (goopQueue.Peek().GetComponent<CombatPlayerMovement>().isFinished)
                    {
                        if(goopQueue.Peek().name == "RedGoops")
                        {
                            finishedRed.enabled = true;
                        }
                        if (goopQueue.Peek().name == "GreenGoops")
                        {
                            finishedGreen.enabled = true;
                        }
                        if (goopQueue.Peek().name == "BlueGoops")
                        {
                            finishedBlue.enabled = true;
                        }
                        currentMaxMovementIndicators -= goopQueue.Peek().GetComponent<CombatPlayerMovement>().currentMovementIndicator.Count;
                        goopQueue.Peek().GetComponent<CombatPlayerMovement>().enabled = false;
                        goopQueue.Dequeue();
                    }
                }

            }

        }
    }
}

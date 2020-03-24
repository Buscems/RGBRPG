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

    Queue<GameObject> goopQueue = new Queue<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.Overworld;
        transition.SetActive(false);

        currentCombatState = CombatState.NotInCombat;

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
                    if (goopQueue.Peek().GetComponent<CombatPlayerMovement>().isFinished)
                    {
                        goopQueue.Peek().GetComponent<CombatPlayerMovement>().enabled = false;
                        goopQueue.Dequeue();
                    }
                }

            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{

    public enum GameState { Overworld, Combat }
    public static GameState currentState;

    bool enterCombatState;

    public GameObject transition;

    // Start is called before the first frame update
    void Start()
    {
        currentState = GameState.Overworld;
        transition.SetActive(false);
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
        }
    }
}

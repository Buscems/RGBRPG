using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscellaneousAnimationFunctions : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TimeScaleOne()
    {
        Time.timeScale = 1;

        GameControl.currentCombatState = GameControl.CombatState.PickMovement;

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSpawner : MonoBehaviour
{

    public GameObject redGoop, greenGoop, blueGoop;
    private int decider;
    private float timer;
    public float timerMin, timerMax;
    public Vector3 spawnPos;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.Range(timerMin, timerMax);
        decider = Random.Range(0, 3);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.DeltaTime;

        if(timer <= 0){
            if(decider == 0){
                Instantiate(redGoop, spawnPos, Quaternion.identity);
            }
            if (decider == 1)
            {
                Instantiate(greenGoop, spawnPos, Quaternion.identity);
            }
            if (decider == 2)
            {
                Instantiate(blueGoop, spawnPos, Quaternion.identity);
            }
            timer = Random.Range(timerMin, timerMax);
            decider = Random.Range(0, 3);
        }
    }
}

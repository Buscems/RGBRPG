using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{

    [Header("Amount of Goops")]
    public int redGoopAmount;
    public int greenGoopAmount;
    public int blueGoopAmount;

    [Header("UI")]
    public TextMeshProUGUI redAmount;
    public TextMeshProUGUI greenAmount;
    public TextMeshProUGUI blueAmount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        redAmount.text = "x" + redGoopAmount;
        greenAmount.text = "x" + greenGoopAmount;
        blueAmount.text = "x" + blueGoopAmount;
    }
}

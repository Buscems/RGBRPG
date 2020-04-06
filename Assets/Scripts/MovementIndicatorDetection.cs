using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementIndicatorDetection : MonoBehaviour
{

    public bool activeIndicator;

    bool halfSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (halfSize)
        {
            transform.localScale = new Vector2(.5f, 1);
        }
        else
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "MovementIndicator" && activeIndicator)
        {
            halfSize = true;
            collision.GetComponent<MovementIndicatorDetection>().halfSize = true;
            this.transform.position -= new Vector3(.25f, 0, 0);
            collision.transform.position += new Vector3(.25f, 0, 0);
        }
    }



}

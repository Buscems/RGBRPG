using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Rewired.ControllerExtensions;

public class PlayerMovement : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player myPlayer;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    public GameObject playerCamera;

    public enum Direction { North, South, East, West}
    public Direction currentDirection;
    public Vector2 direction;
    bool isMoving;
    Vector3 startPos, endPos;
    float timeToMove;

    Animator anim;

    PlayerAttacks pa;
    PlayerInventory pi;

    [Header("Combat Setup")]
    public GameObject redGoop;
    public GameObject greenGoop;
    public GameObject blueGoop;

    Vector3[] spawnPos = new Vector3[3];

    public float walkSpeed;

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
        try
        {
            pa = GetComponent<PlayerAttacks>();
            pi = GetComponent<PlayerInventory>();
            anim = GetComponent<Animator>();
        }
        catch { }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.currentState == GameControl.GameState.Overworld)
        {

            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            if (!isMoving)
            {
                direction = new Vector2(myPlayer.GetAxis("MoveHorizontal"), myPlayer.GetAxis("MoveVertical"));

                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    direction.y = 0;
                }
                else
                {
                    direction.x = 0;
                }

                if (direction != Vector2.zero)
                {

                    if (direction.x < 0)
                    {
                        currentDirection = Direction.West;
                    }
                    if (direction.x > 0)
                    {
                        currentDirection = Direction.East;
                    }
                    if (direction.y < 0)
                    {
                        currentDirection = Direction.South;
                    }
                    if (direction.y > 0)
                    {
                        currentDirection = Direction.North;
                    }
                    if (anim != null)
                    {
                        switch (currentDirection)
                        {
                            case Direction.North:
                                anim.SetFloat("Blend", 1);
                                break;

                            case Direction.East:
                                anim.SetFloat("Blend", 3);
                                break;

                            case Direction.South:
                                anim.SetFloat("Blend", 0);
                                break;

                            case Direction.West:
                                anim.SetFloat("Blend", 2);
                                break;
                        }
                    }

                    StartCoroutine(Movement(transform));
                }
            }
        }

        if (anim != null)
        {
            //animation stuff
            anim.SetBool("isMoving", isMoving);
        }
    }

    IEnumerator Movement(Transform entity)
    {
        isMoving = true;

        startPos = entity.position;
        timeToMove = 0;

        endPos = new Vector3(startPos.x + System.Math.Sign(direction.x), startPos.y + System.Math.Sign(direction.y), startPos.z);

        while(timeToMove < 1f)
        {
            timeToMove += Time.deltaTime * walkSpeed;
            entity.GetComponent<Rigidbody2D>().MovePosition(Vector3.Lerp(startPos, endPos, timeToMove));
            yield return null;
        }

        isMoving = false;
        yield return 0;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {

            foreach (ContactPoint2D contact in collision.contacts)
            {
                //am I getting hit from the top or bottom?
                if (Mathf.Abs(contact.normal.y) > Mathf.Abs(contact.normal.x))
                {
                    if (contact.normal.y >= 0)
                    { //am I being hit from below?
                        for (int i = 0; i < spawnPos.Length; i++)
                        {
                            float spawnInterval = 1;
                            if(i % 2 == 0)
                            {
                                spawnInterval *= -1;
                            }
                            int temp = i;
                            if (i - 1 > 0)
                            {
                                if (i - 1 % 2 != 0)
                                {
                                    temp = i - 1;
                                }
                            }
                            spawnPos[i] = new Vector3(transform.position.x + (temp * spawnInterval), transform.position.y + 2);
                        }
                    }
                    //am I being hit from above?
                    if (contact.normal.y < 0)
                    {
                        for (int i = 0; i < spawnPos.Length; i++)
                        {
                            float spawnInterval = .5f;
                            if (i % 2 == 0)
                            {
                                spawnInterval *= -1;
                            }
                            int temp = i;
                            if (i - 1 > 0)
                            {
                                if (i - 1 % 2 != 0)
                                {
                                    temp = i - 1;
                                }
                            }
                            spawnPos[i] = new Vector3(transform.position.x + temp * spawnInterval, transform.position.y - 2);
                        }
                    }
                }
                //am I getting hit from the left or right?
                else if(Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
                {
                    if (contact.normal.x >= 0)
                    { //am I being hit from the left?
                        for (int i = 0; i < spawnPos.Length; i++)
                        {
                            float spawnInterval = .5f;
                            if (i % 2 == 0)
                            {
                                spawnInterval *= -1;
                            }
                            int temp = i;
                            if (i - 1 > 0)
                            {
                                if (i - 1 % 2 != 0)
                                {
                                    temp = i - 1;
                                }
                            }
                            spawnPos[i] = new Vector3(transform.position.x + 2, transform.position.y + temp * spawnInterval);
                        }
                    }
                    //am I being hit from the right?
                    if (contact.normal.x < 0)
                    {
                        for (int i = 0; i < spawnPos.Length; i++)
                        {
                            float spawnInterval = .5f;
                            if (i % 2 == 0)
                            {
                                spawnInterval *= -1;
                            }
                            int temp = i;
                            if (i - 1 > 0)
                            {
                                if (i - 1 % 2 != 0)
                                {
                                    temp = i - 1;
                                }
                            }
                            spawnPos[i] = new Vector3(transform.position.x - 2, transform.position.y + temp * spawnInterval);
                        }
                    }
                }
            }
            StartCoroutine(DelayedSpawn());
            GameControl.currentState = GameControl.GameState.Combat;
        }
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(.5f);
        if (pi.redGoopAmount > 0)
        {
            Instantiate(redGoop, spawnPos[0], Quaternion.identity);
            if (pi.greenGoopAmount > 0)
            {
                Instantiate(greenGoop, spawnPos[1], Quaternion.identity);
                if (pi.blueGoopAmount > 0)
                {
                    Instantiate(blueGoop, spawnPos[2], Quaternion.identity);
                }
            }
            else if (pi.blueGoopAmount > 0)
            {
                Instantiate(blueGoop, spawnPos[1], Quaternion.identity);
            }
        }
        else if (pi.greenGoopAmount > 0)
        {
            Instantiate(greenGoop, spawnPos[0], Quaternion.identity);
            if (pi.greenGoopAmount > 0)
            {
                Instantiate(blueGoop, spawnPos[1], Quaternion.identity);
            }
        }
        else if (pi.blueGoopAmount > 0)
        {
            Instantiate(blueGoop, spawnPos[0], Quaternion.identity);
        }
        this.gameObject.SetActive(false);
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

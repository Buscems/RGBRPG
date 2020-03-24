using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;
using Rewired.ControllerExtensions;

public class OverworldPlayer : MonoBehaviour
{

    //the following is in order to use rewired
    [Tooltip("Reference for using rewired")]
    [HideInInspector]
    public Player myPlayer;
    [Header("Rewired")]
    [Tooltip("Number identifier for each player, must be above 0")]
    public int playerNum;

    [Header("Inventory")]

    [Header("Amount of Goops")]
    public int redGoopAmount;
    public int greenGoopAmount;
    public int blueGoopAmount;

    [Header("UI")]
    public TextMeshProUGUI redAmount;
    public TextMeshProUGUI greenAmount;
    public TextMeshProUGUI blueAmount;

    public GameObject playerCamera;

    public enum Direction { North, South, East, West }
    [Header("Movement")]
    public Direction currentDirection;
    public Vector2 direction;
    bool isMoving;
    Vector3 startPos, endPos;
    float timeToMove;
    public float walkSpeed;
    public AudioSource walkThud;
    bool playThud;

    Animator anim;

    [Header("Combat Setup")]
    public GameObject redGoop;
    public GameObject greenGoop;
    public GameObject blueGoop;

    Vector3[] spawnPos = new Vector3[3];

    public bool canGoDown;
    public bool canGoUp;
    public bool canGoRight;
    public bool canGoLeft;

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
            anim = GetComponent<Animator>();
        }
        catch { }

        canGoUp = true;
        canGoDown = true;
        canGoLeft = true;
        canGoRight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameControl.currentState == GameControl.GameState.Overworld)
        {

            // Cast a ray straight down.
            RaycastHit2D hitDown = Physics2D.Raycast(transform.position + new Vector3(0, -.51f, 0), -Vector2.up, .9f);

            Debug.DrawRay(transform.position, Vector2.up);
            Debug.DrawRay(transform.position, -Vector2.right);
            Debug.DrawRay(transform.position, Vector2.right);

            // If it hits something...
            if (hitDown.collider != null)
            {
                if (hitDown.collider.gameObject.layer == 8)
                {
                    canGoDown = false;
                    Debug.Log("It work?");
                    Debug.DrawRay(transform.position + new Vector3(0, -.51f, 0), -Vector2.up, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(transform.position + new Vector3(0, -.51f, 0), -Vector2.up, Color.white);
                canGoDown = true;
            }
            // Cast a ray straight up.
            RaycastHit2D hitUp = Physics2D.Raycast(transform.position + new Vector3(0, .51f, 0), Vector2.up, .9f);

            // If it hits something...
            if (hitUp.collider != null)
            {
                if (hitUp.collider.gameObject.layer == 8)
                {
                    canGoUp = false;
                    Debug.Log("It work?");
                    Debug.DrawRay(transform.position + new Vector3(0, .51f, 0), Vector2.up, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(transform.position + new Vector3(0, .51f, 0), Vector2.up, Color.white);
                canGoUp = true;
            }
            // Cast a ray straight right.
            RaycastHit2D hitRight = Physics2D.Raycast(transform.position + new Vector3(.51f, 0, 0), Vector2.right, .9f);

            // If it hits something...
            if (hitRight.collider != null)
            {
                if (hitRight.collider.gameObject.layer == 8)
                {
                    canGoRight = false;
                    Debug.Log("It work?");
                    Debug.DrawRay(transform.position + new Vector3(.51f, 0, 0), Vector2.right, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(transform.position + new Vector3(.51f, 0, 0), Vector2.right, Color.white);
                canGoRight = true;
            }
            // Cast a ray straight left.
            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + new Vector3(-.51f, 0, 0), -Vector2.right, .9f);

            // If it hits something...
            if (hitLeft.collider != null)
            {
                if (hitLeft.collider.gameObject.layer == 8)
                {
                    canGoLeft = false;
                    Debug.Log("It work?");
                    Debug.DrawRay(transform.position + new Vector3(-.51f, 0, 0), -Vector2.right, Color.yellow);
                }
            }
            else
            {
                Debug.DrawRay(transform.position + new Vector3(-.51f, 0, 0), -Vector2.right, Color.white);
                canGoLeft = true;
            }

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
                    if (currentDirection == Direction.South && !canGoDown)
                    {
                        if (!playThud)
                        {
                            walkThud.Play();
                            playThud = true;
                        }
                    }
                    else if (currentDirection == Direction.North && !canGoUp)
                    {
                        if (!playThud)
                        {
                            walkThud.Play();
                            playThud = true;
                        }
                    }
                    else if (currentDirection == Direction.East && !canGoRight)
                    {
                        if (!playThud)
                        {
                            walkThud.Play();
                            playThud = true;
                        }
                    }
                    else if (currentDirection == Direction.West && !canGoLeft)
                    {
                        if (!playThud)
                        {
                            walkThud.Play();
                            playThud = true;
                        }
                    }
                    else
                    {
                        Debug.Log("Yes");
                        StartCoroutine(Movement(transform));
                    }
                }
                else
                {
                    playThud = false;
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

        while (timeToMove < 1f)
        {
            timeToMove += Time.deltaTime * walkSpeed;
            entity.GetComponent<Rigidbody2D>().MovePosition(Vector3.Lerp(startPos, endPos, timeToMove));
            yield return null;
        }

        isMoving = false;
        yield return 0;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {

            var colPos = collision.gameObject.transform.position;

            //am I being hit from below?
            if (colPos.y < this.transform.position.y)
            {
                for (int i = 0; i < spawnPos.Length; i++)
                {
                    float spawnInterval = 1;
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
                    spawnPos[i] = new Vector3(transform.position.x + (temp * spawnInterval), transform.position.y + 2);
                }
            }
            /*
            //am I being hit from above?
            for (int i = 0; i < spawnPos.Length; i++)
            {
                float spawnInterval = 1;
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

            //am I being hit from the left?
            for (int i = 0; i < spawnPos.Length; i++)
            {
                float spawnInterval = 1;
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

            //am I being hit from the right
            for (int i = 0; i < spawnPos.Length; i++)
            {
                float spawnInterval = 1;
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
            */
            StartCoroutine(DelayedSpawn());
            GameControl.currentState = GameControl.GameState.Combat;
        }
    }

    IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(.5f);
        if (redGoopAmount > 0)
        {
            var red = Instantiate(redGoop, spawnPos[0], Quaternion.identity);
            red.GetComponent<PlayerAttacks>().goopAmount = redGoopAmount;
            red.name = "RedGoops";
            if (greenGoopAmount > 0)
            {
                var green = Instantiate(greenGoop, spawnPos[1], Quaternion.identity);
                green.name = "GreenGoops";
                if (blueGoopAmount > 0)
                {
                    var blue = Instantiate(blueGoop, spawnPos[2], Quaternion.identity);
                    blue.name = "BlueGoops";
                }
            }
            else if (blueGoopAmount > 0)
            {
                var blue = Instantiate(blueGoop, spawnPos[1], Quaternion.identity);
                blue.name = "BlueGoops";
            }
        }
        else if (greenGoopAmount > 0)
        {
            var green = Instantiate(greenGoop, spawnPos[0], Quaternion.identity);
            green.name = "GreenGoops";
            if (greenGoopAmount > 0)
            {
                var blue = Instantiate(blueGoop, spawnPos[1], Quaternion.identity);
                blue.name = "BlueGoops";
            }
        }
        else if (blueGoopAmount > 0)
        {
            var blue = Instantiate(blueGoop, spawnPos[0], Quaternion.identity);
            blue.name = "BlueGoops";
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

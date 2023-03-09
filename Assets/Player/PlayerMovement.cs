using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform m_Camera;

    [Header("Movement")]
    public float movespeed = 6f;
    public float crouchmovespeed = 2f;
    public float movementMultiplier = 10f;
    public float dashstrength = 6f;
    public float slideBoost = 4f;
    [SerializeField] float airMultiplier = 0.4f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] KeyCode dashKey = KeyCode.LeftShift;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Drag")]
    float groundDrag = 6f;
    float airDrag = 2f;

    float playerHeight = 2f;



    public float crouch = -1f;

    float horizontalMovement;
    float verticalMovemement;
    

    bool isGrounded;
    Vector3 moveDirection;

    Rigidbody rb;
    public BoxCollider hitbox;
    private void Start()
    {
        //sphereAttack = GetComponent<GameObject>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Awake()
    {
        
    }
    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);

        MyInput();
        ControlDrag();

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            Jump();
        }
        if (Input.GetKeyDown(crouchKey))
        {        
            Crouch();
        }
        else if (Input.GetKeyUp(crouchKey) && isGrounded)
        {
            Stand();
        }
        else if (Input.GetKeyUp(crouchKey) && isGrounded == false)
        {
            Stand();
        }

        if (Input.GetKeyDown(dashKey))
        {
            Dash();
        }
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovemement = Input.GetAxisRaw("Vertical");

        moveDirection = transform.forward * verticalMovemement + transform.right * horizontalMovement;
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void Crouch()
    {       
        if (rb.velocity.magnitude != 0 && isGrounded)
        {
            rb.AddForce(moveDirection.normalized * movespeed * slideBoost, ForceMode.Impulse);
        }
        m_Camera.transform.Translate(0,-0.5f, 0);

        hitbox.center = new Vector3(0, -0.5f,0);
        hitbox.size = new Vector3(0.75f, 1, 0.75f);
    }
    void Stand()
    {
        m_Camera.transform.Translate(0, 0.5f, 0);
        hitbox.center = new Vector3(0, 0, 0);
        hitbox.size = new Vector3(0.75f, 2, 0.75f);
    }

    void Dash()
    {
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * movespeed * dashstrength, ForceMode.VelocityChange);
        }
        else if (isGrounded == false)
        {
            rb.AddForce(moveDirection.normalized * movespeed * airMultiplier * dashstrength, ForceMode.VelocityChange);
        }
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * movespeed * movementMultiplier, ForceMode.Acceleration);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * movespeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
        
    }

}

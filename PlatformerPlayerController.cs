using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class PlatformerPlayerController.cs : MonoBehaviour
{
    //Movement
    //public float moveSpeed = 6;
    public float walkSpeed = 4;
    public float sprintSpeed = 8;
    float lastMove = 1;

    public float jumpVelocity = 7;
    [Range(0, 1)]
    public float airControlPercent = 0.4f;
    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;

    public float currentSpeed;

    public float fireRate = 0; //hit per second
    float timeToFire;

    Rigidbody2D playerRigidbody;
    BoxCollider2D boxCollider2D;
    Animator animator;

    //Grounding
    public bool isGrounded;// => IsGrounded();
    [SerializeField]  LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        IsGrounded();

        HandleMovement();
        HandleShoot();
        HandleJump();
    }

    void HandleJump()
    {
        if (isGrounded && (Input.GetKeyDown(KeyCode.Space)))
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpVelocity);
        }
    }


    void HandleMovement()
    {
        

        float inputDir;
        if (Input.GetKey(KeyCode.Q))
        {
            inputDir = -1;
        }
        else
        {
            if (Input.GetKey(KeyCode.D))
            {
                inputDir = 1;
            }
            else
            {
                inputDir = 0;
                if (isGrounded)
                {
                    currentSpeed = 0;
                    playerRigidbody.velocity = new Vector2(currentSpeed, playerRigidbody.velocity.y);
                    return;
                }
            }
        }
        bool running = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = ((running) ? sprintSpeed : walkSpeed) * inputDir;

        //float targetSpeed = moveSpeed * inputDir;
        
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
        playerRigidbody.velocity = new Vector2(currentSpeed, playerRigidbody.velocity.y);

        currentSpeed = playerRigidbody.velocity.x;
    }


    private void HandleShoot()
    {
        if (fireRate == 0)
        {
            if (Input.GetButtonDown("Fire1")) Hit();
            else if (Input.GetButtonDown("Fire2")) Throw();
        }
        else
        {
            if (Input.GetButton("Fire1") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Hit();
            }
            else if (Input.GetButton("Fire2") && Time.time > timeToFire)
            {
                timeToFire = Time.time + 1 / fireRate;
                Throw();
            }
        }
    }

    void Hit()
    {
        Debug.Log("Hit");

    }
    
    void Throw()
    {
        Debug.Log("Throw");
    }

    private void IsGrounded()
    {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, extraHeight, mask);
        isGrounded = raycastHit.collider != null;
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (isGrounded)
        {
            return smoothTime;
        }
        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

}

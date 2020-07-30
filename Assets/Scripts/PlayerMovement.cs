using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public CharacterController controller;

    public float speed = 7f;//(default: 7f)
    float sprintSpeed = 9f;//(default: 9f)
    float crouchSpeed = 5f;//(default: 5f)
    float jumpHeight = 1f;//(default: 1f)
    float gravity = -20f;//(default: -20f)
    float groundDistance = 0.5f;//(default: 0.5f)
    float ceilingDistance = 0.2f;//(default: 0.2f)
    float ceilingDistanceCrouched = 0.5f;//set safe distance with which the player can stand up (essentially ceilingDistance + player height) (default: 0.5f)

    //gameobject to check if player is on ground or on the ceiling
    public Transform groundCheck;
    public Transform ceilingCheck;

    //layer mask to determine if the interactable model is ground or a ceiling
    public LayerMask groundMask;
    public LayerMask ceilingMask;
    Vector3 move;

    public bool isGrounded; //boolean to check if player's feet are touching the ground
    
    //check if player is hitting ceiling
    bool isHittingCeiling;
    bool isHittingCeilingCrouched;

    //falling velocity
    Vector3 velocity;
    Vector3 lastVelocity;

    //variables to help determine if the player is crouching, sprinting
    public bool isCrouching = false;
    public bool isSprinting = false;

    float crouchHeight = 0.7f;//crouch height value (crouchHeight * playerHeight) default 0.7f

    //initial player values to reset them after changing
    float originalControllerSpeed;
    float originalControllerHeight;

    void Start()
    {
        //save the original players values for future reference
        originalControllerHeight = controller.height;
        originalControllerSpeed = speed;
    }

    void Update()
    {
        InputChecks();
        Jump();
        Sprint();
        Crouch();
        Move();
    }

    private void InputChecks()
    {
        //check if player is on the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //check if player is hitting the ceiling
        isHittingCeiling = Physics.CheckSphere(ceilingCheck.position, ceilingDistance, ceilingMask);

        //check if player can stand up (whether he hits the ceiling if he stands up)
        if (isCrouching) isHittingCeilingCrouched = Physics.CheckSphere(ceilingCheck.position, ceilingDistanceCrouched, ceilingMask);

        //get inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //define move direction depending on inputs x and z
        move = transform.right * x + transform.forward * z;
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if (!isCrouching)
            {
                //save your velocity
                lastVelocity = move;
                //jump
                velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            }
            /*
            else if(!isHittingCeilingCrouched)
            {
                //uncrouch
                isCrouching = false;
                controller.height = originalControllerHeight;
            }
            */
        }

        if (!isGrounded)
        {
            //disables stepOffset and slopelimit when not on ground because so we cannot climb up a stair while falling
            controller.slopeLimit = 0f;
            controller.stepOffset = 0f;
        }
        else
        {
            //when on ground enable step offset and slopelimit
            controller.slopeLimit = 45;
            controller.stepOffset = 0.4f;
        }

        //if player hits ceiling, switch to falling velocity
        if (isHittingCeiling) velocity.y = -2.0f;

        //reset velocity
        if (isGrounded && velocity.y < 0) velocity.y = -2.0f;
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded && !isCrouching)
        {
            //sprint
            isSprinting = true;
            speed = sprintSpeed;
        }
        else if (isGrounded && !isCrouching)
        {
            //stop sprinting
            isSprinting = false;
            speed = originalControllerSpeed;
        }
    }

    private void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl) && !isSprinting)
        {
            //crouch
            isCrouching = true;
            controller.height = crouchHeight;
            speed = crouchSpeed;
        }
        else if (!isHittingCeilingCrouched && !isSprinting)
        {
            //uncrouch
            isCrouching = false;
            controller.height = originalControllerHeight;
            speed = originalControllerSpeed;
        }
    }

    private void Move()
    {
        //always change move speed to its maximum value (to counter diagnal speed problem)
        if (move.magnitude >= 1.0f) move.Normalize();
        if (lastVelocity.magnitude >= 1.0f) lastVelocity.Normalize();

        //move the player, if the player is not touching the ground, you can only control its movements at 50% power
        if (!isGrounded)
        {
            controller.Move(lastVelocity * 0.5f * speed * Time.deltaTime);
            controller.Move(move * 0.5f * speed * Time.deltaTime);
        }
        else controller.Move(move * speed * Time.deltaTime);

        //velocity calculation
        velocity.y += gravity * Time.deltaTime;

        //applying velocity (an extra Time.deltaTime because its based on a physics formula)
        controller.Move(velocity * Time.deltaTime);
    }
}
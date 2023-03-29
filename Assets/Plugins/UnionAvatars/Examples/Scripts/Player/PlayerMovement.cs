using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnionAvatars.Examples
{
    public class PlayerMovement : MonoBehaviour
    {
        //Movement Variables
        const float walkSpeed = 1.4f;
        const float runSpeed = 3;
        const float jumpForce = 5;
        const float gravity = -.2f;
        const float rotationSpeed = 20;
        const float animationBlendSpeed = 10;
        private LayerMask groundLayer;

        //Component References
        private Animator playerAnimator;

        //State
        private float movementBlend = 0;
        private bool _isGrounded;
        private bool IsGrounded
        {
            get => _isGrounded;
            set
            {
                _isGrounded = value;
                playerAnimator.SetBool("isFalling", !value);
            }
        }
        private float yDirection = 0;

        //Input Variables
        private float inputHorizontal;
        private float inputVertical;
        private bool isRunning;

        private void Start()
        {
            playerAnimator = GetComponent<Animator>();
            groundLayer = LayerMask.GetMask("Default"); //Initialize the ground layer mask
        }

        private void Update()
        {
            GetInput();
        }

        void FixedUpdate()
        {
            CheckGround();

            //Gravity
            if(!IsGrounded)
            {
                yDirection += gravity;
            }
            else if(yDirection < 0)
            {
                //Stop the falling
                yDirection = 0;
            }

            var speed = isRunning ? runSpeed : walkSpeed;

            var horizontalMovement = inputHorizontal * speed;
            var verticalMovement = inputVertical * speed;

            Vector3 direction = new Vector3(horizontalMovement, yDirection, verticalMovement);

            transform.Translate(direction * Time.deltaTime, Space.World);
            
            RotateCharacter(direction);

            AnimateCharacter();
        }

        private void CheckGround()
        {
            IsGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.12f, groundLayer);
        }

        private void GetInput()
        {
            //Keys: A, D
            inputHorizontal = Input.GetAxisRaw("Horizontal");
            //Keys: W, S
            inputVertical = Input.GetAxisRaw("Vertical");

            isRunning = Input.GetKey(KeyCode.LeftShift);

            //Jump
            if(Input.GetKeyDown(KeyCode.Space) && IsGrounded)
            {
                yDirection = jumpForce;
            }
        }

        private void AnimateCharacter()
        {
            if(!IsGrounded)
                return;

            float speedBlend = 0;

            if(InputIsPressed())
            {
                if(isRunning)
                    speedBlend = 1;
                else
                    speedBlend = 0.5f;
            }

            movementBlend = Mathf.Lerp(movementBlend, speedBlend, Time.deltaTime * animationBlendSpeed);

            playerAnimator.SetFloat("Speed", movementBlend);
        }

        private bool InputIsPressed()
        {
            return Math.Abs(inputHorizontal) > 0 || Math.Abs(inputVertical) > 0;
        }

        private void RotateCharacter(Vector3 direction)
        {
            if(!InputIsPressed()) return;

            //Remove y axis
            direction.y = 0;

            var newRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
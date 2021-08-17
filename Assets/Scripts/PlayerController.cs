using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8.0f;
    [SerializeField] private float runSpeed = 12.0f;
    [SerializeField] private float gravityModifier = 2.0f;
    [SerializeField] private float jumpPower = 8.0f;
    [SerializeField] private float mouseSensitivity = 2.0f;
    private bool canJump, canDoubleJump;
    private Vector3 moveInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Animator anim;
    
    void Update()
    {
        PlayerMovement();
        CameraRotation();
        PlayBobbing();        
    }

    void PlayerMovement()
    {
        float yStore = moveInput.y;

        Vector3 verticalMove = transform.forward * Input.GetAxis("Vertical");
        Vector3 horizontalMove = transform.right * Input.GetAxis("Horizontal");

        moveInput = horizontalMove + verticalMove;
        moveInput.Normalize();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveInput *= runSpeed;
        }
        else
        {
            moveInput *= moveSpeed;   
        }

        moveInput.y = yStore;

        Jump();

        _characterController.Move(moveInput * Time.deltaTime);
    }

    void CameraRotation()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        cameraTransform.rotation =
            Quaternion.Euler(cameraTransform.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));
    }

    void Jump()
    {
        moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

        if (_characterController.isGrounded)
        {
            moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
        }

        canJump = Physics.OverlapSphere(groundCheckPoint.position, .5f, whatIsGround).Length > 0;

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            moveInput.y = jumpPower;

            canDoubleJump = true;
        }
        else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            moveInput.y = jumpPower;

            canDoubleJump = false;
        }
    }

    void PlayBobbing()
    {
        anim.SetFloat("moveSpeed", moveInput.magnitude);
        anim.SetBool("onGround",canJump);
    }
}
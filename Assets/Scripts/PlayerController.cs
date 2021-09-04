using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private float moveSpeed = 8.0f;
    [SerializeField] private float runSpeed = 12.0f;
    [SerializeField] private float gravityModifier = 2.0f;
    [SerializeField] private float lowGravity = 0.1f;
    [SerializeField] private float jumpPower = 8.0f;
    [SerializeField] private float mouseSensitivity = 2.0f;
    private bool canJump, canDoubleJump;
    private Vector3 moveInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Animator anim;

    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;

    [SerializeField] public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UIController.instance.ammoText.text = "Ammo: " + activeGun.currentAmmo;
    }

    void FixedUpdate()
    {
        PlayerMovement();
        CameraRotation();
        Shooting();
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
        anim.SetBool("onGround", canJump);
    }

    void Shooting()
    {
        if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 50.0f))
            {
                if (Vector3.Distance(cameraTransform.position, hit.point) > 2.0f)
                {
                    firePoint.LookAt(hit.point);
                }
            }
            else
            {
                firePoint.LookAt(cameraTransform.position + (cameraTransform.forward * 30.0f));
            }
            
            //Instantiate(bullet, firePoint.position, firePoint.rotation);
            FireShot();
        }

        //repeats shots :)
        if (Input.GetMouseButton(0) && activeGun.canAutoFire)
        {
            if (activeGun.fireCounter <= 0)
            {
                FireShot();
            }
        }
    }

    public void FireShot()
    {
        if (activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;

            Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);

            activeGun.fireCounter = activeGun.fireRate;
            
            UIController.instance.ammoText.text = "Ammo: " + activeGun.currentAmmo;
        }
    }

    public void LowerGravity()
    {
        gravityModifier = lowGravity;
        moveSpeed = 1.0f;
        runSpeed = 20.0f;
    }

    public void GravityBack()
    {
        gravityModifier = 2.0f;
        moveSpeed = 8.0f;
        runSpeed = 12.0f;
    }
}
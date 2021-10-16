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
    [SerializeField] private float lowGravity = 0.001f;
    [SerializeField] private float jumpPower = 8.0f;
    [SerializeField] private float mouseSensitivity = 2.0f;
    private bool canJump, canDoubleJump;
    private Vector3 moveInput;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Animator anim;

    //[SerializeField] private GameObject bullet;
    [SerializeField] private Transform firePoint;

    [SerializeField] public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public List<Gun> unlockableGuns = new List<Gun>();
    [SerializeField] private int currentGun = 0;

    [SerializeField] private Transform adsPoint, gunHolder;
    private Vector3 gunStartPosition;
    [SerializeField] private float adsSpeed = 2.0f;

    [SerializeField] private GameObject muzzleFlash;

    [SerializeField] private float maxViewAngle = 60.0f;

    private float bounceAmount;
    private bool bounce;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentGun--;
        SwitchGun();

        gunStartPosition = gunHolder.localPosition;
    }

    void FixedUpdate()
    {
        if (!UIController.instance.pauseScreen.activeInHierarchy && !GameManager.instance.ending)
        {
            PlayerMovement();
            CameraRotation();
            Shooting();
            PlayBobbing();
        }
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

            AudioManager.instance.PlaySFX(0);
        }
        else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
        {
            moveInput.y = jumpPower;

            canDoubleJump = false;

            AudioManager.instance.PlaySFX(0);
        }

        if (bounce)
        {
            bounce = false;
            moveInput.y = bounceAmount;

            canDoubleJump = true;
        }
    }

    void PlayBobbing()
    {
        anim.SetFloat("moveSpeed", moveInput.magnitude);
        anim.SetBool("onGround", canJump);
    }

    void Shooting()
    {
        muzzleFlash.SetActive(false);

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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchGun();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CameraController.instance.ZoomIn(activeGun.zoomAmount);
        }

        if (Input.GetMouseButton(1))
        {
            gunHolder.position = Vector3.MoveTowards(gunHolder.position, adsPoint.position, adsSpeed * Time.deltaTime);
        }
        else
        {
            gunHolder.localPosition =
                Vector3.MoveTowards(gunHolder.localPosition, gunStartPosition, adsSpeed * Time.deltaTime);
        }

        if (Input.GetMouseButtonUp(1))
        {
            CameraController.instance.ZoomOut();
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

            muzzleFlash.SetActive(true);
        }
    }

    public void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);

        currentGun++;

        if (currentGun >= allGuns.Count)
        {
            currentGun = 0;
        }

        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);

        UIController.instance.ammoText.text = "Ammo: " + activeGun.currentAmmo;

        firePoint.position = activeGun.firePoint.position;
    }

    public void AddGun(string gunToAdd)
    {
        Debug.Log("Adding " + gunToAdd);

        bool gunUnlocked = false;

        if (unlockableGuns.Count > 0)
        {
            for (int i = 0; i < unlockableGuns.Count; i++)
            {
                if (unlockableGuns[i].gunName == gunToAdd)
                {
                    gunUnlocked = true;
                    allGuns.Add(unlockableGuns[i]);
                    unlockableGuns.RemoveAt(i);
                    i = unlockableGuns.Count;
                }
            }
        }

        if (gunUnlocked)
        {
            currentGun = allGuns.Count - 2;
            SwitchGun();
        }
    }

    public void Bounce(float bounceForce)
    {
        bounceAmount = bounceForce;
        bounce = true;
    }

    public void LowerGravity()
    {
        gravityModifier = lowGravity;
        moveSpeed = 1.0f;
        runSpeed = 2.0f;
    }

    public void GravityBack()
    {
        gravityModifier = 2.0f;
        moveSpeed = 8.0f;
        runSpeed = 12.0f;
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class PlayerController : MonoBehaviour
// {
//     public static PlayerController instance;
//
//     public float moveSpeed, gravityModifier, jumpPower, runSpeed = 12f;
//     public CharacterController charCon;
//
//     private Vector3 moveInput;
//
//     public Transform camTrans;
//
//     public float mouseSensitivity;
//     public bool invertX;
//     public bool invertY;
//
//     private bool canJump, canDoubleJump;
//     public Transform groundCheckPoint;
//     public LayerMask whatIsGround;
//
//     public Animator anim;
//
//     //public GameObject bullet;
//     public Transform firePoint;
//
//     public Gun activeGun;
//     public List<Gun> allGuns = new List<Gun>();
//     public List<Gun> unlockableGuns = new List<Gun>();
//     public int currentGun;
//
//     public Transform adsPoint, gunHolder;
//     private Vector3 gunStartPos;
//     public float adsSpeed = 2f;
//
//     public GameObject muzzleFlash;
//
//     public AudioSource footstepFast, footstepSlow;
//
//     private float bounceAmount;
//     private bool bounce;
//
//     private void Awake()
//     {
//         instance = this;
//     }
//
//     // Start is called before the first frame update
//     void Start()
//     {
//         currentGun--;
//         SwitchGun();
//
//         gunStartPos = gunHolder.localPosition;
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         if (!UIController.instance.pauseScreen.activeInHierarchy && !GameManager.instance.ending)
//         {
//             //moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
//             //moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
//
//             //store y velocity
//             float yStore = moveInput.y;
//
//             Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
//             Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");
//
//             moveInput = horiMove + vertMove;
//             moveInput.Normalize();
//
//             if (Input.GetKey(KeyCode.LeftShift))
//             {
//                 moveInput = moveInput * runSpeed;
//             }
//             else
//             {
//                 moveInput = moveInput * moveSpeed;
//             }
//
//             moveInput.y = yStore;
//
//             moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;
//
//             if (charCon.isGrounded)
//             {
//                 moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
//             }
//
//             canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;
//
//             if (canJump)
//             {
//                 canDoubleJump = false;
//             }
//
//             //Handle Jumping
//             if (Input.GetKeyDown(KeyCode.Space) && canJump)
//             {
//                 moveInput.y = jumpPower;
//
//                 canDoubleJump = true;
//
//                 AudioManager.instance.PlaySFX(8);
//             }
//             else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
//             {
//                 moveInput.y = jumpPower;
//
//                 canDoubleJump = false;
//
//                 AudioManager.instance.PlaySFX(8);
//             }
//
//
//             if (bounce)
//             {
//                 bounce = false;
//                 moveInput.y = bounceAmount;
//
//                 canDoubleJump = true;
//             }
//
//             charCon.Move(moveInput * Time.deltaTime);
//
//
//             //control camera rotation
//             Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) *
//                                  mouseSensitivity;
//
//             if (invertX)
//             {
//                 mouseInput.x = -mouseInput.x;
//             }
//
//             if (invertY)
//             {
//                 mouseInput.y = -mouseInput.y;
//             }
//
//             transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
//                 transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
//
//             camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));
//
//             muzzleFlash.SetActive(false);
//
//             //Handle Shooting
//             //single shots
//             if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
//             {
//                 RaycastHit hit;
//                 if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f))
//                 {
//                     if (Vector3.Distance(camTrans.position, hit.point) > 2f)
//                     {
//                         firePoint.LookAt(hit.point);
//                     }
//                 }
//                 else
//                 {
//                     firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
//                 }
//
//
//                 //Instantiate(bullet, firePoint.position, firePoint.rotation);
//                 FireShot();
//             }
//
//             //repeating shots
//             if (Input.GetMouseButton(0) && activeGun.canAutoFire)
//             {
//                 if (activeGun.fireCounter <= 0)
//                 {
//                     FireShot();
//                 }
//             }
//
//             if (Input.GetKeyDown(KeyCode.Tab))
//             {
//                 SwitchGun();
//             }
//
//             if (Input.GetMouseButtonDown(1))
//             {
//                 CameraController.instance.ZoomIn(activeGun.zoomAmount);
//             }
//
//             if (Input.GetMouseButton(1))
//             {
//                 gunHolder.position =
//                     Vector3.MoveTowards(gunHolder.position, adsPoint.position, adsSpeed * Time.deltaTime);
//             }
//             else
//             {
//                 gunHolder.localPosition =
//                     Vector3.MoveTowards(gunHolder.localPosition, gunStartPos, adsSpeed * Time.deltaTime);
//             }
//
//
//             if (Input.GetMouseButtonUp(1))
//             {
//                 CameraController.instance.ZoomOut();
//             }
//
//
//             anim.SetFloat("moveSpeed", moveInput.magnitude);
//             anim.SetBool("onGround", canJump);
//         }
//     }
//
//     public void FireShot()
//     {
//         if (activeGun.currentAmmo > 0)
//         {
//             activeGun.currentAmmo--;
//
//             Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);
//
//             activeGun.fireCounter = activeGun.fireRate;
//
//             UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
//
//             muzzleFlash.SetActive(true);
//         }
//     }
//
//     public void SwitchGun()
//     {
//         activeGun.gameObject.SetActive(false);
//
//         currentGun++;
//
//         if (currentGun >= allGuns.Count)
//         {
//             currentGun = 0;
//         }
//
//         activeGun = allGuns[currentGun];
//         activeGun.gameObject.SetActive(true);
//
//         UIController.instance.ammoText.text = "AMMO: " + activeGun.currentAmmo;
//
//         firePoint.position = activeGun.firePoint.position;
//     }
//
//     public void AddGun(string gunToAdd)
//     {
//         bool gunUnlocked = false;
//
//         if (unlockableGuns.Count > 0)
//         {
//             for (int i = 0; i < unlockableGuns.Count; i++)
//             {
//                 if (unlockableGuns[i].gunName == gunToAdd)
//                 {
//                     gunUnlocked = true;
//
//                     allGuns.Add(unlockableGuns[i]);
//
//                     unlockableGuns.RemoveAt(i);
//
//                     i = unlockableGuns.Count;
//                 }
//             }
//         }
//
//         if (gunUnlocked)
//         {
//             currentGun = allGuns.Count - 2;
//             SwitchGun();
//         }
//     }
//
//     public void Bounce(float bounceForce)
//     {
//         bounceAmount = bounceForce;
//         bounce = true;
//     }
//
//     public void LowerGravity()
//     {
//         gravityModifier = 0.001f;
//         moveSpeed = 1.0f;
//         runSpeed = 2.0f;
//     }
//
//     public void GravityBack()
//     {
//         gravityModifier = 2.0f;
//         moveSpeed = 8.0f;
//         runSpeed = 12.0f;
//     }
// }
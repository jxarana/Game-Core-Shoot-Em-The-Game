using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage, IInventorySystem, ICanGrappleNow
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Transform orientation;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] Animator animator;
    [Header("General Stats")]
    [SerializeField] int HPOrig;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpVel;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int dashMax;
    [SerializeField] int deathDepth; // Set the height that the player can fall to before dieing
    [SerializeField] int dashCd;
    //[SerializeField] Transform camPivot;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] public playerStats upgradeableStats;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject keyModel;

    [SerializeField] float climbSpeed;
    [SerializeField] float maxClimbTime;
    private float climbTimer;

    [SerializeField] float detectLength;
    [SerializeField] float sphereCastRadius;
    [SerializeField] float maxWallLookAngle;
    private float wallLookAngle;

    public int goldCount;
    public int upgradePoints;
    [Header("Damage")]
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int magMax;
    [SerializeField] int maxAmmo;
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] List<itemPickUp> itemList = new List<itemPickUp>();

    [Header("Slam")]
    [SerializeField] private float minSlamHeight = 3f; // distance needed to be above closest ground to slam
    [SerializeField] private float slamForce = 30f;    //how fast the play slams down
    [SerializeField] private float slamRadius = 5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private GameObject slamEffect;
    [SerializeField] private int minSlamDmg;
    [SerializeField] private int maxSlamDmg;
    [SerializeField] int slamCD;
    bool slaming;
    bool canSlam;

    [SerializeField] public float staminaOrig;
    public float stamina;
    [SerializeField] float staminaRegenRate;
    [SerializeField] float sprintStaminaCost;
    [SerializeField] float mantleStaminaCost;
    [SerializeField] float climbStaminaCost;
    public float grappleStaminaCost;
    bool isSprinting;
    bool canUseStamina;

    [SerializeField] float mantleCheckDist = 1f;
    [SerializeField] float mantleHeight = 1.5f;
    [SerializeField] float mantleDuration = 0.3f;
    [SerializeField] LayerMask mantleLayer;

    [SerializeField] AudioClip gunClip;
    [SerializeField] AudioClip deathClip;

    bool isMantling = false;
    Vector3 mantleStartPos;
    Vector3 mantleEndPos;
    float mantleTimer;

    int dashCount;
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;
    float dashCooldownTimer;
    bool isDashing;
    float dashTimeLeft;
    private Vector3 dashDirection;

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HP;
    int speedOrig;
    int gunListPos;

    float shootTimer;
    float xRotation = 0f;
    bool hasSlamunlocked = false;
    bool hasDashUnlocked = false;
    bool hasGrappleUnlocked = false;
    int magCurrent;
    int currentAmmo;

    public int dmgUp;


    public bool isGrappling;

    private bool isClimbing;
    private bool isWallFront;
    private RaycastHit wallHit;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HP = HPOrig;
        speedOrig = speed + upgradeableStats.speed;
        canUseStamina = true;
        isSprinting = false;
        stamina = staminaOrig;
        isGrappling = false;
        isMantling = false;
        magCurrent = magMax;
        currentAmmo = maxAmmo;
        dashCount = dashMax + upgradeableStats.maxDashes;
        jumpMax = jumpMax + upgradeableStats.maxJumps;
        stamina = staminaOrig;



        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Regenerate stamina only if not doing stamina-draining actions
        if (!isSprinting && !isGrappling && !isClimbing && !isMantling && stamina < staminaOrig)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, staminaOrig);
            updatePlayerUI();
        }

        if (gameManager.instance != null && gameManager.instance.isPaused)
        {
            return;
        }

        if (isMantling)
        {
            MantleMove();
            return;
        }

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        sprint();
        movement();
        if (!controller.isGrounded && Input.GetKey(KeyCode.Space))
        {
            TryMantle();
        }
        // handleCamera();

        wallCheck();
        stateMachine();

    }

    private void stateMachine()
    {
        if (isWallFront && Input.GetKey(KeyCode.C) && wallLookAngle < maxWallLookAngle)
        {
            if (!isClimbing && climbTimer > 0 && stamina > 0)
            {
                startClimbing();
            }

            if (climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }
            if (climbTimer < 0)
            {
                stopClimbing();
            }
        }
        else
        {
            if (isClimbing)
            {
                stopClimbing();
            }
        }
    }


    void movement()
    {
        // Player instantly dies if he falls to a certain depth on the map
        if (gameObject.transform.position.y <= deathDepth)
        {
            gameManager.instance.youLose();
        }

        if (isMantling)
        {
            return;
        }

        shootTimer += Time.deltaTime;
        if (dashCount < dashMax)
        {
            dashCooldownTimer -= Time.deltaTime;
            if (dashCooldownTimer <= 0) // gives the player a dash after x amount of time
            {
                dashCount++;
                dashCooldownTimer = dashCd;
            }
        }

        if (controller.isGrounded)
        {
            playerVel = Vector3.zero;
            jumpCount = 0;
            dashCount = 0;

        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");
        animator.SetFloat("Horizontal", horizontalMovement);
        animator.SetFloat("Vertical", verticalMovement);


        jump();

        controller.Move(playerVel * Time.deltaTime);

        if (!isGrappling)
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        if (Input.GetButtonDown("Dash") && dashCount > 0) // so long as the player has a dash stack they can dash

        {
            StartCoroutine(Dash());
        }

        if (Input.GetButtonDown("Crouch / Slam") && !isMantling && !canSlam)
        {


        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isGrappling && gunList.Count > 0 && gunList[gunListPos].ammoCurr > 0 && magCurrent > 0 && shootTimer > shootRate)
            {
                shoot();
                updatePlayerUI();
            }
            else if (!isGrappling && shootTimer > shootRate && magCurrent == 0)
            {
                reload();
                updatePlayerUI();
            }
        }

        if (Input.GetButtonDown("Reload"))
        {
            if (magCurrent != magMax)
            {
                reload();
                updatePlayerUI();
            }
        }
        selectGun();
        if (isClimbing)
        {
            climbingMovement();
        }
    }

    void jump()
    {
        if (!isMantling && Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel += transform.up * jumpVel;
            jumpCount++;
        }

        if (isClimbing)
        {
            climbingMovement();
        }
    }

    void sprint()
    {
        if (Input.GetButton("Sprint") && controller.isGrounded && moveDir.magnitude > 0.1f && stamina > 0)
        {
            if (!isSprinting)
            {
                isSprinting = true;
                speed *= sprintMod;
            }

            //reduce stamina by the cost and clamp so it doesn't go outside of the stamina range
            stamina -= sprintStaminaCost * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, staminaOrig);
            updatePlayerUI();

            if (stamina <= 0.1f)
            {
                stamina = 0;
                StopSprinting();
            }
        }
        else if(Input.GetButtonUp("Sprint"))
        {
            StopSprinting();
        }
    }

    void StopSprinting()
    {
        if (isSprinting)
        {
            speed = speedOrig;
            isSprinting = false;
        }
    }

    void TryMantle()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 forward = transform.forward;

        if (Physics.Raycast(origin, forward, out RaycastHit wallHit, mantleCheckDist, mantleLayer))
        {
            Vector3 ledgeCheckOrigin = wallHit.point + Vector3.up * mantleHeight;
            if (Physics.Raycast(ledgeCheckOrigin, Vector3.down, out RaycastHit topHit, mantleHeight, mantleLayer))
            {
                if (stamina >= mantleStaminaCost)
                {
                    stamina -= mantleStaminaCost;
                    updatePlayerUI();

                    isMantling = true;
                    mantleTimer = 0f;
                    mantleStartPos = transform.position;
                    mantleEndPos = new Vector3(topHit.point.x, topHit.point.y + 0.1f, topHit.point.z);
                }
            }
        }
    }

    void MantleMove()
    {
        mantleTimer += Time.deltaTime;
        float t = mantleTimer / mantleDuration;
        t = Mathf.Clamp01(t);
        controller.enabled = false;
        transform.position = Vector3.Lerp(mantleStartPos, mantleEndPos, t);

        if (t >= 1f)
        {
            isMantling = false;
            controller.enabled = true;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        magCurrent--;
        gunList[gunListPos].ammoCurr--;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            //Debug.Log(hit.collider.name);
            Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage + dmgUp);
            }
        }
    }

    public void takeDamage(int amount)
    {
        HP -= amount;

        updatePlayerUI();

        StartCoroutine(damageFlashScreen());

        if (HP <= 0)
        {
            //you dead!
            gameManager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.playerStaminaBar.fillAmount = (float)stamina / staminaOrig;
        gameManager.instance.ammoBar.fillAmount = (float)magCurrent / magMax;
        gameManager.instance.inMagCount.text = magCurrent.ToString();
        gameManager.instance.currAmmoCount.text = currentAmmo.ToString();


    }

    IEnumerator damageFlashScreen()
    {
        gameManager.instance.playerDamagePanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamagePanel.SetActive(false);
    }


    void reload()
    {
        if (currentAmmo > 0)
        {
            magCurrent = magMax;
            currentAmmo -= magMax;
        }
    }

    public void replenishAmmo()
    {
        currentAmmo = maxAmmo;
        updatePlayerUI();
    }

    public void healhp(int ammount)
    {
        HP = Mathf.Min(HP + ammount, HPOrig);
        updatePlayerUI();
    }
    public void dashUnlock()
    {
        hasDashUnlocked = true;
    }
    public void grappleUnlock()
    {
        hasGrappleUnlocked = true;
    }

    public void slamUnlock()
    {
        hasSlamunlocked = true;
    }
    public void dashCountUp()
    {
        dashMax += 1;
    }

    public void jumpCountUp()
    {
        jumpMax += 1;
    }

    public void speedUp()
    {
        speed += 1;
        speedOrig = speed;
    }
    public bool dashReturn()
    {
        return hasDashUnlocked;
    }

    public bool grappleReturn()
    {
        return hasGrappleUnlocked;
    }

    public bool slamReturn()
    {

        return hasSlamunlocked;
    }


    //void handleCamera()
    //{
    //    if (Time.timeScale <= 0f) return;
    //    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
    //    float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

    //    transform.Rotate(Vector3.up * mouseX);

    //    xRotation -= mouseY;
    //    xRotation = Mathf.Clamp(xRotation, -60f, 60f);
    //    camPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    //}

    private void wallCheck()
    {
        isWallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out wallHit, detectLength, wallLayer);
        wallLookAngle = Vector3.Angle(orientation.forward, -wallHit.normal);

        if (controller.isGrounded)
        {
            climbTimer = maxClimbTime;
        }
    }
    private void climbingMovement()
    {
        if (stamina > 0)
        {
            stamina -= climbStaminaCost * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, staminaOrig);
            updatePlayerUI();

            //Move upward while stamina is still available
            playerVel = new Vector3(playerVel.y, climbSpeed, playerVel.z);
        }
        else
        {
            //Stop climbing if stamina runs out
            stopClimbing();
        }
    }

    private void startClimbing()
    {
        isClimbing = true;
    }

    private void stopClimbing()
    {
        isClimbing = false;
    }

    IEnumerator Dash()
    {
        isDashing = true;

        dashCount--;

        float startTime = Time.time;

        while (Time.time < startTime + dashDuration)
        {
            if (moveDir.magnitude > 0.1f) // if input detected dash in that direction
            {
                controller.Move(moveDir * (dashSpeed * Time.deltaTime));
            }
            else
                controller.Move(-transform.forward * (dashSpeed * Time.deltaTime));
            yield return null;
        }


        isDashing = false;
    }

    void slamDistCheck()
    {
        RaycastHit hit;
        bool groundClose = Physics.Raycast(transform.position, Vector3.down, out hit, minSlamHeight); //want to make it scale off of distance


        if (!groundClose)
        {
            StartCoroutine(SLAM());
        }
    }

    void slamImpact()
    {
        if (slamEffect != null)
        {
            slamEffect.SetActive(true);
            StartCoroutine(waitSec(.7f));
            slamEffect.SetActive(false);


        }
        Collider[] enemiesIn = Physics.OverlapSphere(transform.position, slamRadius, enemyLayer);
        foreach (Collider enemy in enemiesIn)
        {
            if (enemy.TryGetComponent(out enemyAI Enemy))
                Enemy.takeDamage(minSlamDmg);

        }

    }

    IEnumerator SLAM()
    {
        slaming = true;
        canSlam = false;

        controller.Move(Vector3.down * slamForce * Time.deltaTime);

        while (!controller.isGrounded)
        {
            yield return null;
        }


        slamImpact();
        yield return new WaitForSeconds(slamCD);
        slaming = false;
        canSlam = true;
    }

    IEnumerator waitSec(float secs)
    {
        yield return new WaitForSeconds(secs);
    }

    public void getItemPickUp(itemPickUp item)
    {
        itemList.Add(item);
        gameManager.instance.keyPrefab = item.keyItem;

        keyModel.GetComponent<MeshFilter>().sharedMesh = item.keyItem.GetComponent<MeshFilter>().sharedMesh;
        keyModel.GetComponent<MeshRenderer>().sharedMaterial = item.keyItem.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void getGunStats(gunStats gun)
    {
        gunList.Add(gun);
        gunListPos = gunList.Count - 1;

        changeGun();
    }

    void changeGun()
    {

        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].model.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].model.GetComponent<MeshRenderer>().sharedMaterial;
    }

    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }
}

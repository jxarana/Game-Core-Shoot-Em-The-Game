using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;

//using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage, IInventorySystem, ICanGrappleNow
{
    public static playerController instance;

    [Header("Cameras")]
    [SerializeField] CinemachineCamera normalCam;
    [SerializeField] CinemachineCamera aimCam;
    Transform shootPos;

    [SerializeField] GameObject reticle;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Transform orientation;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] public Animator animator;

    [Header("General Stats")]
    [SerializeField] float HPOrig;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpVel;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int dashMax;
    /*[SerializeField] int deathDepth;*/ // Set the height that the player can fall to before dieing
    [SerializeField] int dashCd;
    [SerializeField] Transform followTarget;
    //[SerializeField] Transform camPivot;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] public playerStats upgradeableStats;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject keyModel;
    [SerializeField] float parryRadius;
    [SerializeField] float climbSpeed;
    [SerializeField] float maxClimbTime;
    [SerializeField] int parryForce;
    [SerializeField] JunkGun myGun;
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

    [Header("Audio Settings:")]
    [SerializeField] AudioSource playerSounds;
    [SerializeField] AudioClip[] playerSoundsClip;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] deathClip;
    [SerializeField] float deathVolume;
    [SerializeField] AudioClip[] playerHurtClip;
    [SerializeField] float hurtVol;
    [SerializeField] AudioClip[] playerReloadClip;
    [SerializeField] float reloadVol;
    [SerializeField] AudioClip[] audStep;
    [SerializeField] float audStepVol;

    [Header("Buffs")]
    bool immortality = false;
    bool unlimitedAmmo = false;
    public int damageMult = 1;
    int extraSpeed = 0;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight;
    [SerializeField] private Vector3 crouchPosition = new(0, 0.595f, 0);
    [SerializeField] private float crouchingSpeed = 7f;
    private float height;
    private Vector3 center;
    private bool crouched;
    private bool lastBitOfLife = false;

    public float crouchSpeed = 1.0f;

    public Rigidbody centerOfMass;

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
    float HP;
    int speedOrig;
    int gunListPos;
    bool IsDead;

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

    bool isPlayingStep;

    private Rigidbody[] ragdollRigidBodies;

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
        followTarget = GameObject.FindGameObjectWithTag("followTarget").transform;
        normalCam = GameObject.FindGameObjectWithTag("NormalCam").GetComponent<CinemachineCamera>();
        aimCam = GameObject.FindGameObjectWithTag("AimCam").GetComponent<CinemachineCamera>();
        reticle = GameObject.FindGameObjectWithTag("Reticle");
        reticle.SetActive(false);
        normalCam.Priority = 10;
        aimCam.Priority = 5;
        center = controller.center;
        height = controller.height;
        myGun = GameObject.FindWithTag("MyGun").GetComponent<JunkGun>();

        updatePlayerUI();
    }

    void Awake()
    {
        ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        disableRagdoll();
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

        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        sprint();
        movement();
        Aim();
        if (!controller.isGrounded && Input.GetKey(KeyCode.Space))
        {
            TryMantle();
        }
        // handleCamera();

        wallCheck();
        stateMachine();
        updateColliders();

        if (Input.GetKey(KeyCode.V))
        {
            crouched = true;
            animator.SetBool("IsCrouched", true);
        }
        if(Input.GetKeyUp(KeyCode.V))
        {
            crouched = false;
            animator.SetBool("IsCrouched", false);
        }
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
        if (IsDead)
        {
            return;
        }

        if(!controller.enabled)
        {
                       return;
        }

        // Player instantly dies if he falls to a certain depth on the map
        //if (gameObject.transform.position.y <= deathDepth)
        //{
        //    gameManager.instance.youLose();
        //}

        if (isMantling)
        {
            return;
        }

        if (isSprinting)
        {
            crouched = false;
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
            if (!isPlayingStep && moveDir.normalized.magnitude > 0.3f)
            {
                StartCoroutine(playStep());
            }

            playerVel = Vector3.zero;
            jumpCount = 0;
            dashCount = 0;

        }
        // Get Input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Camera yaw (from FollowTarget)
        Vector3 camForward = followTarget.forward;
        Vector3 camRight = followTarget.right;

        // Flatten to XZ plane
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Combine movement relative to camera
        moveDir = camForward * vertical + camRight * horizontal;

        // If there is input, rotate the player to face camera forward
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(camForward, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }

        // Apply movement
        if (moveDir.magnitude > 0.1f)
        {
            controller.Move(moveDir * (speed + extraSpeed) * Time.deltaTime);
        }

        if (moveDir != Vector3.zero)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

            Vector3 localMove = transform.InverseTransformDirection(moveDir);

        localMove.Normalize();

        float animVertical = localMove.z;
        float animHorizontal = localMove.x;

        if (isSprinting && vertical > 0f)
        {
            animVertical = 2f;
        }

        // Set animation
        animator.SetFloat("Horizontal", animHorizontal);
        animator.SetFloat("Vertical", animVertical);


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

        if (Input.GetButtonDown("Crouch / Slam") && !isMantling && canSlam)
        {
            slamImpact();

        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isGrappling && myGun.inMag > 0 && shootTimer > myGun.fireRate)
            {
                shoot();
                updatePlayerUI();
            }
            else if (!isGrappling && shootTimer > myGun.fireRate && myGun.inMag == 0)
            {
                reload();
                updatePlayerUI();
            }
        }

        if (Input.GetButtonDown("Reload"))
        {
            if (myGun.inMag != myGun.magMax)
            {
                reload();
                updatePlayerUI();
            }
        }
       
        if (isClimbing)
        {
            climbingMovement();
        }

        if(Input.GetButtonDown("Parry"))
        {
            parry();
        }

        //if (Input.GetKey(KeyCode.V))
        //{
        //    animator.SetBool("IsCrouched", crouched);
        //}
    }

    private void updateColliders()
    {
        Vector3 tCenter = center;
        float tHeight = height;

        if (crouched)
        {
            tCenter = crouchPosition;
            tHeight = crouchHeight;
        }

        controller.height = Mathf.Lerp(controller.height, tHeight, crouchingSpeed * Time.deltaTime);
        controller.center = Vector3.Lerp(controller.center, tCenter, crouchingSpeed * Time.deltaTime);
    }

    void jump()
    {
        if (!isMantling && Input.GetButtonDown("Jump") && jumpCount < jumpMax && !animator.IsInTransition(0))
        {
            if (!crouched)
            {
                playerVel += transform.up * jumpVel;
                jumpCount++;
                playerSounds.PlayOneShot(playerSoundsClip[Random.Range(0, playerSoundsClip.Length)], audJumpVol);
                animator.SetTrigger("Jump");

                if (isSprinting == true)
                {
                    animator.SetBool("IsRunning", true);
                }
                else
                {
                    animator.SetBool("IsRunning", false);
                }
            }

            else if(crouched)
            {
                crouched = false;
            }
        }

        if (isClimbing)
        {
            climbingMovement();
        }
    }

    private void Aim()
    {
        if (Input.GetButton("Aim"))
        {
            normalCam.gameObject.SetActive(false);
            aimCam.gameObject.SetActive(true);
            reticle.SetActive(true);
            animator.SetBool("IsAiming", true);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, aimCam.transform.eulerAngles.y + 45f, 0f), Time.deltaTime * 15f);
        }
        if (Input.GetButtonUp("Aim"))
        {
            normalCam.gameObject.SetActive(true);
            aimCam.gameObject.SetActive(false);
            reticle.SetActive(false);
            animator.SetBool("IsAiming", false);
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
        else if (Input.GetButtonUp("Sprint"))
        {
            StopSprinting();
        }
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;
        playerSounds.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);

        if (isSprinting)
            yield return new WaitForSeconds(0.3f);

        else
            yield return new WaitForSeconds(0.5f);

        isPlayingStep = false;
    }

    void StopSprinting()
    {
        if (isSprinting)
        {
            speed = speedOrig + extraSpeed;
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
        if (!unlimitedAmmo)
        {
            myGun.inMag--;
        }
         myGun.gunSound.PlayOneShot(myGun.soundEffect, gameManager.instance.audioLevels.effectVol);


      
         Instantiate(myGun.randomBullet(),myGun.shootPos.position, aimCam.transform.rotation);      
    }

    public void takeDamage(float amount)
    {
        if (!immortality)
        {
            if (!lastBitOfLife && HP > (HPOrig * .041))
            {
                HP -= amount;
            }
            else if(!lastBitOfLife && HP < (HPOrig * .041))
            {
                lastBitOfLife = true;
            }
            else if(lastBitOfLife)
            {
                HP -= (amount / 3);
            }
            updatePlayerUI();

            StartCoroutine(damageFlashScreen());

            playerSounds.PlayOneShot(playerHurtClip[Random.Range(0, playerHurtClip.Length)], hurtVol);

            if (HP <= 0)
            {
                //you dead!
                IsDead = true;
                enableRagdoll();
                StartCoroutine(activateLoseMenuAfterDelay(3f));
                playerSounds.PlayOneShot(deathClip[Random.Range(0, deathClip.Length)], deathVolume);
            }
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
        gameManager.instance.playerStaminaBar.fillAmount = (float)stamina / staminaOrig;
        gameManager.instance.ammoBar.fillAmount = (float)myGun.inMag / myGun.magMax;
        gameManager.instance.inMagCount.text = myGun.magMax.ToString();
        gameManager.instance.currAmmoCount.text = myGun.inMag.ToString();
        gameManager.instance.goldCountUI.text = goldCount.ToString();

    }

    IEnumerator damageFlashScreen()
    {
        gameManager.instance.playerDamagePanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamagePanel.SetActive(false);
    }


    void reload()
    {
      myGun.inMag = myGun.magMax; 
      playerSounds.PlayOneShot(playerReloadClip[Random.Range(0, playerReloadClip.Length)], reloadVol);
       
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

    void parry()
    {
        int enemyBulletLayer = LayerMask.NameToLayer("Enemy Bullet");
        int playerBulletLayer = LayerMask.NameToLayer("Player Bullet");

        // Get all enemy bullets in range
        GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("Bullet");

        foreach (GameObject bullet in enemyBullets)
        {
            // Check if it's an enemy bullet by layer
            if (bullet.layer == enemyBulletLayer &&
                Vector3.Distance(transform.position, bullet.transform.position) <= parryRadius)
            {
                ParrySingleBullet(bullet, playerBulletLayer);
            }
        }
    }

    void ParrySingleBullet(GameObject bullet, int newLayer)
    {
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Reflect back toward where it came from
            Vector3 reflectDirection = -rb.linearVelocity.normalized;
            rb.linearVelocity = reflectDirection * parryForce;

            // Change layer to player bullet
            bullet.layer = newLayer;

            Debug.Log("Bullet parried!");
        }
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

        //keyModel.GetComponent<MeshFilter>().sharedMesh = item.keyItem.GetComponent<MeshFilter>().sharedMesh;
       // keyModel.GetComponent<MeshRenderer>().sharedMaterial = item.keyItem.GetComponent<MeshRenderer>().sharedMaterial;
    }

   

   
   
    public void savePlayerData(out int gold, out int upgrades, out float hp, out int ammo, out int mag )
    {
        gold = goldCount;
        upgrades = upgradePoints;
        hp = HP;
        ammo = currentAmmo;
        mag = magCurrent;
      
    }

    public void loadPlayerData(int gold, int upgrades, float hp, int ammo, int mag)
    {
        goldCount = gold;
        upgradePoints = upgrades;
        HP = hp;
        currentAmmo = ammo;
        magCurrent = mag;
        

        updatePlayerUI();
    }

    private void disableRagdoll()
    {
        foreach (var rigidbody in ragdollRigidBodies)
        {
            rigidbody.isKinematic = true;
        }

        animator.enabled = true;
        controller.enabled = true;
    }

    private void enableRagdoll()
    {
        Vector3 oppositeDir = -centerOfMass.transform.forward + centerOfMass.transform.up;
        foreach (var rigidbody in ragdollRigidBodies)
        {
            rigidbody.isKinematic = false;
            rigidbody.AddForce(oppositeDir * 30f, ForceMode.Impulse);
        }

        animator.enabled = false;
        controller.enabled = false;
    }

    IEnumerator activateLoseMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gameManager.instance.menuLose != null)
        {
            gameManager.instance.statePause();
            gameManager.instance.menuActive = gameManager.instance.menuLose;
            gameManager.instance.menuLose.SetActive(true);
        }
    }

    void crouchedMovement()
    {

    }

    public IEnumerator applyImmortality(float duration)
    {
        immortality = true;

       yield return new WaitForSeconds(duration);

        immortality = false;
    }

    public IEnumerator applyUnlimitedAmmo(float duration) { 
        
        unlimitedAmmo = true;

        yield return new WaitForSeconds(duration);

        unlimitedAmmo = false;
    
    
    }

    public IEnumerator applyDamageMult(int mult, float duration)
    {
        damageMult = mult;
        yield return new WaitForSeconds(duration);
        damageMult = 1;
    }

    public IEnumerator applyHealthRegen(int amount, float duration)
    {
        float endTime = Time.deltaTime + duration;
        float timer = Time.deltaTime;
        float perTick = amount / duration;

        while (timer < endTime)
        {
            timer += Time.deltaTime;
            healhp((int)perTick);
            yield return new WaitForSeconds(1);
        }

      
    }

    public IEnumerator applySpeedUp(int speed,float duration)
    {
        extraSpeed = speed;
        yield return new WaitForSeconds(duration);
        extraSpeed = 0;
    }



}

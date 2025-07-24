using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage, IInventorySystem, ICanGrappleNow
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    [Header("General Stats")]
    [SerializeField] int HPOrig;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpVel;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int dashMax;
    [SerializeField] int dashCd;
   // [SerializeField] Transform camPivot;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] public playerStats upgradeableStats;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject keyModel;

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

    int dashDuration = 1;

    [SerializeField] public float grappleStaminaCost;
    [SerializeField] public float staminaOrig;

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
    [SerializeField] AudioClip[] audArena;
    [SerializeField] float audArenaVol;

    bool isMantling = false;
    Vector3 mantleStartPos;
    Vector3 mantleEndPos;
    float mantleTimer;

    public float stamina;
    int dashCount;
    public float dashSpeed;

    float dashCooldownTimer;
    bool isDashing;

    private Vector3 dashDirection;



    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    int HP;
    int speedOrig;
    int gunListPos;

    float shootTimer;
    float xRotation = 0f;




    int magCurrent;
    int currentAmmo;

    public int dmgUp;


    public bool isGrappling;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HP = HPOrig;
        speedOrig = speed + upgradeableStats.speed;
        isGrappling = false;
        magCurrent = magMax;
        currentAmmo = maxAmmo;
        dashCount = dashMax + upgradeableStats.maxDashes;
        jumpMax = jumpMax + upgradeableStats.maxJumps;
        stamina = staminaOrig;

        playerSounds.PlayOneShot(audArena[Random.Range(0, audArena.Length)], audArenaVol);

        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
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
        handleCamera();

    }

    void movement()
    {
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

        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);

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
            RaycastHit hit;
            bool groundClose = Physics.Raycast(transform.position, Vector3.down, out hit, minSlamHeight); //want to make it scale off of distance


            if (!groundClose) // player is x distance above the ground so slam
            {
                StartCoroutine(SLAM());
            }
            else // not high enough, slam
            {

            }

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
    }

    void jump()
    {
        if (!isMantling && Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            playerVel += transform.up * jumpVel;
            jumpCount++;
        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
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
                isMantling = true;
                mantleTimer = 0f;
                mantleStartPos = transform.position;
                mantleEndPos = new Vector3(topHit.point.x, topHit.point.y + 0.1f, topHit.point.z);
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


    void handleCamera()
    {
        if (Time.timeScale <= 0f) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
       // camPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
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

        shootDamage = gunList[gunListPos].shootDamage + upgradeableStats.dmgIncreased;
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

    public void savePlayerData(out int gold, out int upgrades, out int hp, out int ammo, out int mag, out List<gunStats> guns)
    {
        gold = goldCount;
        upgrades = upgradePoints;
        hp = HP;
        ammo = currentAmmo;
        mag = magCurrent;
        guns = new List<gunStats>(gunList);
    }

    public void loadPlayerData(int gold, int upgrades, int hp, int ammo, int mag, List<gunStats> guns)
    {
        goldCount = gold;
        upgradePoints = upgrades;
        HP = hp;
        currentAmmo = ammo;
        magCurrent = mag;
        gunList = new List<gunStats>(guns);

        updatePlayerUI();
    }
}

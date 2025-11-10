version https://git-lfs.github.com/spec/v1
oid sha256:29199043878549a88fad31dae507945e8ee188df29a5924e71290e6c48f58953
size 5383

public class enemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource enemySounds;
    [SerializeField] AudioClip[] enemydeathClip;
    [SerializeField] float deathVol;
    [SerializeField] ParticleSystem deathAnim;
    [SerializeField] ParticleSystem healAnim;
    [SerializeField] LayerMask visionMask;

    [SerializeField] int goldDropped;
    [SerializeField] float HP;
    [SerializeField] int fov;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    public Rigidbody centerOfMass;

    Color colorOrg;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;


    bool playerInTrigger;

    Vector3 playerDir;
    Vector3 startingPos;

    private Rigidbody[] ragdollRigidBodies;

    void Awake()
    {
        ragdollRigidBodies = GetComponentsInChildren<Rigidbody>();
        disableRagdoll();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrg = model.material.color;
        gameManager.instance.updateGameGoal(0);
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);

        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
        }

        if (playerInTrigger && !canSeePlayer())
        {
            roamCheck();
        }
        else if (!playerInTrigger)
        {
            roamCheck();
        }
    }

    void roamCheck()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
    }

    bool canSeePlayer()
    {
        Transform player = gameManager.instance.player.transform;
        Vector3 targetPos = player.position + Vector3.up * 1f;
        playerDir = targetPos - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit, 100f, visionMask))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov && HP > 0)
            {
                shootTimer += Time.deltaTime;

                if (shootTimer >= shootRate)
                {
                    shoot();
                }

                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                    faceTarget();

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }

        return false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    public void FacePlayerInstantly(Transform player)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; //Keep upright

        //Rotate to face target
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation,faceTargetSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            agent.stoppingDistance = 0;
        }
    }
    public void takeDamage(float amount)
    {
        float previousHp = HP;

        HP -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            gameManager.instance.playerScript.upgradeableStats.dollarBills += goldDropped;
            gameManager.instance.goldCountUI.text = gameManager.instance.playerScript.upgradeableStats.dollarBills.ToString();
            gameManager.instance.updateGameGoal(-1);
            enemySounds.PlayOneShot(enemydeathClip[Random.Range(0, enemydeathClip.Length)], gameManager.instance.audioLevels.effectVol);
            enableRagdoll();
            Destroy(gameObject, 3);
        }
        else if (HP > previousHp)
        {
           ParticleSystem heal = Instantiate(healAnim, transform.position, Quaternion.identity);
           heal.transform.SetParent(transform);
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrg;
    }

    void shoot()
    {
        shootTimer = 0;

        Transform player = gameManager.instance.player.transform;
        Vector3 targetPos = player.position + Vector3.up * 1f;
        Vector3 direction = (targetPos - shootPos.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        GameObject newBullet = Instantiate(bullet, shootPos.position, lookRotation);

        Collider[] enemyColliders = GetComponentsInChildren<Collider>();
        Collider bulletCollider = newBullet.GetComponent<Collider>();

        foreach(Collider col in enemyColliders)
        {
            Physics.IgnoreCollision(bulletCollider, col);
        }
    }

    private void disableRagdoll()
    {
        foreach (var rigidbody in ragdollRigidBodies)
        {
            rigidbody.isKinematic = true;
        }

        anim.enabled = true;
    }

    private void enableRagdoll()
    {
        Vector3 oppositeDir = -centerOfMass.transform.forward + centerOfMass.transform.up;
        foreach (var rigidbody in ragdollRigidBodies)
        {
            rigidbody.isKinematic = false;
            rigidbody.AddForce(oppositeDir * 30f, ForceMode.Impulse);
        }

        anim.enabled = false;
    }
}

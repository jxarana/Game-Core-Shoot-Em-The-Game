using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class bossAI : MonoBehaviour, IDamage
{
    public enum BossState { idle, roam, chase, attack, dead, orbit }
    public BossState state = BossState.idle;

    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] AudioSource enemySounds;
    [SerializeField] AudioClip[] enemydeathClip;
    [SerializeField] float deathVol;
    [SerializeField] ParticleSystem deathAnim;

    [SerializeField] int goldDropped;
    [SerializeField] int HP;
    [SerializeField] int fov;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorOrg;

    float shootTimer;
    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;
    Vector3 lastKnownPos;

    bool playerInTrigger;
    bool hasLastPos;

    Vector3 playerDir;
    Vector3 startingPos;

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
        switch (state)
        {
            case BossState.idle:
                idle();
                break;
            case BossState.roam:
                roam();
                break;
            case BossState.chase:
                chase();
                break;
            case BossState.attack:
                attack();
                break;
            case BossState.dead:
                death();
                break;
            case BossState.orbit:
                orbit();
                break;
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

    private void orbit()
    {
        if (canSeePlayer() && state != BossState.attack || state != BossState.chase)
        {
            Vector3 playerDir = (gameManager.instance.player.transform.position - transform.position).normalized;
            Vector3 orbitPlayer = gameManager.instance.player.transform.position + Quaternion.Euler(0, 90, 0) * playerDir * 5f;
            agent.SetDestination(orbitPlayer);
        }
    }

    private void death()
    {
        state = BossState.dead;
        gameManager.instance.playerScript.goldCount += goldDropped;
        gameManager.instance.updateGameGoal(-1);
        //enemySounds.PlayOneShot(enemydeathClip[Random.Range(0, enemydeathClip.Length)], deathVol);
        Instantiate(deathAnim, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void chase()
    {
        if (canSeePlayer())
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                changeState(BossState.attack);
            }
        }
        else
        {
            if(hasLastPos)
            {
                agent.SetDestination(lastKnownPos);
                if(Vector3.Distance(transform.position, lastKnownPos) < 1f)
                {
                    changeState(BossState.idle);
                }
            }
            else
            {
                changeState(BossState.idle);
            }
        }
    }

    private void idle()
    {
        roamTimer += Time.deltaTime;
        if (canSeePlayer())
        {
            changeState(BossState.chase);
        }
        else if(roamTimer >= roamPauseTime)
        {
            changeState(BossState.roam);
        }
    }

    private void changeState(BossState curr)
    {
        state = curr;
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
        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;
            if (roamTimer >= roamPauseTime)
            {
                roamTimer = 0;
                agent.stoppingDistance = 0;

                Vector3 ranPos = Random.insideUnitSphere * roamDist;
                ranPos += startingPos;

                NavMeshHit hit;
                NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
                agent.SetDestination(hit.position);
            }
        }
        if (canSeePlayer())
        {
            changeState(BossState.chase);
        }
    }

    bool canSeePlayer()
    {
        Transform player = gameManager.instance.player.transform;
        Vector3 targetPos = player.position + Vector3.up * 1f;
        playerDir = targetPos - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        //Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fov)
            {
                //shootTimer += Time.deltaTime;

                //if (shootTimer >= shootRate)
                //{
                //    shoot();
                //}

                //agent.SetDestination(gameManager.instance.player.transform.position);

                //if (agent.remainingDistance <= agent.stoppingDistance)
                //    faceTarget();
                lastKnownPos = gameManager.instance.player.transform.position;
                hasLastPos = true;
                return true;
            }
        }
        agent.stoppingDistance = stoppingDistOrig;
        return false;
    }

    //void faceTarget()
    //{
    //    Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, transform.position.y, playerDir.z));
    //    transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    //}

    public void FacePlayerInstantly()
    {
        Vector3 direction = (gameManager.instance.player.transform.position - transform.position).normalized;
        direction.y = 0; //Keep upright

        //Rotate to face target
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, faceTargetSpeed * Time.deltaTime);
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
    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            death();
        }
        else
        {
            StartCoroutine(flashRed());
            if (state != BossState.chase && state != BossState.attack)
            {
                changeState(BossState.chase);
            }
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

        Instantiate(bullet, shootPos.position, lookRotation);
    }

    void attack()
    {
        if (canSeePlayer())
        {
            agent.stoppingDistance = stoppingDistOrig;
            FacePlayerInstantly();
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootRate)
            {
                shoot();
            }
        }
        else
        {
            changeState(BossState.chase);
        }
    }
}
version https://git-lfs.github.com/spec/v1
oid sha256:652eb8e21e5bc6d4975d9e6b8ff2ad2e01cbb5169ac13995c376bf3896589f3d
size 1466


public class follower : MonoBehaviour , IDamage// should only be used on levels with a complete flat ground for now



{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int fear = 0;

    [Header("Fear Gains")]
    [SerializeField] int hurtFear;
    [SerializeField] int playerHurtFear;
    [SerializeField] int nearMonsterFear;
  

    public bool isFeared;
    public bool playerClose;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

        if (!isFeared)
        {
            while (agent.remainingDistance > agent.stoppingDistance)
            {
                followPlayer();
            }
        }



    }

    void followPlayer()
    {
        agent.SetDestination(gameManager.instance.playerScript.transform.position);
    }

    public void gainFear(int fear)
    {
        fear = Mathf.Min(fear += fear, 100);
        if(fear >= 100)
        {
            isFeared = true;
        }
    }
    public void takeDamage(float amount)
    {
        health -= amount;
        fear += hurtFear;
        

        

        if (health <= 0)
        {
            //you dead!
            gameManager.instance.youLose();
        }
    }



}

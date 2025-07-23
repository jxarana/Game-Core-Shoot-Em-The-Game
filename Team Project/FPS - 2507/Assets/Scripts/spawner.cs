using UnityEngine;
using UnityEngine.Rendering;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToSpawn;
    [SerializeField] GameObject toSpawn;
    [SerializeField] int howMany;
    [SerializeField] float spawnRate;
    [SerializeField] Transform[] location;

    bool spawning;
    int spawned;
    float timeTracker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager.instance.updateGameGoal(howMany);
    }

    // Update is called once per frame
    void Update()
    {
       if(spawning)
        {
            timeTracker += Time.deltaTime;
            if(spawned < howMany && timeTracker > spawnRate) 
            {
                spawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        ICanGrappleNow spawned = other.GetComponent<ICanGrappleNow>();

        if (spawned != null)
        {
            for (int i = 0; i < objectsToSpawn.Length; ++i)
            {
                objectsToSpawn[i].SetActive(true);
            }
        }

        if (other.CompareTag("Player"))
            spawning = true;    

    }

    void spawn()
    {
        int arrayPos = Random.Range(0, location.Length);

        Instantiate(toSpawn, location[arrayPos].transform.position, location[arrayPos].transform.rotation);
        spawned++;
        timeTracker = 0;
    }
}

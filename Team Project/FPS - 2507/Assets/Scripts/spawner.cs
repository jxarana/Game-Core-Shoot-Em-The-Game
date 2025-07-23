using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class spawner : MonoBehaviour
{
    //[SerializeField] GameObject[] objectToSpawn;  why is it an array?
    [SerializeField] GameObject toSpawn;
    [SerializeField] int howMany;
    [SerializeField] float spawnRate;
    [SerializeField] Transform[] location;

    List<Transform> spawnlocal;
    bool spawning;
    int spawned;
    float timeTracker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnlocal = new List<Transform>(location);
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
        /*
        ICanGrappleNow spawned = other.GetComponent<ICanGrappleNow>(); what is this

        if (spawned != null)
        {
            for (int i = 0; i < objectToSpawn.Length; ++i)
            {
                objectToSpawn[i].SetActive(true);
            }
        }*/

        if(other.CompareTag("Player"))
            spawning = true;    

    }

    void spawn()
    {
        
       
        
        Instantiate(toSpawn, spawnlocal.First().transform.position, spawnlocal.First().transform.rotation);
         spawnlocal.Remove(spawnlocal.First());

        spawned++;
        timeTracker = 0;
    }
}

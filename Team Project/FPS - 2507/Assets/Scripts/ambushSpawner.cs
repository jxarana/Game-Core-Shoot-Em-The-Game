using Unity.VisualScripting;
using UnityEngine;

public class ambushSpawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int spawnAmount;
    [SerializeField] int spawnRate;
    [SerializeField] Transform[] spawnPos;

    float spawnTimer;
    int spawnCount;
    bool startSpawning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        gameManager.instance.updateGameGoal(spawnAmount);
    }

    // Update is called once per frame
    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if(spawnTimer >= spawnRate && spawnCount < spawnAmount)
            {
                Spawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    void Spawn()
    {
        int arrayPos = Random.Range(0,spawnPos.Length);

        Instantiate(objectToSpawn, spawnPos[arrayPos].transform.position, spawnPos[arrayPos].transform.rotation);
        spawnCount++;
        spawnTimer = 0;
    }
}

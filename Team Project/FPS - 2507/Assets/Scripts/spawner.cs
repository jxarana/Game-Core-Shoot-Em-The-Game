using UnityEngine;
using UnityEngine.Rendering;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject[] objectToSpawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        ICanGrappleNow spawned = other.GetComponent<ICanGrappleNow>();

        if (spawned != null)
        {
            for (int i = 0; i < objectToSpawn.Length; ++i)
            {
                Instantiate(objectToSpawn[i], transform.position, Quaternion.identity);
            }
        }
    }
}

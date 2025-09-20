using UnityEngine;

public class bossBulletSpawner : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(other);
        }
    }
}

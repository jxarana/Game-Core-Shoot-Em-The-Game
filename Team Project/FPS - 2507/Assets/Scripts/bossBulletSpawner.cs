using UnityEngine;

public class bossBulletSpawner : MonoBehaviour, IDamage
{
    public void takeDamage(int amount)
    {
        throw new System.NotImplementedException();
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(other);
        }
    }
}

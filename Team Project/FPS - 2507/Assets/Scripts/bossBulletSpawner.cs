using UnityEngine;

public class bossBulletSpawner : MonoBehaviour, IDamage
{
    public void takeDamage(float amount)
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

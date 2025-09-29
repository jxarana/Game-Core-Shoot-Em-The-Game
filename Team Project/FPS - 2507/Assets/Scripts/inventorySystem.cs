version https://git-lfs.github.com/spec/v1
oid sha256:eccb4d9a76a03893f6f9f4cc55f28eeacfb4755667580af402f7e59e7e044a6b
size 1213

public class inventorySystem : MonoBehaviour
{
    enum itemtype { shotgun, pistol, rifle, useableItem, statusEffect, powerUps }
    [SerializeField] itemtype type;
    [SerializeField] gunStats gun;
    [SerializeField] itemPickUp item;
    [SerializeField] powerUps power;
    [SerializeField] public Transform shootpos;

    private void Start()
    {
        if(type != itemtype.pistol || type  != itemtype.shotgun || type != itemtype.rifle )
        {
            shootpos = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IInventorySystem pickUppable = other.GetComponent<IInventorySystem>();

         if (pickUppable != null && type == itemtype.useableItem)
        {
            pickUppable.getItemPickUp(item);
            Destroy(gameObject);
        }
        else if(pickUppable != null && type == itemtype.powerUps)
        {
            pickUppable.getPowerUp(power);
            if (power.powerUp == powerUps.types.immortality)
            {
                gameManager.instance.playerScript.applyImmortality(power.duration);
            }
            else if (power.powerUp == powerUps.types.damageMult)
            {
                gameManager.instance.playerScript.applyDamageMult(power.damageMult, power.duration);
            }
            else if (power.powerUp == powerUps.types.speedUp)
            {
                gameManager.instance.playerScript.applySpeedUp(power.speedup, power.duration);
            }
            else if (power.powerUp == powerUps.types.unlimitedAmmo)
            {
                gameManager.instance.playerScript.applyUnlimitedAmmo(power.duration);
            }
            else if(power.powerUp == powerUps.types.healthRegen)
            {
                gameManager.instance.playerScript.applyHealthRegen(power.healthRegen, power.duration);
            }

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (pickUppable != null && type == itemtype.shotgun)
        {
            pickUppable.getGunStats(gun);
            gun.ammoCurr = gun.ammoMax;
            Destroy(gameObject);
        }
        else if (pickUppable != null && type == itemtype.pistol)
        {
            pickUppable.getGunStats(gun);
            gun.ammoCurr = gun.ammoMax;
            Destroy(gameObject);
        }
        else if (pickUppable != null && type == itemtype.rifle)
        {
            pickUppable.getGunStats(gun);
            gun.ammoCurr = gun.ammoMax;
            Destroy(gameObject);
        }
        else if (pickUppable != null && type == itemtype.useableItem)
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
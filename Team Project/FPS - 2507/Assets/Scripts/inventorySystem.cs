using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventorySystem : MonoBehaviour
{
    enum itemtype { shotgun, pistol, rifle, useableItem, statusEffect }
    [SerializeField] itemtype type;
    [SerializeField] gunStats gun;
    [SerializeField] itemPickUp item;
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
    }
}
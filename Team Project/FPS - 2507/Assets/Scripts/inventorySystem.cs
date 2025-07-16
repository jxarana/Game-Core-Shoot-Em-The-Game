using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inventorySystem : MonoBehaviour
{
    enum guntype { shotgun, pistol, rifle }
    [SerializeField] guntype type;
    [SerializeField] gunStats gun;


    private void OnTriggerEnter(Collider other)
    {
        IInventorySystem pickUppable = other.GetComponent<IInventorySystem>();

        if (pickUppable != null && type == guntype.shotgun)
        {
            pickUppable.getGunStats(gun);
            Destroy(gameObject);
        }
        else if (pickUppable != null && type == guntype.pistol)
        {
            pickUppable.getGunStats(gun);
            Destroy(gameObject);
        }
        else if (pickUppable != null && type == guntype.rifle)
        {
            pickUppable.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}

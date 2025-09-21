using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{

    enum damagetype { moving, stationary, DOT, homing }
    [SerializeField] damagetype type;
    [SerializeField] Rigidbody rb;
    [SerializeField] GameObject model;
    [SerializeField] GameObject shooter;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] bool heal;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(shooter != null &&  shooter.CompareTag("Player"))
        {
            damageAmount = damageAmount + gameManager.instance.playerScript.upgradeableStats.dmgIncreased * gameManager.instance.playerScript.damageMult;
        }



        if (heal)
            damageAmount *= -1;

        if (type == damagetype.moving || type == damagetype.homing)
        {
            Destroy(gameObject, destroyTime);

            if (type == damagetype.moving)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        if (type == damagetype.homing)
        {
            rb.linearVelocity = (gameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type != damagetype.DOT)
        {
            dmg.takeDamage(damageAmount);
        }

       

        if (type == damagetype.moving || type == damagetype.homing)
        {
            Destroy(gameObject);
        }

       
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent <IDamage>();

        if (dmg != null && type == damagetype.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
}

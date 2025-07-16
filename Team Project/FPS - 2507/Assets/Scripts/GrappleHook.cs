using Unity.VisualScripting;
using UnityEngine;

public class GrappleHook : MonoBehaviour
{
    [SerializeField] Transform cam;
    [SerializeField] CharacterController controller;
    [SerializeField] float pullSpeed;
    [SerializeField] float maxGrappleDistance;
    [SerializeField] playerController player;
    [SerializeField] LineRenderer lr;
    [SerializeField] Transform grappleOrigin;
    public LayerMask grappleLayer;

    Transform grappledEnemy;
    Vector3 enemyGrapplePoint;
    bool isGrappling;
    bool isAttached;
    Vector3 grapplePoint;
   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGrappling = false;
        isAttached = false;
        lr.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Grapple") && !isGrappling)
        {
            ShootGrapple();
        }
        
        if (isGrappling)
        {
            lr.enabled = true;
            lr.SetPosition(0, grappleOrigin.position);
            lr.SetPosition(1, grappledEnemy != null ? enemyGrapplePoint : grapplePoint);


            if (!isAttached)
            {
               //Drain stamina while pulling
               if(player.stamina > 0f)
                {
                    player.stamina -= player.grappleStaminaCost * Time.deltaTime;
                    player.stamina = Mathf.Clamp(player.stamina,0f, player.staminaOrig);
                    player.updatePlayerUI();
               }

               //Cancel grapple if stamina depletes
               if(player.stamina <= 0f)
                {
                    StopGrappling();
                    return;
                }

                if (grappledEnemy != null)
                {
                    //Set up to look at pleyer when grabbed by grapple
                    enemyAI enemy = grappledEnemy.GetComponent<enemyAI>();
                    if(enemy != null)
                    {
                        enemy.FacePlayerInstantly(transform);
                    }

                    //Pull enemy toward player
                    Vector3 pullDirection = (transform.position - grappledEnemy.position).normalized;
                    float enemyDistance = Vector3.Distance(grappledEnemy.position, transform.position);

                    if (enemyDistance > 1f)
                    {
                        grappledEnemy.position += pullDirection * pullSpeed * Time.deltaTime;
                    }
                    else
                    {
                        isAttached = true;
                        StopGrappling(); // Optional auto-stop when enemy reaches you 
                    }
                }
                else
                {
                    Vector3 direction = (grapplePoint - transform.position);
                    float distance = direction.magnitude;

                    if (distance < 1f)
                    {
                        isAttached = true;
                    }
                    else
                    {
                        direction.Normalize();
                        Vector3 move = direction * pullSpeed * Time.deltaTime;
                        controller.Move(move);
                    }
                }
            }
            else
            {
                Vector3 holdPosition = grapplePoint - transform.position;
                controller.Move(holdPosition);
            }

            if(Input.GetButtonUp("Grapple"))
            {
                StopGrappling();
            }
        }
        else
        {
            lr.enabled = false;
        }
    }

    void StopGrappling()
    {
        isGrappling = false;
        isAttached = false; 
        player.isGrappling = false;
        grappledEnemy = null;
    }

    void ShootGrapple()
    {
        Ray ray = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleDistance, grappleLayer))
        {
            grapplePoint = hit.point;
            isGrappling = true;
            isAttached = false;
            player.isGrappling = true;


            //Check if we hit an enemy
            if(hit.collider.CompareTag("Enemy"))
            {
                grappledEnemy = hit.collider.transform;
                enemyGrapplePoint = hit.point;
            }
            else
            {
                grappledEnemy = null;
                grapplePoint = hit.point;
            }
        }
    }
}

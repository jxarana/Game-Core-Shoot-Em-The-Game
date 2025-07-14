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
            lr.SetPosition(1, grapplePoint);


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

                Vector3 direction = (grapplePoint - transform.position);
                float distance = direction.magnitude;

                if(distance < 1f)
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
        }
    }
}

using UnityEngine;

public class RightAngleFlip : MonoBehaviour
{
    private Quaternion rot;
    private Vector3 origGravity;

    [SerializeField] public Vector3 floor; //get the floor to keep track of level



    private void Start()
    {
        origGravity = Physics.gravity;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.attachedRigidbody != null)
        {
            FlipDirection(transform.forward, col.transform);
        }
    }

    void FlipDirection(Vector3 newUpDirection, Transform tr)
    {
        var angleBetweenUpDirections = Vector3.Angle(newUpDirection, tr.up);
        var angleThreshold = 0.001f;

        if (angleBetweenUpDirections < angleThreshold)
        {
            return;
        }

        var rotationDifference = Quaternion.FromToRotation(tr.up, newUpDirection);
        tr.rotation = rotationDifference * tr.rotation;

        rot = gameManager.instance.player.transform.rotation;
        Physics.gravity = rot * origGravity;
    }
}
using UnityEngine;

public class floatProp : MonoBehaviour
{
    [SerializeField] float speed = 2f;
    [SerializeField] float height = 0.5f;
    Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(speed * Time.time) * height;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

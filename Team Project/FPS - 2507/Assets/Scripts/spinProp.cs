using UnityEngine;

public class spinProp : MonoBehaviour
{
    [Range(10f, 1000f)]
    [SerializeField] float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f,Space.Self);
    }
}

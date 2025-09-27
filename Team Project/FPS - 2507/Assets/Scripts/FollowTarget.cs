using Unity.Cinemachine;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    [SerializeField] private float rotationalSpeed = 10f;
    [SerializeField] private float bottomClamp = -40f;
    [SerializeField] private float topClamp = 70f;

    private float cinemachineTargetPitch;
    private float cinemachineTargetYaw;
    private void Start()
    {
        followTarget = GameObject.FindGameObjectWithTag("followTarget").transform;

        cinemachineTargetYaw = followTarget.eulerAngles.y;
        cinemachineTargetPitch = followTarget.eulerAngles.x;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void LateUpdate()
    {
        if(gameManager.instance.isPaused) { return; }
        CameraLogic();
    }
    private void CameraLogic()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationalSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationalSpeed;

        cinemachineTargetPitch = UpdateRotation(cinemachineTargetPitch, mouseY, bottomClamp, topClamp, true);
        cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);

        ApplyRotation(cinemachineTargetPitch,cinemachineTargetYaw);
    }

    private void ApplyRotation(float pitch, float yaw)
    {
        followTarget.rotation = Quaternion.Euler(pitch, yaw,followTarget.eulerAngles.z);
    }
    private float UpdateRotation(float currentRotation, float input, float min,float max, bool isXAxis)
    {
        currentRotation += isXAxis ? -input : input;
        return Mathf.Clamp(currentRotation, min, max);
    }
    //private float GetMouseInput(string axis)
    //{
    //    return Input.GetAxis(axis) * rotationalSpeed * Time.deltaTime;
    //}

}

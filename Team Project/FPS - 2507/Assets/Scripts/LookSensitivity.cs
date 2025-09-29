using UnityEngine;
using Unity.Cinemachine;

public class LookSensitivity : MonoBehaviour, AxisState.IInputAxisProvider
{
    [SerializeField] private float sensitivity = 3f;
    [SerializeField] private CinemachineInputProvider provider;

    public float GetAxisValue(int axis)
    {
        return provider.GetAxisValue(axis) * sensitivity;
    }

    public void SetSensitivity(float value)
    {
        sensitivity = Mathf.Clamp(value, 0.1f, 10f);
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("MouseSensitivity"))
            sensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
    }
}

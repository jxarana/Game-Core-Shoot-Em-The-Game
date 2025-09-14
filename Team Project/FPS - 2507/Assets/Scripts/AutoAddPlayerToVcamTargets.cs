using Unity.Cinemachine;
using UnityEngine;

public class AutoAddPlayerToVcamTargets : MonoBehaviour
{
    public string Tag = string.Empty;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var vcam = GetComponent<CinemachineVirtualCameraBase>();
        if (vcam != null && Tag.Length >0)
        {
            var target = GameObject.FindGameObjectWithTag(Tag);
            if(target != null)
            {
                vcam.Follow = target.transform;
            }
        }
    }
}

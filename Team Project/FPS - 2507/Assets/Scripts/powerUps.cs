using UnityEngine;

[CreateAssetMenu]

public class powerUps : ScriptableObject
{
    public enum types { damageUp, speedUp, invincible, unlimitedAmmo, regen}
    [SerializeField] GameObject model;
    [SerializeField] public int duration;
    [SerializeField] types powerUpType;



    public  void ApplyEffect(GameObject target)
    {
        switch (powerUpType)
        {
            case powerUpType.
        }
    }



}

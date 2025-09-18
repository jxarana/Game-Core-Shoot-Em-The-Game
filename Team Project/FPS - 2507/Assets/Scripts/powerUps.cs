using UnityEngine;



[CreateAssetMenu]


public class powerUps : ScriptableObject
{
    public enum types { damageMult, healthRegen, speedUp, immortality, unlimitedAmmo }


    [SerializeField] GameObject model;
    [SerializeField] public types powerUp;
    [SerializeField] public int duration;
    [SerializeField] public int damageMult;
    [SerializeField] public int healthRegen;
    [SerializeField] public int speedup;



}

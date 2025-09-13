using UnityEngine;

[CreateAssetMenu]
public class settings : ScriptableObject
{
   
    [Range(1, 100)] public int menuAudio;
    [Range(1, 100)] public int effectsAudio;
    [Range(1, 100)] public int musicAudio;




}
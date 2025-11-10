using UnityEngine;

[CreateAssetMenu]
public class gameSettings : ScriptableObject
{
    [Range(0, 1)] public float masterVol = 1f;
    [Range(0, 1)] public float musicVol = 1f;
    [Range(0, 1)] public float menuFeedBackVol = 1f;
    [Range(0, 1)] public float effectVol = 1f;




    public void saveSettings()
    {
        masterVol = gameManager.instance.masterVol.value;
        musicVol = gameManager.instance.musicVol.value ;
        menuFeedBackVol = gameManager.instance.menuVol.value ;
        effectVol = gameManager.instance.effectsVol.value ;


        gameManager.instance.music.volume = calcMusicVol();
        gameManager.instance.menuFeedBack.volume = calcMenuFeedBack();
        

    }

    public float calcMusicVol()
    {
        return musicVol * masterVol;
    }

    public float calcMenuFeedBack()
    {
        return menuFeedBackVol * masterVol;
    }

    public float calcEffectVol() { 
    
        return effectVol * masterVol;   
        
    
    }

    


}

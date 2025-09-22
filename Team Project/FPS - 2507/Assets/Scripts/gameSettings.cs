using UnityEngine;

[CreateAssetMenu]
public class gameSettings : ScriptableObject
{
     public float masterVol;
     public float musicVol;
     public float menuFeedBackVol;
     public float effectVol;



    public void saveSettings()
    {
        masterVol = gameManager.instance.masterVol.value/100;
        musicVol = gameManager.instance.musicVol.value/100;
        menuFeedBackVol = gameManager.instance.menuVol.value/100;
        effectVol = gameManager.instance.effectsVol.value/100;

        gameManager.instance.music.volume = (masterVol * musicVol)*100;
        gameManager.instance.menuFeedBack.volume = (masterVol * menuFeedBackVol)*100;
        

    }

    


}

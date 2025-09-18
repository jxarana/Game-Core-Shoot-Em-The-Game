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

        gameManager.instance.music.volume = masterVol * musicVol;
        gameManager.instance.menuFeedBack.volume = masterVol * menuFeedBackVol;
        

    }

    public void loadSettings() // dont need to load not doing "playpref"
    {
        gameManager.instance.music.volume = (masterVol / 100) * (musicVol / 100);
        gameManager.instance.menuFeedBack.volume = (masterVol / 100) * (menuFeedBackVol / 100);
    }



}

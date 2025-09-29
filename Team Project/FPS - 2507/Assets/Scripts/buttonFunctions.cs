using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        if(gameManager.instance.menuActive == gameManager.instance.menuPause)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (SceneManager.GetActiveScene().buildIndex == 5 && gameManager.instance.menuActive == gameManager.instance.menuWin || 
            gameManager.instance.menuActive == gameManager.instance.menuLose)
        {
            SceneManager.LoadScene("Level_1");
        }
            gameManager.instance.stateUnpause();
    }

    public void loadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextIndex);
        gameManager.instance.stateUnpause();
    }

    public void quit()
    {
#if !UNITY_EDITOR
        Application.Quit();
#else
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

   
    public void dmgUp() //done
    {
        if (gameManager.instance.playerScript.goldCount >= 80)
        {
            
            gameManager.instance.playerScript.upgradeableStats.dmgIncreased++;
            gameManager.instance.playerScript.goldCount -= 80;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.menufeedback(gameManager.instance.itemBought,gameManager.instance.audioLevels.menuFeedBackVol);
        }
    }

    public void ammoRefill()
    {
        if (gameManager.instance.playerScript.goldCount >= 50)
        {
            gameManager.instance.playerScript.replenishAmmo();
            gameManager.instance.playerScript.goldCount -= 50;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.menufeedback(gameManager.instance.itemBought, gameManager.instance.audioLevels.menuFeedBackVol);
        }
    }//done

    public void heal25()
    {
        if(gameManager.instance.playerScript.goldCount >= 20)
        { 
            gameManager.instance.playerScript.healhp(25);
            gameManager.instance.playerScript.goldCount -= 20;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.menufeedback(gameManager.instance.itemBought, gameManager.instance.audioLevels.menuFeedBackVol);
        }
    } // done
    public void heal50()
    {
        if (gameManager.instance.playerScript.goldCount >= 45)
        {
            gameManager.instance.playerScript.healhp(50);
            gameManager.instance.playerScript.goldCount -= 45;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.menufeedback(gameManager.instance.itemBought, gameManager.instance.audioLevels.menuFeedBackVol);
        }
    } // done
    public void heal75()
    {
        if (gameManager.instance.playerScript.goldCount >= 70)
        {
            gameManager.instance.playerScript.healhp(75);
            gameManager.instance.playerScript.goldCount -= 70;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.menufeedback(gameManager.instance.itemBought, gameManager.instance.audioLevels.menuFeedBackVol);
        }
    } //done

    

  

   

    

    public void dashUp()
    {
        if (gameManager.instance.playerScript.goldCount >= 30)
        {
            
            gameManager.instance.playerScript.upgradeableStats.maxDashes++;
            gameManager.instance.playerScript.goldCount -= 30;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.menufeedback(gameManager.instance.itemBought, gameManager.instance.audioLevels.menuFeedBackVol);
        }
    }

    public void jumpUp()
    {
        if (gameManager.instance.playerScript.goldCount >= 45 && gameManager.instance.playerScript.upgradeableStats.maxJumps < 5)
        {
            gameManager.instance.playerScript.upgradeableStats.maxJumps++;
            gameManager.instance.playerScript.goldCount -= 45;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.menufeedback(gameManager.instance.itemBought, gameManager.instance.audioLevels.menuFeedBackVol);
        }
    }

    public void speedUp()
    {
        if (gameManager.instance.playerScript.goldCount >= 20)
        {
            gameManager.instance.playerScript.upgradeableStats.speed++;
            gameManager.instance.playerScript.goldCount -= 20;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.menufeedback(gameManager.instance.itemBought, gameManager.instance.audioLevels.menuFeedBackVol);

        }
    }

    public void save()
    {
        gameManager.instance.audioLevels.saveSettings();
        gameManager.instance.menufeedback(gameManager.instance.buttonClick,gameManager.instance.audioLevels.menuFeedBackVol);
    }
    public void Credits()
    {
        gameManager.instance.newmenu(gameManager.instance.menuCredits);
        gameManager.instance.menufeedback(gameManager.instance.buttonClick, gameManager.instance.audioLevels.menuFeedBackVol);

    }

    public void Settings()
    {
        gameManager.instance.newmenu(gameManager.instance.menuSettings);


        

        gameManager.instance.masterVol.value = gameManager.instance.audioLevels.masterVol;
        gameManager.instance.musicVol.value = gameManager.instance.audioLevels.musicVol;
        gameManager.instance.effectsVol.value = gameManager.instance.audioLevels.effectVol;
       
        gameManager.instance.menuVol.value = gameManager.instance.audioLevels.menuFeedBackVol;

        gameManager.instance.masterVolVal.text = gameManager.instance.audioLevels.masterVol.ToString();
        gameManager.instance.musicVolVal.text = gameManager.instance.audioLevels.musicVol.ToString ();
        gameManager.instance.effectsVolVal.text = gameManager.instance.audioLevels.effectVol.ToString();
        gameManager.instance.menuVolVal.text = gameManager.instance.audioLevels.menuFeedBackVol.ToString();


        /*  
          gameManager.instance.masterVolVal.text = gameManager.instance.masterVol.value.ToString();
          gameManager.instance.musicVolVal.text = gameManager.instance.musicVol.value.ToString();
          gameManager.instance.effectsVolVal.text = gameManager.instance.effectsVol.value.ToString();
          gameManager.instance.menuVolVal.text = gameManager.instance.menuVol.value.ToString();


        */
        

        gameManager.instance.menufeedback(gameManager.instance.buttonClick, gameManager.instance.audioLevels.menuFeedBackVol);
        gameManager.instance.LogMenuStack();  

    }

    public void Back()
    {
        gameManager.instance.menufeedback(gameManager.instance.buttonClick, gameManager.instance.audioLevels.menuFeedBackVol);
        gameManager.instance.menuActive.SetActive(false);
        gameManager.instance.menuLists.Pop();
        gameManager.instance.menuActive = gameManager.instance.menuLists.Peek();
        gameManager.instance .menuActive.SetActive(true);
        if(gameManager.instance.menuLists.Count == 0)
        {
            gameManager.instance.music.clip = gameManager.instance.gameMusic;
            gameManager.instance.music.Play();
        }
    }

    public void Play()
    {
        gameManager.instance.menufeedback(gameManager.instance.buttonClick, gameManager.instance.audioLevels.menuFeedBackVol);
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextIndex);
    }
   
    public void mainMenu()
    {
        gameManager.instance.menuLists.Clear();
        gameManager.instance.menuActive.SetActive(false);
        gameManager.instance.QuitToMainMenu();
    }

    public void closeGame()
    {
#if !UNITY_WEBGL
            Application.Quit();
#endif
    }

}

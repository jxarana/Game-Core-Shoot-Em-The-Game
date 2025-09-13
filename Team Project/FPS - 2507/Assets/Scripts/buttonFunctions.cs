using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
        gameManager.instance.audioSource.PlayOneShot(gameManager.instance.buttonClip, gameManager.instance.gameSettings.menuAudio);
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
        gameManager.instance.audioSource.PlayOneShot(gameManager.instance.buttonClip, gameManager.instance.gameSettings.menuAudio);
    }

    public void loadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextIndex);
        gameManager.instance.audioSource.PlayOneShot(gameManager.instance.buttonClip, gameManager.instance.gameSettings.menuAudio);
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
            gameManager.instance.audioSource.PlayOneShot(gameManager.instance.bought, gameManager.instance.gameSettings.menuAudio);
        }
    }

    public void ammoRefill()
    {
        if (gameManager.instance.playerScript.goldCount >= 50)
        {
            gameManager.instance.playerScript.replenishAmmo();
            gameManager.instance.playerScript.goldCount -= 50;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.audioSource.PlayOneShot(gameManager.instance.bought, gameManager.instance.gameSettings.menuAudio);
        }
    }//done

    public void heal25()
    {
        if(gameManager.instance.playerScript.goldCount >= 20)
        { 
            gameManager.instance.playerScript.healhp(25);
            gameManager.instance.playerScript.goldCount -= 20;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.audioSource.PlayOneShot(gameManager.instance.bought);
            gameManager.instance.audioSource.PlayOneShot(gameManager.instance.bought, gameManager.instance.gameSettings.menuAudio);
        }
    } // done
    public void heal50()
    {
        if (gameManager.instance.playerScript.goldCount >= 45)
        {
            gameManager.instance.playerScript.healhp(50);
            gameManager.instance.playerScript.goldCount -= 45;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.audioSource.PlayOneShot(gameManager.instance.bought, gameManager.instance.gameSettings.menuAudio);
        }
    } // done
    public void heal75()
    {
        if (gameManager.instance.playerScript.goldCount >= 70)
        {
            gameManager.instance.playerScript.healhp(75);
            gameManager.instance.playerScript.goldCount -= 70;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.audioSource.PlayOneShot(gameManager.instance.bought,gameManager.instance.gameSettings.menuAudio);
        }
    } //done

    

  

   

    

    public void dashUp()
    {
        if (gameManager.instance.playerScript.goldCount >= 30)
        {
            
            gameManager.instance.playerScript.upgradeableStats.maxDashes++;
            gameManager.instance.playerScript.goldCount -= 30;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.audioSource.PlayOneShot(gameManager.instance.bought, gameManager.instance.gameSettings.menuAudio);
        }
    }

    public void jumpUp()
    {
        if (gameManager.instance.playerScript.goldCount >= 45 && gameManager.instance.playerScript.upgradeableStats.maxJumps < 5)
        {
            gameManager.instance.playerScript.upgradeableStats.maxJumps++;
            gameManager.instance.playerScript.goldCount -= 45;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.audioSource.PlayOneShot(gameManager.instance.bought, gameManager.instance.gameSettings.menuAudio);
        }
    }

    public void speedUp()
    {
        if (gameManager.instance.playerScript.goldCount >= 20)
        {
            gameManager.instance.playerScript.upgradeableStats.speed++;
            gameManager.instance.playerScript.goldCount -= 20;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            gameManager.instance.audioSource.PlayOneShot(gameManager.instance.bought, gameManager.instance.gameSettings.menuAudio);

        }
    }

    



    public void displayCredits()
    {
        gameManager.instance.creditsDisplay();
        gameManager.instance.audioSource.PlayOneShot(gameManager.instance.buttonClip, gameManager.instance.gameSettings.menuAudio);
    }

    public void displayMainMenu()
    {
        gameManager.instance.displayMainMenu();
        gameManager.instance.audioSource.PlayOneShot(gameManager.instance.buttonClip, gameManager.instance.gameSettings.menuAudio);
    }

    public void displaySettings()
    {
        gameManager.instance.settingsDisplay();
        gameManager.instance.buttonSource.PlayOneShot(gameManager.instance.buttonClip, gameManager.instance.gameSettings.menuAudio);
    }

    public void exit()
    {
        Application.Quit();
    }


    public void startGame()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void save()
    {
        gameManager.instance.save();
    }
}

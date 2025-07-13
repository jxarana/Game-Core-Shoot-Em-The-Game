using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateUnpause();
    }

    public void loadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextIndex);
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
            gameManager.instance.playerScript.dmgUp++;
            gameManager.instance.playerScript.goldCount -= 80;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
        }
    }

    public void ammoRefill()
    {
        if (gameManager.instance.playerScript.goldCount >= 50)
        {
            gameManager.instance.playerScript.replenishAmmo();
            gameManager.instance.playerScript.goldCount -= 50;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
        }
    }//done

    public void heal25()
    {
        if(gameManager.instance.playerScript.goldCount >= 20)
        { 
            gameManager.instance.playerScript.healhp(25);
            gameManager.instance.playerScript.goldCount -= 20;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
        }
    } // done
    public void heal50()
    {
        if (gameManager.instance.playerScript.goldCount >= 45)
        {
            gameManager.instance.playerScript.healhp(50);
            gameManager.instance.playerScript.goldCount -= 45;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
        }
    } // done
    public void heal75()
    {
        if (gameManager.instance.playerScript.goldCount >= 70)
        {
            gameManager.instance.playerScript.healhp(75);
            gameManager.instance.playerScript.goldCount -= 70;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
        }
    } //done

    

    public void unlockDash() //done
    {
        if (gameManager.instance.playerScript.upgradePoints > 0 && !gameManager.instance.playerScript.dashReturn())
        {
            gameManager.instance.playerScript.dashUnlock();
            gameManager.instance.playerScript.upgradePoints -= 1;
        }
    }

    public void unlockGrap()
    {
        if (gameManager.instance.playerScript.upgradePoints > 0 && !gameManager.instance.playerScript.grappleReturn())
        {
            gameManager.instance.playerScript.grappleUnlock();
            gameManager.instance.playerScript.upgradePoints -= 1;
        }
    }

    public void unlockSlam()
    {
        if (gameManager.instance.playerScript.upgradePoints > 0 && !gameManager.instance.playerScript.slamReturn())
        {
            gameManager.instance.playerScript.slamUnlock();
            gameManager.instance.playerScript.upgradePoints -= 1;

        }
    }

    public void dashUp()
    {
        if (gameManager.instance.playerScript.goldCount >= 30)
        {
            gameManager.instance.playerScript.dashCountUp();
            gameManager.instance.playerScript.goldCount -= 30;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
        }
    }

    public void jumpUp()
    {
        if (gameManager.instance.playerScript.goldCount >= 45)
        {
            gameManager.instance.playerScript.jumpCountUp();
            gameManager.instance.playerScript.goldCount -= 45;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
        }
    }

    public void speedUp()
    {
        if (gameManager.instance.playerScript.goldCount >= 20)
        {
            gameManager.instance.playerScript.speedUp();
            gameManager.instance.playerScript.goldCount -= 20;
            gameManager.instance.goldCount.text = gameManager.instance.playerScript.goldCount.ToString();
            
        }
    }

   

}

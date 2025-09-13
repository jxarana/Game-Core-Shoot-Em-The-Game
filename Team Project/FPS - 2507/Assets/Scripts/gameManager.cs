using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLevelComplete;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuUnlocks;
    [SerializeField] GameObject menuTutorial;
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] AudioClip arenaClip;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject CreditsMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] public settings gameSettings;
    [SerializeField] public AudioSource buttonSource;
    [SerializeField] public AudioSource menuMusicSource;

    public AudioClip bought;
    public AudioClip menuMusic;
    public AudioClip buttonClip;
    public AudioSource audioSource;

    public TMP_Text goldCount;
    public TMP_Text unlockCount;

    public TMP_Text inMagCount;
    public TMP_Text currAmmoCount;


    [SerializeField] TMP_Text musicVal;
    [SerializeField] TMP_Text menueffectVal;
    [SerializeField] TMP_Text soundEffectVal;
    [SerializeField] public Slider music;
    [SerializeField] public Slider menuEffects;
    [SerializeField] public Slider soundEffects;




    public GameObject menuShop;
    public bool isShopScene;

    [Header("Player Display")]
    public Image ammoBar;
    public Image playerHPBar;
    public Image playerStaminaBar;
    public GameObject playerDamagePanel;

    public bool isPaused;
    public GameObject player;
    public playerController playerScript;
   

    float timeScaleOrig;
    float timeScaleNew;

    int gameGoalCount;
    int gameGoalCountOrig;
    int levelCount;

    [Header("Menu Scenes")]
    bool isMenuScene;
    string[] menuScenes = { "MainMenu", "Settings", "Credits" };
    

   /* [Header("Follower")]
  
    public GameObject followerPrefab;
    public GameObject follower;
    Transform followerSpawn;
   */

    [Header("Player")] 
    public GameObject playerPrefab;
    Transform playerSpawnPoint;

    [Header("Key")]
    public Transform keySpawnPoint;
    public GameObject keyPrefab;

    [Header("Enemy")]
    public GameObject enemyPrefab;
    public int numberOfEnemiesToSpawn = 5;

    //public bool loop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        // Detect whether we're in the shop scene
        isShopScene = SceneManager.GetActiveScene().name.Contains("Shop");

        // Dynamically find the player spawn point
        GameObject spawnPointObj = GameObject.FindWithTag("PlayerSpawn");
        if (spawnPointObj != null)
        {
            playerSpawnPoint = spawnPointObj.transform;
        }

        // Find the player at the correct spawn
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            if (playerPrefab != null && playerSpawnPoint != null)
            {
                player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
                //player.tag = "Player";
            }
        }

        // Grab the player script and enable movement
        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
            playerScript.enabled = true;
            timeScaleOrig = Time.timeScale;
        }
       
        if (!isShopScene)
        {
            gameGoalCount = numberOfEnemiesToSpawn;
            gameGoalCountOrig = gameGoalCount;
            SpawnEnemies();
        }

       
    }

    private void Start()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        bool isMenuScene = System.Array.Exists(menuScenes, scene => scene == currentScene);

        if (isMenuScene)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            ammoBar.enabled = false;
            playerHPBar.enabled = false;
            playerStaminaBar.enabled = false;
            
            keyPrefab = null;
            keySpawnPoint = null;
            playerPrefab = null;
            gameGoalCountText.enabled = false;

            if(currentScene == "MainMenu")
            {
                menuActive = MainMenu;
                menuActive.SetActive(true);
            }
            
        }
        else
        {
            menuActive = null;
            menuActive.SetActive(false);
            ammoBar.enabled = true;
            playerHPBar.enabled = true;
            playerStaminaBar.enabled = true;
        }

        menuMusicSource.volume = gameSettings.musicAudio;
        buttonSource.volume = gameSettings.menuAudio;


            Time.timeScale = 1f;
        gameGoalCount += numberOfEnemiesToSpawn;
        gameGoalCountOrig = gameGoalCount;
        if (!menuActive)
        {
            playAudio(arenaClip, transform, 0.1f, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause || menuActive == menuShop)
            {
                stateUnpause();
            }
        }

        menueffectVal.text = menuEffects.value.ToString();
        musicVal.text = music.value.ToString();
        soundEffectVal.text = soundEffects.value.ToString();

        if (menuActive != null)
        {
            
            menuMusicSource.clip = menuMusic;
            menuMusicSource.Play();
        }



    }

    void SpawnEnemies()
    {
        // Exit early if no enemy prefab is assigned
        if (enemyPrefab == null) return;

        // Find all spawn points in the scene tagged "EnemySpawn"
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("EnemySpawn");
        if (spawnObjects.Length == 0) return;

        // Create a pool of available spawn points to randomize selection without
        List<GameObject> availableSpawns = new List<GameObject>(spawnObjects);

        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            if (availableSpawns.Count == 0)
            {
                // If we've used up all spawn points, reset the pool so spawns can repeat
                availableSpawns = new List<GameObject>(spawnObjects);
            }


            // Select a random spawn point from the available list
            int randomIndex = Random.Range(0, availableSpawns.Count);
            GameObject spawnPoint = availableSpawns[randomIndex];

            // Instantiate the enemy at the chosen spawn location
            Instantiate(enemyPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            // Remove this spawn point to avoid duplicate use until reset
            availableSpawns.RemoveAt(randomIndex);

        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        timeScaleNew = Time.timeScale;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void displayMainMenu()
    {
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            menuActive = MainMenu;
            menuActive.SetActive(true);
        }
        else
            SceneManager.LoadScene("MainMenu");
    }

    public void creditsDisplay()
    {
        menuActive = CreditsMenu;
        menuActive.SetActive(true);
    }

    public void settingsDisplay()
    {
        music.value = gameSettings.musicAudio;
        menuEffects.value = gameSettings.menuAudio;
        soundEffects.value = gameSettings.effectsAudio;
        menuActive = settingsMenu;
        menuActive.SetActive(true);
    }

    public void save()
    {
        gameSettings.effectsAudio = (int)soundEffects.value;
        gameSettings.menuAudio = (int)menuEffects.value;
        gameSettings.musicAudio = (int)music.value;
        menuMusicSource.volume = music.value;
        buttonSource.volume = menuEffects.value;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameGoalCountText.text = gameGoalCount.ToString("F0");
        /*
         if(gameGoalCount <= 0 && levelCount == x)

           statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
         
         
         
         */

        if (gameGoalCount <= 0 && !isShopScene)
        {
            // you win!
            statePause();
            int currentIndex = SceneManager.GetActiveScene().buildIndex;
            int finalIndex = SceneManager.sceneCountInBuildSettings - 1;

            if (currentIndex >= finalIndex)
            {
                // End of last level (show true win menu)
                menuActive = menuWin;
            }
            else
            {
                // Intermediate level completed (show level complete screen)
                menuActive = menuLevelComplete;
            }

            menuActive.SetActive(true);
        }

        /*
         *  statePause();   
            menuActive = menuUlocks;
            menuActive.setActive(true);




         */



    }

    public void tutorial()
    {
        statePause();
        menuActive = menuTutorial;
        menuActive.SetActive(true);
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void levelComplete()
    {
        statePause();
        menuActive = menuLevelComplete;
        menuActive.SetActive(true);
    }

    public void openShop()
    {
        statePause();
        goldCount.text = playerScript.goldCount.ToString();
        unlockCount.text = playerScript.upgradePoints.ToString();
        menuActive = menuShop;
        menuActive.SetActive(true);
    }

    public void playAudio(AudioClip clipAudio, Transform transform, float volume, bool loops)
    {
        
        if (!menuActive)
        {
            if (loops == true)
            {
                audioSource.clip = clipAudio;

                // assigning the volume
                audioSource.volume = volume;

                audioSource.loop = loops;

                // plays sound
                audioSource.Play();

                //assigns length of audio
                float clipDuration = audioSource.clip.length;
            }

            else if (loops == false)
            {
                audioSource.clip = clipAudio;

                // assigning the volume
                audioSource.volume = volume;

                audioSource.loop = loops;

                // plays sound
                audioSource.Play();

                // assigns length of audio
                float clipDuration = audioSource.clip.length;
            }
        }
    }
}

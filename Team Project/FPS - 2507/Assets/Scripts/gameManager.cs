using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;
    [SerializeField] public GameObject menuActive;
    [SerializeField] public GameObject menuPause;
    [SerializeField] GameObject menuLevelComplete;
    [SerializeField] public GameObject menuWin;
    [SerializeField] public GameObject menuLose;
    [SerializeField] GameObject menuUnlocks;
    [SerializeField] public GameObject menuMain;
    [SerializeField] public GameObject menuTutorial;
    [SerializeField] public GameObject menuCredits;
    [SerializeField] public GameObject menuSettings;
    [SerializeField] public GameObject menuBackground;

    public Stack<GameObject> menuLists;

    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] GameObject[] toughEnemies;
    //[SerializeField] AudioClip arenaClip;

    //public AudioSource audioSource;

    public TMP_Text goldCount;
    public TMP_Text goldCountUI;
    public TMP_Text unlockCount;

    public TMP_Text inMagCount;
    public TMP_Text currAmmoCount;

    [Header("Audio")]
    [SerializeField] public AudioSource menuFeedBack;
    [SerializeField] public AudioSource music;
    [SerializeField] public AudioClip buttonClick;
    [SerializeField] public AudioClip itemBought;
    [SerializeField] public AudioClip menuMusic;
    [SerializeField] public AudioClip gameMusic;







    [Header("Settings")]
    [SerializeField] public Slider masterVol;
    [SerializeField] public Slider musicVol;
    [SerializeField] public Slider menuVol;
    [SerializeField] public Slider effectsVol;
    [SerializeField] public TMP_Text masterVolVal;
    [SerializeField] public TMP_Text musicVolVal;
    [SerializeField] public TMP_Text menuVolVal;
    [SerializeField] public TMP_Text effectsVolVal;




    public gameSettings audioLevels;




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

    private int savedGold, savedUpgrades, savedAmmo, savedMag;
    private float savedHP;
    private List<gunStats> savedGuns = new();

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

        menuLists = new Stack<GameObject>();

        music.loop = true;

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

        loadPlayerState();

        if (!isShopScene)
        {
            gameGoalCount = numberOfEnemiesToSpawn + toughEnemies.Count();
            gameGoalCountOrig = gameGoalCount;
            SpawnEnemies();
        }
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            music.clip = menuMusic;
            ammoBar.enabled = false;
            playerHPBar.enabled = false;
            gameGoalCountText.enabled = false;
            playerStaminaBar.enabled = false;
            goldCount.enabled = false;

            menuLists.Push(menuMain);
            menuActive = menuLists.Peek();

            menuActive.SetActive(true);
            menuBackground.SetActive(true);
        }
        else
        {
            music.clip = gameMusic;
            ammoBar.enabled = true;
            playerHPBar.enabled = true;
            gameGoalCountText.enabled = true;
            playerStaminaBar.enabled = true;
            goldCount.enabled = true;
            menuBackground.SetActive(false);
            goldCount.text = playerScript.upgradeableStats.dollarBills.ToString();
        }

        music.Play();

        //playAudio(arenaClip, transform, 0.1f/*, false*/);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && SceneManager.GetActiveScene().name != "MainMenu")
        {


            if (menuActive == null)
            {
                music.clip = menuMusic;
                statePause();
                menuLists.Push(menuPause);
                menuActive = menuLists.Peek();
                menuActive.SetActive(true);
            }
            else if (menuLists.Count > 0)
            {
                menuLists.Pop();
                if (menuLists.Count == 0)
                {
                    stateUnpause();
                    music.clip = gameMusic;
                }
            }

        }

        if (menuActive == menuSettings)
        {
            float masterHolder = masterVol.value * 100;
            float musicHolder = musicVol.value * 100;
            float effectHolder = effectsVol.value * 100;
            float menuHolder = menuVol.value * 100;


            masterVolVal.text = masterHolder.ToString("F0");
            musicVolVal.text = musicHolder.ToString("F0");
            effectsVolVal.text = effectHolder.ToString("F0");
            menuVolVal.text = menuHolder.ToString("F0");

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
        playerScript.StopSprinting();
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        music.clip = gameMusic;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
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
            savePlayerState();

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

    public void savePlayerState()
    {
        if (playerScript == null) return;
        playerScript.savePlayerData(out savedGold, out savedUpgrades, out savedHP, out savedAmmo, out savedMag);
    }

    public void loadPlayerState()
    {
        if (playerScript == null) return;
        playerScript.loadPlayerData(savedGold, savedUpgrades, savedHP, savedAmmo, savedMag);
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
        goldCount.text = playerScript.upgradeableStats.dollarBills.ToString();
        unlockCount.text = playerScript.upgradePoints.ToString();
        menuActive = menuShop;
        menuActive.SetActive(true);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        isPaused = false;
        menuActive = null;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        menuActive = null;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }

    public void menufeedback(AudioClip audio, float volume)
    {
        menuFeedBack.PlayOneShot(audio, volume);
    }

    //public void playAudio(AudioClip clipAudio, Transform transform, float volume/*, bool loops*/)
    //{

    //    if (!menuActive)
    //    {
    //        //if (loops == true)
    //        //{
    //        audioSource.clip = clipAudio;

    //        // assigning the volume
    //        audioSource.volume = volume;

    //        //audioSource.loop = true;

    //        // plays sound
    //        audioSource.Play();

    //        //assigns length of audio
    //        float clipDuration = audioSource.clip.length;

    //        Destroy(gameObject, clipDuration);
    //        //}

    //        //else if (loops == false)
    //        //{
    //        //    audioSource.clip = clipAudio;

    //        //    // assigning the volume
    //        //    audioSource.volume = volume;

    //        //    audioSource.loop = loops;

    //        //    // plays sound
    //        //    audioSource.Play();

    //        //    // assigns length of audio
    //        //    float clipDuration = audioSource.clip.length;
    //        //}
    //    }
    //}
    public void LogMenuStack()
    {
        Debug.Log("=== MENU STACK CONTENTS ===");
        int count = 0;
        foreach (var menu in menuLists)
        {
            Debug.Log($"[{count}] {menu.name}");
            count++;
        }
        Debug.Log("=== END ===");
    }

    public void newmenu(GameObject newMenu)
    {
        if (menuActive != null)
            menuActive.SetActive(false);

        menuLists.Push(newMenu);
        menuActive = newMenu;
        menuActive.SetActive(true);

        LogMenuStack();
    }
}

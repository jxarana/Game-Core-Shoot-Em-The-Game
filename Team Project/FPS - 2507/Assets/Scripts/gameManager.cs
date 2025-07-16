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
    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] AudioClip arenaClip;

    [SerializeField] private AudioSource audioObject;

    public TMP_Text goldCount;
    public TMP_Text unlockCount;

    public TMP_Text inMagCount;
    public TMP_Text currAmmoCount;




    public GameObject menuShop;
    public bool isShopScene;

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


    // Spawn point for the player
    Transform playerSpawnPoint;
    public GameObject playerPrefab;

    // Spawn point for the key
    public Transform keySpawnPoint;
    public GameObject keyPrefab;

    // Randomized spawn for the enemy
    public GameObject enemyPrefab;
    public int numberOfEnemiesToSpawn = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        // Detect whethere we're in the shop scene
        isShopScene = SceneManager.GetActiveScene().name.Contains("Shop");

        // Dynamically find the player spawn point
        GameObject spawnPointObj = GameObject.FindWithTag("PlayerSpawn");
        if (spawnPointObj != null)
        {
            playerSpawnPoint = spawnPointObj.transform;
        }

        // Find the player at the correct spawn
        player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            if (playerPrefab != null && playerSpawnPoint != null)
            {
                player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
                player.tag = "Player";
            }
        }

        // Grab the player script and enable movement
        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
            playerScript.enabled = true;
            timeScaleOrig = Time.timeScale;
        }
        if (!menuActive)
        {
            gameManager.instance.playAudio(arenaClip, transform, 0.1f);
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
        Time.timeScale = 1f;
        gameGoalCount = numberOfEnemiesToSpawn;
        gameGoalCountOrig = gameGoalCount;
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
        if (gameGoalCount < gameGoalCountOrig && !isShopScene)
        {
            spawnKey();
        }

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

    public void playAudio(AudioClip gunAudio, Transform transform, float volume)
    {
        // spawns game object
        AudioSource audioSource = Instantiate(audioObject, transform.position, Quaternion.identity);

        // assigns audio clip
        audioSource.clip = gunAudio;

        // assigning the volume
        audioSource.volume = volume;

        // plays sound
        audioSource.Play();

        // assigns length of audio
        float clipDuration = audioSource.clip.length;

        // destroy object after it is finished playing
        Destroy(audioSource.gameObject, clipDuration);
    }

    public void spawnKey()
    {
        if (keyPrefab == null) return;

        // Find all spawn points in the scene tagged "KeySpawn"
        GameObject spawnObjects = GameObject.FindGameObjectWithTag("KeySpawn");

        Instantiate(keyPrefab, keySpawnPoint.transform.position, keySpawnPoint.transform.rotation);
    }
}

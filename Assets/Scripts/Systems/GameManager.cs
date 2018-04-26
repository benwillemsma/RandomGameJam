using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Static Variables
    public static GameManager Instance;
    public static AudioDictonary AudioManager;
    public static PlayerData player;

    private string m_mainMenuScene = "MainMenu";
    private string m_playScene = "PlayScene";

    public Canvas m_pauseScreen;

    public float objectDestroyDelay = 10;

    private static bool m_gameOver = false;
    public bool GameOver
    {
        get { return m_gameOver; }
        set { m_gameOver = value; }
    }
    private static bool m_paused = false;
    public bool IsPaused
    {
        get { return m_paused; }
        set { m_paused = value; }
    }
    #endregion

    #region Pause Functions
    public void OnApplicationFocus(bool focus)
    {
        if (this != Instance)
            return;

        if (SceneManager.GetActiveScene().buildIndex > 5)
        {
            if (!focus && !m_paused && !Application.isEditor)
                TogglePause();
        }
    }

    public void TogglePause()
    {
        if (this != Instance)
            return;

        m_paused = !m_paused;
        m_pauseScreen.gameObject.SetActive(m_paused);
        Time.timeScale = m_paused ? 0 : 1;
        ToggleCursor(m_paused);
        ToggleStateMachinesPause();

    }

    public void TogglePause(bool pause)
    {
        if (this != Instance)
            return;

        m_paused = pause;
        m_pauseScreen.gameObject.SetActive(pause);
        Time.timeScale = pause ? 0 : 1;
        ToggleCursor(pause);
        ToggleStateMachinesPause();
    }

    public void ToggleCursor(bool value)
    {
        if (value)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ToggleStateMachinesPause()
    {
        StateManager[] managers = FindObjectsOfType<StateManager>();
        for (int i = 0; i < managers.Length; i++)
        {
            managers[i].IsPaused = m_paused;
        }
    }
    #endregion

    #region Player Managment
    public GameObject playerPrefab;

    private void SpawnPlayer()
    {
        PlayerData playerRef = playerPrefab.GetComponent<PlayerData>();
        playerRef = Instantiate(playerRef, playerRef.SpawnPoint, playerRef.spawn ? playerRef.spawn.rotation : Quaternion.identity);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion

    #region Scene Managment
    public void MainMenu()
    {
        SceneManager.LoadScene(m_mainMenuScene);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void Continue()
    {
        int sceneIndex = PlayerPrefs.GetInt("ContinueScene");
        LoadScene(sceneIndex);
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void AddScene(string name)
    {
        Debug.Log("GameManager:AddScene (" + name + ")", this);
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
    }
    public void AddScene(int index)
    {
        SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
    }

    public void UnloadScene(string name)
    {
        if(SceneManager.GetSceneByName(name).isLoaded)
            SceneManager.UnloadSceneAsync(name);
    }
    public void UnloadScene(int index)
    {
        if (SceneManager.GetSceneAt(index).isLoaded)
            SceneManager.UnloadSceneAsync(index);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this != Instance)
            return;

        else if (scene.name == m_mainMenuScene)
        {
            TogglePause(false);
            ToggleCursor(true);
            Time.timeScale = 1;
            if (player) Destroy(player.gameObject);
        }

        else if (scene.name == m_playScene)
        {
            if (!player) SpawnPlayer();
            ToggleCursor(false);
        }

        else
        {
            //Scene is LevelScene
            string currentLevel = scene.name;
            PlayerPrefs.SetString("ContinueScene", currentLevel);
            //SceneManager.SetActiveScene(SceneManager.GetSceneByName("PlayScene"));
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
    }
    #endregion

    #region Static Functions
    public delegate void Callback();
    public static IEnumerator CallAfterDelay(Callback callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (callback != null) callback();
    }
    #endregion

    #region Main

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            StopAllCoroutines();
            DestroyImmediate(gameObject);
        }
    }

    private void Update()
    {
        if (this != Instance)
            return;

        if (Input.GetButtonDown("Pause") && SceneManager.GetActiveScene().name != m_mainMenuScene)
            TogglePause();

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            Time.timeScale++;
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            Time.timeScale--;
        Mathf.Clamp(Time.timeScale, 0, 10);
    }
    #endregion
}

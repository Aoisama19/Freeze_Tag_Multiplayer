using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Gameplay")]
    [SerializeField] private int _totalRunners = 4;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _runnerPrefab;
    [SerializeField] private GameObject _catcherPrefab;
    [SerializeField] private GameObject _powerupsParent;
    [SerializeField] private bl_MiniMap _miniMap;
    [SerializeField] private List<GameObject> _characters;
    [SerializeField] private List<GameObject> _spawnPoints;

    [Header("NPCS")]
    [SerializeField] private GameObject[] _npc;
    [SerializeField] private List<GameObject> _npcSpawnPoints;

    [Header("Game Pause")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private GameObject _pauseMenu;
    private InputAction _pauseBtn;
    private bool _isPaused;

    [Header("Game Win/Lose")]
    [SerializeField] private GameObject _winMenu;
    [SerializeField] private GameObject _loseMenu;
    private bool _isEnded;

    [Header("Current Status Of Game")]
    [SerializeField] private string _playerRole;
    [SerializeField] private int _runnersLeft;
    [SerializeField] private GameObject _catcher;
    [SerializeField] private List<GameObject> _runners;

    private string _playerName;
    private string _playerType;
    
    private void Awake() 
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }
        Cursor.visible = false;
        // Read player role and name
        _playerType = PlayerPrefs.GetString("PlayerType");
        _playerName = PlayerPrefs.GetString("Character");

    }

    private void OnEnable()
    {
        Actions.GameStart += GameStart;
        Actions.GameStop += GameEnd;
        // Pause Menu
        _pauseBtn = _playerInput.actions["PauseMenu"];
        _pauseBtn.Enable();
        _pauseBtn.performed += ShowPauseMenu;
        // Gameplay
        Actions.Baraf += Baraf;
        Actions.Paani += Paani;
    }

    private void OnDisable()
    {
        Actions.GameStart -= GameStart;
        Actions.GameStop -= GameEnd;
        // Pause Menu
        _pauseBtn.Disable();
        // Gameplay
        Actions.Baraf -= Baraf;
        Actions.Paani -= Paani;
    }

    private void Start()
    {
        // Pause Menu
        _pauseBtn = _playerInput.actions["PauseMenu"];
        _isPaused = false;
        // Gameplay
        _playerRole = _playerType;
        _runnersLeft = _totalRunners;
        InitializeNPCS();
        InitializePlayers();
    }

    private void InitializeNPCS()
    {
        int totalNPCS = _npc.Length;
        int i = 0;

        GameObject parent = new GameObject("NPCS");

        foreach (GameObject sp in _npcSpawnPoints)
        {
            Instantiate(_npc[i++ %  totalNPCS], sp.transform.position, Quaternion.identity, parent.transform);
        }
    }

    private void InitializePlayers()
    {
        Debug.Log("Player Name = " + _playerName);
        Debug.Log("Player Role = " + _playerType);
        Debug.Log("Player is at index = " + _characters.FindIndex(item => item.name == _playerName));

        int runnerCount = 0;
        int itemIndex = _characters.FindIndex(item => item.name == _playerName);
        bool catcherSelected = false;
        GameObject parent = new GameObject("Runners");

        if (itemIndex < 0) { itemIndex = 0; }

        // Creating player
        Transform loc = GetSpawnLocation();
        Vector3 pos = loc.position;
        pos.y += 0.92f;
        GameObject player = Instantiate(_playerPrefab, pos, loc.rotation);
        GameObject playerArmature = player.transform.GetChild(2).gameObject;
        Instantiate(_characters[itemIndex], playerArmature.transform.Find("Geometry"));  // Putting character under geometry
        _characters.RemoveAt(itemIndex);    // Removing the character
        _miniMap.Target = playerArmature.transform;
        playerArmature.transform.Find("Geometry").gameObject.AddComponent<DissolvingController>();

        if (_playerType == "Chaser")
        {
            // Player is chaser
            playerArmature.layer = LayerMask.NameToLayer("Chaser");
            playerArmature.tag = "Chaser";
            catcherSelected = true;
            _catcher = player;
            _miniMap.MiniMapLayer = 11;
            _miniMap.playerColor = Color.red;
        }
        else if (_playerType == "Runner")
        {
            // Player is runner
            playerArmature.layer = LayerMask.NameToLayer("Runner");
            playerArmature.tag = "Runner";
            runnerCount++;
            _miniMap.MiniMapLayer = 12;
            _runners.Add(player);
            _miniMap.playerColor = Color.blue;
        }

        playerArmature.AddComponent<AnimationRebinder>();

        // Creating AI bots
        while (runnerCount < _totalRunners)
        {
            loc = GetSpawnLocation();
            int randomIndex = UnityEngine.Random.Range(0, _characters.Count);
            GameObject bot = Instantiate(_runnerPrefab, loc.position, loc.rotation, parent.transform);
            Instantiate(_characters[randomIndex], bot.transform);
            _characters.RemoveAt(randomIndex);
            ++runnerCount;
            _runners.Add(bot);
            bot.AddComponent<AnimationRebinder>();
            bot.AddComponent<DissolvingController>();
            bot.GetComponent<bl_MiniMapEntity>().IconColor = Color.blue;
        }

        if (!catcherSelected)
        {
            loc = GetSpawnLocation();
            int randomIndex = UnityEngine.Random.Range(0, _characters.Count);
            GameObject bot = Instantiate(_catcherPrefab, loc.position, loc.rotation);
            Instantiate(_characters[randomIndex], bot.transform);
            _characters.RemoveAt(randomIndex);
            catcherSelected = true;
            _catcher = bot;
            bot.AddComponent<AnimationRebinder>();
            bot.AddComponent<DissolvingController>();
            bot.GetComponent<bl_MiniMapEntity>().IconColor = Color.red;
        }

    }

    private Transform GetSpawnLocation()
    {
        Transform spawnLocation = null;

        if (_spawnPoints.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, _spawnPoints.Count);
            spawnLocation = _spawnPoints[randomIndex].transform;
            _spawnPoints.RemoveAt(randomIndex);
        }

        return spawnLocation;
    }

    private void GameStart()
    {
        
    }

    private void GameEnd()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        if ((_playerRole == "Runner" && _runnersLeft > 0) || (_playerRole == "Chaser" && _runnersLeft <= 0))
        {
            GoToWinScreen();
        }
        else if ((_playerRole == "Runner" && _runnersLeft <= 0) || (_playerRole == "Chaser" && _runnersLeft > 0))
        {
            GoToLoseScreen();
        }
    }

    private void ShowPauseMenu(InputAction.CallbackContext context)
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Cursor.visible = true;
            Camera.main.GetComponent<CinemachineBrain>().enabled = false;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            _pauseMenu.SetActive(true);
        }
        else
        {
            Cursor.visible = false;
            HidePauseMenu();
        }
    }

    public void GoToWinScreen()
    {
        Camera.main.GetComponent<CinemachineBrain>().enabled = false;
        Time.timeScale = 0f;
        _winMenu.SetActive(true);
    }

    public void GoToLoseScreen()
    {
        Camera.main.GetComponent<CinemachineBrain>().enabled = false;
        Time.timeScale = 0f;
        _loseMenu.SetActive(true);
    }

    public void Baraf()
    {
        _runnersLeft -= 1;
        if (_runnersLeft <= 0)
        {
            if (_playerRole == "Chaser")
                GoToWinScreen();
            else
                GoToLoseScreen();
        }
    }

    public void Paani()
    {
        _runnersLeft += 1;
    }

    public void HidePauseMenu()
    {
        Camera.main.GetComponent<CinemachineBrain>().enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        _isPaused = false;
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        _isPaused = false;
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        SceneController.instance.MainMenu();
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void RestartLevel()
    {
        _loseMenu.SetActive(false);
        Time.timeScale = 1f;
        SceneController.instance.RestartScene();
    }
    
}

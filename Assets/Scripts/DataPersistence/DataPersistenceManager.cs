using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;
    
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private List<IDataPersistence> dataPersistenceObjects;
    private GameData gameData;
    public static DataPersistenceManager instance { get; private set; }
    private FileDataHandler dataHandler;

    private void Awake() {
        if (instance != null) {
            Debug.LogError("Found more than one Data Persistence Manager in the scene. Destroying the new one");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene) {
        SaveGame();
    }

    public void NewGame() {
        this.gameData = new GameData();
    }
    
    public void LoadGame() {
        this.gameData = dataHandler.Load();

        if (this.gameData == null && initializeDataIfNull) {
            NewGame();
        }

        // if no data can be loaded, initialize to a new game
        if (this.gameData == null) {
            Debug.LogWarning("No data was found. Please start a new game before data can be loaded.");
            return;
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame() {
        if (this.gameData == null) {
            Debug.LogWarning("No data was found. Please start a new game before data can be loaded.");
            return;
        }
        
        // pass the data to other scripts to update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private void OnApplicationQuit() {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData() {
        return gameData != null;
    }
}

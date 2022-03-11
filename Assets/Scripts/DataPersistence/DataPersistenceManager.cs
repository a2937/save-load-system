using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private IDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

    private void Awake() 
    {
        if (instance != null && instance != this) 
        {
            Destroy(this.gameObject);
            //Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
       
        //SceneManager.sceneLoaded;
        //SceneManager.sceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= RememberNewScene;
    }

    private void Start() 
    {
        if (instance == this)
        {
            this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
            this.dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
            SceneManager.sceneLoaded += RememberNewScene;
        }
    }

    public void NewGame() 
    {
        this.gameData = new GameData();
    }

    public void RememberNewScene(Scene newScene,LoadSceneMode mode)
    {
        this.gameData.LastScene = newScene.name;
        Debug.Log("Saved scene Name: " + this.gameData.LastScene);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void LoadGame()
    {
        // load any saved data from a file using the data handler
        this.gameData = dataHandler.Load();
        
        // if no data can be loaded, initialize to a new game
        if (this.gameData == null) 
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }
        else
        {
            if(this.gameData.LastScene != SceneManager.GetActiveScene().name
                   && this.gameData.LastScene.Trim() != "")
            {
                Debug.Log("Remembered scene Name: " + this.gameData.LastScene);
                SceneManager.LoadScene(this.gameData.LastScene);
            }
        }
        // push the loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects) 
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        // pass the data to other scripts so they can update it
        ReadSceneData();
        gameData.LastScene = SceneManager.GetActiveScene().name;
        // save that data to a file using the data handler
        dataHandler.Save(gameData);
    }

    /// <summary>
    /// Passes data from the scene about to be unloaded
    /// into this one so the values don't get lost. 
    /// </summary>
    /// <param name="scene">The currently loaded scene</param>
    public void ReadSceneData()
    {
        // pass the data to other scripts so they can update it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        } 
    }

    private void OnApplicationQuit() 
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() 
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}

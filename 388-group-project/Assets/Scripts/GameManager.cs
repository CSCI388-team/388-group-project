using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Scene lists
    public List<string> puzzleScenes = new List<string> { "Puzzle1", "Puzzle2", "Puzzle3" };
    public string startScene = "StartScene";
    public string trophyRoomScene = "TrophyRoom";
    public string masterScene = "Master";

    private List<string> currentPuzzleOrder = new List<string>();
    private HashSet<string> completedPuzzles = new HashSet<string>();
    private string currentLoadedScene = "";

    private void Awake()
    {
        Debug.Log("GameManager.Awake() Called");

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ResetPuzzleOrder();

            if (SceneManager.GetActiveScene().name != masterScene)
            {
                SceneManager.LoadScene(masterScene);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadSceneAdditive(startScene);
    }

    #region Scene Management
    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        if (!string.IsNullOrEmpty(currentLoadedScene))
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentLoadedScene);
            while (!unloadOp.isDone)
            {
                yield return null;
            }
        }

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = true;

        while (!loadOp.isDone)
        {
            yield return null;
        }

        currentLoadedScene = sceneName;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }

    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }
    #endregion

    #region Puzzle Management
    private void ResetPuzzleOrder()
    {
        Debug.Log("ResetPuzzleOrder() Called");
        currentPuzzleOrder = new List<string>(puzzleScenes);
    }

    public void RandomizePuzzleOrder()
    {
        Debug.Log("RandomizePuzzleOrder() Called");
        currentPuzzleOrder = new List<string>(puzzleScenes);

        for (int i = currentPuzzleOrder.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = currentPuzzleOrder[i];
            currentPuzzleOrder[i] = currentPuzzleOrder[j];
            currentPuzzleOrder[j] = temp;
        }
    }

    public string GetNextPuzzleScene()
    {
        Debug.Log("GetNextPuzzleScene() Called");
        foreach (string puzzleScene in currentPuzzleOrder)
        {
            if (!completedPuzzles.Contains(puzzleScene))
            {
                return puzzleScene;
            }
        }
        return trophyRoomScene;
    }

    public void CompletePuzzle(string sceneName)
    {
        Debug.Log("CompletePuzzle() Called");
        if (puzzleScenes.Contains(sceneName))
        {
            completedPuzzles.Add(sceneName);
            if (completedPuzzles.Count == puzzleScenes.Count)
            {
                LoadTrophyRoom();
            }
        }
    }
    #endregion

    #region Public Interface
    public void LoadNextPuzzle()
    {
        Debug.Log("LoadNextPuzzle() Called");
        LoadSceneAdditive(GetNextPuzzleScene());
    }

    public void LoadTrophyRoom()
    {
        Debug.Log("LoadTrophyRoom() Called");
        LoadSceneAdditive(trophyRoomScene);
    }

    public void ReturnToStart()
    {
        Debug.Log("ReturnToStart");
        LoadSceneAdditive(startScene);
    }

    public void ExitGame()
    {
        Debug.Log("ExitGame() Called");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    public bool AllPuzzlesCompleted()
    {
        Debug.Log("AllPuzzleCompleted() Log");
        return completedPuzzles.Count == puzzleScenes.Count;
    }
    #endregion
}
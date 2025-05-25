using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Static class GameManager to allow for access anywhere in the game.
    // Public getter methids and private setter methods.
    public static GameManager Instance { get; private set; }

    // List of puzzle scene names
    public List<string> puzzleScenes = new List<string> { "Puzzle1", "Puzzle2", "Puzzle3" };

    // Current order of puzzles
    private List<string> currentPuzzleOrder = new List<string>();

    // Track completed puzzles (Hashset since that has O(1) lookup time)
    private HashSet<string> completedPuzzles = new HashSet<string>();

    // Start scene name
    public string startScene = "StartScene";

    // Trophy room scene name
    public string trophyRoomScene = "TrophyRoom";

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null) 
        { 
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize puzzle order
            ResetPuzzleOrder();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Reset puzzle order to default
    private void ResetPuzzleOrder()
    {
        currentPuzzleOrder = new List<string>(puzzleScenes);
    }

    // Randomize the puzzle order
    public void RandomizePuzzleOrder()
    {
        currentPuzzleOrder = new List<string>(puzzleScenes);

        // Shuffle order of scenes
        for (int i = currentPuzzleOrder.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = currentPuzzleOrder[i];
            currentPuzzleOrder[i] = currentPuzzleOrder[j];
            currentPuzzleOrder[j] = temp;
        }
    }

    // Get the next puzzle scene
    public string GetNextPuzzleScene()
    {
        foreach (string puzzleScene in currentPuzzleOrder)
        {
            if (!completedPuzzles.Contains(puzzleScene))
            {
                return puzzleScene;
            }
        }

        // All puzzles completed
        return trophyRoomScene;
    }

    // Mark a puzzle as completed
    public void CompletePuzzle(string sceneName)
    {
        if (puzzleScenes.Contains(sceneName))
        {
            completedPuzzles.Add(sceneName);

            // Check if all puzzles are completed
            if (completedPuzzles.Count == puzzleScenes.Count)
            {
                LoadTrophyRoom();
            }
        }
    }

    // Load the next puzzle
    public void LoadNextPuzzle()
    {
        string nextScene = GetNextPuzzleScene();
        SceneManager.LoadScene(nextScene);
    }

    // Load the trophy room
    public void LoadTrophyRoom()
    {
        SceneManager.LoadScene(trophyRoomScene);
    }

    // Return to start scene
    public void ReturnToStart()
    {
        SceneManager.LoadScene(startScene);
    }

    // Exit the game
    public void ExitGame()
    {
        // Preproccer directive allows for different behaviour depending on
        // where the game is being run.
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Check if all puzzles are completed
    public bool AllPuzzlesCompleted()
    {
        return completedPuzzles.Count == puzzleScenes.Count;
    }
}

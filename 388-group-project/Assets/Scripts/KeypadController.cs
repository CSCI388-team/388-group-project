using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class KeypadController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private string correctCode = "1234"; // Easily altered
    [SerializeField] private float buttonPressDuration = 0.2f;
    [SerializeField] private Color wrongCodeColor = Color.red;
    [SerializeField] private float wrongCodeFlashDuration = 1f;

    private string currentInput = "";
    private Vector3[] originalButtonScales;
    private Coroutine wrongCodeRoutine;
    private Color originalDisplayColor;

    private void Start()
    {
        // Store original button scales for press animation
        originalButtonScales = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            originalButtonScales[i] = transform.GetChild(i).localScale;
        }

        originalDisplayColor = displayText.color;
        UpdateDisplay();
    }

    public void AddDigit(string digit)
    {
        if(currentInput.Length < 4) // Limit to 4 digits
        {
            currentInput += digit;
            UpdateDisplay();
            StartCoroutine(AnimateButtonPress(digit));
        }
    }

    public void ClearInput()
    {
        currentInput = "";
        UpdateDisplay();
        StartCoroutine(AnimateButtonPress("Clear"));
    }

    public void SubmitCode()
    {
        StartCoroutine(AnimateButtonPress("Submit"));

        if (currentInput == correctCode)
        {
            // Correct code
            if (GameManager.Instance != null)
            {
                // Mark puzzle as completed
                GameManager.Instance.CompletePuzzle(SceneManager.GetActiveScene().name);

                // Load next puzzle
                GameManager.Instance.LoadNextPuzzle();
            }
            else
            {
                Debug.LogError("GameManager instance not found!");
            }
        }
        else
        {
            // Wrong code
            if (wrongCodeRoutine != null)
            {
                StopCoroutine(wrongCodeRoutine);
            }
            wrongCodeRoutine = StartCoroutine(WrongCodeFeedback());
        }
    }

    private void UpdateDisplay()
    {
        displayText.text = string.IsNullOrEmpty(currentInput) ? "****" : currentInput;
    }

    private IEnumerator AnimateButtonPress(string buttonIdentifier)
    {
        Transform button = null;

        // Find the button that was pressed
        foreach (Transform child in transform)
        {
            if (child.name.Contains(buttonIdentifier))
            {
                button = child;
                break;
            }
        }

        if (button != null)
        {
            // Scale down for press effect
            button.localScale = originalButtonScales[button.GetSiblingIndex()] * 0.8f;

            // Wait for the press duration
            yield return new WaitForSeconds(buttonPressDuration);

            // Scale back up
            button.localScale = originalButtonScales[button.GetSiblingIndex()];
        }
    }

    private IEnumerator WrongCodeFeedback()
    {
        // Flash red color
        displayText.color = wrongCodeColor;
        yield return new WaitForSeconds(wrongCodeFlashDuration);
        displayText.color = originalDisplayColor;

        // Clear input after feedback
        currentInput = "";
        UpdateDisplay();

        wrongCodeRoutine = null;
    }
}

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;

public class RandomiseButtonInteractable : MonoBehaviour
{
    private XRSimpleInteractable simpleInteractable;

    private void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
        simpleInteractable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Call LoadNextPuzzle when grabbed
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RandomizePuzzleOrder();
        }
        else
        {
            Debug.LogError("GameManager instance not found!");
        }
    }

    private void OnDestroy()
    {
        // Clean up even listeners
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }
}

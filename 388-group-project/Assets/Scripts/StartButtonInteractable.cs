using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class StartButtonInteractable : MonoBehaviour
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
            GameManager.Instance.LoadNextPuzzle();
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


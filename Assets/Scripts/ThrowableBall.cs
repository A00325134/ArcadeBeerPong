
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class ThrowableBall : MonoBehaviour
{
    private bool scoredThisThrow = false;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable == null)
            grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // A new grab means a new throw.
        scoredThisThrow = false;
    }

    public bool TryRegisterScore()
    {
        if (scoredThisThrow)
            return false;

        scoredThisThrow = true;
        return true;
    }
}


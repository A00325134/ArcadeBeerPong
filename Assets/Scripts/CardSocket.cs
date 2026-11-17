
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSocketInteractor))]
public class CardSocket : MonoBehaviour
{
    [Header("Socket Settings")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private bool isHigherTarget;

    private XRSocketInteractor socket;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        socket.selectEntered.AddListener(OnCardSelected);
    }

    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(OnCardSelected);
    }

    private void OnCardSelected(SelectEnterEventArgs args)
    {
        Transform selectedObject = args.interactableObject.transform;

        PlayingCard card = selectedObject.GetComponent<PlayingCard>();

        if (card == null)
            card = selectedObject.GetComponentInParent<PlayingCard>();

        if (card == null)
            card = selectedObject.GetComponentInChildren<PlayingCard>(true);

        if (card == null)
        {
            Debug.LogWarning("Socket received something that isn't a playing card!");
            return;
        }

        if (gameManager == null)
        {
            Debug.LogError("CardSocket: GameManager reference missing!");
            return;
        }

        Debug.Log("Card entered " +
                  (isHigherTarget ? "HIGHER" : "LOWER") +
                  ": " + card.name);

        gameManager.RegisterChoice(isHigherTarget, card);
    }
}

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class EndGame : MonoBehaviour
{
    [SerializeField] private SceneTransitionManager sceneTransitionManager;
    [SerializeField] private CountdownTimer countdownTimer;

    private void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(x => FinishGame());
    }

    private void FinishGame()
    {
        countdownTimer.gameEnded = true;
        sceneTransitionManager.GoToSceneAsync(0);
    }
}
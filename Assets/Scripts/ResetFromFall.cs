using UnityEngine;

public class ResetFromFall : MonoBehaviour
{
    [SerializeField] SceneTransitionManager sceneTransitionManager;
    [SerializeField] CountdownTimer countdownTimer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            countdownTimer.gameEnded = true;
            sceneTransitionManager.GoToSceneAsync(1);
        }
    }
}
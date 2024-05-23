using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public bool gameEnded = false;

    [HideInInspector] public double currentTime;

    [SerializeField] private double startingTime;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private SceneTransitionManager sceneTransitionManager;

    private void Start()
    {
        currentTime = startingTime;
    }

    private void Update()
    {
        if (!gameEnded)
            Countdown();
    }

    private void Countdown()
    {
        currentTime -= Time.deltaTime;
        countdownText.text = "Time Remaining: " + currentTime.ToString("00.00");

        if (currentTime <= 0)
        {
            gameEnded = true;
            sceneTransitionManager.GoToSceneAsync(1);
        }
    }
}
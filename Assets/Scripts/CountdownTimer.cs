using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    [HideInInspector] public double currentTime;

    [SerializeField] private double startingTime;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject noTimePanel;

    void Start()
    {
        currentTime = startingTime;
    }

    void Update()
    {
        currentTime -= Time.deltaTime;
        countdownText.text = currentTime.ToString("00.00");

        if (currentTime <= 0)
        {
            Time.timeScale = 0f;
            noTimePanel.SetActive(true);
        }
    }
}
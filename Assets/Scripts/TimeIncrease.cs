using UnityEngine;

public class TimeIncrease : MonoBehaviour
{
    public CountdownTimer countdownTimer;

    [SerializeField] private float timeIncrease;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            countdownTimer.currentTime += timeIncrease;
            GameObject.Destroy(gameObject);
        }
    }
}
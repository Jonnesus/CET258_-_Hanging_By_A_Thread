using UnityEngine;

public class TimeIncrease : MonoBehaviour
{
    public CountdownTimer countdownTimer;

    [SerializeField] private float timeIncrease;    
    [SerializeField] private AudioSource playerAudioSource;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            countdownTimer.currentTime += timeIncrease;
            playerAudioSource.Play();
            GameObject.Destroy(gameObject);
        }
    }
}
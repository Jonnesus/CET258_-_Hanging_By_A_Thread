using UnityEngine;

public class CloseInnerAirlockDoor : MonoBehaviour
{
    [SerializeField] private Animator animatorInnerAirlock;
    [SerializeField] private AudioSource doorAudio;
    [SerializeField] private string boolNameAirlock = "airlockOpen";

    private void OnTriggerEnter(Collider other)
    {
        bool isOpen = animatorInnerAirlock.GetBool(boolNameAirlock);
        animatorInnerAirlock.SetBool(boolNameAirlock, !isOpen);
        doorAudio.Play();
        this.gameObject.SetActive(false);
    }
}
using UnityEngine;

public class CloseInnerAirlockDoor : MonoBehaviour
{
    [SerializeField] private Animator animatorInnerAirlock;
    [SerializeField] private string boolNameAirlock = "airlockOpen";

    private void OnTriggerEnter(Collider other)
    {
        bool isOpen = animatorInnerAirlock.GetBool(boolNameAirlock);
        animatorInnerAirlock.SetBool(boolNameAirlock, !isOpen);
        this.gameObject.SetActive(false);
    }
}
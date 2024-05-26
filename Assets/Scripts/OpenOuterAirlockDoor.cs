using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OpenOuterAirlockDoor: MonoBehaviour
{
    [SerializeField] private Animator animatorOuterAirlock;
    [SerializeField] private AudioSource doorAudio;
    [SerializeField] private string boolNameOuterAirlock = "airlockOpen";

    private void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(x => ToggleAirlockOpen());
    }

    public void ToggleAirlockOpen()
    {
        bool isOpen = animatorOuterAirlock.GetBool(boolNameOuterAirlock);
        animatorOuterAirlock.SetBool(boolNameOuterAirlock, !isOpen);
        doorAudio.Play();
    }
}
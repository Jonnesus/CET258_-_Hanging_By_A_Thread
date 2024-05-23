using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class OpenInnerAirlockDoor : MonoBehaviour
{
    [SerializeField] private Animator animatorOuterAirlock;
    [SerializeField] private Animator animatorInnerAirlock;
    [SerializeField] private string boolNameAirlock = "airlockOpen";

    private void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(x => ToggleAirlockOpen());
    }

    public void ToggleAirlockOpen()
    {
        bool isOpen = animatorOuterAirlock.GetBool(boolNameAirlock);
        animatorOuterAirlock.SetBool(boolNameAirlock, !isOpen);
        animatorInnerAirlock.SetBool(boolNameAirlock, isOpen);
    }
}
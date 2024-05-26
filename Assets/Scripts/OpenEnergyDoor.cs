using UnityEngine;

public class OpenEnergyDoor : MonoBehaviour
{
    [SerializeField] private Animator animatorEnergy;
    [SerializeField] private AudioSource doorAudio;
    [SerializeField] private string boolNameEnergy = "cubeInPlace";

    private void Start()
    {
        GetComponent<XRSocketTagInteractor>().selectEntered.AddListener(x => ToggleEnergyDoorOpen());
    }

    public void ToggleEnergyDoorOpen()
    {
        bool isOpen = animatorEnergy.GetBool(boolNameEnergy);
        animatorEnergy.SetBool(boolNameEnergy, !isOpen);
        doorAudio.Play();
    }   
}
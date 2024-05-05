using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerOutcomes : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject portalPanel;

    private bool gameFreeze = false;

    [SerializeField] string playScene = "Overworld";
    [SerializeField] string mainMenuScene = "StartScene";

    private void Update()
    {
        if (gameFreeze == true && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(playScene);
            deathPanel.SetActive(false);
            gameFreeze= false;
            Time.timeScale = 1f;
        }
        else if (gameFreeze == true && Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(mainMenuScene);
            deathPanel.SetActive(false);
            gameFreeze= false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 1f;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DeathObject"))
        {
            Time.timeScale = 0f;
            deathPanel.SetActive(true);
            gameFreeze= true;
        }

        if (collision.gameObject.CompareTag("LevelPortal"))
        {
            Time.timeScale = 0f;
            portalPanel.SetActive(true);
            gameFreeze = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Title and End Screen overlay */

public class ScreenOverlayController : MonoBehaviour
{
    [Header("References")]
    public TouchMovement touchMovement;
    public HelixRotation helixRotation;
    public HelixManager helixManager;
    public GameObject TitleScreen;
    public GameObject EndScreen;
    public GameObject score;
    public GameObject swipeDown;

    [Header("Buttons")]
    public GameObject ObstaclesOff;
    public GameObject ObstaclesOn;

    [Header("Confetti")]
    public GameObject confettiPSPrefab;
    public Vector3 viewportPos = new(0.5f, 0f, 15);

    [Header("Audio")]
    private AudioSource audioSource;

    private void Start()
    {
        if (audioSource == null)
        {
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.Log("no audiosource component");
            }
        }
    }

    public void PlayGame()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        swipeDown.SetActive(true);
        touchMovement.isTitleScreen = false;
        helixRotation.enabled = false;
        helixRotation.transform.rotation = Quaternion.identity;
        score.SetActive(true);
        TitleScreen.SetActive(false);
    }

    public void RetryGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void SetObstaclesOn()
    {
        ObstaclesOn.SetActive(true);
        ObstaclesOff.SetActive(false);
        helixManager.Regenerate();
    }

    public void SetObstaclesOff()
    {
        ObstaclesOn.SetActive(false);
        ObstaclesOff.SetActive(true);
        helixManager.Regenerate();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void GameEnd()
    {
        EndScreen.SetActive(true);
        touchMovement.isTitleScreen = true;
        helixRotation.enabled = true;
        Vector3 confettiPosition = Camera.main.ViewportToWorldPoint(viewportPos);
        Quaternion confettiRotation = Quaternion.Euler(-60, 0, 0);
        GameObject confetti = Instantiate(confettiPSPrefab, confettiPosition, confettiRotation);
        Destroy(confetti, 4f);
    }
}

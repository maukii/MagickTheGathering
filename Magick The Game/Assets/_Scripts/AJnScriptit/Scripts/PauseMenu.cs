using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject pauseObject;

    private bool isPaused = false;

    void Start()
    {
        if (!isPaused && pauseObject.activeInHierarchy)
        {
            pauseObject.SetActive(isPaused);
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            FlipPauseState();
        }
    }

    public void FlipPauseState()
    {
        isPaused = !isPaused;
        pauseObject.SetActive(isPaused);
        player.EnableControls(!isPaused);
        Time.timeScale = isPaused ? 0.0f : 1.0f;
    }
}

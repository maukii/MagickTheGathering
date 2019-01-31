using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private GameObject gameOverObject;

    void Start()
    {
        if (!player.CheckIfDead() && gameOverObject.activeInHierarchy)
        {
            gameOverObject.SetActive(false);
        }
    }

    void Update()
    {
        if (player.CheckIfDead())
        {
            gameOverObject.SetActive(true);
        }
    }
}

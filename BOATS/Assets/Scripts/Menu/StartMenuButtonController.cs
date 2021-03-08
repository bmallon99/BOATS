using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuButtonController : MonoBehaviour
{
    private PlayerControls _player;

    void Start()
    {
        _player = Camera.main.GetComponent<PlayerControls>();
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("Main Map");
    }

    public void GoToInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }

    public void GoToInstructions2()
    {
        SceneManager.LoadScene("Instructions2");
    }

    public void GoToStartMenu()
    {
        SceneManager.LoadScene("Start Menu");
    }

    public void ResumeGame()
    {
        _player.ResumeGame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

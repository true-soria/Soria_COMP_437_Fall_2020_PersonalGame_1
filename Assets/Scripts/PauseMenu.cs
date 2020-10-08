using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused = false;

    public GameObject pauseMenuUI;
    public GameObject countDownUI;
    public GameObject endMenuUI;
    public GameObject ScoreDisplay;
    public Text countDownText;
    public AudioSource countSound;
    public Text scoreDisplayText;
    public Text finalScoreDisplayText;

    private Scene _currentScene;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        scoreDisplayText.text = $"{PlatformerPlayer.Score}pts ";
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        StartCoroutine(SlowUnpause());
    }
    
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
    }

    public void Unpause()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }

    public void EndPause()
    {
        Time.timeScale = 0f;
        ScoreDisplay.SetActive(false);
        finalScoreDisplayText.text = $"{PlatformerPlayer.Score}pts ";
        endMenuUI.SetActive(true);
    }

    public void Restart()
    {
        // SceneManager.LoadScene(_currentScene.name);
        Unpause();
        SceneManager.LoadScene("BeepBlockSkyway");
        
        
    }

    IEnumerator SlowUnpause()
    {
        pauseMenuUI.SetActive(false);
        countDownUI.SetActive(true);
        countSound.Play();
        countDownText.text = "3";
        yield return new WaitForSecondsRealtime(0.5f);
        countDownText.text = "3\t2";
        yield return new WaitForSecondsRealtime(0.5f);
        countDownText.text = "3\t2\t1";
        yield return new WaitForSecondsRealtime(0.5f);
        countDownUI.SetActive(false);
        Time.timeScale = 1f;
        GamePaused = false;
    }
}

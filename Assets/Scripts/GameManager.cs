using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject EndMenu;

    private int playerLives = 3;
    private int score = 0;

    public bool isPaused { get; private set; } = true;
    public bool isPlaying { get; private set; } = false;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P)) {
            TogglePause();
        }
    }

    public void UpdateLives(int lives) {
        playerLives += lives;
        livesText.text = "Lives: " + playerLives;
        if (playerLives <= 0) {
            GameOver();
        }
    }

    public void UpdateScore(int scoreToAdd) {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void TogglePause() {
        if(!isPaused) {
            Time.timeScale = 0;
            isPaused = true;
            pauseMenu.SetActive(true);
        }else if(isPaused) {
            Time.timeScale = 1;
            isPaused = false;
            pauseMenu.SetActive(false);
        }
    }

    private void GameOver() {
        Time.timeScale = 0;
        isPlaying = false;
        EndMenu.SetActive(true);
    }
}

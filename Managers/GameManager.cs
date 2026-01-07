using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Cinemachine;

public class GameManager : MonoBehaviour
{

    public GameObject player1;

    public GameObject player2;

    public float endTime;

    public float endTimer;

    public TMP_Text winText;

    public GameObject targetGroup;

    public int numPlayers;

    public GameObject[] players;

    private bool roundOver;

    public GameObject pauseScreen;

    public int menuState;

    private void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }


    public void WinCheck()
    {
        if (numPlayers <= 1)
        {
            foreach (var player in players)
            {
                if (player.GetComponent<PlayerBehaviorRewire>().isDead == false)
                {
                    StartCoroutine(endRoundNum());
                    winText.text = "Player " + (player.GetComponent<PlayerBehaviorRewire>().playerNumber + 1) + " Wins";
                    numPlayers = 0;
                }
            }
        }
    }

    void endRound()
    {
        endTime += Time.deltaTime;

        if(endTime >= endTimer)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            endTime = 0;
            numPlayers = 0;
        }
    }

    public void Pause()
    {
        if (pauseScreen != null)
        {
            pauseScreen.GetComponent<Canvas>().enabled = true;
        }

        foreach (var player in players)
        {
            Time.timeScale = 0f;
            player.GetComponent<PlayerBehaviorRewire>().EnableMenuControls();
        }
    }

    public void Unpause()
    {
        if(pauseScreen != null)
        {
            pauseScreen.GetComponent<Canvas>().enabled = false;
        }

        foreach (var player in players)
        {
            Time.timeScale = 1f;
            player.GetComponent<PlayerBehaviorRewire>().EnablePlayControls();
        }
    }


    IEnumerator endRoundNum()
    {       
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void StartSingleGame()
    {
        SceneManager.LoadScene(2);
    }
}

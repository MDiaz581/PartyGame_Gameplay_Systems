using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class PauseMenu : MonoBehaviour
{

    public enum MenuState {Resume, Menu};
    public Animator resumeAnimator;
    public Animator returnAnimator;

    private GameManager GM;

    private Player player;

    public int stateInt;

    public int maxButtons;
    public int minButtons;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

    }
    
    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {

            if (player.GetButtonDown("Cancel"))
            {
                Resume();
            }

            switch (stateInt)
            {
                case 0:
                    resumeAnimator.SetBool("Over", true);
                    returnAnimator.SetBool("Over", false);

                    Debug.Log("On Resume");

                    if (player.GetButtonDown("Confirm"))
                    {
                        Resume();
                    }
                    break;
                case 1:
                    resumeAnimator.SetBool("Over", false);
                    returnAnimator.SetBool("Over", true);

                    Debug.Log("On Return");

                    if (player.GetButtonDown("Confirm"))
                    {
                        ReturnToMenu();
                    }
                    break;
                default:
                    Debug.LogWarning("Incorrect Value");
                    break;
            }
        }




    }

    public void OnPause(int PlayerNumber)
    {
        Debug.Log("Assigning Player: " + PlayerNumber);

        player = ReInput.players.GetPlayer(PlayerNumber);

        stateInt = 0;
    }


    public void ChangeState(int change)
    {
        stateInt += change;
        if(stateInt < minButtons)
        {
            stateInt = maxButtons;
        }
        if (stateInt > maxButtons)
        {
            stateInt = minButtons;
        }

    }

    void Resume()
    {
       GM.Unpause();
    }

    void ReturnToMenu()
    {
        Debug.LogWarning("Returning to Menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        
    }
    
}

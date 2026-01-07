using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;


public class MenuManager : MonoBehaviour
{
    private Player player;

    public Animator resumeAnimator;
    public Animator returnAnimator;
    public Animator quitAnimator;
    public Animator returntoMainAnimator;
    public Animator selectAnimator;


    public Canvas mainMenuCanvas;
    public Canvas howToPlayCanvas;
    public Canvas startGameCanvas;

    public int stateInt;

    public int maxButtons;
    public int minButtons;

    public int masterState;

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(0);
        ChangeState(0);

        howToPlayCanvas.enabled = false;
        Time.timeScale = 1f;
    }

    void Update()
    {

        NavigateMenu();

        if (player != null)
        {
            switch (masterState) 
            {
                //This is the Base Menu
                case 0:
                    switch (stateInt)
                    {
                        case 0:
                            resumeAnimator.SetBool("Over", true);
                            returnAnimator.SetBool("Over", false);
                            quitAnimator.SetBool("Over", false);

                            Debug.Log("On Start");

                            if (player.GetButtonDown("Confirm"))
                            {
                                StartGame();
                            }
                            break;
                        case 1:
                            resumeAnimator.SetBool("Over", false);
                            quitAnimator.SetBool("Over", false);
                            returnAnimator.SetBool("Over", true);

                            Debug.Log("On HowToPlay");

                            if (player.GetButtonDown("Confirm"))
                            {
                                HowToPlay();
                            }
                            break;

                        case 2:
                            Debug.Log("Quit");
                            quitAnimator.SetBool("Over", true);

                            resumeAnimator.SetBool("Over", false);
                            returnAnimator.SetBool("Over", false);

                            if (player.GetButtonDown("Confirm"))
                            {
                                Application.Quit();
                            }

                            break;
                        default:
                            Debug.LogWarning("Incorrect Value");
                            break;
                    }
                    break;

                //This is the How to Play Menu
                case 1:
                    switch (stateInt)
                    {
                        case 0:
                            returntoMainAnimator.SetBool("Over", true);

                            Debug.Log("Return");

                            if (player.GetButtonDown("Confirm") || player.GetButtonDown("Cancel"))
                            {
                                Debug.Log("on back");
                                MainMenu();
                            }
                            break;
                        default:
                            Debug.LogWarning("Incorrect Value");
                            break;
                    }
                    break;

                //This is the Start Game Menu
                case 2:
                    Debug.Log("On StartGame");
                    if (player.GetButtonDown("Cancel"))
                    {
                        stateInt = 1;
                    }
                    //This switch statement is all kinds of messed up needs fixing, simple fix was to shift everything around
                    switch (stateInt)
                    {
                        case 0:
                            selectAnimator.SetInteger("StateInt", 0);

                            Debug.Log("On 2");

                            if (player.GetButtonDown("Confirm"))
                            {
                                SceneManager.LoadScene(1);
                            }
                            break;

                        case 1:
                            selectAnimator.SetInteger("StateInt", 1);
                            Debug.Log("3");
                            if (player.GetButtonDown("Confirm"))
                            {
                                SceneManager.LoadScene(2);
                                
                            }
                            break;
                        case 2:
                            selectAnimator.SetInteger("StateInt", 2);
                            Debug.Log("4");
                            if (player.GetButtonDown("Confirm"))
                            {
                                SceneManager.LoadScene(3);
                                
                            }
                            break;
                        case 3:
                            selectAnimator.SetInteger("StateInt", 3);
                            Debug.Log("5");
                            if (player.GetButtonDown("Confirm"))
                            {
                                SceneManager.LoadScene(4);
                            }
                            break;
                        case 4:
                            selectAnimator.SetInteger("StateInt", 4);

                            if (player.GetButtonDown("Confirm") || player.GetButtonDown("Cancel"))
                            {
                                MainMenu();
                            }
                            break;
                        default:
                            Debug.LogWarning("Incorrect Value");
                            break;
                    }
                    break;

                default:
                    Debug.LogWarning("Out of Bounds");
                    break;
            }
        }

    }

    public void NavigateMenu()
    {
        if (player.GetNegativeButtonDown("Vertical Move"))
        {

           ChangeState(1);
        }

        if (player.GetButtonDown("Vertical Move"))
        {

           ChangeState(-1);
        }

    }


    public void ChangeState(int change)
    {
        stateInt += change;
        if (stateInt < minButtons)
        {
            stateInt = maxButtons;
        }
        if (stateInt > maxButtons)
        {
            stateInt = minButtons;
        }

 
    }

    void StartGame()
    {
        Debug.LogWarning("Loading New Menu");

        masterState = 2;
        stateInt = 0;
        maxButtons = 4;

        startGameCanvas.enabled = true;

        mainMenuCanvas.enabled = false;
    }

    void HowToPlay()
    {
        masterState = 1;
        stateInt = 0;
        maxButtons = 0;
        
        howToPlayCanvas.enabled = true;

        mainMenuCanvas.enabled = false;


    }

    void MainMenu()
    {
        masterState = 0;
        stateInt = 0;
        maxButtons = 2;

        mainMenuCanvas.enabled = true;

        howToPlayCanvas.enabled = false;
        startGameCanvas.enabled = false;
    }
}

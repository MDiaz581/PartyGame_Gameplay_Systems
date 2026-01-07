using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamFindPlayers : MonoBehaviour
{

    public CinemachineTargetGroup TG;

    public GameManager GM;

    public GameObject[] players;

    [SerializeField]
    [Tooltip("This is the speed in which the target player will lose target group weight after they die")]
    private float lerpSpeed;
 

    // Start is called before the first frame update
    void Start()
    {
        //Quickly removes the editor targets
        TG.RemoveMember(TG.m_Targets[1].target);
        TG.RemoveMember(TG.m_Targets[0].target);
        
        //Finds every player in the game
        players = GameObject.FindGameObjectsWithTag("Player");

        //loops and adds them into the target group list
        foreach(var player in players)
        {

            TG.AddMember(player.transform, 1f, 10f);

        }
    }

    //This function is called when the player dies in the PlayerBehavior script
    public void RemovePlayer()
    {
        foreach (var player in players)
        {

            if (player.layer == LayerMask.NameToLayer("Dead"))
            {
                //tracks the number of the dead player not used but could be useful
                int pNumber = TG.FindMember(player.transform);

                StartCoroutine(LerpWeight(player));

            }
        }
    }

    IEnumerator LerpWeight(GameObject player)
    {
        float t = 0f;

        while(TG.m_Targets[TG.FindMember(player.transform)].weight > 0.01f)
        {

            TG.m_Targets[TG.FindMember(player.transform)].weight = Mathf.Lerp(1f, 0f, t);

            t += lerpSpeed * Time.deltaTime;

            yield return null;
            
        }

        TG.m_Targets[TG.FindMember(player.transform)].weight = 0;

        yield return null;
    }

}

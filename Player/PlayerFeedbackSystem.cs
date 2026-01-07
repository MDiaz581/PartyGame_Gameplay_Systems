using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerFeedbackSystem : MonoBehaviour
{

    public MMFeedbacks _screenShake;

    // Start is called before the first frame update
    void Start()
    {
        _screenShake = GameObject.Find("Feedbacks/CameraShake").GetComponent<MMFeedbacks>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScreenShake()
    {
        _screenShake?.PlayFeedbacks();
    }
}

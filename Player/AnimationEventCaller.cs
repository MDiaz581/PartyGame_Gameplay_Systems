using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCaller : MonoBehaviour
{

    public PlayerBehaviorRootmotion PBR;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Jump()
    {
        PBR.grounded = false;
    }

    void Land()
    {
        PBR.grounded = true;
    }

    void DisableRootMotion()
    {
        PBR.animator.applyRootMotion = false;
    }

    void EnableRootMotion()
    {
        PBR.animator.applyRootMotion = true;
    }
}

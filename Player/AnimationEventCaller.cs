using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCaller : MonoBehaviour
{
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

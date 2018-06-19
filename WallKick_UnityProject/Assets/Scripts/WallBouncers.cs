using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBouncers : MonoBehaviour {

    public bool leftBouncers;
    public bool rightBouncers;

    public SplitWallLimiters leftSplitWallLimiter;
    public SplitWallLimiters rightSplitWallLimiter;

    public GameObject leftParticleEmitter;
    public GameObject rightParticleEmitter;

    // Update is called once per frame
    void Update () {
        if (leftBouncers)
        {
            if (leftSplitWallLimiter.isHit)
            {
                Instantiate(leftParticleEmitter, transform.position, Quaternion.identity);
            }
        }

        if (rightBouncers)
        {
            if (rightSplitWallLimiter.isHit)
            {
                Instantiate(rightParticleEmitter, transform.position, Quaternion.identity);
            }
        }
    }
}

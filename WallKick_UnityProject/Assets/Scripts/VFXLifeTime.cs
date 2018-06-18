using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXLifeTime : MonoBehaviour {

    public float lifeTime;

	void Update () {
        lifeTime -= 1 * Time.deltaTime;

        if (lifeTime <= 0)
            Destroy(gameObject);
    }
}

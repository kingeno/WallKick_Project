using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour {

    public float repulsionForce;

	// Use this for initialization
	void Start () {
		if (repulsionForce < 0.5f)
            Debug.LogError("On the \"Force Field\" object, check the \"repulsion force\" variable in the Inspector, it should be at least greater than [...]");
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player1")
        {
            other.attachedRigidbody.AddForce(transform.right * -repulsionForce, ForceMode2D.Impulse);
            other.attachedRigidbody.AddForce(transform.up * repulsionForce, ForceMode2D.Impulse);
        }
        if (other.tag == "Player2")
        {
            other.attachedRigidbody.AddForce(transform.right * repulsionForce, ForceMode2D.Impulse);
        }
    }
}

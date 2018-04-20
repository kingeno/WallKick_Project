using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSplitGear : MonoBehaviour {

    public GameObject splitWall;
    public WallSplitMovement splitWallMovement;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //Set the horizontal position of the button on the wall
        transform.position = new Vector3(splitWall.transform.position.x, transform.position.y, transform.position.z );

        transform.Rotate(Vector3.back * Time.deltaTime * splitWallMovement.horizontalVelocity * 55f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour {

    public bool isGrounded = false;

    public GameObject[] plateforms;
    public Collider2D[] plateformColliders;
    public string plateformName;

    private void Start()
    {
        plateformColliders = new Collider2D[4];

        plateforms = GameObject.FindGameObjectsWithTag("Plateform");

        for (int i = 0; i < plateforms.Length; i++)
        {
            plateformColliders[i] = plateforms[i].GetComponent<Collider2D>();
            plateformColliders[i].name = "PlateformCol_" + i.ToString();
            //Debug.Log(plateformColliders[i].name);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ground" || other.tag == "Plateform")
        {
            if (other.tag == "Plateform")
            {
                plateformName = other.name;
                //Debug.Log("plateform name = " + plateformName);
            }

            isGrounded = true;
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ground" || other.tag == "Plateform")
        {
            isGrounded = false;
            plateformName = "empty";
            //Debug.Log("plateform name = " + plateformName);
        }
    }
}

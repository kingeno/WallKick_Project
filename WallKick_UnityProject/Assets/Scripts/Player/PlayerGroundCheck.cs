using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour {

    public bool isGrounded = false;

    public GameObject[] plateforms;
    public List <Collider2D> plateformColliders;
    public string plateformName;
    public int numberOfPlateforms;

    private void Start()
    {
        plateformColliders = new List<Collider2D>();

        plateforms = GameObject.FindGameObjectsWithTag("Plateform");

        for (int i = 0; i < plateforms.Length; i++)
        {
            plateformColliders.Add(plateforms[i].GetComponent<Collider2D>());
            //plateformColliders[i] = plateforms[i].GetComponent<Collider2D>();
            plateformColliders[i].name = "PlateformCol_" + i.ToString();
            Debug.Log(plateformColliders[i].name);
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

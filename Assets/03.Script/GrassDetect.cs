using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassDetect : MonoBehaviour {
    public Material _SolidGrassMat;
    public Material _TransparentGrassMat;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grass"))
        {
            other.GetComponent<Renderer>().material = _TransparentGrassMat;
        }
        else if(other.CompareTag("Competition"))
        {
            other.GetComponent<AIController>()._detected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Grass"))
        {
            other.GetComponent<MeshRenderer>().material = _SolidGrassMat;
        }
        else if (other.CompareTag("Competition"))
        {
            other.GetComponent<AIController>()._detected = false;
        }
    }
}

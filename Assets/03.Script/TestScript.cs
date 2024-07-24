using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {
    public GameObject start;
    public GameObject end;

    float time;
    float up = 1f;
	// Use this for initialization
	void Start () {
        time = 0;

	}

    bool force = false;
    public void addforce()
    {
        force = true;
    }

    private void FixedUpdate()
    {
        if (force)
        {
            force = false;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 9.3f, 5), ForceMode.VelocityChange);
            //StartCoroutine(timecheck());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().useGravity = false;

        }
    }
    IEnumerator timecheck()
    {
        yield return new WaitForSeconds(2.0f);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = false;

    }
    // Update is called once per frame
    void Update () {
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateCrystal : MonoBehaviour
{
    public GameObject _crystal;
    public float _createTerm = 15f;
    float _timeChecker;

    Vector3[] _createDir = new Vector3[8];
    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {
        _timeChecker = 0f;
        _createDir[0] = new Vector3(0, 5, 1);
        _createDir[1] = new Vector3(1, 5, 1);
        _createDir[2] = new Vector3(1, 5, 0);
        _createDir[3] = new Vector3(1, 5, -1);
        _createDir[4] = new Vector3(0, 5, -1);
        _createDir[5] = new Vector3(-1, 5, 1);
        _createDir[6] = new Vector3(-1, 5, 0);
        _createDir[7] = new Vector3(-1, 5, 1);
                
    }

    // Update is called once per frame
    void Update()
    {
        _timeChecker += Time.deltaTime;

        if(_timeChecker >= _createTerm)
        {
            _timeChecker = 0f;

            GameObject crystal = Instantiate(_crystal, new Vector3(0, 0.5f, 0), Quaternion.Euler(0, 0, 0));
            crystal.GetComponent<Rigidbody>().AddForce(_createDir[Random.Range(0, 8)],ForceMode.VelocityChange);

        }
    }
}

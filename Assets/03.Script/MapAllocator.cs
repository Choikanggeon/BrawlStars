using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAllocator : MonoBehaviour
{
    #region Singleton
    public static MapAllocator instance = null;

    public int _selectedCharacterIdx;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }
    #endregion

    public GameObject[] _playerPos = new GameObject[10];

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}

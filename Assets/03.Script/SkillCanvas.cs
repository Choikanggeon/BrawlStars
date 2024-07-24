using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillCanvas : JoyStickController
{
    #region Singleton
    public static SkillCanvas instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);
    }
    #endregion

    public GameObject _gage;

    // Use this for initialization
    void Start()
    {
        base.Start();
        _stickDir = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        _stickDir = _innerCircle.transform.position - _outterCircle.transform.position;
    }

}


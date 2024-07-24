using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameManager : MonoBehaviour
{
    UIGameManager _instance = null;

    private void Awake()
    {
        _instance = this;
    }


}

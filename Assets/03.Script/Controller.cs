using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Animator anim;
    float temp = 1;

    private void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f)
        {
            if (temp >= 0)
            {
                temp -= Time.deltaTime;
            }
            anim.SetLayerWeight(1, temp);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelly_Projectile : Projectile_Colt
{
    // Update is called once per frame
	void Update () {
        base.Update();
	}

    //new public void TheStart(float velocity, int damage, float range, bool isSkill)
    //{
    //    _velocity = velocity; _damage = damage; _range = range;
    //    _isSkill = isSkill;

    //    _startPos = transform.position;
    //    Vector3 dir = transform.forward.normalized * range;
    //    _endPos = dir;
    //    _endPos += _startPos;
    //    _journeyLength = (_endPos - _startPos).magnitude;
    //    _startTime = Time.time;
    //    _hit = false;
    //}


}

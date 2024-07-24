using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    protected int _damage;
    protected bool _isSkill;
    protected bool _hit;

    public GameObject _trail;
    public GameObject _playerHitParticle;
    public GameObject _explosionPlarticle;

    public GameObject _rockhitParticle;
    public GameObject _rockExplosionParticle;
    public GameObject _grassParticle;

    public GameObject _bulletMeshObj;

    protected GameObject _caller;
    protected string _enemyTag;

    protected IEnumerator DistortCollider(Collider other)
    {
        float startTime = Time.time;
        float timeChecker = 0;
        Vector3 origin = other.transform.localScale;
        // 크게
        while (timeChecker <= 0.05f)
        {
            timeChecker = Time.time - startTime;
            other.transform.localScale = new Vector3(other.transform.localScale.x + Time.deltaTime,
                other.transform.localScale.y + Time.deltaTime,
                other.transform.localScale.z + Time.deltaTime);
            yield return null;
        }
        startTime = Time.time;
        timeChecker = 0;
        // 작게
        while (timeChecker <= 0.1f)
        {
            timeChecker = Time.time - startTime;
            other.transform.localScale = new Vector3(other.transform.localScale.x - Time.deltaTime,
                other.transform.localScale.y - Time.deltaTime,
                other.transform.localScale.z - Time.deltaTime);
            yield return null;
        }
        startTime = Time.time;
        timeChecker = 0;
        // 크게
        while (timeChecker <= 0.05f)
        {
            timeChecker = Time.time - startTime;
            other.transform.localScale = new Vector3(other.transform.localScale.x + Time.deltaTime,
                other.transform.localScale.y + Time.deltaTime,
                other.transform.localScale.z + Time.deltaTime);
            yield return null;
        }

        other.transform.localScale = origin;
    }

    protected abstract IEnumerator DestroySelf(float time);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectile_Colt : Projectile
{
    protected float _range;
    protected float _slowvelocity;
    protected float _fastvelocity;
    

    protected Vector3 _startPos;
    protected Vector3 _endPos;
    protected float _startTime;
    protected float _journeyLength;

    // Use this for initialization

    // not unity default Start method
    public void TheStart(GameObject caller, float fastvelocity, int damage, float range, bool isSkill, string enemyTag)
    {
        _caller = caller;
        _fastvelocity = fastvelocity;
        _damage = damage;
        _range = range;
        _isSkill = isSkill;
        _enemyTag = enemyTag;

        _startPos = transform.position;
        Vector3 dir = transform.forward.normalized * range;
        _endPos = dir;
        _endPos += _startPos;
        _journeyLength = (_endPos - _startPos).magnitude;
        _startTime = Time.time;
        _hit = false;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!_hit)
        {
            float distCovered = (Time.time - _startTime) *_fastvelocity;
            float fracJourney = distCovered / _journeyLength;
            if (fracJourney >= 1.0f)
                fracJourney = 1.0f;
            transform.position = Vector3.Lerp(_startPos, _endPos, fracJourney);
            if (fracJourney >= 1.0f)
            {
                StartCoroutine(DestroySelf(1.0f));
            }
        }
    }


    protected void OnTriggerEnter(Collider other)
    {

        if (!_hit) // 한놈만 패려고 
        {
            if (other.CompareTag("NotGrass"))
            {
                if (_isSkill)
                {
                    GameObject particle = Instantiate(_rockExplosionParticle, other.transform.position, other.transform.rotation);
                    Destroy(other.gameObject);
                }
                else
                {
                    GameObject particle = Instantiate(_rockhitParticle, transform.position, transform.rotation);
                    if (other.GetComponent<DistrotObject>())
                    {
                        other.GetComponent<DistrotObject>().StartDistort();
                    }
                    else
                    {
                        other.gameObject.AddComponent<DistrotObject>();
                        other.GetComponent<DistrotObject>().StartDistort();
                    }
                    //StartCoroutine(DistortCollider(other));
                    StartCoroutine(DestroySelf(0.5f));
                }
            }
            else if (other.CompareTag("Grass"))
            {
                if (_isSkill)
                {
                    GameObject particle = Instantiate(_grassParticle, other.transform.position, other.transform.rotation);
                    Destroy(other.gameObject);
                }

            }
            else if (other.CompareTag(_enemyTag))
            {
                _caller.GetComponent<PlayerStats>().HitCompetition(other.gameObject);
                other.GetComponent<PlayerStats>().HitByPlayerProjectile(_damage);
                StartCoroutine(DestroySelf(1.0f));
            }
            else if (_caller.gameObject.tag.CompareTo("Competition") == 0 &&
                other.CompareTag("Player"))
            {
                _caller.GetComponent<PlayerStats>().HitCompetition(other.gameObject);
                other.GetComponent<PlayerStats>().HitByPlayerProjectile(_damage);
                StartCoroutine(DestroySelf(1.0f));
            }
            if (other.CompareTag("EnergyBox"))
            {
                GameObject particle = Instantiate(_rockhitParticle, transform.position, transform.rotation);
                if (other.GetComponent<DistrotObject>())
                {
                    other.GetComponent<DistrotObject>().StartDistort();
                }
                else
                {
                    other.gameObject.AddComponent<DistrotObject>();
                    other.GetComponent<DistrotObject>().StartDistort();
                }
                //StartCoroutine(DistortCollider(other));
                StartCoroutine(DestroySelf(0.5f));
                other.GetComponent<PlayerStats>().HitByPlayerProjectile(_damage);
            }
        }
    }

    protected override IEnumerator DestroySelf(float time)
    {
        _hit = true;
        _bulletMeshObj.GetComponent<Renderer>().enabled = false;
        _explosionPlarticle.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}

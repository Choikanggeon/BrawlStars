using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BarleyPlaneAttack : MonoBehaviour {
    int _damage;

    List<GameObject> _enteredEnemy;
    Dictionary<GameObject, float> _dictionary;

    GameObject _caller;
    string _enemyTag;

	// Use this for initialization
	public void TheStart (GameObject caller, int damage, string enemyTag) {
        _caller = caller;
        _damage = damage;
        _dictionary = new Dictionary<GameObject, float>();
        _enemyTag = enemyTag;

        StartCoroutine(DestorySelf(2.0f));
	}
	
    IEnumerator DestorySelf(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponentInChildren<SphereCollider>().enabled = false;

        float startTime = 0;
        float destroyTime = 0.3f;
        while (startTime <= destroyTime)
        {
            startTime += Time.deltaTime;

            transform.localScale =
                new Vector3(0.2f - 0.2f * startTime / destroyTime,
                0.2f - 0.2f * startTime / destroyTime,
                0.2f - 0.2f * startTime / destroyTime);

            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(_enemyTag))
        {
             _caller.GetComponent<PlayerStats>().HitCompetition(other.gameObject);
            _dictionary.Add(other.gameObject, Time.time);
            other.GetComponent<PlayerStats>().HitByPlayerProjectile(_damage);

        }
        else if (_caller.gameObject.tag.CompareTo("Competition") == 0 &&
               other.CompareTag("Player"))
        {
            _caller.GetComponent<PlayerStats>().HitCompetition(other.gameObject);
            try
            {
                _dictionary.Add(other.gameObject, Time.time);
            }catch(Exception e)
            {
                ;
            }
            other.GetComponent<PlayerStats>().HitByPlayerProjectile(_damage);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Competition"))
        {
            try
            {
                float t = _dictionary[other.gameObject];
                if (Time.time - t >= 1.2f)
                {
                    _caller.GetComponent<PlayerStats>().HitCompetition(other.gameObject);
                    _dictionary[other.gameObject] = Time.time;
                }
            }catch(KeyNotFoundException knfe)
            {

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Competition"))
        {
            _dictionary.Remove(other.gameObject);
        }
    }
}

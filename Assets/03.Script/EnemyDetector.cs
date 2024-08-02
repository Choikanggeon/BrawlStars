using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    AIController _aiController;
    GameObject _target;
    // Start is called before the first frame update
    void Start()
    {
        _aiController = GetComponentInParent<AIController>();
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EnergyBox"))
        {
            
            if (_aiController._energyBox == null)
            {
                _aiController._energyBox = other.gameObject;
            }
            
            else
            {
                float distanceTarget = Vector3.Distance(
                    transform.position, 
                    _aiController._energyBox.transform.position
                    );
                float distanceCollider = Vector3.Distance(
                    transform.position, 
                    other.transform.position
                    );
               
                if (distanceCollider < distanceTarget)
                {
                    _aiController._energyBox = other.gameObject;
                }
            }
           
        }

        if (_aiController.CompareTag("Competition") && other.gameObject != _aiController.gameObject)
        {
            //Debug.Log("if - Competition");
            if (other.CompareTag("Competition") || other.CompareTag(_aiController._enemyTag))
            {
               //Debug.Log("if - Player");

                if (!other.GetComponent<PlayerStats>()._isCharacterInGrass)
                {
                    if (_aiController._enemy == null)
                    {
                        //Debug.Log(other.gameObject.name);
                        _aiController._enemy = other.gameObject;
                    }
                    else
                    {
                        float distanceEnemy =
                            Vector3.Distance(
                                transform.position,
                                _aiController._enemy.transform.position
                                );
                        float distanceCollider =
                            Vector3.Distance(
                                transform.position,
                                other.transform.position
                                );

                        // 기존에 할당된 enemy 와 거리 비교후 더 가까운 적을 타겟으로 설정
                        if (distanceCollider < distanceEnemy)
                        {
                            _aiController._enemy = other.gameObject;
                        }
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EnergyBox"))
        {
            if (_aiController._energyBox == other.gameObject)
                _aiController._energyBox = null;
        }
        else if (other.CompareTag("Player") || other.CompareTag(_aiController._enemyTag) && other.gameObject != _aiController.gameObject)
        {
            if (_aiController._enemy == other.gameObject)
                _aiController._enemy = null;
        }
    }
}

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

    //private void OnTriggerEnter(Collider other)
    //{
    //    // 트리거 오브젝트가 크리스탈일 때
    //    if (other.CompareTag("Crystal"))
    //    {
    //        // 기존에 크리스탈이 할당 안되어 있을 때
    //        if (_aiController._crystal == null)
    //        {
    //            _aiController._crystal = other.gameObject;
    //        }
    //        // 기존의 크리스탈과 새로 enter 한 크리스탈을 현재 위치를 기준으로 비교
    //        else
    //        {
    //            float distanceTarget = Vector3.Distance(transform.position, _aiController._crystal.transform.position);
    //            float distanceCollider = Vector3.Distance(transform.position, other.transform.position);
    //            // 더 가까운 크리스탈을 할당
    //            if (distanceCollider < distanceTarget)
    //            {
    //                _aiController._crystal = other.gameObject;
    //            }
    //        }
    //        // 트리거 오브젝트가 사용자 이거나 혹은 상대방일 경우
    //    }
    //    else if (other.CompareTag("Player") || other.CompareTag(_aiController._enemyTag))
    //    {
    //        // PlayerStats 의 변수인 _isCharacterInGrass로 풀숲에 들어간 경우
    //        // 확인할 수 없으므로 할당할 수 없다.
    //        if (!other.GetComponent<PlayerStats>()._isCharacterInGrass)
    //        {
    //            if (_aiController._enemy == null)
    //            {
    //                _aiController._enemy = other.gameObject;
    //            }
    //            else
    //            {
    //                float distanceEnemy = 
    //                    Vector3.Distance(
    //                        transform.position, 
    //                        _aiController._enemy.transform.position
    //                        );
    //                float distanceCollider = 
    //                    Vector3.Distance(
    //                        transform.position, 
    //                        other.transform.position
    //                        );

    //                // 기존에 할당된 enemy 와 거리 비교후 더 가까운 적을 타겟으로 설정
    //                if (distanceCollider < distanceEnemy)
    //                {
    //                    _aiController._enemy = other.gameObject;
    //                }
    //            }
    //        }
    //    }
    //}

    private void OnTriggerStay(Collider other)
    {
        // 트리거 오브젝트가 크리스탈일 때
        if (other.CompareTag("Crystal"))
        {
            // 기존에 크리스탈이 할당 안되어 있을 때
            if (_aiController._crystal == null)
            {
                _aiController._crystal = other.gameObject;
            }
            // 기존의 크리스탈과 새로 enter 한 크리스탈을 현재 위치를 기준으로 비교
            else
            {
                float distanceTarget = Vector3.Distance(
                    transform.position, 
                    _aiController._crystal.transform.position
                    );
                float distanceCollider = Vector3.Distance(
                    transform.position, 
                    other.transform.position
                    );
                // 더 가까운 크리스탈을 할당
                if (distanceCollider < distanceTarget)
                {
                    _aiController._crystal = other.gameObject;
                }
            }
            // 트리거 오브젝트가 사용자 이거나 혹은 상대방일 경우
        }

        if (_aiController.CompareTag("Competition"))
        {
            //Debug.Log("if - Competition");
            if (other.CompareTag("Player") || other.CompareTag(_aiController._enemyTag))
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
        else if (_aiController.CompareTag("Company"))
        {
            if (other.CompareTag(_aiController._enemyTag))
            {
                if (!other.GetComponent<PlayerStats>()._isCharacterInGrass)
                {
                    if (_aiController._enemy == null)
                    {
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
        // 트리거 오브젝트가 크리스탈일 때
        if (other.CompareTag("Crystal"))
        {
            if (_aiController._crystal == other.gameObject)
                _aiController._crystal = null;
        }
        else if (other.CompareTag("Player") || other.CompareTag(_aiController._enemyTag))
        {
            if (_aiController._enemy == other.gameObject)
                _aiController._enemy = null;
        }
    }
}

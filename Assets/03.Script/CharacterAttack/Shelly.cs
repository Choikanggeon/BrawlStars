using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shelly : CharacterAttack
{
    GameObject _firePosParent;
    public GameObject _shotIndicatorPrefab;
    GameObject _shotIndicator;
    Coroutine _drawIndicatorRoutine;
    public float viewRadius = 3;
    public float viewAngle = 30f;
    public float meshResolution = 1f;
    MeshFilter viewMeshFilter;
    Mesh viewMesh;
    public LayerMask obstacleMask;
    public int edgeResolveIterations;
    public float edgeDstThreshold;
    public CharacterInfo _characterInfo;

    bool hasHitOccurred = false; // 충돌 발생 여부를 추적하는 변수
    LineRenderer lineRenderer; // LineRenderer 변수 추가

    void Start()
    {
        base.Start();
        _shotIndicator = GameObject.Instantiate(_shotIndicatorPrefab, transform.position, transform.rotation);
        _shotIndicator.SetActive(false);
        viewMeshFilter = _shotIndicator.GetComponent<MeshFilter>();
        _playerStats = GetComponent<PlayerStats>();
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        _shotIndicator.SetActive(false);

        if (!_isCharacterAI)
            _firePosParent = PlayerController.instance._firePosParent;
        else
            _firePosParent = GetComponent<AIController>()._firePosParent;

        lineRenderer = GetComponent<LineRenderer>(); // LineRenderer 컴포넌트 가져오기
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>(); // 없다면 추가
        }

        // LineRenderer 설정
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        if (!_isCharacterAI)
            _fireDir = PlayerController.instance._attackStickDir;
        else
            _fireDir = _AIController._attackStickDir;
        _shotIndicator.transform.position = transform.position;
    }

    public override void FireBaseAttack()
    {
        if (!_playerStats.onFire()) return; //onFire()의 bool값이 false면 함수 나가고
        StartCoroutine(ActivateBaseAttack(false));//코루틴 실행 activateBaseAttack
    }

    public override void FireSkillAttack()
    {
        base.FireSkillAttack();
        StartCoroutine(ActivateBaseAttack(true));
    }

    public override void StartDrawIndicator(bool isSkill)
    {
        if (_isCharacterAI) return;

        if (!_shotIndicator.activeSelf)
        {
            hasHitOccurred = false; // 초기화
            _shotIndicator.SetActive(true);
            _drawIndicatorRoutine = StartCoroutine(DrawAttackIndicator(isSkill));
        }
    }

    public override void StopDrawIndicator()
    {
        if (_isCharacterAI) return;

        if (_drawIndicatorRoutine != null)
        {
            StopCoroutine(_drawIndicatorRoutine);
            _shotIndicator.SetActive(false);
        }
    }

    IEnumerator ActivateBaseAttack(bool isSkill)
    {
        _audioSource.clip = _fireSound;

        string enemyTag;
        if (!_isCharacterAI)
        {
            PlayerController.instance.isActivatingSkill = true;
            enemyTag = "Competition";
        }
        else
        {
            _AIController.isActivatingSkill = true;
            enemyTag = GetComponent<AIController>()._enemyTag;
        }

        Vector3 fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);
        GameObject muzzle;
        GameObject projectile;

        int stepCount = 5;
        float stepAngleSize = viewAngle / stepCount;

        _upperBodyDir = fireDir;
        rotate = true;

        if (isSkill)
        {
            viewRadius = PlayerController.instance._playerStats._skillRange;
            viewAngle = 45f;
            stepCount = 5;
        }
        else
        {
            viewRadius = PlayerController.instance._playerStats._bulletRange;
            viewAngle = 35f;
            stepCount = 5;
        }

        _audioSource.Play();

        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < stepCount; i++)
        {
            float y_stick = Vector3.Angle(Vector3.forward, fireDir);
            if (fireDir.x < 0)
                y_stick *= -1;
            float angle = y_stick - viewAngle / 2 + stepAngleSize * i;

            Vector3[] startPoints = new Vector3[4];
            for (int j = 0; j < 4; j++)
            {
                if (j != 2)
                    angle += 1.7f * stepAngleSize / 4;
                Vector3 dirFromAngle =
                          new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad),
                           0,
                           Mathf.Cos(angle * Mathf.Deg2Rad));

                if (j == 0 || j == 3)
                    startPoints[j] = transform.position + 4 * Vector3.up + dirFromAngle * 0.5f;
                else if (j == 1)
                {
                    startPoints[j] = transform.position + 4 * Vector3.up + dirFromAngle * 0.05f;
                }
                else if (j == 2)
                    startPoints[j] = transform.position + 4 * Vector3.up + dirFromAngle * 1.5f;
                if (!isSkill)
                {
                    projectile = GameObject.Instantiate(_projectile,
                        startPoints[j],
                        new Quaternion(0, 0, 0, 0));
                }
                else
                    projectile = GameObject.Instantiate(_projectile,
                        startPoints[j],
                        new Quaternion(0, 0, 0, 0));
                projectile.transform.rotation = Quaternion.LookRotation(dirFromAngle);


                projectile.GetComponent<Projectile_Colt>().TheStart(
                    gameObject,
                    _playerStats._bulletFastVelocity,
                    _playerStats._damage
                    , _playerStats._bulletRange
                    , isSkill
                    , enemyTag
                    );
            }
        }

        if (!_isCharacterAI)
            PlayerController.instance.isActivatingSkill = false;
        else
            _AIController.isActivatingSkill = false;
        rotate = false;
    }

    IEnumerator DrawAttackIndicator(bool isSkill)
    {
        if (isSkill)
        {
            viewRadius = PlayerController.instance._playerStats._skillRange;
            viewAngle = 40f;
        }
        else
        {
            viewRadius = PlayerController.instance._playerStats._bulletRange;
            viewAngle = 30f;
        }

        while (true)
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();
            Vector3 fireParentPos = _firePosParent.transform.localPosition; 

            for (int i = 0; i < stepCount; i++)
            {
                float y_stick = Vector3.Angle(Vector3.forward, new Vector3(_fireDir.x, 0, _fireDir.y));
                if (_fireDir.x < 0)
                    y_stick *= -1;
                float angle = y_stick - viewAngle / 2 + stepAngleSize * i;

                Vector3 dirFromAngle = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
                Vector3 point = fireParentPos + dirFromAngle.normalized * 0.5f + Vector3.up;

                RaycastHit hit;
                if (Physics.Raycast(fireParentPos + Vector3.up, dirFromAngle, out hit, viewRadius, obstacleMask))
                {
                    point = hit.point;
                    viewPoints.Add(point);
                    break; // 충돌 지점에서 그리기 중단
                }
                else
                {
                    point = fireParentPos + dirFromAngle * viewRadius + Vector3.up + dirFromAngle * 0.5f;
                    viewPoints.Add(point);
                }
            }

            int vertexCount = viewPoints.Count * 2;

            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            for (int i = 0; i < vertexCount / 2; i++)
            {
                vertices[i * 2] = fireParentPos + Vector3.up;
                vertices[i * 2 + 1] = viewPoints[i];
                if (i * 6 + 5 < (vertexCount - 2) * 3)
                {
                    try
                    {
                        triangles[i * 6] = i * 2;
                        triangles[i * 6 + 1] = i * 2 + 1;
                        triangles[i * 6 + 2] = i * 2 + 2;

                        triangles[i * 6 + 3] = i * 2 + 2;
                        triangles[i * 6 + 4] = i * 2 + 1;
                        triangles[i * 6 + 5] = i * 2 + 3;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError("Triangle creation error at index: " + i + " - " + e.Message);
                    }
                }
            }

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();

            // LineRenderer 설정
            lineRenderer.positionCount = viewPoints.Count;
            lineRenderer.SetPositions(viewPoints.ToArray());

            yield return new WaitForEndOfFrame();
        }
    }
}

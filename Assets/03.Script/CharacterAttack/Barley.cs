using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barley : CharacterAttack {
    LineRenderer _shotIndicator;
    Coroutine _lineRenderingRoutine;

    GameObject _circleIndicator;
    public GameObject _attackCircle;
    public GameObject _skillCircle;

    public float _indicatorSensitivity = 1f;
    Vector3 _landingPosition;

    float _lastTimeFire;

    // Use this for initialization
    void Start () {
        base.Start();
        if (!_isCharacterAI)
        {
            _shotIndicator = GetComponent<LineRenderer>();
            _shotIndicator.positionCount = 10;
        }
        _attackCircle.SetActive(false);
        _skillCircle.SetActive(false);
        _landingPosition = Vector3.zero;

        _lastTimeFire = Time.time;

    }

    // Update is called once per frame
    void Update () {
        if (!_isCharacterAI)
            _fireDir = PlayerController.instance._attackStickDir;
        else
            _fireDir = _AIController._attackStickDir;

    }

    override public void StartDrawIndicator(bool isSkill)
    {
        if (_isCharacterAI) return;

        if (_shotIndicator.enabled == false)
        {
            if (isSkill)
            {
                _circleIndicator = _skillCircle;
            }
            else
            {
                _circleIndicator = _attackCircle;
            }
            _shotIndicator.enabled = true;
            _lineRenderingRoutine = StartCoroutine(DrawAttackIndicator(isSkill));
        }
    }

    override public void StopDrawIndicator()
    {
        if (_isCharacterAI) return;

        if (_lineRenderingRoutine != null)
            StopCoroutine(_lineRenderingRoutine);

        _skillCircle.SetActive(false);
        _attackCircle.SetActive(false);
        _shotIndicator.enabled = false;
    }

    override public void FireBaseAttack()
    {
        if (!_playerStats.onFire()) return;

        if (Time.time - _lastTimeFire < 0.5f) return;
        _lastTimeFire = Time.time;

        _audioSource.clip = _fireSound;

        string enemyTag;
        Vector3 targetPosition = Vector3.zero;
        Vector3 fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);

        _upperBodyDir = fireDir;
        rotate = true;

        if (!_isCharacterAI)
        {
            enemyTag = "Competition";
            targetPosition = _circleIndicator.transform.position;
        }
        else
        {
            enemyTag = GetComponent<AIController>()._enemyTag;
            targetPosition = GetComponent<AIController>()._enemy.transform.position;
        }

        _audioSource.Play();

        GameObject muzzle = Instantiate(_muzzleEffect,
            _firePos.transform.position,
            _firePos.transform.rotation);
        StartCoroutine(DestroyMuzzle(muzzle, 0.4f));

        GameObject projectile = GameObject.Instantiate(_projectile,
            _firePos.transform.position,
            _firePos.transform.rotation);
        projectile.GetComponent<Projectile_Barley>().TheStart(
            gameObject,
            _playerStats._damage,
            false,
            targetPosition,
            enemyTag
            );

        rotate = false;

    }

    override public void FireSkillAttack() {
        StartCoroutine(ActivateSkill());
    }

    IEnumerator ActivateSkill()
    {
        int shotCount = 0;
        string enemyTag;

        _audioSource.clip = _fireSound;


        Vector3 fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);
        Vector3[] landingPositions = new Vector3[5];
        Vector3 targetPosition = Vector3.zero;

        if (!_isCharacterAI)
        {
            PlayerController.instance.isActivatingSkill = true;
            enemyTag = "Competition";
            targetPosition = _circleIndicator.transform.position;
        }
        else
        {
            _AIController.isActivatingSkill = true;
            enemyTag = GetComponent<AIController>()._enemyTag;
            targetPosition = GetComponent<AIController>()._enemy.transform.position;
        }


        landingPositions[0] = targetPosition;
        landingPositions[1] = landingPositions[0] +
                    new Vector3(1, 0, 1);
        landingPositions[2] = landingPositions[0] +
                    new Vector3(-1, 0, 1);
        landingPositions[3] = landingPositions[0] +
                    new Vector3(1, 0, -1);
        landingPositions[4] = landingPositions[0] +
                    new Vector3(-1, 0, -1);

        _upperBodyDir = fireDir;
        rotate = true;

        while (shotCount < 5)
        {
            _audioSource.Play();

            GameObject muzzle = Instantiate(_muzzleEffect,
            _firePos.transform.position,
            _firePos.transform.rotation);
            StartCoroutine(DestroyMuzzle(muzzle, 0.4f));

            GameObject projectile = GameObject.Instantiate(_projectile,
                _firePos.transform.position,
                _firePos.transform.rotation);
            projectile.GetComponent<Projectile_Barley>().TheStart(
                gameObject,
                _playerStats._damage,
                false,
                landingPositions[shotCount],
                enemyTag
                );
            //Debug.Log(projectile.name + "  :  " + landingPositions[shotCount]);
            shotCount++;
            yield return new WaitForSeconds(0.2f);
        }

        if (!_isCharacterAI)
            PlayerController.instance.isActivatingSkill = false;
        else
            _AIController.isActivatingSkill = false;

        rotate = false;

    }

    IEnumerator DestroyMuzzle(GameObject muzzle, float destroyTime)
    {
        float startTime = 0;

        while (startTime <= destroyTime)
        {
            startTime += Time.deltaTime;

            muzzle.transform.localScale =
                new Vector3(1 - startTime / destroyTime,
                1 - startTime / destroyTime,
                1 - startTime / destroyTime);

            yield return null;
        }
        Destroy(muzzle);
    }

    //public IEnumerator DrawSkillIndicator()
    //{
    //    Vector3 fireDir;
    //    Vector3 endPoint;
    //    _shotIndicator.material = PlayerController.instance._skillIncatorMat;

    //    while (true)
    //    {
    //        fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);

    //        endPoint = transform.position + fireDir.normalized * _playerStats._skillRange;
    //        endPoint.y = 1;

    //        _shotIndicator.SetPosition(0, transform.position + new Vector3(0, 1, 0));
    //        _shotIndicator.SetPosition(1, endPoint);
    //        yield return null;
    //    }
    //}

    IEnumerator DrawAttackIndicator(bool isSkill)
    {
        Vector3 startPoint;

        Vector3[] linePoisitions = new Vector3[_shotIndicator.positionCount];
        float limitRange;
        if (isSkill)
            limitRange = GetComponent<PlayerStats>()._skillRange;
        else
            limitRange = GetComponent<PlayerStats>()._bulletRange;

        _shotIndicator.material = PlayerController.instance._attackIncatorMat;
        _circleIndicator.transform.position = transform.position;
        _landingPosition = Vector3.zero;

        _circleIndicator.SetActive(true);

        while (true)
        {
            startPoint = _firePos.transform.position;

            float magnitude = new Vector2(
                _fireDir.x / InGameMainUI.instance._resolutionWidthRatio,
                _fireDir.y / InGameMainUI.instance._resolutionHeightRatio).magnitude;
            magnitude *= _indicatorSensitivity;

            _landingPosition = new Vector3(_fireDir.normalized.x * magnitude,
                    0,
                    _fireDir.normalized.y * magnitude);

            if (magnitude >= limitRange)
            {
                _landingPosition = _landingPosition.normalized *  limitRange;
            }            
            _circleIndicator.transform.position = startPoint + _landingPosition + new Vector3(0,-1,0);

            // linerenderer positioning arc
            for (int i = 1; i <= _shotIndicator.positionCount; i++)
            {
                float ratio = _landingPosition.magnitude / limitRange;
                float _bullet_up_speed = 9.8f * ratio;
                ratio = ratio * i * 0.2f;
                float y = 1 + (_bullet_up_speed * ratio - 9.8f * ratio * ratio / 2);

                linePoisitions[i-1] = Vector3.Lerp(startPoint, _circleIndicator.transform.position, (float)i/10);
                linePoisitions[i-1].y = y;
                           
            }
            _shotIndicator.SetPositions(linePoisitions);
            yield return null;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colt : CharacterAttack {
    LineRenderer _shotIndicator;
    Coroutine _lineRenderingRoutine;


    // Use this for initialization
    private void Start()
    {
        base.Start();
        _shotIndicator = GetComponent<LineRenderer>();

    }

    private void Update()
    {
        if (!_isCharacterAI)
            _fireDir = PlayerController.instance._attackStickDir;
        else
            _fireDir = _AIController._attackStickDir;
    }

    override public void StartDrawIndicator(bool isSkill) {
        if (_isCharacterAI) return;

        if (_shotIndicator.enabled == false)
        {
            if (isSkill)
            {
                _shotIndicator.enabled = true;
                _lineRenderingRoutine = StartCoroutine(DrawSkillIndicator());
            }
            else
            {
                _shotIndicator.enabled = true;
                _lineRenderingRoutine = StartCoroutine(DrawAttackIndicator());
            }
        }
    }

    override public void StopDrawIndicator() {
        if (_isCharacterAI) return;

        if (_lineRenderingRoutine != null)
        {
            StopCoroutine(_lineRenderingRoutine);
            _shotIndicator.enabled = false;
        }
        else
        {
            _shotIndicator.enabled = false;
        }
    }

    override public void FireBaseAttack() {
        if (!_playerStats.onFire()) return;

        StartCoroutine(ActivateBaseAttack());
    }



    IEnumerator ActivateBaseAttack()
    {
        string enemyTag;

        _audioSource.clip = _fireSound;

        _animator.SetBool("Firing", true);
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

        int shotCount = 0;

        Vector3 fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);

        GameObject muzzle;
        GameObject projectile;

        _upperBodyDir = fireDir;
        rotate = true;

        while (shotCount < 4)
        {
            _audioSource.Play();
            muzzle = Instantiate(_muzzleEffect,
                  _firePos.transform.position + _firePos.transform.right *0.15f,
                  _firePos.transform.rotation);
            StartCoroutine(DestroyMuzzle(muzzle, 0.4f));

            projectile = GameObject.Instantiate(_projectile,
                _firePos.transform.position + _firePos.transform.right * 0.15f,
                _firePos.transform.rotation);
            projectile.GetComponent<Projectile_Colt>().TheStart(
                gameObject,
                _playerStats._bulletFastVelocity,
                _playerStats._damage
                , _playerStats._bulletRange
                , false
                , enemyTag
                );

            yield return new WaitForSeconds(0.02f);

            muzzle = Instantiate(_muzzleEffect,
                 _firePos.transform.position - _firePos.transform.right * 0.15f,
                 _firePos.transform.rotation);
            StartCoroutine(DestroyMuzzle(muzzle, 0.4f));

            projectile = GameObject.Instantiate(_projectile,
                _firePos.transform.position - _firePos.transform.right * 0.15f,
                _firePos.transform.rotation);
            projectile.GetComponent<Projectile_Colt>().TheStart(
                gameObject,
                _playerStats._bulletFastVelocity,
                _playerStats._damage
                , _playerStats._bulletRange
                , false
                , enemyTag
                );

            shotCount++;
            yield return new WaitForSeconds(0.02f);
        }
        if (!_isCharacterAI)
            PlayerController.instance.isActivatingSkill = false;
        else
            _AIController.isActivatingSkill = false;

        _animator.SetBool("Firing", false);
        rotate = false;

    }

    override public void FireSkillAttack()
    {
        StartCoroutine(ActivateSkill());
    }

    IEnumerator ActivateSkill()
    {
        _audioSource.clip = _fireSound;

        _animator.SetBool("Firing", true);

        int shotCount = 0;
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

        _upperBodyDir = fireDir;
        rotate = true;

        //Quaternion upperSpineOrginRot = _upperSpine.transform.rotation;
        //_upperSpine.transform.rotation = Quaternion.LookRotation(fireDir);

        while (shotCount < 8)
        {
            _audioSource.Play();

            muzzle = Instantiate(_muzzleEffect,
                  _firePos.transform.position + _firePos.transform.right * 0.15f,
                  _firePos.transform.rotation);
            StartCoroutine(DestroyMuzzle(muzzle, 0.4f));

            projectile = GameObject.Instantiate(_projectile,
                _firePos.transform.position + _firePos.transform.right * 0.15f,
                _firePos.transform.rotation);
            projectile.GetComponent<Projectile_Colt>().TheStart(
                gameObject,
                _playerStats._bulletFastVelocity,
                _playerStats._damage
                , _playerStats._skillRange
                , true
                , enemyTag
                );

            yield return new WaitForSeconds(0.02f);

            muzzle = Instantiate(_muzzleEffect,
                 _firePos.transform.position - _firePos.transform.right * 0.15f,
                 _firePos.transform.rotation);
            StartCoroutine(DestroyMuzzle(muzzle, 0.4f));

            projectile = GameObject.Instantiate(_projectile,
                _firePos.transform.position - _firePos.transform.right * 0.15f,
                _firePos.transform.rotation);
            projectile.GetComponent<Projectile_Colt>().TheStart(
                gameObject,
                _playerStats._bulletFastVelocity,
                _playerStats._damage
                , _playerStats._skillRange
                , true
                , enemyTag
                );

            shotCount++;
            yield return new WaitForSeconds(0.02f);
        }

        if (!_isCharacterAI)
            PlayerController.instance.isActivatingSkill = false;
        else
            _AIController.isActivatingSkill = false;
        _animator.SetBool("Firing", false);

        //_upperSpine.transform.rotation = upperSpineOrginRot;
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

    IEnumerator DrawAttackIndicator()
    {
        Vector3 startPoint;
        Vector3 fireDir;
        Vector3 endPoint;
        _shotIndicator.material = PlayerController.instance._attackIncatorMat;

        while (true)
        {
            startPoint = _firePos.transform.position;
            fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);
            RaycastHit hit;
            if (Physics.Raycast(startPoint, fireDir, out hit, _playerStats._bulletRange, LayerMask.NameToLayer("Grass")))
                endPoint = hit.point;
            else
                endPoint = startPoint + fireDir.normalized * _playerStats._bulletRange;

            endPoint.y = 1;
            _shotIndicator.SetPosition(0, startPoint);
            _shotIndicator.SetPosition(1, endPoint);
            yield return null;
        }
    }

    public IEnumerator DrawSkillIndicator()
    {
        Vector3 fireDir;
        Vector3 endPoint;
        _shotIndicator.material = PlayerController.instance._skillIncatorMat;

        while (true)
        {
            fireDir = new Vector3(_fireDir.x, 0, _fireDir.y);

            endPoint = transform.position + fireDir.normalized * _playerStats._skillRange;
            endPoint.y = 1;

            _shotIndicator.SetPosition(0, transform.position + new Vector3(0, 1, 0));
            _shotIndicator.SetPosition(1, endPoint);
            yield return null;
        }
    }
}

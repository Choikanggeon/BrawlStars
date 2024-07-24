using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterAttack : MonoBehaviour {
    public GameObject _upperSpine;
    protected Vector3 _fireDir;
    protected PlayerStats _playerStats;
    protected GameObject _firePos;

    public GameObject _projectile;
    public GameObject _muzzleEffect;

    protected Animator _animator;
    protected Vector3 _upperBodyDir;
    protected bool rotate = false;

    protected bool _isCharacterAI;
    protected AIController _AIController;

    public AudioClip _fireSound;
    protected AudioSource _audioSource;
    
    protected void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _playerStats = GetComponent<PlayerStats>();
        _animator = GetComponentInChildren<Animator>();
        if (gameObject.CompareTag("Player"))
        {
            _isCharacterAI = false;
            _firePos = PlayerController.instance._firePos;
        }
        else
        {
            _isCharacterAI = true;
            _AIController = GetComponent<AIController>();
            _firePos = _AIController._firePos;
        }
    }

    private void LateUpdate()
    {
        if (rotate)
        {
            if (_upperBodyDir == Vector3.zero) return;
            Vector3 spineRot = Quaternion.LookRotation(_upperBodyDir).eulerAngles;
            spineRot -= transform.eulerAngles;

            _upperSpine.transform.localRotation = Quaternion.Euler(
                _upperSpine.transform.localEulerAngles.x + spineRot.y,
                _upperSpine.transform.localEulerAngles.y,
                _upperSpine.transform.localEulerAngles.z
                );
        }
    }

    virtual public void StartDrawIndicator(bool isSkill)   {    }

    virtual public void StopDrawIndicator()    {    }

    virtual public void FireBaseAttack()   {    }

    virtual public void FireSkillAttack()    {    }
}

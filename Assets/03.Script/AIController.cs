using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    Animator _animator;
    CharacterAttack _characterAttack;
    PlayerStats _playerStats;
    NavMeshAgent _agent;

    public Vector3 _attackStickDir;
    public bool isActivatingSkill;
    public GameObject _firePosParent;
    public GameObject _firePos;
    MapAllocator _mapAllocator;

    GameObject _enemyInGrass;
    public GameObject _enemy;
    public GameObject _energyBox;

    public string _tag = null;
    public string _enemyTag = null;
    public Vector3 _LastSeenPosition { get; private set; }

    public bool _detected; // Player가 GrassDetect로 set 해주는 변수

    Vector3[] _avoidEnemy = new Vector3[2];

    // 발사 대기시간을 위한 변수
    private float lastFireTime;
    private float fireCooldown = 0.2f; // 0.2초간 대기시간

    // Start is called before the first frame update
    void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _characterAttack = GetComponent<CharacterAttack>();
        _animator = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = true;
        _agent.updatePosition = true;

        _tag = gameObject.tag;
        if (_tag.CompareTo("Competition") == 0)
            _enemyTag = "Competition";

        _mapAllocator = MapAllocator.instance;
        if (_mapAllocator != null)
        {
            int characterIdx = _mapAllocator._selectedCharacterIdx;
            GameObject playerPos = _mapAllocator._playerPos[characterIdx];

            // playerPos에 따라 다른 행동을 수행하는 switch 문
            switch (characterIdx)
            {
                case 0:
                    _LastSeenPosition = new Vector3(286, 0, 60);
                    break;
                case 1:
                    _LastSeenPosition = new Vector3(286, 0, 60);
                    break;
                case 2:
                    _LastSeenPosition = new Vector3(268, 0, 189);
                    break;
                case 3:
                    _LastSeenPosition = new Vector3(239, 0, 224);
                    break;
                case 4:
                    _LastSeenPosition = new Vector3(159, 0, 195);
                    break;
                case 5:
                    _LastSeenPosition = new Vector3(114, 0, 190);
                    break;
                case 6:
                    _LastSeenPosition = new Vector3(60, 0, 75);
                    break;
                case 7:
                    _LastSeenPosition = new Vector3(113, 0, 60);
                    break;
                case 8:
                    _LastSeenPosition = new Vector3(165, 0, -3);
                    break;
                case 9:
                    _LastSeenPosition = new Vector3(210, 0, 40);
                    break;
            }
        }

        _agent.SetDestination(_LastSeenPosition);
        _detected = false;

        _avoidEnemy[0] = new Vector3(10, 0, 16);
        _avoidEnemy[1] = new Vector3(10, 0, -16);

        // 초기 발사 시간을 0으로 설정
        lastFireTime = 0f;
    }

    private void FixedUpdate()
    {
        // OnTriggerStay 이벤트가 FixedUpdate보다 늦게 호출되기 때문에
        // 여기서 무조건 true로 해주고, OnTriggerStay에서 다시 한번 확인한다.
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        _playerStats._playerInfoCanvas.GetComponent<Canvas>().enabled = true;
    }



    // Update is called once per frame
    void Update()
    {
        // 적이 할당되어 있다면, _firePosParent 를 적의 방향으로 회전시켜야함
        // isActivatingSkill == true 라는 것은 공격하는 중,
        // 브롤스타즈 특성상 공격중에는 방향이 바뀌지 않음
        if (_enemy && !isActivatingSkill)
        {
            _attackStickDir = _enemy.transform.position - transform.position;
            _attackStickDir = new Vector3(_attackStickDir.x, _attackStickDir.z, 0);
            _firePosParent.transform.rotation = Quaternion.LookRotation(_attackStickDir);
        }
        else if (_attackStickDir != Vector3.zero)
        {
            _firePosParent.transform.rotation = Quaternion.LookRotation(_attackStickDir);
        }

        if (_enemy != null)
        {
            if (Vector3.Distance(transform.position, _enemy.transform.position) <
                _playerStats._bulletRange)
            {
                if (_playerStats._skillFireReady)
                    FireSkillAttack();
                else
                    FireBasicAttack();
            }
        }

        if (_playerStats._health <= 1000)
        {
            if (_enemy != null && _enemy.GetComponent<PlayerStats>()._health <=
                _playerStats._health)
            {
                if (!_enemy.GetComponent<PlayerStats>()._isCharacterInGrass)
                {
                    _agent.stoppingDistance = _playerStats._bulletRange - 2;
                    _agent.SetDestination(_enemy.transform.position);
                    _LastSeenPosition = _enemy.transform.position;
                }
                else
                    _enemy = null;
            }
            else if (gameObject.CompareTag("Company"))
                _agent.SetDestination(_avoidEnemy[1]);
            else if (gameObject.CompareTag("Competition"))
                _agent.SetDestination(_avoidEnemy[0]);
        }
        else if (_energyBox != null && _enemy != null)
        {
            float distanceTarget = Vector3.Distance(transform.position, _energyBox.transform.position);
            float distanceEnemy = Vector3.Distance(transform.position, _enemy.transform.position);
            if (distanceTarget < distanceEnemy)
            {
                _agent.stoppingDistance = 0.5f;
                _agent.SetDestination(_energyBox.transform.position);
            }
            else
            {
                if (!_enemy.GetComponent<PlayerStats>()._isCharacterInGrass)
                {
                    _agent.stoppingDistance = _playerStats._bulletRange - 2;
                    _agent.SetDestination(_enemy.transform.position);
                    _LastSeenPosition = _enemy.transform.position;
                }
                else
                    _enemy = null;
            }
        }
        else if (_energyBox != null) // 에너지 상자만 할당되었을 때
        {
            _agent.stoppingDistance = 0.5f;
            _agent.SetDestination(_energyBox.transform.position);
        }
        else if (_enemy != null) // 적만 할당되었을 때
        {
            if (!_enemy.GetComponent<PlayerStats>()._isCharacterInGrass)
            {
                _agent.stoppingDistance = _playerStats._bulletRange - 2;
                _agent.SetDestination(_enemy.transform.position);
                _LastSeenPosition = _enemy.transform.position;
            }
            else
                _enemy = null;
        }
        else if (_LastSeenPosition != Vector3.zero)
        {
            _agent.stoppingDistance = 0.5f;
            _agent.SetDestination(_LastSeenPosition);
        }
        else
        {
            _agent.SetDestination(new Vector3(0, 0, 0));
        }

        // 애니메이터의 변수를 설정
        if (_agent.velocity.magnitude >= 0.1f)
        {
            _animator.SetBool("isMoving", true);
            _animator.SetFloat("MoveSpeed", _agent.velocity.magnitude / _agent.speed);
        }
        else
        {
            _animator.SetBool("isMoving", false);
            _animator.SetFloat("MoveSpeed", _agent.velocity.magnitude / _agent.speed);
        }
    }

    public void FireBasicAttack()
    {
        _characterAttack.FireBaseAttack();
    }

    public void FireSkillAttack()
    {
        _playerStats.InitializeSkillGage();
        _characterAttack.FireSkillAttack();
    }

    private void OnTriggerStay(Collider other)
    {
        // AI 자신의 몸을 숨기는 기능
        if (other.CompareTag("Grass") && gameObject.CompareTag("Competition") && !_detected)
        {
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            _playerStats._playerInfoCanvas.GetComponent<Canvas>().enabled = false;
        }
    }

    private void OnEnable()
    {
        isActivatingSkill = false;
        _enemy = null;
        _LastSeenPosition = Vector3.zero;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class PlayerStats : MonoBehaviour {
    public GameObject _playerInfoCanvas;

    Vector3 _offset;
    Vector3 _rot;
    public bool _isCharacterInGrass;
    public GameObject _ExplosionRedPrefab;
    public GameObject _ExplosionBluePrefab;
    public GameObject _EnergyPrefab;
    public Vector3 _spawnPosition;
    public GameObject _crystal;
    public AudioClip _dyingSound;
    protected AudioSource _audioSource;

    float _lastHitTime;
    Coroutine _healthReproductionRoutine;
    Coroutine _reloadRoutine;

    public GameObject _shieldSphere;
    bool _invincible;

    public CharacterInfo _characterInfo;
    #region Character Information
    //public FightType _fightType { get; private set;}
    public CharacterAttack _attackScript { get; private set; }
    public FightType _fightType { get; private set; }
    public int _health { get; private set; }
    public int _damage { get; private set; }
    public float _bulletFastVelocity { get; private set; }
    public float _bulletSlowVelocity { get; private set; }
    public float _bulletRange { get; private set; }
    public int _ammo { get; private set; }
    public float _reloadTime { get; private set; }
    public int _energyNum { get; private set; }
    private bool _isCorrected = false;

    public float _skillEnergyLimit { get; private set; }
    public float _skillRange { get; private set; }
    public float _skillDamage { get; private set; }
    #endregion

    #region OnGame Information
    public string _name;

    public int _score { get; private set; }
    int _currentAmmo;
    bool _reload;
    float _currentReloadAmount;
    public bool _skillFireReady { get; private set; }
    public float _skillEnergy { get; private set; }
    #endregion

    #region PlayerInfo Canvas
    GameObject _healthBar;
    GameObject _healthTxt;
    GameObject _ammoBar;
    GameObject _reloadBar;
    GameObject _ID;
    GameObject _Energy;
    GameObject _EnergyTxt;
    GameObject _downArrow;
    #endregion

    #region Skill UI
    public Sprite _skillBG_Before;
    public Sprite _skillBG_After;
    public Sprite _skillImg_Before;
    public Sprite _skillImg_After;
    #endregion
    

    private void OnEnable () {
        _offset = new Vector3(0, 15f, 2f);
        if (gameObject.CompareTag("EnergyBox"))
            _offset = new Vector3(-4.5f, 7.5f, 14f);
        _rot = new Vector3(68, 0, 0);
        _score = 0;
        _isCharacterInGrass = false;
        _invincible = false;
        _audioSource = GetComponent<AudioSource>();

        #region CharacterInformation Setting      
        _attackScript = GetComponent<CharacterAttack>();

        _fightType = _characterInfo._fightType;
        _health = _characterInfo._health;
        _damage = _characterInfo._damage;
        _bulletFastVelocity = _characterInfo._bulletfastVelocity;
        _bulletRange = _characterInfo._bulletRange;
        _ammo = _characterInfo._ammo;
        _reloadTime = _characterInfo._reloadTime;
        _energyNum = _characterInfo._energyNum;

        _skillEnergyLimit = _characterInfo._skillEnergyLimit;
        _skillRange = _characterInfo._skillRange;
        _skillDamage = _characterInfo._skillDamage;

        #endregion

        #region PlayerInfo Canvas Setting
        if (gameObject.CompareTag("Player"))
            _playerInfoCanvas = Resources.Load<GameObject>("UI/PlayerUI");
        /*else if (gameObject.CompareTag("Company"))
            _playerInfoCanvas = Resources.Load<GameObject>("UI/CompanyUI");*/
        else if (gameObject.CompareTag("Competition"))
            _playerInfoCanvas = Resources.Load<GameObject>("UI/EnemyUI");
        else if (gameObject.CompareTag("EnergyBox"))
        {
            _playerInfoCanvas = Resources.Load<GameObject>("UI/EnergyBoxUI");
            
        }
        _playerInfoCanvas = Instantiate(_playerInfoCanvas);
        
        _playerInfoCanvas.GetComponent<RectTransform>().position =
    transform.position + _offset;
        _playerInfoCanvas.GetComponent<RectTransform>().localScale = new Vector3(0.3f, 0.3f, 0.3f);
        _playerInfoCanvas.GetComponent<RectTransform>().rotation = Quaternion.Euler(_rot);

        _healthBar = _playerInfoCanvas.GetComponent<PlayerUI>().healthBar;
        _healthTxt = _playerInfoCanvas.GetComponent<PlayerUI>().healthTxt;
        _ammoBar = _playerInfoCanvas.GetComponent<PlayerUI>().ammoBar;
        _reloadBar = _playerInfoCanvas.GetComponent<PlayerUI>().reloadBar;
        _ID = _playerInfoCanvas.GetComponent<PlayerUI>().ID;
        _downArrow = _playerInfoCanvas.GetComponent<PlayerUI>().downArrow; 
        _Energy = _playerInfoCanvas.GetComponent<PlayerUI>().energy;
        if (_Energy != null)
        {
            _Energy.SetActive(false);
        }
        _EnergyTxt = _playerInfoCanvas.GetComponent<PlayerUI>().energyTxt;

        #endregion
        _reload = false;
        _currentAmmo = _ammo;
        _currentReloadAmount = 1f;
        _skillEnergy = 0;
        _skillFireReady = false;

        _lastHitTime = 0f;
    }

    // Update is called once per frame
    void Update () {
        _playerInfoCanvas.GetComponent<RectTransform>().position =
    transform.position + _offset;
    }

    IEnumerator HealthReproduct()
    {
        yield return new WaitForSeconds(3f);
        while(_health < _characterInfo._health)
        {
            _health += 200;
            if (_health >= _characterInfo._health)
                _health = _characterInfo._health;
            _healthBar.GetComponent<Image>().fillAmount = (float)_health / _characterInfo._health;            
            _healthTxt.GetComponent<Text>().text =
                (int.Parse(_healthTxt.GetComponent<Text>().text) + 200).ToString();
            
            yield return new WaitForSeconds(1f);
        }

        _healthReproductionRoutine = null;
    }

    private void FixedUpdate()
    {
        _isCharacterInGrass = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Grass"))
        {
            _isCharacterInGrass = true;
        }
    }
 

    public bool onFire()
    {
        if (_currentAmmo <= 0) return false;
        if (_reloadRoutine != null)
            StopCoroutine(_reloadRoutine);
        _currentAmmo--;

        _currentReloadAmount -= (float)1 / _ammo;
        if (gameObject.CompareTag("Player"))
        {
            _ammoBar.GetComponent<Image>().fillAmount = _currentReloadAmount;
            _reloadBar.GetComponent<Image>().fillAmount = _currentReloadAmount;
        }
        _reload = true;
        //StopAllCoroutines();
        _reloadRoutine = StartCoroutine(Reload());

        return true;
    }
    
    IEnumerator Reload()
    {
        float startTime = Time.time;
        Image reloadBarImage = null;
        if (gameObject.CompareTag("Player"))
            reloadBarImage = _reloadBar.GetComponent<Image>();
        float _reloadamount = _currentReloadAmount;
        while (_currentAmmo < _ammo)
        {
            _currentReloadAmount = _reloadamount + (Time.time - startTime) / ((float)_reloadTime * _ammo);

            if (gameObject.CompareTag("Player"))
                reloadBarImage.fillAmount = _currentReloadAmount;

            int i = 0; 
            while(i <= _ammo)
            {
                if(_currentReloadAmount >= (float)(_ammo-i) / _ammo)
                {
                    _currentAmmo = _ammo - i;
                    if(gameObject.CompareTag("Player"))
                        _ammoBar.GetComponent<Image>().fillAmount = (float)(_ammo - i) / _ammo;
                    break;
                }
                i++;
            }
            yield return null;
        }
    }

    public void HitCompetition(GameObject gameObject)
    {

        _skillEnergy += _damage;
        if(_skillEnergy >= _skillEnergyLimit)
        {
            if (this.gameObject.CompareTag("Player"))
            {
                SkillCanvas.instance._gage.GetComponent<Image>().enabled = false;
                SkillCanvas.instance._outterCircle.GetComponent<Image>().sprite =
                    _skillBG_After;
                SkillCanvas.instance._innerCircle.GetComponent<Image>().sprite =
                    _skillImg_After;
                SkillCanvas.instance._innerCircle.GetComponent<Image>().raycastTarget = true;
            }
            _skillFireReady = true;
        }
        if (this.gameObject.CompareTag("Player"))
        {
            SkillCanvas.instance._gage.GetComponent<Image>().fillAmount =
                (float)_skillEnergy / _skillEnergyLimit;
        }

        _lastHitTime = Time.time;

    }

    public void InitializeSkillGage()
    {
        _skillEnergy = 0;
        _skillFireReady = false;

        if (!gameObject.CompareTag("Player")) return;
        SkillCanvas.instance._gage.GetComponent<Image>().enabled = true;
        SkillCanvas.instance._gage.GetComponent<Image>().fillAmount = 0;
        SkillCanvas.instance._innerCircle.GetComponent<Image>().raycastTarget = false;

        SkillCanvas.instance._outterCircle.GetComponent<Image>().sprite =
                _skillBG_Before;
        SkillCanvas.instance._innerCircle.GetComponent<Image>().sprite =
            _skillImg_Before;
    }

    IEnumerator SetPlayerInfo()
    {
        _playerInfoCanvas.SetActive(true);
        yield return new WaitForSeconds(3f);
        _playerInfoCanvas.SetActive(false);

    }

    public void EnergyUp()
    {
        _characterInfo._health += 400;
        _health += 400;
        _damage += Mathf.RoundToInt((float)_damage/10);
        _healthTxt.GetComponent<Text>().text =
                (int.Parse(_healthTxt.GetComponent<Text>().text) + 400).ToString();
        _Energy.SetActive(true);

        // 텍스트가 비어 있거나 null인지 확인 후 파싱
        int energyCount = 0;
        if (!string.IsNullOrEmpty(_EnergyTxt.GetComponent<Text>().text))
        {
            energyCount = int.Parse(_EnergyTxt.GetComponent<Text>().text);
        }
        _EnergyTxt.GetComponent<Text>().text = (energyCount + 1).ToString();
        _EnergyTxt.SetActive(true);
    }


    public void HitByPlayerProjectile(int damage)
    {
        if (_invincible) return;

        if (_healthReproductionRoutine != null)
        {
            StopCoroutine(_healthReproductionRoutine);
        }

        _health -= damage;
        _healthBar.GetComponent<Image>().fillAmount = (float)_health / _characterInfo._health;
        _healthTxt.GetComponent<Text>().text = (_health).ToString();
        //공격을 받으면 실행
        if (_health > 0)
        {
            if(gameObject.CompareTag("EnergyBox"))
            StartCoroutine(SetPlayerInfo());
        }
        
        if (_health <= 0)
        {
            if (gameObject.CompareTag("Player") || gameObject.CompareTag("Company"))
            {
                _audioSource.clip = _dyingSound;
                _audioSource.Play();
                GameObject particle = Instantiate(_ExplosionRedPrefab, transform.position, transform.rotation);
                particle.GetComponent<ParticleSystem>().Play();
            }else if (gameObject.CompareTag("Competition"))
            {
                _audioSource.clip = _dyingSound;
                _audioSource.Play();
                GameObject particle = Instantiate(_ExplosionBluePrefab, transform.position, transform.rotation);
                particle.GetComponent<ParticleSystem>().Play();
            }
            else if (gameObject.CompareTag("EnergyBox"))
            {
                if (_isCorrected) return;
                _isCorrected = true;
                GameObject particle = Instantiate(_ExplosionBluePrefab, transform.position, transform.rotation);
                particle.GetComponent<ParticleSystem>().Play();
                GameObject Energy = Instantiate(_EnergyPrefab, transform.position + Vector3.up*5 + Vector3.left*5 + Vector3.forward*11, transform.rotation);
            }
            StartCoroutine(SetPlayerCanvasActive(false));
        }

        _healthReproductionRoutine = StartCoroutine(HealthReproduct());

    }
    IEnumerator SetPlayerCanvasActive(bool active)
    {
        yield return new WaitForSeconds(0.1f);
        _playerInfoCanvas.SetActive(active);
        if(gameObject.CompareTag("EnergyBox"))
            Destroy(gameObject);
    }
    private void Start()
    {
        if(_playerInfoCanvas != null)
        _playerInfoCanvas.SetActive(true);
        if (_healthBar == null) return;
        _health = _characterInfo._health;
        _healthBar.GetComponent<Image>().fillAmount = 1;
        _healthTxt.GetComponent<Text>().text = _health.ToString();

        _reload = false;
        _currentAmmo = _ammo;
        _currentReloadAmount = 1f;

        if (gameObject.CompareTag("Player"))
        {
            _ammoBar.GetComponent<Image>().fillAmount = _currentReloadAmount;
            StartCoroutine(MoveArrow());
        }
        if (gameObject.CompareTag("EnergyBox"))
            _playerInfoCanvas.SetActive(false);
        

        _invincible = true;
        StartCoroutine(StartShield());
    }

    IEnumerator StartShield()
    {
        _shieldSphere.SetActive(true);
        yield return new WaitForSeconds(10.0f);

        _invincible = false;
        _shieldSphere.SetActive(false);
    }

    IEnumerator MoveArrow()
    {
        _downArrow.SetActive(true);
        Vector3 endPos = _downArrow.transform.position + 3 * Vector3.up;
        float timeElapsed = 0.0f;
        while (timeElapsed < 3f)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / 3f;
            _downArrow.transform.position = Vector3.Lerp(_downArrow.transform.position, endPos, Mathf.PingPong(t, 1));
            yield return null;
        }
        _downArrow.SetActive(false);
    }

    
}

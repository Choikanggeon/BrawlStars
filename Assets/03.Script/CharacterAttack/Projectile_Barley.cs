using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Barley : Projectile{
    float _gravity ;
    float _forward_velovity ;
    float _bullet_up_speed;
    float _landingTimeMax;
    float _landingTime;
    float _startTime ;
    //float _yPos;

    bool _launch = false;

    public float _rotSpeed = 10;
    Quaternion targetRotation;

    Transform _meshTr;

    Vector3 _startPosition;
    Vector3 _landingPosition;
    Vector3 _endPos;

    float _length;

    public GameObject _attackPlane;

    public void TheStart(GameObject caller, int damage, bool isSkill, Vector3 landingPosition , string enemyTag)
    {
        _caller = caller;
        _damage = damage; _isSkill = isSkill; 
        _landingPosition = landingPosition;
        _enemyTag = enemyTag; 

        _gravity = 9.8f;
        _landingTimeMax = 2f;
        _forward_velovity = PlayerController.instance._playerStats._bulletRange / _landingTimeMax;
        _startPosition = transform.position;

        _length = (landingPosition - (transform.position + new Vector3(0,-1,0))).magnitude;  // use rigidbody

        _landingTime = _length / PlayerController.instance._playerStats._bulletRange * _landingTimeMax;  // use rigidbody
        // _bullet_up_speed extracted from -1 = integration(0-_landingTime) Vo - 9.8t dt  
        // Vo == _bullet_up_speed
        _bullet_up_speed = _gravity / 2 * _landingTime - 1f / _landingTime;  // use rigidbody
        
        _startTime = Time.time;

        _meshTr = _bulletMeshObj.GetComponent<Transform>();
        targetRotation = _meshTr.rotation;

        _hit = false;
        _launch = true;
    }

    private void FixedUpdate()
    {
        if (_launch)
        {
            _launch = false;
            GetComponent<Rigidbody>().useGravity = true;
            Vector3 forwardForce = (_landingPosition - transform.position).normalized * _forward_velovity;
            GetComponent<Rigidbody>().AddForce(new Vector3(forwardForce.x, _bullet_up_speed, forwardForce.z), ForceMode.VelocityChange);
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().useGravity = false;
            StartCoroutine(DestroySelf(1.0f));
        }
    }

    // Update is called once per frame
    void Update() {
        if (_hit) return;

        targetRotation *= Quaternion.AngleAxis(10, Vector3.left);
        _meshTr.rotation =
            Quaternion.Lerp(_meshTr.rotation,
            targetRotation,
            10 * _rotSpeed * Time.deltaTime);
    }

    protected override IEnumerator DestroySelf(float time)
    {
        _hit = true;

        _bulletMeshObj.GetComponent<Renderer>().enabled = false;
        _explosionPlarticle.GetComponent<ParticleSystem>().Play();
        _trail.GetComponent<ParticleSystem>().Stop();

        Vector3 initPos = new Vector3(transform.position.x, 0.1f, transform.position.z);
        GameObject plane = GameObject.Instantiate(_attackPlane,
            initPos,
            Quaternion.Euler(0, 0, 0)
            );
        plane.GetComponent<BarleyPlaneAttack>().TheStart(_caller, _damage, _enemyTag);

        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}

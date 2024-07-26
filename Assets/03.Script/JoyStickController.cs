using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoyStickController : MonoBehaviour
{
    public GameObject _innerCircle;
    public GameObject _outterCircle;
    SkillAttack SkillAttack;
    PlayerController _playerController;
    public static bool skillButtonClicked = false;

    bool aiming = false;
    int fingerID = -1;

    public bool _readToFire { get; protected set; }
    public Vector3 _stickDir { get; protected set; }

    // Use this for initialization
    protected void Start()
    {
        _playerController = PlayerController.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (!aiming) return;

        if (Input.GetMouseButton(0))
        {
            HandleInput(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopControlling();
        }
    }

    private void HandleInput(Vector2 position)
    {
        gameObject.SetActive(true);
        _innerCircle.transform.position = position;
        _stickDir = _innerCircle.transform.position - _outterCircle.transform.position;

        float outterRadius = (_outterCircle.GetComponent<RectTransform>().rect.width / 2) *
            InGameMainUI.instance._resolutionWidthRatio;

        if (_stickDir.magnitude < outterRadius / 3)
        {
            if (fingerID == PlayerController.instance._right_touch_id)
                PlayerController.instance._isReadyToFire = false;
            else if (fingerID == PlayerController.instance._left_touch_id)
                PlayerController.instance._isReadyToRun = false;
        }
        else if (_stickDir.magnitude < outterRadius)
        {
            _innerCircle.transform.position = position;
            if (fingerID == PlayerController.instance._right_touch_id)
            {
                PlayerController.instance._isReadyToFire = true;
                PlayerController.instance._attackStickDir = _stickDir;
            }
            else if (fingerID == PlayerController.instance._left_touch_id)
                PlayerController.instance._isReadyToRun = true;
        }
        else
        {
            if (fingerID == PlayerController.instance._right_touch_id)
            {
                PlayerController.instance._isReadyToFire = true;
                PlayerController.instance._attackStickDir = _stickDir;
            }
            else if (fingerID == PlayerController.instance._left_touch_id)
                PlayerController.instance._isReadyToRun = true;

            Vector2 dirNormalized = _stickDir.normalized * outterRadius;
            Vector2 newPos = new Vector2(_outterCircle.transform.position.x + dirNormalized.x,
                _outterCircle.transform.position.y + dirNormalized.y);
            _innerCircle.transform.position = newPos;
        }
    }

    public void StartControlling(int fingerID)
    {
        if (aiming) return;
        aiming = true;
        this.fingerID = fingerID;

        Vector2 position = _playerController._attackStartPos;

        if (Input.touchCount > 0)
        {
            // 터치 입력일 때
            if (fingerID >= 0 && fingerID < Input.touches.Length)
            {
                _innerCircle.GetComponent<Transform>().position = Input.touches[fingerID].position;
                _outterCircle.GetComponent<Transform>().position = Input.touches[fingerID].position;
            }
            else
            {
                Debug.LogError("Invalid fingerID: " + fingerID);
                aiming = false; // aiming 초기화
                return;
            }
        }
        else
        {
            // 마우스 입력일 때
            _innerCircle.GetComponent<Transform>().position = Input.mousePosition;
            _outterCircle.GetComponent<Transform>().position = Input.mousePosition;
        }

        _innerCircle.SetActive(true);
        _outterCircle.SetActive(true);
        
    }

    public void StopControlling()
    {
        aiming = false;
        this.fingerID = -1;

        _innerCircle.SetActive(false);
        _outterCircle.SetActive(false);

    }
}

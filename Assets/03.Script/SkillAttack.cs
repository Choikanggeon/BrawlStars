using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillAttack : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    Vector2 _startPos;
    PlayerStats _playerStats;
    bool _skillFire;
    bool _baseAttckFire;
    GameObject _outterCircle;
    public bool _isControlling { get; private set; }
    // Use this for initialization
    void Start () {
        _startPos = transform.position;
        _playerStats = PlayerController.instance._playerStats;
        _skillFire = false;
        _baseAttckFire = false;
        _isControlling = false;

        _outterCircle = SkillCanvas.instance._outterCircle;

        GetComponent<Image>().raycastTarget = false;
        
	}

    public void OnDrag(PointerEventData data)
    {
        if (!_isControlling) return;

        Vector2 stickDir = data.position - _startPos;

        float outterRadius = (_outterCircle.GetComponent<RectTransform>().rect.width / 2) *
              InGameMainUI.instance._resolutionWidthRatio;

        Vector3 dir = new Vector3(stickDir.x, 0, stickDir.y);
        PlayerController.instance._firePosParent.transform.rotation = Quaternion.LookRotation(dir);

        if (stickDir.magnitude < outterRadius / 3)
        {
            _skillFire = false;
            PlayerController.instance._characterAttack.StopDrawIndicator();
        }
        else if (stickDir.magnitude < outterRadius)
        {
            transform.position = data.position;
            _skillFire = true;
            PlayerController.instance.DrawSkillIndicator();
            PlayerController.instance._attackStickDir = stickDir;
        }
        else
        {
            _skillFire = true;
            PlayerController.instance.DrawSkillIndicator();
            PlayerController.instance._attackStickDir = stickDir;

            Vector2 dirNormalized = stickDir.normalized * outterRadius;
            Vector2 newPos = new Vector2(_startPos.x + dirNormalized.x,
                _startPos.y + dirNormalized.y);
            transform.position = newPos;

            //PlayerController.instance.DrawSkillIndicator();
            PlayerController.instance._attackStickDir = stickDir;
            _skillFire = true;
        }

    }

    public void OnPointerUp(PointerEventData data)
    {
        if (!_isControlling) return;

        _isControlling = false; 
        transform.position = _startPos;
        
        if (_skillFire)
             PlayerController.instance.FireSkillAttack();
        PlayerController.instance._attackStickDir = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData data) {
        if(!PlayerController.instance._shotIndicator.enabled)
            _isControlling = true;
    }
}

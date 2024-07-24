using TMPro;
using UnityEngine;
using System.Collections;
public class InGameMainUI : MonoBehaviour
{
    #region Singleton
    public static InGameMainUI instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }
    #endregion

    public GameObject _AttackStick_Panel;
    public GameObject _MovementStick_Panel;
    public GameObject _SkillSitck_Panel;
    public GameObject _StartText;
    public GameObject _PlayerInfoUp;
    public GameObject _PlayerInfoDown;
    public GameObject _MachStartTime;
    public GameObject _BrawlTextImg;
    public AudioClip _Brawlintro;

    public JoyStickController AttackStick { get; private set; }
    public JoyStickController MovementStick { get; private set; }
    public float _resolutionWidthRatio { get; private set; }
    public float _resolutionHeightRatio { get; private set; }
    public bool _isCameraToPlayer = false;

    void Start()
    {
        AttackStick = _AttackStick_Panel.GetComponent<JoyStickController>();
        MovementStick = _MovementStick_Panel.GetComponent<JoyStickController>();

        _resolutionWidthRatio = (float)Screen.currentResolution.width / 1280;
        _resolutionHeightRatio = (float)Screen.currentResolution.width / 800;
        StartCoroutine(StartTextOn());
        StartCoroutine(StartPlayerInfo());
        StartCoroutine(StartBrawlMessage());
    }

    void Update()
    {
    }

    IEnumerator StartTextOn()
    {
        yield return new WaitForSeconds(0.5f);
        _StartText.SetActive(true);
        yield return new WaitForSeconds(4f);
        _StartText.SetActive(false);
    }

    IEnumerator StartPlayerInfo()
    {
        yield return new WaitForSeconds(5f);
        _MachStartTime.SetActive(true);
        _PlayerInfoUp.SetActive(true);
        _PlayerInfoDown.SetActive(true);
        StartCoroutine(MatchTimeCount());
        float startTime = 0f;
        Vector3 InfoUpStartPos = _PlayerInfoUp.GetComponent<RectTransform>().position;
        Vector3 InfoDownStartPos = _PlayerInfoDown.GetComponent<RectTransform>().position;
        Vector3 InfoEndPos = Vector3.zero;

        // 새로운 음악 재생
        if (GameManagerScript.instance != null)
        {
            GameManagerScript.instance.ChangeBackgroundMusic(_Brawlintro);
        }

        while (startTime <= 0.3f)
        {
            startTime += Time.deltaTime;
            _PlayerInfoUp.transform.position = Vector3.Lerp(InfoUpStartPos, InfoEndPos, startTime / 0.3f);
            _PlayerInfoDown.transform.position = Vector3.Lerp(InfoDownStartPos, InfoEndPos, startTime / 0.3f);
            yield return null;
        }

        yield return new WaitForSeconds(4f);
        _PlayerInfoUp.SetActive(false);
        _PlayerInfoDown.SetActive(false);
        _isCameraToPlayer = true;
    }

    IEnumerator MatchTimeCount()
    {
        int matchstartTime = 3;
        float elapsedTime = 0f;

        Transform text_MatchStartTime = _MachStartTime.transform.Find("Text_MatchStartTimeText");
        var matchTime = text_MatchStartTime.gameObject.GetComponent<TextMeshProUGUI>();

        while (matchstartTime > 0)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= 1f)
            {
                elapsedTime = 0f;
                matchstartTime--;
                matchTime.text = matchstartTime.ToString();
            }

            yield return null;
        }

        matchTime.text = "0";
        _MachStartTime.SetActive(false);
    }

    IEnumerator StartBrawlMessage()
    {
        yield return new WaitForSeconds(10f);
        _BrawlTextImg.SetActive(true);
        yield return new WaitForSeconds(1f);
        _BrawlTextImg.SetActive(false);
    }
}

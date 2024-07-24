using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    #region Singleton
    public static LobbyManager instance = null;

    public int _selectedCharacterIdx;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }
    #endregion

    public GameObject[] _characters = new GameObject[3];
    GameObject _selectedCharacter;
    int _selectedIdx;
    int _maxCharacterNum;
    // Start is called before the first frame update
    void Start()
    {
        _maxCharacterNum = 3;
        _selectedIdx = 0;
        _selectedCharacter = GameObject.Instantiate(_characters[_selectedIdx]);
        _selectedCharacter.transform.position = new Vector3(0, -1, 3.1f);
        _selectedCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);
    }
    
    public void SelectRightCharacter()
    {
        if (_selectedIdx + 1 >= _maxCharacterNum) return;
        _selectedIdx++;
        GameManagerScript.instance._selectedCharacterIdx = _selectedIdx;
        StartCoroutine(SlideCharacterToLeft());
    }

    public void SelectLeftCharacter()
    {
        if (_selectedIdx - 1 < 0) return;
        _selectedIdx--;
        GameManagerScript.instance._selectedCharacterIdx = _selectedIdx;
        StartCoroutine(SlideCharacterToRight());

    }

    IEnumerator SlideCharacterToLeft()
    {
        float startTime = 0;

        GameObject newSelectedCharacter = Instantiate(_characters[_selectedIdx]);
        newSelectedCharacter.transform.position = new Vector3(30, -1, 3.1f);
        newSelectedCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);

        Vector3 selectedStartPos = _selectedCharacter.transform.position;
        Vector3 newSelectedStartPos = newSelectedCharacter.transform.position;
        Vector3 selectedEndPos = _selectedCharacter.transform.position - new Vector3(30, 0, 0);
        Vector3 newSelectedEndPos = newSelectedCharacter.transform.position - new Vector3(30, 0, 0);
        while (startTime <= 0.3f)
        {
            //Debug.Log(startTime);
            startTime += Time.deltaTime;
            _selectedCharacter.transform.position =
                Vector3.Lerp(
                    selectedStartPos,
                   selectedEndPos,
                   startTime / 0.3f
                   );

            newSelectedCharacter.transform.position =
                Vector3.Lerp(
                   newSelectedStartPos,
                   newSelectedEndPos,
                   startTime / 0.3f
                   );
            yield return null;

        }
        Destroy(_selectedCharacter);
        _selectedCharacter = newSelectedCharacter;
    }

    IEnumerator SlideCharacterToRight()
    {
        float startTime = 0;

        GameObject newSelectedCharacter = Instantiate(_characters[_selectedIdx]);
        newSelectedCharacter.transform.position = new Vector3(-30, -1, 3.1f);
        newSelectedCharacter.transform.rotation = Quaternion.Euler(0, 180, 0);

        Vector3 selectedStartPos = _selectedCharacter.transform.position;
        Vector3 newSelectedStartPos = newSelectedCharacter.transform.position;
        Vector3 selectedEndPos = _selectedCharacter.transform.position + new Vector3(30, 0, 0);
        Vector3 newSelectedEndPos = newSelectedCharacter.transform.position + new Vector3(30, 0, 0);
        while (startTime <= 0.3f)
        {
            //Debug.Log(startTime);
            startTime += Time.deltaTime;
            _selectedCharacter.transform.position =
                Vector3.Lerp(
                    selectedStartPos,
                   selectedEndPos,
                   startTime / 0.3f
                   );

            newSelectedCharacter.transform.position =
                Vector3.Lerp(
                   newSelectedStartPos,
                   newSelectedEndPos,
                   startTime / 0.3f
                   );
            yield return null;

        }
        Destroy(_selectedCharacter);
        _selectedCharacter = newSelectedCharacter;
    }

    public void PlayBtnPressed()
    {
        SceneManager.LoadScene(1);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    #region Singleton
    public static GameManagerScript instance = null;

    public GameObject[] _characters = new GameObject[3];
    public GameObject[] _AICharacters = new GameObject[3];

    public GameObject[] _spawnedEnemy { get; private set; }
    public GameObject _spawnedCompany { get; private set; }

    public int _selectedCharacterIdx;

    int _playerEnergyPower;

    public GameObject _centerObject;

    Coroutine _startCountRoutine;

    AudioSource _audioSource1;
    AudioSource _audioSource2;
    public AudioClip _LobbySound;
    public AudioClip _InGameSound;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        _audioSource1 = gameObject.AddComponent<AudioSource>();
        _audioSource2 = gameObject.AddComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main UI")
        {
            _audioSource1.clip = _LobbySound;
            _audioSource1.loop = true;
            _audioSource1.volume = 0.5f;
            _audioSource1.Play();
        }
        else if (scene.name == "SoloShowDown")
        {
            _audioSource1.clip = _InGameSound;
            _audioSource1.loop = true;
            _audioSource1.volume = 0.5f;
            _audioSource1.Play();
            AllocatePlayers();
        }
    }

    public void AllocatePlayers()
    {
        _spawnedCompany = null;
        _spawnedEnemy = new GameObject[9];

        List<int> availablePositions = new List<int>();
        for (int i = 0; i < MapAllocator.instance._playerPos.Length; i++)
        {
            availablePositions.Add(i);
        }

        int GetRandomPosition()
        {
            int index = Random.Range(0, availablePositions.Count);
            int position = availablePositions[index];
            availablePositions.RemoveAt(index);
            return position;
        }

        // 플레이어 캐릭터 생성
        GameObject player = Instantiate(_characters[_selectedCharacterIdx]);
        int randomPos = GetRandomPosition();
        player.transform.position = MapAllocator.instance._playerPos[randomPos].transform.position;
        player.GetComponent<PlayerStats>()._spawnPosition = MapAllocator.instance._playerPos[randomPos].transform.position;
        _spawnedCompany = player;

        // AI 캐릭터 생성
        for (int i = 0; i < 9; i++)
        {
            int aiIndex = i % 3; // AI 캐릭터는 3가지 종류 중 하나를 선택
            randomPos = GetRandomPosition();
            GameObject ai = Instantiate(_AICharacters[aiIndex]);
            ai.transform.position = MapAllocator.instance._playerPos[randomPos].transform.position;
            ai.GetComponent<PlayerStats>()._spawnPosition = MapAllocator.instance._playerPos[randomPos].transform.position;
            ai.transform.rotation = Quaternion.Euler(0, 180, 0);
            ai.gameObject.tag = "Competition";
            _spawnedEnemy[i] = ai;
        }
    }


    public void ChangeBackgroundMusic(AudioClip newClip)
    {
        if (_audioSource1.isPlaying)
        {
            _audioSource2.clip = newClip;
            _audioSource2.loop = false;
            _audioSource2.volume = 0.5f;
            _audioSource2.Play();
        }
    }
}

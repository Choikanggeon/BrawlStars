using System.Collections;
using UnityEngine;


public class Energy : MonoBehaviour
{
    
    Animator _animator;
    private bool _isCollected = false; // 플래그 변수 추가
    private float _moveSpeed = 0.1f;
    public float sphereRadius = 3f;  // 구의 반지름
    public float maxDistance = 3f;    // 레이캐스트 최대 거리
    public LayerMask layerMask;        // 충돌 레이어 설정



    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("Energy");
    }
    void Update()
    {
        // SphereCastHit 변수 선언
        RaycastHit hit;

        var hits =  Physics.OverlapSphere(transform.position, sphereRadius, layerMask);

        // SphereCast 실행
        if (hits.Length > 0)
        {
            // 충돌한 오브젝트가 있을 경우 처리할 내
            transform.position = Vector3.MoveTowards(transform.position, hits[0].transform.position, _moveSpeed);
        }
    }

    void OnDrawGizmos()
    {
        // 시작 지점에 구를 시각적으로 표시
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (_isCollected) return; // 이미 처리된 경우, 더 이상 실행하지 않음
        if (other.CompareTag("Player"))
        {
            _isCollected = true; // 충돌 처리 플래그 설정
            StartCoroutine(DestroyAfterDelay()); // 일정 시간 후에 객체 파괴
            other.GetComponent<PlayerStats>().EnergyUp(); // EnergyUp 메서드 호출
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // 충분한 시간을 기다린 후에 파괴
        Destroy(gameObject); // 에너지 아이템 파괴
        _isCollected = false; // 충돌 처리 플래그 초기화
    }
}
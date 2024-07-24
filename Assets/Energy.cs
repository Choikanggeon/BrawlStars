using System.Collections;
using UnityEngine;


public class Energy : MonoBehaviour
{
    
    Animator _animator;
    private bool _isCollected = false; // �÷��� ���� �߰�
    private float _moveSpeed = 0.1f;
    public float sphereRadius = 3f;  // ���� ������
    public float maxDistance = 3f;    // ����ĳ��Ʈ �ִ� �Ÿ�
    public LayerMask layerMask;        // �浹 ���̾� ����



    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("Energy");
    }
    void Update()
    {
        // SphereCastHit ���� ����
        RaycastHit hit;

        var hits =  Physics.OverlapSphere(transform.position, sphereRadius, layerMask);

        // SphereCast ����
        if (hits.Length > 0)
        {
            // �浹�� ������Ʈ�� ���� ��� ó���� ��
            transform.position = Vector3.MoveTowards(transform.position, hits[0].transform.position, _moveSpeed);
        }
    }

    void OnDrawGizmos()
    {
        // ���� ������ ���� �ð������� ǥ��
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (_isCollected) return; // �̹� ó���� ���, �� �̻� �������� ����
        if (other.CompareTag("Player"))
        {
            _isCollected = true; // �浹 ó�� �÷��� ����
            StartCoroutine(DestroyAfterDelay()); // ���� �ð� �Ŀ� ��ü �ı�
            other.GetComponent<PlayerStats>().EnergyUp(); // EnergyUp �޼��� ȣ��
        }
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); // ����� �ð��� ��ٸ� �Ŀ� �ı�
        Destroy(gameObject); // ������ ������ �ı�
        _isCollected = false; // �浹 ó�� �÷��� �ʱ�ȭ
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform target_Tr;

    public Vector3 initialPosition = new Vector3(185, 0, 136); // 게임 시작 시 카메라의 초기 위치
    public Vector3 offset = new Vector3(0, 37.5f, -25); // 카메라와 플레이어 사이의 오프셋
    public Vector3 rotation = new Vector3(55, 0, 0); // 카메라의 회전 각도

    bool moving = false;
    bool following = false;
    float StartCameraSpeed = 1f;
    float CurrentCameraSpeed = 0.125f;

    void Start()
    {
        target_Tr = PlayerController.instance.GetComponent<Transform>();

        transform.position = initialPosition + offset; // 카메라의 초기 위치 설정
        transform.rotation = Quaternion.Euler(rotation);
        StartCoroutine(WaitCameraMoving());
    }

    void LateUpdate()
    {
        if (moving)
        {
            if(following)
            {
                CameraMovementSet(CurrentCameraSpeed);
            }
            else
                CameraMovementSet(StartCameraSpeed);
        }
    }

    void CameraMovementSet(float cameraSpeed)
    {
        Vector3 ExceptX = new Vector3(target_Tr.position.x, 0, target_Tr.position.z);
        Vector3 desiredPos = ExceptX + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, cameraSpeed * Time.deltaTime);
        transform.position = smoothedPos;
        if (transform.position == desiredPos)
        {
            following = true;
        }
    }

    IEnumerator WaitCameraMoving()
    {
        yield return new WaitForSeconds(4.5f);
        moving = true;
    }
}

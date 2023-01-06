using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Detected : MonoBehaviour
{
    public float tracking_delay = 0.0f;  // 추적 딜레이
    private float reload_delay = 0.0f;   // 재장전 시간

    private bool stop_tracking;          // 플레이어 추적 결정
    private bool raycast_detected;       // RayCast 감지 결과
    
    public GameObject turret;

    void Start()
    {
        
    }

    void Update()
    {
        if (reload_delay > 0) // 재장전 시간 카운트
        {
            reload_delay -= Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // 감지 범위에 플레이어 탱크가 닿은 경우
        {
            RaycastHit hit; // Raycast를 맞은 물체 정보 저장
            Vector3 fwd = turret.transform.TransformDirection(Vector3.forward); // 포탑의 정면을 글로벌 좌표계로 변환
            raycast_detected = Physics.Raycast(turret.GetComponent<Enemy_Turret_Control>().sp_point.position, fwd * 20, out hit);
            
            if (raycast_detected == true && hit.transform.CompareTag("Player") && reload_delay <= 0)
            {   // 감지범위 내에 플러이어가 있고 중간에 아무것도 없는 경우(사격각이 나오는 경우)
                turret.GetComponent<Enemy_Turret_Control>().Shooting();
                reload_delay = 5.0f; // 재장전 시간 5초
            }
        }
    }
}

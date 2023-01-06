using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell_Control : MonoBehaviour
{
    public float damage = 50.0f; // 포탄 데미지
    public float shell_detected_delay = 0.0f;
    
    public Transform explosion_effect; // 포탄 폭발 파티클

    void Start()
    {
        Destroy(this, 1.5f); // 생성 후 1.5초 뒤 파괴
        shell_detected_delay = 0.0f;
    }

    void Update()
    {
        if (shell_detected_delay > 0.0f)
        {
            shell_detected_delay -= Time.deltaTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name != "DetectionRange" && shell_detected_delay <= 0) // 감지 범위 콜라이더에 닿아서 터지는 경우 방지
        {
            shell_detected_delay = 1.0f; // 중복감지 회피
            Instantiate(explosion_effect, this.transform.position, this.transform.rotation); // 파티클 생성
            Destroy(this.gameObject); // 파괴
            if (other.name == "Panzer_VI_E_Track_L") // 플레이어 탱크 왼쪽 궤도에 맞은 경우
            {
                Scroll_Left_Track.trackHealth -= damage; // 궤도 체력 감소
            }

            if (other.name == "Panzer_VI_E_Track_R") // 플레이어 탱크 오른쪽 궤도에 맞은 경우
            {
                Scroll_Right_Track.trackHealth -= damage; // 궤도 체력 감소
            }

            if (other.gameObject.tag == "Enemy") // 적군에 닿은 경우
            {
                other.GetComponentInParent<Enemy_Control>().enemy_health -= damage; // 적군 체력 감소
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel_Control : MonoBehaviour
{
    public static float BarrelHealth = 150.0f; // 주포 체력
    public static bool destroy = false;        // 주포 파괴 여부

    void Start()
    {
        
    }

    void Update()
    {
        BarrelDestroy(); // 주포 파괴 확인
    }

    void BarrelDestroy()
    {
        if (BarrelHealth <= 0)  // 주포가 파괴된 경우
        {
            destroy = true; // 현재 상태 변경
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet") // 포탄에 맞은 경우
        {
            BarrelHealth -= other.gameObject.GetComponentInParent<Shell_Control>().damage; // 포신 체력 감소
        }
    }
}

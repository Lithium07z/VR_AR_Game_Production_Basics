using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel_Control : MonoBehaviour
{
    public static float BarrelHealth = 150.0f; // ���� ü��
    public static bool destroy = false;        // ���� �ı� ����

    void Start()
    {
        
    }

    void Update()
    {
        BarrelDestroy(); // ���� �ı� Ȯ��
    }

    void BarrelDestroy()
    {
        if (BarrelHealth <= 0)  // ������ �ı��� ���
        {
            destroy = true; // ���� ���� ����
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet") // ��ź�� ���� ���
        {
            BarrelHealth -= other.gameObject.GetComponentInParent<Shell_Control>().damage; // ���� ü�� ����
        }
    }
}

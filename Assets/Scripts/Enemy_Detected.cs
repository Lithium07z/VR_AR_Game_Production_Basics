using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Detected : MonoBehaviour
{
    public float tracking_delay = 0.0f;  // ���� ������
    private float reload_delay = 0.0f;   // ������ �ð�

    private bool stop_tracking;          // �÷��̾� ���� ����
    private bool raycast_detected;       // RayCast ���� ���
    
    public GameObject turret;

    void Start()
    {
        
    }

    void Update()
    {
        if (reload_delay > 0) // ������ �ð� ī��Ʈ
        {
            reload_delay -= Time.deltaTime;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) // ���� ������ �÷��̾� ��ũ�� ���� ���
        {
            RaycastHit hit; // Raycast�� ���� ��ü ���� ����
            Vector3 fwd = turret.transform.TransformDirection(Vector3.forward); // ��ž�� ������ �۷ι� ��ǥ��� ��ȯ
            raycast_detected = Physics.Raycast(turret.GetComponent<Enemy_Turret_Control>().sp_point.position, fwd * 20, out hit);
            
            if (raycast_detected == true && hit.transform.CompareTag("Player") && reload_delay <= 0)
            {   // �������� ���� �÷��̾ �ְ� �߰��� �ƹ��͵� ���� ���(��ݰ��� ������ ���)
                turret.GetComponent<Enemy_Turret_Control>().Shooting();
                reload_delay = 5.0f; // ������ �ð� 5��
            }
        }
    }
}

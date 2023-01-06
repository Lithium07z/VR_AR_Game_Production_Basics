using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell_Control : MonoBehaviour
{
    public float damage = 50.0f; // ��ź ������
    public float shell_detected_delay = 0.0f;
    
    public Transform explosion_effect; // ��ź ���� ��ƼŬ

    void Start()
    {
        Destroy(this, 1.5f); // ���� �� 1.5�� �� �ı�
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
        if (other.name != "DetectionRange" && shell_detected_delay <= 0) // ���� ���� �ݶ��̴��� ��Ƽ� ������ ��� ����
        {
            shell_detected_delay = 1.0f; // �ߺ����� ȸ��
            Instantiate(explosion_effect, this.transform.position, this.transform.rotation); // ��ƼŬ ����
            Destroy(this.gameObject); // �ı�
            if (other.name == "Panzer_VI_E_Track_L") // �÷��̾� ��ũ ���� �˵��� ���� ���
            {
                Scroll_Left_Track.trackHealth -= damage; // �˵� ü�� ����
            }

            if (other.name == "Panzer_VI_E_Track_R") // �÷��̾� ��ũ ������ �˵��� ���� ���
            {
                Scroll_Right_Track.trackHealth -= damage; // �˵� ü�� ����
            }

            if (other.gameObject.tag == "Enemy") // ������ ���� ���
            {
                other.GetComponentInParent<Enemy_Control>().enemy_health -= damage; // ���� ü�� ����
            }
        }
    }
}

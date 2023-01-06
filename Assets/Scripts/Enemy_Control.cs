using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Control : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;

    public float rot_angle = 5.0f;
    public float tracking_delay = 0.0f; // ���� ������
    public float enemy_health = 200.0f; // ���� ü��

    public bool stop_tracking = false;
    
    public GameObject player_tank; // �÷��̾� ��ũ
    public GameObject turret;      // �ͷ�
    private GameObject myself;     // ���� ��ũ��Ʈ�� ���� ��ũ�� �����̹Ƿ� ������Ʈ���� �Ǵ������ ��

    public AudioSource audio_source;
    public AudioClip audio_clip;
    
    void Start()
    {
        player_tank = GameObject.Find("Panzer_VI_E");
        myself = this.gameObject;
        audio_source = this.GetComponent<AudioSource>();
    }
    
    void Update()
    { 
        MoveToPlayer();

        if (enemy_health <= 0) // �ı��� ���
        {
            Tank_Control.game_score += 100;        // ���� ���� 100�� ����
            audio_source.clip = audio_clip;
            audio_source.Play();
            Destroy(GameObject.Find(myself.name)); // ��ũ ���� 
        }
    }

    void MoveToPlayer()
    {
        agent.SetDestination(player_tank.transform.position);
    }
}

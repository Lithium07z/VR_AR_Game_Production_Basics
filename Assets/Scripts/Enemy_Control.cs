using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy_Control : MonoBehaviour
{
    public UnityEngine.AI.NavMeshAgent agent;

    public float rot_angle = 5.0f;
    public float tracking_delay = 0.0f; // 추적 딜레이
    public float enemy_health = 200.0f; // 적군 체력

    public bool stop_tracking = false;
    
    public GameObject player_tank; // 플레이어 탱크
    public GameObject turret;      // 터렛
    private GameObject myself;     // 단일 스크립트로 여러 탱크를 움직이므로 오브젝트마다 판단해줘야 함

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

        if (enemy_health <= 0) // 파괴된 경우
        {
            Tank_Control.game_score += 100;        // 유저 점수 100점 증가
            audio_source.clip = audio_clip;
            audio_source.Play();
            Destroy(GameObject.Find(myself.name)); // 탱크 삭제 
        }
    }

    void MoveToPlayer()
    {
        agent.SetDestination(player_tank.transform.position);
    }
}

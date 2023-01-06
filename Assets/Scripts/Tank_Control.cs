using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tank_Control : MonoBehaviour
{
    public static float tank_health = 700.0f;  // 탱크 체력
    public static float goal_score;      // 게임 목표 점수
    public static int game_score = 0;    // 플레이어 점수
    private float tank_speed = 0.0f;     // 탱크 속도
    private float rot_speed = 100.0f;    // 탱크 회전각도 
    private float shell_power = 4000.0f; // 포탄 파워 
    private float reload_delay = 0.0f;   // 재장전 시간
    private float repair_time = 0.0f;    // 부품 수리 시간
    private float shell_detected_delay = 0.0f; // 포탄 감지 딜레이

    private float moving_velocity;     // 앞, 뒤 움직임
    private float tank_angle;          // 좌, 우 움직임
    private bool repairing = false;    // 수리 상태
    private bool gunner_camera_switch; // 포수 카메라 상태
    
    public GameObject turret;        // 터렛
    public GameObject left_track;    // 왼쪽 궤도
    public GameObject right_track;   // 오른쪽 궤도
    public GameObject barrel;        // 포신
    public GameObject spawn_point;   // 포탄 스폰지점
    public GameObject gunner_camera; // 포수 시점
    public Transform shell;          // 포탄

    private AudioSource audio_source; // 오디오 소스
    public AudioClip engine_sound;    // 엔진 오디오 소스 클립

    private GUIStyle tank_UI_style = new GUIStyle(); // 탱크 기본 UI 스타일
    private GUIStyle warning_style = new GUIStyle(); // 탱크 경고 UI 스타일

    public Font font;

    void Awake()
    {
        tank_UI_style.normal.textColor = Color.black;
        tank_UI_style.fontSize = 25;
        tank_UI_style.font = font;
        warning_style.normal.textColor = Color.red;
        warning_style.fontSize = 25;
        warning_style.font = font;
    }

    void Start()
    {
        /***********************************************************************
        *                          게임 초기 값 설정
        ***********************************************************************/
        
        if (Main_Scene_Control.level == 1)      // 쉬움
        {
            goal_score = 200;
        }
        else if (Main_Scene_Control.level == 2) // 보통
        {
            goal_score = 400;
        }
        else if (Main_Scene_Control.level == 3) // 어려움
        {
            goal_score = 600;
        }
        else if (Main_Scene_Control.level == 4) // 매우 어려움
        {
            goal_score = 800;
        } 
        else
        {
            goal_score = 200; // 선택 없을 시 기본 쉬움 난이도
        }
        
        audio_source = this.GetComponent<AudioSource>();
        spawn_point = GameObject.Find("sp_shell"); // 포탄 발사 위치 
        audio_source.Play();

        Barrel_Control.destroy = false;
        left_track.GetComponent<Scroll_Left_Track>().destroy = false;
        right_track.GetComponent<Scroll_Right_Track>().destroy = false;

        game_score = 0; // 게임 스코어 초기화
        tank_health = 700.0f; // 탱크 체력 초기화 
        Barrel_Control.BarrelHealth = 150.0f;    // 부품 별 체력 초기화
        Scroll_Left_Track.trackHealth = 150.0f;  // 부품 별 체력 초기화
        Scroll_Right_Track.trackHealth = 150.0f; // 부품 별 체력 초기화
    }

    void Update()
    {
        Acceleration();    // 탱크 가감속 구현 함수
        Movement();        // 탱크 이동 구현 함수
        TrackTexture();    // 궤도 텍스쳐 구현 함수
        EngineSound();     // 엔진 사운드 구현 함수 
        Shooting();        // 탱크 사격 구현 함수
        RepairingParts();  // 탱크 수리 구현 함수
        CameraChange();    // 탱크 시점 변환 구현 함수
    }

    /***********************************************************************
    *                 키 입력 시간에 따른 탱크 가감속 구현
    ***********************************************************************/

    void Acceleration()
    {
        moving_velocity = Input.GetAxis("Vertical");
        tank_angle = Input.GetAxis("Horizontal");

        if (moving_velocity == 1.0f || moving_velocity == -1.0f)
        {
            tank_speed += Time.deltaTime * 1.5f;
            if (moving_velocity == 1.0f && (left_track.GetComponent<Scroll_Left_Track>().destroy 
                || right_track.GetComponent<Scroll_Right_Track>().destroy) && tank_speed > 8.0f) 
            {   // 궤도가 파괴된 경우 최고 속도 8로 제한
                tank_speed = 8.0f;
            } 
            else if (moving_velocity == 1.0f && tank_speed > 15.0f) // 전진 속도
            {   // 기본 최고 속도 15
                tank_speed = 15.0f;
            }

            if (moving_velocity == -1.0f && (left_track.GetComponent<Scroll_Left_Track>().destroy
              || right_track.GetComponent<Scroll_Right_Track>().destroy) && tank_speed > 4.0f)
            {   // 궤도가 파괴된 경우 최고 속도 4로 제한
                tank_speed = 4.0f;
            } 
            else if (moving_velocity == -1.0f && tank_speed > 8.0f) // 후진 속도
            {   // 기본 최고 속도 8
                tank_speed = 8.0f;
            }
        }
        else if (tank_speed > 0 && moving_velocity == 0.0f) // 정지 상태시
        {
            tank_speed -= Time.deltaTime * 3.0f; // 속도 감속
        }
    }

    /***********************************************************************
    *                          탱크 움직임 구현부
    ***********************************************************************/

    void Movement()
    {
        float distance_per_frame = tank_speed * Time.deltaTime;
        float degrees_per_frame = rot_speed * Time.deltaTime;

        if (repairing == false && moving_velocity >= 0) // 전진
        {
            this.transform.Translate(Vector3.forward * moving_velocity * distance_per_frame);
            this.transform.Rotate(0.0f, tank_angle * degrees_per_frame, 0.0f);
        }
        else if (repairing == false && moving_velocity < 0) // 후진
        {
            this.transform.Translate(Vector3.forward * moving_velocity * distance_per_frame);
            this.transform.Rotate(0.0f, -tank_angle * degrees_per_frame, 0.0f);
        }
    }

    /***********************************************************************
    *                탱크 움직임에 따른 궤도 텍스쳐 방향 전환
    ***********************************************************************/

    void TrackTexture()
    {
        if (moving_velocity == 0.0f && tank_angle == 0.0f) // 정지 상태
        {
            Scroll_Left_Track.scrollSpeed = 0.0f;
            Scroll_Right_Track.scrollSpeed = 0.0f;
        }
        else if (moving_velocity > 0 && tank_angle < 0) // 좌회전
        {
            Scroll_Left_Track.scrollSpeed = 0.0f;
            Scroll_Right_Track.scrollSpeed = tank_speed;
        }
        else if (moving_velocity > 0 && tank_angle > 0) // 우회전
        {
            Scroll_Left_Track.scrollSpeed = tank_speed;
            Scroll_Right_Track.scrollSpeed = 0.0f;
        }
        else if (moving_velocity < 0 && tank_angle < 0) // 왼쪽 후진
        {
            Scroll_Left_Track.scrollSpeed = 0.0f;
            Scroll_Right_Track.scrollSpeed = -tank_speed;
        }
        else if (moving_velocity < 0 && tank_angle > 0) // 오른쪽 후진
        {
            Scroll_Left_Track.scrollSpeed = -tank_speed;
            Scroll_Right_Track.scrollSpeed = 0.0f;
        }
        else if (moving_velocity > 0) // 전진
        {
            Scroll_Left_Track.scrollSpeed = tank_speed;
            Scroll_Right_Track.scrollSpeed = tank_speed;
        }
        else if (moving_velocity < 0) //후진
        {
            Scroll_Left_Track.scrollSpeed = -tank_speed;
            Scroll_Right_Track.scrollSpeed = -tank_speed;
        }
        else if (tank_angle < 0) // 왼쪽 피봇턴
        {
            Scroll_Left_Track.scrollSpeed = -2.5f;
            Scroll_Right_Track.scrollSpeed = 2.5f;
        }
        else if (tank_angle > 0) // 오른쪽 피봇턴
        {
            Scroll_Left_Track.scrollSpeed = 2.5f;
            Scroll_Right_Track.scrollSpeed = -2.5f;
        }
    }

    /***********************************************************************
    *                          엔진 사운드 구현
    ***********************************************************************/

    void EngineSound()
    {
        if (moving_velocity == 0) // 정지 상태시
        {
            audio_source.volume -= Time.deltaTime / 2; // 볼륨 감소
        }
        else // 이동 상태시
        {
            audio_source.volume += Time.deltaTime; // 볼륨 증가
        }
    }

    /***********************************************************************
    *                           포탄 발사 구현부
    ***********************************************************************/

    void Shooting()
    {
        if (Input.GetButtonDown("Fire1") && reload_delay <= 0 && Barrel_Control.destroy == false)
        {   // 마우스 왼쪽 클릭 했을 때, 재장전 시간이 0보다 같거나 작고 포신이 파괴된 상태가 아니라면
            Transform prefab_shell = Instantiate(shell, spawn_point.transform.position, spawn_point.transform.rotation); // 포탄 생성
            prefab_shell.GetComponent<Rigidbody>().AddForce(barrel.transform.forward * shell_power); // 포탄 발사
            turret.GetComponent<AudioSource>().Play(); // 포 발사음 재생
            reload_delay = 5.0f; // 재장전 시간 5초
        }

        if (reload_delay > 0) // 재장전 시간 카운트
        {
            reload_delay -= Time.deltaTime;
        }

        if (shell_detected_delay > 0)
        {
            shell_detected_delay -= Time.deltaTime;
        } 
        else
        {
            flag = true;
        }
    }

    /***********************************************************************
    *                          카메라 전환 구현부
    ***********************************************************************/

    void CameraChange()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            gunner_camera_switch = !gunner_camera_switch;  // 포수 카메라 상태 전환
            gunner_camera.SetActive(gunner_camera_switch); // 포수 카메라 활성화, 비활성화
        }
    }

    /***********************************************************************
    *                        탱크 부품 수리 구현부
    ***********************************************************************/

    void RepairingParts()
    {
        if ((Barrel_Control.destroy ||
            left_track.GetComponent<Scroll_Left_Track>().destroy ||
            right_track.GetComponent<Scroll_Right_Track>().destroy) && repairing == false && Input.GetKeyDown(KeyCode.F))
        {   // 주포, 양쪽 궤도 중 하나라도 파괴된 상태이고 F키를 눌러 수리하려 하는 경우 
            repair_time = 0.0f; // 수리 시간 0으로 초기화
            repairing = true;   // 상태 변환

            if (Barrel_Control.destroy) // 포신이 파괴된 경우
            {
                repair_time += 6.0f;
            }
            if (left_track.GetComponent<Scroll_Left_Track>().destroy) // 왼쪽 궤도가 파괴된 경우
            {
                repair_time += 5.0f;
            }
            if (right_track.GetComponent<Scroll_Right_Track>().destroy) // 오른쪽 궤도가 파괴된 경우
            {
                repair_time += 5.0f;
            }
        } 
        else if (repairing == true && Input.GetKeyDown(KeyCode.F)) // 수리를 취소하려는 경우
        {
            repairing = false; // 상태 변환
        }

        if (repairing == true && repair_time > 0) // 수리 중
        {
            tank_speed = 0.0f;
            repair_time -= Time.deltaTime; 
        } 
        else if (repairing == true && repair_time <= 0) // 수리 완료
        {
            if (Barrel_Control.destroy)
            {
                Barrel_Control.destroy = false; // 상태 변환
                Barrel_Control.BarrelHealth = 150.0f; // 부품 체력 복구
            }

            if (left_track.GetComponent<Scroll_Left_Track>().destroy)
            {
                left_track.GetComponent<Scroll_Left_Track>().destroy = false; // 상태 변환
                Scroll_Left_Track.trackHealth = 150.0f; // 부품 체력 복구
            }

            if (right_track.GetComponent<Scroll_Right_Track>().destroy)
            {
                right_track.GetComponent<Scroll_Right_Track>().destroy = false; // 상태 변환
                Scroll_Right_Track.trackHealth = 150.0f; // 부품 체력 복구
            }
            repairing = false; // 상태 변환
        }
    }

    /***********************************************************************
    *                           탱크 체력 처리
    ***********************************************************************/
    bool flag = true;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet" && flag) // 포탄에 맞은 경우
        {
            tank_health -= other.gameObject.GetComponentInParent<Shell_Control>().damage; // 탱크 체력 감소
            flag = false; // 포탄 감지 false, 단기간에 너무 많이 맞는 경우 핸디캡 + 중복감지 회피
            shell_detected_delay = 0.3f; // 다음 포탄 감지까지 0.3초
            Destroy(other); // 포탄 삭제
            // shell mesh 분리로 인해 GetComponentInParent사용, 모든 shell 충돌의 경우에 shell mesh 분리 고려해서 작성할 것
            if (tank_health <= 0 && game_score >= goal_score) // 탱크는 파괴됬지만 승리한 경우
            {
                SceneManager.LoadScene("WinnerScene", LoadSceneMode.Single);
            }
            else if (tank_health <= 0 && game_score < goal_score) // 탱크도 파괴되고 패배한 경우
            {
                SceneManager.LoadScene("LoserScene", LoadSceneMode.Single);
            }
        }
    }

    /***********************************************************************
    *                           화면 GUI 구현
    ***********************************************************************/

    void OnGUI()
    {
        /***********************************************************************
        *                             기본 UI 표시
        ***********************************************************************/

        GUI.Label(new Rect(20, Screen.height - 100, 128, 32), "점수 : " + ((int)game_score).ToString(), tank_UI_style);
        GUI.Label(new Rect(20, Screen.height - 75, 128, 32), "체력 : " + ((int)tank_health).ToString(), tank_UI_style);
        GUI.Label(new Rect(20, Screen.height - 50, 128, 32), "속도 : " + ((int)tank_speed).ToString(), tank_UI_style);

        /***********************************************************************
        *                           수리 중 경고 표시
        ***********************************************************************/

        if (repairing == true)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 25, 256, 32), "수리 중에는 움직일 수 없습니다.", warning_style);
            GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height / 2, 256, 32), "수리 완료까지 : " + (int)repair_time + "초", warning_style);
        }

        /***********************************************************************
        *                           재장전 시간 표시
        ***********************************************************************/

        if (reload_delay > 0)
        {
            GUI.Label(new Rect(Screen.width / 2 - 63, Screen.height / 2 - 45, 256, 32), "Reloading", warning_style);
            GUI.Label(new Rect(Screen.width / 2 - 20, Screen.height / 2 + 15, 256, 32), (Mathf.Round(reload_delay * 10) * 0.1f).ToString(), warning_style);
        }

        /***********************************************************************
        *                            부품 상태 표시
        ***********************************************************************/

        if (Barrel_Control.destroy)
        {
            GUI.Label(new Rect(20, 20, 128, 32), "주포 파괴됨!", warning_style);
        } 
        else
        {
            GUI.Label(new Rect(20, 20, 128, 32), "주포 체력 : " + Barrel_Control.BarrelHealth, tank_UI_style);
        }

        if (left_track.GetComponent<Scroll_Left_Track>().destroy)
        {
            GUI.Label(new Rect(20, 45, 128, 32), "왼쪽 궤도 파괴됨!", warning_style);
        }
        else
        {
            GUI.Label(new Rect(20, 45, 128, 32), "왼쪽 궤도 체력 : " + Scroll_Left_Track.trackHealth, tank_UI_style);
        }

        if (right_track.GetComponent<Scroll_Right_Track>().destroy)
        {
            GUI.Label(new Rect(20, 70, 128, 32), "오른쪽 궤도 파괴됨!", warning_style);
        }
        else
        {
            GUI.Label(new Rect(20, 70, 128, 32), "오른쪽 궤도 체력 : " + Scroll_Right_Track.trackHealth, tank_UI_style);
        }
    }
}

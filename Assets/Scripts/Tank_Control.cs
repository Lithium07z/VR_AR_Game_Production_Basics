using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tank_Control : MonoBehaviour
{
    public static float tank_health = 700.0f;  // ��ũ ü��
    public static float goal_score;      // ���� ��ǥ ����
    public static int game_score = 0;    // �÷��̾� ����
    private float tank_speed = 0.0f;     // ��ũ �ӵ�
    private float rot_speed = 100.0f;    // ��ũ ȸ������ 
    private float shell_power = 4000.0f; // ��ź �Ŀ� 
    private float reload_delay = 0.0f;   // ������ �ð�
    private float repair_time = 0.0f;    // ��ǰ ���� �ð�
    private float shell_detected_delay = 0.0f; // ��ź ���� ������

    private float moving_velocity;     // ��, �� ������
    private float tank_angle;          // ��, �� ������
    private bool repairing = false;    // ���� ����
    private bool gunner_camera_switch; // ���� ī�޶� ����
    
    public GameObject turret;        // �ͷ�
    public GameObject left_track;    // ���� �˵�
    public GameObject right_track;   // ������ �˵�
    public GameObject barrel;        // ����
    public GameObject spawn_point;   // ��ź ��������
    public GameObject gunner_camera; // ���� ����
    public Transform shell;          // ��ź

    private AudioSource audio_source; // ����� �ҽ�
    public AudioClip engine_sound;    // ���� ����� �ҽ� Ŭ��

    private GUIStyle tank_UI_style = new GUIStyle(); // ��ũ �⺻ UI ��Ÿ��
    private GUIStyle warning_style = new GUIStyle(); // ��ũ ��� UI ��Ÿ��

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
        *                          ���� �ʱ� �� ����
        ***********************************************************************/
        
        if (Main_Scene_Control.level == 1)      // ����
        {
            goal_score = 200;
        }
        else if (Main_Scene_Control.level == 2) // ����
        {
            goal_score = 400;
        }
        else if (Main_Scene_Control.level == 3) // �����
        {
            goal_score = 600;
        }
        else if (Main_Scene_Control.level == 4) // �ſ� �����
        {
            goal_score = 800;
        } 
        else
        {
            goal_score = 200; // ���� ���� �� �⺻ ���� ���̵�
        }
        
        audio_source = this.GetComponent<AudioSource>();
        spawn_point = GameObject.Find("sp_shell"); // ��ź �߻� ��ġ 
        audio_source.Play();

        Barrel_Control.destroy = false;
        left_track.GetComponent<Scroll_Left_Track>().destroy = false;
        right_track.GetComponent<Scroll_Right_Track>().destroy = false;

        game_score = 0; // ���� ���ھ� �ʱ�ȭ
        tank_health = 700.0f; // ��ũ ü�� �ʱ�ȭ 
        Barrel_Control.BarrelHealth = 150.0f;    // ��ǰ �� ü�� �ʱ�ȭ
        Scroll_Left_Track.trackHealth = 150.0f;  // ��ǰ �� ü�� �ʱ�ȭ
        Scroll_Right_Track.trackHealth = 150.0f; // ��ǰ �� ü�� �ʱ�ȭ
    }

    void Update()
    {
        Acceleration();    // ��ũ ������ ���� �Լ�
        Movement();        // ��ũ �̵� ���� �Լ�
        TrackTexture();    // �˵� �ؽ��� ���� �Լ�
        EngineSound();     // ���� ���� ���� �Լ� 
        Shooting();        // ��ũ ��� ���� �Լ�
        RepairingParts();  // ��ũ ���� ���� �Լ�
        CameraChange();    // ��ũ ���� ��ȯ ���� �Լ�
    }

    /***********************************************************************
    *                 Ű �Է� �ð��� ���� ��ũ ������ ����
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
            {   // �˵��� �ı��� ��� �ְ� �ӵ� 8�� ����
                tank_speed = 8.0f;
            } 
            else if (moving_velocity == 1.0f && tank_speed > 15.0f) // ���� �ӵ�
            {   // �⺻ �ְ� �ӵ� 15
                tank_speed = 15.0f;
            }

            if (moving_velocity == -1.0f && (left_track.GetComponent<Scroll_Left_Track>().destroy
              || right_track.GetComponent<Scroll_Right_Track>().destroy) && tank_speed > 4.0f)
            {   // �˵��� �ı��� ��� �ְ� �ӵ� 4�� ����
                tank_speed = 4.0f;
            } 
            else if (moving_velocity == -1.0f && tank_speed > 8.0f) // ���� �ӵ�
            {   // �⺻ �ְ� �ӵ� 8
                tank_speed = 8.0f;
            }
        }
        else if (tank_speed > 0 && moving_velocity == 0.0f) // ���� ���½�
        {
            tank_speed -= Time.deltaTime * 3.0f; // �ӵ� ����
        }
    }

    /***********************************************************************
    *                          ��ũ ������ ������
    ***********************************************************************/

    void Movement()
    {
        float distance_per_frame = tank_speed * Time.deltaTime;
        float degrees_per_frame = rot_speed * Time.deltaTime;

        if (repairing == false && moving_velocity >= 0) // ����
        {
            this.transform.Translate(Vector3.forward * moving_velocity * distance_per_frame);
            this.transform.Rotate(0.0f, tank_angle * degrees_per_frame, 0.0f);
        }
        else if (repairing == false && moving_velocity < 0) // ����
        {
            this.transform.Translate(Vector3.forward * moving_velocity * distance_per_frame);
            this.transform.Rotate(0.0f, -tank_angle * degrees_per_frame, 0.0f);
        }
    }

    /***********************************************************************
    *                ��ũ �����ӿ� ���� �˵� �ؽ��� ���� ��ȯ
    ***********************************************************************/

    void TrackTexture()
    {
        if (moving_velocity == 0.0f && tank_angle == 0.0f) // ���� ����
        {
            Scroll_Left_Track.scrollSpeed = 0.0f;
            Scroll_Right_Track.scrollSpeed = 0.0f;
        }
        else if (moving_velocity > 0 && tank_angle < 0) // ��ȸ��
        {
            Scroll_Left_Track.scrollSpeed = 0.0f;
            Scroll_Right_Track.scrollSpeed = tank_speed;
        }
        else if (moving_velocity > 0 && tank_angle > 0) // ��ȸ��
        {
            Scroll_Left_Track.scrollSpeed = tank_speed;
            Scroll_Right_Track.scrollSpeed = 0.0f;
        }
        else if (moving_velocity < 0 && tank_angle < 0) // ���� ����
        {
            Scroll_Left_Track.scrollSpeed = 0.0f;
            Scroll_Right_Track.scrollSpeed = -tank_speed;
        }
        else if (moving_velocity < 0 && tank_angle > 0) // ������ ����
        {
            Scroll_Left_Track.scrollSpeed = -tank_speed;
            Scroll_Right_Track.scrollSpeed = 0.0f;
        }
        else if (moving_velocity > 0) // ����
        {
            Scroll_Left_Track.scrollSpeed = tank_speed;
            Scroll_Right_Track.scrollSpeed = tank_speed;
        }
        else if (moving_velocity < 0) //����
        {
            Scroll_Left_Track.scrollSpeed = -tank_speed;
            Scroll_Right_Track.scrollSpeed = -tank_speed;
        }
        else if (tank_angle < 0) // ���� �Ǻ���
        {
            Scroll_Left_Track.scrollSpeed = -2.5f;
            Scroll_Right_Track.scrollSpeed = 2.5f;
        }
        else if (tank_angle > 0) // ������ �Ǻ���
        {
            Scroll_Left_Track.scrollSpeed = 2.5f;
            Scroll_Right_Track.scrollSpeed = -2.5f;
        }
    }

    /***********************************************************************
    *                          ���� ���� ����
    ***********************************************************************/

    void EngineSound()
    {
        if (moving_velocity == 0) // ���� ���½�
        {
            audio_source.volume -= Time.deltaTime / 2; // ���� ����
        }
        else // �̵� ���½�
        {
            audio_source.volume += Time.deltaTime; // ���� ����
        }
    }

    /***********************************************************************
    *                           ��ź �߻� ������
    ***********************************************************************/

    void Shooting()
    {
        if (Input.GetButtonDown("Fire1") && reload_delay <= 0 && Barrel_Control.destroy == false)
        {   // ���콺 ���� Ŭ�� ���� ��, ������ �ð��� 0���� ���ų� �۰� ������ �ı��� ���°� �ƴ϶��
            Transform prefab_shell = Instantiate(shell, spawn_point.transform.position, spawn_point.transform.rotation); // ��ź ����
            prefab_shell.GetComponent<Rigidbody>().AddForce(barrel.transform.forward * shell_power); // ��ź �߻�
            turret.GetComponent<AudioSource>().Play(); // �� �߻��� ���
            reload_delay = 5.0f; // ������ �ð� 5��
        }

        if (reload_delay > 0) // ������ �ð� ī��Ʈ
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
    *                          ī�޶� ��ȯ ������
    ***********************************************************************/

    void CameraChange()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            gunner_camera_switch = !gunner_camera_switch;  // ���� ī�޶� ���� ��ȯ
            gunner_camera.SetActive(gunner_camera_switch); // ���� ī�޶� Ȱ��ȭ, ��Ȱ��ȭ
        }
    }

    /***********************************************************************
    *                        ��ũ ��ǰ ���� ������
    ***********************************************************************/

    void RepairingParts()
    {
        if ((Barrel_Control.destroy ||
            left_track.GetComponent<Scroll_Left_Track>().destroy ||
            right_track.GetComponent<Scroll_Right_Track>().destroy) && repairing == false && Input.GetKeyDown(KeyCode.F))
        {   // ����, ���� �˵� �� �ϳ��� �ı��� �����̰� FŰ�� ���� �����Ϸ� �ϴ� ��� 
            repair_time = 0.0f; // ���� �ð� 0���� �ʱ�ȭ
            repairing = true;   // ���� ��ȯ

            if (Barrel_Control.destroy) // ������ �ı��� ���
            {
                repair_time += 6.0f;
            }
            if (left_track.GetComponent<Scroll_Left_Track>().destroy) // ���� �˵��� �ı��� ���
            {
                repair_time += 5.0f;
            }
            if (right_track.GetComponent<Scroll_Right_Track>().destroy) // ������ �˵��� �ı��� ���
            {
                repair_time += 5.0f;
            }
        } 
        else if (repairing == true && Input.GetKeyDown(KeyCode.F)) // ������ ����Ϸ��� ���
        {
            repairing = false; // ���� ��ȯ
        }

        if (repairing == true && repair_time > 0) // ���� ��
        {
            tank_speed = 0.0f;
            repair_time -= Time.deltaTime; 
        } 
        else if (repairing == true && repair_time <= 0) // ���� �Ϸ�
        {
            if (Barrel_Control.destroy)
            {
                Barrel_Control.destroy = false; // ���� ��ȯ
                Barrel_Control.BarrelHealth = 150.0f; // ��ǰ ü�� ����
            }

            if (left_track.GetComponent<Scroll_Left_Track>().destroy)
            {
                left_track.GetComponent<Scroll_Left_Track>().destroy = false; // ���� ��ȯ
                Scroll_Left_Track.trackHealth = 150.0f; // ��ǰ ü�� ����
            }

            if (right_track.GetComponent<Scroll_Right_Track>().destroy)
            {
                right_track.GetComponent<Scroll_Right_Track>().destroy = false; // ���� ��ȯ
                Scroll_Right_Track.trackHealth = 150.0f; // ��ǰ ü�� ����
            }
            repairing = false; // ���� ��ȯ
        }
    }

    /***********************************************************************
    *                           ��ũ ü�� ó��
    ***********************************************************************/
    bool flag = true;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet" && flag) // ��ź�� ���� ���
        {
            tank_health -= other.gameObject.GetComponentInParent<Shell_Control>().damage; // ��ũ ü�� ����
            flag = false; // ��ź ���� false, �ܱⰣ�� �ʹ� ���� �´� ��� �ڵ�ĸ + �ߺ����� ȸ��
            shell_detected_delay = 0.3f; // ���� ��ź �������� 0.3��
            Destroy(other); // ��ź ����
            // shell mesh �и��� ���� GetComponentInParent���, ��� shell �浹�� ��쿡 shell mesh �и� ����ؼ� �ۼ��� ��
            if (tank_health <= 0 && game_score >= goal_score) // ��ũ�� �ı������� �¸��� ���
            {
                SceneManager.LoadScene("WinnerScene", LoadSceneMode.Single);
            }
            else if (tank_health <= 0 && game_score < goal_score) // ��ũ�� �ı��ǰ� �й��� ���
            {
                SceneManager.LoadScene("LoserScene", LoadSceneMode.Single);
            }
        }
    }

    /***********************************************************************
    *                           ȭ�� GUI ����
    ***********************************************************************/

    void OnGUI()
    {
        /***********************************************************************
        *                             �⺻ UI ǥ��
        ***********************************************************************/

        GUI.Label(new Rect(20, Screen.height - 100, 128, 32), "���� : " + ((int)game_score).ToString(), tank_UI_style);
        GUI.Label(new Rect(20, Screen.height - 75, 128, 32), "ü�� : " + ((int)tank_health).ToString(), tank_UI_style);
        GUI.Label(new Rect(20, Screen.height - 50, 128, 32), "�ӵ� : " + ((int)tank_speed).ToString(), tank_UI_style);

        /***********************************************************************
        *                           ���� �� ��� ǥ��
        ***********************************************************************/

        if (repairing == true)
        {
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 25, 256, 32), "���� �߿��� ������ �� �����ϴ�.", warning_style);
            GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height / 2, 256, 32), "���� �Ϸ���� : " + (int)repair_time + "��", warning_style);
        }

        /***********************************************************************
        *                           ������ �ð� ǥ��
        ***********************************************************************/

        if (reload_delay > 0)
        {
            GUI.Label(new Rect(Screen.width / 2 - 63, Screen.height / 2 - 45, 256, 32), "Reloading", warning_style);
            GUI.Label(new Rect(Screen.width / 2 - 20, Screen.height / 2 + 15, 256, 32), (Mathf.Round(reload_delay * 10) * 0.1f).ToString(), warning_style);
        }

        /***********************************************************************
        *                            ��ǰ ���� ǥ��
        ***********************************************************************/

        if (Barrel_Control.destroy)
        {
            GUI.Label(new Rect(20, 20, 128, 32), "���� �ı���!", warning_style);
        } 
        else
        {
            GUI.Label(new Rect(20, 20, 128, 32), "���� ü�� : " + Barrel_Control.BarrelHealth, tank_UI_style);
        }

        if (left_track.GetComponent<Scroll_Left_Track>().destroy)
        {
            GUI.Label(new Rect(20, 45, 128, 32), "���� �˵� �ı���!", warning_style);
        }
        else
        {
            GUI.Label(new Rect(20, 45, 128, 32), "���� �˵� ü�� : " + Scroll_Left_Track.trackHealth, tank_UI_style);
        }

        if (right_track.GetComponent<Scroll_Right_Track>().destroy)
        {
            GUI.Label(new Rect(20, 70, 128, 32), "������ �˵� �ı���!", warning_style);
        }
        else
        {
            GUI.Label(new Rect(20, 70, 128, 32), "������ �˵� ü�� : " + Scroll_Right_Track.trackHealth, tank_UI_style);
        }
    }
}

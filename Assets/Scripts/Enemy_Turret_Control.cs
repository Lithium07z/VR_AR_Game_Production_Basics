using UnityEngine;

public class Enemy_Turret_Control : MonoBehaviour
{
    [SerializeField]
    private float shell_power = 4000.0f;
    [SerializeField] 
    private Enemy_Turret_Aim TurretAim = null;

    private int attack_num; // 공격할 부품 번호

    public bool destroy = false; // 파괴 여부

    public GameObject TargetPoint = null;

    public Transform shell;        // 포탄
    public GameObject player_tank; // 유저 탱크
    public Transform sp_point;     // 포탄 생성 지점

    public GameObject barrel; // 포신
    public GameObject[] player_tank_parts = new GameObject[5]; // 유저 탱크 부품저장 배열

    void Start()
    {
        player_tank_parts[0] = GameObject.Find("Panzer_VI_E");         // 유저 탱크 몸통 지정
        player_tank_parts[1] = GameObject.Find("Panzer_VI_E_Track_L"); // 유저 탱크 왼쪽 궤도 지정
        player_tank_parts[2] = GameObject.Find("Panzer_VI_E_Track_R"); // 유저 탱크 오른쪽 궤도 지정
        player_tank_parts[3] = GameObject.Find("Panzer_VI_E_Turret");  // 유저 탱크 포탑 지정
        player_tank_parts[4] = GameObject.Find("Panzer_VI_E_Gun");     // 유저 탱크 포신 지정
        player_tank = GameObject.Find("Panzer_VI_E"); // 유저 탱크 지정
        TargetPoint = player_tank; // 추적 목표 유저 탱크
    }

    void Update()
    {
        if (TurretAim == null)
            return;

        if (TargetPoint == null)
            Enemy_Turret_Aim.IsIdle = TargetPoint == null;
        else
            Enemy_Turret_Aim.AimPosition = TargetPoint.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Shell") {
            this.gameObject.GetComponentInParent<Enemy_Control>().enemy_health -= other.gameObject.GetComponentInParent<Shell_Control>().damage;
            // shell mesh 분리로 인해 GetComponentInParent사용, 모든 shell 충돌의 경우에 shell mesh 분리 고려해서 작성할 것
        }
    }

    /***********************************************************************
    *                          포탄 발사 구현부
    ***********************************************************************/

    public void Shooting()
    {
        this.GetComponent<AudioSource>().Play();
        attack_num = Random.Range(0, 5); // 플레이어 탱크의 몸통, 양쪽 궤도, 포탑, 포신 중 랜덤으로 하나 지정
        TargetPoint = player_tank_parts[attack_num]; // 지정된 부품을 바라봄

        Transform prefab_shell = Instantiate(shell, sp_point.transform.position, sp_point.transform.rotation); // 포탄 prefab 생성
        prefab_shell.GetComponent<Rigidbody>().AddForce(barrel.transform.forward * shell_power); // 포탄 발사
    }
}

using UnityEngine;

public class Enemy_Turret_Control : MonoBehaviour
{
    [SerializeField]
    private float shell_power = 4000.0f;
    [SerializeField] 
    private Enemy_Turret_Aim TurretAim = null;

    private int attack_num; // ������ ��ǰ ��ȣ

    public bool destroy = false; // �ı� ����

    public GameObject TargetPoint = null;

    public Transform shell;        // ��ź
    public GameObject player_tank; // ���� ��ũ
    public Transform sp_point;     // ��ź ���� ����

    public GameObject barrel; // ����
    public GameObject[] player_tank_parts = new GameObject[5]; // ���� ��ũ ��ǰ���� �迭

    void Start()
    {
        player_tank_parts[0] = GameObject.Find("Panzer_VI_E");         // ���� ��ũ ���� ����
        player_tank_parts[1] = GameObject.Find("Panzer_VI_E_Track_L"); // ���� ��ũ ���� �˵� ����
        player_tank_parts[2] = GameObject.Find("Panzer_VI_E_Track_R"); // ���� ��ũ ������ �˵� ����
        player_tank_parts[3] = GameObject.Find("Panzer_VI_E_Turret");  // ���� ��ũ ��ž ����
        player_tank_parts[4] = GameObject.Find("Panzer_VI_E_Gun");     // ���� ��ũ ���� ����
        player_tank = GameObject.Find("Panzer_VI_E"); // ���� ��ũ ����
        TargetPoint = player_tank; // ���� ��ǥ ���� ��ũ
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
            // shell mesh �и��� ���� GetComponentInParent���, ��� shell �浹�� ��쿡 shell mesh �и� ����ؼ� �ۼ��� ��
        }
    }

    /***********************************************************************
    *                          ��ź �߻� ������
    ***********************************************************************/

    public void Shooting()
    {
        this.GetComponent<AudioSource>().Play();
        attack_num = Random.Range(0, 5); // �÷��̾� ��ũ�� ����, ���� �˵�, ��ž, ���� �� �������� �ϳ� ����
        TargetPoint = player_tank_parts[attack_num]; // ������ ��ǰ�� �ٶ�

        Transform prefab_shell = Instantiate(shell, sp_point.transform.position, sp_point.transform.rotation); // ��ź prefab ����
        prefab_shell.GetComponent<Rigidbody>().AddForce(barrel.transform.forward * shell_power); // ��ź �߻�
    }
}

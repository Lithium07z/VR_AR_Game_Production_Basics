using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Turret_Control : MonoBehaviour
{
    [SerializeField] private Turret_Aim TurretAim = null;
    public bool destroy = false; 

    public Transform TargetPoint = null;

    private void Awake()
    {
        if (TurretAim == null)
            Debug.LogError(name + ": TurretController not assigned a TurretAim!");
    }

    private void Update()
    {
        if (TurretAim == null)
            return;

        if (TargetPoint == null)
            Turret_Aim.IsIdle = TargetPoint == null;
        else
            Turret_Aim.AimPosition = TargetPoint.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Bullet") // ��ź�� ���� ���
        {
            Tank_Control.tank_health -= other.gameObject.GetComponentInParent<Shell_Control>().damage; // ��ũ ü�� ����
            Destroy(other); // ��ź ����
        }
    }
}

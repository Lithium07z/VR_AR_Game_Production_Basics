using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Scene_Control : MonoBehaviour
{
    public static int level;
    public float rot_angle = 10.0f;
    private float time_count = 0;
    private bool flag = true;

    public GameObject tank;

    void Start()
    {
        tank = GameObject.Find("Panzer_VI_E");
    }

    void Update()
    {
        float current_angle = rot_angle * Time.deltaTime;
        time_count += Time.deltaTime;

        if ((int)time_count < 3 && flag)
        {
            this.transform.RotateAround(tank.transform.position, Vector3.up, current_angle);
        } 
        else if ((int)time_count < 5 && !flag)
        {
            this.transform.RotateAround(tank.transform.position, Vector3.up, current_angle);
        }
        else
        {
            this.transform.RotateAround(tank.transform.position, Vector3.up, -current_angle);
            if ((int)time_count == 8 && flag)
            {
                time_count = 0.0f;
                flag = false;
            }
            if ((int)time_count == 10 && !flag)
            {
                time_count = 0.0f;
                flag = false;
            }
        }
        
        
    }
}

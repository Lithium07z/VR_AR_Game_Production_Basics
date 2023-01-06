using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Defeat_Control : MonoBehaviour
{
    private float shell_power = 3000.0f; // Æ÷Åº ÆÄ¿ö
    public float delay = 2.0f;

    public bool check = true;

    public Transform shell; // Æ÷Åº

    public GameObject spawn_point;   // Æ÷Åº ½ºÆùÁöÁ¡
    public GameObject barrel;        // Æ÷½Å
    public GameObject turret;        // ÅÍ·¿

    void Start()
    {
        
    }

    void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }

        if (delay <= 0 && check == true)
        {
            Transform prefab_shell = Instantiate(shell, spawn_point.transform.position, spawn_point.transform.rotation);
            prefab_shell.GetComponent<Rigidbody>().AddForce(barrel.transform.forward * shell_power);
            check = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Map2", LoadSceneMode.Single);
        }
    }

}

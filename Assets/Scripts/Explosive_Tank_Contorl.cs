using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive_Tank_Contorl : MonoBehaviour
{
    public float delay = 2.0f;
    public bool check = true;

    public GameObject particle1;
    public GameObject particle2;
    public GameObject particle3;

    private GUIStyle UI_style = new GUIStyle();
    public Font font;

    void Start()
    {
        particle1.gameObject.SetActive(false);
        particle2.gameObject.SetActive(false);
        particle3.gameObject.SetActive(false);

        UI_style.font = font;
        UI_style.fontSize = 30;
        UI_style.normal.textColor = Color.black;
    }

    
    void Update()
    {
        if (delay > 0)
        {
            delay -= Time.deltaTime;
        }

        if (delay <= 0 && check)
        {
            particle1.gameObject.SetActive(true);
            particle2.gameObject.SetActive(true);
            particle3.gameObject.SetActive(true);
            this.transform.GetComponent<Rigidbody>().AddForce(150, 400, 400);
            this.GetComponent<AudioSource>().Play();
            check = false;
        } 
        else
        {
            particle1.gameObject.SetActive(false);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(50, Screen.height - 110, 128, 32), "R키를 눌러 다시 시작", UI_style);
        GUI.Label(new Rect(50, Screen.height - 75, 128, 32), "최고 점수 : " + Tank_Control.game_score.ToString(), UI_style);
    }
}

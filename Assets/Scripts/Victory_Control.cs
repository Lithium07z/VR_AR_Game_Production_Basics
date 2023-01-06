using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory_Control : MonoBehaviour
{
    private GUIStyle UI_style = new GUIStyle();
    public Font font;

    void Start()
    {
        UI_style.font = font;
        UI_style.fontSize = 30;
        UI_style.normal.textColor = Color.black;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Map2", LoadSceneMode.Single);
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(50, Screen.height - 110, 128, 32), "R키를 눌러 다시 시작", UI_style);
        GUI.Label(new Rect(50, Screen.height - 75, 128, 32), "최고 점수 : " + Tank_Control.game_score.ToString(), UI_style);
    }
}

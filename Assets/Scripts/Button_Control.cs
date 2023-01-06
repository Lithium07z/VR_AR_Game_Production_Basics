using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Button_Control : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BTNType currentType;
    public Transform buttonScale;
    Vector3 defaultScale;

    private void Start()
    {
        defaultScale = buttonScale.localScale;
    }

    public void OnBtnClick()
    {
        switch(currentType)
        {
            case BTNType.StartGame:
                SceneManager.LoadScene("Map2", LoadSceneMode.Single);
                break;

            case BTNType.HowtoPlay:
                SceneManager.LoadScene("HelpScene", LoadSceneMode.Single);
                break;

            case BTNType.Exit:
                Application.Quit();
                break;

            case BTNType.Easy:
                Main_Scene_Control.level = 1;
                break;

            case BTNType.Normal:
                Main_Scene_Control.level = 2;
                break;

            case BTNType.Hard:
                Main_Scene_Control.level = 3;
                break;

            case BTNType.Hell:
                Main_Scene_Control.level = 4;
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonScale.localScale = defaultScale;
    }
}
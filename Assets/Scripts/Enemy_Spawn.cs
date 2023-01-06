using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy_Spawn : MonoBehaviour
{
    public GameObject[] enemy_spawn = new GameObject[12]; // ���� ��ũ �������� ���� �迭
    public GameObject[] enemy_tank = new GameObject[6];   // ���� ��ũ ���� ���� �迭

    private float time_count = 300.0f; // ���� ����ð�
    private bool check = true;

    private GUIStyle world_UI_style = new GUIStyle(); // �⺻ UI ��Ÿ��
    public Font font; // ��Ʈ

    public List<int> random_number = new List<int>(); // �ߺ� ���� �������� ������ ���� ����Ʈ

    void Start()
    {
        EnemySpawn();
        world_UI_style.normal.textColor = Color.black;
        world_UI_style.fontSize = 25;
        world_UI_style.font = font;
    }

    void Update()
    {
        time_count -= Time.deltaTime;

        if ((int)time_count / 100 == 1 && check == true) // ���� ���� �� 100�� ��� 
        {
            EnemySpawn();
            check = false;
        }
        else if ((int)time_count / 100 == 0 && check == false) // ���� ���� �� 200�� ���
        {
            EnemySpawn();
            check = true;
        }

        if (time_count <= 0 && Tank_Control.game_score >= Tank_Control.goal_score)
        {
            SceneManager.LoadScene("WinnerScene", LoadSceneMode.Single);
        }
        else if (time_count <= 0 && Tank_Control.game_score < Tank_Control.goal_score)
        {
            SceneManager.LoadScene("LoserScene", LoadSceneMode.Single);
        }
    }

    void EnemySpawn()
    {
        RandomNumber(0, 12);
        for (int i = 0; i < 6; i++)
        {
            Instantiate(enemy_tank[i], enemy_spawn[random_number[i]].transform.position, enemy_spawn[random_number[i]].transform.rotation);
        }
    }

    void RandomNumber(int min, int max)
    {
        int current_Number = Random.Range(min, max);
        int count = 0;
        random_number.Clear();

        while (count < 11)
        {
            if (random_number.Contains(current_Number))
            {
                while (count < 11)
                {
                    current_Number = Random.Range(min, max);
                    if (!random_number.Contains(current_Number))
                    {
                        random_number.Add(current_Number);
                        count++;
                        break;
                    }
                }
            }
            else
            {
                random_number.Add(current_Number);
                count++;
            }
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 200, 20, 128, 32), "���� �ð� : " + ((int)time_count).ToString(), world_UI_style);
    }
}

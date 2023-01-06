using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll_Left_Track : MonoBehaviour
{
    public static float trackHealth = 150.0f; // �˵� ü��
    public static float scrollSpeed = 0.05f;  // �˵� ������ �ӵ�
    private float offset = 0.0f;              // �⺻ ������
    public bool destroy = false;              // �˵� �ı�����

    private Renderer r;

    void Start()
    {
        r = GetComponent<Renderer>();
    }

    void Update()
    {
        TextureMoving(); // �˵� �ؽ��� ������ ���� 
        TrackDestroy();  // �˵� �ı����� Ȯ��
    }

    /***********************************************************************
    *                        �˵� �ؽ��� ������ ����
    ***********************************************************************/

    void TextureMoving()
    {
        offset = (offset + Time.deltaTime * scrollSpeed) % 1f;
        r.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
    }

    /***********************************************************************
    *                           �˵� �ı� Ȯ��
    ***********************************************************************/

    void TrackDestroy()
    {
        if (trackHealth <= 0)  // �˵��� �ı��� ���
        {
            destroy = true;    // ���� ���� ����
            this.gameObject.SetActive(false); // �˵� ��Ȱ��ȭ
        }
    }
}

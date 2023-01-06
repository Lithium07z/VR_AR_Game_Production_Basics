using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll_Left_Track : MonoBehaviour
{
    public static float trackHealth = 150.0f; // 궤도 체력
    public static float scrollSpeed = 0.05f;  // 궤도 움직임 속도
    private float offset = 0.0f;              // 기본 오프셋
    public bool destroy = false;              // 궤도 파괴여부

    private Renderer r;

    void Start()
    {
        r = GetComponent<Renderer>();
    }

    void Update()
    {
        TextureMoving(); // 궤도 텍스쳐 움직임 구현 
        TrackDestroy();  // 궤도 파괴여부 확인
    }

    /***********************************************************************
    *                        궤도 텍스쳐 움직임 구현
    ***********************************************************************/

    void TextureMoving()
    {
        offset = (offset + Time.deltaTime * scrollSpeed) % 1f;
        r.material.SetTextureOffset("_MainTex", new Vector2(offset, 0f));
    }

    /***********************************************************************
    *                           궤도 파괴 확인
    ***********************************************************************/

    void TrackDestroy()
    {
        if (trackHealth <= 0)  // 궤도가 파괴된 경우
        {
            destroy = true;    // 현재 상태 변경
            this.gameObject.SetActive(false); // 궤도 비활성화
        }
    }
}

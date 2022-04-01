using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    float height;

    [SerializeField]
    float speed = 5f;
    GameManager GM;

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        height = collider.size.y;
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //게임오버시 리턴
        if (GM.isGameOver == true)
            return;
        
        //배경 이미지를 일정한 속도로 이동시킴
        transform.Translate(Vector2.down * speed * Time.deltaTime, Space.World);
        
        if(transform.position.y <= -height)
        {
            Reposition();
        }
    }

    void Reposition()//배경 이미지를 원위치 시킴
    {
        Vector3 offset = new Vector3(0, height * 2f - 1f);

        transform.position = transform.position + offset;
    }
}

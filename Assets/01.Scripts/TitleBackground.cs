using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBackground : MonoBehaviour
{
    float height;           //배경의 움직임이 끝나는 지점

    [SerializeField]
    float speed = 5f;       //속도


    // Start is called before the first frame update
    void Start()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        height = collider.size.y;
    }

    // Update is called once per frame
    void Update()
    { 
        transform.Translate(Vector2.down * speed * Time.deltaTime, Space.World);

        if (transform.position.y <= -height)
        {
            Reposition();
        }
    }

    //위치 초기화
    void Reposition()
    {
        Vector3 offset = new Vector3(0, height * 2f - 1f);

        transform.position = transform.position + offset;
    }
}

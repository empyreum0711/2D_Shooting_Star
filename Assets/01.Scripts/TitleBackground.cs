using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBackground : MonoBehaviour
{
    float height;

    [SerializeField]
    float speed = 5f;


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

    void Reposition()
    {
        Vector3 offset = new Vector3(0, height * 2f - 1f);

        transform.position = transform.position + offset;

    }
}

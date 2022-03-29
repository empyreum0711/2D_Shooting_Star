using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTest : MonoBehaviour
{
    public bool isTest = true;

    private void OnEnable() //활성화가 될때마다 호출이 되는 함수
    {
        Debug.Log("활성화됌!");
        isTest = true;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isTest = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointMove : EnemyMove
{
    [SerializeField]
    string patternName1;
    [SerializeField]
    string patternName2;

    Vector3[] wayPoints;
    Vector3[] wayPoints2;

    int moveIndex;

    int randomNum;

    // Start is called before the first frame update
    void Start()
    {
        randomNum = Random.Range(0, 2);
        GetComponent<Enemy>().enabled = true;

        int count = GameObject.Find(patternName1).transform.childCount;
        int count2 = GameObject.Find(patternName2).transform.childCount;

        wayPoints = new Vector3[count];

        wayPoints2 = new Vector3[count2];

        for (int i = 0; i < count; i++)
        {
            wayPoints[i] = GameObject.Find(patternName1).transform.GetChild(i).position;
        }

        for (int i = 0; i < count2; i++)
        {
            wayPoints2[i] = GameObject.Find(patternName2).transform.GetChild(i).position;
        }

        //방법2
        //Transform[] children = GameObject.Find(patternName).GetComponentsInChildren<Transform>();
        ////현재 children배열에는 부모 자신의 Transform까지도 포함이 되어있다.

        //for(int i=0; i< children.Length -1; i++ )
        //{
        //    wayPoints[i] = children[i + 1].position;
        //}

        if (randomNum == 0)
        {
            moveIndex = Random.Range(0, count);
        }
        else if (randomNum == 1)
        {
            moveIndex = Random.Range(0, count2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    protected override void Move()
    {
        if (randomNum == 0)
        {
            moveVec = (wayPoints[moveIndex] - transform.position).normalized; //normalized는 해당 벡터의 단위벡터를 반환한다.
        }
        else if (randomNum == 1)
        {
            moveVec = (wayPoints2[moveIndex] - transform.position).normalized; //normalized는 해당 벡터의 단위벡터를 반환한다.
        }
 

        //base.Move(); //부모한테 있는 Move함수가 발동이 된다.
        if (Vector2.Distance(transform.position, wayPoints[moveIndex]) < 0.1f)
        {
            GetComponent<Enemy>().isCanFire = true;

            moveIndex++;

            if (moveIndex >= wayPoints.Length)
            {
                moveIndex = 0;
            }
        }

        if (Vector2.Distance(transform.position, wayPoints2[moveIndex]) < 0.1f)
        {
            GetComponent<Enemy>().isCanFire = true;

            moveIndex++;

            if (moveIndex >= wayPoints2.Length)
            {
                moveIndex = 0;
            }
        }


        //일정한 속도로 이동하게 하기위해 단위벡터를 이용
        transform.Translate(moveVec * speed * Time.deltaTime, Space.World);
    }



}

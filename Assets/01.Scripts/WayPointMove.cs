using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayPointMove : EnemyMove
{
    [SerializeField]
    string patternName1;        //패턴1의 이름
    [SerializeField]
    string patternName2;        //패턴2의 이름

    Vector3[] wayPoints;        //웨이포인트1번
    Vector3[] wayPoints2;       //웨이포인트2번

    int moveIndex;              //움직일 지점의 갯수

    int randomStartPoint;       //스타트 위치를 랜덤하게 지정 또는 

    // Start is called before the first frame update
    void Start()
    {
        randomStartPoint = Random.Range(0, 2);
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

        if (randomStartPoint == 0)
        {
            moveIndex = Random.Range(0, count);
        }
        else if (randomStartPoint == 1)
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
        //웨이포인트 인덱스를 향하여 이동
        if (randomStartPoint == 0)
        {
            moveVec = (wayPoints[moveIndex] - transform.position).normalized; //normalized는 해당 벡터의 단위벡터를 반환한다.
        }
        //웨이포인트 인덱스를 향하여 이동
        else if (randomStartPoint == 1)
        {
            moveVec = (wayPoints2[moveIndex] - transform.position).normalized; //normalized는 해당 벡터의 단위벡터를 반환한다.
        }
 
        //base.Move(); //부모한테 있는 Move함수가 발동이 된다.
        //웨이포인트1의 시작점에 도착하면
        if (Vector2.Distance(transform.position, wayPoints[moveIndex]) < 0.1f)
        {
            GetComponent<Enemy>().isCanFire = true;

            moveIndex++;

            if (moveIndex >= wayPoints.Length)
            {
                moveIndex = 0;
            }
        }

        //웨이포인트2의 시작점에 도착하면
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

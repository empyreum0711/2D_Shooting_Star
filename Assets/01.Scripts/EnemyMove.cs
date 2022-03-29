using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    protected Vector2 startPos;     //리젠 시작위치

    [SerializeField]
    protected GameObject Enemy;     //적 비행체 오브젝트

    [SerializeField]
    protected float speed = 5f;     //움직임 속도

    protected bool isAwake;         //적 비행체의 공격을 시작할지

    [SerializeField]
    protected float awakeDist = 5f; //움직

    protected Vector2 moveVec;      //적의 Vector 위치

    //베지어 함수를 위한 변수
    protected Vector3 pos1;
    protected Vector3 pos2;
    protected Vector3 pos3;
    protected Vector3 pos4;
    [SerializeField] protected float vezierValue;
    [SerializeField] protected bool isTrun;

    int randomNum;          //어떤 움직임을 할것인지
    int randomStart;        //어떤 시작점에서 시작할지
    // Start is called before the first frame update
    void Start()
    {
        randomNum = Random.Range(0, 2);
        randomStart = Random.Range(0, 2);

        startPos = transform.position;
        vezierValue = 0.0f;
        pos1 = new Vector3(-4.57f, 3.5f, 0.0f);
        pos2 = new Vector3(-4.57f, 1.0f, 0.0f);
        pos3 = new Vector3(4.57f, 1.0f, 0.0f);
        pos4 = new Vector3(4.57f, 3.5f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (randomNum == 0)
        {
            Move();
        }
        else if (randomNum == 1)
        {
            BeizerMove();
        }
    }

    //좌우 이동 함수
    protected virtual void Move()
    {
        if (isAwake == false)
        {
            moveVec = Vector2.down;

            if (Vector2.Distance(transform.position, startPos) > awakeDist)
            {
                isAwake = true;
                moveVec = Vector2.right;

                GetComponent<Enemy>().isCanFire = true;
            }
        }
        else
        {
            BasicMove();
        }

        transform.Translate(moveVec * speed * Time.deltaTime, Space.World);
    }

    //기본적인 좌우 이동
    protected void BasicMove()
    {
        if (5 - transform.position.x < 0.1f)
        {
            moveVec = Vector2.left;
        }
        else if (-5 - transform.position.x > 0.1f)
        {
            moveVec = Vector2.right;
        }

    }

    //베지어 함수
    protected Vector3 Bezier(Vector3 Pos_1, Vector3 Pos_2,
        Vector3 Pos_3, Vector3 Pos_4, float value)
    {
        Vector3 PosA = Vector3.Lerp(Pos_1, Pos_2, value);
        Vector3 PosB = Vector3.Lerp(Pos_2, Pos_3, value);
        Vector3 PosC = Vector3.Lerp(Pos_3, Pos_4, value);

        Vector3 PosD = Vector3.Lerp(PosA, PosB, value);
        Vector3 PosE = Vector3.Lerp(PosB, PosC, value);

        Vector3 PosF = Vector3.Lerp(PosD, PosE, value);

        return PosF;
    }

    //곡선 이동 함수
    void BeizerMove()
    {
        if(!isAwake)
        {
            if (randomStart == 0)
            {
                moveVec = (pos1 - transform.position).normalized;//시작점 방향(pos1)을 향해서
            }
            else if (randomStart == 1)
            {
                moveVec = (pos4 - transform.position).normalized;//시작점 방향(pos4)을 향해서
            }
            if (Vector2.Distance(transform.position, pos1) < 0.1f
                && randomStart == 0)
            {
                vezierValue = 0.0f;
                isAwake = true;
                GetComponent<Enemy>().isCanFire = true;
            }
            else if (Vector2.Distance(transform.position, pos4) < 0.1f
                && randomStart == 1)
            {
                vezierValue = 1.0f;
                isAwake = true;
                GetComponent<Enemy>().isCanFire = true;
            }
            //시작지점으로 이동
            transform.Translate(moveVec * speed * Time.deltaTime, Space.World);
        }
        else//isAwake일 때
        {
            if (vezierValue >= 1.0f)
            {
                isTrun = true;
            }
            else if (vezierValue <= 0.0f)
            {
                isTrun = false;
            }
            if (!isTrun)
            {
                vezierValue += 0.5f * Time.deltaTime;
            }
            else if (isTrun)
            {
                vezierValue -= 0.5f * Time.deltaTime;
            }
            //곡선이동
            Enemy.transform.position = Bezier(pos1, pos2, pos3, pos4, vezierValue);
        }      
    }
}

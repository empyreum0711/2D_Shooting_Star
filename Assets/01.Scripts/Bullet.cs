using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    float angle;            //총알의 각도

    [SerializeField]
    float speed = 8f;       //총알의 속도

    Vector2 moveVector;     //총알이 움직일 방향

    Vector2 rightBorder;    //우측 상단의 지점
    Vector2 leftBorder;     //좌측 하단의 지점

    bool isPlayerBullet = true; //적탄인지 플레이어탄인지에 대한 정보

    [SerializeField]
    float damage;           //데미지

    [SerializeField]
    GameObject effectPrefab;    //폭발시 나오는 이펙트

    bool isFired;               //공격할 수 있는지의 여부


    private void OnEnable()
    {
        isFired = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        rightBorder = GameObject.Find("Right").transform.position;//우상단으로 최대 이동할 수 있는 좌표
        leftBorder = GameObject.Find("Left").transform.position;//좌하단으로 최대 이동할 수 있는 좌표
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveVector * speed * Time.deltaTime, Space.World);

        if (transform.position.x > rightBorder.x + 2f
            || transform.position.y > rightBorder.y + 2f
            || transform.position.x < leftBorder.x - 2f
            || transform.position.y < leftBorder.y - 2f)
        {
            gameObject.SetActive(false);
        }
    }

    //총알을 세팅하는 함수
    public void SetBullet(float angle, bool isPlayerBullet)
    {
        this.isPlayerBullet = isPlayerBullet;
        this.angle = angle;

        moveVector.x = Mathf.Cos(angle * Mathf.Deg2Rad);
        moveVector.y = Mathf.Sin(angle * Mathf.Deg2Rad);

        transform.right = moveVector;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFired == true)
            return;

        if (isPlayerBullet == true)//플레이어가 쏜 탄알일때
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if(enemy != null && enemy.isActiveAndEnabled)
            {
                isFired = true;

                GameObject effect = Instantiate(effectPrefab);//이펙트 생성
                effect.transform.position = transform.position;

                enemy.OnDamaged(GlobalValue.g_Damage);//적한테 데미지를 입힌다

                gameObject.SetActive(false);
            }
        }//if (isPlayerBullet == true)//플레이어가 쏜 탄알일때
        else//적이 쏜 탄알일때
        {
            if (collision.CompareTag("Player"))
            {
                if(collision.GetComponent<PlayerCtrl>().OnDamaged(damage) == true)//플레이어한테 데미지를 입힌다.
                {
                    isFired = true;

                    GameObject effect = Instantiate(effectPrefab);//이펙트 생성
                    effect.transform.position = transform.position;

                    gameObject.SetActive(false);
                }              
            }
        }//if (isPlayerBullet == false)//적이 쏜 탄알일때
    }
}

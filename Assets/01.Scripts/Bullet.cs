using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    float angle;

    [SerializeField]
    float speed = 8f;

    Vector2 moveVector;

    Vector2 rightBorder;
    Vector2 leftBorder;

    bool isPlayerBullet = true; //적탄인지 플레이어탄인지에 대한 정보

    [SerializeField]
    float damage;

    [SerializeField]
    GameObject effectPrefab;

    bool isFired;


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
            //Destroy(gameObject);
        }

    }

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
                //Destroy(gameObject);
            }
        }
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
                    //Destroy(gameObject);
                }
               
            }
        }
    }
}

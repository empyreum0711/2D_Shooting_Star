using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    float reFireTime;           //공격의 텀

    [SerializeField]
    GameObject bulletPrefab;    //레이저 프리펩

    float fireAngle;            //레이저의 각도

    bool isFire;//공격중인지에 대한 정보

    public bool isCanFire;      //시작점 위치에 도착했는지

    [SerializeField]
    float hp = 100;             //적 비행체의 체력

    SpriteRenderer renderer;

    [SerializeField]
    Image hpBar;                //체력바 이미지

    public Text nameUI;     //적의 이름


    [SerializeField]
    int score;              //점수

    bool isDead;            //사망 상태 체크
    [SerializeField] GameObject boomPrefab;     //죽었을 때 이미지

    GameManager GM;         //게임매니저

    int randomNum;          //어떤 패턴의 공격을 할지

    void Awake()
    {
        randomNum = Random.Range(0, 2);

        renderer = GetComponent<SpriteRenderer>();

        hpBar = GetComponentInChildren<Image>();

        nameUI = GetComponentInChildren<Text>();
        nameUI.text = gameObject.name;

        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFire == false && isCanFire == true)
        {
            if (gameObject.name == "Enemy")  //적이 Enemy의 이름을 가졌다면
            {
                if (randomNum == 0)
                {
                    StartCoroutine(Fire());
                }
                else
                {
                    StartCoroutine(Fire2());
                }
            }//if (gameObject.name == "Enemy")  //적이 Enemy의 이름을 가졌다면
            else                            //적이 Enemy2의 이름을 가졌다면
            {
                if (randomNum == 0)
                {
                    StartCoroutine(Fire3());
                }
                else
                {
                    StartCoroutine(Fire4());
                }
            }//if (gameObject.name == "Enemy2")  //적이 Enemy2의 이름을 가졌다면
        }//if (isFire == false && isCanFire == true)
    }

    //기본 공격 함수 ↓로만 공격
    IEnumerator Fire()
    {
        FireBullet(-90f);

        isFire = true;

        yield return new WaitForSeconds(reFireTime);

        isFire = false;
    }

    //전 방위에 15개의 총알을 30도 간격으로 발사함
    IEnumerator Fire2()
    {
        isFire = true;

        for (int i = 0; i < 15; i++)    //총알의 갯수
        {
            FireBullet(fireAngle);

            fireAngle += 30;
        }

        fireAngle = 0;

        yield return new WaitForSeconds(3.0f);//공격을 재시작하는시간

        isFire = false;
    }

    //4방향으로 공격 ↑↓←→방향으로 공격하며 반시계방향으로 회전함
    IEnumerator Fire3()
    {
        isFire = true;

        for (int i = 0; i < 90; i++)//총알의 갯수
        {
            FireBullet(fireAngle);
            FireBullet(fireAngle - 180f);
            FireBullet(fireAngle - 90f);
            FireBullet(fireAngle - 270f);
            yield return new WaitForSeconds(0.3f);

            fireAngle += 3;

            if (fireAngle % 3 == 0)
            {
                fireAngle += 10;
            }
        }
        fireAngle = 0;

        yield return new WaitForSeconds(reFireTime);

        isFire = false;
    }

    //3방향으로 공격 ↙↓↘으로 10번 공격후 2초대기한다
    IEnumerator Fire4()
    {
        isFire = true;

        for (int i = 0; i < 10; i++)//총알의 갯수
        {

            FireBullet(fireAngle - 90f);
            FireBullet(fireAngle - 80f);
            FireBullet(fireAngle - 100f);

            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(2.0f);

        isFire = false;
    }

    //적 비행체의 이름 표시
    public void SetName(string name)
    {
        gameObject.name = name;
        nameUI.text = name;
    }

    //적 비행체가 공격하는 함수
    void FireBullet(float angle)
    {
        GameObject bullet = PoolingManager.Instance.GetObject(bulletPrefab.name);

        bullet.transform.position = transform.position;

        bullet.GetComponent<Bullet>().SetBullet(angle, false);
    }

    //적의 비행체가 피해를 입었을 때
    public void OnDamaged(float damage)
    {
        if (isDead == true)
            return;

        hp -= damage;
        renderer.color = Color.red;
        Invoke("ReturnColor", 0.1f);//0.1초 있다가 ReturnColor라는 함수를 호출한다.

        if (hp <= 0)
        {
            isDead = true;

            GM.AddScore(score);
            GameObject boom = Instantiate(boomPrefab); //폭발 이펙트 생성
            boom.transform.position = transform.position;

            Camera.main.GetComponent<CameraShaker>().CameraShake(0.5f, 0.5f);

            Destroy(gameObject);
        }//if (hp <= 0)
        else//if (hp >= 0)
        {
            hpBar.fillAmount = hp / 100f;
        }
    }

    //색을 원래대로 바꿔는 함수
    void ReturnColor()
    {
        renderer.color = Color.white;
    }

}

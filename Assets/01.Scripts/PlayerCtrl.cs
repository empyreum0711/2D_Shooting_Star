using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;

    Vector2 moveVector;

    Vector2 leftBorder;
    Vector2 RightBorder;

    Vector3 setPos;

    [SerializeField]
    GameObject bulletPrefab;

    public float hp;
    //public int Damage = 10;
    SpriteRenderer renderer;

    [SerializeField]
    Color color;

    bool isDead;
    bool isDamageable; //데미지를 받을 수 있는 상태에 대한 정보

    [SerializeField] GameObject boomPrefab;
    public Image m_PlayerHpbar = null;

    GameManager GM;
    // Start is called before the first frame update
    void Start()
    {
        //플레이어가 최대로 움직일 수 있는 지점
        leftBorder = GameObject.Find("Left").transform.position;
        RightBorder = GameObject.Find("Right").transform.position;
        //플레이어가 최대로 움직일 수 있는 지점
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

        renderer = GetComponent<SpriteRenderer>();

       
        StartCoroutine(Rebirth());

        hp = GlobalValue.g_PlayerHp;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Fire();
    }

    //플레이어 리스폰
    IEnumerator Rebirth()
    {
        int count = 0;

        while(count < 10)
        {
            count++;

            renderer.color = Color.clear;

            yield return new WaitForSeconds(0.1f);

            renderer.color = Color.white;

            yield return new WaitForSeconds(0.1f);

        }
        isDamageable = true;
    }

    //플레이어가 공격
    void Fire()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.Instance.PlayEffSound("gun");
                //Bullet생성 및 셋팅
                //GameObject bullet = Instantiate(bulletPrefab);
                GameObject bullet = PoolingManager.Instance.GetObject(bulletPrefab.name);

                bullet.transform.position = transform.position; //위치 설정

                float angle = 90f;
                bullet.GetComponent<Bullet>().SetBullet(angle, true);                    
        }

    }

    //플레이어의 움직임
    void Move()
    {
        moveVector.x = Input.GetAxisRaw("Horizontal");
        moveVector.y = Input.GetAxisRaw("Vertical");

        transform.Translate(moveVector * speed * Time.deltaTime, Space.World);

        //Clamp이용하기 (최솟값 또는 최대값으로 자르는 함수)
        setPos.Set(
            Mathf.Clamp(transform.position.x, leftBorder.x, RightBorder.x),
            Mathf.Clamp(transform.position.y, leftBorder.y, RightBorder.y),
            0
            );
        transform.position = setPos;
    }

    //플레이어가 피격받았을 때
    public bool OnDamaged(float damage)
    {
        if (isDead == true || isDamageable == false)
            return false;

        hp -= (int)damage;

        m_PlayerHpbar.fillAmount = hp / GlobalValue.g_PlayerHp;

        renderer.color = color;
        Invoke("ReturnColor", 0.1f);

        if (hp <= 0)
        {
            isDead = true;
           
            GM.LoseLife();
            GameObject boom = Instantiate(boomPrefab); //폭발 이펙트 생성
            boom.transform.position = transform.position;

            Camera.main.GetComponent<CameraShaker>().CameraShake(0.5f, 0.5f);

            Destroy(gameObject);
        }

        return true;
    }

    //색을 원래대로 바꿔는 함수
    void ReturnColor()
    {
        renderer.color = Color.white;
    }

}

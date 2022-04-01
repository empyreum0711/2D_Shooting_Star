using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    static GameManager m_instance; //private변수

    public bool isGameOver;         //게임오버 여부

    int score;                      //플레이어의 최고 점수
    int point;                      //플레이어의 최고 포인트

    int playerLife = 0;             //플레이어의 라이프

    [SerializeField] GameObject playerPrefab;   //플레이어 오브젝트

    [SerializeField] Texture2D[] numTextures = new Texture2D[10];       //라이프 숫자이미지를 교체할 이미지의 배열
    [SerializeField] Image lifeNumImage;                                //라이프 숫자이미지
    [SerializeField] Text scoreText;                                    //스코어 텍스트
    [SerializeField] Text MyPointTxt;                                   //포인트 텍스트
    [SerializeField] GameObject gameOverUI;                             //게임오버UI
        
    string UpdateUrl;                                                   //Update Php서버 Url주소

    bool m_PauseOnOff = false;                                          //Pause상태체크

    [SerializeField] GameObject m_PauseUI = null;                       //Pause상태일 때 나오는 UI
    [SerializeField] Button m_PauseRSBtn = null;                        //Pause의 리스타트 버튼
    [SerializeField] Button m_PauseLobbyBtn = null;                     //Pause의 로비 버튼
    [SerializeField] Button m_GameOverRSBtn = null;                     //게임오버일때 리스타트 버튼
    [SerializeField] Button m_GameOveLobbyBtn = null;                   //게임오버일때 로비 버튼

    private void Awake() //Start이전에 실행이되는 함수
    {
        if (m_instance == null)
            m_instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.InitData();

        SoundManager.Instance.PlayBGM("sound_bgm_island_001");
        playerLife = GlobalValue.g_Life;
        point = GlobalValue.g_MyPoint;
        
        UpdateLifeUI();
        scoreText.text = score.ToString();

        //Pause의 리셋버튼을 눌렀을 때
        if (m_PauseRSBtn != null)
            m_PauseRSBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("SampleScene");
                SoundManager.Instance.PlayBGM("sound_bgm_island_001");
                Time.timeScale = 1;
            });

        //Pause의 로비버튼을 눌렀을 때
        if (m_PauseLobbyBtn != null)
            m_PauseLobbyBtn.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LobbyScene");
                SoundManager.Instance.PlayBGM("Elec_Background");
                Time.timeScale = 1;
            });

        //게임오버시 리셋버튼을 눌렀을 때
        if (m_GameOverRSBtn != null)
            m_GameOverRSBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_MyPoint = point;
                if (GlobalValue.g_BestScore <= this.score)
                {
                    GlobalValue.g_BestScore = this.score;
                }
                StartCoroutine(UpdateDataCo());
                SceneManager.LoadScene("SampleScene");
                SoundManager.Instance.PlayBGM("sound_bgm_island_001");
                Time.timeScale = 1;
            });

        //게임오버시 로비버튼을 눌렀을 때
        if (m_GameOveLobbyBtn != null)
            m_GameOveLobbyBtn.onClick.AddListener(() =>
            {
                GlobalValue.g_MyPoint = point;
                if (GlobalValue.g_BestScore <= this.score)
                {
                    GlobalValue.g_BestScore = this.score;
                }
                StartCoroutine(UpdateDataCo());
                SceneManager.LoadScene("LobbyScene");
                SoundManager.Instance.PlayBGM("Elec_Background");
                Time.timeScale = 1;
            });

        UpdateUrl = "http://empyreum0711.dothome.co.kr/ShootingDB/Updatedata.php";
    }

    // Update is called once per frame
    void Update()
    {
        MyPointTxt.text = point.ToString();

        //Esc버튼을 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SoundManager.Instance.PlayGUISound("Pop");
            m_PauseOnOff = !m_PauseOnOff;
            m_PauseUI.SetActive(m_PauseOnOff);
            if (m_PauseOnOff)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public void AddScore(int score) //적들이 파괴될때 호출이 될 함수
    {
        int AddPoint = Random.Range(50, 100);
        this.score += score;

        point += AddPoint;//AddPoint;

        scoreText.text = this.score.ToString(); //UI에 표시하기
        MyPointTxt.text = point.ToString();
    }

    //유저의 라이프 상태 이미지를 갱신하는 함수
    public void UpdateLifeUI()
    {
        lifeNumImage.sprite = Sprite.Create(numTextures[playerLife],
            lifeNumImage.sprite.rect,
            lifeNumImage.rectTransform.pivot);
    }

    
    public void LoseLife() //플레이어가 죽을때 호출이될 함수
    {
        playerLife--;

        if (playerLife < 0)
        {
            gameOverUI.SetActive(true); //게임오버UI 표시하기
            isGameOver = true;
            Time.timeScale = 0;
        }
        else
        {
            Instantiate(playerPrefab);
            UpdateLifeUI(); //UI에 Life 표시하기
        }

    }

    //서버에 유저의 정보를 갱신하는 코루틴함수
    IEnumerator UpdateDataCo()
    {
        if (GlobalValue.g_Unique_ID == "")
        {
            SceneManager.LoadScene("LobbyScene");
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_score", GlobalValue.g_BestScore);
        form.AddField("Input_point", GlobalValue.g_MyPoint);

        WWW webRequest = new WWW(UpdateUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string a_ReStr = enc.GetString(webRequest.bytes);

        if (a_ReStr.Contains("UpDateSuccess~") == true)
        {
            Debug.Log("UpdateScoreSuccess");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;




public class LobbyManager : MonoBehaviour
{
    public Button m_StartBtn = null;        //스타트버튼
    public Button m_RankingBtn = null;      //랭킹버튼
    public Button m_StoreBtn = null;        //상점버튼
    public Button m_LogOuttn = null;        //로그아웃버튼

    string MyScoreUrl;                      //점수 체크를 위한 Url

    List<UserInfo> m_RkList = new List<UserInfo>(); //유저의 순위를 확인하기 위한 리스트

    int m_My_Rank = 0;                              //나의 랭킹
    bool a_IsFirst = true;                          //한번만 실행
    bool m_EscOnOff = false;                        //Esc를 눌렀는지의 상태
    [SerializeField] GameObject m_EscUI = null;     //Esc UI
    [SerializeField] Button m_ContinueBtn = null;   //Esc의 컨티뉴 버튼
    [SerializeField] Button m_ExitBtn = null;       //Esc의 Exit 버튼

    //사운드 판넬 관련 변수
    [Header("SoundPanel")]
    public Button m_SoundBtn = null;            //사운드 버튼
    public GameObject m_SoundUI = null;         //사운드 UI

    public GUISkin mySkin = null; // Asset - Create - GUI Skin 생성 후 만들어진 파일에 폰트를 넣어준다.


    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.InitData();

        //게임 시작 버튼
        if (m_StartBtn != null)
            m_StartBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayGUISound("Pop");
                SceneManager.LoadScene("SampleScene");
            });

        //랭킹 버튼
        if (m_RankingBtn != null)
            m_RankingBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayGUISound("Pop");
                SceneManager.LoadScene("RankScene");
            });

        //상점 버튼
        if (m_StoreBtn != null)
            m_StoreBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayGUISound("Pop");
                SceneManager.LoadScene("StoreScene");
            });

        //로그아웃 버튼
        if (m_LogOuttn != null)
            m_LogOuttn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayGUISound("Pop");
                GlobalValue.g_Unique_ID = "";
                GlobalValue.g_NickName = "";
                GlobalValue.g_BestScore = 0;
                GlobalValue.g_MyPoint = 0;
                GlobalValue.g_PlayerHp = 100;
                GlobalValue.g_Damage = 10;
                GlobalValue.g_Life = 3;
                GlobalValue.m_CrDataList.Clear();
                SceneManager.LoadScene("TitleScene");
            });

        //Esc를 눌렀을 때 컨티뉴 버튼
        if (m_ContinueBtn != null)
            m_ContinueBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayGUISound("Pop");
                m_EscUI.SetActive(false);
                m_EscOnOff = false;
                Time.timeScale = 1;
            });

        //Esc를 눌렀을 때 Exit 버튼
        if (m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayGUISound("Pop");
                Application.Quit();
            });

        //사운드 버튼
        if (m_SoundBtn != null)
        {
            m_SoundBtn.onClick.AddListener(() =>
            {
                if (m_SoundUI == null)
                    m_SoundUI = Resources.Load("Prefab/SoundUI") as GameObject;

                GameObject a_SoundUIObj = (GameObject)Instantiate(m_SoundUI);
                a_SoundUIObj.transform.SetParent(this.transform, false);
                Time.timeScale = 0.0f;
            });
        }

        MyScoreUrl = "http://empyreum0711.dothome.co.kr/ShootingDB/Get_ID_Rank.php";
    }

    // Update is called once per frame
    void Update()
    {
        if (a_IsFirst == true)
        {
            if (GlobalValue.g_Unique_ID != "")
                StartCoroutine(GetmyScoreCo());

            a_IsFirst = false;
        }

        //Esc를 눌렀을 때
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SoundManager.Instance.PlayGUISound("Pop");
            m_EscOnOff = !m_EscOnOff;
            m_EscUI.SetActive(m_EscOnOff);
            if (m_EscOnOff)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }//if (Input.GetKeyDown(KeyCode.Escape))
    }

    //랭킹을 띄워주는 코루틴 함수
    IEnumerator GetmyScoreCo()
    {
        if (GlobalValue.g_Unique_ID == "")
            yield break;

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        var webRequest = new WWW(MyScoreUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8; //<--이렇게 해야 안드로이드에서 한글이 안깨진다.
        string a_ReStr = enc.GetString(webRequest.bytes);

        if (string.IsNullOrEmpty(webRequest.error))
        {
            if (a_ReStr.Contains("Get_score_list_Success~") == true)
            {
                //점수를 표시하는 함수를 호출
                RecRankList_MyRank(a_ReStr);
            }
        }
        else
        {
            Debug.Log("Error : " + webRequest.error);
        }
    }

    //점수를 표시하는 함수
    void RecRankList_MyRank(string strJsonData)
    {
        m_RkList.Clear();

        //JSON 파일 파싱
        var N = JSON.Parse(strJsonData);

        int ranking = 0;
        UserInfo a_UserNd;
        for (int i = 0; i < N["RkList"].Count; i++)
        {
            ranking = i + 1;
            string userID = N["RkList"][i]["user_id"];
            string nick_name = N["RkList"][i]["nick_name"];
            int best_score = N["RkList"][i]["best_score"].AsInt;

            a_UserNd = new UserInfo();
            a_UserNd.m_ID = userID;
            a_UserNd.m_Nick = nick_name;
            a_UserNd.m_BestScore = best_score;

            m_RkList.Add(a_UserNd);
        }//for (int i = 0; i < N["RkList"].Count; i++)
        m_My_Rank = N["my_rank"].AsInt;
    }

    //GUI출력
    private void OnGUI()
    {
        GUI.skin = mySkin;  //내가 만든 GUI스킨을 적용한다

        GUI.Label(new Rect(350, 300, 1500, 60),
                          "<color=#ffff00><size=24>" +
                          "내정보 : 별명(" + GlobalValue.g_NickName +
                          ") : 순위(" + m_My_Rank.ToString() + "등) : 점수(" +
                          GlobalValue.g_BestScore.ToString() + "점) : 포인트(" +
                          GlobalValue.g_MyPoint.ToString() + "점)" +
                          "</size></color>");
    }
}

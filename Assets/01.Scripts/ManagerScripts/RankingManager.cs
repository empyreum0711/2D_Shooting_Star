using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SimpleJSON;


public class UserInfo
{
    public string m_ID = "";
    public string m_Nick = "";
    public int m_BestScore = 0;
    public UserInfo()
    {

    }
};

public class RankingManager : MonoBehaviour
{


    public Button m_LobbyBtn = null;                //로비로 가는 버튼
    string RankingUrl;                              //랭킹서버연결 url
    List<UserInfo> m_RkList = new List<UserInfo>(); //랭킹 리스트
    int m_My_Rank = 0;                              //나의 등수
    bool a_IsFirst = true;                          //한번만 실행하기 위한 bool형 변수

    //ESC를 눌렀을때...
    bool m_EscOnOff = false;                        //Esc상태 버튼
    [SerializeField] GameObject m_EscUI = null;     //Esc를 눌렀을 때 나오는 UI 오브젝트
    [SerializeField] Button m_ContinueBtn = null;   //게임을 계속하기 위한 버튼
    [SerializeField] Button m_ExitBtn = null;       //게임을 끄기위한 버튼

    public GUISkin mySkin = null; // Asset - Create - GUI Skin 생성 후 만들어진 파일에 폰트를 넣어준다.

    // Start is called before the first frame update
    void Start()
    {

        if (m_LobbyBtn != null) //로비버튼을 눌렀을 때
            m_LobbyBtn.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayGUISound("Pop");
                SceneManager.LoadScene("LobbyScene");
            });

        if (m_ContinueBtn != null)  //컨티뉴버튼을 눌렀을 때
            m_ContinueBtn.onClick.AddListener(() =>
            {
                m_EscUI.SetActive(false);
                m_EscOnOff = false;
                Time.timeScale = 1;
            });

        if (m_ExitBtn != null)  //Exit버튼을 눌렀을 때
            m_ExitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        RankingUrl = "http://empyreum0711.dothome.co.kr/ShootingDB/Get_ID_Rank.php";
    }

    // Update is called once per frame
    void Update()
    {
        if (a_IsFirst == true)
        {
            if (GlobalValue.g_Unique_ID != "")
                StartCoroutine(GetScoreListCo());

            a_IsFirst = false;
        }
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

    //서버의 저장된 정보를 불러옴
    IEnumerator GetScoreListCo()
    {
        if (GlobalValue.g_Unique_ID == "")
        {
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        var webRequest = new WWW(RankingUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string a_ReStr = enc.GetString(webRequest.bytes);

        if (string.IsNullOrEmpty(webRequest.error))
        {
            if (a_ReStr.Contains("Get_score_list_Success~") == true)
            {
                RecRankList_MyRank(a_ReStr);
            }
        }
        else
        {
            Debug.Log("Error : " + webRequest.error);
        }
    }

    //나의 랭크를 확인
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
            Debug.Log(N["RkList"][i]["user_id"]);
            m_RkList.Add(a_UserNd);
        }

        m_My_Rank = N["my_rank"].AsInt;
    }

    //GUI출력
    private void OnGUI()
    {
        GUI.skin = mySkin;  //내가 만든 GUI스킨을 적용한다

        //자신의 랭킹 노출시키기
        GUI.Label(new Rect(450, 600, 1500, 60), "<color=#ffff00><size=24>" +
            "내정보 : 별명(" + GlobalValue.g_NickName + ") : 순위(" + m_My_Rank.ToString() +
            "등) : 점수(" + GlobalValue.g_BestScore.ToString() + ")점" + "</size></color>");

        //전체 랭킹 노출시키기
        string a_TempStr;
        for (int i = 0; i < m_RkList.Count; i++)
        {
            a_TempStr = (i + 1).ToString() + "등 : " +
                "(" + m_RkList[i].m_Nick + ") : " +
                m_RkList[i].m_BestScore.ToString() + " 점";


            if (m_RkList[i].m_ID == GlobalValue.g_Unique_ID)    //나의 순위 보기
            {
                GUI.Label(new Rect(450, (i * 50) + 100, 1500, 60),
                    "<color=#03ff39><size=25>" + a_TempStr + "</size></color>");
            }
            else                                                //다른 사람의 순위 보기
            {
                GUI.Label(new Rect(450, (i * 50) + 100, 1500, 60),
                    "<color=#ffffff><size=25>" + a_TempStr + "</size></color>");
            }
        }//for(int i= 0; i < m_RkList.Count; i++)
    }
}

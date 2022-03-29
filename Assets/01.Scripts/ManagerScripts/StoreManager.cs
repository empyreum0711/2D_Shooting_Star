using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;


public class StoreManager : MonoBehaviour
{
    private string g_Message = "";
    public Button m_GoLobby = null;
    public GameObject PanelRoot;
    ItemNodeCtrl[] m_CrNodeList;
    public Text m_MyPoint = null;
    //public Text[] ItemPriceText = null;

    //-- 지금 뭘 구입하려고 시도한 건지?
    CharType m_BuyCrType;
    string m_SvStrJson = ""; //서버에 전달하려고 하는 JSON형식이 뭔지?
    int m_SvMyPoint = 0;  //서버에 전달하려고 하는 차감된 내포인트가 얼마인지?
    //-- 지금 뭘 구입하려고 시도한 건지?

    string BuyRequestUrl;
    string SpecUpUrl;

    float m_Delay = 0.0f;

    //ESC를 눌렀을때...
    bool m_EscOnOff = false;
    [SerializeField] GameObject m_EscUI = null;
    [SerializeField] Button m_ContinueBtn = null;
    [SerializeField] Button m_ExitBtn = null;

    public GUISkin mySkin = null;

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.InitData();

        CharInfo charInfo;

        m_MyPoint.text = "" + GlobalValue.g_MyPoint;

        if (m_GoLobby != null)
            m_GoLobby.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlayGUISound("Pop");
                UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
            });

        if (m_ContinueBtn != null)
            m_ContinueBtn.onClick.AddListener(() =>
            {
                m_EscUI.SetActive(false);
                m_EscOnOff = false;
                Time.timeScale = 1;
            });

        if (m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        BuyRequestUrl = "http://empyreum0711.dothome.co.kr/ShootingDB/Buy_Request.php";
        SpecUpUrl = "http://empyreum0711.dothome.co.kr/ShootingDB/SpecUp.php";

        RenewCrItemList();
    }

    // Update is called once per frame
    void Update()
    {
        if (0.0f < m_Delay)
            m_Delay = m_Delay - Time.deltaTime;

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
        }
    }
    public void TryBuyCrItem(CharType a_ChType)
    {//리스트뷰에 있는 캐릭터 가격버튼을 눌러 구입시도를 한 경우

        if (0.0f < m_Delay)
        {
            g_Message = "잠시 후 다시 시도해 주세요.";
            return;
        }


        if (GlobalValue.m_CrDataList.Count <= (int)a_ChType)
            return;

        m_BuyCrType = a_ChType;

        CharInfo a_CrInfo = GlobalValue.m_CrDataList[(int)m_BuyCrType];

        if (5 <= a_CrInfo.m_Count)
        {
            g_Message = "최대 갯수를 소지하였습니다.";
            return;
        }
        else if (GlobalValue.g_MyPoint < a_CrInfo.m_Price + (a_CrInfo.m_Price * a_CrInfo.m_Count))
        {
            g_Message = "보유(누적) 포인트가 모자랍니다.";
            Debug.Log("포인트가 모자랍니다.");
            return;
        }

        m_Delay = 2.0f; //2초 뒤에 다시 구입 가능하게 한다.

        m_SvStrJson = "";
        bool a_BuyOK = false;
        int a_CacLevel = 0; //서버에 전달하려는 JSON형식을 계산하기 위하여...

        JSONObject a_MkJSON = new JSONObject();
        JSONArray jArray = new JSONArray();//배열이 필요할때

        for (int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        {
            a_CrInfo = GlobalValue.m_CrDataList[ii];
            a_CacLevel = a_CrInfo.m_Count;
            if (ii == (int)m_BuyCrType && a_CrInfo.m_Count < 5) //구매 조건 체크
            {
                int a_Cost = a_CrInfo.m_Price + (a_CrInfo.m_Price * a_CrInfo.m_Count);//

                if (a_Cost <= GlobalValue.g_MyPoint)
                {
                    //GlobalValue.g_MyPoint -= a_Cost; //서버로부터 응답을 받은 다음에 차감해 준다.
                    m_SvMyPoint = GlobalValue.g_MyPoint;
                    m_SvMyPoint -= a_Cost;  //서버로부터 응답을 받은 다음에 차감해 준다.
                    a_CacLevel++;
                    a_BuyOK = true;
                    if (a_Cost == 500)
                    {
                        GlobalValue.g_PlayerHp += 10;
                        StartCoroutine(UpdateSpecCo());
                    }
                    else if (a_Cost == 1000)
                    {
                        GlobalValue.g_Damage += 5;
                        StartCoroutine(UpdateSpecCo());
                    }
                    else if (a_Cost == 5000)
                    {
                        GlobalValue.g_Life += 1;
                        StartCoroutine(UpdateSpecCo());
                    }
                }
            }//if (ii == (int)m_BuyCrType && a_CrInfo.m_Count < 5) //구매

            //JSON 만들기...
            jArray.Add(a_CacLevel);

        }//for(int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        a_MkJSON.Add("CrList", jArray);//배열을 넣음
        m_SvStrJson = a_MkJSON.ToString();

        if (a_BuyOK == true)
            StartCoroutine(BuyRequestCo());
    }

    IEnumerator BuyRequestCo()
    {
        if (m_SvStrJson == "")
        {
            yield break;            //구매 실패 상태라면 그냥 리턴
        }

        if (GlobalValue.g_Unique_ID == "")
        {
            yield break;            //구매 실패 상태라면 그냥 리턴
        }

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_point", m_SvMyPoint);
        form.AddField("item_list", m_SvStrJson, System.Text.Encoding.UTF8);
        // 웹서버에서 받을 때 한글이 않깨지게 하려면 , System.Text.Encoding.UTF8 로 보내야 한다.
        // 뭘 사고 싶은 건지? 서버에서 모두 검수 및 차감, 추가 시킨 수 결과만 내려서 갱신해 준다.

        WWW webRequest = new WWW(BuyRequestUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8; //<--이렇게 해야 안드로이드에서 한글이 않깨진다.
        string a_ReStr = enc.GetString(webRequest.bytes);

        if (a_ReStr.Contains("UpDateSuccess~") == true)
        {
            //응답완료가 되면 전체 갱신(전체 값을 받아서 갱신하는 방법이 있고,
            //m_SvMyPoint, m_BuyCrType 를 가지고 갱신하는 방법이 있다.)
            GlobalValue.g_MyPoint = m_SvMyPoint; //갱신필요
            GlobalValue.m_CrDataList[(int)m_BuyCrType].m_Count++; //갱신필요

            //매뉴상태를 갱신해 주어야 한다.

            RenewCrItemList();
            m_MyPoint.text = GlobalValue.g_MyPoint.ToString();
        }

        //g_Message = webRequest.text; //<--어차피 영문만 나온다.

        //yield return null;
    }
    void RenewCrItemList()
    {
        if (PanelRoot != null)
        {
            if (m_CrNodeList == null || m_CrNodeList.Length <= 0)
                m_CrNodeList = PanelRoot.GetComponentsInChildren<ItemNodeCtrl>();
        }

        for (int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)
        {
            if (m_CrNodeList.Length <= ii)
                continue;

            //활성화
            m_CrNodeList[ii].SetState(GlobalValue.m_CrDataList[ii].m_Price + (GlobalValue.m_CrDataList[ii].m_Price * GlobalValue.m_CrDataList[ii].m_Count),
                                      GlobalValue.m_CrDataList[ii].m_Count);

        }//for(int ii = 0; ii < GlobalValue.m_CrDataList.Count; ii++)

    }
    IEnumerator UpdateSpecCo()
    {
        if (GlobalValue.g_Unique_ID == "")
        {
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("Input_user", GlobalValue.g_Unique_ID, System.Text.Encoding.UTF8);
        form.AddField("Input_hp", GlobalValue.g_PlayerHp);
        form.AddField("Input_life", GlobalValue.g_Life);
        form.AddField("Input_damage", GlobalValue.g_Damage);

        WWW webRequest = new WWW(SpecUpUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string a_ReStr = enc.GetString(webRequest.bytes);

        if (a_ReStr.Contains("UpDateSuccess~") == true)
        {
            Debug.Log("UpdateScoreSuccess");
        }


    }

    private void OnGUI()
    {
        GUI.skin = mySkin;  //내가 만든 GUI스킨을 적용한다

        if (g_Message != "")
        {
            GUILayout.Label("<color=White><size=25>" + g_Message + "</size></color>");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SimpleJSON;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;


public class TitleMgr : MonoBehaviour
{
    public string g_Message = "";

    [Header("LoginPanel")]
    public GameObject m_LoginPanel = null;          //로그인 Panel
    public InputField m_IDInputField;               //로그인을 위한 아이디 입력창
    public InputField m_PassInputField;             //로그인을 위한 비밀번호 입력창
    public Button m_LoginBtn = null;                //로그인 버튼
    public Button m_CreateAccOpenBtn = null;        //계정생성창을 여는 버튼
    public Button m_DelAccountBtn = null;           //계정삭제 버튼

    [Header("CreateAccountPanel")]
    public GameObject m_CreateAccPanel = null;      //계정생성 Panel
    public InputField m_NewIdInputField;            //새 계정의 아이디를 입력하는 창
    public InputField m_NewPassInputField;          //새 계정의 비밀번호를 입력하는 창
    public InputField m_NewNickInputField;          //새 계정의 닉네임을 입력하는 창
    public Button m_CreateAccountBtn = null;        //새 계정을 생성하는 버튼
    public Button m_CancelButton = null;            //계정 생성 Panel을 종료하는 버튼

    [Header("DeleteAccountPanel")]
    public GameObject m_DeleteAccPanel = null;      //계정삭제 Panel
    public InputField m_DelIdInputField;            //삭제할계정의 아이디를 입력하는 창
    public InputField m_DelPassInputField;          //삭제할 계정의의 비밀번호를 입력하는 창
    public InputField m_DelReasonInputField;        //계정의 삭제의 사유를 입력하는 창
    public Button m_DeleteAccReqBtn = null;         //계정을 삭제하는 버튼
    public Button m_DelCancelButton = null;         //계정 삭제 Panel을 종료하는 버튼
    //https://www.google.com/settings/security/lesssecureapps 여기서 보안수준 낮게 설정해야댐


    string LoginUrl;                                //php서버에 로그인정보를 전달하는 Url
    string CreateUrl;                               //php서버에 새 계정의 정보를 저장하는 Url
    string DelteUrl;                                //php서버에 계정삭제를 할 아이디의 정보를 확인하는 Url

    [Header("EscPanel")]
    bool m_EscOnOff = false;                        //EscUI의 On/Off를 체크하는 변수
    [SerializeField] GameObject m_EscUI = null;     //EscUI
    [SerializeField] Button m_ContinueBtn = null;   //EscUI를 종료하고 게임을 계속 진행하는 버튼
    [SerializeField] Button m_ExitBtn = null;       //게임을 종료하는 버튼

    [Header("SoundPanel")]
    public Button m_SoundBtn = null;                //SoundUI를 OnOff하는 버튼
    public GameObject m_SoundUI = null;             //SoundUI

    public GUISkin mySkin = null; // Asset - Create - GUI Skin 생성 후 만들어진 파일에 폰트를 넣어준다.

    // Start is called before the first frame update
    void Start()
    {
        GlobalValue.InitData();

        SoundManager.Instance.PlayBGM("Elec_Background");

        //로그인 버튼
        if (m_LoginBtn != null)
            m_LoginBtn.onClick.AddListener(LoginBtn);

        //계정 생성 버튼
        if (m_CreateAccountBtn != null)
            m_CreateAccountBtn.onClick.AddListener(CreateAccountBtn);

        //계정 생성 창 열기 버튼
        if (m_CreateAccOpenBtn != null)
            m_CreateAccOpenBtn.onClick.AddListener(OpenCreateAccBtn);

        //계정 생성 취소 버튼
        if (m_CancelButton != null)
            m_CancelButton.onClick.AddListener(CreateCancelBtn);

        //EscUI창 종료 버튼
        if (m_ContinueBtn != null)
            m_ContinueBtn.onClick.AddListener(() =>
            {
                m_EscUI.SetActive(false);
                m_EscOnOff = false;
                Time.timeScale = 1;
            });

        //게임 종료 버튼
        if (m_ExitBtn != null)
            m_ExitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        //사운드 UI On/Off버튼
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

        //계정 삭제 요청 Panel 여는 버튼
        if (m_DelAccountBtn != null)
            m_DelAccountBtn.onClick.AddListener(DeleteRequestBtn);

        //계정 삭제 요청 Panel 닫는 버튼
        if (m_DelCancelButton != null)
            m_DelCancelButton.onClick.AddListener(DeleteRequestCancelBtn);

        //계정 삭제를 요청 하는 버튼
        if (m_DeleteAccReqBtn != null)
            m_DeleteAccReqBtn.onClick.AddListener(DeleteBtn);


        LoginUrl = "http://empyreum0711.dothome.co.kr/ShootingDB/Login.php";
        CreateUrl = "http://empyreum0711.dothome.co.kr/ShootingDB/CreateAccount.php";
        DelteUrl = "http://empyreum0711.dothome.co.kr/ShootingDB/Delte.php";

    }

    // Update is called once per frame
    void Update()
    {
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

    //로그인 버튼을 눌렀을 때 함수
    void LoginBtn()
    {
        StartCoroutine(LoginCo());
    }

    //로그인하는 코루틴 함수
    IEnumerator LoginCo()
    {
        GlobalValue.g_Unique_ID = "";

        WWWForm form = new WWWForm();
        form.AddField("Input_user", m_IDInputField.text, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", m_PassInputField.text);

        WWW webRequest = new WWW(LoginUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        g_Message = sz;

        if (sz.Contains("Login-Success!!") == true)
        {
            GlobalValue.g_Unique_ID = m_IDInputField.text;

            //JSON 파싱
            if (sz.Contains("nick_name") == true)
            {
                var N = JSON.Parse(sz);
                if (N != null)
                {
                    if (N["nick_name"] != null)
                        GlobalValue.g_NickName = N["nick_name"];

                    if (N["best_score"] != null)
                        GlobalValue.g_BestScore = N["best_score"].AsInt;

                    if (N["mypoint"] != null)
                        GlobalValue.g_MyPoint = N["mypoint"].AsInt;

                    if (N["myhp"] != null)
                        GlobalValue.g_PlayerHp = N["myhp"].AsInt;

                    if (N["mylife"] != null)
                        GlobalValue.g_Life = N["mylife"].AsInt;

                    if (N["mydamage"] != null)
                        GlobalValue.g_Damage = N["mydamage"].AsInt;

                    if (N["item_list"] != null)
                    {
                        string m_StrJson = N["item_list"];
                        if (m_StrJson != "" && m_StrJson.Contains("CrList") == true)
                        {
                            //---info쪽 JSON 파일 파싱
                            var a_N = JSON.Parse(m_StrJson);
                            for (int ii = 0; ii < a_N["CrList"].Count; ii++)
                            {
                                int a_CrLevel = a_N["CrList"][ii].AsInt;

                                if (ii <= GlobalValue.m_CrDataList.Count)
                                {
                                    GlobalValue.m_CrDataList[ii].m_Count = a_CrLevel;
                                }//if (ii < GlobalValue.m_CrDataList.Count)
                            }//for (int ii = 0; ii < a_N["CrList"].Count; ii++)
                            //---info쪽 JSON 파일 파싱
                        }//if (m_StrJson != "")
                    }//if (N["item_list"] != null)
                    //Debug.Log(GlobalValue.g_NickName + " : " + GlobalValue.g_BestScore + " : ");
                }
            }//if (sz.Contains("nick_name") == true)
            SceneManager.LoadScene("LobbyScene");
        }
    }

    //계정생성 버튼을 눌렀을때 함수
    void CreateAccountBtn()
    {
        StartCoroutine(CreateCO());
    }

    //계정생성하는 코루틴 함수
    IEnumerator CreateCO()
    {
        if (m_NewIdInputField.text == "" || m_NewPassInputField.text == "" || m_NewNickInputField.text == "")
        {
            //빈아이디 생성 방지 예외처리
            g_Message = "ID, PW, 별명 빈칸 없이 입력해 주셔야 합니다.";
            yield break;
        }

        string a_IdStr = m_NewIdInputField.text;
        string a_PwStr = m_NewPassInputField.text;
        string a_NickStr = m_NewNickInputField.text;

        if (a_IdStr.Trim() == "" || a_PwStr.Trim() == "" || a_NickStr.Trim() == "")
        {
            g_Message = "ID, PW, 별명 빈칸없이 입력해 주셔야 합니다.";
            yield break;
        }

        if (!(3 <= a_IdStr.Length && a_IdStr.Length < 10))
        {
            g_Message = "ID는 3글자 이상 10글자 이하로 작성해 주세요.";
            yield break;
        }

        if (!(4 <= a_PwStr.Length && a_PwStr.Length < 15))
        {
            g_Message = "비밀번호는 4글자 이상 15글자 이하로 작성해 주세요.";
            yield break;
        }

        if (!(2 <= a_NickStr.Length && a_NickStr.Length < 15))
        {
            g_Message = "별명은 2글자 이상 15글자 이하로 작성해 주세요.";
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("Input_user", m_NewIdInputField.text, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", m_NewPassInputField.text);
        form.AddField("Input_nick", m_NewNickInputField.text, System.Text.Encoding.UTF8);
        //웹서버에서 받을때 한글이 깨지지 않게 하기 위해 System.Text.Encoding.UTF8을 사용



        WWW webRequest = new WWW(CreateUrl, form);
        yield return webRequest;

        g_Message = webRequest.text;

        if (m_LoginPanel != null)
            m_LoginPanel.SetActive(true);

        if (m_CreateAccPanel != null)
            m_CreateAccPanel.SetActive(false);
    }

    //계정생성 판넬을 여는 버튼
    void OpenCreateAccBtn()
    {
        if (m_LoginPanel != null)
            m_LoginPanel.SetActive(false);

        if (m_CreateAccPanel != null)
            m_CreateAccPanel.SetActive(true);
    }

    //계정생성 판넬을 닫는 함수
    void CreateCancelBtn()
    {
        if (m_LoginPanel != null)
            m_LoginPanel.SetActive(true);

        if (m_CreateAccPanel != null)
            m_CreateAccPanel.SetActive(false);
    }

    //계정 삭제를 요청하는 판넬을 여는 버튼
    void DeleteRequestBtn()
    {
        if (m_LoginPanel != null)
            m_LoginPanel.SetActive(false);

        if (m_DeleteAccPanel != null)
            m_DeleteAccPanel.SetActive(true);
    }

    //계정 삭제를 요청하는 판넬을 닫는 버튼
    void DeleteRequestCancelBtn()
    {
        if (m_LoginPanel != null)
            m_LoginPanel.SetActive(true);

        if (m_DeleteAccPanel != null)
            m_DeleteAccPanel.SetActive(false);
    }

    //계정삭제를 요청하는 버튼
    void DeleteBtn()
    {
        StartCoroutine(DelteCo());
    }

    //#if UNITY_EDITOR
    void OnGUI()
    {
        GUI.skin = mySkin;  //내가 만든 GUI스킨을 적용한다

        if (g_Message != "")
        {
            GUILayout.Label("<color=White><size=25>" + g_Message + "</size></color>");
        }
    }
    //#endif

  
    //계정삭제를 하는 코루틴 함수
    IEnumerator DelteCo()
    {
        GlobalValue.g_Unique_ID = "";

        if (m_DelIdInputField.text == "" || m_DelPassInputField.text == "" || m_DelReasonInputField.text == "")
        {
            g_Message = "아이디와 비밀번호와 사유를 빈칸 없이 작성해 주세요.";
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("Input_user", m_DelIdInputField.text, System.Text.Encoding.UTF8);
        form.AddField("Input_pass", m_DelPassInputField.text);

        WWW webRequest = new WWW(DelteUrl, form);
        yield return webRequest;

        System.Text.Encoding enc = System.Text.Encoding.UTF8;
        string sz = enc.GetString(webRequest.bytes);
        g_Message = sz;

        if (sz.Contains("Correct_Info") == true)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("empyreum0711@gmail.com"); // 보내는사람
            mail.To.Add("ksunj0711@naver.com"); // 받는 사람
            mail.Subject = "Shooting Star 계정 삭제 요청"; //메일 제목
            mail.Body = "아이디 : " + m_DelIdInputField.text + "\n" + "비밀번호 : " + m_DelPassInputField.text + "\n" +
                "사유 :" + m_DelReasonInputField.text;    //이메일의 내용

            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new System.Net.NetworkCredential("empyreum0711@gmail.com", "rlatjswnd683537@") as ICredentialsByHost; // 보내는사람 주소 및 비밀번호 확인
            smtpServer.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
            smtpServer.Send(mail);

            g_Message = "계정삭제를 요청하였습니다.\n" + "2 ~ 3일 이내로 계정 삭제가 완료 됩니다.";
            Debug.Log("success");
        }
    }
}


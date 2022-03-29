using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class ItemNodeCtrl : MonoBehaviour
{
    public Text m_LvText;           //아이템의 소지 갯수
    public Image m_ItemIconImg;     //아이템의 이미지
    public Text m_EffectText;       //아이템의 효과 설명
    public Text m_BuyText;          //아이템의 가격
    StoreManager m_StoreMgr;

    CharType m_BuyCrType;


    // Start is called before the first frame update
    void Start()
    {
        GameObject a_StoreObj = GameObject.Find("StoreMgr");
        

        if (a_StoreObj != null)
        {
            m_StoreMgr = a_StoreObj.GetComponent<StoreManager>();
        }

        Button m_BtnCom = this.GetComponentInChildren<Button>(); //리스트뷰에 있는 캐릭터 가격버튼을 눌러 구입시도를 한 경우

        if (m_BtnCom != null)
        {
            m_BtnCom.onClick.AddListener(() =>
            {
                string a_Str = this.gameObject.name;
                CharType a_CharType = CharType.Char_0;
                CharInfo a_CrInfo = GlobalValue.m_CrDataList[(int)m_BuyCrType];


                if (a_Str.Contains("_1") == true)//체력 아이템
                {
                    a_CharType = CharType.Char_0;
                }
                else if (a_Str.Contains("_2") == true)//공격력 아이템
                {
                    a_CharType = CharType.Char_1;
                }
                else if (a_Str.Contains("_3") == true)//목숨 아이템
                {
                    a_CharType = CharType.Char_2;
                }

                if (m_StoreMgr != null)
                {
                    m_StoreMgr.TryBuyCrItem(a_CharType);
                }
            });
        }//if (m_FireBtnCom != null)
    }

    //아이템 상태
    public void SetState(int a_Price, int a_Lv = 0)
    {
        m_LvText.color = new Color32(255, 255, 255, 255);
        m_LvText.text = a_Lv.ToString() + "/5";
        m_BuyText.text = a_Price.ToString() + " POINT";   //여기서는 업데이트 가격
    }
}

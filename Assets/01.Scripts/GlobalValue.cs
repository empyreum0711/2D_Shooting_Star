using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharType
{
    Char_0 = 0,     //체력 아이템 고유번호
    Char_1,         //공격력 아이템 고유번호
    Char_2,         //목숨 아이템 고유번호
    CrCount         //아이템 갯수
}

public class CharInfo
{
    public CharType m_CrType = CharType.Char_0;
    public int m_Price = 500; //아이템 기본 가격 
    public int m_Count = 0;   //아이템 소지 수량
}
public class GlobalValue
{
    public static string g_Unique_ID = "";      //계정 아이디
        
    public static string g_NickName = "";       //유저 닉네임
    public static int g_BestScore = 0;          //유저의 최고 점수
    public static int g_MyPoint = 0;            //유저의 포인트
    public static int g_Life = 3;               //유저 목숨
    public static int g_Damage = 10;            //유저 공격력
    public static int g_PlayerHp = 100;         //유저 체력

    public static List<CharInfo> m_CrDataList = new List<CharInfo>();


    //유저의 정보
    public static void InitData()
    {
        if (0 < m_CrDataList.Count)
            return;

        CharInfo a_CrItemNd;

        for (int ii = 0; ii < (int)CharType.CrCount; ii++)
        {
            a_CrItemNd = new CharInfo();

            a_CrItemNd.m_CrType = (CharType)ii;

            if (ii == 0)
                a_CrItemNd.m_Price = 500; //기본가 + (구매수량 * 기본가)                   
            else if (ii == 1)
                a_CrItemNd.m_Price = 1000; //기본가 + (구매수량 * 기본가) 
            else if (ii == 2)
                a_CrItemNd.m_Price = 5000; //기본가 + (구매수량 * 기본가) 


            m_CrDataList.Add(a_CrItemNd);
        }
    }//public static void InitData()
}

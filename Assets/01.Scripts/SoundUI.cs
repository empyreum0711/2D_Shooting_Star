using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundUI : MonoBehaviour
{
    public Button m_SoundObjClosebtn = null;

    public Toggle m_SoundToggle = null;
    public Slider m_SoundVolume = null;
    // Start is called before the first frame update
    void Start()
    {
        if (m_SoundObjClosebtn != null)
        {
            m_SoundObjClosebtn.onClick.AddListener(() =>
            {
                Time.timeScale = 1.0f;
                Destroy(this.gameObject);
            });
        }

        //사운드 체크박스 온오프시 호출
        if (m_SoundToggle != null)
            m_SoundToggle.onValueChanged.AddListener(SoundOnOff);

        //사운드 볼륨조절시 호출
        if (m_SoundVolume != null)
            m_SoundVolume.onValueChanged.AddListener(ValueTrCgCheck);

        //사운드 On/Off 초기값 설정(On)
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);

        if(m_SoundToggle != null)
        {
            if (a_SoundOnOff == 0)
                m_SoundToggle.isOn = false;
            else
                m_SoundToggle.isOn = true;
        }
        //사운드 On/Off 초기값 설정(On)

        //사운드 볼륨 초기값 설정
        float a_SoundV = PlayerPrefs.GetFloat("SoundVolume", 1.0f);

        if (m_SoundVolume != null)
            m_SoundVolume.value = a_SoundV;
        //사운드 볼륨 초기값 설정


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SoundOnOff(bool value)//사운드 on/off 체크상태가 변경되었을 때 호출
    {
        if(value == true)//체크박스 On
        {
            SoundManager.Instance.SoundOnOff(true);
            PlayerPrefs.SetInt("SoundOnOff", 1);
        }
        else //체크박스 off
        {
            SoundManager.Instance.SoundOnOff(false);
            PlayerPrefs.SetInt("SoundOnOff", 0);
        }
    }

    public void ValueTrCgCheck(float value)//슬라이드 상태 변경시 호출되는 함수
    {
        SoundManager.Instance.SoundVolume(value);
        PlayerPrefs.SetFloat("SoundVolume", value);
    }
}

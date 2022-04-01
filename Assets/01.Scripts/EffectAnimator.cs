using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimator : MonoBehaviour
{
    int maxFrameNum;        //프레임의 최대수

    [SerializeField]
    Sprite[] aniSprites;    //애니메이션에 사용될 스프라이트의 배엻

    SpriteRenderer renderer;    //바꿔줄 스프라이트의 랜더러

    [SerializeField]
    float aniSpeed;         //이펙트의 재생속도

    float curTime;          //진행되는 시간

    [SerializeField]
    int frameIndex = 0;     //프레임의 갯수

    [SerializeField]
    bool isLoop;            //반복중인지


    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        maxFrameNum = aniSprites.Length;

        frameIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        curTime += aniSpeed * Time.deltaTime;

        if(curTime > 1.0f)//이미지를 바꿔줄 타이밍이 됐을때
        {
            curTime = 0;

            if (isLoop == true)//반복되는 애니메이션일때
            {
                renderer.sprite = aniSprites[frameIndex];

                frameIndex++;

                if(frameIndex >= maxFrameNum)
                {
                    frameIndex = 0;
                }
            }//if (isLoop == true)//반복되는 애니메이션일때
            else
            {
                if(frameIndex < maxFrameNum)
                {
                    renderer.sprite = aniSprites[frameIndex];
                }

                frameIndex++;

                if(frameIndex >= maxFrameNum + 1)
                {
                    Destroy(gameObject);
                }
            }//if (isLoop == false)//반복되는 애니메이션이 아닐때
        }
    }
}

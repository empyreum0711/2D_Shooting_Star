using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimator : MonoBehaviour
{
    int maxFrameNum;

    [SerializeField]
    Sprite[] aniSprites;

    SpriteRenderer renderer;

    [SerializeField]
    float aniSpeed;

    float curTime;

    [SerializeField]
    int frameIndex = 0;

    [SerializeField]
    bool isLoop;


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

            }
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
            }

        }

    }
}

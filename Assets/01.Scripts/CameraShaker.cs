using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    [SerializeField] Vector3 originPos;


    public void CameraShake(float time = 1, float power = 1)
    {
        StartCoroutine(CoShake(time, power));
    }

    IEnumerator CoShake(float time = 1, float power = 1)
    {
        originPos = transform.localPosition;

        float curTime = 0;

        while (curTime < time)
        {
            curTime += Time.deltaTime;

            float shakeX = Random.Range(-0.2f, 0.2f) * power;
            float shakeY = Random.Range(-0.2f, 0.2f) * power;
            transform.localPosition = new Vector3(shakeX, shakeY, transform.position.z);

            yield return new WaitForSeconds(0.01f);
        }

        transform.localPosition = originPos;

    }

}

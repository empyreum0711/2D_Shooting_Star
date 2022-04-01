using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    //생성된 이펙트를 제거한다
    public void DestroyEffect()
    {
        Destroy(gameObject);
    }

}

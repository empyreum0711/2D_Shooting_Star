using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //인스펙터창에서 관리를 가능하게 해준다.
public struct PoolingInfo
{
    public GameObject prefab;
    public int count;
    
}

public class PoolingManager : MonoBehaviour
{
    Dictionary<string, List<GameObject>> poolingObjects = new Dictionary<string, List<GameObject>>();
    
    static PoolingManager m_instance;

    static public PoolingManager Instance
    {
        get
        {
            if (m_instance == null)
                m_instance = FindObjectOfType<PoolingManager>();

            return m_instance;
        }
    }

    [SerializeField] PoolingInfo[] poolingInfos;


    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Initialize()
    {
        for(int i=0; i< poolingInfos.Length; i++)
        {
            List<GameObject> list = new List<GameObject>();

            for(int j=0; j<poolingInfos[i].count; j++)
            {
                GameObject go = Instantiate(poolingInfos[i].prefab);
                go.SetActive(false);

                list.Add(go);
            }

            string keyName = poolingInfos[i].prefab.name;
            poolingObjects.Add(keyName, list);
        }

    }

    public GameObject GetObject(string key)
    {
        List<GameObject> list = new List<GameObject>();

        if(poolingObjects.ContainsKey(key) == true) //해당 키값이 있을때
        {
            list = poolingObjects[key];

            foreach(GameObject obj  in list)
            {
                if(obj.activeSelf == false)
                {
                    obj.SetActive(true);
                    return obj;
                }
            }

            for(int i=0; i< 20; i++)
            {
                GameObject go = Instantiate(list[0]);
                go.SetActive(false);
                list.Add(go);
            }

            list[list.Count - 1].SetActive(true);
            return list[list.Count - 1];

        }

        return null;


    }

}

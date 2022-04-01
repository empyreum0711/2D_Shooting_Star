using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    float regenTime = 5f;   //리젠 시간

    [SerializeField]
    GameObject[] enemyPrefabs;  //적 비행체

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //적 비행체 스폰
    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            int index = Random.Range(0, enemyPrefabs.Length);

            GameObject go = Instantiate(enemyPrefabs[index]);
            go.name = enemyPrefabs[index].name;
            go.GetComponent<Enemy>().SetName(enemyPrefabs[index].name);

            yield return new WaitForSeconds(regenTime);
        }
    }
}

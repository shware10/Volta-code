using UnityEngine;
using System.Collections.Generic;


public class StickySpawner : MonoBehaviour
{
    public static StickySpawner Instance;
    // sticky의 prefab을 저장해 놓을 풀;
    public Queue<GameObject> spawnedStickyPool = new Queue<GameObject>();

    [SerializeField] private GameObject spawnedStickyPrefab;
    [SerializeField] private List<Material> stickyList = new List<Material>();

    [SerializeField] private RandomPosGenerator randomSpawnPos;

    void Awake()
    {
        Instance = this;
        for (int i = 0; i < 100; ++i)
        {
            GameObject sticky = Instantiate(spawnedStickyPrefab, transform);
            spawnedStickyPool.Enqueue(sticky);
            sticky.SetActive(false);
        }
    }

    //게임매니저의 카운트를 받아 스티키를 생성하고 랜덤한 위치에 생성하는 함수
    public void SpawnSticky()
    {
        int randIdx = Random.Range(0, 10);
        Material selectedMat = null;
        float bonus = 0f;
        //20%
        if (randIdx <= 1)
        {
            selectedMat = stickyList[4];
            bonus = 0.25f;
        }
        else
        {
            selectedMat = stickyList[Random.Range(0, 4)];
            bonus = 0.1f;
        }
        if (spawnedStickyPool.Count != 0)
        {
            // 사용할 빈 SpawnSticky prefab 생성
            GameObject prefab = spawnedStickyPool.Dequeue();
            SpawnedSticky sticky = prefab.GetComponent<SpawnedSticky>();
            //해당 스티키에 뽑힌 데이터 적용
            sticky.Init(selectedMat, bonus);

            // 프리펩을 넣으면 해당 프리펩의 랜덤한 위치를 생성하는 함수
            Vector3 randPos = randomSpawnPos.GetRandomPos(prefab);
            // 그 위치에 스폰
            sticky.Stick(randPos);
        }
    }
}
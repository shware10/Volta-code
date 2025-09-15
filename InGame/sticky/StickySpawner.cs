using UnityEngine;
using System.Collections.Generic;


public class StickySpawner : MonoBehaviour
{
    public static StickySpawner Instance;
    // sticky�� prefab�� ������ ���� Ǯ;
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

    //���ӸŴ����� ī��Ʈ�� �޾� ��ƼŰ�� �����ϰ� ������ ��ġ�� �����ϴ� �Լ�
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
            // ����� �� SpawnSticky prefab ����
            GameObject prefab = spawnedStickyPool.Dequeue();
            SpawnedSticky sticky = prefab.GetComponent<SpawnedSticky>();
            //�ش� ��ƼŰ�� ���� ������ ����
            sticky.Init(selectedMat, bonus);

            // �������� ������ �ش� �������� ������ ��ġ�� �����ϴ� �Լ�
            Vector3 randPos = randomSpawnPos.GetRandomPos(prefab);
            // �� ��ġ�� ����
            sticky.Stick(randPos);
        }
    }
}
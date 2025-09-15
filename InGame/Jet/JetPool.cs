using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPool : MonoBehaviour
{
    public List<Jet> Pool = new List<Jet>();
    public Vector3[] jetDir = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
    private int[] jetAngle = { 0, 180, 90, 270 };
    public Color[] jetColors = { Color.red, Color.green, Color.blue, Color.white };


    [SerializeField] private Transform rectanglePos;
    [SerializeField] private GameObject[] jetPrefabs = new GameObject[4];

    void Awake()
    {
        //모든 제트 조합을 pool에 미리 생성
        for (int i = 0; i < jetColors.Length;  ++i)
        {
            for (int j = 0; j < jetDir.Length; ++j)
            {
                GameObject obj = Instantiate(jetPrefabs[i], transform);
                Jet jet = obj.GetComponent<Jet>();
                jet.color = jetColors[i];

                jet.dir = jetDir[j];
                jet.angle = jetAngle[j];

                jet.poolPos = transform.position;
                jet.spawnPos = rectanglePos.position;

                Pool.Add(jet);
            }
        }
    }
}

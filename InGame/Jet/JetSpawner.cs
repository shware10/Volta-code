using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class JetSpawner : MonoBehaviour, IGameStateListener
{
    [SerializeField] private JetPool JetPool;

    private bool isInGame = false;
    private int mxIndex;

    void Awake()
    {
        mxIndex = JetPool.jetDir.Length * JetPool.jetColors.Length;
    }

    public void SpawnJet()
    {
        int randIndex = Random.Range(0, mxIndex);
        while (JetPool.Pool[randIndex].curState == JetState.isSpawn)
        {
            randIndex = Random.Range(0, mxIndex);
        }
        JetPool.Pool[randIndex].Go2SpawnPos();
    }

    public void OnStateChanged(GameState state)
    {
        isInGame = state == GameState.InGame;
        if (isInGame)
        {
            StartCoroutine(SpawnRoutine());
        }
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(2f);
        SpawnJet();
    }
}

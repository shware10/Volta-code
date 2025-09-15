using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum JetState
{ 
    isSpawn,
    isPool
}
public class Jet : MonoBehaviour,IGameStateListener
{
    public Color color;
    public Vector3 dir;
    public float angle;

    [SerializeField] private float spinDuration = 1f;
    [SerializeField] private float minSpinDuration = 1.2f;
    [SerializeField] private float maxSpinDuration = 1f;
    [SerializeField] private float readyTime = 0f;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float minSpeed = 2.5f;
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float transDuration = 120f;

    public Vector3 spawnPos;
    public Vector3 poolPos;

    public JetState curState = JetState.isPool;

    private Collider col;
    private SpriteRenderer rdr;
    ParticleSystem fitParticle;
    ParticleSystem unfitParticle;
    ParticleSystem netParticle;

    Coroutine moveRoutine;
    Coroutine transRoutine;

    void Awake()
    {
        rdr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider>();
        fitParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        unfitParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        netParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void Fit()
    {
        StartCoroutine(FitRoutine());
    }
    public void Unfit()
    {
        StartCoroutine(UnfitRoutine());
    }
    public void Net()
    {
        StartCoroutine(NetRoutine());
    }

    IEnumerator FitRoutine()
    {
        StopCoroutine(moveRoutine);
        FitEffect();
        rdr.enabled = false;
        col.enabled = false;
        yield return new WaitForSeconds(fitParticle.main.duration);
        Go2PoolPos();
    }
    IEnumerator UnfitRoutine()
    {
        StopCoroutine(moveRoutine);
        UnfitEffect();
        rdr.enabled = false;
        col.enabled = false;
        yield return new WaitForSeconds(unfitParticle.main.duration);
        Go2PoolPos();
    }
    IEnumerator NetRoutine()
    {
        StopCoroutine(moveRoutine);
        NetEffect();
        rdr.enabled = false;
        col.enabled = false;
        yield return new WaitForSeconds(unfitParticle.main.duration);
        Go2PoolPos();
    }
    public void FitEffect()
    {
        fitParticle.Play();
    }

    public void UnfitEffect()
    {
        unfitParticle.Play();
    }
    public void NetEffect()
    {
        netParticle.Play();
    }
    public void Go2SpawnPos()
    {
        rdr.enabled = true;
        col.enabled = true;
        transform.position = spawnPos;
        curState = JetState.isSpawn;
        moveRoutine = StartCoroutine(MoveRoutine());
    }
    public void Go2PoolPos()
    {
        curState = JetState.isPool;
        transform.position = poolPos;
        transform.localScale = Vector3.zero;
    }

    IEnumerator MoveRoutine()
    {
        yield return StartCoroutine(SpinMotion());
        yield return new WaitForSeconds(readyTime);
        float time = 0;
        while (curState == JetState.isSpawn)
        {
            transform.position += dir * moveSpeed * Time.deltaTime;

            time += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator SpinMotion()
    {
        float time = 0;

        float originZ = transform.localEulerAngles.z;
        float targetAngle = 360f;
        float targetScale = 0.5f;
        if ((this.angle / 90) % 2 == 0) // 각도 0, 180 의 경우
        {
            while (time < spinDuration)
            {
                float scale = Mathf.Lerp(0, targetScale, time / spinDuration); // 스케일 증가
                float angle = Mathf.Lerp(0, targetAngle, time / spinDuration); // 회전
                transform.rotation = Quaternion.Euler(0, angle, originZ);
                transform.localScale = Vector3.one * scale;

                time += Time.deltaTime;
                yield return null;
            }

            transform.rotation = Quaternion.Euler(0, targetAngle, originZ);
            transform.localScale = Vector3.one * targetScale;
        }
        else // 각도 90, 270 의 경우
        {
            while (time < spinDuration)
            {
                float scale = Mathf.Lerp(0, targetScale, time / spinDuration); // 스케일 증가
                float angle = Mathf.Lerp(0, targetAngle, time / spinDuration); // 회전
                transform.rotation = Quaternion.Euler(angle, 0, originZ);
                transform.localScale = Vector3.one * scale;

                time += Time.deltaTime;
                yield return null;
            }

            transform.rotation = Quaternion.Euler(targetAngle, 0, originZ);
            transform.localScale = Vector3.one * targetScale;
        }
        
    }

    public void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.InGame:
                transRoutine = StartCoroutine(SpeedTransition());
                break;
            case GameState.GameOver:
                StopCoroutine(transRoutine);
                InitSpeed();
                break;
        }
    }

    IEnumerator SpeedTransition()
    {
        float time = 0;
        while(time < transDuration)
        {
            float percent = time / transDuration;

            moveSpeed = Mathf.Lerp(minSpeed, maxSpeed, percent);
            spinDuration = Mathf.Lerp(minSpinDuration, maxSpinDuration, percent);
            time += Time.deltaTime;
            yield return null;
        }
        moveSpeed = maxSpeed;
        spinDuration = maxSpinDuration;
    }

    void InitSpeed()
    {
        moveSpeed = minSpeed;
        spinDuration = minSpinDuration;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FeverEffecter : Singleton<FeverEffecter>
{
    [SerializeField] float fireInterval = 1.0f;
    [SerializeField] ParticleSystem[] particles;
    protected override void Awake()
    {
        base.Awake();
        particles = GetComponentsInChildren<ParticleSystem>();
    }

    public void FireWork()
    {
        StartCoroutine(FireWorkMotion());
    }
    IEnumerator FireWorkMotion()
    {
        for(int i = 0; i < particles.Length; ++i)
        {
            particles[i].Play();
            yield return new WaitForSeconds(fireInterval);
        }
    }
}

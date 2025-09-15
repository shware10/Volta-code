using UnityEngine;

enum BGMState
{ 
    InGameBGM
}

public class AudioManager : Singleton<AudioManager>,IGameStateListener
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [SerializeField] private AudioClip[] bgmClips;
    [SerializeField] private AudioClip[] sfxClips;
    [SerializeField] private AudioClip[] uiClips;

    protected override void Awake()
    {
        base.Awake();
    }

    public void PlaySFX(int index)
    {
        sfxSource.clip = sfxClips[index];
        sfxSource.loop = false;
        sfxSource.Play();
    }

    public void PlayBGM(int index)
    {
        bgmSource.clip = bgmClips[index];
        bgmSource.loop = true;
        bgmSource.Play();
    }
    public void StopBGM() => bgmSource.Stop();

    public void OnStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.InGame:
                PlayBGM((int)BGMState.InGameBGM);
                break;
            default:
                StopBGM();
                break;
        }
    }
}

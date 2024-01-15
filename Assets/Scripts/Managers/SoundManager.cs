using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// 클래스 자체를 직렬화 SerializeField처럼 인스펙터창에 띄운다
[System.Serializable]
public class Sound
{
    public string name; // 곡의 이름
    public AudioClip clip; // 곡
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    #region Singleton
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion Singleton

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBgm;
    public AudioSource audioSourceTitleBgm;

    public string[] playSoundName;

    public Sound[] effectSounds;
    public Sound[] bgmSounds;
    public Sound titleBgmSounds;

    private bool isRandomBgmPlaying;

    [SerializeField] private Button bgmBtn;
    private Color btnColor;

    private void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
        isRandomBgmPlaying = false;
    }


    public void PlaySE(string _name)
    {
        for(int i = 0; i < effectSounds.Length; i++)
        {
            if(_name == effectSounds[i].name)
            {
                for(int j = 0; j < audioSourceEffects.Length; j++)
                {
                    if (!audioSourceEffects[j].isPlaying)
                    {
                        playSoundName[j] = effectSounds[i].name;
                        audioSourceEffects[j].clip = effectSounds[i].clip;
                        audioSourceEffects[j].Play();
                        return;
                    }
                }
                Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                return;
            }
        }
        Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
    }

    public void StopAllSE() 
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                return;
            }
        }
        Debug.Log("재생 중인" + _name + "사운드가 없습니다.");
    }

    //#0
    public void PlayRequestedBGM(string _name)
    {
        if (isRandomBgmPlaying)
        {
            audioSourceBgm.Stop();
            isRandomBgmPlaying = false;
        }

        for (int i = 0; i < bgmSounds.Length; i++)
        {
            if (_name == bgmSounds[i].name)
            {
                audioSourceBgm.clip = bgmSounds[i].clip;
                audioSourceBgm.Play();
                return;
            }
        }
        Debug.Log(_name + " 사운드가 SoundManager에 등록되지 않았습니다.");
    }

    //#0
    public void PlayRandomBGM()
    {
        isRandomBgmPlaying = !isRandomBgmPlaying;

        if (isRandomBgmPlaying)
        {
            btnColor = bgmBtn.image.color;
            btnColor.a = 1f;
            bgmBtn.image.color = btnColor;
            StartCoroutine(RandomBGMCheckCoroutine());
        }
        else
        {
            btnColor = bgmBtn.image.color;
            btnColor.a = 0.5f;
            bgmBtn.image.color = btnColor;
            StopCoroutine(RandomBGMCheckCoroutine());
            audioSourceBgm.Stop();
        }
    }

    //#0
    IEnumerator RandomBGMCheckCoroutine()
    {
        yield return new WaitUntil(() => !audioSourceBgm.isPlaying);

        while (isRandomBgmPlaying)
        {
            int randIdx = Random.Range(0, bgmSounds.Length);
            audioSourceBgm.clip = bgmSounds[randIdx].clip;
            Debug.Log("<color=yellow>" + bgmSounds[randIdx].name + "</color>" + " 재생을 시작합니다");
            audioSourceBgm.Play();

            yield return new WaitForSeconds(audioSourceBgm.clip.length);
        }
        Debug.Log("랜덤 BGM 재생을 종료합니다");
    }

    //#1
    public void TitleBgmPlay()
    {
        audioSourceTitleBgm.clip = titleBgmSounds.clip;
        audioSourceTitleBgm.loop = true;
        audioSourceTitleBgm.Play();
    }

    //#1
    public void TitleBgmStop()
    {
        audioSourceTitleBgm.Stop();
    }
}

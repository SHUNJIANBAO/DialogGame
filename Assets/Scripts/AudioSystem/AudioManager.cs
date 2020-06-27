using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region 单例
    private static AudioManager _instace;
    public static AudioManager Instance
    {
        get
        {
            _instace = FindObjectOfType<AudioManager>();
            if (_instace == null)
            {
                _instace = new GameObject("AudioManager").AddComponent<AudioManager>();
            }
            //if (_instace == null)
            //{
            //    _instace = new AudioManager();
            //}
            return _instace;
        }
    }
    #endregion

    #region 初始化
    private static readonly string audioPath = "Audio";
    //public AudioManager()
    //{
    //OnInit();
    //}
    private void Awake()
    {
        OnInit();
    }
    void OnInit()
    {
        if (cameraObj == null) cameraObj = GameObject.FindObjectOfType<AudioListener>().gameObject;
        if (source == null) source = new SourceManager(cameraObj);
        if (clip == null) clip = new ClipManager(audioPath);
        InvokeRepeating("ClearFreeSource", 5, 5);
    }
    #endregion

    #region 控制的组件
    public AudioSource bgmSource;
    GameObject cameraObj;
    SourceManager source;
    ClipManager clip;
    #endregion

    #region 音量控制面板
    private float masterVolume=1;
    public float MasterVolume
    {
        get { return masterVolume; }
        set
        {
            MasterVolumeChange(value);
        }
    }


    private float bgmVolume = 1;
    public float BgmVolume
    {
        get
        {
            return bgmVolume;
        }
        set
        {
            BgmVolumeChange(value);
        }
    }

    private float audioVolume = 1;
    public float AudioVolume
    {
        get
        {
            return audioVolume;
        }

        set
        {
            AudioVolumeChange(value);
        }
    }


    /// <summary>
    /// 总音量大小
    /// </summary>
    /// <param name="value"></param>
    private void MasterVolumeChange(float value)
    {
        masterVolume = Mathf.Clamp(value, 0f, 1f);
        if (source == null || clip == null) return;
        AudioListener.volume = masterVolume;
    }

    /// <summary>
    /// 背景音乐音量大小
    /// </summary>
    /// <param name="value"></param>
    private void BgmVolumeChange(float value)
    {
        bgmVolume = Mathf.Clamp(value, 0f, 1f);
        if (source == null || clip == null) return;
        if (bgmSource == null) bgmSource = source.GetFreeSource();
        if (bgmSource.loop == false) bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
    }

    /// <summary>
    /// 音效大小
    /// </summary>
    /// <param name="value"></param>
    private void AudioVolumeChange(float value)
    {
        audioVolume = Mathf.Clamp(value, 0f, 1f);
        if (source == null || clip == null) return;
        foreach (AudioSource temp in source.sourceList)
        {
            if (temp != bgmSource)
            {
                temp.volume = value;
            }
        }
    }

    #endregion

    #region 功能接口
    /// <summary>
    /// 播放一次音频
    /// </summary>
    /// <param name="audioName"></param>
    public void PlayAudio(string audioName)
    {
        AudioSource temp = source.GetFreeSource();
        temp.volume = audioVolume;
        if (temp == bgmSource) bgmSource = null;
        if (temp.loop == true) temp.loop = false;
        AudioClip tempClip = clip.GetClip(audioName);
        temp.clip = tempClip;
        temp.Play();
    }

    /// <summary>
    /// 循环播放音乐
    /// </summary>
    /// <param name="bgmName"></param>
    public void PlayBGM(string bgmName)
    {
        if (bgmSource == null)
        {
            bgmSource = source.GetFreeSource();
            bgmSource.volume = bgmVolume;
        }
        if (bgmSource.loop == false) bgmSource.loop = true;
        AudioClip tempClip = clip.GetClip(bgmName);
        if (bgmSource.clip == null)
        {
            bgmSource.clip = tempClip;
            bgmSource.Play();
        }
        else
        {
            StartCoroutine(BgmChange(tempClip));
        }
    }
    IEnumerator BgmChange(AudioClip newClip)
    {
        float vol = bgmVolume;
        while (bgmSource.volume > 0)
        {
            BgmVolume -= Time.deltaTime*4;
            yield return null;
        }
        bgmSource.clip = newClip;
        bgmSource.Play();
        while (bgmSource.volume < vol)
        {
            BgmVolume += Time.deltaTime * 4;
            yield return null;
        }
    }

    /// <summary>
    /// 清除所有音频
    /// </summary>
    public void StopAudio()
    {
        source.ClearAllSource();
        bgmSource = null;
    }
    /// <summary>
    /// 清除指定音频
    /// </summary>
    /// <param name="audioName"></param>
    public void StopAudio(string audioName)
    {
        source.ClearSource(audioName);
    }
    #endregion

    #region 其它功能
    void ClearFreeSource()
    {
        source.ClearFreeSource();
    }
    #endregion
}

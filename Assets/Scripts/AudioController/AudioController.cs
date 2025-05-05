using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using System.Linq;


public static class AudioController
{
    // Start is called before the first frame update
    public static AudioSource audio_player;
    public static AudioSource audio_bgm;
    private static string path;
    public static bool effect;
    public static bool bgm;
    private static Dictionary<string, AudioClip> audio_files = new Dictionary<string, AudioClip>();
    public static void InitialLoadAudioClip()
    {
        // #if UNITY_ANDROID && !UNITY_EDITOR
        //             path = Application.streamingAssetsPath;
        // #elif UNITY_EDITOR
        //         path = Application.streamingAssetsPath;
        // #endif
        // StreamReader reader = new StreamReader(path + "/audiolist.txt", Encoding.UTF8);
        var textFile = Resources.Load<TextAsset>("audiolist");
        List<string> words = new List<string>(textFile.text.Trim().Split('\n'));
        foreach (var item in words)
        {
            if (item.Count() > 0)
            {
                audio_files.Add(item.Trim(), Resources.Load<AudioClip>(item.Trim()));
                Debug.Log(item.Trim());
                if (audio_files[item.Trim()] == null)
                {
                    Debug.Log("its null");
                }
            }
        }
        if (PlayerPrefs.HasKey("effect") && PlayerPrefs.HasKey("bgm"))
        {
            if (PlayerPrefs.GetInt("effect") == 0)
            {
                effect = false;
            }
            else
            {
                effect = true;
            }
            if (PlayerPrefs.GetInt("bgm") == 0)
            {
                bgm = false;
            }
            else
            {
                bgm = true;
            }
        }
        else{
            PlayerPrefs.SetInt("effect",1);
            PlayerPrefs.SetInt("bgm",1);
            effect = true;
            bgm = true;
        }

    }
    public static void SetAudioSource(AudioSource audioSource, AudioSource audioSourceBGM)
    {
        audio_player = audioSource;
        audio_bgm = audioSourceBGM;
        SetVolume();
        
    }
    public static void SetnPlay(string file_path)
    {
        if (audio_player.isPlaying)
        {
            audio_player.Stop();
        }
        audio_player.clip = audio_files[file_path];
        audio_player.Play();
    }
    public static void SetnPlayBGM(string file_path, bool play_loop = true)
    {
        audio_bgm.loop = play_loop;
        audio_bgm.clip = audio_files[file_path];
        audio_bgm.Play();

    }
    public static void StopPlayBGM()
    {
        audio_bgm.Stop();
    }
    public static void ToggleEffect()
    {
        effect = !effect;
        PlayerPrefs.SetInt("effect", effect?1:0);
        audio_player.volume = effect?1:0;
        
    }
    public static void ToggleBGM()
    {
        bgm = !bgm;
        PlayerPrefs.SetInt("bgm", bgm?1:0);
        audio_bgm.volume =  bgm?1:0;
    }

    public static void ForceVolume(){
        audio_bgm.volume =  1;
        audio_player.volume = 1;
    }
    public static void SetVolume(){
        audio_player.volume = effect?1:0;
        audio_bgm.volume =  bgm?1:0;
    }

}

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
                if (audio_files[item.Trim()] == null)
                {
                    Debug.Log("its null");
                }
            }
        }
        // string line_path;
        // while (reader.Peek() > 0)
        // {
        //     line_path = reader.ReadLine();
        //     if (line_path.Count() > 0)
        //     {
        //         audio_files.Add(line_path, Resources.Load<AudioClip>(line_path));
        //         if (audio_files[line_path] == null)
        //         {
        //             Debug.Log("its null");
        //         }

        //     }
        //     else
        //     {
        //         break;
        //     }

        // }
        // reader.Close();
    }
    public static void SetAudioSource(AudioSource audioSource, AudioSource audioSourceBGM)
    {
        audio_player = audioSource;
        audio_bgm = audioSourceBGM;
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
    public static void StopPlayGBM()
    {
        audio_bgm.Stop();
    }
}

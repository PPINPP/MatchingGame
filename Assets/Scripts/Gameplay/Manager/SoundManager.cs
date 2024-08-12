using System.Collections;
using System.Collections.Generic;
using MatchingGame.Gameplay;
using UnityEngine;

public class SoundManager : MonoInstance<SoundManager>
{
    
    [SerializeField] int Amount;

    private Queue<SoundEffect> _effects = new Queue<SoundEffect>();
    public override void Init()
    {
        base.Init();
        for (int i = 0; i < Amount; i++)
        {
            var obj = Instantiate(GameplayResources.Instance.soundPrefab);
            _effects.Enqueue(obj.GetComponent<SoundEffect>());
            obj.SetActive(false);
        }
    }

    public void PlaySoundEffect(SoundType type)
    {
        var sound = GameplayResources.Instance.SoundEffectList.Find(f => f.soundType == type);
        AudioClip audioClip = sound.soundEffectClip;
        var soundEffect = _effects.Dequeue();
        soundEffect.SetSound(audioClip);
        soundEffect.gameObject.SetActive(true);
        StartCoroutine(DisableSound(soundEffect, audioClip.length));
    }
    
    private IEnumerator DisableSound(SoundEffect effect, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        effect.gameObject.SetActive(false);
        _effects.Enqueue(effect);
    }
}

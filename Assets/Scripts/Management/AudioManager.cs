using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("Audio Manager");
                _instance = go.AddComponent<AudioManager>();
                _instance._source = go.AddComponent<AudioSource>();
            }

            return _instance;
        }
    }

    private AudioSource _source;

    public void PlaySFX(AudioClip clip, float volume = 1)
    {
        _source.PlayOneShot(clip, volume);
    }
}
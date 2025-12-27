using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFGinc.Objects.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayerPrefab : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        public async void Play(AudioClip clip, float volume = 0.2f, float pitch = 1f)
        {
            try
            {
                float time = clip.length;
                _audioSource.pitch = pitch;
                _audioSource.PlayOneShot(clip, volume);

                await Task.Delay((int)(time * 1000) + 2000);
                Destroy(gameObject);
            }
            catch (Exception)
            {
            }
        }
    }
}
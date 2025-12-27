using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using ZFGinc.Objects.Placement;
using Random = UnityEngine.Random;

namespace ZFGinc.Objects.Audio
{
    public class ObjectSound : MonoBehaviour
    {
        [SerializeField] private bool _isLocalAudio = true;
        [SerializeField, ShowIf(nameof(Local))] private AudioSource _audioSource;
        [SerializeField, ShowIf(nameof(Prefab))] private AudioPlayerPrefab _audioSourcePrefab;
        [SerializeField] private bool _isRandomPitch = false;
        [SerializeField, ShowIf(nameof(RandomPitch)), MinMaxSlider(0.8f, 1.2f)] Vector2 _randomPitch = new(0.8f, 1.2f);
        [SerializeField, ShowIf(nameof(StaticPitch)), Range(.1f, 2f)] private float _pitch = 1f;
        [Space(15)]
        [Header("Audio Clips")]
        [SerializeField] private List<AudioClip> _audioClipsForHoldStart;
        [SerializeField] private List<AudioClip> _audioClipsForHoldEnd;
        [SerializeField] private List<AudioClip> _audioClipsForPush;
        [Space]
        [SerializeField] private List<AudioClip> _audioClipsForGrab;
        [SerializeField] private List<AudioClip> _audioClipsForDrop;
        [SerializeField] private List<AudioClip> _audioClipsForEquip;
        [SerializeField] private List<AudioClip> _audioClipsForUnequip;
        [Space]
        [SerializeField] private List<AudioClip> _audioClipsForInteractError;
        [Space]
        [SerializeField] private List<AudioClip> _audioClipsForUse;
        [SerializeField] private List<AudioClip> _audioClipsForStopUse;
        [Space]
        [SerializeField] private List<AudioClip> _audioClipsForActions;
        [SerializeField] private List<AudioClip> _audioClipsForUseActions;
        [Space]
        [SerializeField] private List<AudioClip> _audioClipsForCollision;
        [SerializeField] private AudioClip _audioClipsForSlide;
        [SerializeField] private float _multiplierVolume = 1f;
        [SerializeField] private float _delay = 1f;

        private float _timer;
        private bool Local => _isLocalAudio;
        private bool Prefab => !_isLocalAudio;
        private bool StaticPitch => !_isRandomPitch;
        private bool RandomPitch => _isRandomPitch;

        private Rigidbody _rigidbody;
        private ObjectToPlace _objectToPlace;

        private AudioSource AudioSource
        {
            get
            {
                if (_isLocalAudio)
                {
                    if (_audioSource == null) TryGetComponent(out _audioSource);
                    if (_audioSource == null)
                    {
                        gameObject.AddComponent<AudioSource>();
                        TryGetComponent(out _audioSource);
                    }
                }

                return _audioSource;
            }
        }

        private Rigidbody Rigidbody
        {
            get
            {
                if (_rigidbody == null)
                {
                    if (TryGetComponent(out InteractObject interactObject))
                    {
                        _rigidbody = interactObject.Rigidbody;
                    }
                }
                return _rigidbody;
            }
        }

        private ObjectToPlace ObjectToPlace
        {
            get
            {
                if (_objectToPlace == null)
                {
                    if (TryGetComponent(out ObjectToPlace objectToPlace))
                    {
                        _objectToPlace = objectToPlace;
                    }
                }
                return _objectToPlace;
            }
        }

        private void Awake()
        {
            if (_isLocalAudio) AudioSource.clip = null;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (_isLocalAudio)
            {
                if (_audioSource == null)
                    TryGetComponent(out _audioSource);
            }
#endif
        }

        protected float GetPitch()
        {
            if (_isRandomPitch) return Random.Range(_randomPitch.x, _randomPitch.y);
            return _pitch;
        }

        protected virtual void PlayAudio(ref List<AudioClip> listAudio, float volume = 0.5f)
        {
            if (listAudio.Count == 0) return;

            AudioClip clip = listAudio[Random.Range(0, listAudio.Count)];

            if (_isLocalAudio)
            {
                if (AudioSource == null) return;

                AudioSource.volume = Mathf.Clamp(volume, 0, 1);
                AudioSource.pitch = GetPitch();
                AudioSource.clip = clip;
                AudioSource.Play();
            }
            else
            {
                AudioPlayerPrefab prefab = Instantiate(_audioSourcePrefab, transform.position, Quaternion.identity);
                prefab.Play(clip, Mathf.Clamp(volume, 0, 1), GetPitch());
            }
        }

        protected virtual void PlayAudio(AudioClip audio, float volume = 0.6f)
        {
            if (_isLocalAudio)
            {
                if (AudioSource == null) return;

                AudioSource.volume = Mathf.Clamp(volume, 0f, 1f);
                AudioSource.clip = audio;
                AudioSource.pitch = GetPitch();
                AudioSource.Play();
            }
            else
            {
                AudioPlayerPrefab prefab = Instantiate(_audioSourcePrefab, transform.position, Quaternion.identity);
                prefab.Play(audio, volume, GetPitch());
            }
        }

        public virtual void HoldAudioStart()
        {
            PlayAudio(ref _audioClipsForHoldStart);
        }

        public virtual void HoldAudioEnd()
        {
            PlayAudio(ref _audioClipsForHoldEnd);
        }

        public virtual void GrabAudio()
        {
            PlayAudio(ref _audioClipsForGrab);
        }

        public virtual void DropAudio()
        {
            PlayAudio(ref _audioClipsForDrop);
        }

        public virtual void PushAudio()
        {
            PlayAudio(ref _audioClipsForPush);
        }

        public virtual void InteractAudioError()
        {
            PlayAudio(ref _audioClipsForInteractError);
        }

        public virtual void UseAudio()
        {
            PlayAudio(ref _audioClipsForUse);
        }

        public virtual void StopUseAudio()
        {
            PlayAudio(ref _audioClipsForStopUse);
        }

        public virtual void UseActionAudio()
        {
            PlayAudio(ref _audioClipsForUseActions);
        }

        public virtual void ActionAudio()
        {
            PlayAudio(ref _audioClipsForActions);
        }

        public virtual void EquipAudio()
        {
            PlayAudio(ref _audioClipsForEquip);
        }

        public virtual void UnequipAudio()
        {
            PlayAudio(ref _audioClipsForUnequip);
        }

        public virtual void CollisionAudio(float volume = 0.5f)
        {
            PlayAudio(ref _audioClipsForCollision, volume);
        }

        public virtual void SlideAudio(float volume = 0.2f)
        {
            if (_audioClipsForSlide == null) return;

            if (Time.time > _timer)
            {
                PlayAudio(_audioClipsForSlide, Mathf.Clamp(volume, 0, 1));
                _timer = Time.time + _delay;
            }
        }

        private bool IsPlacedObject()
        {
            if (ObjectToPlace != null)
            {
                return ObjectToPlace.IsPlaced;
            }
            return false;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (IsPlacedObject()) return;
            if (Rigidbody == null) return;

            float velocity = Mathf.Clamp(Mathf.Abs(Rigidbody.linearVelocity.sqrMagnitude), 0.1f, 1f);
            CollisionAudio(velocity * _multiplierVolume);
        }

        private void OnCollisionStay(Collision other)
        {
            if (IsPlacedObject()) return;
            if (Rigidbody == null) return;

            if (Mathf.Abs(Rigidbody.angularVelocity.sqrMagnitude) > 3f) SlideAudio(Mathf.Abs(Rigidbody.angularVelocity.sqrMagnitude / 200));
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace ZFGinc.Player
{
    public class MovementAudio : MonoBehaviour
    {
        [SerializeField] private Character _character;
        [SerializeField] private CharacterMovement _characterMovement;
        [Header("Base audio sources")]
        [SerializeField] private AudioSource _audioSourceForSteps;
        [SerializeField] private AudioSource _audioSourceForJumps;

        [Space(15)]
        [Header("Base audio clips")]
        [SerializeField] private List<AudioClip> _playerStepsWalk;
        [SerializeField] private List<AudioClip> _playerStepsSprint;
        [SerializeField] private float stepIntervalWalk = 0.5f;
        [SerializeField] private float stepIntervalSprint = 0.25f;
        [Space]
        [SerializeField] private List<AudioClip> _playerJumpsStart;
        [SerializeField] private List<AudioClip> _playerJumpsEnd;

        private float nextStepTime = 0f;

        public void Initialize(Character character, CharacterMovement characterMovement)
        {
            _character = character;
            _characterMovement = characterMovement;

            _character.Jumped += PlayJumpStart;
            _character.OnGrounded += PlayJumpEnd;
        }

        private void Update()
        {
            HandleFootsteps();
        }

        private void OnDestroy()
        {
            _character.Jumped -= PlayJumpStart;
            _character.OnGrounded -= PlayJumpEnd;
        }

        private void HandleFootsteps()
        {
            if (!_characterMovement.isGrounded) return;
            if (Time.time < nextStepTime) return;

            if (_character.GetSpeed() > 0 && _character.GetSpeed() < _character.maxSprintSpeed && _character.speed > 0)
            {
                PlayStepWalk();
                nextStepTime = Time.time + stepIntervalWalk;
            }
            else if (_character.GetSpeed() >= _character.maxSprintSpeed && _character.speed > 0)
            {
                PlayStepSprint();
                nextStepTime = Time.time + stepIntervalSprint;
            }
        }

        public void PlayStepWalk()
        {
            _audioSourceForSteps.clip = _playerStepsWalk[Random.Range(0, _playerStepsWalk.Count)];
            _audioSourceForSteps.Play();
        }

        public void PlayStepSprint()
        {
            _audioSourceForSteps.clip = _playerStepsSprint[Random.Range(0, _playerStepsSprint.Count)];
            _audioSourceForSteps.Play();
        }

        public void PlayJumpStart()
        {
            _audioSourceForJumps.clip = _playerJumpsStart[Random.Range(0, _playerJumpsStart.Count)];
            _audioSourceForJumps.Play();
        }

        public void PlayJumpEnd()
        {
            _audioSourceForJumps.clip = _playerJumpsEnd[Random.Range(0, _playerJumpsEnd.Count)];
            _audioSourceForJumps.Play();
        }
    }
}
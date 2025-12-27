using UnityEngine;

namespace ZFGinc.Player
{
    public class CharacterInput : MonoBehaviour
    {
        public bool IsLockedInput = false;

        private Character _character;
        private InputBinding _inputBinding;

        public void Initialize(Character character, InputBinding inputBinding)
        {
            _character = character;
            _inputBinding = inputBinding;
        }

        private void Update()
        {
            if (IsLockedInput)
            {
                _character.SetMovementInput(Vector2.zero);
                _character.SetMovementDirection(Vector3.zero);
                return;
            }

            if (_inputBinding.IsMenuOpened) return;

            Vector2 inputMove = _inputBinding.GetMovementDirection();

            Vector3 movementDirection = Vector3.zero;

            if (_character.IsSwimming())
            {
                // Strafe

                movementDirection += _character.GetRightVector() * inputMove.x;

                // Forward, along camera view direction (if any) or along character's forward if camera not found 

                Vector3 forward =
                    _character.camera ? _character.cameraTransform.forward : _character.GetForwardVector();

                movementDirection += forward * inputMove.y;

                // Vertical movement

                if (_character.jumpInputPressed)
                {
                    // Use immersion depth to check if we are at top of water line,
                    // if yes, jump of of water

                    float depth = _character.CalcImmersionDepth();
                    if (depth > 0.65f)
                        movementDirection += _character.GetUpVector();
                    else
                    {
                        // Jump out of water

                        _character.SetMovementMode(Character.MovementMode.Falling);
                        _character.LaunchCharacter(_character.GetUpVector() * 9.0f, true);
                    }
                }
            }
            else
            {
                // Regular First Person movement relative to character's view direction

                movementDirection += _character.GetRightVector() * inputMove.x;
                movementDirection += _character.GetForwardVector() * inputMove.y;
            }

            _character.SetMovementInput(inputMove);
            _character.SetMovementDirection(movementDirection);

            if (Input.GetKeyDown(_inputBinding.crouchButton))
                _character.Crouch();
            else if (Input.GetKeyUp(_inputBinding.crouchButton))
                _character.UnCrouch();

            if (Input.GetKeyDown(_inputBinding.jumpButton))
                _character.Jump();
            else if (Input.GetKeyUp(_inputBinding.jumpButton))
                _character.StopJumping();

            if (Input.GetKey(_inputBinding.sprintButton))
            {
                _character.Sprint();
            }
            else if (Input.GetKeyUp(_inputBinding.sprintButton))
            {
                _character.StopSprint();
            }
        }
    }
}

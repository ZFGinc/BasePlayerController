using UnityEngine;

namespace ZFGinc.Player
{
    /// <summary>
    /// First person mouse look input.
    /// </summary>

    public class FirstPersonCharacterLookInput : MonoBehaviour
    {
        [Space(15.0f)]
        public bool invertLook = true;
        [Tooltip("Mouse look sensitivity")]
        public Vector2 mouseSensitivity = new Vector2(1.0f, 1.0f);

        [Space(15.0f)]
        [Tooltip("How far in degrees can you move the camera down.")]
        public float minPitch = -80.0f;
        [Tooltip("How far in degrees can you move the camera up.")]
        public float maxPitch = 80.0f;

        private FirstPersonCharacter _character;
        private InputBinding _input;

        public void Initialize(FirstPersonCharacter character, InputBinding inputBinding)
        {
            _character = character;
            _input = inputBinding;
        }

        private void Update()
        {
            Vector2 lookInput = _input.GetMouseDirection();

            lookInput *= mouseSensitivity;

            _character.AddControlYawInput(lookInput.x);
            _character.AddControlPitchInput(invertLook ? -lookInput.y : lookInput.y);
        }
    }
}

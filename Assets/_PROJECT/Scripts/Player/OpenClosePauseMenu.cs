using UnityEngine;

namespace ZFGinc.Player
{
    public class OpenClosePauseMenu : MonoBehaviour
    {
        public static OpenClosePauseMenu instance;

        [SerializeField] private GameObject _pausePanel;

        private InputBinding _inputBinding;

        private void Awake()
        {
            if (instance == null) instance = this;
            else
            {
                Destroy(instance.gameObject);
                instance = this;
            }

            _inputBinding = GetComponent<InputBinding>();
        }

        private void Start()
        {
            _inputBinding.IsMenuOpened = true;

            Initialize();
        }

        private void Update()
        {
            PlayerInput();
        }

        private void PlayerInput()
        {
            if (Input.GetKeyDown(_inputBinding.escapeButton))
            {
                PanelActiveSelf();
                CursorLockedState();
            }
        }

        private void Initialize()
        {
            PanelActiveSelf();
            CursorLockedState();
        }

        private void PanelActiveSelf()
        {
            _inputBinding.IsMenuOpened = !_inputBinding.IsMenuOpened;
            _pausePanel.SetActive(_inputBinding.IsMenuOpened);

            SetTimeScale();
        }

        private void SetTimeScale()
        {
            Time.timeScale = _inputBinding.IsMenuOpened ? 0f : 1f;
        }

        public void CursorLockedState()
        {
            Cursor.lockState = _inputBinding.IsMenuOpened ? CursorLockMode.None : CursorLockMode.Locked;
        }

        public void ButtonContinue()
        {
            PanelActiveSelf();
            CursorLockedState();
        }

        void OnApplicationQuit()
        {
            Destroy(instance.gameObject);
            instance = null;
        }
    }
}

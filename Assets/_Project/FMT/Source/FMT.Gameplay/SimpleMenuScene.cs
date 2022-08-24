using FMT.Network;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace FMT.Gameplay
{
    public class SimpleMenuScene : MonoBehaviour
    {
        [SerializeField] private Button _hostButton;
        [SerializeField] private Button _joinPanelButton;
        [SerializeField] private Button _joinButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private TMP_InputField _joinInputField;
        [SerializeField] private CanvasGroup _mainPanelCanvasGroup;
        [SerializeField] private CanvasGroup _joinPanelCanvasGroup;
        [SerializeField] private GameNetworkManager _networkManager;

        private CanvasGroup _currentPanel;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _hostButton.onClick.AddListener(HandleHostGameButtonClick);
            _joinPanelButton.onClick.AddListener(HandleJoinPanelButtonClick);
            _joinButton.onClick.AddListener(HandleJoinGameButtonClick);
            _quitButton.onClick.AddListener(HandleQuitButtonClick);

            _currentPanel = _mainPanelCanvasGroup;
        }

        private void HandleHostGameButtonClick()
        {
            _networkManager.StartHost();
        }

        private void HandleJoinPanelButtonClick()
        {
            ShowPanel(_joinPanelCanvasGroup);
        }

        private void HandleJoinGameButtonClick()
        {
            _networkManager.networkAddress = _joinInputField.text;

            _networkManager.StartClient();
        }

        private void ShowPanel(CanvasGroup canvas)
        {
            HidePanel(_currentPanel);

            canvas.alpha = 1;
            canvas.blocksRaycasts = true;
            canvas.interactable = true;
            _currentPanel = canvas;
        }

        private void HidePanel(CanvasGroup canvas)
        {
            canvas.alpha = 0;
            canvas.blocksRaycasts = false;
            canvas.interactable = false;
        }

        private void HandleQuitButtonClick()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}

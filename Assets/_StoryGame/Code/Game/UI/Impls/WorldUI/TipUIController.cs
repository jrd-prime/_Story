using _StoryGame.Core.Interactables.Interfaces;
using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.WorldUI
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(UIDocument))]
    public sealed class InteractablesTipUI : MonoBehaviour
    {
        [SerializeField] private int panelWidth = 1600;
        [SerializeField] private int panelHeight = 720;
        [SerializeField] private float panelScale = 1f;
        [SerializeField] private float pixelsPerUnit = 500f;
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        [SerializeField] private PanelSettings panelSettingsAsset;
        [SerializeField] private RenderTexture renderTextureAsset;

        private const string NameLabelId = "name";
        private const string TipLabelId = "tip";

        private UIDocument _uiDocument;
        private Label _nameLabel;
        private Label _tipLabel;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var meshFilter = GetComponent<MeshFilter>();
            var meshRenderer = GetComponent<MeshRenderer>();
            var panelSettings = Instantiate(panelSettingsAsset);

            var initializer = new TipInitializer();
            initializer.Init(new WorldTipData(_uiDocument, visualTreeAsset, transform, meshFilter, meshRenderer,
                renderTextureAsset, panelSettings,
                panelWidth, panelHeight, panelScale, pixelsPerUnit
            ));
        }

        private void OnEnable()
        {
            _nameLabel = _uiDocument.rootVisualElement.Q<Label>(NameLabelId);
            _tipLabel = _uiDocument.rootVisualElement.Q<Label>(TipLabelId);
        }

        public void SetNameText(string text) => _nameLabel.text = text;

        public void SetType(string type) => _tipLabel.text = type;
    }
}

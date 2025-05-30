using _StoryGame.Data.Room;
using _StoryGame.Game.Loot.Impls;
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
        private VisualElement _root;
        private Label _coreItem;
        private Label _note;
        private Label _energy;
        private Label _empty;
        private VisualElement _chancesC;
        private VisualElement _lootC;
        private Label _my;

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;
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

            _chancesC = _uiDocument.rootVisualElement.Q<VisualElement>("chances");
            _chancesC.style.display = DisplayStyle.None;

            _lootC = _uiDocument.rootVisualElement.Q<VisualElement>("loot");
            _lootC.style.display = DisplayStyle.None;
            _my = _uiDocument.rootVisualElement.Q<Label>("my");

            _coreItem = _uiDocument.rootVisualElement.Q<Label>("core-chance");
            _note = _uiDocument.rootVisualElement.Q<Label>("note-chance");
            _energy = _uiDocument.rootVisualElement.Q<Label>("energy-chance");
            _empty = _uiDocument.rootVisualElement.Q<Label>("empty-chance");
        }

        public void SetNameText(string text) => _nameLabel.text = text;

        public void SetType(string type) => _tipLabel.text = type;

        public void ShowLootChance(RoomBaseLootChanceVo lootChance)
        {
            _chancesC.style.display = DisplayStyle.Flex;

            _coreItem.text = $"{lootChance.coreItemBaseChance} %";
            _note.text = $"{lootChance.noteBaseChance} %";
            _energy.text = $"{lootChance.energyBaseChance} %";
            _empty.text = $"{lootChance.emptyBaseChance} %";
        }

        public void ShowObjLoot(InspectableData lootFor)
        {
            _lootC.style.display = DisplayStyle.Flex;

            var s = "";

            foreach (var loot in lootFor.InspectablesLoot)
                s += loot.Currency.LocalizationKey + " ";

            _my.text = s.ToUpper();
        }
    }
}

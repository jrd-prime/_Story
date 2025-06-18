using _StoryGame.Data.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace _StoryGame.Game.UI.Impls.Views.WorldViews
{
    public sealed class TipInitializer
    {
        private const string KTransparentShader = "Unlit/Transparent";
        private const string KTextureShader = "Unlit/Texture";
        private const string KMainTex = "_MainTex";
        private static readonly int MainTex = Shader.PropertyToID(KMainTex);

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private UIDocument _uiDocument;
        private RenderTexture _renderTexture;
        private PanelSettings _panelSettings;
        private Material _material;

        private int _panelWidth;
        private int _panelHeight;
        private float _panelScale;
        private float _pixelsPerUnit;
        private Transform _transform;
        private VisualTreeAsset _visualTreeAsset;

        public void Init(WorldTipData uiDocument)
        {
            _uiDocument = uiDocument.UiDocument;
            _visualTreeAsset = uiDocument.VisualTreeAsset;
            _transform = uiDocument.Transform;
            _meshFilter = uiDocument.MeshFilter;
            _meshRenderer = uiDocument.MeshRenderer;
            _renderTexture = uiDocument.RenderTextureAsset;
            _panelSettings = uiDocument.PanelSettings;
            _panelWidth = uiDocument.PanelWidth;
            _panelHeight = uiDocument.PanelHeight;
            _panelScale = uiDocument.PanelScale;
            _pixelsPerUnit = uiDocument.PixelsPerUnit;

            InitComponents();
            BuildPanel();
        }

        private void BuildPanel()
        {
            CreateRenderTexture();
            CreatePanelSettings();
            CreateUIDocument();
            CreateMaterial();

            SetMaterialToRenderer();
            SetPanelSize();
        }

        private void SetMaterialToRenderer()
        {
            if (_meshRenderer != null) _meshRenderer.sharedMaterial = _material;
        }

        private void SetPanelSize()
        {
            if (_renderTexture != null &&
                (_renderTexture.width != _panelWidth || _renderTexture.height != _panelHeight))
            {
                _renderTexture.Release();
                _renderTexture.width = _panelWidth;
                _renderTexture.height = _panelHeight;
                _renderTexture.Create();

                _uiDocument?.rootVisualElement?.MarkDirtyRepaint();
            }

            _transform.localScale = new Vector3(_panelWidth / _pixelsPerUnit, _panelHeight / _pixelsPerUnit, 1f);
        }

        private void CreateMaterial()
        {
            var shaderName = _panelSettings.colorClearValue.a < 1f ? KTransparentShader : KTextureShader;
            _material = new Material(Shader.Find(shaderName));
            _material.SetTexture(MainTex, _renderTexture);
        }

        private void CreateUIDocument()
        {
            _uiDocument.panelSettings = _panelSettings;
            _uiDocument.visualTreeAsset = _visualTreeAsset;
        }

        private void CreatePanelSettings()
        {
            _panelSettings.targetTexture = _renderTexture;
            _panelSettings.clearColor = true;
            _panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            _panelSettings.scale = _panelScale;
            _panelSettings.name = "Panel Settings";
        }

        private void CreateRenderTexture()
        {
            // RenderTextureDescriptor descriptor = renderTextureAsset.descriptor;
            RenderTextureDescriptor descriptor = _renderTexture.descriptor;
            descriptor.width = _panelWidth;
            descriptor.height = _panelHeight;

            _renderTexture = new RenderTexture(descriptor)
            {
                name = "UI Render Texture"
            };
        }

        private void InitComponents()
        {
            InitMeshRenderer();

            _meshFilter.sharedMesh = GetQuadMesh();
        }

        private void InitMeshRenderer()
        {
            _meshRenderer.sharedMaterial = null;
            _meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _meshRenderer.receiveShadows = false;
            _meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
            _meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            _meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }

        private static Mesh GetQuadMesh()
        {
            var tempQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var quadMesh = tempQuad.GetComponent<MeshFilter>().sharedMesh;
            Object.Destroy(tempQuad);
            return quadMesh;
        }
    }
}

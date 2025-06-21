using System;
using _StoryGame.Core.Common.Interfaces;
using _StoryGame.Data.UI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace _StoryGame.Game.UI.Impls.Views.WorldViews
{
    public sealed class TipInitializer
    {
        private const string KMainTex = "_MainTex";
        private static readonly int MainTex = Shader.PropertyToID(KMainTex);

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private UIDocument _uiDocument;
        private RenderTexture _renderTexture;
        private PanelSettings _panelSettings;
        private Material _material;
        private Shader _transparentShader;
        private Shader _textureShader;

        private int _panelWidth;
        private int _panelHeight;
        private float _panelScale;
        private float _pixelsPerUnit;
        private Transform _transform;
        private VisualTreeAsset _visualTreeAsset;
        private readonly IJLog _log;

        public TipInitializer(IJLog log) => _log = log;

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
            _transparentShader = uiDocument.TransparentShader;
            _textureShader = uiDocument.TextureShader;

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
            if (!_meshRenderer && !_material)
                throw new NullReferenceException("MeshRenderer or Material is null!");

            _meshRenderer.sharedMaterial = _material;
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
            var shader = _panelSettings.colorClearValue.a < 1f ? _transparentShader : _textureShader;

            if (shader == null)
            {
                _material.SetTexture(MainTex, Texture2D.whiteTexture);
                _log.Error($"Shader {shader.name} not found! Set Texture2D.whiteTexture");
                return;
            }

            _material = new Material(shader);
            _material.SetTexture(MainTex, _renderTexture);
        }

        private void CreateUIDocument()
        {
            _uiDocument.panelSettings = _panelSettings;
            _uiDocument.visualTreeAsset = _visualTreeAsset;

            if (!_uiDocument.panelSettings || !_uiDocument.visualTreeAsset)
                _log.Error("UIDocument configuration failed: PanelSettings or VisualTreeAsset is null!");
        }

        private void CreatePanelSettings()
        {
            _panelSettings.targetTexture = _renderTexture;
            _panelSettings.clearColor = true;
            _panelSettings.colorClearValue = new Color(0, 0, 0, 0); // Прозрачный фон
            _panelSettings.scaleMode = PanelScaleMode.ConstantPixelSize;
            _panelSettings.scale = _panelScale;
            _panelSettings.name = "Panel Settings";
        }

        private void CreateRenderTexture()
        {
            RenderTextureDescriptor descriptor = _renderTexture.descriptor;
            descriptor.width = _panelWidth;
            descriptor.height = _panelHeight;
            descriptor.graphicsFormat = GraphicsFormat.R8G8B8A8_SRGB;
            descriptor.depthBufferBits = 0;

            _renderTexture = new RenderTexture(descriptor) { name = "UI Render Texture" };

            if (!_renderTexture.Create())
                _log.Error("Failed to create RenderTexture!");
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

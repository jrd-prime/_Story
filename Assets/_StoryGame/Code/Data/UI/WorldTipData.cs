using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Data.UI
{
    public struct WorldTipData
    {
        public WorldTipData(UIDocument uiDocument, VisualTreeAsset visualTreeAsset, Transform transform,
            MeshFilter meshFilter, MeshRenderer pixelsPerUnit, RenderTexture renderTextureAsset,
            PanelSettings panelSettings, int panelWidth, int panelHeight, float panelScale, float pixelPerUnit,
            Shader transparentShader, Shader textureShader)
        {
            UiDocument = uiDocument;
            VisualTreeAsset = visualTreeAsset;
            Transform = transform;
            MeshFilter = meshFilter;
            MeshRenderer = pixelsPerUnit;
            RenderTextureAsset = renderTextureAsset;
            PanelSettings = panelSettings;
            PanelWidth = panelWidth;
            PanelHeight = panelHeight;
            PanelScale = panelScale;
            PixelsPerUnit = pixelPerUnit;
            TransparentShader = transparentShader;
            TextureShader = textureShader;
        }

        public UIDocument UiDocument { get; private set; }
        public VisualTreeAsset VisualTreeAsset { get; private set; }
        public Transform Transform { get; private set; }
        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }
        public RenderTexture RenderTextureAsset { get; private set; }
        public PanelSettings PanelSettings { get; private set; }
        public int PanelWidth { get; private set; }
        public int PanelHeight { get; private set; }
        public float PanelScale { get; private set; }
        public float PixelsPerUnit { get; private set; }
        public Shader TransparentShader { get; set; }
        public Shader TextureShader { get; set; }
    }
}

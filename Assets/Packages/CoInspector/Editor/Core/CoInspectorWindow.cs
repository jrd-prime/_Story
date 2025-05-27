using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Text;
using System.IO;
using UnityEditorInternal;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Audio;
#if UNITY_2021_2_OR_NEWER
#else
using UnityEditor.Experimental.SceneManagement;
#endif
namespace CoInspector
{
    public class CoInspectorWindow : EditorWindow, IHasCustomMenu
    {
        //RUNTIME SETTINGS
        internal static bool showHistory = true;
        internal static bool showTabName = true;
        internal static bool showTabTree = true;
        internal static bool showFilterBar = true;
        internal static bool showInstallMessage = true;
        internal static bool softSelection = true;
        internal static bool showIcons = true;
        internal static bool richNames = true;
        internal static bool autoFocus = false;
        internal static bool showScrollBar = true;
        internal static bool showAdditionalOptions = true;
        internal static bool rememberSessions = true;
        internal static bool useThumbKeys = true;
        internal static bool ignoreFolders = true;
        internal static bool collapsePrefabComponents = true;
        internal static bool openPrefabsInNewTab = true;
        internal static bool showTextAssetPreviews = true;
        internal static bool showAssetLabels = false;
        internal static bool showCollapseTool = true;
        internal static bool showLastClicked = true;
        internal static bool showMostClicked = true;
        internal static bool assetInspection = true;
        internal static bool componentCulling = true;
        internal static int sessionsMode = 0;
        internal static int tabCompactMode = 1;
        internal static int doubleClickMode = 1;
        internal static int scrollSpeedX = 2;
        internal static int scrollSpeedY = 2;
        internal static int scrollDirectionX = 1;
        internal static int scrollDirectionY = 1;

        //SETTINGS ASSET
        [SerializeField] internal UserSaveData settingsData;

        //EDITOR LOGIC
        private Editor gameObjectEditor;
        private Editor[] materialEditors;
        private Editor[] prefabMaterialEditors;
        private Editor[] componentEditors;
        private Editor assetEditor;
        private Editor[] prefabEditors;
        internal bool differentComponents = false;
        internal bool differentPrefabComponents = false;
#if UNITY_2020_2_OR_NEWER
        private UnityEditor.AssetImporters.AssetImporterEditor assetImportSettingsEditor;
#else
        private UnityEditor.Experimental.AssetImporters.AssetImporterEditor assetImportSettingsEditor;
#endif
        private AssetImporter assetImporter;
        private AssetImporter[] assetImporters;
        private Dictionary<Editor, MethodInfo> sceneMethods;
        private bool[] componentFoldouts_;
        private bool[] prefabFoldouts_;
        private bool[] prefabFoldoutsChangeTracker_;
        static internal string rootPath;
        private List<Action> methodsToRun = new List<Action>();
        private Component[] runtimePrefabComponents;
        private MaterialMapManager prefabMaterialManager = new MaterialMapManager();
        internal Rect flexibleRect;
        internal Rect componentScrollRect;
        internal Rect prefabScrollRect;
        internal Rect headerRect;
        internal float startComponentY;
        private Vector2 addComponentVector = new Vector2(230, 350);
        internal GameObjectTracker tracker;
        internal string currentTip = "";
        internal string numberOfTips = "";

        //DRAG AND RESIZE LOGIC
        internal bool assetsCollapsed = false;
        internal bool hoveringResize = false;
        internal int maximizeMode = 0;
        private RectOffset _paddingIcon;
        private RectOffset _paddingNoIcon;
        internal FloatingTab floatingTab;
        internal float lastValidTabWidth = 0;
        internal float suggestedHeight = 0;
        internal float userHeight = 0;
        internal float max;
        internal float currentPreviewHeight = 0;
        internal float settingsWidth = 0;
        internal float rawUserHeight = 0;
        internal bool alreadyCalculatedHeight = false;
        internal float lastKnownHeight = 0;
        private bool pendingComponentDrag = false;
        internal ComponentDragOperation pendingOperation;
        private bool dragging = false;
        private bool triggeringADrag = false;
        [SerializeField] internal bool GOdragging = false;
        private int dragIndex = -1;
        private int dragTargetIndex = -1;
        private Rect dragRect;
        private Rect assetViewRect;
        private Vector2 mousePositionOnClick;
        private bool waitingToDrag = false;
        private float resizeOriginalCursorY;
        private float startHeight = 0;
        private bool resizingAssetView = false;
        private bool ignoreNextDragEvent = false;
        internal static bool alreadyMovingComponent = false;
        private int performPasteComponent = 0;

        //RUNTIME TARGETS AND SELECTION
        internal GameObject targetGameObject;
        internal UnityObject targetObject;
        internal List<List<GameObject>> lastClicked;
        internal List<List<GameObject>> mostClicked;
        [SerializeField] internal UnityObject[] targetObjects;
        private UnityObject[] ignoreSelection;
        [SerializeField] private Dictionary<Type, List<UnityObject>> sortedAssetSelection;
        [SerializeField] internal bool forceSelection = false;
        [SerializeField] internal bool ignoreNextSelection = false;

        //TABS
        [SerializeField] internal List<TabInfo> tabs;
        [SerializeField] internal List<TabInfo> closedTabs;
        [SerializeField] internal TabSession lastSessionData;
        [SerializeField] internal List<HistoryAssets> historyAssets;
        [NonSerialized] internal float barSpacing = -1;
        internal int activeIndex = 0;
        internal int lastActiveIndex = -1;
        private Rect scrollRect;
        private Rect activeTabRect;
        private int pendingTabSwitch = -1;
        private Vector2 mousePosition = Vector2.zero;
        private Vector2 clickMousePosition = Vector2.zero;


        //STYLES   
        private GUIStyle lockButtonStyle;

        //RUNTIME VARIABLES
        [SerializeField] internal bool lockedAsset = false;
        [SerializeField] internal bool maximizedAssetView = false;
        [SerializeField] internal bool filteringComponents = false;
        internal bool showImportSettings = false;
        internal bool holdingCtrl = false;
        private bool isRepainting = false;
        private float dirtyDuration = 0f;
        private float startTime = 0f;
        internal static List<CoInspectorWindow> instances = new List<CoInspectorWindow>();
        internal static float lastOpen = -1;
        internal static bool textPluginPresent = false;
        internal static bool odinInspectorPresent = false;
        [SerializeField] internal bool pendingRestore = false;
        [SerializeField] internal bool exitingPlayMode = false;
        [SerializeField] internal bool enteringPlayMode = false;
        [SerializeField] internal bool scenesChanged = false;
        [SerializeField] internal bool changingScenes = false;
        private static List<string> namespaces = new List<string> { "ScriptInspector" };
        internal static SceneInfo activeScene;
        internal static string lastValidScenePath = "";
        private bool switchingTabs = false;
        [SerializeField] internal bool debugAsset = false;
        private bool awaitingAssetClick = false;
        private bool onPrefabSceneMode = false;
        [SerializeField] internal Vector2 scrollPosition;
        [NonSerialized] internal bool toolScrollBarVisible = false;
        [SerializeField] internal Vector2 toolbarScrollPosition;
        private Vector2 assetScrollPosition = Vector2.zero;
        private List<float> totalWidth = new List<float>();
        internal Rect previewRect = new Rect(0, 0, 0, 0);
        internal static bool justOpened = false;
        [SerializeField] internal bool isLocked = true;
        [SerializeField] internal int lastClickedTab = -1;
        [SerializeField] internal TabInfo previousTab;
        private float lastTabClick = -1;
        private float lastChangeOfState = 0;
        [SerializeField] internal bool inActualPlayMode = false;

        [MenuItem("GameObject/★ Open Selection in a New Tab", false, 0)]
        public static void OpenSelectionInNewTab()
        {
            if (Time.realtimeSinceStartup - lastOpen < 0.5f && lastOpen != -1)
            {
                return;
            }
            CoInspectorWindow insp = null;

            if (MainCoInspector)
            {
                insp = MainCoInspector;
            }
            else
            {
                ShowWindow();
                if (MainCoInspector)
                {
                    insp = MainCoInspector;
                }
            }
            if (insp != null)
            {
                lastOpen = Time.realtimeSinceStartup;
                EditorApplication.delayCall += () =>
                {
                    if (Selection.gameObjects.Length == 0)
                    {
                        return;
                    }

                    if (Selection.gameObjects.Length == 1)
                    {
                        insp.AddTabNext(Selection.gameObjects[0], false);
                        return;
                    }

                    if (Selection.gameObjects.Length > 1)
                    {
                        insp.AddMultiTabNext(Selection.gameObjects, false);
                    }
                };
            }
        }

        [MenuItem("GameObject/★ Open Selection in a New Tab", true)]
        internal static bool ValidateOpenSelectionInNewTab()
        {
            return Selection.activeGameObject != null;
        }
        [MenuItem("Window/CoInspector/Open CoInspector")]
        public static void ShowWindow()
        {
            justOpened = true;
            CoInspectorWindow insp = GetWindow<CoInspectorWindow>("CoInspector");

            /* Type[] desiredDockNextTo = new Type[] { Reflected.GetInspectorWindowType() };
             CoInspectorWindow insp = GetWindow<CoInspectorWindow>("CoInspector", desiredDockNextTo);*/
            insp.titleContent = new GUIContent("CoInspector", CustomGUIContents.MainIconImage);

            Vector2 middle = FirstInstallWindow.RightSideOfScreen(450, 700);
            insp.position = new Rect(middle.x, middle.y, 450, 700);
            RegisterWindow(insp);
            insp.Focus();
        }
        [MenuItem("CONTEXT/Component/Collapse Components/Collapse All", false, 1000)]
        public static void CollapseAllComponents(MenuCommand command)
        {
            Component component = (Component)command.context;
            if (component)
            {
                if (MainCoInspector)
                {
                    MainCoInspector.SetAllComponentsTo(false);
                }
            }
        }
        [MenuItem("CONTEXT/Component/Expand Components/Expand All", false, 1000)]
        public static void ExpandAllComponents(MenuCommand command)
        {
            if (Time.realtimeSinceStartup - lastOpen < 0.5f && lastOpen != -1)
            {
                return;
            }
            Component component = (Component)command.context;
            if (component)
            {
                if (MainCoInspector)
                {
                    MainCoInspector.SetAllComponentsTo(true);
                }
            }
        }
        [MenuItem("CONTEXT/Component/Move to Top", false, 0)]
        public static void MoveComponentToFirst(MenuCommand command)
        {
            Component component = (Component)command.context;


            if (component)
            {
                while (ComponentUtility.MoveComponentUp(component))
                {
                    ComponentUtility.MoveComponentUp(component);
                }
                FocusComponentIfNecessary(component);
            }
        }
        static void FocusComponentIfNecessary(Component component)
        {
            if (!EditorUtils.IsAPrefabAsset(component.gameObject))
            {
                if (MainCoInspector)
                {
                    ComponentMap map = MainCoInspector.GetActiveTab().GetFoldoutMapForComponent(component, null);
                    if (map != null)
                    {
                        map.awaitingScroll = true;
                        map.focusAfter = true;
                        MainCoInspector.RepaintForAWhile();
                    }
                }
            }
            else if (MainCoInspector)
            {
                MainCoInspector.alreadyCalculatedHeight = false;
                MainCoInspector.RepaintForAWhile();
            }
        }
        [MenuItem("CONTEXT/Component/Move to Top", true)]
        internal static bool ValidateMoveComponentToFirst(MenuCommand command)
        {
            Component component = (Component)command.context;
            Component[] components = component.GetComponents<Component>();
            if (components.Length > 1 && components[1] != component)
            {
                return true;
            }
            return false;
        }
        [MenuItem("CONTEXT/Component/Expand Components/Expand All But This", false, 1000)]
        internal static void ExpandAllButThis(MenuCommand command)
        {
            if (Time.realtimeSinceStartup - lastOpen < 0.5f && lastOpen != -1)
            {
                return;
            }
            Component component = (Component)command.context;
            if (component)
            {
                if (MainCoInspector)
                {
                    MainCoInspector.SetAllComponentsTo(true, component);
                }
            }
        }
        [MenuItem("CONTEXT/Component/Expand Components/Expand All But This", true)]
        internal static bool ValidateExpandAllButThis(MenuCommand command)
        {
            Component component = (Component)command.context;
            return !EditorUtils.IsAPrefabAsset(component.gameObject);
        }
        [MenuItem("CONTEXT/Component/Collapse Components/Collapse All But This", true)]
        internal static bool ValidateCollapseAllButThis(MenuCommand command)
        {
            Component component = (Component)command.context;
            return !EditorUtils.IsAPrefabAsset(component.gameObject);
        }
        [MenuItem("CONTEXT/Component/Collapse Components/Collapse All", true)]
        internal static bool ValidateCollapseAllComponents(MenuCommand command)
        {
            Component component = (Component)command.context;
            if (!EditorUtils.IsAPrefabAsset(component.gameObject))
            {
                if (MainCoInspector)
                {
                    return !MainCoInspector.GetActiveTab().AreAllCollapsed();
                }
            }
            return false;
        }
        [MenuItem("CONTEXT/Component/Expand Components/Expand All", true)]
        internal static bool ValidateExpandAllComponents(MenuCommand command)
        {
            Component component = (Component)command.context;
            if (!EditorUtils.IsAPrefabAsset(component.gameObject))
            {
                if (MainCoInspector)
                {
                    return !MainCoInspector.GetActiveTab().AreAllExpanded();
                }
            }
            return false;
        }
        [MenuItem("CONTEXT/Component/Collapse Components/Collapse All But This", false, 1000)]
        internal static void CollapseAllButThis(MenuCommand command)
        {
            if (Time.realtimeSinceStartup - lastOpen < 0.5f && lastOpen != -1)
            {
                return;
            }
            Component component = (Component)command.context;
            if (component)
            {
                if (MainCoInspector)
                {
                    MainCoInspector.SetAllComponentsTo(false, component);
                }
            }
        }
        void SetAllComponentsTo(bool expanded, Component componentToExclude = null)
        {
            lastOpen = Time.realtimeSinceStartup;
            if (GetActiveTab().componentMaps != null && GetActiveTab().componentMaps.Count > 0)
            {
                foreach (ComponentMap map in GetActiveTab().componentMaps)
                {
                    if (map.component == null)
                    {

                        continue;
                    }
                    if (map.component != componentToExclude)
                    {

                        map.foldout = expanded;
                    }
                    else if (componentToExclude != null)
                    {
                        map.foldout = !expanded;
                        map.awaitingScroll = true;
                        map.focusAfter = true;

                    }
                    RepaintForAWhile();
                }
            }
        }
        private void CloseCoInspector()
        {
            CloseCoInspector(SceneManager.GetActiveScene(), true);
        }
        internal void RepaintForAWhile()
        {
            Repaint();
            RepaintFor(0.1f);
        }
        internal void RepaintFor(float time)
        {
            if (!isRepainting)
            {
                isRepainting = true;
                dirtyDuration = time;
                startTime = Time.realtimeSinceStartup;
            }
        }
        private void KeepRepainting()
        {
            if (isRepainting && Time.realtimeSinceStartup < startTime + dirtyDuration)
            {
                GUI.changed = true;
                Repaint();
                return;
            }
            if (isRepainting)
            {
                startTime = 0;
                dirtyDuration = 0;
            }
            isRepainting = false;
        }
        private void CloseCoInspector(Scene scene)
        {
            if (!EnteringPlaymode)
            {
                scenesChanged = true;
                changingScenes = true;
            }
            CloseCoInspector(scene, true);
        }
        private void CloseCoInspector(Scene scene, bool closing)
        {
            if (closing || !closing && !exitingPlayMode)
            {
                DoClose();
            }
        }

        private void DoClose()
        {
            activeScene = SceneInfo.FromActiveScene(activeScene);
            if (!Application.isPlaying)
            {
                UpdateAllTabPaths();
            }
            CloseScene();
            CleanAllEditors();
            instances.Remove(this);
        }

        private void ReopenCoInspector(Scene scene, OpenSceneMode mode)
        {
            if (instances.Contains(this))
            {
                return;
            }
            if (!instances.Contains(this))
            {
                RegisterWindow(this);
                justOpened = true;
                OnEnable();
            }/*
            else
            {
                Debug.Log("CoInspector already open 2");
            }*/
        }
        private void ReopenCoInspector(Scene scene, LoadSceneMode mode)
        {
            if (instances.Contains(this))
            {
                return;
            }
            ReopenCoInspector(scene, OpenSceneMode.Single);
        }
        internal RectOffset PaddingIcon
        {
            get
            {
                if (_paddingIcon == null || _paddingIcon.left != 20)
                    _paddingIcon = new RectOffset(20, 0, 0, 0);
                return _paddingIcon;
            }
            set { _paddingIcon = value; }
        }
        internal RectOffset PaddingNoIcon
        {
            get
            {
                if (_paddingNoIcon == null || _paddingNoIcon.left != 8)
                    _paddingNoIcon = new RectOffset(8, 0, 0, 0);
                return _paddingNoIcon;
            }
            set { _paddingNoIcon = value; }
        }

        internal FloatingTab FloatingTab
        {
            get
            {
                if (floatingTab == null)
                {
                    floatingTab = new FloatingTab(this);
                }
                return floatingTab;
            }
        }
        bool OnBlankWorkspace()
        {
            return tabs.Count == 1 && tabs[0].newTab;
        }

        void ManageAssemblyReload()
        {
            if (!exitingPlayMode && !EnteringPlaymode)
            {
                SaveSettings();
            }
            CleanAllAssetEditors();
            CleanGameObjectEditors();
            if (sessionsMode == 2)
            {
                pendingRestore = true;
            }
            triggerDrag = false;
            triggeringADrag = false;
            isRepainting = false;
        }
        void ManageAfterAssemblyReload()
        {

            triggerDrag = false;
            triggeringADrag = false;
            isRepainting = false;
            if (sessionsMode == 2 && pendingRestore)
            {
                RestoreSession(true);
            }
            pendingRestore = false;
            UpdateAllWidths();
            ScrollToActiveTab();
            return;
        }
        private void OnActiveSceneChanged(Scene previousScene, Scene newScene)
        {
            DoRegisterSceneChange();
        }

        private void DoRegisterSceneChange()
        {
            if (exitingPlayMode)
            {
                //Debug.Log("Scene change was called when exiting play mode");
                return;
            }
            HandleSceneChanged();
        }

        void AddIfNecessary(Editor editor)
        {
            if (editor == null)
            {
                return;
            }
            if (sceneMethods == null)
            {
                sceneMethods = new Dictionary<Editor, MethodInfo>();
            }
            if (sceneMethods.ContainsKey(editor))
            {
                return;
            }
            MethodInfo method = editor.GetType().GetMethod("OnSceneGUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (method != null)
            {
                sceneMethods.Add(editor, method);
            }
        }

        void _OnSceneGUI(SceneView sceneView)
        {
            if (sceneMethods != null)
            {
                Dictionary<Editor, MethodInfo> toKeep = new Dictionary<Editor, MethodInfo>();
                foreach (Editor editor in sceneMethods.Keys)
                {
                    if (editor == null || editor.target == null)
                    {
                        continue;
                    }
                    MethodInfo method = sceneMethods[editor];
                    toKeep.Add(editor, method);
                    if (method != null)
                    {
                        method.Invoke(editor, null);
                    }
                }
                sceneMethods = toKeep;
            }
        }
#if UNITY_2021_2_OR_NEWER
        void OpenPrefabStage(UnityEditor.SceneManagement.PrefabStage prefabStage)
        {
            onPrefabSceneMode = true;
            ignoreNextSelection = true;
            AddTabNext(prefabStage.prefabContentsRoot, true);
        }
        void ClosePrefabStage(UnityEditor.SceneManagement.PrefabStage prefabStage)
        {
            onPrefabSceneMode = SceneIsInPrefabMode();
        }
#else
        void OpenPrefabStage(UnityEditor.Experimental.SceneManagement.PrefabStage prefabStage)
        {
            onPrefabSceneMode = true;
            ignoreNextSelection = true;
            AddTabNext(prefabStage.prefabContentsRoot, true);
        }
        void ClosePrefabStage(UnityEditor.Experimental.SceneManagement.PrefabStage prefabStage)
        {
            onPrefabSceneMode = SceneIsInPrefabMode();
        }
#endif
        void RefreshNamesAndRepaint()
        {
            RefreshAllTabNames();
            RefreshAllIcons();
            Repaint();
        }

        void HookEvents()
        {
#if UNITY_2021_2_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage.prefabStageOpened += OpenPrefabStage;
            UnityEditor.SceneManagement.PrefabStage.prefabStageClosing += ClosePrefabStage;
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabStageOpened += OpenPrefabStage;
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabStageClosing += ClosePrefabStage;
#endif
            EditorApplication.hierarchyChanged += RefreshNamesAndRepaint;
            SceneView.duringSceneGui += _OnSceneGUI;
            Selection.selectionChanged += HandleSelectionChange;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
            EditorApplication.hierarchyWindowItemOnGUI += HandleMiddleClick;
            EditorApplication.projectWindowItemOnGUI += HandleAssetClick;
            EditorApplication.update += BackUpdate;
            EditorApplication.quitting += TrySaveSession;
            Undo.undoRedoPerformed += UndoRedoPerformed;
            AssemblyReloadEvents.afterAssemblyReload += ManageAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += ManageAssemblyReload;
            EditorSceneManager.sceneSaving += OnSceneSaving;
            // EditorSceneManager.sceneClosing += CloseCoInspector;
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
            onPrefabSceneMode = SceneIsInPrefabMode();
            // SceneManager.sceneUnloaded += CloseCoInspector;

        }
        void UnhookEvents()
        {
#if UNITY_2021_2_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage.prefabStageOpened -= OpenPrefabStage;
            UnityEditor.SceneManagement.PrefabStage.prefabStageClosing -= ClosePrefabStage;
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabStageOpened -= OpenPrefabStage;
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabStageClosing -= ClosePrefabStage;
#endif
            EditorApplication.hierarchyChanged -= RefreshNamesAndRepaint;
            SceneView.duringSceneGui -= _OnSceneGUI;
            Selection.selectionChanged -= HandleSelectionChange;
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.hierarchyWindowItemOnGUI -= HandleMiddleClick;
            EditorApplication.projectWindowItemOnGUI -= HandleAssetClick;
            EditorApplication.update -= BackUpdate;
            EditorApplication.quitting -= TrySaveSession;
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            AssemblyReloadEvents.afterAssemblyReload -= ManageAfterAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload -= ManageAssemblyReload;
            EditorSceneManager.sceneSaving -= OnSceneSaving;
            EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
        private static CoInspectorWindow mainCoInspector;
        public static CoInspectorWindow MainCoInspector
        {
            get
            {
                if (mainCoInspector == null && instances != null && instances.Count > 0)
                {
                    mainCoInspector = instances[0];
                }
                return mainCoInspector;
            }
        }
        internal static UserSaveData FindSettingsObject()
        {
            if (MainCoInspector)
            {
                if (MainCoInspector.settingsData && AssetDatabase.GetAssetPath(MainCoInspector.settingsData) != "")
                {
                    return MainCoInspector.settingsData;
                }
            }
            string path = _GetRootPath() + "/Settings/UserData.asset";
            UnityObject _settingsData = AssetDatabase.LoadAssetAtPath<UserSaveData>(path);
            if (_settingsData)
            {
                return _settingsData as UserSaveData;
            }
            UserSaveData settingsData = AssetDatabase.LoadAssetAtPath<UserSaveData>(path);
            if (settingsData)
            {
                return settingsData;
            }
            return null;
        }
        static string GetLastModifiedAssetPath(string[] guids)
        {
            string lastModifiedPath = "";
            System.DateTime lastModifiedTime = System.DateTime.MinValue;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DateTime assetModifiedTime = File.GetLastWriteTime(path);

                if (assetModifiedTime > lastModifiedTime)
                {
                    lastModifiedPath = path;
                    lastModifiedTime = assetModifiedTime;
                }
            }
            return lastModifiedPath;
        }
        void FixReferences()
        {
            if (settingsData == null)
            {
                settingsData = FindSettingsObject();
            }
            if (settingsData == null)
            {
                settingsData = AutoCreateSettings();
            }
        }
        private void OnEnable()
        {
            FixReferences();
            CleanTabs();
            if (instances == null)
            {
                instances = new List<CoInspectorWindow>();
            }
            RegisterWindow(this);

            titleContent = new GUIContent("CoInspector", CustomGUIContents.MainIconImage);
            /*macOS 'UpdateScene every frame' makes it impossible to keep for a good performance.*/
            if (!(Application.platform is RuntimePlatform.OSXEditor))
            {
                autoRepaintOnSceneChange = true;
            }
            wantsMouseEnterLeaveWindow = true;
            isRepainting = false;
            UnhookEvents();
            HookEvents();
            RestoreLastAssets();
            textPluginPresent = IsAnyNamespacePresent(namespaces);
            odinInspectorPresent = IsOdinInspectorPresent();
            lastTabClick = -1;
            lastClickedTab = -1;
            if (settingsData)
            {
                if (TryLoadSession() /*&& ((sessionsMode != 2)*/ || inActualPlayMode || exitingPlayMode)
                {
                    if ((!justOpened && !lastSessionData.checkingToLoad) || sessionsMode == 1 || (inActualPlayMode && !lastSessionData.checkingToLoad && sessionsMode == 1) || (pendingRestore && !lastSessionData.checkingToLoad) || exitingPlayMode && !lastSessionData.checkingToLoad)
                    {
                        if (pendingRestore)
                        {
                            pendingRestore = false;
                        }
                        if (!StartedPlayModeInOtherScene() || sessionsMode == 1)
                        {
                            RestoreSession(true);
                        }

                        else
                        {
                            HandleSceneChanged(false);
                        }
                    }
                    else
                    {
                        if (lastSessionData != null && lastSessionData.checkingToLoad)
                        {
                            justOpened = true;
                            lastSessionData.checkingToLoad = false;
                        }
                        CleanTabs();
                    }
                }
                else if (justOpened)
                {
                    UpdateCurrentTip();
                    CleanTabs();
                }
            }
            if (tabs == null)
            {
                tabs = new List<TabInfo>();
            }
            if (tabs.Count == 0)
            {
                tabs.Add(new TabInfo(GetActiveTab().target, 0, this));
                FocusTab(0);
            }
            UpdateAllWidths();
            if (exitingPlayMode)
            {
                exitingPlayMode = false;
                activeScene = SceneInfo.FromActiveScene(activeScene);
            }
        }

        private bool StartedPlayModeInOtherScene()
        {
            if (inActualPlayMode && !exitingPlayMode && settingsData != null)
            {
                if (settingsData.playModeStartScene != default(Scene) && settingsData.playModeStartScene != SceneManager.GetActiveScene())
                {
                    return true;
                }
            }
            if (settingsData != null)
            {
                settingsData.playModeStartScene = default(Scene);
            }
            return false;
        }

        private void UndoRedoPerformed()
        {
            RefreshAllTabNames();
            RefreshAllIcons();
            CleanAllTabMaps();
        }
        void CleanGameObjectEditors()
        {
            CleanAllTabMaps();
            if (materialEditors != null)
            {
                for (int i = 0; i < materialEditors.Length; i++)
                {
                    if (materialEditors[i] != null)
                    {
                        DestroyImmediate(materialEditors[i]);
                    }
                }
                materialEditors = null;
            }
            if (componentEditors != null)
            {
                for (int i = 0; i < componentEditors.Length; i++)
                {
                    if (componentEditors[i] != null)
                    {
                        DestroyImmediate(componentEditors[i]);
                    }
                }
                componentEditors = null;
            }
            if (gameObjectEditor != null)
            {
                DestroyImmediate(gameObjectEditor);
            }
        }
        void CleanAllEditors()
        {
            CleanAllTabMaps();
            if (componentEditors != null)
            {
                for (int i = 0; i < componentEditors.Length; i++)
                {
                    if (componentEditors[i] != null)
                    {
                        DestroyImmediate(componentEditors[i]);
                    }
                }
                componentEditors = null;
            }
            if (prefabEditors != null)
            {
                for (int i = 0; i < prefabEditors.Length; i++)
                {
                    if (prefabEditors[i] != null)
                    {
                        DestroyImmediate(prefabEditors[i]);
                    }
                }
                prefabEditors = null;
            }
            if (materialEditors != null)
            {
                for (int i = 0; i < materialEditors.Length; i++)
                {
                    if (materialEditors[i] != null)
                    {
                        DestroyImmediate(materialEditors[i]);
                    }
                }
                materialEditors = null;
            }
            if (prefabMaterialEditors != null)
            {
                for (int i = 0; i < prefabMaterialEditors.Length; i++)
                {
                    if (prefabMaterialEditors[i] != null)
                    {
                        DestroyImmediate(prefabMaterialEditors[i]);
                    }
                }
                prefabMaterialEditors = null;
            }
            if (assetEditor != null)
            {
                DestroyImmediate(assetEditor);
            }
            if (assetImportSettingsEditor != null)
            {
                DestroyImmediate(assetImportSettingsEditor);
            }
            if (assetImporters != null)
            {
                assetImporters = null;
            }
            if (assetImporter != null)
            {
                assetImporter = null;
            }
            if (gameObjectEditor != null)
            {
                DestroyImmediate(gameObjectEditor);
            }
        }
        void ReadLastClicked()
        {
            if (tracker != null)
            {
                mostClicked = tracker.GetMostClicked();
                lastClicked = tracker.GetRecentlyClicked();
                tracker.UpdateContents();
            }
        }
        internal void UpdateClicked(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }
            if (tracker == null)
            {
                tracker = new GameObjectTracker();
            }
            tracker.UpdateClicked(gameObject);
        }

        internal void UpdateClicked(GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0)
            {
                return;
            }
            if (tracker == null)
            {
                tracker = new GameObjectTracker();
            }
            tracker.UpdateClicked(gameObjects);
        }

        internal void CleanTabs()
        {
            tabs = new List<TabInfo>();
            closedTabs = new List<TabInfo>();
            activeIndex = -1;
            //tracker = new GameObjectTracker();
            CleanGameObjectEditors();
            AddTabNext();
        }


        void CloseScene()
        {
            if (EnteringPlaymode)
            {
                return;
            }
            if (settingsData && !InRecoverScreen())
            {
                settingsData.SaveData(false, this, true);
                CleanTabs();
            }
            else if (InRecoverScreen() && lastSessionData != null)
            {
                lastSessionData.checkingToLoad = true;
                lastSessionData.SaveAssets(this);
            }
            CleanAllEditors();
        }

        private void OnDisable()
        {
            if (!settingsData)
            {
                settingsData = AutoCreateSettings();
            }
            UnhookEvents();
            if (settingsData && !InRecoverScreen())
            {
                if (!inActualPlayMode)
                {
                    settingsData.SaveData(this);
                }
                else
                {
                    settingsData.SaveData(false, this);
                }
            }
            else if (InRecoverScreen())
            {
                lastSessionData.checkingToLoad = true;
            }
            EditorApplication.delayCall += CleanAllEditors;
        }

        internal void RunNextFrame(Action action)
        {
            if (action == null)
            {
                return;
            }
            if (methodsToRun == null)
            {
                methodsToRun = new List<Action>();
            }
            if (methodsToRun.Contains(action))
            {
                return;
            }
            methodsToRun.Add(action);
            Repaint();
        }

        internal void RunDelayedMethods()
        {
            if (methodsToRun != null && methodsToRun.Count > 0)
            {
                foreach (var method in methodsToRun)
                {
                    if (method != null)
                    {
                        method.Invoke();
                    }
                }
                methodsToRun.Clear();
            }
        }

        internal static void CallVeryLate(Action action, int numNestedDelays = 3)
        {
            DoCallVeryLate(action, numNestedDelays);
        }
        static void DoCallVeryLate(Action action, int numNestedDelays, int currentDelay = 0)
        {
            if (currentDelay < numNestedDelays)
            {
                currentDelay++;
                EditorApplication.delayCall += () =>
                {
                    DoCallVeryLate(action, numNestedDelays, currentDelay);
                };
            }
            else
            {
                action();
            }
        }
        void TriggerDelayedDrag()
        {
            HandleComponentDrag();
            triggerDrag = false;
            triggeringADrag = false;
        }

        /* Let's give this a think.

            private bool HasSerializedObjectChanged(SerializedObject serializedObject, SerializedProperty[] storedProperties)
        {
                SerializedProperty property = serializedObject.GetIterator();
                int index = 0;
                while (property.NextVisible(true))
                {
                    if (property.displayName == "X")
                    {
                         Debug.Log("Checking property " + property.floatValue + " against " + storedProperties[index].floatValue);
                    }
                    if (index >= storedProperties.Length)
                        return true;

                    if (!SerializedProperty.EqualContents(property, storedProperties[index]))
                        return true;
                    index++;
                }
                if (index != storedProperties.Length)
                    return true;

                return false;
        } */

        void CheckForMacChanges()
        {
            if (IsActiveTabValid())
            {
                var activeTab = GetActiveTab();
                if (!activeTab.newTab)
                {
                    if (EditorUtils.CheckEditorChanged(gameObjectEditor))
                    {
                        return;
                    }
                    if (EditorUtils.CheckEditorsChanged(componentEditors))
                    {
                        return;
                    }
                    if (EditorUtils.CheckEditorsChanged(materialEditors))
                    {
                        return;
                    }
                }
            }
            if (EditorUtils.CheckEditorChanged(assetEditor))
            {
                return;
            }
            if (EditorUtils.CheckEditorChanged(assetImportSettingsEditor))
            {
                return;
            }
            if (EditorUtils.CheckEditorsChanged(prefabEditors))
            {
                return;
            }
            if (EditorUtils.CheckEditorsChanged(prefabMaterialEditors))
            {
                return;
            }
        }
        void BackUpdate()
        {
            if (Reflected.IsTimeControlPlaying(assetEditor))
            {
                Repaint();
            }
            AutoCheckScene();
            UpdateTabComponentCulling();
            if (Application.platform is RuntimePlatform.OSXEditor)
            {
                CheckForMacChanges();
            }
            if (DragAndDrop.objectReferences.Length > 0 && Event.current != null && Event.current.type == EventType.Repaint)
            {
                bool isAnyNull = false;
                foreach (var obj in DragAndDrop.objectReferences)
                {
                    if (obj == null)
                    {
                        isAnyNull = true;
                        break;
                    }
                }
                if (isAnyNull)
                {
                    pendingComponentDrag = false;
                    pendingOperation = null;
                    triggerDrag = false;
                    triggeringADrag = false;
                    DragAndDrop.objectReferences = null;
                    Repaint();
                }
            }
            if (triggerDrag && !triggeringADrag)
            {
                triggeringADrag = true;
                CallVeryLate(TriggerDelayedDrag, 2);
            }
            KeepRepainting();
        }

        void UpdateTabComponentCulling()
        {
            if (!componentCulling)
            {
                return;
            }
            if (GetActiveTab() != null && !IsActiveTabNew())
            {
                if (GetActiveTab().componentMaps != null)
                {
                    foreach (ComponentMap map in GetActiveTab().componentMaps)
                    {
                        if (map != null)
                        {
                            UpdateComponentMapCulling(map);
                        }
                    }
                }
            }
        }
        void UpdateComponentMapCulling(ComponentMap componentMap)
        {
            if (componentMap.height == -1)
            {
                componentMap.isCulled = false;
                return;
            }
            float yPosition = GetComponentHeightForMap(componentMap);
            float componentBottom = yPosition + componentMap.height;
            if (componentBottom < componentScrollRect.y)
            {
                if (!componentMap.isCulled)
                {
                    //  Debug.Log("Culling component " + componentMap.index + " " + yPosition + " " + componentMap.height);
                    componentMap.isCulled = true;
                    Repaint();
                }
                return;
            }
            else if (yPosition > componentScrollRect.yMax)
            {
                if (!componentMap.isCulled)
                {
                    //Debug.Log("Culling component " + componentMap.index + " " + yPosition + " " + componentMap.height);
                    componentMap.isCulled = true;
                    Repaint();
                }
                return;
            }
            else if (componentMap.isCulled)
            {
                Repaint();
            }
            componentMap.isCulled = false;
            return;
        }


        void RestoreCurrentWorkspace()
        {
            if (TryLoadSession())
            {
                RestoreSession();
            }
        }
        internal bool EmptySavedSession()
        {
            if (lastSessionData != null && lastSessionData.tabs != null && lastSessionData.tabs.Count == 1 && lastSessionData.tabs[0].newTab)
            {
                return true;
            }
            return false;
        }

        internal float GetPreviewHeight()
        {
            return IntGetPreviewHeight();
        }
        internal int IntGetPreviewHeight()
        {
            if (targetObject != null && !ShouldDrawImportSettings())
            {
                if (targetObject is Texture || targetObject is Sprite)
                {
                    return 150;
                }
                if (targetObject is AnimationClip)
                {
                    return 200;
                }
                if (targetObject is VideoClip)
                {
                    return 250;
                }

            }
            else if (targetObjects != null && targetObjects.Length > 1)
            {
                int baseValue = 100;
                int increment = (targetObjects.Length - 1) / 5 * 100;
                return Mathf.Clamp(baseValue + increment, 100, 300);
            }
            return 100;
        }
        internal bool InRecoverScreen()
        {
            if (justOpened && tabs != null && tabs.Count == 1 && IsThereValidSession() && sessionsMode == 0)
            {
                if (EmptySavedSession())
                {
                    return false;
                }
                AdjustTabsToStartMessage();
                return true;
            }
            if (sessionsMode == 0 && lastSessionData != null && lastSessionData.checkingToLoad)
            {
                if (EmptySavedSession())
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        void AdjustTabsToStartMessage()
        {
            if (tabs.Count != 1 || tabs.Count == 1 && !tabs[0].newTab)
            {
                CleanTabs();
            }
        }
        internal static UserSaveData _AutoCreateSettings()
        {
            rootPath = _GetRootPath();
            rootPath += "/Settings";
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            string path = rootPath + "/UserData.asset";
            UserSaveData _settingsData = ScriptableObject.CreateInstance<UserSaveData>();
            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
            }
            AssetDatabase.CreateAsset(_settingsData, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UserSaveData newSettingsData = AssetDatabase.LoadAssetAtPath<UserSaveData>(path);
            if (newSettingsData != null)
            {
                if (MainCoInspector)
                {
                    MainCoInspector.settingsData = newSettingsData;
                }
                return newSettingsData;
            }
            else
            {
                return null;
            }
        }
        internal UserSaveData AutoCreateSettings()
        {
            if (settingsData == null)
            {
                settingsData = FindSettingsObject();
                if (settingsData == null)
                {
                    rootPath = _GetRootPath();
                    if (rootPath != null)
                    {
                        rootPath += "/Settings";
                    }
                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(rootPath);
                    }

                    EditorUtility.DisplayDialog("Missing User Save Data", "CoInspector User Save Data file is missing. CoInspector can't work without it.\n\nCreating a new one at " + rootPath, "OK");

                    string assetPath = rootPath + "/UserData.asset";
                    UserSaveData _settingsData = ScriptableObject.CreateInstance<UserSaveData>();
                    if (File.Exists(assetPath))
                    {
                        AssetDatabase.DeleteAsset(assetPath);
                    }
                    AssetDatabase.CreateAsset(_settingsData, assetPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    settingsData = AssetDatabase.LoadAssetAtPath<UserSaveData>(assetPath);
                    /*
                    if (settingsData != null)
                    {
                        Debug.Log("Created new settings data at " + assetPath);
                    }
                    else
                    {
                        Debug.LogError("Failed to create settings data at " + assetPath);
                    }*/
                }
            }
            return settingsData;
        }
        internal static string _GetRootPath()
        {


            if (Directory.Exists("Assets/CoInspector/Editor"))
            {
                return "Assets/CoInspector/Editor";
            }
            if (Directory.Exists("Assets/Plugins/CoInspector/Editor"))
            {
                return "Assets/Plugins/CoInspector/Editor";
            }
            Assembly assembly = typeof(CoInspectorWindow).Assembly;
            string assemblyPath = assembly.Location;
            assemblyPath = assemblyPath.Replace("\\", "/");
            if (!assemblyPath.Contains("/Assets/"))
            {
                return GetRootScript();
            }
            assemblyPath = assemblyPath.Substring(assemblyPath.LastIndexOf("Assets"));
            assemblyPath = assemblyPath.Substring(0, assemblyPath.LastIndexOf("/"));
            rootPath = assemblyPath;
            return assemblyPath;
        }

        internal static string GetRootScript()
        {
            string _rootPath = "";
            string[] guids = AssetDatabase.FindAssets("CoInspector t:Script");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _rootPath = path.Substring(0, path.LastIndexOf("/"));
                _rootPath = _rootPath.Substring(0, path.LastIndexOf("/Core/"));
                rootPath = _rootPath;
            }
            return _rootPath;
        }

        void GetRootPath()
        {
            if (settingsData)
            {
                rootPath = settingsData.GetRootPath();
            }
            if (rootPath != null && rootPath.Length > 0)
            {
                if (Directory.Exists(rootPath))
                {
                    return;
                }
            }
            rootPath = _GetRootPath();
        }
        internal void SaveSession()
        {
            if (settingsData == null)
            {
                settingsData = AutoCreateSettings();
            }
            if (settingsData && !InRecoverScreen())
            {
                settingsData.SaveData(this);
            }
            else if (settingsData)
            {
                TrySaveLastAssets();
                EditorUtility.SetDirty(settingsData);
                AssetDatabase.SaveAssets();
            }
        }
        private void OnSceneSaving(Scene scene, string path)
        {
            SaveSession();
        }
        void RepopulateTabs(bool skipGo = false)
        {
            if (lastSessionData != null)
            {
                RestoreSession();
            }
            ReinitializeComponentEditors(skipGo);
        }
        void ManageDebugMode(bool _debug)
        {
            if (IsActiveTabValid())
            {
                {
                    GetActiveTab().debug = _debug;
                    ReinitializeComponentEditors(false);
                }
            }
        }
        static void RegisterWindow(CoInspectorWindow window)
        {
            if (instances == null)
            {
                instances = new List<CoInspectorWindow>();
            }
            if (!instances.Contains(window))
            {
                instances.Add(window);
            }
            mainCoInspector = window;
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] == null)
                {
                    instances.RemoveAt(i);
                    i--;
                }
            }

        }
        bool InEmptyWorkspace()
        {
            if (tabs != null || tabs.Count == 1)
            {
                if (tabs[0].newTab)
                {
                    return true;
                }
            }
            return false;
        }
        bool IsThereAPreviousSession()
        {
            if (settingsData)
            {
                lastSessionData = settingsData.LoadData(this);
                if (lastSessionData != null && lastSessionData.tabs != null && lastSessionData.tabs.Count > 0)
                {
                    //   Debug.Log("Loaded a session with " + lastSessionData.tabs.Count + " tabs");
                    return true;
                }
                else
                {
                    //   Debug.Log("No session found");
                    return false;
                }
            }
            return false;
        }

        void SetupTracker()
        {
            if (lastSessionData != null && lastSessionData.tracker != null)
            {
                //   Debug.Log("Setting up tracke with " + lastSessionData.tracker.GetMostClicked().Count + " clicks");
                tracker = new GameObjectTracker(lastSessionData.tracker);
            }
            else
            {
                tracker = new GameObjectTracker();
            }
            ReadLastClicked();
        }

        bool TryLoadSession()
        {
            if (settingsData)
            {
                lastSessionData = settingsData.LoadData(this);
                SetupTracker();
                if (sessionsMode == 2 && InEmptyWorkspace())
                {
                    return false;
                }
                if (lastSessionData != null && lastSessionData.tabs != null && lastSessionData.tabs.Count > 0)
                {
                    // Debug.Log("Loaded a session with " + lastSessionData.tabs.Count + " tabs");
                    return true;
                }
                else
                {
                    // Debug.Log("No session found");
                    return false;
                }
            }
            else
            {
                settingsData = AutoCreateSettings();
            }
            return false;
        }

        void Close(int index)
        {
            if (tabs.Count > 1 && index < tabs.Count)
            {
                TabInfo tab = tabs[index];
                FloatingTab.StartClosingTab(tab);
            }
        }

        internal void CloseTab(TabInfo tab, bool remember = true, bool bulkClosing = false)
        {
            if (tabs.Count > 1 && tab != null)
            {
                int index = tabs.IndexOf(tab);
                if (index < tabs.Count)
                {
                    CloseTab(index, remember, bulkClosing);
                }
            }
        }

        internal void DoCloseTab(TabInfo tab, bool remember = true, bool bulkClosing = false)
        {
            if (tab == null)
            {
                return;
            }
            if (closedTabs == null)
            {
                closedTabs = new List<TabInfo>();
            }
            int index = tabs.IndexOf(tab);
            tab.DestroyAllMaterialMaps();
            if (tab == GetActiveTab())
            {
                EditorToolCache.RestorePreviousPersistentTool();
            }
            tab.index = index;
            if (!tab.newTab && remember)
            {
                closedTabs.Insert(0, tab);
                ManageClosedTabs();
            }
            tabs.Remove(tab);
            if (activeIndex == index && previousTab != null)
            {
                activeIndex = previousTab.index;
            }
            if (activeIndex > 0 && index <= activeIndex)
            {
                activeIndex -= 1;
            }
            if (!bulkClosing)
            {
                if (IsActiveTabValid())
                {
                    FocusTab(activeIndex);
                }
                else
                {
                    FocusTab(0);
                }
            }
            Repaint();
        }

        void CloseTab(int index, bool remember = true, bool bulkClosing = false)
        {
            if (tabs.Count > 1 && index < tabs.Count)
            {
                if (closedTabs == null)
                {
                    closedTabs = new List<TabInfo>();
                }
                TabInfo tab = tabs[index];
                if (tab == null)
                {
                    return;
                }
                if (!bulkClosing/* && index < tabs.Count - 1*/)
                {
                    FloatingTab.StartClosingTab(tab);
                }
                else
                {

                    DoCloseTab(tab, remember, bulkClosing);
                }
            }
        }
        static bool ShowTextPreviews()
        {
            if (showTextAssetPreviews && !textPluginPresent)
            {
                return true;
            }
            return false;
        }
        bool AreThereClosedTabs()
        {
            if (closedTabs != null && closedTabs.Count > 0)
            {
                return true;
            }
            return false;
        }
        internal static void ResetToDefault()
        {
            showHistory = true;
            showTabName = true;
            showTabTree = true;
            showFilterBar = true;
            softSelection = true;
            showIcons = true;
            richNames = true;
            autoFocus = false;
            showScrollBar = true;
            showCollapseTool = true;
            showAdditionalOptions = true;
            showLastClicked = true;
            showMostClicked = true;
            rememberSessions = true;
            useThumbKeys = true;
            ignoreFolders = true;
            collapsePrefabComponents = true;
            openPrefabsInNewTab = true;
            showTextAssetPreviews = true;
            showAssetLabels = false;
            assetInspection = true;
            componentCulling = true;
            sessionsMode = 0;
            tabCompactMode = 1;
            doubleClickMode = 1;
            scrollSpeedX = 2;
            scrollSpeedY = 2;
            scrollDirectionX = 1;
            scrollDirectionY = 1;
            if (MainCoInspector)
            {
                MainCoInspector.UpdateAllWidths();
            }
        }
        void ManageClosedTabs()
        {
            if (closedTabs != null && closedTabs.Count > 10)
            {
                closedTabs[closedTabs.Count - 1].DestroyAllMaterialMaps();
                closedTabs.RemoveAt(closedTabs.Count - 1);
            }
        }

        internal void UpdateAllWidths()
        {
            // Debug.Log("Updating all widths");  
            if (tabs != null && tabs.Count > 0)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabs[i].UpdateTabWidth();
                }
            }
        }
        void RestoreClosedTab()
        {
            if (closedTabs != null && closedTabs.Count > 0)
            {
                TabInfo tab = closedTabs[0];
                if (tab == null || (tab.target == null && !tab.IsValidMultiTarget()))
                {
                    closedTabs.Remove(tab);
                    if (closedTabs.Count > 0)
                    {
                        RestoreClosedTab();
                    }
                    return;
                }
                tab.markForDeletion = false;
                tab.willBeDeleted = false;
                int previousIndex = tab.index;
                UpdatePreviousTab();
                if (previousIndex > tabs.Count)
                {
                    tabs.Add(tab);
                    totalWidth.Add(100);
                    previousIndex = tabs.Count - 1;
                }
                else
                {
                    tabs.Insert(previousIndex, tab);
                    totalWidth.Insert(previousIndex, 100);
                }
                closedTabs.Remove(tab);
                FocusTab(previousIndex);
            }
        }
        internal static bool ContainsMask(Component component)
        {
            if (component == null)
            {
                return false;
            }
            return ContainsMask(component.gameObject);
        }
        internal static bool ContainsMask(GameObject obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetComponent<SpriteMask>())
            {
                return true;
            }
            return false;
        }
        void FocusTab()
        {
            if (tabs.Count > 0 && activeIndex < tabs.Count)
            {
                FocusTab(activeIndex);
            }
        }
        void FocusTab(int index, bool forceAfter = false, bool keepCount = false)
        {
            if (tabs.Count > index)
            {
                GameObject[] gos;
                if (tabs[index].IsValidMultiTarget())
                {
                    gos = tabs[index].targets;
                    targetGameObject = null;
                }
                else
                {
                    targetGameObject = tabs[index].target;
                    gos = new GameObject[] { targetGameObject };
                }
                string[] names = null;
                if (gos != null)
                {
                    gos = gos.Where(t => t != null).ToArray();
                    if (gos.Length > 0)
                    {
                        names = gos.Where(t => t).Select(t => t.name).ToArray();
                    }
                    else
                    {
                        gos = null;
                    }
                }
                if (EditorGUIUtility.editingTextField)
                {
                    EditorGUIUtility.keyboardControl = -1;
                }
                DoFocusTab(index, forceAfter, gos, names, keepCount);
                if (tabs[index].newTab)
                {
                    ReadLastClicked();
                }
            }
        }
        internal void UpdateTabBar(bool @override = false)
        {

            if (Event.current == null && !@override)
            {
                return;
            }
            if (tabs == null || tabs.Count == 0)
            {
                return;
            }
            HandlePendingTabDeletion();
            if (totalWidth == null)
            {
                totalWidth = new List<float>();
            }
            totalWidth.Clear();
            for (int i = 0; i < tabs.Count; i++)
            {
                UpdateTab(tabs[i], i);
            }
        }
        void UpdateTab(TabInfo tab, int index)
        {
            if (tab == null)
            {
                return;
            }
            tab.UpdateTabName();
            tab.index = index;
            if (FloatingTab.isClosing && FloatingTab.linkedTab != null && FloatingTab.linkedTab == tab)
            {
                totalWidth.Add(FloatingTab.GetClosingTabWidth());
            }
            else if (FloatingTab.isOpening && FloatingTab.linkedTab != null && FloatingTab.linkedTab == tab)
            {
                totalWidth.Add(FloatingTab.GetOpeningTabWidth());
            }

            else
            {
                totalWidth.Add(tab.tabWidth);
            }

        }
        void DoFocusTab(int index, bool forceAfter, GameObject[] gos, string[] names, bool keepCount)
        {
            FixActiveIndex();
            GetActiveTab().DestroyAllMaterialMaps();
            if (keepCount)
            {
                bool clickingSame = index == activeIndex;
                if (!clickingSame)
                {
                    UpdateClicked(gos);
                }
            }
            activeIndex = index;
            scrollPosition.y = GetActiveTab().scrollPosition;
            GetActiveTab().zoomFocus = false;
            if (autoFocus && !IsActiveTabNew())
            {
                FocusOnSceneView(gos);
            }
            if (!softSelection && !tabs[index].newTab)
            {
                SelectIfNotAlready(gos, true, forceAfter);
            }
            ReinitializeComponentEditors(false);
            LoadTabFoldoutsIfPresent();
            RefreshAllTabNames();
            RefreshAllIcons();
            if (gos != null && names != null && gos.Length == names.Length)
            {
                EditorApplication.delayCall += () =>
                {
                    for (int i = 0; i < gos.Length; i++)
                    {
                        if (gos[i] != null && i < names.Length)
                        {
                            if (gos[i].name != names[i])
                            {
                                gos[i].name = names[i];
                            }
                        }
                    }
                };
            }
            ScrollToIndex(index);
        }
        bool IsItAMultiPrefabTarget()
        {
            if (targetObject)
            {
                return false;
            }
            if (targetObjects != null && targetObjects.Length > 0)
            {
                if (targetObjects.All(t => t is GameObject))
                {
                    return true;
                }
            }
            return false;
        }
        internal void RefreshAllIcons()
        {
            if (tabs == null)
            {
                return;
            }
            for (int i = 0; i < tabs.Count; i++)
            {
                tabs[i].RefreshIcon();
            }
        }
        void SaveFoldoutsToTab()
        {
            if (GetActiveTab() != null)
            {
                if (componentFoldouts_ != null)
                {
                    GetActiveTab().runtimeFoldouts = new bool[componentFoldouts_.Length];
                    Array.Copy(componentFoldouts_, GetActiveTab().runtimeFoldouts, componentFoldouts_.Length);
                }
            }
        }
        void LoadTabFoldoutsIfPresent()
        {
            if (GetActiveTab() != null)
            {
                if (GetActiveTab().runtimeFoldouts != null && componentEditors != null && componentEditors.Length == GetActiveTab().runtimeFoldouts.Length)
                {
                    componentFoldouts_ = new bool[GetActiveTab().runtimeFoldouts.Length];
                    Array.Copy(GetActiveTab().runtimeFoldouts, componentFoldouts_, componentFoldouts_.Length);
                }
            }
        }


        public void SetTargetGameObject(GameObject go, bool historyMovement = false)
        {
            ReadLastClicked();
            GetActiveTab().runtimeMultiComponents = null;
            bool allCollapsed = GetActiveTab().AreAllCollapsed();
            if (go || IsActiveTabNew())
            {
                if (go != null)
                {
                    GameObject[] gos = new GameObject[] { go };
                    ignoreSelection = gos;
                    if (!softSelection && !IsAlreadySelected(gos))
                    {
                        if (ignoreSelection != null)
                        {
                            Selection.objects = gos;
                        }
                    }
                }
                /*
                bool isInPrefabMode = go != null && IsGameObjectInPrefabMode(go);
                if (isInPrefabMode)
                {
                    if (!onPrefabSceneMode)
                    {
                         Debug.Log("Entering prefab mode");
                        onPrefabSceneMode = true;
                        AddTabNext(go);
                        return;
                    }
                }
                else if (onPrefabSceneMode && !historyMovement)
                {
                       Debug.Log("Exiting prefab mode");
                    onPrefabSceneMode = false;
                    FixActiveIndex(true);
                    RepaintForAWhile();
                    return;
                }*/
                GetActiveTab().multiEditMode = false;
                GetActiveTab().targets = null;
                /*
                if (go != null)
                {
                    GetActiveTab().prefab = isInPrefabMode;
                }*/
                GetActiveTab().AddToHistoryIfProceeds(go);
                UpdateClicked(go);
                targetGameObject = go;
                RefreshAllTabNames();
                UpdateAllTabPaths();
                GetActiveTab().RefreshIcon();
                dragIndex = -1;
                ReinitializeComponentEditors(false);
                GetActiveTab().allCollapsed = allCollapsed;
                FocusTab();
            }
        }
        public void SetTargetGameObjects(GameObject[] gameObjects, bool historyMovement = false)
        {
            ReadLastClicked();
            if (tabs != null && GetActiveTab() != null)
            {
                bool allCollapsed = GetActiveTab().AreAllCollapsed();
                targetGameObject = null;
                gameObjects = gameObjects.Distinct().ToArray();
                GetActiveTab().runtimeMultiComponents = null;
                GetActiveTab().target = null;
                GetActiveTab().newTab = false;
                GetActiveTab().multiEditMode = true;
                ignoreSelection = gameObjects;
                EditorToolCache.RestorePreviousPersistentTool();
                if (!softSelection && !IsAlreadySelected(gameObjects))
                {
                    Selection.objects = gameObjects;
                }
                GetActiveTab().prefab = false;
                foreach (GameObject go in gameObjects)
                    if (go != null)
                    {
                        if (IsGameObjectInPrefabMode(go))
                        {
                            GetActiveTab().prefab = true;
                            break;
                        }
                    }
                GetActiveTab().AddToHistoryIfProceeds(gameObjects);
                UpdateAllTabPaths();
                RefreshAllTabNames();
                if (autoFocus)
                {
                    FocusOnSceneView(gameObjects);
                }
                GetActiveTab().RefreshIcon();
                UpdateClicked(gameObjects);
                dragIndex = -1;
                ReinitializeComponentEditors(false);
                FocusTab();
                GetActiveTab().allCollapsed = allCollapsed;
            }
        }
        internal void ShowButton(Rect position)
        {
            if (lockButtonStyle == null)
            {
                lockButtonStyle = "IN LockButton";
            }
            if (maximizedAssetView)
            {
                EditorGUI.BeginChangeCheck();
                bool lockStatus = GUI.Toggle(position, lockedAsset, GUIContent.none, lockButtonStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    lockedAsset = lockStatus;
                    Repaint();
                }
                GUIContent content = CustomGUIContents.DebugIconOFF;
                if (debugAsset)
                {
                    content = CustomGUIContents.DebugIconON;
                }
                EditorGUI.BeginChangeCheck();
                Rect rect = new Rect(position.x - 18, position.y, 20, 20);
                bool debugStatus = GUI.Toggle(rect, debugAsset, content, CustomGUIStyles.DebugIconStyle);
                GUI.enabled = true;
                if (EditorGUI.EndChangeCheck())
                {
                    debugAsset = debugStatus;
                    SetAllAssetEditorsDebugTo(debugAsset);
                }

                return;
            }
            if (tabs == null || tabs.Count == 0)
            {
                return;
            }
            if (tabs.Count > activeIndex && activeIndex >= 0)
            {
                if (GetActiveTab().newTab)
                {
                    GUI.enabled = false;
                }
                EditorGUI.BeginChangeCheck();
                bool lockStatus = GUI.Toggle(position, GetActiveTab().locked, GUIContent.none, lockButtonStyle);
                GUI.enabled = true;
                if (EditorGUI.EndChangeCheck())
                {
                    if (GetActiveTab().target != null || GetActiveTab().IsValidMultiTarget())
                    {
                        GetActiveTab().locked = lockStatus;
                        UpdateAllWidths();
                    }
                }
                if (GetActiveTab().newTab)
                {
                    GUI.enabled = false;
                }
                GUIContent content = CustomGUIContents.DebugIconOFF;
                if (GetActiveTab().debug)
                {
                    content = CustomGUIContents.DebugIconON;
                }
                EditorGUI.BeginChangeCheck();
                Rect rect = new Rect(position.x - 18, position.y, 20, 20);
                bool debugStatus = GUI.Toggle(rect, GetActiveTab().debug, content, CustomGUIStyles.DebugIconStyle);
                GUI.enabled = true;
                if (EditorGUI.EndChangeCheck())
                {
                    if (GetActiveTab().target != null || GetActiveTab().IsValidMultiTarget())
                    {
                        ManageDebugMode(debugStatus);
                    }
                }
            }
        }
        public void AddItemsToMenu(GenericMenu menu)
        {
            if (GetActiveTab() != null && !IsActiveTabNew())
            {
                menu.AddItem(new GUIContent("Normal"), !GetActiveTab().debug, () =>
                 {
                     GetActiveTab().debug = false;
                     ManageDebugMode(false);
                     Repaint();
                 });
                menu.AddItem(new GUIContent("Debug"), GetActiveTab().debug, () =>
                {
                    GetActiveTab().debug = true;
                    ManageDebugMode(true);
                    Repaint();
                });
                menu.AddSeparator("");
            }
            if (MainCoInspector)
            {
                if (MainCoInspector.IsThereAPreviousSession())
                {
                    menu.AddItem(new GUIContent("Restore Last Saved Session"), false, () =>
                    {
                        ShowRecoverSessionDialogue();
                    });
                }
            }
            menu.AddItem(new GUIContent("★ CoInspector Settings"), false, () =>
            {
                SettingsWindow.ShowWindow();
            });
        }
        internal void ShowSettingsMenu(GenericMenu menu, bool skip = false)
        {
            if (GetActiveTab() != null && !IsActiveTabNew() && !skip)
            {
                menu.AddItem(new GUIContent("Normal"), !GetActiveTab().debug, () =>
                 {
                     GetActiveTab().debug = false;
                     ManageDebugMode(false);
                     Repaint();
                 });
                menu.AddItem(new GUIContent("Debug"), GetActiveTab().debug, () =>
                {
                    GetActiveTab().debug = true;
                    ManageDebugMode(true);
                    Repaint();
                });
                menu.AddSeparator("");
            }
            menu.AddItem(new GUIContent("Inspector Settings/On Tab Click/Select in Hierarchy"), !softSelection, () =>
            {
                softSelection = !softSelection;
                SaveSettings();
                Repaint();
                if (tabs != null && tabs.Count > 0)
                {
                    if (GetActiveTab() != null)
                    {
                        FocusTab(activeIndex);
                    }
                }
            });
            menu.AddItem(new GUIContent("Inspector Settings/On Tab Click/Focus on Scene View"), autoFocus, () =>
           {
               autoFocus = !autoFocus;
               SaveSettings();
               Repaint();
               if (tabs != null && tabs.Count > 0)
               {
                   if (GetActiveTab() != null)
                   {
                       FocusTab(activeIndex);
                   }
               }
           });
            menu.AddDisabledItem(new GUIContent("Inspector Settings/On Tab Double-click/CHOOSE ONE"));
            menu.AddItem(new GUIContent("Inspector Settings/On Tab Double-click/Lock or Unlock Tab"), doubleClickMode == 0, () =>
            {
                doubleClickMode = 0;
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/On Tab Double-click/Select in Hierarchy"), doubleClickMode == 1, () =>
            {
                doubleClickMode = 1;
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/On Tab Double-click/Focus on Scene View"), doubleClickMode == 2, () =>
            {
                doubleClickMode = 2;
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/On Tab Double-click/Show In Local Hierarchy View"), doubleClickMode == 3, () =>
           {
               doubleClickMode = 3;
               SaveSettings();
               Repaint();
           });
            menu.AddItem(new GUIContent("Inspector Settings/On Tab Hover Info/Tree view"), showTabTree && showTabTree, () =>
       {
           showTabName = true;
           showTabTree = true;
           SaveSettings();
           Repaint();
       });
            menu.AddItem(new GUIContent("Inspector Settings/On Tab Hover Info/Name"), showTabName && !showTabTree, () =>
            {
                showTabName = true;
                showTabTree = false;
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/On Tab Hover Info/Nothing"), !showTabName, () =>
           {
               showTabName = false;
               showTabTree = false;
               SaveSettings();
               Repaint();
           });

            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Design/Tab Size/Compact"), tabCompactMode == 1, () =>
            {
                tabCompactMode = 1;
                UpdateAllWidths();
                ScrollToActiveTab();
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Design/Tab Size/Normal"), tabCompactMode == 2, () =>
            {
                tabCompactMode = 2;
                UpdateAllWidths();
                ScrollToActiveTab();
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Design/Show History Tracking buttons"), showHistory, () =>
        {
            showHistory = !showHistory;
            SaveSettings();
            Repaint();
        });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Design/Show Tab icons"), showIcons, () =>
        {
            showIcons = !showIcons;
            UpdateAllWidths();
            SaveSettings();
            Repaint();
        });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Design/Show Scrollbar"), showScrollBar, () =>
           {
               showScrollBar = !showScrollBar;
               SaveSettings();
               Repaint();
           });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Design/Show Collapse button"), showCollapseTool, () =>
                {
                    showCollapseTool = !showCollapseTool;
                    SaveSettings();
                    Repaint();
                });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Design/Show Component Filter bar"), showFilterBar, () =>
            {
                showFilterBar = !showFilterBar;
                if (!showFilterBar)
                {
                    filteringComponents = false;
                }
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Scroll Settings/Vertical Mouse Wheel Speed/Normal"), scrollSpeedY == 2, () =>
         {
             scrollSpeedY = 2;
             SaveSettings();
             Repaint();
         });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Scroll Settings/Vertical Mouse Wheel Speed/Fast"), scrollSpeedY == 3, () =>
            {
                scrollSpeedY = 3;
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Scroll Settings/Vertical Mouse Wheel Speed/Fastest"), scrollSpeedY == 4, () =>
            {
                scrollSpeedY = 4;
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Scroll Settings/Horizontal Mouse Wheel Speed/Normal"), scrollSpeedX == 2, () =>
      {
          scrollSpeedX = 2;
          SaveSettings();
          Repaint();
      });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Scroll Settings/Horizontal Mouse Wheel Speed/Fast"), scrollSpeedX == 3, () =>
           {
               scrollSpeedX = 3;
               SaveSettings();
               Repaint();
           });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Scroll Settings/Horizontal Mouse Wheel Speed/Fastest"), scrollSpeedX == 4, () =>
           {
               scrollSpeedX = 4;
               SaveSettings();
               Repaint();
           });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Scroll Settings/Invert Vertical Mouse Wheel"), scrollDirectionY == -1, () =>
             {
                 if (scrollDirectionY == -1)
                 {
                     scrollDirectionY = 1;
                 }
                 else
                 {
                     scrollDirectionY = -1;
                 }
                 SaveSettings();
                 Repaint();
             });
            menu.AddItem(new GUIContent("Inspector Settings/Tab Bar Scroll Settings/Invert Horizontal Scroll Wheel"), scrollDirectionX == -1, () =>
          {
              if (scrollDirectionX == -1)
              {
                  scrollDirectionX = 1;
              }
              else
              {
                  scrollDirectionX = -1;
              }
              SaveSettings();
              Repaint();
          });
            menu.AddItem(new GUIContent("Inspector Settings/Sessions Behavior/Always Ask"), sessionsMode == 0, () =>
           {
               sessionsMode = 0;
               rememberSessions = true;
               SaveSettings();
               Repaint();
           });
            menu.AddItem(new GUIContent("Inspector Settings/Sessions Behavior/Always Restore"), sessionsMode == 1, () =>
            {
                sessionsMode = 1;
                rememberSessions = true;
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/Sessions Behavior/Disable Sessions"), sessionsMode == 2, () =>
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Your workspace and open tabs will be reset when you switch scenes or restart the editor.", "Yes!", "No"))
                {
                    sessionsMode = 2;
                    rememberSessions = false;
                    SaveSettings();
                    Repaint();
                }
            });
            menu.AddItem(new GUIContent("Inspector Settings/Ignore Folder Inspection"), ignoreFolders, () =>
             {
                 ignoreFolders = !ignoreFolders;
                 SaveSettings();
                 Repaint();
             });
            menu.AddItem(new GUIContent("Inspector Settings/Disable Asset Inspection"), !assetInspection, () =>
            {
                assetInspection = !assetInspection;
                if (!assetInspection)
                {
                    CloseAssetView();
                }
                SaveSettings();
                Repaint();
            });
            menu.AddItem(new GUIContent("Inspector Settings/★ Open the Settings Window"), false, () =>
            {
                SettingsWindow.ShowWindow();
            });
        }
        /* LET'S SEE IF WE MANAGE TO MAKE THIS WORK SOME DAY!!!

            bool IsSameToolActive(Type sentType)
            {
                string typeToSearch = "UnityEditor." + sentType.ToString() + "Tool";
                typeToSearch = typeToSearch.Replace("UnityEngine.", "");
                if (ToolManager.activeToolType != null && ToolManager.activeToolType.ToString() == typeToSearch)
                {
                    return true;
                }
                return false;
            } */

        internal void RefreshAllTabNames()
        {
            if (tabs == null)
            {
                return;
            }
            for (int i = 0; i < tabs.Count; i++)
            {
                if (tabs[i] == null)
                {
                    tabs.RemoveAt(i);
                    i--;
                    continue;
                }
                tabs[i].RefreshName();
                tabs[i].SetPrefabMode();
            }
        }
        internal static bool IsAssetAFolder(string assetPath)
        {
            return AssetDatabase.IsValidFolder(assetPath);
        }
        internal void HideHeaderMargin()
        {
            AutoScrollOnDrag();
            Rect underRect = new Rect(scrollRect);
            if (!toolScrollBarVisible)
            {
                underRect.y += underRect.height - 2;
                underRect.height = 15;
                underRect.x = 0;
                underRect.width = this.position.width;
                //   if (!IsActiveTabNew())
                {
                    EditorUtils.DrawLineOverRect(underRect, CustomColors.DefaultInspector, -0, 5);
                }
                EditorUtils.DrawLineOverRect(underRect);
                if (!EditorUtils.IsLightSkin())
                {
                    EditorUtils.DrawLineOverRect(underRect, CustomColors.HardShadow, 1);
                }
                return;
            }
            underRect.y += underRect.height + 5;
            underRect.height = 20;
            underRect.x = 0;
            underRect.width = this.position.width;
            EditorUtils.DrawLineOverRect(underRect, CustomColors.DefaultInspector, -10, 3);
            EditorUtils.DrawLineOverRect(underRect, -10);
            if (!EditorUtils.IsLightSkin())
            {
                EditorUtils.DrawLineOverRect(underRect, CustomColors.MediumShadow, -9, 1);
            }
            Rect rect = underRect;
            rect.x = rect.x + rect.width - 13;
            rect.y -= 2;
            rect.width = 13;
            rect.height = 15;
            EditorUtils.DrawLineOverRect(underRect, CustomColors.DefaultInspector, 2);
            {
                EditorUtils.DrawLineOverRect(underRect, CustomColors.MediumShadow, 2);
            }
            if (!EditorUtils.IsLightSkin())
            {
                EditorUtils.DrawLineOverRect(underRect, CustomColors.MediumShadow, 8);
            }
            EditorGUI.DrawRect(rect, CustomColors.DefaultInspector);
            rect.x = 0;
            rect.width = 13;
            EditorGUI.DrawRect(rect, CustomColors.DefaultInspector);
        }

        internal void DrawActiveTabUnder()
        {
            if (GetActiveTab() != null)
            {
                Rect rect = activeTabRect;

                rect.y = 0;

                rect = new Rect(rect.x, rect.y + 20, rect.width - 1, 3);
                if (rect.xMax > position.width - 24)
                {
                    rect.width = position.width - 24 - rect.x;
                }
                if (rect.xMin > position.width - 24)
                {
                    rect.width = 0;
                }
                if (showHistory)
                {
                    if (rect.x < 40)
                    {
                        rect.width = rect.width - (40 - rect.x);
                        rect.x = 40;
                    }
                    if (rect.xMax < 40)
                    {
                        rect.width = 0;
                    }
                }
                else if (dragging && !GOdragging && dragIndex == activeIndex)
                {
                    rect.y -= 1;
                }
                Rect fullLine = new Rect(0, rect.y + 3, this.position.width, 1);
                Color editorColor = CustomColors.DefaultInspector;
                if (showScrollBar)
                {
                    if (toolScrollBarVisible)
                    {
                        Rect hugeLine = new Rect(fullLine)
                        {
                            height = 6
                        };
                        EditorGUI.DrawRect(hugeLine, CustomColors.DefaultInspector);
                    }
                }
                EditorGUI.DrawRect(fullLine, editorColor);
                EditorUtils.DrawLineOverRect(fullLine, CustomColors.SimpleShadow, 1);
                EditorUtils.DrawLineOverRect(fullLine, CustomColors.SoftShadow, 1);
                EditorUtils.DrawLineOverRect(fullLine, CustomColors.SimpleBright);
                rect.y += 2;
                rect.height = 2;
                if (dragging && !GOdragging && dragIndex == activeIndex)
                {

                }
                else
                {
                    EditorGUI.DrawRect(rect, editorColor);
                }
            }
        }

        internal void DrawScrollBar()
        {
            bool repaintAfter = false;
            if (!showScrollBar || InRecoverScreen() || tabs == null || tabs.Count == 0)
            {
                if (toolScrollBarVisible)
                {
                    repaintAfter = true;
                }
                LimitScrollBar();
                toolScrollBarVisible = false;
                if (repaintAfter)
                {
                    Repaint();
                }
                return;
            }
            int historyVar = 36;
            int addVar = 24;
            if (!showHistory)
            {
                historyVar = 0;
            }
            float windowWidth = position.width;
            float scrollbarWidth = windowWidth - addVar;
            float contentWidth = GetTotalTabsWidth();
            if (contentWidth <= 0)
            {
                return;
            }
            float viewportWidth = windowWidth - addVar - historyVar;
            //  Debug.Log("Content width: " + contentWidth + " Viewport width: " + viewportWidth);
            if (contentWidth <= viewportWidth)
            {
                LimitScrollBar();
                toolScrollBarVisible = false;
                toolbarScrollPosition.x = 0;
                return;
            }
            if (!toolScrollBarVisible)
            {
                repaintAfter = true;
            }
            toolScrollBarVisible = true;
            GUILayout.Space(1);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            float newScrollPosition = GUILayout.HorizontalScrollbar(
                toolbarScrollPosition.x,
                viewportWidth,
                0f,
                contentWidth,
                GUILayout.Width(scrollbarWidth)
            );
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            Rect underRect = GUILayoutUtility.GetLastRect();
            if (!EditorUtils.IsLightSkin())
            {
                underRect.y -= 1;
                EditorUtils.DrawLineOverRect(underRect, CustomColors.MediumShadow, -2);
            }
            else
            {
                EditorUtils.DrawLineOverRect(underRect, CustomColors.SoftShadow, -2);
            }
            toolbarScrollPosition.x = newScrollPosition;
            LimitScrollBar();
            if (repaintAfter)
            {
                Repaint();
            }
        }

        internal void DrawComponentFilterField(float xPosition = 0, float yPosition = 0, bool multi = false)
        {
            if (!showFilterBar)
            {
                return;
            }
            float MiddlePoint = position.width / 2 - 10;
            float searchWidth = 70;
            float diff = 0;
            if (MiddlePoint < 200)
            {
                diff = 200 - MiddlePoint;
                searchWidth -= diff;
                xPosition += diff;

            }
            /* if (!filteringComponents)
             {
                 searchWidth = 18;
             }*/
            float xPositionSearch = xPosition + 8;
            if (multi)
            {
                xPositionSearch += 15;
            }
            GUIContent filterButton = CustomGUIContents.FilterOFFContent;
            if (filteringComponents)
            {
                filterButton = CustomGUIContents.FilterONContent;
            }
            if (GUI.Button(new Rect(xPositionSearch + searchWidth, yPosition + 1, 12, 20), filterButton, CustomGUIStyles.FilterButtonStyle))
            {
                filteringComponents = !filteringComponents;
                if (!filteringComponents)
                {
                    GUI.FocusControl(null);
                }
                Repaint();
            }
            GUI.enabled = filteringComponents;
            bool isSearchFocused = GUI.GetNameOfFocusedControl() == "SearchField";
            GUI.SetNextControlName("SearchField");
            if (diff < 54)
            {
                GetActiveTab().filterString = EditorGUI.TextField(new Rect(xPositionSearch, yPosition + 4, searchWidth, 16), GetActiveTab().filterString, CustomGUIStyles.FilterStyle);

                GUI.Label(new Rect(xPositionSearch + 3, yPosition + 4, 15, 15), CustomGUIContents.SearchButtonImage);
            }
            if (diff < 29 && GetActiveTab().filterString == "" && !isSearchFocused)
            {
                GUI.Label(new Rect(xPositionSearch + 13, yPosition + 3, searchWidth, 15), "<color=grey>Type</color>", CustomGUIStyles.FilterComponentLabel);
            }

            if (Event.current.type == EventType.MouseDown && !new Rect(xPositionSearch, yPosition + 4, searchWidth, 15).Contains(Event.current.mousePosition) && isSearchFocused)
            {
                GUI.FocusControl(null);
                Repaint();
            }
            GUI.enabled = true;

        }
        internal void DrawUnderButtons()
        {
            GUILayout.BeginHorizontal(CustomGUIStyles.ButtonsUpSection, GUILayout.Height(20));
            EditorGUILayout.Space(1);
            GUILayout.EndHorizontal();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            EditorUtils.DrawLineUnderRect(lastRect, CustomColors.DefaultInspector, -1);
            EditorUtils.DrawLineOverRect(lastRect, CustomColors.DefaultInspector, 8, 15);
            GUIStyle toolbarButtonStyle = CustomGUIStyles.ButtonsUpRight;
            Rect inspectRect;
            Rect hierarchyRect;
            Rect selectRect;
            Rect focusRect;
            bool popup = Event.current.alt || Event.current.shift;
            float yPosition = lastRect.y - 11;
            if (GetActiveTab().IsAPrefabTab())
            {
                yPosition += 2;
            }

            float buttonWidth = 20;
            float buttonHeight = 20;
            float xPosition = this.position.width - buttonWidth - 5;
            int toAdd = 0;
            if (popup)
            {
                toAdd += 12;
            }
            hierarchyRect = new Rect(xPosition, yPosition, buttonWidth, buttonHeight);
            inspectRect = new Rect(xPosition - buttonWidth - toAdd, yPosition, buttonWidth + toAdd, buttonHeight);
            focusRect = new Rect(xPosition - buttonWidth * 2 - toAdd, yPosition, buttonWidth, buttonHeight);
            selectRect = new Rect(xPosition - buttonWidth * 3 - toAdd, yPosition, buttonWidth, buttonHeight);
            float totalWidth = hierarchyRect.width + inspectRect.width + focusRect.width + selectRect.width;
            DrawComponentFilterField(position.width - totalWidth - 100, yPosition - 1);
            GUI.enabled = true;
            GUIContent focusContent = CustomGUIContents.FocusContent;
            if (GUI.Button(focusRect, focusContent, toolbarButtonStyle))
            {
                ScrollToActiveTab();
                if (Event.current.button == 2)
                {
                    return;
                }
                if (IsActiveTabValidMulti())
                {
                    if (Event.current.button == 0)
                    {
                        FocusOnSceneView(GetActiveTab().targets);
                    }
                    else
                    {
                        EditorGUIUtility.PingObject(GetActiveTab().targets[0]);
                    }
                }
                else
                {
                    if (Event.current.button == 0)
                    {
                        FocusOnSceneView(GetActiveTab().target);
                    }
                    else
                    {
                        EditorGUIUtility.PingObject(GetActiveTab().target);
                    }
                }
            }
            if (!IsActiveTabValidMulti() && IsAlreadySelected(new GameObject[1] { GetActiveTab().target }))
            {
                GUI.enabled = false;
            }
            else if (IsActiveTabValidMulti() && IsAlreadySelected(GetActiveTab().targets))
            {
                GUI.enabled = false;
            }

            if (GUI.Button(selectRect, CustomGUIContents.SelectContent, toolbarButtonStyle))
            {
                ScrollToActiveTab();
                if (IsActiveTabValidMulti())
                {
                    ignoreSelection = GetActiveTab().targets;
                    Selection.objects = GetActiveTab().targets;
                }
                else
                {
                    ignoreSelection = new GameObject[] { GetActiveTab().target };
                    Selection.objects = ignoreSelection;
                }
                if (Event.current.button != 0)
                {
                    FocusOnSceneView(Selection.gameObjects);
                }
            }
            GUI.enabled = true;
            GUIContent editContent = CustomGUIContents.EditContentDefault;
            if (popup)
            {
                toolbarButtonStyle = CustomGUIStyles.ButtonsUpRight_Wide;
                editContent = CustomGUIContents.EditContentPopup;
            }
            if (GUI.Button(inspectRect, editContent, toolbarButtonStyle))
            {
                if (IsActiveTabValidMulti())
                {
                    PopUpInspectorWindow(GetActiveTab().targets, popup);
                }
                else
                {
                    PopUpInspectorWindow(new GameObject[] { GetActiveTab().target }, popup);
                }
            }
            toolbarButtonStyle = CustomGUIStyles.ButtonsUpRight;
            GUI.enabled = !GetActiveTab().newTab && !GetActiveTab().IsValidMultiTarget();
            if (GUI.Button(hierarchyRect, CustomGUIContents.HierarchyContent, toolbarButtonStyle))
            {
                HierarchyPopup.ShowWindow(GetActiveTab().target, this, clickMousePosition);
            }
            GUI.enabled = true;
            if (showCollapseTool)
            {
                float middleX = this.position.width / 2 - 24 / 2;
                Rect middleRect = new Rect(middleX, yPosition + 4, 35, 12);
                Color color = GUI.color;
                bool allCollapsed = GetActiveTab().AreAllCollapsed();
                if (allCollapsed)
                {
                    if (EditorUtils.IsLightSkin())
                    {
                        GUI.color += CustomColors.AllCollapsed * 2;
                    }
                    else
                    {
                        GUI.color += CustomColors.AllCollapsed;
                    }
                }
                else
                {
                    if (!EditorUtils.IsLightSkin())
                    {

                        GUI.color -= CustomColors.NotAllCollapsed;
                    }
                }
                if (GUI.Button(middleRect, CustomGUIContents.GetExpandCollapseContent(allCollapsed), CustomGUIStyles.ExpandButtonStyle))
                {
                    if (Event.current.button == 0 && !allCollapsed)
                    {
                        SetAllComponentsTo(false);
                    }
                    else if (Event.current.button == 1 || allCollapsed)
                    {
                        SetAllComponentsTo(true);
                    }
                }
                GUI.color = color;
                middleRect.x += 1;
                middleRect.width -= 2;
                middleRect.height = 10;
                middleRect.y += 1;
                EditorUtils.DrawLineOverRect(middleRect, CustomColors.HarderBright);
            }
        }

        internal void HandlePendingTabDeletion()
        {

            if (tabs == null || tabs.Count == 0)
            {
                return;
            }
            if ((FloatingTab.isClosing || FloatingTab.isOpening) && (FloatingTab.linkedTab == null))
            {
                FloatingTab.isClosing = false;
                FloatingTab.isOpening = false;
            }
            if (FloatingTab.isClosing && FloatingTab.linkedTab != null && FloatingTab.linkedTab.markForDeletion && Event.current.type == EventType.Layout)
            {
                FloatingTab.isClosing = false;
                DoCloseTab(FloatingTab.linkedTab);
            }

        }

        internal void DrawHeader()
        {
            if (!GetActiveTab().multiEditMode)
            {
                if (gameObjectEditor != null && GetActiveTab().prefab && !IsGameObjectInPrefabMode(GetActiveTab().target) && !onPrefabSceneMode)
                {
                    DestroyImmediate(gameObjectEditor);
                    //GetActiveTab().ResetTab();
                }
                if (gameObjectEditor != null && gameObjectEditor.target == null)
                {
                    DestroyImmediate(gameObjectEditor);
                }
                if (gameObjectEditor != null && gameObjectEditor.target != null && gameObjectEditor.target != GetActiveTab().target)
                {
                    DestroyImmediate(gameObjectEditor);
                }
                if (gameObjectEditor == null && GetActiveTab().target != null)
                {
                    gameObjectEditor = Editor.CreateEditor(GetActiveTab().target);
                }
                if (gameObjectEditor != null && gameObjectEditor.target && GetActiveTab().target)
                {
                    gameObjectEditor.DrawHeader();
                }
            }
            else
            {
                if (gameObjectEditor != null && gameObjectEditor.targets != null && gameObjectEditor.targets != GetActiveTab().targets)
                {
                    DestroyImmediate(gameObjectEditor);
                }
                if (gameObjectEditor != null && gameObjectEditor.targets == null)
                {
                    DestroyImmediate(gameObjectEditor);
                }
                if (gameObjectEditor == null && GetActiveTab().targets != null && !IsAnyAPrefabAsset(GetActiveTab().targets))
                {
                    gameObjectEditor = Editor.CreateEditor(GetActiveTab().targets);
                }
                if (gameObjectEditor != null && GetActiveTab().targets != null)
                {
                    gameObjectEditor.DrawHeader();
                }
            }
            DrawUnderButtons();
        }

        void HandleTabDoubleClick(TabInfo tab)
        {
            if (doubleClickMode == 0)
            {
                tab.locked = !tab.locked;
                UpdateAllWidths();
            }
            else if (doubleClickMode == 1)
            {
                if (tab.IsValidMultiTarget())
                {
                    Selection.objects = tab.targets;
                }
                else
                {
                    Selection.activeGameObject = tab.target;
                }
            }
            else if (doubleClickMode == 2)
            {
                if (tab.IsValidMultiTarget())
                {
                    FocusOnSceneView(tab.targets);
                }
                else
                {
                    FocusOnSceneView(tab.target);
                }
            }
            else if (doubleClickMode == 3 && tab.target && !tab.newTab && !tab.IsValidMultiTarget())
            {
                HierarchyPopup.ShowWindow(tab.target, this, clickMousePosition);
            }
        }

        internal void DrawMultiInspector()
        {
            if (CanSkipRefresh())
            {
                return;
            }
            if (componentEditors == null)
            {
                return;
            }
            TabInfo activeTab = GetActiveTab();
            if (activeTab == null || activeTab.targets == null || activeTab.targets.Length == 0)
            {
                return;
            }
            Component[] components = activeTab.targets[0].GetComponents<Component>();
            bool hasNullComponent = components.Any(c => c == null);
            activeTab.CleanMap(components);
            if (activeTab.runtimeMultiComponents != null && activeTab.runtimeMultiComponents.Length > 0)
            {
                if (components != null)
                {
                    if (components.Length != activeTab.runtimeMultiComponents.Length)
                    {
                        ReinitializeComponentEditors(false);
                        Repaint();
                    }
                    else
                    {
                        for (int i = 0; i < components.Length; i++)
                        {
                            if (components[i] == null)
                            {
                                continue;
                            }
                            if (components[i].GetType() != activeTab.runtimeMultiComponents[i].GetType())
                            {
                                ReinitializeComponentEditors(false);
                                Repaint();
                                break;
                            }
                        }
                    }
                }
            }
            activeTab.runtimeMultiComponents = new Component[components.Length];
            Array.Copy(components, activeTab.runtimeMultiComponents, components.Length);
            if (hasNullComponent)
            {
                ShowMissingComponent(true);
            }
            for (int i = 0; i < componentEditors.Length; i++)
            {
                if (!IsComponentFilteredInTab(componentEditors[i].target as Component, componentEditors[i]) && AreWeFilteringComponents())
                {
                    continue;
                }
                if (componentEditors[i] == null)
                {
                    ReinitializeComponentEditors(false);
                    break;
                }
                bool hasDestroyedTarget = false;
                foreach (var target in componentEditors[i].targets)
                {
                    if (!target)
                    {
                        hasDestroyedTarget = true;
                        break;
                    }
                }
                if (hasDestroyedTarget)
                {
                    ReinitializeComponentEditors(false);
                    Repaint();
                    break;
                }



                if (componentFoldouts_ == null || componentFoldouts_.Length != componentEditors.Length)
                {
                    componentFoldouts_ = new bool[componentEditors.Length];
                    for (int u = 0; u < componentFoldouts_.Length; u++)
                    {
                        componentFoldouts_[u] = true;
                    }
                }
                ComponentMap componentMap = GetActiveTab().GetFoldoutMapForComponent(componentEditors[i].targets[0] as Component, componentEditors[i]);
                componentMap.index = i;

                bool componentHidden = !EditorUtils.HasVisibleFields(componentEditors[i]);
                Behaviour behaviour = componentEditors[i].targets[0] as Behaviour;
                bool isBehaviour = behaviour != null;
                bool wasEnabled = isBehaviour && behaviour.enabled;
                if (isBehaviour)
                {
                    EditorGUI.BeginChangeCheck();
                }
                EditorGUILayout.BeginVertical();
                bool flag = EditorGUILayout.InspectorTitlebar(componentMap.foldout,
                componentEditors[i].targets, !componentHidden);
                if (isBehaviour)
                {
                    if (DrawMultiComponentEnabledToggle(componentEditors[i], wasEnabled))
                    {
                        flag = componentMap.foldout;
                    }
                }
                if (componentHidden)
                {
                    flag = true;
                }
                bool isLastComponent = i == componentEditors.Length - 1;
                bool changed = flag != componentFoldouts_[i];
                if (changed)
                {
                    componentFoldouts_[i] = flag;
                    GetActiveTab().SaveFoldoutToMap(componentEditors[i].targets[0] as Component, flag, componentEditors[i]);
                }
                EditorUtils.DrawLineOverRect(GUILayoutUtility.GetLastRect(), -1);
                EditorUtils.DrawLineUnderRect(GUILayoutUtility.GetLastRect(), CustomColors.SoftShadow, -1, 2);
                if (componentFoldouts_[i] && !componentHidden)
                {
                    if (!IsComponentCulled(componentMap))
                    {
                        EditorGUILayout.BeginVertical(CustomGUIStyles.CompStyle);
                        EditorGUI.BeginChangeCheck();
                        if (GetActiveTab().debug || HasUnityEvent(componentEditors[i]))
                        {
                            componentEditors[i].DrawDefaultInspector();
                        }
                        else
                        {
                            componentEditors[i].OnInspectorGUI();
                        }
                        if (EditorGUI.EndChangeCheck() && EditorUtils.IsMaterialComponent(componentEditors[i].targets[0] as Component))
                        {
                            GetActiveTab().MarkMaterialsForRebuild();
                            Repaint();
                        }
                        EditorGUILayout.EndVertical();
                        MaterialMap materialMap = GetActiveTab().GetMaterialMapForComponent(componentEditors[i].targets[0] as Component, componentEditors[i]);
                        if (materialMap.materials.Count > 0)
                        {
                            EditorUtils.DrawMaterials(materialMap, componentEditors[i].targets[0] as Component);
                        }
                        /*
                        else
                        {
                            GetActiveTab().DestroyIfPresent(componentEditors[i].targets[0] as Component);
                        }*/
                        GUILayout.Space(2);
                    }
                }
                EditorGUILayout.EndVertical();
                UpdateComponentHeight(componentMap);
                AddIfNecessary(componentEditors[i]);

            }
            GetActiveTab().RebuildMaterialsIfNecessary();
            EditorUtils.MultiEditFooter(differentComponents);

        }

        void AutoScrollOnDrag()
        {
            int extendedX = 40;
            if (!showHistory)
            {
                extendedX = 0;
            }
            if (!dragging && DragAndDrop.objectReferences.Length > 0)
            {
                Rect leftRect = new Rect(0, 0, 30 + extendedX, 42);
                Rect rightRect = new Rect(position.width - 50, 0, 50, 42);
                if (leftRect.Contains(Event.current.mousePosition))
                {
                    if (toolbarScrollPosition.x > 0)
                    {
                        toolbarScrollPosition.x -= 2;
                        LimitScrollBar();
                    }
                }
                else if (rightRect.Contains(Event.current.mousePosition))
                {
                    toolbarScrollPosition.x += 2;
                    LimitScrollBar();
                }
            }
        }

        private void DrawUnderMultiObjectHeader(Rect headerRect)
        {
            if (EditorUtils.IsLightSkin())
            {
                EditorUtils.DrawLineUnderRect(headerRect);
            }
            else
            {
                EditorUtils.DrawLineUnderRect(headerRect, CustomColors.HardShadow);
            }
            int selectedObjectCount = GetActiveTab().targets.Length;
            Rect foldoutRect = new Rect(headerRect.x + 6, headerRect.y + headerRect.height - 17, 15, 20);
            GUIContent countContent = CustomGUIContents.SelectionContent(selectedObjectCount);
            GUIStyle italicStyle = CustomGUIStyles.ItalicStyle;
            Rect _labelRect = new Rect(foldoutRect.xMax, foldoutRect.y - 3, italicStyle.CalcSize(countContent).x, foldoutRect.height);
            GUI.Label(_labelRect, countContent, italicStyle);
            foldoutRect.width += _labelRect.width + 30;
            GUIContent foldoutContent = CustomGUIContents.FoldedFoldout;
            if (GetActiveTab().multiFoldout)
            {
                foldoutContent = CustomGUIContents.UnfoldedFoldout;
            }
            if (GUI.Button(foldoutRect, foldoutContent, GUIStyle.none))
            {
                GetActiveTab().multiFoldout = !GetActiveTab().multiFoldout;
            }
            if (GetActiveTab().multiFoldout)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                GUIStyle labelStyle = CustomGUIStyles.MultiFoldoutStyle;
                float width = CustomGUIStyles.RichMiniLabel.CalcSize(CustomGUIContents.MultiSelectingContent).x + 10;
                if (EditorUtils.IsLightSkin())
                {
                    GUILayout.Label("Selecting:", CustomGUIStyles.RichMiniLabel, GUILayout.Width(width));
                }
                else
                {
                    GUILayout.Label("<b>Selecting:</b>", CustomGUIStyles.RichMiniLabel, GUILayout.Width(width));
                }
                float currentWidth = width;
                float maxWidth = position.width - 35;
                for (int i = 0; i < selectedObjectCount; i++)
                {
                    GameObject go = GetActiveTab().targets[i];
                    GUIContent goContent = CustomGUIContents.MultiSelectionContent(GetActiveTab().targets, i);
                    Vector2 size = labelStyle.CalcSize(goContent);
                    if (currentWidth + size.x > maxWidth)
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        currentWidth = 0;
                    }
                    Rect labelRect = GUILayoutUtility.GetRect(goContent, labelStyle, GUILayout.Width(size.x));
                    EditorGUIUtility.AddCursorRect(labelRect, MouseCursor.Link);
                    if (GUI.Button(labelRect, goContent, labelStyle))
                    {
                        if (Event.current.button == 0)
                        {
                            EditorApplication.delayCall += () =>
                            {
                                SetTargetGameObject(go);
                            };
                        }
                        if (Event.current.button == 2)
                        {
                            EditorApplication.delayCall += () =>
                            {
                                AddTabNext(go);
                            };
                        }
                        else if (Event.current.button == 1)
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Open in new Tab"), false, () =>
                            {
                                AddTabNext(go);
                                Repaint();
                            });
                            menu.AddItem(new GUIContent("Select"), false, () =>
                            {
                                Selection.activeGameObject = go;
                            });
                            menu.AddSeparator("");
                            menu.AddItem(new GUIContent("Ping"), false, () =>
                            {
                                EditorGUIUtility.PingObject(go);
                            });
                            menu.AddItem(new GUIContent("Focus on Scene View"), false, () =>
                            {
                                FocusOnSceneView(go);
                            });
                            menu.AddItem(new GUIContent("Show In Local Hierarchy View"), false, () =>
                            {
                                HierarchyPopup.ShowWindow(go, this, clickMousePosition);
                            });
                            menu.AddSeparator("");
                            menu.AddItem(new GUIContent("Remove from selection"), false, () =>
                            {
                                List<GameObject> newTargets = GetActiveTab().targets.ToList();
                                newTargets.Remove(go);
                                FocusTab(activeIndex);
                                bool wasLocked = GetActiveTab().locked;
                                GetActiveTab().locked = false;
                                EditorApplication.delayCall += () =>
                                {
                                    if (newTargets.Count == 1)
                                    {
                                        SetTargetGameObject(newTargets[0]);
                                    }
                                    else
                                    {
                                        SetTargetGameObjects(newTargets.ToArray());
                                    }
                                    GetActiveTab().locked = wasLocked;
                                };
                            });
                            menu.ShowAsContext();
                        }
                    }
                    currentWidth += size.x;
                    if (i < selectedObjectCount - 1)
                    {
                        GUIContent plusContent = CustomGUIContents.PlusSymbolContent;
                        Vector2 plusSize = CustomGUIContents.plusSize;
                        Rect plusRect = GUILayoutUtility.GetRect(plusContent, CustomGUIStyles.PlusStyle, GUILayout.Width(plusSize.x));
                        GUI.Label(plusRect, plusContent, CustomGUIStyles.PlusStyle);
                        currentWidth += plusSize.x;
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
        }

        void RepositionDraggedTab(Rect buttonRect, int i)
        {
            if (dragging && i != dragIndex && !GOdragging)
            {
                Rect fixedRect = new Rect
                {
                    width = totalWidth[dragIndex],
                    x = Event.current.mousePosition.x - FloatingTab.tabDragPoint
                };
                if (FloatingTab.tabRect != null && !EditorUtils.AreRectsOverlapping(buttonRect, fixedRect))
                {
                    return;
                }
                PopUpTip.Hide();
                float correction = 0;
                float tabLeft = fixedRect.x;
                float tabRight = fixedRect.xMax;
                float tabCenter = fixedRect.center.x;
                float targetLeft = buttonRect.x;
                float targetRight = buttonRect.xMax;
                float targetCenter = buttonRect.center.x;
                if (tabRight > targetCenter + correction && tabRight >= targetLeft && tabRight < targetRight && tabCenter < targetCenter)
                {
                    dragTargetIndex = i + 1;
                }
                else if (tabLeft < targetCenter - correction && tabLeft <= targetRight && tabLeft > targetLeft && tabCenter > targetCenter)
                {
                    dragTargetIndex = i;
                }
            }
        }

        void DragTabLogic(Rect fixedButtonRect, Rect buttonRect, int i, int click, bool ignoreHover)
        {
            if (!dragging && i != dragIndex && tabs.Count > 1)
            {
                dragIndex = i;
                if (PopUpTip.waitingToOpen)
                {
                    PopUpTip.Hide();
                }
                dragRect = fixedButtonRect;
                dragTargetIndex = i;
            }
            if (dragging && i != dragIndex && !GOdragging)
            {

            }
            else if (dragging && i == dragIndex && !GOdragging)
            {
                dragTargetIndex = -1;
            }
            else if (!dragging && !FloatingTab.isClosing)
            {
                Rect _lastRect = buttonRect;
                Rect rect1 = new Rect(_lastRect.x, _lastRect.y + 25, 100, 30);
                if (showHistory)
                {
                    rect1.x += 44;
                }
                string text = tabs[i].name;
                if (click == activeIndex)
                {
                    text = "";
                }
                if (!tabs[click].newTab && DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0 && activeIndex != click)
                {
                    if (pendingTabSwitch == -1 || pendingTabSwitch != click)
                    {
                        lastTabClick = Time.realtimeSinceStartup;
                        pendingTabSwitch = click;
                        RepaintFor(0.4f);
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                    }
                }
                else if (pendingTabSwitch != -1)
                {
                    pendingTabSwitch = -1;
                }
                else
                {
                    if (ignoreHover)
                    {
                        PopUpTip.Hide();
                    }
                    else if (showTabTree && !tabs[click].newTab && !tabs[click].multiEditMode)
                    {
                        text = CustomGUIContents.GetSingleHoverName(tabs[click].target);
                    }
                    else if (tabs[click].multiEditMode)
                    {
                        text = CustomGUIContents.GetMultiHoverName(tabs[click].targets);
                    }
                }
                rect1.x -= toolbarScrollPosition.x;
                if (tabs[click].multiEditMode)
                {
                    PopUpTip.ShowMulti(text, rect1, this);
                }
                else
                {
                    PopUpTip.Show(text, rect1, this);
                }
            }
        }


        private void DrawAddComponentBar(Color backgroundColor)
        {
            EditorGUILayout.BeginVertical(CustomGUIStyles.InspectorSectionStyle, GUILayout.Height(36));
            GUILayout.Space(5);
            GUI.backgroundColor = backgroundColor;
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Add Component", GUILayout.Height(25), GUILayout.Width(180)))
            {
                Vector2 s = addComponentVector;
                Vector2 mousePos = Event.current.mousePosition;
                float distanceFromBottom = position.height - GUIUtility.GUIToScreenPoint(mousePos).y;
                Vector2 p = new Vector2(mousePos.x - s.x / 2, mousePos.y - s.y);

                Rect rect = new Rect(p, s);
                if (distanceFromBottom < rect.height + 25)
                {
                    float diff = rect.height - distanceFromBottom + 25;
                    rect.y -= diff;
                }
                UnityEngine.GameObject[] objects = GetActiveTab().multiEditMode ? GetActiveTab().targets : new GameObject[] { GetActiveTab().target };
                Reflected.ShowAddComponentWindow(rect, objects);
            }
            Rect rect1 = GUILayoutUtility.GetLastRect();
            if (UpdateChecker.IsUpdateAvailable && position.width > 322)
            {
                Rect buttonRect = new Rect(5, rect1.y + 2, 54, 21);
                if (GUI.Button(buttonRect, CustomGUIContents.UpdateContent, GUIStyle.none))
                {
                    UnityEditor.PackageManager.UI.Window.Open("");
                }
                GUI.Label(new Rect(buttonRect.x + 20, buttonRect.y - 2, 54, 21), UpdateChecker.LatestVersion, CustomGUIStyles.MiniLabel);
            }
            Rect rect2 = new Rect(rect1.x + 1, rect1.y, rect1.width - 2, rect1.height);
            EditorUtils.DrawLineOverRect(rect2, CustomColors.HarderBright, -1);
            EditorUtils.DrawLineOverRect(rect2, CustomColors.SubtleBright, -1, 4);
            EditorUtils.DrawLineOverRect(rect2, CustomColors.SubtleBright, -1, 8);
            EditorUtils.DrawRectBorder(rect1, CustomColors.SimpleShadow, 1);
            DrawAssetHistoryButton();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            Rect addRect = EditorUtils.GetLastLineRect();
            EditorUtils.DrawLineOverRect(addRect, CustomColors.HardShadow, 0);
            EditorUtils.DrawLineOverRect(addRect, CustomColors.HarderBright, -1);
            EditorUtils.DrawLineUnderRect(addRect, CustomColors.SimpleShadow, -1);
        }
        private void DrawRecoverScreen()
        {
            GUILayout.Space(10);
            GUIStyle labelStyle = CustomGUIStyles.CenterLabel;
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.x += 20;
            rect.width = position.width - 40;
            rect.y += 5;
            int numberTabs = lastSessionData.tabs.Count;
            string tabsText = " Tabs";
            if (numberTabs == 1)
            {
                tabsText = " Tab";
            }
            GUIContent welcomeMessage = CustomGUIContents.WelcomeMessage(numberTabs + tabsText);
            rect.height = labelStyle.CalcHeight(welcomeMessage, rect.width) + 15;
            EditorGUILayout.BeginHorizontal();
            GUIContent content = welcomeMessage;
            string tabs = "";
            if (lastSessionData.tabs != null && lastSessionData.tabs.Count > 0)
            {
                if (lastSessionData.GetSaveTime() != default)
                {
                    tabs += "   " + lastSessionData.lastSaveTimePrint + "   \nTabs:\n";
                }
                for (int i = 0; i < lastSessionData.tabs.Count; i++)
                {
                    TabInfo tab = lastSessionData.tabs[i];
                    if (i != 0)
                    {
                        tabs += "\n";
                    }
                    if (tab.newTab)
                    {
                        tabs += "   " + (i + 1) + ".  " + "*New Tab" + "   ";
                    }
                    else
                    {
                        tabs += "   " + (i + 1) + ".  " + tab.name + "   ";
                    }
                }
            }
            content.tooltip = tabs;
            CustomGUIStyles.HelpBox(content, true);
            EditorGUILayout.EndHorizontal();
            Rect rect1 = GUILayoutUtility.GetLastRect();
            if (rect1.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(rect1, MouseCursor.Link);
            }
            GUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Color color = GUI.backgroundColor;
            GUI.backgroundColor += CustomColors.WelcomeBackColor;
            if (EditorUtils.IsLightSkin())
            {
                GUI.backgroundColor = Color.blue / 3;
            }
            if (GUILayout.Button("Restore!", GUILayout.Width(90), GUILayout.Height(25)))
            {
                sessionsMode = 0;
                justOpened = false;
                RestoreSession();
                ScrollToIndex(activeIndex);
            }
            CustomGUIContents.DrawCustomButton(true);
            GUI.backgroundColor = color;
            if (GUILayout.Button("Don't restore", GUILayout.Width(100), GUILayout.Height(25)))
            {
                lastSessionData.checkingToLoad = false;
                sessionsMode = 0;
                lastSessionData = null;
            }
            CustomGUIContents.DrawCustomButton(true);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUIStyle foldoutStyle = EditorStyles.foldout;
            GUILayout.Space(35);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(25);
            showAdditionalOptions = EditorGUILayout.Foldout(showAdditionalOptions, " Additional Options", true, foldoutStyle);
            EditorGUILayout.EndHorizontal();
            Rect rect2 = EditorUtils.GetLastLineRect();
            if (!showAdditionalOptions)
            {
                EditorUtils.DrawLineUnderRect(rect2, 6);
                EditorUtils.DrawLineUnderRect(rect2, CustomColors.HarderBright, 7);
            }
            else
            {
                EditorUtils.DrawLineUnderRect(rect2, CustomColors.HardShadow, 6);
                EditorUtils.DrawLineUnderRect(rect2, CustomColors.SoftShadow, 7);
                EditorUtils.DrawLineUnderRect(rect2, CustomColors.VerySoftShadow, 8, 3);
                EditorUtils.DrawLineUnderRect(rect2, CustomColors.VerySoftShadow, 8, 89);
                GUILayout.Space(25);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Always restore", GUILayout.Height(22), GUILayout.Width(100)))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?", "CoInspector will automatically continue your previous Sessions without asking.", "Yes!", "Oops, that sounds annoying"))
                    {
                        sessionsMode = 1;
                        SaveSettings();
                        RestoreSession();
                        ScrollToIndex(activeIndex);
                        justOpened = false;
                    }
                }
                CustomGUIContents.DrawCustomButton(true);
                GUI.backgroundColor += CustomColors.WelcomeSubBackColor;
                if (EditorUtils.IsLightSkin())
                {
                    GUI.backgroundColor -= CustomColors.LightSkinRed;
                }
                if (GUILayout.Button("Don't ask again", GUILayout.Height(22), GUILayout.Width(105)))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?", "This will completely disable the Sessions feature until enabled again.", "Yeah, shut up already", "No"))
                    {
                        sessionsMode = 2;
                        lastSessionData.checkingToLoad = false;
                        rememberSessions = false;
                        lastSessionData = null;
                        SaveSettings();
                    }
                }
                CustomGUIContents.DrawCustomButton(true);
                GUI.backgroundColor = color;
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
                EditorGUILayout.LabelField("<b>*</b><i>You can always change it in the Settings!</i>", CustomGUIStyles.CenterLabel);
                rect2 = EditorUtils.GetLastLineRect();
                EditorUtils.DrawLineUnderRect(rect2, 6);
                EditorUtils.DrawLineUnderRect(rect2, CustomColors.HarderBright, 7);
            }
            GUILayout.Space(10);
        }

        private void DrawLastClickedSection()
        {
            if (lastClicked != null && lastClicked.Count > 0)
            {
                float width = position.width - 55;
                if (lastClicked.Count < 5)
                {
                    width = width / lastClicked.Count;
                }
                else
                {
                    width = width / 5;
                }
                GUILayout.BeginHorizontal(GUILayout.Height(10));
                GUILayout.Space(17);

                bool flag = EditorGUILayout.Foldout(showLastClicked, " Last Clicked", true, CustomGUIStyles.BoldFoldoutStyle);
                if (!hoveringResize)
                {
                    showLastClicked = flag;
                }
                GUILayout.EndHorizontal();
                if (!showLastClicked)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(0);
                    GUILayout.EndHorizontal();
                    Rect rect = GUILayoutUtility.GetLastRect();
                    rect.width = this.position.width - 10;
                    rect.x = 5;
                    EditorUtils.DrawLineUnderRect(rect, CustomColors.HardShadow);
                    EditorUtils.DrawLineUnderRect(rect, CustomColors.HarderBright, 1);
                    return;
                }
                GUILayout.BeginVertical(CustomGUIStyles.BoxStyle);
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                for (int i = 0; i < Mathf.Min(lastClicked.Count, 5); i++)
                {
                    DrawGameObjectEntry(lastClicked[i], width, 1, i);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                if (lastClicked.Count > 5)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    for (int i = 5; i < Mathf.Min(lastClicked.Count, 10); i++)
                    {
                        DrawGameObjectEntry(lastClicked[i], width, 1, i);
                        GUILayout.FlexibleSpace();
                    }
                    int remaining = 10 - lastClicked.Count;
                    if (remaining > 0)
                    {
                        for (int i = 0; i < remaining; i++)
                        {
                            GUILayout.Space(width);
                            GUILayout.FlexibleSpace();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                EditorUtils.DrawRectBorder(GUILayoutUtility.GetLastRect(), CustomColors.MediumShadow, 1);
                EditorUtils.DrawLineUnderRect(CustomColors.SimpleBright, 0);
            }
        }

        void RemoveFromMostClicked(List<GameObject> gos)
        {
            if (tracker == null)
            {
                return;
            }
            tracker.RemoveFromMost(gos);
        }

        void RemoveFromLastClicked(List<GameObject> gos)
        {
            if (tracker == null)
            {
                return;
            }
            tracker.RemoveFromLast(gos);
        }

        private void DrawMostClickedSection()
        {
            if (mostClicked != null && mostClicked.Count > 0)
            {
                float width = position.width - 55;
                if (mostClicked.Count < 5)
                {
                    width = width / mostClicked.Count;
                }
                else
                {
                    width = width / 5;
                }
                GUILayout.Space(10);
                GUILayout.BeginHorizontal(GUILayout.Height(20));
                GUILayout.Space(17);

                bool flag = EditorGUILayout.Foldout(showMostClicked, " Most Clicked", true, CustomGUIStyles.BoldFoldoutStyle);
                {
                    showMostClicked = flag;
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                if (!showMostClicked)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(0);
                    GUILayout.EndHorizontal();
                    Rect rect = GUILayoutUtility.GetLastRect();
                    rect.width = this.position.width - 10;
                    rect.x = 5;
                    EditorUtils.DrawLineUnderRect(rect, CustomColors.HardShadow);
                    EditorUtils.DrawLineUnderRect(rect, CustomColors.HarderBright, 1);
                    return;
                }
                GUILayout.BeginVertical(CustomGUIStyles.BoxStyle);
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();

                for (int i = 0; i < Mathf.Min(mostClicked.Count, 5); i++)
                {
                    DrawGameObjectEntry(mostClicked[i], width, 2, i);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.EndHorizontal();
                if (mostClicked.Count > 5)
                {
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    for (int i = 5; i < Mathf.Min(mostClicked.Count, 10); i++)
                    {
                        DrawGameObjectEntry(mostClicked[i], width, 2, i);
                        GUILayout.FlexibleSpace();
                    }
                    int remaining = 10 - mostClicked.Count;
                    if (remaining > 0)
                    {
                        for (int i = 0; i < remaining; i++)
                        {
                            GUILayout.Space(width);
                            GUILayout.FlexibleSpace();
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                EditorUtils.DrawRectBorder(GUILayoutUtility.GetLastRect(), CustomColors.MediumShadow, 1);
                EditorUtils.DrawLineUnderRect(CustomColors.SimpleBright, 0);
            }
        }

        private void DrawGameObjectEntry(List<GameObject> entry, float width, int mode, int index)
        {
            GUILayout.BeginVertical(GUILayout.Width(width));
            GUIContent content;
            if (mode == 1)
            {
                content = tracker.GetContentForLast(index);
            }
            else if (mode == 2)
            {
                content = tracker.GetContentForMost(index);
            }
            else
            {
                content = CustomGUIContents.EmptyContent;
            }
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Color color = GUI.backgroundColor;
            if (!EditorUtils.IsLightSkin())
            {
                GUI.backgroundColor = CustomColors.NewTabButton;
            }
            else
            {
                GUI.backgroundColor -= CustomColors.SoftShadow;
            }
            if (GUILayout.Button(content, CustomGUIStyles.CustomButtonStyle))
            {
                if (Event.current.button == 0)
                {
                    if (entry.Count == 1)
                    {
                        SetTargetGameObject(entry[0]);
                    }
                    else
                    {
                        SetTargetGameObjects(entry.ToArray());
                    }
                    //UpdateClicked(entry.ToArray());
                    RepaintForAWhile();
                    FocusTab(activeIndex);
                }
                else if (Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Open in new Tab"), false, () =>
                    {
                        if (entry.Count == 1)
                        {
                            AddTabNext(entry[0]);
                        }
                        else
                        {
                            AddMultiTabNext(entry.ToArray());
                        }
                        //UpdateClicked(entry.ToArray());
                        Repaint();
                    });
                    menu.AddItem(new GUIContent("Select"), false, () =>
                    {
                        if (entry.Count == 1)
                        {
                            SetTargetGameObject(entry[0]);
                        }
                        else
                        {
                            SetTargetGameObjects(entry.ToArray());
                        }
                        //UpdateClicked(entry.ToArray());
                        RepaintForAWhile();
                        FocusTab(activeIndex);
                    });
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Ping"), false, () =>
                    {
                        EditorGUIUtility.PingObject(entry[0]);
                    });
                    menu.AddItem(new GUIContent("Focus on Scene View"), false, () =>
                    {
                        if (entry.Count == 1)
                        {
                            FocusOnSceneView(entry[0]);
                        }
                        else
                        {
                            FocusOnSceneView(entry.ToArray());
                        }
                    });
                    if (entry.Count == 1)
                    {
                        menu.AddItem(new GUIContent("Show In Local Hierarchy View"), false, () =>
                        {
                            HierarchyPopup.ShowWindow(entry[0], this, clickMousePosition);

                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Show In Local Hierarchy View"));

                    }
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("Remove from List"), false, () =>
                    {
                        if (mode == 1)
                        {
                            RemoveFromLastClicked(entry);
                        }
                        else
                        {
                            RemoveFromMostClicked(entry);
                        }
                        ReadLastClicked();
                    });
                    menu.ShowAsContext();
                }
                else if (Event.current.button == 2)
                {
                    if (entry.Count == 1)
                    {
                        AddTabNext(entry[0]);
                    }
                    else
                    {
                        AddMultiTabNext(entry.ToArray());
                    }
                    //UpdateClicked(entry.ToArray());
                    Repaint();
                }
            }
            GUI.backgroundColor = color;
            Rect rect = GUILayoutUtility.GetLastRect();
            CustomGUIContents.DrawCustomButton(rect);
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (entry.Count == 1)
            {
                GUILayout.Label(entry[0].name, CustomGUIStyles.ObjectListLabel);
            }
            else
            {
                GUIContent content2 = CustomGUIContents.EmptyContent;
                content2.text = $"({entry.Count}) Objects";
                content2.tooltip = string.Join("\n", entry.ConvertAll(go => go.name));
                GUILayout.Label(content2, CustomGUIStyles.ObjectListLabel);
            }
            GUILayout.EndVertical();
        }


        internal void UpdateCurrentTip()
        {
            ReadLastClicked();
            if (!justOpened)
            {
                SaveSettings();
            }
            currentTip = TipsManager.GetRandomTip();
            numberOfTips = TipsManager.GetTipsProgress();
        }

        internal string GetCurrentTip()
        {
            if (currentTip != null && currentTip.Length > 0)
            {
                return currentTip;
            }
            else return "Select a GameObject to inspect it!";
        }

        private void StartMessageGUI()
        {
            if (tabs == null || tabs.Count == 0 || Event.current == null)
            {
                return;
            }
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, CustomGUIStyles.InspectorSectionStyle);
            GUILayout.Space(10);
            if (!InRecoverScreen())
            {
                DrawLastClickedSection();
                DrawMostClickedSection();
                EditorUtils.DrawTipSection(this);
                justOpened = false;
            }
            else
            {
                DrawRecoverScreen();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndScrollView();
            if (Event.current.type == EventType.Repaint)
            {
                Rect componentSection = GUILayoutUtility.GetLastRect();
                startComponentY = componentSection.y + 100;
            }
        }

        void ShowRecoverSessionDialogue()
        {
            int tabNumber = lastSessionData.tabs.Count;
            string tabNames = "";
            for (int j = 0; j < tabNumber; j++)
            {
                if (lastSessionData.tabs[j].newTab)
                {
                    tabNames += "\n" + (j + 1) + ".  " + "*New Tab";
                }
                else
                {
                    tabNames += "\n" + (j + 1) + ".  " + lastSessionData.tabs[j].name;
                }
            }
            if (EditorUtility.DisplayDialog("Are you sure?", "This will restore " + tabNumber + " Tabs from your Session:\n\n" + lastSessionData.lastSaveTimePrint + "\n" + tabNames, "Yes!", "No"))
            {
                RestoreSession();
            }
        }

        private void DrawTabBar()
        {
            if (Event.current == null)
            {
                return;
            }
            float padding = 0;
            GUIStyle toolbar = CustomGUIStyles.ScrollTabsStyle;
            if (showHistory)
            {
                toolbar = CustomGUIStyles.ScrollTabsHistoryStyle;
                EditorGUILayout.BeginVertical();
                DrawBackNextButtons();
                EditorGUILayout.EndVertical();
                Rect _lineRect = GUILayoutUtility.GetLastRect();
                _lineRect.width = 1;
                _lineRect.height = 23;
                _lineRect.x += 19;
                EditorGUI.DrawRect(_lineRect, CustomColors.SimpleShadow);
                _lineRect.x += 20;
                EditorGUI.DrawRect(_lineRect, CustomColors.SimpleShadow);
                padding = 40;
            }
            if (tabs != null && tabs.Count > 1 && dragging)
            {
                Rect rect1 = new Rect(padding, 0, GetTotalTabsWidth() - 3, 3);
                if (!EditorUtils.IsLightSkin())
                {
                    EditorGUI.DrawRect(rect1, CustomColors.DarkInspector);
                    rect1.y += 3;
                    rect1.height = 20;
                }
                else
                {
                    rect1.height = 24;
                    EditorUtils.DrawLineOverRect(rect1, CustomColors.HardShadow);
                }
                EditorGUI.DrawRect(rect1, CustomColors.LineColor * 1.4f);

            }
            GUIStyle toolbarButton;
            toolbar.margin.top = 0;
            bool ignoreHover = showHistory && (Event.current.mousePosition.x <= 40);
            GUILayoutOption[] options = CustomGUIStyles.ToolBarOptions(position.width);
            EditorGUILayout.BeginHorizontal(options);
            if (showHistory)
            {
                GUILayout.Space(40);
            }
            GUILayout.BeginScrollView(toolbarScrollPosition, false, false, GUIStyle.none, GUIStyle.none, toolbar, GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginHorizontal();
            Color color = GUI.color;
            List<TabInfo> temp;
            bool recoverScreen = InRecoverScreen();
            if (recoverScreen)
            {
                temp = new List<TabInfo>();
            }
            else
            {
                temp = new List<TabInfo>(tabs);
            }
            bool alreadyDragging = false;
            for (int i = 0; i < temp.Count; i++)
            {
                GUI.color = color;
                var item = temp[i].target;
                toolbarButton = CustomGUIStyles.ToolbarButtonTabs_Active;
                if (temp[i].prefab)
                {
                    toolbarButton = CustomGUIStyles.ToolbarButtonTabsPrefab_Active;
                }
                if (i != activeIndex)
                {
                    GUI.color *= CustomColors.TextPreviewColor;
                    if (!temp[i].prefab)
                    {
                        toolbarButton = CustomGUIStyles.ToolbarButtonTabs;
                        if (!EditorGUIUtility.isProSkin)
                        {
                            toolbarButton = CustomGUIStyles.ToolbarButtonTabs_White;
                        }
                    }
                    else
                    {
                        toolbarButton = CustomGUIStyles.ToolbarButtonTabsPrefab;
                    }
                }
                if (!(tabs.Count - 1 >= i))
                {
                    continue;
                }
                GUIContent inspectorContent = CustomGUIContents.TabContent;
                inspectorContent.text = tabs[i].shortName;
                GUIContent tabImage = CustomGUIContents.TabIconContent;
                tabImage.image = null;
                float size = 16;
                float pad = 4;
                if (item)
                {
                    if (showIcons)
                    {
                        tabImage.image = tabs[i].icon;
                    }

                    if (tabs[i].locked)
                    {
                        tabImage.image = CustomGUIContents.AssetLocked.image as Texture2D;
                        size = 17;
                        pad = 3;
                    }
                }
                else if (tabs[i].IsValidMultiTarget())
                {
                    if (tabs[i].locked)
                    {
                        tabImage.image = CustomGUIContents.AssetLocked.image as Texture2D;
                        size = 17;
                        pad = 3;
                    }
                    else
                    {
                        tabImage.image = CustomGUIContents.TabMulti.image as Texture2D;
                    }
                }
                if (tabImage.image != null && (showIcons || tabs[i].locked))
                {
                    toolbarButton.padding = PaddingIcon;
                }
                else
                {
                    toolbarButton.padding = PaddingNoIcon;
                    tabImage.image = null;
                }
                toolbarButton.fixedHeight = 23;

                int click = i;
                /*
                if (i < temp.Count - 1 && FloatingTab.isClosing && FloatingTab.linkedTab != null && FloatingTab.linkedTab == temp[i])
                {
                    float _width = FloatingTab.UpdateMoveDownAnimation();
                    GUILayout.Space(_width);
                    GUI.color = color;
                    continue;
                }*/

                if (!alreadyDragging && dragging && !GOdragging && dragTargetIndex == i)
                {
                    float _width;
                    if (FloatingTab.fallingTab)
                    {
                        _width = totalWidth[FloatingTab.dragIndex];
                    }
                    else
                    {
                        _width = totalWidth[dragIndex];
                    }
                    GUILayout.Space(_width);
                    Rect lineRect = GUILayoutUtility.GetLastRect();
                    lineRect.y += 1;
                    alreadyDragging = true;
                    if (!FloatingTab.fallingTab && dragIndex == activeIndex)
                    {
                        if (showHistory)
                        {
                            lineRect.y -= 1;
                            lineRect.x += 40;
                        }
                        activeTabRect = lineRect;
                    }

                }
                else
                {
                    GUILayout.Space(0);
                }
                if (dragging && dragIndex == i)
                {
                    GUI.color = color;
                    if (!GOdragging)
                    {
                        EditorGUILayout.BeginVertical();
                        GUILayout.Space(0);
                        EditorGUILayout.EndVertical();
                        continue;
                    }
                }
                float width = totalWidth[i];
                toolbarButton.contentOffset = Vector2.zero;
                EditorGUILayout.BeginVertical(GUILayout.Width(width));
                var button = GUILayout.Button(inspectorContent, toolbarButton, GUILayout.Width(width));
                EditorGUILayout.EndVertical();
                toolbarButton.contentOffset = Vector2.zero;
                Rect buttonRect = GUILayoutUtility.GetLastRect();
                Rect fixedButtonRect = new Rect(buttonRect);
                if (showHistory)
                {
                    fixedButtonRect.x += 40;
                }
                fixedButtonRect.x -= toolbarScrollPosition.x;
                bool drawLine = false;
                if (tabs[i].IsValidMultiTarget())
                {
                    if (IsAlreadySelected(tabs[i].targets))
                    {
                        drawLine = true;
                    }
                }
                else if (Selection.gameObjects.Contains(item))
                {
                    drawLine = true;
                }
                tabs[i].isSelected = drawLine;
                if (tabs[i].prefab)
                {
                    EditorUtils.DrawFadeToBottom(buttonRect, CustomColors.FadeBlue, 23);
                }
                else
                {
                    if (EditorUtils.IsLightSkin())
                    {
                        EditorUtils.DrawFadeToBottom(buttonRect, CustomColors.SimpleBright, 12);
                    }
                    else if (i == activeIndex)
                    {
                        EditorUtils.DrawFadeToBottom(buttonRect, CustomColors.SoftBright, 12);
                    }
                }
                float opacity = 0.7f;
                if (activeIndex != i)
                {
                    opacity = 0.6f;
                }
                else
                {
                    activeTabRect = fixedButtonRect;
                }
                float _pad = 0;
                if (i == tabs.Count - 1)
                {
                    _pad = 1;
                }
                if (drawLine)
                {
                    Rect lineRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.width - _pad, 2);
                    EditorGUI.DrawRect(lineRect, new Color(0.5f, 0.5f, 0.8f, opacity));
                }
                else
                {
                    Rect lineRect = new Rect(buttonRect.x, buttonRect.y, buttonRect.width - _pad, 1);
                    EditorGUI.DrawRect(lineRect, CustomColors.HarderBright);
                    EditorUtils.DrawLineOverRect(lineRect);
                }
                Rect rect = new Rect(buttonRect.x + 1, buttonRect.y + pad, size, size);
                if (tabs[i].prefab)
                {
                    GUI.color += Color.blue * 2;
                }
                GUI.Label(rect, tabImage);
                if (tabs[i].prefab)
                {
                    GUI.color -= Color.blue * 2;
                }
                Rect overlayRect = new Rect(buttonRect);
                if (Application.isPlaying)
                {
                    if (activeIndex != i)
                    {
                        EditorGUI.DrawRect(overlayRect, CustomColors.InGameCorrectionColor);
                    }
                    else
                    {
                        EditorGUI.DrawRect(overlayRect, CustomColors.InGameCorrectionColorActive);
                    }
                }
                else
                {
                    if (activeIndex == i)
                    {
                        EditorGUI.DrawRect(overlayRect, CustomColors.InGameCorrectionColorActive);
                    }
                    else
                    {
                        EditorGUI.DrawRect(overlayRect, CustomColors.InGameCorrectionColor);
                    }
                }
                bool hovered = IsLastRectHovered();
                if (hovered)
                {
                    DragTabLogic(fixedButtonRect, buttonRect, i, click, ignoreHover);
                    if (tabs[click].locked && EditorUtils.NotDragging())
                    {
                        float accumulatedWidth = GetTotalTabsWidth(click - 1);
                        Rect lockRect = new Rect(accumulatedWidth - 1, 4, 15, 15);
                        GUI.Button(lockRect, CustomGUIContents.QuickUnlock, CustomGUIStyles.EmptyButton);
                        if (lockRect.Contains(Event.current.mousePosition))
                        {
                            EditorGUI.DrawRect(lockRect, CustomColors.SimpleBright);
                            GUI.Label(rect, tabImage);
                            PopUpTip.Hide();
                        }
                    }
                }
                RepositionDraggedTab(buttonRect, i);
                if (!hovered && ignoreHover)
                {
                    PopUpTip.Hide();
                }

                if (button || pendingTabSwitch == click)
                {
                    dragging = false;
                    if (Event.current.button == 2 && pendingTabSwitch == -1)
                    {
                        if (tabs.Count > 1)
                        {
                            CloseTab(click);
                        }
                    }
                    else if (Event.current.button == 1 && pendingTabSwitch == -1)
                    {
                        PopUpTip.Hide();
                        ShowTabContextMenu(buttonRect, i, click, item);
                    }
                    else
                    {
                        if (tabs[click].locked && EditorUtils.NotDragging())
                        {
                            float accumulatedWidth = GetTotalTabsWidth(click - 1);
                            Rect lockRect = new Rect(accumulatedWidth - 1, 4, 15, 15);
                            if (lockRect.Contains(Event.current.mousePosition))
                            {
                                if (item || tabs[click].IsValidMultiTarget())
                                {
                                    tabs[click].locked = !tabs[click].locked;
                                    GUI.color = color;
                                    GUI.enabled = true;
                                    UpdateAllWidths();
                                    continue;
                                }
                            }
                        }
                        if (pendingTabSwitch == -1)
                        {
                            if (Time.realtimeSinceStartup - lastTabClick < 0.3f && lastTabClick != -1)
                            {
                                if ((item || tabs[click].IsValidMultiTarget()) && click == lastClickedTab)
                                {
                                    HandleTabDoubleClick(tabs[click]);
                                }
                            }
                            lastClickedTab = click;
                            lastTabClick = Time.realtimeSinceStartup;
                        }
                        else
                        {
                            if (Time.realtimeSinceStartup - lastTabClick < 0.5f)
                            {
                                GUI.color = color;
                                GUI.enabled = true;
                                continue;
                            }
                            else
                            {

                                lastTabClick = Time.realtimeSinceStartup;
                            }
                        }
                        PopUpTip.Hide();
                        bool keepCount = false;
                        if (activeIndex != click || !softSelection)
                        {
                            SaveFoldoutsToTab();
                            if (click != activeIndex)
                            {
                                keepCount = true;
                                UpdatePreviousTab();
                            }
                            if (pendingTabSwitch != -1)
                            {
                                click = pendingTabSwitch;
                                pendingTabSwitch = -1;
                                if (!softSelection)
                                {
                                    switchingTabs = true;
                                }
                            }
                        }
                        FocusTab(click, switchingTabs, keepCount);
                    }
                }
                GUI.color = color;
                GUI.enabled = true;
            }
            if (!alreadyDragging && dragging && dragTargetIndex == temp.Count && !GOdragging)
            {
                GUILayout.Space(totalWidth[dragIndex]);
            }
            else
            {
                GUILayout.Space(0);
            }

            GUILayout.FlexibleSpace();
            flexibleRect = GUILayoutUtility.GetLastRect();
            if (flexibleRect.x != 0)
            {
                flexibleRect.height = 23;
                Rect drawFlexibleRect = new Rect(flexibleRect);
                drawFlexibleRect.width += 1;
                drawFlexibleRect.x -= 1;
                if (dragging && !GOdragging)
                {
                    if (flexibleRect.Contains(new Vector2(Event.current.mousePosition.x, dragRect.y)))
                    {
                        dragTargetIndex = temp.Count;
                        //  Debug.Log("Drag target index " + dragTargetIndex);
                    }
                }
                else if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
                {
                    if (flexibleRect.x != 0 && flexibleRect.Contains(Event.current.mousePosition))
                    {
                        ShowNoTabContextMenu(flexibleRect);
                    }
                }
                EditorGUI.DrawRect(drawFlexibleRect, CustomColors.FlexibleColor);
                float spacing = flexibleRect.width;
                if (flexibleRect.x != 0)
                {
                    barSpacing = spacing;
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
            Rect _scrollRect = GUILayoutUtility.GetLastRect();
            if (EditorUtils.IsLightSkin() && recoverScreen)
            {
                EditorUtils.DrawLineUnderRect(_scrollRect, -3);
                EditorUtils.DrawLineUnderRect(_scrollRect, CustomColors.SimpleBright, -2);
            }
            _scrollRect.width -= barSpacing;
            if (_scrollRect.width > 1)
            {
                scrollRect = _scrollRect;
            }
            DrawAddButton();
            GUILayout.Space(2);
            if (FloatingTab.isClosing || FloatingTab.isOpening)
            {
                Repaint();
            }
        }

        void ShowNoTabContextMenu(Rect flexibleRect)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add New Tab"), false, () =>
            {
                AddTabNext();
            });
            menu.AddSeparator("");
            if (AreThereClosedTabs())
            {
                menu.AddItem(new GUIContent("Restore Closed Tab"), false, () =>
                {
                    RestoreClosedTab();
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Restore Closed Tab"));
            }
            if (IsThereAPreviousSession())
            {
                menu.AddItem(new GUIContent("Restore Last Saved Session"), false, () =>
                {
                    ShowRecoverSessionDialogue();
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Restore Last Saved Session"));
            }
            menu.AddSeparator("");
            ShowSettingsMenu(menu, false);
            Rect _rect = new Rect(Event.current.mousePosition.x, flexibleRect.y + 25, 0, 0);
            menu.DropDown(_rect);
        }

        internal void DuplicateTabNext(int click)
        {
            int previousIndex = activeIndex;
            int index = click + 1;
            tabs.Insert(index, new TabInfo(tabs[click]));
            tabs[index].index = index;
            activeIndex = index;
            totalWidth.Add(100);
            FocusTab();
            UpdatePreviousTab(tabs[previousIndex]);
        }

        void UpdatePreviousTab(TabInfo tab = null)
        {
            if (tab == null)
            {
                tab = GetActiveTab();
            }
            if (tab != null)
            {
                previousTab = tab;
            }
        }

        void ResetMaximizedView()
        {
            if (maximizedAssetView)
            {
                maximizedAssetView = false;
                assetsCollapsed = false;
                ResetAssetViewSize();
            }
        }

        public void AddTabNext(GameObject target = null, bool focus = true)
        {
            ResetMaximizedView();
            tabs.Insert(activeIndex + 1, new TabInfo(target, tabs.Count, this));
            UpdatePreviousTab();
            int index = activeIndex + 1;
            if (focus)
            {
                activeIndex = index;
            }
            TabInfo tab = tabs[index];
            tab.newTab = true;
            totalWidth.Add(100);
            if (target)
            {
                tab.newTab = false;
                if (IsGameObjectInPrefabMode(target))
                {
                    tab.prefab = true;
                    tab.history = null;
                }
                UpdateClicked(target);
            }
            if (focus)
            {
                FocusTab(index);
            }
            else
            {
                RefreshAllIcons();
                ScrollToIndex(index, true);
            }
            RepaintForAWhile();
        }
        internal void AddMultiTabNext(GameObject[] targets, bool focus = true)
        {
            ResetMaximizedView();
            tabs.Insert(activeIndex + 1, new TabInfo(targets, tabs.Count, this));
            UpdatePreviousTab();
            int index = activeIndex + 1;
            if (focus)
            {
                activeIndex = index;
            }
            TabInfo tab = tabs[index];
            tab.newTab = true;
            tab.multiEditMode = true;
            totalWidth.Add(100);
            if (targets != null && targets.Length > 0)
            {
                tab.newTab = false;
                if (IsGameObjectInPrefabMode(targets[0]))
                {
                    tab.prefab = true;
                    tab.history = null;
                }
                UpdateClicked(targets);
            }
            if (focus)
            {
                FocusTab(index);
            }
            else
            {
                RefreshAllIcons();
                ScrollToIndex(index, true);
            }
            RepaintForAWhile();
        }

        void DrawAddButton()
        {

            GUIStyle addStyle = CustomGUIStyles.AddStyle;
            GUIContent addContent = CustomGUIContents.AddContent;
            int yPosition = 0;
            Color color = GUI.backgroundColor;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.backgroundColor += CustomColors.AddButtonColorDark;
            }
            else
            {
                GUI.backgroundColor -= CustomColors.AddButtonColorLight;
            }
            Rect buttonRect = new Rect(this.position.width - 23, yPosition, 23, 23);

            if (GUI.Button(buttonRect, addContent, addStyle))
            {
                UpdateCurrentTip();
                if (InRecoverScreen())
                {
                    tabs.Clear();
                    activeIndex = -1;
                    lastSessionData = null;
                }
                if (activeIndex > tabs.Count - 1)
                {
                    activeIndex = tabs.Count - 1;
                }
                int index = tabs.Count;
                if (Event.current.button == 0)
                {
                    index = activeIndex + 1;
                }
                UpdatePreviousTab();
                tabs.Insert(index, new TabInfo(null, index, this, ""));
                tabs[index].newTab = true;
                activeIndex = index;
                targetGameObject = null;
                //OverrideTarget(null);
                totalWidth.Add(100);
                ScrollToIndex(activeIndex);
                RepaintForAWhile();
            }
            CustomGUIContents.DrawCustomButton(buttonRect, false, false);
            GUI.backgroundColor = color;
        }
        void DrawBackNextButtons()
        {
            GUIStyle toolbarButton = CustomGUIStyles.ModifiedToolbarButton;
            int yPosition = 0;
            int height = 24;
            GUI.enabled = GetActiveTab().CanMoveBack();
            Color backColor = GUI.backgroundColor;
            Color guiColor = GUI.color;
            if (GUI.enabled)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    GUI.backgroundColor += CustomColors.DarkHistoryButton;
                }
                else
                {
                    GUI.backgroundColor -= CustomColors.LightHistoryButton * 1.75f;
                }
            }
            else
            {
                if (EditorGUIUtility.isProSkin)
                {
                    GUI.backgroundColor -= CustomColors.DarkHistoryDisabled;
                }
                else
                {
                    GUI.backgroundColor -= CustomColors.LightHistoryDisabled * 1.75f;
                }
            }
            GUIContent forwardContent = CustomGUIContents.ForwardContent;
            if (GetActiveTab().CanMoveForward())
            {
                forwardContent.tooltip = "Go to next Selection\nMiddle click -> Open in new tab";
            }
            GUIContent backContent = CustomGUIContents.BackContent;
            if (GetActiveTab().CanMoveBack())
            {
                backContent.tooltip = "Go to previous Selection\n" + "Middle click -> Open in new tab";
            }
            Rect buttonRect = new Rect(0, yPosition, 20, height);
            if (GUI.Button(buttonRect, backContent, toolbarButton))
            {
                if (Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < GetActiveTab()._BackHistory().Count; i++)
                    {
                        var go = GetActiveTab().BackHistory()[i];
                        menu.AddItem(new GUIContent(GetActiveTab()._BackHistory()[i]), false, () =>
                         {
                             GetActiveTab().MoveBackUntil(go);
                         });
                    }
                    Rect rect = new Rect(0, yPosition + 25, 0, 0);
                    menu.DropDown(rect);
                }
                else if (Event.current.button == 0)
                {
                    GetActiveTab().MoveBack();
                }
                else if (Event.current.button == 2)
                {
                    GameObject[] _step = GetActiveTab().BackHistory()[0];
                    if (_step != null)
                    {
                        if (_step.Length == 1)
                        {
                            AddTabNext(_step[0]);
                        }
                        else
                        {
                            AddMultiTabNext(_step);
                        }
                    }
                    List<GameObject[]> history = new List<GameObject[]>(tabs[activeIndex - 1].history);
                    GetActiveTab().history = history;
                    GetActiveTab().historyPosition = tabs[activeIndex - 1].historyPosition - 1;
                }
            }
            if (GUI.enabled)
            {
                CustomGUIContents.DrawCustomButton(buttonRect, false, false, true);
            }
            else
            {
                EditorUtils.DrawLineOverRect(buttonRect, 0);
            }
            GUI.enabled = true;
            GUI.backgroundColor = backColor;
            bool showPast = GetActiveTab().CanMoveForward();
            GUI.enabled = showPast;
            if (showPast)
            {
                if (EditorGUIUtility.isProSkin)
                {
                    GUI.backgroundColor += CustomColors.DarkHistoryButton;
                }
                else
                {
                    GUI.backgroundColor -= CustomColors.LightHistoryButton * 1.75f;
                }
            }
            else
            {
                if (EditorGUIUtility.isProSkin)
                {
                    GUI.backgroundColor -= CustomColors.DarkHistoryDisabled;
                }
                else
                {
                    GUI.backgroundColor -= CustomColors.LightHistoryDisabled * 1.75f;
                }
            }
            buttonRect = new Rect(20, yPosition, 20, height);
            if (GUI.Button(buttonRect, forwardContent, toolbarButton))
            {
                if (Event.current.button == 1)
                {
                    GenericMenu menu = new GenericMenu();
                    for (int i = 0; i < GetActiveTab()._ForwardHistory().Count; i++)
                    {
                        var go = GetActiveTab().ForwardHistory()[i];
                        menu.AddItem(new GUIContent(GetActiveTab()._ForwardHistory()[i]), false, () =>
                        {
                            GetActiveTab().MoveForwardUntil(go);
                        });
                    }
                    Rect rect = new Rect(20, yPosition + 25, 0, 0);
                    menu.DropDown(rect);
                }
                else if (Event.current.button == 0)
                {
                    GetActiveTab().MoveForward();
                }
                else if (Event.current.button == 2)
                {
                    GameObject[] _step = GetActiveTab().ForwardHistory()[0];
                    if (_step != null)
                    {
                        if (_step.Length == 1)
                        {
                            AddTabNext(_step[0]);
                        }
                        else
                        {
                            AddMultiTabNext(_step);
                        }
                    }
                    List<GameObject[]> history = new List<GameObject[]>(tabs[activeIndex - 1].history);
                    GetActiveTab().history = history;
                    GetActiveTab().historyPosition = tabs[activeIndex - 1].historyPosition + 1;
                }
            }
            if (GUI.enabled)
            {
                CustomGUIContents.DrawCustomButton(buttonRect, false, false, true);
            }
            else
            {
                EditorUtils.DrawLineOverRect(buttonRect, 0);
            }
            GUI.enabled = true;
            GUI.backgroundColor = backColor;
        }

        void LimitScrollBar()
        {
            if (toolbarScrollPosition.x < 0)
            {
                toolbarScrollPosition.x = 0;
                return;
            }
            float maxScroll = GetMaximumScroll();


            if (maxScroll < 0)
            {
                maxScroll = 0;
            }
            if (maxScroll == 0)
            {
                toolbarScrollPosition.x = 0;
                return;
            }
            if (toolbarScrollPosition.x > maxScroll)
            {
                toolbarScrollPosition.x = maxScroll;
            }
        }

        float GetMaximumScroll()
        {
            int historyVar = 36;
            float windowWidth = position.width;
            int addVar = 24;
            if (!showHistory)
            {
                historyVar = 0;
            }
            float viewportWidth = windowWidth - addVar - historyVar;
            float total = GetTotalTabsWidth() - viewportWidth;
            if (total < 0)
            {
                total = 0;
            }
            return total;
        }
        internal float GetTotalTabsWidth(int upToIndex)
        {
            if (upToIndex == -1)
            {
                return 3;
            }

            float total = 0;
            int lastIndex = Mathf.Min(upToIndex, tabs.Count - 1);

            for (int i = 0; i <= lastIndex; i++)
            {
                total += tabs[i].tabWidth;
            }
            total += 3;
            return total;
        }

        internal float GetTotalTabsWidth()
        {
            if (totalWidth == null || totalWidth.Count == 0)
            {
                return lastValidTabWidth;
            }
            float total = 0;
            for (int i = 0; i < tabs.Count; i++)
            {
                total += tabs[i].tabWidth;
            }
            if (total == 0)
            {
                return lastValidTabWidth;
            }
            total += 3;
            lastValidTabWidth = total;
            return total;
        }
        void ScrollToActiveTab()
        {
            if (activeIndex < 0 || tabs == null || activeIndex > tabs.Count - 1)
            {
                return;
            }
            FocusTab(activeIndex);
        }
        void ScrollToIndex(int index, bool softScroll = false)
        {
            if (tabs != null && tabs.Count > 0)
            {
                UpdateAllWidths();
                UpdateTabBar(true);
                LimitScrollBar();
            }
            if (index > tabs.Count - 1)
            {
                index = tabs.Count - 1;
            }
            if (index < 0)
            {
                index = 0;
            }
            if (totalWidth.Count == 0 || totalWidth == null || totalWidth.Count < index || totalWidth.Count < 1)
            {
                totalWidth.Add(100);
                return;
            }
            if (!softScroll)
            {
                activeIndex = index;
            }
            if (activeIndex < 0 || activeIndex > totalWidth.Count - 1)
            {
                totalWidth.Add(100);
            }
            float buttonWidth = totalWidth[index];
            float total = 0f;
            float totalContentWidth = 0f;
            for (int i = 0; i < index; i++)
            {
                total += totalWidth[i];
            }
            for (int i = 0; i < totalWidth.Count; i++)
            {
                if (!isLocked && i == 0)
                {
                    continue;
                }
                totalContentWidth += totalWidth[i];
            }
            float viewWidth = scrollRect.width + 1;
            if (showHistory)
            {
                viewWidth -= 36;
            }
            if (total - 25 < toolbarScrollPosition.x)
            {
                toolbarScrollPosition.x = total - 25;
            }
            else if (total + buttonWidth + 25 > toolbarScrollPosition.x + viewWidth)
            {
                toolbarScrollPosition.x = total + 25 + buttonWidth - viewWidth;
            }
            //    Debug.Log("Scrolling to a width of " + buttonWidth + " " + GetActiveTab().TabTextContent.text);
            toolbarScrollPosition.x = Mathf.Max(0, Mathf.Min(toolbarScrollPosition.x, total));
            LimitScrollBar();
            RepaintForAWhile();
        }

        void SelectCurrentTab()
        {
            GameObject[] targets = null;

            if (IsActiveTabValidMulti())
            {
                targets = GetActiveTab().targets;
            }
            else if (GetActiveTab().target)
            {
                targets = new GameObject[] { GetActiveTab().target };
            }
            if (targets != null && targets.Length > 0)
            {
                SelectIfNotAlready(targets);
            }
        }

        bool DrawComponentEnabledToggle(Editor editor, Behaviour behaviour, bool wasEnabled, bool isDragging = false)
        {
            if (behaviour == null)
            {
                return false;
            }
            Rect position = GUILayoutUtility.GetLastRect();
            position.x = 40;
            if (isDragging)
            {
                position.x += 10;
            }
            EditorGUI.Toggle(position, "", behaviour.enabled);
            if (EditorGUI.EndChangeCheck())
            {
                float cursorX = Event.current.mousePosition.x;
                if (cursorX < 55 && cursorX > 40)
                {
                    editor.serializedObject.Update();
                    EditorUtility.SetObjectEnabled(editor.serializedObject.targetObject, !wasEnabled);
                    editor.serializedObject.ApplyModifiedProperties();
                    Repaint();
                    return true;
                }
            }
            return false;
        }

        bool DrawMultiComponentEnabledToggle(Editor editor, bool wasEnabled)
        {
            Rect position = GUILayoutUtility.GetLastRect();
            position.x = 40;
            bool allSame = true;
            for (int i = 0; i < editor.targets.Length; i++)
            {
                if ((editor.targets[i] as Behaviour).enabled != wasEnabled)
                {
                    allSame = false;
                    break;
                }
            }
            EditorGUI.showMixedValue = !allSame;
            EditorGUI.Toggle(position, "", wasEnabled);

            if (EditorGUI.EndChangeCheck())
            {
                float cursorX = Event.current.mousePosition.x;
                if (cursorX < 55 && cursorX > 40)
                {
                    editor.serializedObject.Update();
                    foreach (var target in editor.targets)
                    {
                        EditorUtility.SetObjectEnabled(target, !wasEnabled);
                    }
                    editor.serializedObject.ApplyModifiedProperties();
                    Repaint();
                    return true;
                }
            }
            return false;
        }

        void DrawComponentEditorTools(Component component, Rect compHeaderRect)
        {
#if UNITY_2020_2_OR_NEWER && !UNITY_2022_2_OR_NEWER
            DoDrawComponentEditorTools(component, compHeaderRect);
#else
            return;
#endif
        }
        void DoDrawComponentEditorTools(Component component, Rect compHeaderRect)
        {
            if (Reflected.ComponentHasEditorTool(component))
            {

                if (!GetActiveTab().debug)
                {
                    GUI.enabled = GetActiveTab().target.activeInHierarchy;
                    var tools = EditorToolCache.GetToolsForComponent(component);
                    for (int i = 0; i < tools.Count; i++)
                    {
                        var tool = tools[i];
                        GUIContent button = EditorToolCache.
                        GetToolIconForComponent(component, tool);
                        GUIStyle style = CustomGUIStyles.ActiveButtonStyle;
                        Rect buttonRect = compHeaderRect;
                        buttonRect.y = compHeaderRect.yMax + 11;
                        buttonRect.width = 32;
                        buttonRect.height = 22;
                        buttonRect.x = EditorGUIUtility.labelWidth + buttonRect.width / 2 + 3;
                        buttonRect.x += buttonRect.width * i;
                        Color color1 = GUI.color;
                        bool active = EditorToolCache.IsToolActiveForComponent(component, tool);
                        if (!active)
                        {
                            GUI.color -= CustomColors.EditorToolInactive;
                        }
                        else
                        {
                            GUI.color += CustomColors.EditorToolActive;
                        }
                        if (GUI.Button(buttonRect, "", style))
                        {
                            if (!active)
                            {
                                EditorToolCache.ActivateTool(tool, component);
                            }
                            else
                            {
                                EditorToolCache.RestorePreviousPersistentTool();
                            }
                        }
                        GUI.color = color1;
                        GUI.Label(buttonRect, button, CustomGUIStyles.CenteredLabelStyle);
                        buttonRect.width -= 2;
                        buttonRect.x += 1;
                        EditorUtils.DrawLineOverRect(buttonRect, -1);
                    }
                    GUI.enabled = true;
                    GUILayout.Space(5);
                }
            }
        }

        internal void HandleAssetClick(string guid, Rect selectionRect)
        {
            Event current = Event.current;
            if (current.isMouse && current.button == 0 && !current.alt && !EditorUtils.IsCtrlHeld() && !current.shift)
            {
                if (!selectionRect.Contains(current.mousePosition))
                {
                    return;
                }
                if (lockedAsset || (ignoreFolders && IsAssetAFolder(AssetDatabase.GUIDToAssetPath(guid))))
                {
                    return;
                }
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(guid));
                if (asset == null)
                {
                    return;
                }

                if (!lockedAsset && asset != targetObject)
                {
                    if (current.type == EventType.MouseDown)
                    {
                        awaitingAssetClick = true;
                        return;
                    }
                    else if (current.type == EventType.MouseUp && awaitingAssetClick)
                    {
                        ignoreSelection = new UnityObject[] { asset };
                        if (!CheckForApplyRevertOnClose())
                        {/*
                            if (targetObject)
                            {

                            }*/
                            return;
                        }
                        SetTargetAsset(asset);
                    }
                }
            }
            if (current.type == EventType.MouseUp)
            {
                awaitingAssetClick = false;
            }
        }

        internal void OpenInNewTab(GameObject[] gameObjects, bool focusAfter = true)
        {
            tabs.Add(new TabInfo(gameObjects, tabs.Count, this));

            if (!focusAfter)
            {
                ignoreNextSelection = true;
                RefreshAllIcons();
                RepaintForAWhile();
                return;
            }
            else
            {
                UpdatePreviousTab();
            }
            activeIndex = tabs.Count - 1;
            tabs[tabs.Count - 1].newTab = false;
            totalWidth.Add(1000);
            if (IsActiveTabValidMulti())
            {
                SetTargetGameObjects(GetActiveTab().targets);
            }
            else
            {
                SetTargetGameObject(GetActiveTab().target);
            }
            FocusTab(activeIndex);
            RepaintForAWhile();
        }

        internal void HandleMiddleClick(int instanceID, Rect selectionRect)
        {

            Event current = Event.current;
            if (current.type == EventType.MouseUp && current.button == 0 && !current.alt && !current.shift && !EditorUtils.IsCtrlHeld())
            {
                if (GetActiveTab() != null)
                {
                    GetActiveTab().RefreshName();
                }
                if (Time.realtimeSinceStartup - lastTabClick < 0.1f && lastTabClick != -1)
                {
                    return;
                }
                if (!selectionRect.Contains(current.mousePosition))
                {
                    return;
                }
                lastTabClick = Time.realtimeSinceStartup;
                if (!selectionRect.Contains(current.mousePosition))
                {
                    return;
                }
                if (current.button == 0)
                {
                    if (!selectionRect.Contains(current.mousePosition))
                    {
                        return;
                    }
                    GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                    if (go == null)
                    {
                        return;
                    }
                    if (IsActiveTabLocked())
                    {
                        return;
                    }
                    /* NEEDS SOME REWORK. DISABLED FOR NOW

                    if (EditorUtils.IsCtrlHeld() && GetActiveTab() != null && !GetActiveTab().newTab)
                    {
                        Debug.Log("Holding ctrl");
                        if (Selection.gameObjects != null && Selection.gameObjects.Length > 0)
                        {
                            GameObject[] tabObjects;
                            if (IsActiveTabValidMulti())
                            {
                                tabObjects = GetActiveTab().targets;
                            }
                            else
                            {
                                tabObjects = new GameObject[] { GetActiveTab().target };
                            }

                            if (!AreArraysEqual(Selection.gameObjects, tabObjects))
                            {
                                Debug.Log("Holding nulling");
                                List<GameObject> newObjects = new List<GameObject>(tabObjects);
                               
                                if (!newObjects.Contains(go))
                                {                               
                                    newObjects.Add(go);
                                }
                                Debug.Log("new objects count is " + newObjects.Count);
                                 
                                OverrideMulti(newObjects.ToArray());
                                Selection.objects = null;
                               ignoreNextSelection = true;
                               ignoreSelection = newObjects.ToArray();
                                Selection.objects = newObjects.ToArray();
                                return;

                            }
                        }
                    } */

                    if (tabs.Count > 0 && GetActiveTab().target != go)
                    {
                        GetActiveTab().multiEditMode = false;
                        EditorToolCache.RestorePreviousPersistentTool();
                        SetTargetGameObject(go);
                        Repaint();
                        //UpdateClicked(go);
                    }

                    return;
                }
            }

            else if (current.type == EventType.MouseUp && !EditorUtils.IsCtrlHeld() && current.button == 2 && !current.shift)
            {
                if (GetActiveTab() != null)
                {
                    GetActiveTab().RefreshName();
                }
                if (Time.realtimeSinceStartup - lastTabClick < 0.1f && lastTabClick != -1)
                {
                    return;
                }
                if (!selectionRect.Contains(current.mousePosition))
                {
                    return;
                }
                GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                if (go == null)
                {
                    return;
                }
                lastTabClick = Time.realtimeSinceStartup;
                if (Selection.gameObjects != null && Selection.gameObjects.Length > 1)
                {
                    if (Selection.gameObjects.Contains(go))
                    {
                        if (InRecoverScreen())
                        {
                            tabs.Clear();
                            activeIndex = -1;
                            lastSessionData = null;
                        }
                        AddMultiTabNext(Selection.gameObjects);
                        //UpdateClicked(Selection.gameObjects);
                        return;
                    }
                }
                Selection.activeGameObject = go;
                if (InRecoverScreen())
                {
                    tabs.Clear();
                    activeIndex = -1;
                    lastSessionData = null;
                }
                AddTabNext(go);
                // UpdateClicked(go);               
            }
        }

        internal static bool AreArraysEqual(GameObject[] a, GameObject[] b)
        {
            if (a == null || b == null)
            {
                return false;
            }
            if (a.Length != b.Length)
            {
                return false;
            }
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }
            return true;
        }

        void PerformRedo()
        {
            //  if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                //   Undo.PerformRedo();
                //  Repaint();
            }

        }
        void HandleMoveComponent(Component draggedComponent, int index, int targetIndex, Component[] components)
        {
            if (pendingOperation == null || pendingOperation.consumed)
            {
                return;
            }
            pendingOperation.consumed = true;
            int compIndex = index;
            if (compIndex > targetIndex)
            {
                targetIndex += 1;
                if (targetIndex != compIndex)
                {
                    MoveComponentUp(draggedComponent, compIndex, targetIndex);
                    PerformRedo();
                    RefreshAllIcons();
                }
                else
                {
                    PerformRedo();
                    RefreshAllIcons();
                }
            }
            else if (compIndex < targetIndex)
            {
                if (targetIndex != compIndex)
                {
                    MoveComponentDown(draggedComponent, compIndex, targetIndex);
                    PerformRedo();
                }
                else
                {
                    PerformRedo();
                    RefreshAllIcons();
                }
            }
            else
            {
                PerformRedo();
                RefreshAllIcons();
            }
            performPasteComponent = 0;
        }


        void MoveComponentUp(Component draggedComponent, int compIndex, int targetIndex)
        {
            while (compIndex > targetIndex)
            {
                ComponentUtility.MoveComponentUp(draggedComponent);
                compIndex--;
            }
        }
        void MoveComponentDown(Component draggedComponent, int compIndex, int targetIndex)
        {
            while (compIndex < targetIndex)
            {
                ComponentUtility.MoveComponentDown(draggedComponent);
                compIndex++;
            }
        }
        internal bool EnteringPlaymode
        {
            get
            {
                if (settingsData != null)
                {
                    return settingsData.enteringPlayMode;
                }
                return enteringPlayMode;
            }
        }
        internal void SetEnteringPlayMode()
        {
            if (settingsData != null)
            {
                settingsData.enteringPlayMode = true;
            }
            enteringPlayMode = true;
        }
        internal void SetNotEnteringPlayMode()
        {
            if (settingsData != null)
            {
                settingsData.enteringPlayMode = false;
            }
            enteringPlayMode = false;
        }
        internal static bool CanAddMultipleTimes(Component T)
        {
            object[] attributes = T.GetType().GetCustomAttributes(typeof(DisallowMultipleComponent), inherit: true);
            return attributes.Length == 0;
        }
        private void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            ReloadPreview();
            switch (state)
            {

                case PlayModeStateChange.ExitingEditMode:
                    inActualPlayMode = true;
                    lastTabClick = -1;
                    lastTabClick = -1;
                    lastOpen = -1;
                    SetEnteringPlayMode();
                    pendingRestore = false;
                    //Debug.Log("Exited Edit Mode");
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    inActualPlayMode = false;
                    SetNotEnteringPlayMode();
                    CloseCoInspector();
                    exitingPlayMode = true;
                    if (sessionsMode == 2)
                    {
                        pendingRestore = true;
                    }
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    inActualPlayMode = false;
                    SetNotEnteringPlayMode();
                    lastTabClick = -1;
                    lastTabClick = -1;
                    lastOpen = -1;
                    pendingRestore = !InRecoverScreen();
                    activeScene = SceneInfo.FromActiveScene();
                    ReopenCoInspector(SceneManager.GetActiveScene(), LoadSceneMode.Single);
                    scenesChanged = false;
                    pendingRestore = false;
                    RunNextFrame(FocusTab);
                    break;
                case PlayModeStateChange.EnteredPlayMode:
                    inActualPlayMode = true;
                    SetNotEnteringPlayMode();
                    lastTabClick = -1;
                    lastTabClick = -1;
                    FocusTab();
                    EditorApplication.delayCall += ReinitializeComponentEditors;
                    EditorApplication.delayCall += Repaint;
                    if (instances == null)
                    {
                        instances = new List<CoInspectorWindow>();
                    }
                    if (!instances.Contains(this))
                    {
                        instances.Add(this);
                    }
                    //Debug.Log("Entered Play Mode");
                    break;
            }
        }
        void SetAllEditorsDebugTo(bool value)
        {
            if (componentEditors != null)
            {
                InspectorMode inspectorMode = InspectorMode.Normal;
                if (value)
                {
                    inspectorMode = InspectorMode.Debug;
                }
                for (int i = 0; i < componentEditors.Length; i++)
                {
                    if (componentEditors[i] != null)
                    {
                        Reflected.SetInspectorMode(componentEditors[i], inspectorMode);
                    }
                }
            }
        }
        void SetAllAssetEditorsDebugTo(bool value)
        {
            if (prefabEditors != null)
            {
                InspectorMode inspectorMode = InspectorMode.Normal;
                if (value)
                {
                    inspectorMode = InspectorMode.Debug;
                }
                for (int i = 0; i < prefabEditors.Length; i++)
                {
                    if (prefabEditors[i] != null)
                    {
                        Reflected.SetInspectorMode(prefabEditors[i], inspectorMode);
                    }
                }
            }
            if (assetEditor != null)
            {
                InspectorMode inspectorMode = InspectorMode.Normal;
                if (value)
                {
                    inspectorMode = InspectorMode.Debug;
                }
                Reflected.SetInspectorMode(assetEditor, inspectorMode);
            }
            if (assetImportSettingsEditor != null)
            {
                InspectorMode inspectorMode = InspectorMode.Normal;
                if (value)
                {
                    inspectorMode = InspectorMode.Debug;
                }
                Reflected.SetInspectorMode(assetImportSettingsEditor, inspectorMode);
            }
            ResetAssetViewSize();
        }
        internal static string GetPrefabStageRootPath()
        {
#if UNITY_2021_2_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
            if (prefabStage != null && prefabStage.prefabContentsRoot != null)
            {
#if UNITY_2020_1_OR_NEWER
                return prefabStage.assetPath;
#else
                return prefabStage.prefabAssetPath;
#endif
            }
            return "";
        }

        internal static GameObject GetPrefabStageRoot()
        {
#if UNITY_2021_2_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
            if (prefabStage != null && prefabStage.prefabContentsRoot != null)
            {
                return prefabStage.prefabContentsRoot;
            }
            return null;
        }

        internal static bool SceneIsInPrefabMode()
        {
#if UNITY_2021_2_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
            if (prefabStage != null && prefabStage.prefabContentsRoot != null)
            {
                return true;
            }
            return false;
        }
        internal static bool AreAllGameObjectsInPrefabMode(GameObject[] gameObjects)
        {
            if (gameObjects == null || gameObjects.Length == 0)
            {
                return false;
            }
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (!IsGameObjectInPrefabMode(gameObjects[i]))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsGameObjectInPrefabMode(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
#if UNITY_2021_2_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
            if (prefabStage != null && prefabStage.scene == gameObject.scene)
            {
                return true;
            }
            return false;
        }
        internal static void DestroyAllIfNotNull(Editor[] editors)
        {
            if (editors != null)
            {
                for (int i = 0; i < editors.Length; i++)
                {
                    DestroyIfNotNull(editors[i]);
                }
            }
        }
        internal static void DestroyIfNotNull(Editor editor)
        {
            if (editor != null)
            {
                DestroyImmediate(editor);
            }
        }

        private void ReinitializePrefabComponentEditors()
        {
            if (prefabMaterialEditors != null)
            {
                for (int i = 0; i < prefabMaterialEditors.Length; i++)
                {
                    if (prefabMaterialEditors[i] != null)
                    {
                        DestroyImmediate(prefabMaterialEditors[i]);
                        prefabMaterialEditors[i] = null;
                    }
                }
                prefabMaterialEditors = null;
            }
            if (prefabEditors != null)
            {
                for (int i = 0; i < prefabEditors.Length; i++)
                {
                    if (prefabEditors[i] != null)
                    {
                        DestroyImmediate(prefabEditors[i]);
                    }
                }
            }
            if (targetObject != null && targetObject is GameObject && (targetObjects == null || targetObjects.Length == 0))
            {
                GameObject prefab = targetObject as GameObject;
                Component[] components = prefab.GetComponents<Component>();
                if (prefabEditors == null || prefabEditors.Length != components.Length)
                {
                    prefabEditors = new Editor[components.Length];
                }
                for (int i = 0; i < components.Length; i++)
                {
                    if (prefabEditors[i] == null || EditorUtility.InstanceIDToObject(prefabEditors[i].GetInstanceID()) == null)
                    {
                        if (prefabEditors[i] != null)
                        {
                            DestroyImmediate(prefabEditors[i]);
                        }
                        prefabEditors[i] = Editor.CreateEditor(components[i]);
                    }
                }
                prefabFoldouts_ = new bool[prefabEditors.Length];
                prefabFoldoutsChangeTracker_ = new bool[prefabEditors.Length];

                for (int i = 0; i < prefabEditors.Length; i++)
                {
                    prefabFoldouts_[i] = !collapsePrefabComponents;
                    prefabFoldoutsChangeTracker_[i] = !collapsePrefabComponents;
                }
            }
            else if (targetObjects != null && targetObjects.Length > 1 && (EditorUtils.AreAllTargetsPrefabs(targetObjects) || EditorUtils.AreAllTargetsImportedObjects(targetObjects)))
            {
                RebuildPrefabMultiComponentEditors();
            }
            else
            {
                prefabEditors = null;
                prefabFoldouts_ = null;
            }
            SetAllAssetEditorsDebugTo(debugAsset);
        }
        private void CleanAllTabMaps()
        {
            if (tabs != null)
            {
                for (int i = 0; i < tabs.Count; i++)
                {
                    tabs[i].DestroyAllMaterialMaps();
                    tabs[i].ResetCulling();
                }
            }
            PrefabMaterialMapManager.DestroyAllMaterialMaps();
        }
        private void ReinitializeComponentEditors()
        {
            ReinitializeComponentEditors(true);
        }
        internal void ReinitializeComponentEditors(bool skipGO = false)
        {
            CleanAllTabMaps();
            ReadLastClicked();
            if (!skipGO)
            {
                EditorApplication.delayCall += () =>
                {
                    if (gameObjectEditor != null)
                    {
                        if (!EditorApplication.isCompiling)
                        {
                            DestroyImmediate(gameObjectEditor);
                        }
                    }
                };
            }
            if (materialEditors != null)
            {
                if (materialEditors != null)
                {
                    foreach (var materialEditor in materialEditors)
                    {
                        if (materialEditor != null)
                        {
                            DestroyImmediate(materialEditor);
                        }
                    }
                    materialEditors = null;
                }
            }
            if (tabs != null && GetActiveTab().multiEditMode)
            {
                if (componentEditors != null)
                {
                    for (int i = 0; i < componentEditors.Length; i++)
                    {
                        if (componentEditors[i] != null)
                        {
                            DestroyImmediate(componentEditors[i]);
                        }
                    }
                }
                differentComponents = false;
                Dictionary<Type, List<List<Component>>> map = EditorUtils.OrderedComponentMap(GetActiveTab().targets, this);
                if (map == null)
                {
                    return;
                }
                List<Editor> editorList = new List<Editor>();
                foreach (var entry in map)
                {
                    List<List<Component>> componentLists = entry.Value;
                    int componentCount = componentLists[0].Count;
                    for (int i = 0; i < componentCount; i++)
                    {
                        Component[] editorTargets = componentLists.Select(list => list[i]).ToArray();
                        Editor editor;
                        if (editorTargets.Length > 1)
                        {
                            editor = Editor.CreateEditor(editorTargets);
                        }
                        else
                        {
                            editor = Editor.CreateEditor(editorTargets[0]);
                        }
                        editorList.Add(editor);
                    }
                }
                componentEditors = editorList.ToArray();
                componentFoldouts_ = new bool[componentEditors.Length];
                for (int i = 0; i < componentEditors.Length; i++)
                {
                    componentFoldouts_[i] = true;
                }
                SetAllEditorsDebugTo(GetActiveTab().debug);
                RefreshAllTabNames();
                RefreshAllIcons();
                UpdateAllTabPaths();
                Repaint();
                return;
            }


            if (componentEditors != null)
            {
                for (int i = 0; i < componentEditors.Length; i++)
                {
                    if (componentEditors[i] != null)
                    {
                        DestroyImmediate(componentEditors[i]);
                    }
                }
                componentFoldouts_ = null;
            }
            if (GetActiveTab().target != null)
            {
                Component[] components = GetActiveTab().target.GetComponents<Component>();
                if (componentEditors == null || componentEditors.Length != components.Length)
                {
                    componentEditors = new Editor[components.Length];
                }
                for (int i = 0; i < components.Length; i++)
                {
                    if (componentEditors[i] == null || EditorUtility.InstanceIDToObject(componentEditors[i].GetInstanceID()) == null)
                    {
                        if (componentEditors[i] != null)
                        {
                            DestroyImmediate(componentEditors[i]);
                        }
                        componentEditors[i] = Editor.CreateEditor(components[i]);
                    }
                }
            }
            else
            {
                componentEditors = null;
                componentFoldouts_ = null;
            }
            SetAllEditorsDebugTo(GetActiveTab().debug);
            RefreshAllTabNames();
            RefreshAllIcons();
            UpdateAllTabPaths();
            Repaint();
        }
        bool LastChangeWasNotNow()
        {
            if (Time.realtimeSinceStartup - lastChangeOfState > 0.5f)
            {
                return true;
            }
            return false;
        }
        internal static void FixSaveDataReferences()
        {
            if (MainCoInspector)
            {
                if (MainCoInspector.settingsData && AssetDatabase.GetAssetPath(MainCoInspector.settingsData) != "")
                {

                }
                else
                {
                    MainCoInspector.settingsData = null;
                    MainCoInspector.AutoCreateSettings();
                }
            }

        }
        internal void SaveSettings()
        {
            FixSaveDataReferences();
            if (settingsData == null)
            {
                settingsData = AutoCreateSettings();
            }
            if (settingsData)
            {
                if (!InRecoverScreen())
                {
                    settingsData.SaveData(false, this);
                }
                else
                {
                    settingsData.SaveData(false);
                }
            }
        }
        internal Dictionary<Type, List<UnityObject>> SortedAssetSelection { get => sortedAssetSelection; set => sortedAssetSelection = value; }
        public bool IsAlreadySelected(UnityObject[] array2)
        {
            UnityObject[] array1 = Selection.objects;

            if (array1 == null || array2 == null)
            {
                return false;
            }

            var set1 = new HashSet<int>(array1.Where(obj => obj != null).Select(obj => obj.GetInstanceID()));
            var set2 = new HashSet<int>(array2.Where(obj => obj != null).Select(obj => obj.GetInstanceID()));

            return set1.SetEquals(set2);
        }
        void SelectIfNotAlready(GameObject[] array, bool ignoreAfter = false, bool forceAfter = false)
        {
            if (array == null || array.Length == 0)
            {
                return;
            }
            if (!IsAlreadySelected(array))
            {
                EditorToolCache.RestorePreviousPersistentTool();
                nextSelection = array;
                if (ignoreAfter)
                {
                    ignoreSelection = array;
                }
                if (forceAfter)
                {
                    forceSelection = true;
                    Selection.objects = array;
                }
                else
                {
                    Selection.objects = array;
                }
                RepaintForAWhile();
            }
        }
        UnityObject[] nextSelection;
        void SetSelected()
        {
            UnityObject[] array = nextSelection;
            if (array == null || array.Length == 0)
            {
                return;
            }
            Selection.objects = array;
        }
        void FrameObjectsBounds(GameObject[] gameObjects, SceneView sceneView)
        {
            if (gameObjects == null || gameObjects.Length == 0)
            {
                return;
            }
            Bounds? totalBounds = null;
            foreach (GameObject go in gameObjects)
            {
                if (go == null)
                {
                    continue;
                }
                Collider collider = go.GetComponent<Collider>();
                if (collider != null)
                {
                    if (totalBounds == null)
                    {
                        totalBounds = collider.bounds;
                    }
                    else
                    {
                        totalBounds.Value.Encapsulate(collider.bounds);
                    }
                    continue;
                }

                Renderer renderer = go.GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (totalBounds == null)
                    {
                        totalBounds = renderer.bounds;
                    }
                    else
                    {
                        totalBounds.Value.Encapsulate(renderer.bounds);
                    }
                }
            }
            if (totalBounds.HasValue)
            {
                sceneView.Frame(totalBounds.Value, false);
            }
            else
            {
                sceneView.LookAt(MiddlePoint(gameObjects), sceneView.camera.transform.rotation, 10);
            }
        }
        void FrameObjectBounds(GameObject gameObject, SceneView sceneView)
        {
            if (gameObject == null)
            {
                return;
            }
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                sceneView.Frame(collider.bounds, false);
                return;
            }
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                sceneView.Frame(renderer.bounds, false);
                return;
            }
            sceneView.LookAt(gameObject.transform.position, sceneView.camera.transform.rotation, 10);
        }
        void HandleSelectionChange()
        {
            if (ignoreNextSelection)
            {
                ignoreNextSelection = false;
                return;
            }
            if (Selection.objects == null || Selection.objects.Length == 0)
            {
                return;
            }
            //Debug.Log("Selection changed on " + Time.realtimeSinceStartup);
            if ((ignoreSelection != null && IsAlreadySelected(ignoreSelection)) || !LastChangeWasNotNow())
            {
                if (switchingTabs)
                {
                    switchingTabs = false;
                    Undo.PerformRedo();
                }
                ignoreSelection = null;
                return;
            }
            List<GameObject> realGameObjects = new List<GameObject>();
            List<UnityEngine.Object> realAssets = new List<UnityEngine.Object>();
            foreach (var obj in Selection.objects)
            {
                if (obj is GameObject gameObject && !EditorUtils.IsAPrefabAsset(gameObject) && !EditorUtils.IsAnImportedObject(gameObject))
                {
                    realGameObjects.Add(gameObject);
                }
                else
                {
                    realAssets.Add(obj);
                }
            }
            GameObject[] gameArray = realGameObjects.ToArray();
            UnityEngine.Object[] assetArray = realAssets.ToArray();
            if (gameArray.Length > 0)
            {
                if (tabs?.Count == 0)
                {
                    activeIndex = 0;
                    tabs.Insert(0, new TabInfo(null, 0, this, ""));
                    tabs[0].newTab = true;
                }
                bool startingPrefabMode = !onPrefabSceneMode && IsGameObjectInPrefabMode(gameArray[0]);
                if (realGameObjects.Count == 1 && (startingPrefabMode || !IsActiveTabLocked()))
                {
                    SetTargetGameObject(gameArray[0]);
                }
                else if (!IsActiveTabLocked())
                {
                    SetTargetGameObjects(gameArray);
                }
            }
            if (assetArray.Length > 0 && !lockedAsset && assetInspection && !EditorUtils.AssetsAlreadyTargets(assetArray, this))
            {
                if (targetObject == null && (targetObjects == null || targetObjects.Length == 0))
                {
                    lockedAsset = false;
                }
                if (assetArray.Length == 1)
                {
                    SetTargetAsset(assetArray[0]);
                }
                else
                {
                    SetTargetAssets(assetArray);
                }
            }
        }

        bool IsAssetImporterEditor(Editor _editor)
        {
            bool isIt = false;
#if UNITY_2020_2_OR_NEWER
            if (_editor is UnityEditor.AssetImporters.AssetImporterEditor)
            {
                isIt = true;
            }
#else
            if (_editor is UnityEditor.Experimental.AssetImporters.AssetImporterEditor)
            {
                isIt = true;
            }
#endif
            return isIt;
        }

        bool IsEditorValid(Editor editor)
        {
            if (editor == null)
            {
                return false;
            }
            if (editor.targets == null)
            {
                return false;
            }
            else
            if (editor.target != null || (editor.targets != null && editor.targets.Length > 0))
            {
                if (IsAssetImporterEditor(editor))
                {
                    if (assetImporter == null && assetImporters == null)
                    {
                        return false;
                    }
                    if (editor.targets.Length == 1 && assetImporter != editor.target)
                    {
                        return false;
                    }
                    else if (editor.targets.Length > 1 && assetImporters != editor.targets)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        void ReinitializeAssetEditors()
        {
            bool wasLocked = lockedAsset;
            lockedAsset = false;
            if (targetObject && (targetObjects == null || targetObjects.Length == 0))
            {
                ResetAssetInspector();
            }
            else if (targetObjects != null && targetObjects.Length > 1)
            {
                ResetMultiAssetEditors();
                targetObject = null;
            }
            lockedAsset = wasLocked;
        }

        public void SetTargetAssets(UnityEngine.Object[] objects, bool overrideLock = false)
        {
            if (objects != null && objects.Length == 1)
            {
                bool locked = lockedAsset;
                lockedAsset = false;
                targetObjects = null;
                SetTargetAsset(objects[0]);
                EditorApplication.delayCall += () =>
                {
                    lockedAsset = locked;
                    //  Debug.Log("Going back to single object");
                };
                return;
            }
            else if (objects == null || objects.Length == 0)
            {
                CloseAssetView();
                return;
            }
            runtimePrefabComponents = null;
            bool _override = overrideLock && targetObjects != null && targetObjects.Length > 0;
            if (!_override)
            {
                if (lockedAsset && targetObjects != null && targetObjects.Length > 0)
                {
                    return;
                }
            }
            Dictionary<Type, List<UnityEngine.Object>> objectsByType = new Dictionary<Type, List<UnityEngine.Object>>();
            List<UnityEngine.Object> folders = new List<UnityEngine.Object>();
            List<UnityEngine.Object> importedObjects = new List<UnityEngine.Object>();
            List<UnityEngine.Object> finalAssets = new List<UnityEngine.Object>();
            alreadyCalculatedHeight = false;
            foreach (UnityEngine.Object obj in objects)
            {
                if (!obj || obj is GameObject && !EditorUtils.IsAPrefabAsset(obj))
                {
                    if (EditorUtils.IsAnImportedObject(obj))
                    {
                        finalAssets.Add(obj);
                        importedObjects.Add(obj);
                    }
                    continue;
                }
                else
                {
                    finalAssets.Add(obj);
                }
                if (PoolCache.IsAssetAFolder(obj))
                {
                    folders.Add(obj);
                    continue;
                }
                Type objType = obj.GetType();
                if (!objectsByType.ContainsKey(objType))
                {
                    objectsByType[objType] = new List<UnityEngine.Object>();
                }
                objectsByType[objType].Add(obj);
            }
            if (folders.Count > 0)
            {
                if (objectsByType.ContainsKey(typeof(UnityObject)))
                {
                    if (!objectsByType.ContainsKey(typeof(DefaultAsset)))
                    {
                        objectsByType.Add(typeof(DefaultAsset), folders);
                    }
                    else
                    {
                        objectsByType[typeof(DefaultAsset)].AddRange(folders);
                    }
                }
                else
                {
                    objectsByType.Add(typeof(UnityObject), folders);
                }
            }
            if (importedObjects.Count > 0)
            {
                if (objectsByType.ContainsKey(typeof(UnityObject)))
                {
                    if (!objectsByType.ContainsKey(typeof(GameObject)))
                    {
                        objectsByType.Add(typeof(GameObject), importedObjects);
                    }
                    else
                    {
                        objectsByType[typeof(GameObject)].AddRange(importedObjects);
                    }
                }
                else
                {
                    objectsByType.Add(typeof(UnityObject), importedObjects);
                }
            }
            var sortedByCount = objectsByType.OrderByDescending(x => x.Value.Count);
            objectsByType = sortedByCount.ToDictionary(x => x.Key, x => x.Value);
            SortedAssetSelection = objectsByType;
            targetObjects = finalAssets.ToArray();
            if (targetObjects != null && targetObjects.Length == 1)
            {
                bool locked = lockedAsset;
                lockedAsset = false;
                targetObjects = null;
                SetTargetAsset(finalAssets[0]);
                EditorApplication.delayCall += () =>
                {
                    lockedAsset = locked;
                    // Debug.Log("Going back to single object");
                };
                return;
            }
            else if (finalAssets == null || finalAssets.Count == 0)
            {
                targetObjects = null;
                ResetMultiAssetEditors();
                return;
            }
            PrefabMaterialMapManager.DestroyAllMaterialMaps();
            targetObject = null;
            HandleAssetHistory(targetObjects);
            ResetMultiAssetEditors();
            StringBuilder summary = new StringBuilder();
            summary.AppendLine("Found:");
            foreach (var type in objectsByType.Keys)
            {
                int count = objectsByType[type].Count;
                summary.AppendLine($"{count} {type.Name}(s)");
            }
            alreadyCalculatedHeight = false;
        }
        void ResetMultiAssetEditors()
        {
            CleanAllAssetEditors();
            if (SortedAssetSelection == null || SortedAssetSelection.Count == 0)
            {
                if (targetObjects == null || targetObjects.Length == 0)
                {
                    return;
                }
                else
                {
                    SetTargetAssets(targetObjects);
                }/*
                if (SortedAssetSelection != null && SortedAssetSelection.Count > 0)
                {
                    Debug.Log("Rebuilt multi asset editors");
                }*/
            }
            else if (SortedAssetSelection.Count == 1)
            {
                //Debug.Log("Created a multi editor of type " + SortedAssetSelection.Keys.ElementAt(0));
                var targets = SortedAssetSelection.Values.ElementAt(0).ToArray();
                assetEditor = Editor.CreateEditor(targets);
                AssetInfo assetInfo = PoolCache.GetAssetInfo(targets[0]);
                assetImporters = new AssetImporter[targets.Length];
                bool isPrefab = targets[0] is GameObject;
                bool isImportedObject = assetInfo.isImportedObject;
                bool isNested = !assetInfo.isMainAsset;
                HashSet<string> seenPaths = new HashSet<string>();
                assetImporters = targets
                    .Select(t => AssetDatabase.GetAssetPath(t))
                    .Where(path => seenPaths.Add(path))
                    .Select(path => AssetImporter.GetAtPath(path))
                    .ToArray();
                if (assetImporters != null && assetImporters.Length == targets.Length && (!isPrefab || (isImportedObject && !isNested)))
                {
                    DestroyIfNotNull(assetImportSettingsEditor);
                    Editor temp = Editor.CreateEditor(assetImporters);
                    if (IsAssetImporterEditor(temp))
                    {
#if UNITY_2020_2_OR_NEWER
                        assetImportSettingsEditor = temp as UnityEditor.AssetImporters.AssetImporterEditor;
#else
                        assetImportSettingsEditor = temp as UnityEditor.Experimental.AssetImporters.AssetImporterEditor;
#endif
                        if (assetImportSettingsEditor == null)
                        {
                            DestroyImmediate(temp);
                        }
                        Reflected.UpdateCurrentApplyRevertMethod(assetImportSettingsEditor, assetEditor);
                    }
                    else
                    {
                        DestroyImmediate(temp);
                    }
                }
                if (isPrefab || isImportedObject)
                {
                    ReinitializePrefabComponentEditors();
                }
            }
            else
            {
                assetEditor = Editor.CreateEditor(targetObjects[0]);
            }

            Reflected.GatherTimeControl(assetEditor);
            SetAllAssetEditorsDebugTo(debugAsset);
            Repaint();
        }
        void DrawMultiAssetSummary()
        {
            if (SortedAssetSelection == null || SortedAssetSelection.Count == 0 || targetObjects == null || targetObjects.Length == 0)
            {
                return;
            }
            EditorGUIUtility.wideMode = true;
            DrawAssetBar(false, false, false, true);
            if (!assetsCollapsed)
            {
                if (targetObjects == null || targetObjects.Length == 0)
                {
                    return;
                }
                GUIStyle headerStyle = CustomGUIStyles.HeaderStyle;
                BeginDraggableAssetView();
                EditorGUILayout.BeginVertical();
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal(GUILayout.Height(50));
                GUILayout.Space(8);
                Rect textureRect = GUILayoutUtility.GetRect(40, 40, GUILayout.ExpandWidth(false));
                GUI.DrawTexture(textureRect, CustomGUIContents.MultiAsset.image);
                EditorGUILayout.LabelField(targetObjects.Length + " Objects", headerStyle);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                Rect rect1 = GUILayoutUtility.GetLastRect();
                EditorUtils.DrawLineUnderRect(rect1, CustomColors.HardShadow);
                EditorUtils.DrawLineUnderRect(rect1, CustomColors.SoftShadow, 0, 4);
                EditorUtils.DrawLineOverRect(rect1, CustomColors.HardShadow);
                EditorUtils.DrawLineOverRect(rect1, -1);
                EditorGUI.DrawRect(rect1, CustomColors.SubtleBright);
                rect1.y += rect1.height;
                rect1.height = this.position.height - rect1.y;
                EditorGUI.DrawRect(rect1, CustomColors.SoftShadow);
                GUILayout.Space(2);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.LabelField("Narrow the Selection:", EditorStyles.label);
                GUIContent buttonContent = CustomGUIContents.EmptyContent;
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
                for (int i = 0; i < SortedAssetSelection.Keys.Count; i++)
                {
                    var type = SortedAssetSelection.Keys.ElementAt(i);
                    buttonContent = CustomGUIContents.AssetContent(SortedAssetSelection, i);
                    GUIStyle buttonsStyle = CustomGUIStyles.MultiAssetButtonsStyle(position.width);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    if (GUILayout.Button(buttonContent, buttonsStyle, GUILayout.MaxHeight(20)))
                    {
                        bool lockWas = lockedAsset;
                        lockedAsset = false;
                        Selection.objects = SortedAssetSelection[type].ToArray();
                        EditorApplication.delayCall += () =>
                        {
                            lockedAsset = lockWas;
                        };
                    }
                    EditorGUILayout.EndHorizontal();
                    Rect rect = GUILayoutUtility.GetLastRect();
                    if (rect.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
                    }
                }
                GUILayout.Space(10);
                EndDraggableAssetView();
            }
        }
        void AdjustRawUserHeight()
        {

            bool hasPreviewGUI = assetEditor.HasPreviewGUI();
            rawUserHeight = userHeight;
            if (userHeight == 1)
            {
                assetsCollapsed = true;
                if (hasPreviewGUI)
                {
                    rawUserHeight -= GetPreviewHeight();

                }
            }
            else
            {
                assetsCollapsed = false;
                if (!ShouldDrawImportSettings() && hasPreviewGUI)
                {
                    if ((rawUserHeight + GetPreviewHeight()) > max)
                    {
                        userHeight = max - GetPreviewHeight();
                        rawUserHeight = userHeight;
                        suggestedHeight = userHeight;
                    }

                }
                else if (rawUserHeight + GetPreviewHeight() > previewRect.height && hasPreviewGUI && ShouldDrawImportSettings())
                {
                    userHeight = previewRect.height - GetPreviewHeight();
                    rawUserHeight = userHeight;
                    suggestedHeight = userHeight;
                }
            }
            if (maximizedAssetView)
            {
                userHeight = position.height - 150;
                rawUserHeight = userHeight;
                suggestedHeight = userHeight;
                return;
            }

        }
        void AutoCalculateMultiAssetHeight()
        {
            if (assetsCollapsed)
            {
                return;
            }
            if (alreadyCalculatedHeight)
            {
                return;
            }
            float height = GUILayoutUtility.GetLastRect().height;
            if (height > 1 && height != previewRect.height)
            {
                if (height > position.height / 1.5f)
                {
                    height = position.height / 1.5f;
                }
                if (height <= 4)
                {
                    height = 1;
                }
                previewRect.height = height;
                userHeight = height;
                suggestedHeight = height;
                alreadyCalculatedHeight = true;
                AdjustRawUserHeight();
                Repaint();
            }
        }
        void BeginDraggableAssetView()
        {
            float height = 1;
            if (previewRect != null && previewRect.height != 1)
            {
                height = userHeight;
            }
            if (assetEditor != null && assetEditor.HasPreviewGUI())
            {
                height = userHeight;
            }
            if (alreadyCalculatedHeight)
            {
                lastKnownHeight = height;
            }
            GUILayoutOption[] options = new GUILayoutOption[1] { GUILayout.Height(userHeight) };
            if (lastKnownHeight <= 50)
            {
                assetScrollPosition = EditorGUILayout.BeginScrollView(assetScrollPosition, GUIStyle.none, GUIStyle.none, options);
            }
            else
            {

                assetScrollPosition = EditorGUILayout.BeginScrollView(assetScrollPosition, options);
            }
            EditorGUILayout.BeginVertical();
        }
        void FixNullAssets()
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                return;
            }
            bool nulls = false;
            for (int i = 0; i < targetObjects.Length; i++)
            {
                if (targetObjects[i] == null)
                {
                    nulls = true;
                }
            }
            if (nulls)
            {
                SetTargetAssets(targetObjects);
                targetObjects = null;
            }
        }
        void LimitAssetViewHeight()
        {
            if (assetEditor == null)
            {
                maximizedAssetView = false;
                return;
            }
            bool isSingleAsset = assetEditor.targets != null && assetEditor.targets.Length == 1;
            if (maximizedAssetView)
            {
                userHeight = position.height - 80 - currentPreviewHeight;
                if (!assetEditor.HasPreviewGUI() && isSingleAsset)
                {
                    userHeight += currentPreviewHeight;
                }

                else if (showAssetLabels && assetEditor.HasPreviewGUI())
                {
                    userHeight -= 60;
                }
                return;
            }

            if (userHeight > previewRect.height)
            {
                userHeight = previewRect.height;
            }

            if (userHeight > max)
            {
                userHeight = max;
            }
            if (ShouldDrawImportSettings() && assetEditor.HasPreviewGUI() && userHeight + 123 > max)
            {
                userHeight = max - 123;
            }
            /*
            suggestedHeight = previewRect.height;
            if (suggestedHeight > max )
            {
                suggestedHeight = max;
            }           */
            if (userHeight < 1)
            {
                userHeight = 1;
            }
        }
        void EndDraggableAssetView()
        {
            if (maximizedAssetView)
            {
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.EndVertical();
            bool prefabMode = targetObject != null && targetObject is GameObject && IsGameObjectInPrefabMode(targetObject as GameObject);
            Rect rect1 = GUILayoutUtility.GetLastRect();
            rect1.y += rect1.height - 2;
            rect1.height = this.position.height - rect1.y;
            EditorGUI.DrawRect(rect1, CustomColors.SoftShadow);
            AutoCalculatePreviewHeight(prefabMode);
            EditorGUILayout.EndScrollView();
            if (ShouldDrawImportSettings())
            {
                EditorUtils.DrawLineUnderRect(CustomColors.HardShadow, -1, 1);
            }
        }
        void DrawMultiAssetHeader()
        {
            EditorGUIUtility.wideMode = true;
            if (SortedAssetSelection == null || SortedAssetSelection.Count == 0)
            {
                return;
            }
            if (SortedAssetSelection.Count == 1)
            {
                DrawMultiAssetSameTypeEditors();
                DrawAssetLabelGUI(targetObjects);
                DrawAssetBottomBar(false, true);
            }
            else
            {
                DrawMultiAssetSummary();
                DrawAssetBottomBar(true, true);
            }
        }

        void DrawMultiAssetSameTypeEditors()
        {
            if (assetEditor == null || targetObjects == null || targetObjects.Length < 2)
            {
                return;
            }
            AssetInfo assetInfo = PoolCache.GetAssetInfo(targetObjects[0]);
            bool isPrefab = assetInfo.isPrefab;
            bool importedObjectMode = assetInfo.isImportedObject;
            bool brokenPrefabs = false;
            if (isPrefab)
            {
                brokenPrefabs = assetInfo.isBrokenPrefab;
            }
            bool isFolder = assetInfo.isFolder;
            DrawAssetBar(isFolder, isPrefab, true, true);
            if (!assetsCollapsed)
            {
                BeginDraggableAssetView();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginVertical();
                if (assetImportSettingsEditor && ShouldDrawImportSettings())
                {
                    assetImportSettingsEditor.DrawHeader();
                    if (importedObjectMode)
                    {
                        EditorGUILayout.BeginVertical();
                        assetImportSettingsEditor.OnInspectorGUI();
                        EditorGUILayout.Space();
                        EditorGUILayout.EndVertical();
                    }
                }
                else if (!assetEditor.HasPreviewGUI() || (targetObjects != null && targetObjects[0] is Material))
                {
                    assetEditor.DrawHeader();
                }
                EditorGUILayout.EndVertical();
                if ((isPrefab && !brokenPrefabs) || importedObjectMode)
                {
                    assetEditor.DrawHeader();
                    DrawMultiPrefabInspector();
                    EditorGUILayout.EndVertical();
                    EndDraggableAssetView();
                    DynamicResizePreview();
                    return;
                }
                if (brokenPrefabs)
                {
                    EditorUtils.DrawBrokenPrefabMessage(true);
                }
                if (assetImportSettingsEditor != null && ShouldDrawImportSettings())
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUI.indentLevel = 1;

                    assetImportSettingsEditor.OnInspectorGUI();

                    if (debugAsset)
                    {
                        EditorGUILayout.Space();
                        assetEditor.DrawHeader();
                        assetEditor.DrawDefaultInspector();
                    }
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }
                else if (assetEditor != null && ShouldDrawImportSettings())
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUI.indentLevel = 1;
                    if (!debugAsset)
                    {
                        assetEditor.OnInspectorGUI();
                    }
                    else
                    {
                        assetEditor.DrawDefaultInspector();
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                EndDraggableAssetView();
                if (assetEditor.HasPreviewGUI())
                {
                    DynamicResizePreview();
                    EditorUtils.DrawLineOverRect();
                    EditorUtils.DrawLineOverRect(CustomColors.SoftShadow, 3, 3);
                }
            }
        }
        public void SetTargetAsset(UnityObject ob, bool overrideLock = false)
        {
            bool _override = ob != null && overrideLock && assetInspection;
            if (!_override)
            {
                if (ob == null || lockedAsset || !assetInspection)
                {
                    if (assetEditor == null && lockedAsset)
                    {
                        lockedAsset = false;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            if (!CheckForApplyRevertOnClose())
            {
                return;
            }
            if (ignoreFolders && PoolCache.IsAssetAFolder(ob))
            {
                return;
            }
            HandleAssetHistory(new UnityObject[] { ob });
            targetObjects = null;
            {
                PrefabMaterialMapManager.DestroyAllMaterialMaps();
                runtimePrefabComponents = null;
                targetObject = ob;
                ReinitializePrefabComponentEditors();
                ResetAssetInspector(EditorUtils.IsAPrefabAsset(targetObject));
                ResetAssetViewSize();
                EditorApplication.delayCall += Repaint;
            }
        }

        void SaveCurrentTargetOrTargets()
        {
            if (targetObject != null)
            {
                EditorUtils.SaveAsset(targetObject, assetImportSettingsEditor);
            }
            else if (targetObjects != null && targetObjects.Length > 0)
            {
                EditorUtils.SaveAssets(targetObjects, assetImportSettingsEditor);
            }
            Repaint();
        }
        internal void ResetAssetViewSize(bool keepCollapsed = false)
        {
            if (!assetsCollapsed)
            {
                alreadyCalculatedHeight = false;
                if (userHeight != suggestedHeight)
                {
                    userHeight = suggestedHeight;
                }
            }
            ReloadPreview();
            RepaintForAWhile();
        }
        void AutoCalculatePreviewHeight(bool prefabMode = false)
        {
            if (assetsCollapsed || resizingAssetView)
            {
                return;
            }
            if (alreadyCalculatedHeight)
            {
                return;
            }
            float _height = GUILayoutUtility.GetLastRect().height;
            if (_height == 1)
            {
                return;
            }
            int previewHeight = 0;
            if (assetEditor != null && assetEditor.HasPreviewGUI() && ShouldDrawImportSettings() && !prefabMode)
            {
                previewHeight = IntGetPreviewHeight();

            }
            if (IsItAMultiPrefabTarget())
            {
                previewHeight = IntGetPreviewHeight();
            }
            if (_height + previewHeight > max)
            {
                _height = max - previewHeight;

            }
            if (_height <= 4)
            {
                _height = 1;
            }
            userHeight = _height;
            rawUserHeight = _height;
            suggestedHeight = _height;
            previewRect.height = _height;
            if (assetEditor != null && assetEditor.HasPreviewGUI() && !ShouldDrawImportSettings() && (targetObjects == null || targetObjects.Length == 0))
            {
                previewRect.height = max;
            }
            if (prefabMode && targetObject != null)
            {
                previewRect.height = max - previewHeight;

            }
            alreadyCalculatedHeight = true;
            RepaintForAWhile();
        }

        bool ShouldDrawImportSettings()
        {
            if (assetEditor == null)
            {
                return false;
            }
            if (showImportSettings || maximizedAssetView)
            {
                return true;
            }
            return false;
        }
        void ForceCollapseOrDefault(int forceSpecific = 0)
        {
            if (forceSpecific == 0)
            {
                if (maximizedAssetView)
                {
                    maximizedAssetView = false;
                    assetsCollapsed = true;
                    userHeight = 1;

                }
                if (!assetsCollapsed)
                {
                    userHeight = 1;
                    rawUserHeight = userHeight;
                    assetsCollapsed = true;
                    maximizedAssetView = false;
                    if (assetEditor.HasPreviewGUI())
                    {
                        rawUserHeight -= IntGetPreviewHeight();
                    }
                }
                else
                {
                    assetsCollapsed = false;
                    alreadyCalculatedHeight = false;
                }
                lastKnownHeight = rawUserHeight;
            }
            else if (forceSpecific == 1)
            {
                userHeight = suggestedHeight;
                rawUserHeight = userHeight;
            }
            else if (forceSpecific == 2)
            {

                userHeight = 1;
                rawUserHeight = userHeight;
                if (assetEditor != null)
                {
                    rawUserHeight -= IntGetPreviewHeight();
                }
            }
            RepaintForAWhile();
        }
        void DynamicResizePreview()
        {
            if (!assetEditor || !assetEditor.HasPreviewGUI())
            {
                currentPreviewHeight = 0;
                return;
            }
            float prevHeight = IntGetPreviewHeight();
            if (rawUserHeight < 0)
            {
                prevHeight = IntGetPreviewHeight() + rawUserHeight;
            }
            if (prevHeight < 5)
            {
                prevHeight = 1;
            }
            Rect rect = GUILayoutUtility.GetRect(IntGetPreviewHeight(), prevHeight);
            rect.x = 0;
            EditorGUI.DrawRect(rect, CustomColors.DarkGray);
            if (rect.y == 0 && rect.x == 0)
            {
                return;
            }
            if (prevHeight > 70)
            {
                if (rect.height < 1)
                {
                    rect.height = IntGetPreviewHeight();
                }
                if (rect.width <= 1)
                {
                    rect.width = position.width;
                }
                if (rect.height <= 1 || rect.width <= 1)
                {
                    return;
                }
                GUI.enabled = true;
                DrawPreview(rect);

            }
        }

        void ReloadPreview()
        {
            if (assetEditor == null)
            {
                return;
            }
            if (assetEditor.HasPreviewGUI())
            {
                assetEditor.ReloadPreviewInstances();
            }
            else if (assetImportSettingsEditor != null)
            {
                assetImportSettingsEditor.ReloadPreviewInstances();
            }
        }
        void DrawPreview(Rect rect)
        {
            bool videoClip = targetObject is VideoClip;
            if (assetEditor != null && !videoClip)
            {
                EditorGUI.BeginChangeCheck();
                assetEditor.DrawPreview(rect);

                if (EditorGUI.EndChangeCheck())
                {
                    Repaint();
                }
            }
            else if (videoClip && assetImportSettingsEditor != null)
            {
                EditorGUI.BeginChangeCheck();
                assetImportSettingsEditor.DrawPreview(rect);
                if (EditorGUI.EndChangeCheck())
                {
                    Repaint();
                }
            }
            float height = GUILayoutUtility.GetLastRect().height;
            currentPreviewHeight = height;
        }
        void ResetAssetInspector(bool isPrefab = false)
        {
            if (assetEditor != null)
            {
                ResetAssetViewSize();
                DestroyImmediate(assetEditor);
            }
            if (prefabMaterialEditors != null)
            {
                if (prefabMaterialEditors != null)
                {
                    foreach (var editor in prefabMaterialEditors)
                    {
                        DestroyImmediate(editor);
                    }
                    prefabMaterialEditors = null;
                }
            }
            if (assetImportSettingsEditor != null)
            {
                DestroyImmediate(assetImportSettingsEditor);
            }
            if (assetImporter != null)
            {
                assetImporter = null;
            }
            if (isPrefab)
            {
                ResetAssetViewSize();
                GameObject prefab = targetObject as GameObject;
                assetEditor = Editor.CreateEditor(targetObject);
                /*if (assetEditor != null)
                {
                    Debug.Log("Created new prefab editor");
                }*/
                if (assetImporter == null || assetImporter.assetPath != AssetDatabase.GetAssetPath(targetObject))
                {
                    if (assetImportSettingsEditor != null)
                    {
                        DestroyImmediate(assetImportSettingsEditor);
                    }
                    assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(targetObject));
                }
                Repaint();
                return;
            }
            assetEditor = Editor.CreateEditor(targetObject);
            if (assetEditor && EditorUtils.IsMainAsset(targetObject))
            {
                if (assetImporter == null || assetImporter.assetPath != AssetDatabase.GetAssetPath(targetObject))
                {
                    if (assetImportSettingsEditor != null)
                    {
                        DestroyImmediate(assetImportSettingsEditor);
                    }
                    assetImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(targetObject));
                    if (assetImporter != null)
                    {
                        Editor _editor = Editor.CreateEditor(assetImporter);
                        if (IsAssetImporterEditor(_editor))
                        {
#if UNITY_2020_2_OR_NEWER
                            assetImportSettingsEditor = _editor as UnityEditor.AssetImporters.AssetImporterEditor;
#else
                            assetImportSettingsEditor = _editor as UnityEditor.Experimental.AssetImporters.AssetImporterEditor;
#endif                          
                            if (assetImportSettingsEditor == null)
                            {
                                DestroyImmediate(_editor);
                            }
                            Reflected.UpdateCurrentApplyRevertMethod(assetImportSettingsEditor, assetEditor);
                        }
                        else
                        {
                            DestroyImmediate(_editor);
                        }
                    }
                }
                if (targetObject is Material)
                {
                    UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(targetObject, ShouldDrawImportSettings());
                }
            }
            Reflected.GatherTimeControl(assetEditor);
            SetAllAssetEditorsDebugTo(debugAsset);
            Repaint();
        }
        static bool EditorBefore2021_2()
        {
            bool version = true;
#if UNITY_2021_2_OR_NEWER
            version = false;
#endif
            return version;
        }
        void DoOpenPrefabInScene(string path, GameObject item)
        {
            if (path == "")
            {
                return;
            }
#if UNITY_2021_2_OR_NEWER
            if (item == null)
            {
                return;
            }
            UnityEditor.SceneManagement.PrefabStageUtility.OpenPrefab(path, item);
#else
            Reflected.OpenPrefab(path, item);
#endif
        }
        void DoOpenPrefab(string path)
        {
            if (path == "")
            {
                return;
            }
#if UNITY_2021_2_OR_NEWER
            UnityEditor.SceneManagement.PrefabStageUtility.OpenPrefab(path);
#else
            Reflected.OpenPrefab(path);
#endif
        }
        void OpenPrefab()
        {
            onPrefabSceneMode = false;
            FixActiveIndex();
            if (!assetsCollapsed)
            {
                if (maximizedAssetView)
                {
                    ResetMaximizedView();
                }
                ForceCollapseOrDefault(0);

            }
            DoOpenPrefab(AssetDatabase.GetAssetPath(targetObject));


            if (!openPrefabsInNewTab)
            {
                SetTargetGameObject(GetActiveTab().target as GameObject);
            }
        }

        bool IsInvisibleAssetEditor()
        {
            if (assetEditor == null || targetObject == null)
            {
                return false;
            }
            if (assetEditor.target == null)
            {
                return false;
            }
            return !EditorUtils.HasVisibleFields(assetEditor);
        }
        bool HandleMultiAssetNulls()
        {
            if (targetObject == null && targetObjects != null && targetObjects.Length > 0)
            {
                List<UnityObject> objects = new List<UnityObject>(targetObjects);
                foreach (var obj in targetObjects)
                {
                    if (obj == null)
                    {
                        objects.Remove(obj);
                    }
                }
                if (objects.Count == targetObjects.Length)
                {
                    return true;
                }
                if (objects.Count == 0)
                {
                    CloseAssetView();
                    return false;
                }
                UnityObject[] _objects = objects.ToArray();
                bool wasLocked = lockedAsset;
                lockedAsset = false;
                if (_objects.Length == 1)
                {
                    SetTargetAsset(_objects[0]);
                    Repaint();
                }
                else
                {
                    SetTargetAssets(_objects);
                    Repaint();
                }
                EditorApplication.delayCall += () =>
                {
                    lockedAsset = wasLocked;
                };
            }
            return false;
        }
        void DrawAssetInspector(bool addHistory = false)
        {
            UpdateLabelSize();

            if (targetObjects != null && targetObjects.Length > 1)
            {
                if (assetEditor == null || SortedAssetSelection == null || SortedAssetSelection.Count == 0)
                {
                    if (assetEditor == null)
                    {
                        Debug.LogWarning("Asset editor is null!");
                    }
                    ResetMultiAssetEditors();
                    targetObject = null;
                    targetObjects = null;
                }
                if (!HandleMultiAssetNulls())
                {
                    GUILayout.Space(userHeight);
                    Repaint();
                    return;
                }
                DrawMultiAssetHeader();
                return;
            }
            if (targetObject == null)
            {
                return;
            }
            AssetInfo assetInfo = PoolCache.GetAssetInfo(targetObject);
            bool isDirty = false;
            bool prefabMode = assetInfo.isPrefab || assetInfo.isImportedObject;
            if (assetEditor == null || assetEditor.target != targetObject)
            {
                ResetAssetInspector(prefabMode);
            }
            else
            {
                isDirty = IsAssetDirty(targetObject) || IsAssetImportSettingsDirty();
            }
            if (assetEditor != null)
            {
                bool hasPreviewGUI = assetEditor.HasPreviewGUI();
                /*
                if (assetImportSettingsEditor != null && showImportSettings)
                {
                    {
                        isDirty = true;
                    }
                }*/
                float height = 1;
                if (previewRect != null && previewRect.height != 1)
                {
                    height = userHeight;
                }

                if (hasPreviewGUI)
                {
                    height = userHeight;
                }
                bool isFolder = assetInfo.isFolder;
                DrawAssetBar(isFolder, prefabMode, isDirty, false);
                bool brokenPrefab = assetInfo.isBrokenPrefab;
                bool importedObjectMode = assetInfo.isImportedObject;
                bool alreadyDrawnDebugHeader = false;
                if (alreadyCalculatedHeight)
                {
                    lastKnownHeight = height;
                }
                if (!assetsCollapsed)
                {
                    GUILayoutOption[] options = CustomGUIStyles.GetUserHeightOptions(userHeight);
                    if (lastKnownHeight <= 50 || (!ShouldDrawImportSettings() && lastKnownHeight >= suggestedHeight && !prefabMode && suggestedHeight < previewRect.height))
                    {
                        assetScrollPosition = EditorGUILayout.BeginScrollView(assetScrollPosition, GUIStyle.none, GUIStyle.none, options);
                    }
                    else
                    {
                        assetScrollPosition = EditorGUILayout.BeginScrollView(assetScrollPosition, options);
                    }
                    // assetScrollPosition = EditorGUILayout.BeginScrollView(assetScrollPosition, GUILayout.MaxHeight(max));
                    EditorGUILayout.BeginVertical();
                    if (ShouldDrawImportSettings() && assetImportSettingsEditor != null && (!prefabMode || importedObjectMode))
                    {
                        EditorGUILayout.BeginVertical();
                        if (assetImportSettingsEditor)
                        {
                            assetImportSettingsEditor.DrawHeader();
                            assetImportSettingsEditor.OnInspectorGUI();
                            EditorGUILayout.Space();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    if (((isText(targetObject) || (ShouldDrawImportSettings() && assetImportSettingsEditor == null)) && !prefabMode) || targetObject is Material || IsInvisibleAssetEditor())
                    {
                        if (assetEditor != null)
                        {
                            EditorGUILayout.BeginVertical();
                            EditorGUI.BeginChangeCheck();
                            assetEditor.DrawHeader();
                            alreadyDrawnDebugHeader = true;
                            if (EditorGUI.EndChangeCheck())
                            {
                                ResetAssetViewSize();
                            }
                            if (targetObject is Mesh)
                            {
                                assetEditor.OnInspectorGUI();
                            }
                            EditorGUILayout.EndVertical();
                        }
                    }
                    if (prefabMode && targetObject)
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(expand: true));
                        assetEditor.DrawHeader();
                        EditorGUILayout.EndVertical();
                        if (brokenPrefab)
                        {
                            EditorUtils.DrawBrokenPrefabMessage();
                        }
                        else
                        {
                            DrawPrefabComponentInspectors(targetObject as GameObject);
                            if (!hasPreviewGUI)
                            {
                                GUI.color += CustomColors.SoftShadow;
                                GUILayout.Space(10);
                                GUI.color -= CustomColors.SoftShadow;
                            }

                        }
                    }
                    else
                    {
                        if (isText(targetObject))
                        {
                            if (rootVisualElement.childCount < 2)
                            {
                                while (rootVisualElement.childCount < 2)
                                {
                                    rootVisualElement.Add(new VisualElement());
                                }
                            }
                        }
                        EditorGUILayout.BeginVertical();
                        if (assetEditor != null)
                        {
                            EditorGUI.indentLevel = 1;
                            if (!debugAsset)
                            {
                                //TO AVOID MISLEADING TEXTURE PREVIEW SETTINGS
                                if (assetImportSettingsEditor == null || !(assetImportSettingsEditor.target is TextureImporter))
                                {
                                    if (!ShouldDrawImportSettings() && isText(targetObject))
                                    {
                                        GUILayout.Space(10);
                                    }
                                    if (!(targetObject is Mesh))
                                    {
                                        assetEditor.OnInspectorGUI();
                                    }
                                }
                            }
                            else if (debugAsset)
                            {
                                if (!alreadyDrawnDebugHeader && assetImportSettingsEditor != null && ShouldDrawImportSettings())
                                {
                                    assetEditor.DrawHeader();
                                }
                                EditorGUILayout.BeginVertical();

                                assetEditor.DrawDefaultInspector();
                                GUILayout.Space(10);
                                EditorGUILayout.EndVertical();
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                    if (assetEditor != null && hasPreviewGUI && (!ShouldDrawImportSettings() || prefabMode && !maximizedAssetView))
                    {
                        EditorGUILayout.BeginHorizontal();
                        float prevHeight = IntGetPreviewHeight();
                        if ((!ShouldDrawImportSettings() || prefabMode) && userHeight > suggestedHeight)
                        {
                            prevHeight = IntGetPreviewHeight() + userHeight - suggestedHeight;
                        }
                        GUI.enabled = true;
                        {
                            Rect rect = GUILayoutUtility.GetRect(IntGetPreviewHeight(), prevHeight);
                            rect.x = 0;
                            DrawPreview(rect);
                        }
                        EditorGUILayout.EndHorizontal();
                        if (ShouldDrawImportSettings() && prevHeight > 10)
                        {
                            EditorUtils.DrawLineOverRect(CustomColors.HardShadow);
                        }
                        EditorGUILayout.BeginVertical();
                        DrawAssetLabelGUI(targetObject);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndVertical();
                        AutoCalculatePreviewHeight(prefabMode);
                        EditorGUILayout.EndScrollView();
                        DrawAssetBottomBar(isFolder);
                    }
                    else if (assetEditor != null && hasPreviewGUI && ShouldDrawImportSettings())
                    {
                        EndDraggableAssetView();
                        DynamicResizePreview();
                        DrawAssetLabelGUI(targetObject);
                        DrawAssetBottomBar(isFolder);
                    }
                    else
                    {
                        if (targetObject is TextAsset || targetObject is Shader)
                        {
                            /*  DrawTextAssetPreview(targetObject);                  
                             float textHeight = GUILayoutUtility.GetLastRect().height;               
                             if (textHeight > 1)
                             {
                                 if (textHeight > 600)
                                 {
                                     textHeight = 600;
                                 }
                                 height2 += textHeight;                       
                             } */
                            DrawAssetLabelGUI(targetObject);
                            EditorGUILayout.EndVertical();
                            AutoCalculatePreviewHeight(prefabMode);
                            EditorGUILayout.EndScrollView();
                            DrawAssetBottomBar(isFolder);
                        }
                        else
                        {
                            DrawAssetLabelGUI(targetObject);
                            EditorGUILayout.EndVertical();
                            AutoCalculatePreviewHeight(prefabMode);
                            EditorGUILayout.EndScrollView();
                            DrawAssetBottomBar(isFolder);
                        }
                    }
                }
                else
                {
                    DrawAssetBottomBar(isFolder);
                }
            }
        }
        private void DrawAssetBundleNameGUI(UnityObject[] objects)
        {
            if (!showAssetLabels)
            {
                return;
            }
            if (objects == null || objects.Length == 0)
            {
                return;
            }
            if (Reflected.GetShowAssetBundleNameMethod() != null)
            {
                if (Reflected.assetBundleNameGUIInstance == null)
                {
                    Reflected.assetBundleNameGUIInstance = Activator.CreateInstance(Reflected.GetAssetBundleNameType());
                }
                if (Reflected.assetBundleNameGUIInstance != null)
                {
                    IEnumerable<UnityEngine.Object> assetsEnumerable = objects.Cast<UnityEngine.Object>();
                    EditorGUILayout.BeginHorizontal();
                    Reflected.GetShowAssetBundleNameMethod().Invoke(Reflected.assetBundleNameGUIInstance, new object[] { assetsEnumerable });
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    Debug.LogWarning("AssetBundleNameGUI instance is null");
                }
            }
        }
        private void DrawAssetLabelGUI(UnityObject _object)
        {
            if (!showAssetLabels)
            {
                return;
            }
            if (_object == null)
            {
                return;
            }
            DrawAssetLabelGUI(new UnityObject[] { _object });
        }
        private void DrawAssetLabelGUI(UnityObject[] _objects)
        {
            if (!showAssetLabels || assetsCollapsed)
            {
                return;
            }
            if (_objects == null || _objects.Length == 0)
            {
                return;
            }
            if (Reflected.GetShowAssetLabelMethod() != null)
            {
                if (Reflected.labelGUIInstance == null)
                {
                    Reflected.labelGUIInstance = Activator.CreateInstance(Reflected.GetAssetLabelsType());
                }
                if (Reflected.labelGUIInstance != null)
                {
                    if (maximizedAssetView)
                    {
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical(CustomGUIStyles.InspectorSectionStyle);
                    Reflected.GetShowAssetLabelMethod().Invoke(Reflected.labelGUIInstance, new object[] { _objects });
                    EditorUtils.DrawLineOverRect(CustomColors.DefaultInspector, 5, 5);
                    EditorUtils.DrawLineOverRect(CustomColors.SimpleShadow, 5);
                    EditorUtils.DrawLineOverRect(4);
                }
            }
            DrawAssetBundleNameGUI(_objects);
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        static bool IsAnyAPrefabAsset(GameObject[] gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                if (EditorUtils.IsAPrefabAsset(gameObject))
                {
                    return true;
                }
            }
            return false;
        }
        void DrawAssetBar(bool isFolder, bool prefabMode, bool isDirty, bool multiMode = false)
        {
            if (!targetObject && targetObjects == null)
            {
                return;
            }
            bool sameType = false;
            bool importedObjectMode = false;
            Color color = GUI.backgroundColor;
            if (EditorUtils.IsLightSkin())
            {
                GUI.backgroundColor += Color.white / 8;
            }
            GUIStyle toolbarButton = CustomGUIStyles.AssetToolbarButton;
            GUIContent content = CustomGUIContents.EmptyContent;
            GUIContent emptyButton = CustomGUIContents.NoneContent;
            string extension = "";
            bool isMainAsset;
            AssetInfo assetInfo = null;
            if (!multiMode)
            {
                assetInfo = PoolCache.GetAssetInfo(targetObject);
                isMainAsset = assetInfo.isMainAsset;
                if (isMainAsset)
                {
                    extension = assetInfo.extension;
                }
            }
            else
            {
                sameType = SortedAssetSelection.Count == 1;
                if (sameType)
                {
                    assetInfo = PoolCache.GetAssetInfo(targetObjects[0]);
                    string _extension = assetInfo.extension;
                    if (prefabMode && _extension == ".prefab")
                    {
                        extension = "Prefab";
                    }
                    else if (isFolder)
                    {
                        extension = "Folder";
                    }
                    else
                    {
                        string niceType = assetInfo.niceType;
                        if (assetInfo.isImportedObject)
                        {
                            niceType = "Imported Object";
                            importedObjectMode = true;
                        }
                        extension = niceType;
                    }
                }
                else
                {
                    extension = "Asset";
                }
            }
            string assetName;
            string modified = "";
            if (IsAssetImportSettingsDirty())
            {
                modified = "*";
            }
            if (!multiMode)
            {
                if (prefabMode && extension == ".prefab")
                {
                    assetName = " <b>" + targetObject.name + modified + " (Prefab)</b>";
                }
                else
                {
                    string optExtension = "";
                    if (targetObject is AudioClip || targetObject is TextAsset || targetObject is UnityEngine.Texture || targetObject is GameObject)
                    {
                        optExtension += extension;
                    }
                    string niceType = assetInfo.niceType;
                    if (prefabMode && extension != ".prefab")
                    {
                        niceType = "Imported Object";
                        importedObjectMode = true;
                    }
                    /*
                    if (EditorUtils.IsLightSkin())
                    {
                        assetName = targetObject.name + optExtension +  modified +"  (" + niceType + ")";
                    }
                    else*/
                    {
                        assetName = "<b>" + targetObject.name + optExtension + modified + " </b> (" + niceType + ")";
                    }
                }
            }
            else
            {
                assetName = "<b>Selecting " + targetObjects.Length + " " + extension;
            }
            if (extension == "")
            {
                assetName = assetName.Replace("Default Asset", "Folder");
            }
            string plural = multiMode ? "s" : "";
            assetName = assetName.Replace("UnityEngine.", "");
            content.text = assetName;
            if (multiMode)
            {
                if (assetName[assetName.Length - 1] != 's')
                {
                    content.text += plural + "</b>" + modified;
                }
            }
            if (EditorUtils.IsLightSkin())
            {
                content.text = content.text.Replace("<b>", "");
                content.text = content.text.Replace("</b>", "");
            }
            emptyButton.text = "";
            if (content.image == null)
            {
                if (!multiMode || sameType)
                {
                    if (prefabMode && !importedObjectMode)
                    {
                        content.image = CustomGUIContents.PrefabIcon.image;
                    }
                    else if (importedObjectMode)
                    {
                        content.image = CustomGUIContents.ImportedIcon.image;
                    }
                    else if (isFolder)
                    {
                        content.image = CustomGUIContents.FolderIcon.image;
                    }
                    else if (!multiMode)
                    {
                        content.image = assetInfo.icon;
                    }
                    else
                    {
                        content.image = AssetPreview.GetMiniTypeThumbnail(targetObjects[0].GetType());
                    }
                }
                else
                {
                    content.image = CustomGUIContents.TabMulti.image;
                }
            }
            if (maximizedAssetView)
            {
                emptyButton.tooltip = "Click to Exit Asset Exclusive Mode\nDrag to Drag the Asset";
            }
            else
            {
                if (userHeight != 1)
                {
                    emptyButton.tooltip = "Click to Collapse\nDrag to Drag the Asset";
                }
                else
                {
                    emptyButton.tooltip = "Click to Expand\nDrag to Drag the Asset";
                }
            }
            emptyButton.tooltip += plural;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.backgroundColor += CustomColors.AssetBarBackColor;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginHorizontal(CustomGUIStyles.InspectorButtonStyle, GUILayout.ExpandWidth(true));
            if (debugAsset)
            {
                if (GUILayout.Button(CustomGUIContents.DebugIconON, toolbarButton, GUILayout.Width(25)))
                {
                    debugAsset = false;
                    SetAllAssetEditorsDebugTo(debugAsset);
                }
            }
            else
            {
                GUILayout.Space(5);
            }
            GUIStyle labelStyle = CustomGUIStyles.AlignedLabelStyle;
            Texture2D icon = content.image as Texture2D;
            content.image = null;
            if (icon != null)
            {
                GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));
            }
            GUILayout.Label(content, labelStyle);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            DrawAssetTopBarButtons(toolbarButton, prefabMode, isDirty, multiMode);
            EditorGUILayout.EndHorizontal();
            Rect adjustedRect = GUILayoutUtility.GetLastRect();
            adjustedRect.x -= 5;
            adjustedRect.width += 10;
            EditorUtils.DrawLineOverRect(adjustedRect, CustomColors.HarderBright, 0);
            if (EditorGUIUtility.isProSkin)
            {
                EditorUtils.DrawLineOverRect(adjustedRect, CustomColors.SubtleBlue, 0);
                if (userHeight > 55)
                {
                    EditorUtils.DrawLineUnderRect(adjustedRect, CustomColors.HardShadow, -1);
                }
                GUI.backgroundColor = color;
            }
            else
            {
                EditorUtils.DrawLineOverRect(adjustedRect, CustomColors.SoftShadow, 1);
            }
            Event _event = Event.current;
            if (_event.type == EventType.Repaint)
            {
                assetViewRect = adjustedRect;
            }
            Rect clickRect = new Rect(adjustedRect);
            clickRect.height -= 3;
            clickRect.y += 3;

            if (!resizingAssetView && clickRect.Contains(_event.mousePosition))
            {
                if (_event.button == 0)
                {
                    if (_event.type == EventType.MouseDrag)
                    {
                        DragAndDrop.PrepareStartDrag();
                        if (multiMode)
                        {
                            DragAndDrop.objectReferences = targetObjects;
                            DragAndDrop.StartDrag("Dragging " + targetObjects.Length + " " + extension);
                            _event.Use();
                        }
                        else
                        {
                            DragAndDrop.objectReferences = new UnityEngine.Object[] { targetObject };
                            DragAndDrop.StartDrag("Dragging " + targetObject.name);
                            _event.Use();
                        }
                    }
                }
            }
            GUI.backgroundColor = color;
            if (GUI.Button(clickRect, emptyButton, GUIStyle.none))
            {
                if (_event.button == 0)
                {
                    ForceCollapseOrDefault();
                }
                else if (_event.button == 1)
                {
                    AssetTopBarMenus(prefabMode, multiMode);
                }
                else if (_event.button == 2)
                {
                    if (!CheckForApplyRevertOnClose())
                    {
                        return;
                    }
                    CloseAssetView();
                }
            }
            Rect rect = EditorUtils.GetLastLineRect();
            EditorUtils.DrawLineOverRect(rect, CustomColors.SimpleShadow, 2);
            if (EditorGUIUtility.isProSkin)
            {
                EditorUtils.DrawLineUnderRect(rect, CustomColors.HardShadow, -1, 1);
            }
            else
            {
                EditorUtils.DrawLineUnderRect(rect, CustomColors.MediumShadow, -1, 1);
            }
        }

        void OpenMaximizedAssetMode()
        {
            maximizedAssetView = true;
            assetsCollapsed = false;
            maximizeMode = 0;
            RepaintForAWhile();

        }
        void InitiateMovementRecording(Component component, GameObject _targetGameObject)
        {
            bool nullComponent = !component && _targetGameObject;
            bool differentGameObjects = false;
            if (!nullComponent)
            {
                var gameObject = component.gameObject;
                differentGameObjects = gameObject != _targetGameObject;
                Undo.RecordObject(gameObject, "Component Modification");
            }
            if (differentGameObjects || nullComponent)
            {
                Undo.RecordObject(_targetGameObject, "Component Modification");
            }
        }
        void FinalizeMovementRecording(Component component, GameObject _targetGameObject, string operationName = "Component Drag")
        {
            bool nullComponent = !component && _targetGameObject;
            bool differentGameObjects = false;
            if (!nullComponent)
            {
                var gameObject = component.gameObject;
                differentGameObjects = gameObject != _targetGameObject;
                EditorUtility.SetDirty(gameObject);
            }
            if (differentGameObjects || nullComponent)
            {
                EditorUtility.SetDirty(_targetGameObject);
            }
            Undo.SetCurrentGroupName(operationName);
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            pendingOperation.consumed = true;
            pendingOperation = null;
        }
        void DrawAssetTopBarButtons(GUIStyle toolbarButton, bool prefabMode, bool isDirty, bool multiMode = false)
        {
            if (targetObject == null && targetObjects == null)
            {
                return;
            }
            bool sameType = false;
            bool importObjectMode = false;
            if (multiMode)
            {
                sameType = SortedAssetSelection.Count == 1;
            }
            if (!EditorUtils.IsLightSkin())
            {
                GUI.backgroundColor += CustomColors.SubtleBlue;
            }
            GUIContent lockAssetContent = CustomGUIContents.AssetUnlocked;
            if (lockedAsset)
            {
                lockAssetContent = CustomGUIContents.AssetLocked;
            }
            GUIContent openContent = CustomGUIContents.OpenPrefabContent;
            if (multiMode)
            {
                isDirty = IsAnyAssetDirty(targetObjects) || IsAssetImportSettingsDirty();
                if (prefabMode && sameType)
                {
                    importObjectMode = EditorUtils.IsAnImportedObject(targetObjects[0]);
                }
            }
            else
            {
                isDirty = IsAssetDirty(targetObject);
                if (prefabMode)
                {
                    importObjectMode = EditorUtils.IsAnImportedObject(targetObject);
                }
            }
            float currentX = position.width;
            Rect rect = GUILayoutUtility.GetLastRect();
            float y = rect.y;
            int buttonHeight = 24;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.backgroundColor -= CustomColors.SubtleBlue;
            }
            Color color = GUI.backgroundColor;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.backgroundColor += Color.red * 1.5f;
            }
            else
            {
                GUI.backgroundColor -= CustomColors.LightSkinRed * 1.75f;
            }
            if (GUI.Button(new Rect(currentX - 22, y, 24, buttonHeight), CustomGUIContents.CloseAsset, toolbarButton))
            {
                if (!CheckForApplyRevertOnClose())
                {
                    return;
                }
                CloseAssetView();
            }
            currentX -= 22;
            GUI.backgroundColor = color;
            currentX -= 23;
            if (maximizedAssetView)
            {
                if (GUI.Button(new Rect(currentX, y, 23, buttonHeight), CustomGUIContents.MinimizeContent, toolbarButton))
                {
                    ResetMaximizedView();
                }
                //  currentX -= 23;
            }
            else
            {
                if (GUI.Button(new Rect(currentX, y, 23, buttonHeight), lockAssetContent, toolbarButton))
                {
                    lockedAsset = !lockedAsset;
                }
                if (lockedAsset)
                {
                    if (!EditorUtils.IsLightSkin())
                    {
                        EditorGUI.DrawRect(new Rect(currentX, y, 22, buttonHeight), CustomColors.SimpleBright);
                    }
                    else
                    {
                        EditorGUI.DrawRect(new Rect(currentX, y, 22, buttonHeight), CustomColors.ButtonEnabled);
                    }
                }
            }
            GUIContent _content = Event.current.alt || Event.current.shift ? CustomGUIContents.InspectAssetPopup : CustomGUIContents.InspectAssetNormal;
            int inspectButtonWidth = Event.current.alt || Event.current.shift ? 30 : 23;
            currentX -= inspectButtonWidth;
            bool popup = Event.current.alt || Event.current.shift;
            if (GUI.Button(new Rect(currentX, y, inspectButtonWidth, buttonHeight), _content, toolbarButton))
            {
                if (multiMode)
                {
                    PopUpInspectorWindow(targetObjects, popup);
                }
                else
                {
                    PopUpInspectorWindow(new UnityObject[] { targetObject }, popup);
                }
            }
            if ((multiMode && !sameType) || (prefabMode && !importObjectMode) || IsInvisibleAssetEditor())/* || targetObject is Material*/
            {
                GUI.enabled = false;
            }
            /*
            if (multiMode && sameType && targetObjects != null && targetObjects[0] is Material)
            {
                GUI.enabled = false;
            }*/
            GUIContent importContent = showImportSettings ? CustomGUIContents.HideImport : CustomGUIContents.ShowImport;

            if (!maximizedAssetView)
            {
                currentX -= 23;
                if (GUI.Button(new Rect(currentX, y, 23, buttonHeight), importContent, toolbarButton))
                {
                    showImportSettings = !showImportSettings;
                    if (!multiMode && targetObject is Material)
                    {
                        UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(targetObject, showImportSettings);
                    }
                    else if (multiMode && sameType && targetObjects[0] is Material)
                    {
                        UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(targetObjects[0], showImportSettings);
                    }
                    ResetAssetViewSize();
                    Repaint();
                }
                if (GUI.enabled)
                {
                    if (showImportSettings)
                    {
                        if (!EditorUtils.IsLightSkin())
                        {
                            EditorGUI.DrawRect(new Rect(currentX, y, 22, buttonHeight), CustomColors.SimpleBright);
                        }
                        else
                        {
                            EditorGUI.DrawRect(new Rect(currentX, y, 22, buttonHeight), CustomColors.ButtonEnabled);
                        }
                    }
                }
                else
                {
                    if (EditorUtils.IsLightSkin())
                    {
                        EditorGUI.DrawRect(new Rect(currentX, y, 22, buttonHeight), CustomColors.SoftShadow);
                    }
                }
            }

            if (!multiMode && targetObject != null && targetObject is Material)
            {
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(targetObject, ShouldDrawImportSettings());
            }
            else if (multiMode && sameType && targetObjects != null && targetObjects.Length > 1 && targetObjects[0] is Material)
            {
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(targetObjects[0], ShouldDrawImportSettings());
            }

            GUI.enabled = true;
            if ((isDirty && (!multiMode || sameType)) || prefabMode)
            {
                currentX -= 22;
                if (GUI.Button(new Rect(currentX, y, 22, buttonHeight), CustomGUIContents.SaveAsset, toolbarButton))
                {
                    SaveCurrentTargetOrTargets();
                }
            }
            if (prefabMode && !importObjectMode)
            {
                if (multiMode)
                {
                    GUI.enabled = false;
                }
                else if (!(targetObject is GameObject))
                {
                    GUI.enabled = false;
                }
                currentX -= 54;
                openContent.text = "OPEN";
                if (GUI.Button(new Rect(currentX, y, 54, buttonHeight), openContent, CustomGUIStyles.AssetToolbarButton_Open))
                {
                    OpenPrefab();
                }
            }
            GUI.enabled = true;
            EditorGUILayout.BeginHorizontal(CustomGUIStyles.InspectorSectionStyle);
            GUILayout.Space(position.width - currentX);
            EditorGUILayout.EndHorizontal();
        }
        void DrawAssetBottomBar(bool isFolder, bool multiMode = false)
        {
            if (targetObject == null && targetObjects == null)
            {
                return;
            }
            bool sameMultiType = false;
            if (multiMode)
            {
                sameMultiType = SortedAssetSelection.Count == 1;
            }
            if (maximizedAssetView)
            {
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.BeginHorizontal(CustomGUIStyles.InspectorButtonStyle);
            GUIContent label = CustomGUIContents.EmptyContent;
            if (!multiMode)
            {
                AssetInfo assetInfo = PoolCache.GetAssetInfo(targetObject);
                label = assetEditor.GetPreviewTitle();
                label.text = assetInfo.path;
            }
            else
            {
                label.text = " ―――";
            }
            bool hideEdit = false;
            if ((multiMode && !sameMultiType) || isFolder)
            {
                hideEdit = true;
            }
            EditorGUILayout.BeginHorizontal();
            ShowOpenAssetButtons(hideEdit, multiMode);
            EditorGUILayout.EndHorizontal();
            Rect labelRect = GUILayoutUtility.GetLastRect();
            labelRect.x += labelRect.width + 6;
            labelRect.y -= 2;
            labelRect.width = position.width;
            GUI.Label(labelRect, label, CustomGUIStyles.MiniLabel);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            float minusX = 0;
            if (assetEditor != null)
            {
                bool hasPreviewGUI = assetEditor.HasPreviewGUI();
                if ((!multiMode && hasPreviewGUI) || (multiMode && hasPreviewGUI && sameMultiType))
                {
                    EditorGUILayout.BeginHorizontal(CustomGUIStyles.InspectorSectionStyle);
                    bool videoClip = targetObject is VideoClip;
                    if (!videoClip)
                    {
                        assetEditor.OnPreviewSettings();
                    }
                    else if (assetImportSettingsEditor != null)
                    {
                        assetImportSettingsEditor.OnPreviewSettings();
                    }
                    EditorGUILayout.EndHorizontal();
                    minusX = GUILayoutUtility.GetLastRect().width;
                    if (Event.current.type == EventType.Repaint)
                    {
                        settingsWidth = minusX;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
            if (!multiMode)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                GUIContent emptyButton = CustomGUIContents.EmptyButton;
                if (settingsWidth > 0)
                {
                    labelRect.width -= (settingsWidth + 25);
                }
                labelRect.width -= settingsWidth;
                if (GUI.Button(labelRect, emptyButton, GUIStyle.none))
                {
                    if (Event.current.button == 0)
                    {
                        EditorWindow.GetWindow(Reflected.GetProjectWindowType());
                        EditorGUIUtility.PingObject(targetObject);
                    }
                    else if (Event.current.button == 1)
                    {
                        string path = AssetDatabase.GetAssetPath(targetObject);
                        EditorGUIUtility.systemCopyBuffer = path;
                        Debug.Log("Copied '" + path + "' to clipboard!");
                    }
                }
            }
            EditorUtils.DrawLineOverRect(CustomColors.HarderBright);
            EditorUtils.DrawLineOverRect(CustomColors.SimpleShadow, 1);
        }
        void ShowOpenAssetButtons(bool onlyFolder = false, bool multiMode = false)
        {
            if (targetObject != null || targetObjects != null)
            {
                if (!EditorUtils.IsLightSkin())
                {
                    GUI.backgroundColor += CustomColors.SubtleBlue;
                }
                GUIStyle toolbarButton = CustomGUIStyles.ToolbarButtonAsset;
                GUIContent openFolderContent = CustomGUIContents.FolderIcon;
                if (onlyFolder)
                {
                    if (GUILayout.Button(openFolderContent, toolbarButton, GUILayout.Width(22)))
                    {
                        if (!multiMode)
                        {
                            string path = AssetDatabase.GetAssetPath(targetObject);
                            if (path != "")
                            {
                                EditorUtility.RevealInFinder(path);
                            }
                        }
                        else
                        {
                            foreach (var obj in targetObjects)
                            {
                                string path = AssetDatabase.GetAssetPath(obj);
                                if (path != "")
                                {
                                    EditorUtility.RevealInFinder(path);
                                }
                            }
                        }
                    }
                    CustomGUIContents.DrawCustomButton(true, false, true);
                    if (!EditorUtils.IsLightSkin())
                    {
                        GUI.backgroundColor -= CustomColors.SubtleBlue;
                    }
                    return;
                }
                GUIContent openContent = CustomGUIContents.OpenContent;
                if (GUILayout.Button(openFolderContent, toolbarButton, GUILayout.Width(22)))
                {
                    if (!multiMode)
                    {
                        string path = AssetDatabase.GetAssetPath(targetObject);
                        if (path != "")
                        {
                            EditorUtility.RevealInFinder(path);
                        }
                    }
                    else
                    {
                        foreach (var obj in targetObjects)
                        {
                            string path = AssetDatabase.GetAssetPath(obj);
                            if (path != "")
                            {
                                EditorUtility.RevealInFinder(path);
                            }
                        }
                    }
                }
                CustomGUIContents.DrawCustomButton(true, false, true);
                if (GUILayout.Button(openContent, toolbarButton, GUILayout.Width(22)))
                {
                    if (!multiMode)
                    {
                        string path = AssetDatabase.GetAssetPath(targetObject);
                        if (path != "")
                        {
                            OpenAsset(path);
                        }
                    }
                    else
                    {
                        foreach (var obj in targetObjects)
                        {
                            string path = AssetDatabase.GetAssetPath(obj);
                            if (path != "")
                            {
                                OpenAsset(path);
                            }
                        }
                    }
                }
                CustomGUIContents.DrawCustomButton(true, false, true);
                if (!EditorUtils.IsLightSkin())
                {
                    GUI.backgroundColor -= CustomColors.SubtleBlue;
                }
            }
        }
        internal static void OpenAsset(string pathToAsset)
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pathToAsset);
            if (asset != null)
            {
                AssetDatabase.OpenAsset(asset);
            }
            else
            {
                Debug.LogError("Asset not found at: " + pathToAsset);
            }
        }
        internal static void OpenAsset(UnityEngine.Object asset)
        {
            if (asset != null)
            {
                AssetDatabase.OpenAsset(asset);
            }
        }
        private void PopUpInspectorWindow(UnityEngine.Object[] newTargets, bool popUp = false)
        {
            Type inspectorType = Reflected.GetInspectorWindowType();
            if (inspectorType != null && newTargets != null)
            {
                /*
                   if (newTargets is Component)
                   {
                       Component component = newTargets as Component;              
                       newTargets = component.gameObject;
                   } */
                EditorWindow inspector = null;
                if (popUp)
                {
                    inspector = (EditorWindow)ScriptableObject.CreateInstance(inspectorType);
                }
                else
                {
                    inspector = EditorWindow.GetWindow(inspectorType);
                }
                if (inspector != null)
                {
                    UnityObject[] currentSelection = Selection.objects;
                    Selection.objects = newTargets;
                    inspector.Show();
                    inspector.Focus();
                    EditorApplication.delayCall += () =>
                    {
                        if (popUp)
                        {
                            Reflected.SetLockState(inspector, true);
                            ignoreNextSelection = true;
                            Selection.objects = currentSelection;
                        }
                    };
                }
            }
        }

        void FixActiveIndex(bool exitingPrefab = false)
        {
            max = position.height - startComponentY - 22 - 36 - 22 - 36;
            if (showAssetLabels && ShouldDrawImportSettings())
            {
                max -= 50;
            }
            if (onPrefabSceneMode)
            {
                if (!SceneIsInPrefabMode())
                {
                    onPrefabSceneMode = false;
                    exitingPrefab = true;
                }
            }
            bool removedGO = false;
            bool removedActiveGO = false;
            if (tabs != null)
            {
                List<TabInfo> toRemove = new List<TabInfo>();
                for (int i = 0; i < tabs.Count; i++)
                {
                    TabInfo tab = tabs[i];
                    if (FloatingTab.isClosing && FloatingTab.linkedTab == tab)
                    {
                        continue;
                    }
                    if (tab.newTab && tab.prefab)
                    {
                        tab.prefab = false;
                        toRemove.Add(tab);
                    }
                    if (!tab.IsTabValid() && !tab.willBeDeleted)
                    {
                        EditorUtils.RebuildTab(tab, this);
                    }
                    if (tab.target == null && !tab.newTab && !tab.IsValidMultiTarget())
                    {
                        if (tab.prefab)
                        {
                            if (tab.target != null || tab.IsValidMultiTarget())
                            {
                                tab.prefab = false;
                                continue;
                            }
                            tab.willBeDeleted = true;
                            toRemove.Add(tab);
                        }
                        else
                        {
                            if (tab.target != null || tab.IsValidMultiTarget())
                            {
                                continue;
                            }
                            string name = tab.name;
                            toRemove.Add(tab);
                        }
                    }
                    else if (tab.IsValidMultiTarget())
                    {
                        bool removed = false;
                        foreach (GameObject go in tab.targets)
                        {
                            if (go == null)
                            {
                                tab.targets = tab.targets.Where(g => g != null).ToArray();
                                removed = true;
                                break;
                            }
                        }
                        if (tab.targets.Length == 1)
                        {
                            tab.target = tab.targets[0];
                            tab.targets = new GameObject[0];
                            tab.multiEditMode = false;
                        }
                        if (tab.targets.Length == 0)
                        {
                            tab.TrySetValidHistoryTarget();
                            if (tab.target != null || tab.IsValidMultiTarget())
                            {
                                tab.RefreshIcon();
                                tab.RefreshName();
                                continue;
                            }
                            removed = true;
                        }
                        if (removed)
                        {
                            removedGO = true;
                        }
                    }
                }
                if (toRemove.Count > 0)
                {
                    for (int i = 0; i < toRemove.Count; i++)
                    {
                        TabInfo tab = toRemove[i];
                        if (tab == null)
                        {
                            continue;
                        }

                        int tabIndex = tabs.IndexOf(tab);
                        if (tabIndex == -1)
                        {
                            continue;
                        }

                        if (i == toRemove.Count)
                        {
                            tab.ResetTab();
                            activeIndex = tab.index;
                        }
                        else
                        {
                            if (tabIndex == activeIndex)
                            {
                                DoCloseTab(tab, false);
                            }
                            else
                            {
                                CloseTab(tabIndex, false);
                            }
                            if (activeIndex > tabs.Count - 1)
                            {
                                activeIndex = tabs.Count - 1;
                            }
                        }
                    }
                    if (tabs == null || tabs.Count == 0)
                    {
                        tabs = new List<TabInfo>
                        {
                            new TabInfo(null, 0, this)
                        };
                        activeIndex = 0;
                    }
                    ReinitializeComponentEditors(false);
                    Repaint();
                }
            }
            if (tabs == null || tabs.Count == 0)
            {
                tabs = new List<TabInfo>
                {
                    new TabInfo(null, 0, this)
                };
                activeIndex = 0;
                Repaint();
                return;
            }
            if (activeIndex > tabs.Count - 1)
            {
                activeIndex = tabs.Count - 1;
            }
            if (activeIndex < 0)
            {
                activeIndex = 0;
            }
            if (removedGO)
            {
                ReinitializeComponentEditors(false);
                RefreshAllTabNames();
            }
            if (removedActiveGO && GetActiveTab().target)
            {
                SetTargetGameObject(GetActiveTab().target);
            }
            else if (removedActiveGO && GetActiveTab().IsValidMultiTarget())
            {
                SetTargetGameObjects(GetActiveTab().targets);
            }
            /*
            if (exitingPrefab)
            {
                foreach (TabInfo tab in tabs)
                {
                    tab.FixNulls();                 
                    
                    if (tab.prefab && !tab.markForDeletion && !tab.willBeDeleted)
                    {
                        Debug.Log("We're setting prefab off here for " + tab.name);
                        tab.prefab = false;
                    }
                
                }
            } */
        }
        void StartFallingTab()
        {
            if (GOdragging || FloatingTab.tabRect == null || FloatingTab.tabRect == Rect.zero)
            {
                return;
            }

            float padding = 0;
            if (showHistory)
            {
                padding = 40;
            }
            FloatingTab.startX = FloatingTab.tabRect.x;
            FloatingTab.targetTabX = GetTotalTabsWidth(dragTargetIndex - 1) + padding - toolbarScrollPosition.x - 3;
            //calculate distance between start and target

            FloatingTab.fallingTab = true;
            FloatingTab.startTime = Time.realtimeSinceStartup;
            FloatingTab.tabRect.x = FloatingTab.targetTabX;
        }
        float UpdateFallingTab()
        {
            if (!FloatingTab.fallingTab || FloatingTab.startTime < 0f)
            {
                FloatingTab.fallingTab = false;
                FloatingTab.tabRect = Rect.zero;
                return -1f;
            }
            float t = (Time.realtimeSinceStartup - FloatingTab.startTime) / FloatingTab.animationDuration * 4f;
            if (t >= 1f)
            {
                FloatingTab.startTime = -1f;
                FloatingTab.fallingTab = false;
                FloatingTab.tabRect = Rect.zero;
                return FloatingTab.targetTabX;
            }
            Rect rect = new Rect(FloatingTab.tabRect);
            rect.height -= 3;
            rect.y += 3;
            EditorGUI.DrawRect(rect, CustomColors.LineColor * 1f);
            float x = Mathf.Lerp(FloatingTab.startX, FloatingTab.targetTabX, t);
            Repaint();
            return x;
        }
        void EndDrag()
        {
            GUIUtility.hotControl = 0;
            waitingToDrag = false;
            dragging = false;
            DragAndDrop.objectReferences = null;
            if (GOdragging)
            {
                GOdragging = false;
                dragIndex = -1;
                dragTargetIndex = -1;
            }
            if (dragIndex == -1 || dragTargetIndex == -1)
            {
                StartFallingTab();
                dragIndex = -1;
                dragTargetIndex = -1;
            }
            if (dragIndex == dragTargetIndex)
            {
                StartFallingTab();
                dragIndex = -1;
                dragTargetIndex = -1;
            }
            if (dragIndex <= dragTargetIndex)
            {
                dragTargetIndex -= 1;
            }
            if (dragIndex != -1 && dragTargetIndex != -1)
            {
                if (dragIndex > tabs.Count - 1 || dragTargetIndex > tabs.Count - 1 || dragIndex < 0 || dragTargetIndex < 0)
                {
                    dragIndex = -1;
                    dragTargetIndex = -1;
                    return;
                }

                TabInfo tabToMove = tabs[dragIndex];
                TabInfo activeTab = GetActiveTab();
                tabs.RemoveAt(dragIndex);
                if (tabToMove != null)
                {
                    tabs.Insert(dragTargetIndex, tabToMove);
                }
                activeIndex = tabs.IndexOf(activeTab);
                StartFallingTab();
                dragIndex = -1;
                dragTargetIndex = -1;
                PopUpTip.Hide();
            }
        }
        void HandleTabDragging()
        {
            if (dragging)
            {
                waitingToDrag = false;
                if (GOdragging)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
                int _control = GUIUtility.GetControlID(FocusType.Passive);
                Event currentEvent = Event.current;
                switch (currentEvent.GetTypeForControl(_control))
                {
                    case EventType.MouseDrag:
                        GUIUtility.hotControl = _control;
                        if (dragging)
                        {
                            if (!GOdragging && (tabs[dragIndex].target) || tabs[dragIndex].IsValidMultiTarget())
                            {
                                if (currentEvent.mousePosition.y > 30)
                                {
                                    GOdragging = true;
                                    FloatingTab.Reset(this);
                                    DragAndDrop.PrepareStartDrag();
                                    if (tabs[dragIndex].IsValidMultiTarget())
                                    {
                                        DragAndDrop.objectReferences = tabs[dragIndex].targets;
                                    }
                                    else
                                    {
                                        DragAndDrop.objectReferences = new UnityEngine.Object[] { tabs[dragIndex].target };
                                    }
                                    DragAndDrop.StartDrag(tabs[dragIndex].shortName);
                                }
                            }
                        }
                        currentEvent.Use();
                        break;
                    case EventType.MouseUp:
                        EndAssetViewResize();
                        EndDrag();
                        currentEvent.Use();
                        break;
                    case EventType.DragExited:
                        EndDrag();
                        break;
                    case EventType.DragUpdated:
                        if (dragging)
                        {
                            if (GOdragging)
                            {
                                if (currentEvent.mousePosition.y < 30)
                                {
                                    GOdragging = false;
                                    DragAndDrop.objectReferences = null;
                                    DragAndDrop.visualMode = DragAndDropVisualMode.None;
                                    Repaint();
                                }
                            }
                            else if (!GOdragging && tabs[dragIndex].target)
                            {
                                if (currentEvent.mousePosition.y > 30)
                                {
                                    GOdragging = true;
                                    FloatingTab.Reset(this);
                                    DragAndDrop.objectReferences = new UnityEngine.Object[] { tabs[dragIndex].target };
                                    Event.current.Use();
                                }
                            }
                        }
                        break;
                }
            }
        }
        void EndAssetViewResize()
        {
            GUIUtility.hotControl = 0;
            resizingAssetView = false;
            resizeOriginalCursorY = 0;
            alreadyCalculatedHeight = true;
            if (maximizedAssetView && maximizeMode == 1)
            {
                maximizeMode = 0;
                Repaint();
                RunNextFrame(() =>
                {
                    ResetMaximizedView();
                });
            }
            else if (!maximizedAssetView && maximizeMode == 2)
            {
                maximizeMode = 0;
                Repaint();
                RunNextFrame(() =>
                {
                    OpenMaximizedAssetMode();
                });
            }
            else
            {
                maximizeMode = 0;
                Repaint();
            }
        }
        void HandleAssetViewResize()
        {
            bool newTabNoAssetCase = false;
            if (!targetObject && (targetObjects == null || targetObjects.Length == 0))
            {
                if (!IsActiveTabNew() && !maximizedAssetView)
                {
                    return;
                }
                else
                {
                    //  GUILayout.FlexibleSpace();
                    EditorGUILayout.BeginHorizontal(CustomGUIStyles.InspectorSectionStyle);
                    GUILayout.Space(35);
                    EditorGUILayout.EndHorizontal();
                    newTabNoAssetCase = true;
                }
            }
            Rect actionRect = new Rect(assetViewRect)
            {
                height = 40
            };
            actionRect.y -= 1;
            actionRect.y -= 36;
            if (newTabNoAssetCase)
            {
                actionRect = GUILayoutUtility.GetLastRect();
                actionRect.width = position.width;
                actionRect.height = 36;
                actionRect.y -= 37;
            }
            if (IsActiveTabNew() || maximizedAssetView)
            {
                Rect drawRect = new Rect(actionRect)
                {
                    height = 35
                };
                drawRect.y += 1;
                Color color = CustomColors.DefaultInspector * 0.85f;
                color.a = 1;
                EditorGUI.DrawRect(drawRect, color);
                EditorUtils.DrawLineOverRect(actionRect, CustomColors.SoftShadow, -3, 33);
                EditorUtils.DrawLineOverRect(actionRect, CustomColors.HarderBright, -2);
                EditorUtils.DrawLineOverRect(actionRect, CustomColors.HardShadow, -1);
                EditorUtils.DrawLineUnderRect(drawRect, CustomColors.HardShadow, 0);
            }
            Rect leftRect = new Rect(actionRect);
            Rect rightRect = new Rect(actionRect);
            leftRect.width = leftRect.width / 2 - 100;
            rightRect.x += leftRect.width + 200;
            rightRect.width = leftRect.width;
            if (UpdateChecker.IsUpdateAvailable && position.width > 322)
            {
                leftRect.width -= 55;
                leftRect.x += 55;
            }

            if (!assetsCollapsed && !newTabNoAssetCase)
            {
                EditorGUIUtility.AddCursorRect(leftRect, MouseCursor.ResizeVertical);
                EditorGUIUtility.AddCursorRect(rightRect, MouseCursor.ResizeVertical);
                if (Event.current.type == EventType.Repaint)
                {
                    hoveringResize = leftRect.Contains(Event.current.mousePosition) || rightRect.Contains(Event.current.mousePosition);
                }
            }
            leftRect.y += 14;
            leftRect.width -= 20;
            leftRect.x += 13;

            if (!assetsCollapsed && !newTabNoAssetCase)
            {
                EditorUtils.DrawLineOverRect(leftRect, CustomColors.MediumShadow, -2);
                EditorUtils.DrawLineOverRect(leftRect, CustomColors.MediumShadow, -6);
                EditorUtils.DrawLineOverRect(leftRect, CustomColors.HarderBright, -3);
                EditorUtils.DrawLineOverRect(leftRect, CustomColors.HarderBright, -7);
            }
            if (UpdateChecker.IsUpdateAvailable && position.width > 322)
            {
                leftRect.width += 55;
                leftRect.x -= 55;
            }
            rightRect = leftRect;
            rightRect.x += leftRect.width + 13 + 200;
            if (!assetsCollapsed && !newTabNoAssetCase)
            {
                EditorUtils.DrawLineOverRect(rightRect, CustomColors.MediumShadow, -2);
                EditorUtils.DrawLineOverRect(rightRect, CustomColors.MediumShadow, -6);
                EditorUtils.DrawLineOverRect(rightRect, CustomColors.HarderBright, -3);
                EditorUtils.DrawLineOverRect(rightRect, CustomColors.HarderBright, -7);
            }
            if (IsActiveTabNew() || maximizedAssetView)
            {
                Vector2 _position = new Vector2(position.width / 2 - 103, actionRect.y + 6);
                if (newTabNoAssetCase)
                {
                    _position = new Vector2(position.width / 2 - 103, actionRect.y + 6);
                    actionRect.y = position.height - 4;
                    actionRect.height = 15;
                }
                DrawAssetHistoryButton(_position);
                if (newTabNoAssetCase)
                {
                    return;
                }
            }
            if (Event.current.type == EventType.MouseDown && !resizingAssetView && actionRect.Contains(Event.current.mousePosition))
            {
                if (assetsCollapsed)
                {
                    if (Event.current.button == 2)
                    {
                        if (!CheckForApplyRevertOnClose())
                        {
                            return;
                        }
                        CloseAssetView();
                        return;
                    }
                    ForceCollapseOrDefault();
                    Repaint();
                    return;
                }
                startHeight = userHeight;
                if (rawUserHeight < 0)
                {
                    startHeight = rawUserHeight;
                }
                Event.current.Use();
                resizingAssetView = true;
                resizeOriginalCursorY = Event.current.mousePosition.y;
            }
        }

        void DrawMaximizedAssetGuide()
        {
            if (InRecoverScreen())
            {
                return;
            }
            if (maximizedAssetView && resizingAssetView)
            {
                Rect windowRect = new Rect(0, 0, position.width, position.height);
                EditorGUIUtility.AddCursorRect(windowRect, MouseCursor.ResizeVertical);
                float y = position.height - suggestedHeight - 86;
                float mouseY = Event.current.mousePosition.y;

                if (mouseY > 130)
                {
                    float extra = 0;
                    if (targetObjects != null && targetObjects.Length > 1)
                    {
                        extra += currentPreviewHeight;
                    }
                    if (showAssetLabels)
                    {
                        extra += 50;
                    }
                    Rect rect = new Rect(0, y - extra, position.width, suggestedHeight + 66 + extra);
                    EditorGUI.DrawRect(rect, CustomColors.FadeBlue);
                    rect.height = 3;
                    EditorGUI.DrawRect(rect, CustomColors.CustomBlue);
                    maximizeMode = 1;
                }
                else
                {
                    maximizeMode = 0;
                }
                Repaint();
            }
            else if (resizingAssetView && !maximizedAssetView && !assetsCollapsed)
            {
                float mouseY = Event.current.mousePosition.y;
                Rect windowRect = new Rect(0, 0, position.width, position.height);
                EditorGUIUtility.AddCursorRect(windowRect, MouseCursor.ResizeVertical);
                if (assetViewRect.y - mouseY > 100)
                {
                    Rect rect = new Rect(0, 0, position.width, 3);
                    EditorGUI.DrawRect(rect, CustomColors.CustomBlue);
                    rect = new Rect(0, 0, position.width, position.height - 20);
                    EditorGUI.DrawRect(rect, CustomColors.FadeBlue);
                    maximizeMode = 2;
                }
                else
                {
                    maximizeMode = 0;
                }
            }
            else if (maximizeMode != 0)
            {
                maximizeMode = 0;
                resizingAssetView = false;
            }
        }
        void DrawAssetHistoryButton(Vector2 _position)
        {
            GUIContent addText = CustomGUIContents.AddComponent;
            GUIStyle addButtonStyle = GUI.skin.button;
            float addButtonWidth = 180f;
            float addButtonHeight = 25f;
            Rect addButtonRect = new Rect(_position.x, _position.y, addButtonWidth, addButtonHeight);
            GUI.enabled = false;
            GUI.Button(addButtonRect, addText, addButtonStyle);
            EditorUtils.DrawRectBorder(addButtonRect, CustomColors.SimpleShadow);
            EditorUtils.DrawLineOverRect(addButtonRect, CustomColors.HarderBright, -1, 1);
            GUI.enabled = true;
            if (UpdateChecker.IsUpdateAvailable && position.width > 322)
            {
                Rect buttonRect = new Rect(5, addButtonRect.y + 2, 54, 21);
                if (GUI.Button(buttonRect, CustomGUIContents.UpdateContent, GUIStyle.none))
                {
                    UnityEditor.PackageManager.UI.Window.Open("");
                }
                GUI.Label(new Rect(buttonRect.x + 20, buttonRect.y - 2, 54, 21), UpdateChecker.LatestVersion, CustomGUIStyles.MiniLabel);
            }
            GUIContent _historyButton = CustomGUIContents.HistoryButton;
            GUIStyle historyButtonStyle = CustomGUIStyles.NoMarginButton;
            float historyButtonWidth = 20f;
            float historyButtonHeight = 25f;
            Rect historyButtonRect = new Rect(addButtonRect.x + addButtonRect.width + 3, _position.y, historyButtonWidth, historyButtonHeight);
            if (EditorUtils.IsLightSkin())
            {
                GUI.backgroundColor -= CustomColors.LightHistoryButton * 1.75f;
            }
            else
            {
                GUI.backgroundColor += CustomColors.BackHistory;
            }
            GUI.enabled = historyAssets != null && historyAssets.Count > 0;
            if (GUI.Button(historyButtonRect, _historyButton, historyButtonStyle))
            {
                ShowHistoryContextMenu();
                Event.current.Use();
            }
            if (GUI.enabled)
            {
                CustomGUIContents.DrawCustomButton(historyButtonRect, false, true);
            }
            else
            {
                EditorUtils.DrawLineOverRect(historyButtonRect, -1);
                EditorUtils.DrawRectBorder(historyButtonRect, CustomColors.SimpleShadow);
                GUI.enabled = true;
            }
            if (EditorUtils.IsLightSkin())
            {
                GUI.backgroundColor += CustomColors.LightHistoryButton * 1.75f;
            }
            else
            {
                GUI.backgroundColor -= CustomColors.BackHistory;
            }
        }
        void DrawAssetHistoryButton()
        {
            GUIContent _historyButton = CustomGUIContents.HistoryButton;
            if (EditorUtils.IsLightSkin())
            {
                GUI.backgroundColor -= CustomColors.LightHistoryButton * 1.75f;
            }
            else
            {
                GUI.backgroundColor += CustomColors.BackHistory;
            }
            GUI.enabled = historyAssets != null && historyAssets.Count > 0;
            if (GUILayout.Button(_historyButton, CustomGUIStyles.NoMarginButton, GUILayout.Height(25), GUILayout.Width(20)))
            {
                ShowHistoryContextMenu();
                Event.current.Use();
            }
            if (GUI.enabled)
            {
                CustomGUIContents.DrawCustomButton(false, true);
            }
            else
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                EditorUtils.DrawLineOverRect(rect, -1);
                EditorUtils.DrawRectBorder(rect, CustomColors.SimpleShadow);
                GUI.enabled = true;
            }
            if (EditorUtils.IsLightSkin())
            {
                GUI.backgroundColor += CustomColors.LightHistoryButton * 1.75f;
            }
            else
            {
                GUI.backgroundColor -= CustomColors.BackHistory;
            }

        }
        private bool triggerDrag = false;
        private void HandleMouseEvents()
        {
            Event _event = Event.current;
            if (_event == null)
            {
                return;
            }
            clickMousePosition.x = _event.mousePosition.x;
            EventType eventType = _event.rawType;
            if (eventType == EventType.DragExited && !triggerDrag)
            {
                if (ignoreNextDragEvent)
                {
                    ignoreNextDragEvent = false;
                }
                else if (pendingOperation != null && !pendingOperation.consumed && pendingOperation.mouseOverRect.Contains(mousePosition))
                {
                    triggerDrag = true;
                    RepaintForAWhile();
                }
                else
                {
                    pendingComponentDrag = false;
                    pendingOperation = null;
                }
                GUIUtility.hotControl = 0;
            }

            if (eventType == EventType.DragUpdated && pendingOperation != null)
            {
                mousePosition = new Vector2(_event.mousePosition.x, _event.mousePosition.y + scrollPosition.y);
            }
            if (eventType == EventType.MouseUp)
            {
                if (resizingAssetView)
                {
                    EndAssetViewResize();
                }
                if (dragging)
                {
                    waitingToDrag = false;
                    EndDrag();
                    EndAssetViewResize();
                }
                DragAndDrop.objectReferences = null;
            }
            if (resizingAssetView && (_event.isMouse || eventType == EventType.MouseDrag || eventType == EventType.DragUpdated))
            {
                DragAndDrop.objectReferences = null;
                _event.Use();
                float delta = _event.mousePosition.y - resizeOriginalCursorY;
                float newHeight = startHeight - delta;
                float maxHeight = max;
                rawUserHeight = Mathf.Max(newHeight, -IntGetPreviewHeight());
                userHeight = Mathf.Clamp(rawUserHeight, 1, Mathf.Min(maxHeight, previewRect.height));
                return;
            }
            if (eventType == EventType.MouseDown)
            {
                FloatingTab.fallingTab = false;
                if (_event.button == 0 && dragRect.Contains(_event.mousePosition))
                {
                    waitingToDrag = true;
                    mousePositionOnClick = _event.mousePosition;
                }
                else
                {
                    waitingToDrag = false;
                }
                PopUpTip.Hide();
            }

            if (scrollRect != null && scrollRect.width > 1)
            {
                Rect _scrollRect = new Rect(scrollRect) { height = toolScrollBarVisible && showScrollBar ? 40 : 20 };
                if (_scrollRect.Contains(_event.mousePosition))
                {
                    float mouseY = _event.mousePosition.y;
                    if (_event.isScrollWheel)
                    {
                        _event.Use();
                        int multiplier = _event.delta.x > 0 ? scrollSpeedX * scrollDirectionX : scrollSpeedY * scrollDirectionY;
                        toolbarScrollPosition.x += _event.delta.y > 0 || _event.delta.x > 0 ? 10 * multiplier : -10 * multiplier;
                        LimitScrollBar();
                        PopUpTip.Hide();
                    }
                    else if (mouseY <= 20)
                    {
                        PopUpTip.show = true;
                    }
                    else
                    {
                        PopUpTip.Hide();
                    }
                }
                else
                {
                    pendingTabSwitch = -1;
                    PopUpTip.Hide();
                }
            }
            if (_event.keyCode == KeyCode.Escape)
            {
                PopUpTip.Hide();
                dragging = false;
                waitingToDrag = false;
                dragIndex = -1;
                dragTargetIndex = -1;
            }
            if (eventType == EventType.MouseDrag && waitingToDrag && dragRect != null && dragRect.Contains(_event.mousePosition) && dragIndex != -1 && tabs.Count > 1 && _event.button == 0)
            {
                PopUpTip.Hide();
                if (Vector2.Distance(mousePositionOnClick, _event.mousePosition) > 2 && dragIndex < tabs.Count)
                {
                    FloatingTab.tabDragPoint = mousePositionOnClick.x - dragRect.position.x;
                    dragging = true;
                    FloatingTab.tabRect = Rect.zero;
                    dragTargetIndex = dragIndex;
                    if (activeIndex != dragIndex)
                    {
                        PopUpTip.Hide();
                    }
                }
            }
        }
        private void HandleFloatingTabInBar()
        {

            if ((dragging && !GOdragging) || FloatingTab.fallingTab)
            {
                Rect buttonRect = dragRect;
                if (FloatingTab.fallingTab)
                {
                    buttonRect = FloatingTab.tabRect;
                }
                else
                {
                    dragRect.width = totalWidth[dragIndex];
                    FloatingTab.linkedTab = tabs[dragIndex];
                    FloatingTab.dragTargetIndex = dragTargetIndex;
                    FloatingTab.dragIndex = dragIndex;
                }

                Color color = GUI.color;
                GUI.color += CustomColors.FloatingBarBase;
                GUIStyle style = CustomGUIStyles.ToolbarButtonTabs;
                if (FloatingTab.linkedTab.prefab)
                {
                    style = CustomGUIStyles.ToolbarButtonTabsPrefab;
                }
                style.contentOffset = new Vector2(0, 0);
                Texture2D icon = null;
                if (!FloatingTab.linkedTab.newTab && !FloatingTab.linkedTab.multiEditMode)
                {
                    if (showIcons)
                    {
                        icon = FloatingTab.linkedTab.icon;
                        style.padding = PaddingIcon;
                    }
                    else
                    {
                        style.padding = PaddingNoIcon;
                    }

                    if (FloatingTab.linkedTab.locked)
                    {
                        icon = CustomGUIContents.ContentWithLockIcon.image as Texture2D;
                        style.padding = PaddingIcon;
                    }
                }
                else if (FloatingTab.linkedTab.IsValidMultiTarget() && showIcons && !FloatingTab.linkedTab.locked)
                {
                    style.padding = PaddingIcon;
                    icon = CustomGUIContents.TabMulti.image as Texture2D;
                }
                else
                {
                    style.padding = PaddingNoIcon;
                    if (FloatingTab.linkedTab.IsValidMultiTarget() && FloatingTab.linkedTab.locked)
                    {
                        icon = CustomGUIContents.ContentWithLockIcon.image as Texture2D;
                        style.padding = PaddingIcon;
                    }
                }
                buttonRect.y = dragRect.y;
                int xLimit = 0;
                if (FloatingTab.fallingTab)
                {
                    buttonRect.x = UpdateFallingTab();
                    if (buttonRect.x < 0)
                    {
                        GUI.color = color;
                        return;
                    }
                }
                else
                {
                    buttonRect.x = Event.current.mousePosition.x - FloatingTab.tabDragPoint;
                    if (showHistory)
                    {
                        xLimit = 40;
                    }
                    if (buttonRect.x < xLimit)
                    {
                        buttonRect.x = xLimit;
                        toolbarScrollPosition.x -= 2;
                    }
                    if (buttonRect.xMax + 30 > position.width)
                    {
                        toolbarScrollPosition.x += 2;
                    }
                }
                LimitScrollBar();
                style.fixedHeight = 23;
                if (!FloatingTab.fallingTab)
                {
                    FloatingTab.style = style;
                    FloatingTab.tabRect = buttonRect;
                    FloatingTab.icon = icon;
                    FloatingTab.isSelected = activeIndex == dragIndex;
                }
                else
                {
                    if (FloatingTab.isSelected)
                    {
                        FloatingTab.style = CustomGUIStyles.ToolbarButtonTabs;
                    }
                }
                if (activeIndex != FloatingTab.dragIndex)
                {
                    GUI.color = CustomColors.FloatingTabUnselected;
                }
                GUI.Button(buttonRect, FloatingTab.linkedTab.shortName, FloatingTab.style);
                EditorUtils.DrawLineOverRect(buttonRect, CustomColors.SimpleBright);
                GUI.color = color;
                if ((showIcons || FloatingTab.linkedTab.locked) && FloatingTab.icon)
                {
                    float size = 16;
                    float pad = 4;
                    if (FloatingTab.linkedTab.locked)
                    {
                        size = 17;
                        pad = 3;
                    }
                    Rect iconRect = new Rect(buttonRect.x + 1, buttonRect.y + pad, size, size);
                    GUI.Label(iconRect, FloatingTab.icon);
                }
                buttonRect.width -= 1;
                if (FloatingTab.linkedTab.isSelected && !FloatingTab.linkedTab.newTab)
                {
                    Color _color = CustomColors.FloatingTabSelected2;
                    if (activeIndex == dragIndex)
                    {
                        _color = CustomColors.FloatingTabSelected;
                    }
                    EditorGUI.DrawRect(new Rect(buttonRect.x, buttonRect.y, buttonRect.width, 2), _color);
                }
                if (FloatingTab.isSelected)
                {
                    Color editorColor = EditorGUIUtility.isProSkin ? CustomColors.ActiveDark : CustomColors.ActiveLight;
                    Rect rect = new Rect(buttonRect.x, buttonRect.y + 23, buttonRect.width - 1, 3);
                    EditorGUI.DrawRect(rect, editorColor);
                }
                if (buttonRect.x - xLimit <= 0)
                {
                    dragTargetIndex = 0;
                }
                Repaint();
            }
        }
        bool MultiEditMode()
        {
            if (tabs == null || tabs.Count == 0)
            {
                return false;
            }
            if (GetActiveTab().multiEditMode)
            {
                return true;
            }
            return false;
        }
        private void CloseAssetView()
        {
            targetObject = null;
            targetObjects = null;
            lockedAsset = false;
            maximizedAssetView = false;
            ResetAssetViewSize();
            lastKnownHeight = 1;
            alreadyCalculatedHeight = false;
            EditorApplication.delayCall += () =>
            {
                CleanAllAssetEditors();
                Repaint();
            };
        }
        private void CleanAllAssetEditors()
        {
            PrefabMaterialMapManager.DestroyAllMaterialMaps();
            runtimePrefabComponents = null;
            if (assetEditor != null)
            {
                DestroyImmediate(assetEditor);
            }
            if (assetImportSettingsEditor != null)
            {
                DestroyImmediate(assetImportSettingsEditor);
            }
            if (assetImporter != null)
            {
                assetImporter = null;
            }
            if (assetImporters != null)
            {
                assetImporters = null;
            }
            if (prefabEditors != null)
            {
                foreach (var editor in prefabEditors)
                {
                    if (editor != null)
                    {
                        DestroyImmediate(editor);
                    }
                }
                prefabEditors = null;
            }
            if (prefabMaterialEditors != null)
            {
                foreach (var editor in prefabMaterialEditors)
                {
                    if (editor != null)
                    {
                        DestroyImmediate(editor);
                    }
                }
                prefabMaterialEditors = null;
            }
        }

        void TrySaveSession()
        {
            if (activeScene == null || !activeScene.IsValid())
            {
                return;
            }
            if (settingsData == null)
            {
                settingsData = AutoCreateSettings();
            }
            if (settingsData && !InRecoverScreen())
            {
                //Debug.Log("Saving scene session.");
                settingsData.SaveSession(this, activeScene);
            }
            else if (settingsData)
            {
                //Debug.Log("Saving scene session assets.");
                TrySaveLastAssets();
                EditorUtility.SetDirty(settingsData);
                AssetDatabase.SaveAssets();
            }
        }

        void HandleSceneChanged(bool saveLast = true)
        {
            changingScenes = false;
            ReloadPreview();
            if (enteringPlayMode && !StartedPlayModeInOtherScene())
            {
                return;
            }
            if (saveLast)
            {
                TrySaveSession();
            }
            activeScene = SceneInfo.FromActiveScene(activeScene);
            lastValidScenePath = activeScene.ScenePath;
            if (sessionsMode != 2 || exitingPlayMode)
            {
                if (instances != null && instances.Contains(this))
                {
                    instances.Remove(this);
                }
                ReopenCoInspector(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            }
            else
            {
                CleanTabs();
                RestoreLastAssets();
                TryLoadSession();
                RegisterWindow(this);
            }
            UpdateCurrentTip();
        }


        void AutoCheckScene()
        {
            if (exitingPlayMode)
            {
                return;
            }

            Scene currentScene = SceneManager.GetActiveScene();
            bool isSceneValid = currentScene.IsValid() && !string.IsNullOrEmpty(currentScene.path);

            if (!isSceneValid)
            {
                return;
            }
            if (activeScene == null || activeScene.ScenePath != currentScene.path)
            {
                activeScene = SceneInfo.FromActiveScene();
                lastValidScenePath = activeScene.ScenePath;
            }
        }
        private void OnGUI()
        {
            RunDelayedMethods();
            FixActiveIndex();
            if (changingScenes || (exitingPlayMode && scenesChanged))
            {
                HandleAssetViewResize();
                return;
            }
            if (exitingPlayMode)
            {
                //Debug.Log("Stuck exiting Play Mode");
                return;
            }
            HandleMouseEvents();
            if (!maximizedAssetView)
            {
                HandleTabDragging();
                if (tabs != null && tabs.Count > 0)
                {
                    UpdateTabBar();
                    DrawTabBar();
                    DrawScrollBar();
                    GetActiveTab().scrollPosition = scrollPosition.y;
                }

                headerRect = Rect.zero;

                if (IsActiveTabNew())
                {
                    CleanGameObjectEditors();
                    headerRect = new Rect(0, 35, position.width, 20);
                }
                else if (!maximizedAssetView && (GetActiveTab().target || IsActiveTabValidMulti()))
                {
                    EditorGUILayout.BeginVertical();
                    DrawHeader();
                    EditorGUILayout.EndVertical();
                    headerRect = GUILayoutUtility.GetLastRect();
                }
                if (headerRect.height > 1)
                {
                    clickMousePosition.y = headerRect.height;
                }
                EditorGUIUtility.hierarchyMode = true;
                HideHeaderMargin();
                if (!InRecoverScreen())
                {
                    DrawActiveTabUnder();
                    if (toolbarScrollPosition.x > 0)
                    {
                        Rect rect = showHistory ? PoolCache.historyMarginRect : PoolCache.marginRect;
                        EditorGUI.DrawRect(new Rect(rect.x - 1, rect.y, 1, rect.height), CustomColors.MediumShadow);
                        if (EditorUtils.IsLightSkin())
                        {
                            rect.x -= 1;
                            EditorUtils.DrawFadeToRight(rect, CustomColors.SimpleBright);
                        }
                        else
                        {
                            EditorUtils.DrawFadeToRight(rect, CustomColors.GradientShadow);
                        }
                    }
                    if (toolbarScrollPosition.x < GetMaximumScroll())
                    {
                        Rect rect = new Rect(position.width - 24 - 20, 1, 20, 22);
                        if (EditorUtils.IsLightSkin())
                        {
                            rect.x += 1;
                            EditorUtils.DrawFadeToLeft(rect, CustomColors.SimpleBright);
                        }
                        else
                        {
                            EditorUtils.DrawFadeToLeft(rect, CustomColors.GradientShadow);
                        }
                    }
                    HandleFloatingTabInBar();
                }
                Rect multiHeaderRect = Rect.zero;
                if (IsActiveTabValidMulti())
                {
                    DrawUnderMultiObjectHeader(headerRect);
                    multiHeaderRect = GUILayoutUtility.GetLastRect();
                    multiHeaderRect.y -= GetActiveTab().multiFoldout ? 2 : 4;
                    multiHeaderRect = EditorUtils.GetLastLineRect(multiHeaderRect);
                    EditorUtils.DrawLineUnderRect(multiHeaderRect, CustomColors.HardShadow, 4);
                }
                if (GetActiveTab().target || IsActiveTabValidMulti())
                {
                    float startComponentSection = headerRect.y + headerRect.height;

                    scrollPosition = GUILayout.BeginScrollView(scrollPosition);
                    if (!GetActiveTab().multiEditMode)
                    {
                        DrawComponentInspectors(GetActiveTab().target);
                    }
                    else
                    {
                        DrawMultiInspector();
                    }
                    bool darken = targetObject || (targetObjects != null && targetObjects.Length > 0);
                    GUILayout.EndScrollView();
                    Rect componentSection = GUILayoutUtility.GetLastRect();
                    if (Event.current.type == EventType.Repaint)
                    {
                        startComponentY = componentSection.y;
                    }
                    float endComponentSection = componentSection.y + componentSection.height;
                    UpdateComponentScrollRect(startComponentSection, endComponentSection);
                    Color backgroundColor = GUI.backgroundColor;
                    if (darken)
                    {
                        EditorUtils.DrawLineUnderRect(0);
                        EditorUtils.DrawLineUnderRect(CustomColors.SimpleBright);
                        if (!EditorUtils.IsLightSkin())
                        {
                            GUI.backgroundColor -= CustomColors.BackDarkColor;
                        }
                        else
                        {
                            GUI.backgroundColor = Color.gray / 4.5f;
                        }
                    }
                    DrawAddComponentBar(backgroundColor);
                    if (scrollPosition.y > 0)
                    {
                        float padding = GetActiveTab().IsValidMultiTarget() && GetActiveTab().multiFoldout ? multiHeaderRect.height + 4 : 0;
                        headerRect.y += padding + headerRect.height;
                        Color shadowColor = CustomColors.GradientShadow;
                        int height = 8;
                        if (EditorUtils.IsLightSkin())
                        {
                            shadowColor = CustomColors.SimpleShadow;
                            height = 12;
                        }
                        EditorUtils.DrawFadeToBottom(headerRect, shadowColor, height);
                    }
                }
                if (IsActiveTabNew())
                {

                    StartMessageGUI();
                }
            }
            else
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Space(36);
                EditorGUILayout.EndVertical();
            }

            if (!dragging && !maximizedAssetView)
            {
                PopUpTip.ShowGUI();
            }
            bool drawAssetInspector = targetObject || (targetObjects != null && targetObjects.Length > 0);

            if (drawAssetInspector)
            {

                DrawAssetInspector(true);
                LimitAssetViewHeight();
            }
            else if (!IsActiveTabNew())
            {
                EditorUtils.DrawLineUnderRect(CustomColors.SimpleShadow, 0, 1);
                EditorUtils.DrawLineUnderRect(CustomColors.SimpleShadow, 0, 2);
                EditorUtils.DrawLineUnderRect(CustomColors.SoftShadow, 0, 5);
                if (targetObject || (targetObjects != null && targetObjects.Length > 0))
                {

                    return;
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUI.DrawRect(GUILayoutUtility.GetLastRect(), CustomColors.SoftShadow);
            }
            if (!drawAssetInspector)
            {
                targetObject = null;
                targetObjects = null;
                CleanAllAssetEditors();
                maximizedAssetView = false;
            }
            HandleAssetViewResize();
            FixActiveIndex();
            if (drawAssetInspector)
            {
                DrawMaximizedAssetGuide();
            }
        }

        void FocusOnSceneView(GameObject[] targets)
        {
            if (targets == null || targets.Length == 0)
            {
                return;
            }
            if (SceneView.sceneViews.Count > 0)
            {
                Vector3 middle = MiddlePoint(targets);
                SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                if (GetActiveTab().zoomFocus)
                {
                    sceneView.LookAt(middle, sceneView.camera.transform.rotation, 1);
                }
                else
                {
                    FrameObjectsBounds(targets, sceneView);
                }
                GetActiveTab().zoomFocus = !GetActiveTab().zoomFocus;
            }
        }
        internal static void _FocusOnSceneView(GameObject target)
        {
            if (target == null)
            {
                return;
            }
            if (SceneView.sceneViews.Count > 0)
            {
                SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                sceneView.LookAt(target.transform.position, sceneView.camera.transform.rotation, 1);
            }
        }
        void FocusOnSceneView(GameObject target)
        {
            if (target == null)
            {
                return;
            }
            if (SceneView.sceneViews.Count > 0)
            {
                SceneView sceneView = (SceneView)SceneView.sceneViews[0];
                if (GetActiveTab().zoomFocus)
                {
                    sceneView.LookAt(target.transform.position, sceneView.camera.transform.rotation, 1);
                }
                else
                {
                    FrameObjectBounds(target, sceneView);
                }
                GetActiveTab().zoomFocus = !GetActiveTab().zoomFocus;
            }
        }
        static Vector3 MiddlePoint(GameObject[] gameObjects)
        {
            Vector3 middle = Vector3.zero;
            foreach (GameObject go in gameObjects)
            {
                if (go == null)
                {
                    continue;
                }
                middle += go.transform.position;
            }
            middle /= gameObjects.Length;
            return middle;
        }
        void AssetTopBarMenus(bool prefabMode, bool multiMode)
        {
            if (!targetObject && targetObjects == null)
            {
                return;
            }

            GenericMenu menu = new GenericMenu();


            if (!maximizedAssetView && (assetImportSettingsEditor != null || prefabMode))
            {
                if (!showImportSettings)
                {
                    string label = "Show All Import Settings";
                    if (prefabMode)
                    {
                        label = "Show All Import Settings";
                    }
                    menu.AddItem(new GUIContent(label), false, () =>
                    {
                        showImportSettings = true;
                        if (!assetsCollapsed)
                        {
                            ResetAssetViewSize();
                            Repaint();
                        }
                    });
                }
                else
                {
                    string label = "Hide Import Settings";
                    if (prefabMode)
                    {
                        label = "Hide Import Settings";
                    }
                    menu.AddItem(new GUIContent(label), false, () =>
                    {
                        showImportSettings = false;
                        if (userHeight != 1)
                        {
                            ResetAssetViewSize();
                        }
                    });
                }
                menu.AddSeparator("");
            }
            if (!InRecoverScreen())
            {

                string maximized = "Show in Asset Exclusive Mode";
                if (maximizedAssetView)
                {
                    maximized = "Exit Asset Exclusive Mode";
                }
                menu.AddItem(new GUIContent(maximized), false, () =>
                   {
                       maximizedAssetView = !maximizedAssetView;
                       if (maximizedAssetView)
                       {
                           assetsCollapsed = false;
                       }
                       ResetAssetViewSize();
                       Repaint();
                   });
            }
            if (userHeight != 1)
            {
                menu.AddItem(new GUIContent("Collapse the Asset View"), false, () =>
                {
                    ForceCollapseOrDefault();
                });
            }
            else
            {
                menu.AddItem(new GUIContent("Expand the Asset View"), false, () =>
                {
                    ForceCollapseOrDefault();
                });
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Normal"), !debugAsset, () =>
               {
                   debugAsset = false;
                   SetAllAssetEditorsDebugTo(false);
               });
            menu.AddItem(new GUIContent("Debug"), debugAsset, () =>
            {
                debugAsset = true;
                SetAllAssetEditorsDebugTo(true);
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Reveal in Folder"), false, () =>
            {
                if (multiMode)
                {
                    foreach (var obj in targetObjects)
                    {
                        EditorUtility.RevealInFinder(AssetDatabase.GetAssetPath(obj));
                    }
                }
                else
                {
                    EditorUtility.RevealInFinder(AssetDatabase.GetAssetPath(targetObject));
                }
            });
            if (!multiMode)
            {
                menu.AddItem(new GUIContent("Ping Asset in Project"), false, () =>
                {
                    EditorWindow.GetWindow(Reflected.GetProjectWindowType());
                    EditorGUIUtility.PingObject(targetObject);
                });
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Ignore Folder Inspection"), ignoreFolders, () =>
            {
                ignoreFolders = !ignoreFolders;
                SaveSettings();
            });
            menu.AddItem(new GUIContent("Disable Asset Inspection"), !assetInspection, () =>
                {
                    assetInspection = !assetInspection;
                    if (!assetInspection)
                    {
                        CloseAssetView();
                    }
                    SaveSettings();
                    Repaint();
                });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Collapsed Prefab View"), collapsePrefabComponents, () =>
                {
                    collapsePrefabComponents = !collapsePrefabComponents;
                    SaveSettings();
                    if (prefabFoldouts_ != null)
                    {
                        for (int i = 0; i < prefabFoldouts_.Length; i++)
                        {
                            prefabFoldouts_[i] = !collapsePrefabComponents;
                        }
                    }
                });
            menu.AddItem(new GUIContent("Show AssetBundle Footer"), showAssetLabels, () =>
            {
                showAssetLabels = !showAssetLabels;
                SaveSettings();
                ResetAssetViewSize();
                RepaintForAWhile();
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Close the Asset View"), false, () =>
            {
                if (!CheckForApplyRevertOnClose())
                {
                    return;
                }
                CloseAssetView();
            });
            float height = 15 * (menu.GetItemCount() + 1);
            if (position.height - Event.current.mousePosition.y < height)
            {
                menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - height, 0, 0));
            }
            else
            {
                menu.ShowAsContext();
            }
        }
        public bool NotNullTabs()
        {
            if (tabs != null && tabs.Count > 0)
            {
                return true;
            }
            return false;
        }
        public bool IsThereANullEditor()
        {
            if (componentEditors != null)
            {
                foreach (var editor in componentEditors)
                {
                    if (editor == null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        internal bool CanSkipRefresh()
        {
            if (Event.current.type == EventType.Used)
            {
                //       oops not using this now
            }
            return false;
        }
        void HandleComponentDrag()
        {
            DragAndDrop.objectReferences = null;
            if (!pendingComponentDrag)
            {
                return;
            }
            pendingComponentDrag = false;

            if (pendingOperation == null || pendingOperation.targetObject == null || pendingOperation.consumed)
            {
                return;
            }
            InitiateMovementRecording(pendingOperation.draggedComponent, pendingOperation.targetObject);
            if (pendingOperation.isAsset)
            {
                if (pendingOperation.assets == null || pendingOperation.assets.Count == 0)
                {
                    pendingOperation = null;
                    return;
                }
                for (int i = 0; i < pendingOperation.assets.Count; i++)
                {
                    if (pendingOperation.assets[i] == null)
                    {
                        continue;
                    }
                    UnityObject toAdd = pendingOperation.assets[i];
                    Type typeToAdd = toAdd.GetType();
                    if (toAdd is MonoScript)
                    {
                        typeToAdd = (toAdd as MonoScript).GetClass();
                    }
                    if (toAdd is AudioClip)
                    {
                        typeToAdd = typeof(AudioSource);
                    }
                    else if (toAdd is AudioMixerGroup)
                    {
                        typeToAdd = typeof(AudioSource);
                    }
                    else if (toAdd is UnityEditor.Animations.AnimatorController)
                    {
                        typeToAdd = typeof(Animator);
                    }
                    else if (toAdd is AnimationClip)
                    {
                        typeToAdd = typeof(Animation);
                    }
                    else if (toAdd is Sprite)
                    {
                        if (EditorUtils.IsAnUIObject(pendingOperation.targetObject))
                        {
                            typeToAdd = typeof(UnityEngine.UI.Image);
                        }
                        else
                        {
                            typeToAdd = typeof(SpriteRenderer);
                        }
                    }
                    else if (toAdd is Texture2D)
                    {
                        typeToAdd = typeof(UnityEngine.UI.RawImage);
                    }
                    else if (toAdd is VideoClip)
                    {
                        typeToAdd = typeof(UnityEngine.Video.VideoPlayer);
                    }
                    else if (toAdd is Font)
                    {
                        typeToAdd = typeof(UnityEngine.UI.Text);
                    }
                    else if (toAdd is Material)
                    {
                        typeToAdd = typeof(MeshRenderer);
                    }
                    alreadyMovingComponent = true;
                    if (Undo.AddComponent(pendingOperation.targetObject, typeToAdd))
                    {
                        Component justAdded = pendingOperation.targetObject.GetComponents<Component>()[pendingOperation.targetObject.GetComponents<Component>().Length - 1];
                        if (justAdded is MeshRenderer)
                        {
                            MeshRenderer meshRenderer = justAdded as MeshRenderer;
                            meshRenderer.sharedMaterial = toAdd as Material;
                        }
                        else if (justAdded is AudioSource)
                        {
                            AudioSource audioSource = justAdded as AudioSource;
                            if (toAdd is AudioClip)
                            {
                                audioSource.clip = toAdd as AudioClip;
                            }
                            else if (toAdd is AudioMixerGroup)
                            {
                                audioSource.outputAudioMixerGroup = toAdd as AudioMixerGroup;
                            }
                        }
                        else if (justAdded is Animator)
                        {
                            Animator animator = justAdded as Animator;
                            animator.runtimeAnimatorController = toAdd as UnityEditor.Animations.AnimatorController;
                        }
                        else if (justAdded is SpriteRenderer)
                        {
                            SpriteRenderer spriteRenderer = justAdded as SpriteRenderer;
                            spriteRenderer.sprite = toAdd as Sprite;
                        }
                        else if (justAdded is Animation)
                        {
                            Animation animation = justAdded as Animation;
                            animation.clip = toAdd as AnimationClip;
                        }
                        else if (justAdded is UnityEngine.UI.Image)
                        {
                            UnityEngine.UI.Image image = justAdded as UnityEngine.UI.Image;
                            image.sprite = toAdd as Sprite;
                        }
                        else if (justAdded is UnityEngine.UI.RawImage)
                        {
                            UnityEngine.UI.RawImage _image = justAdded as UnityEngine.UI.RawImage;
                            _image.texture = toAdd as Texture2D;
                        }
                        else if (justAdded is UnityEngine.Video.VideoPlayer)
                        {
                            UnityEngine.Video.VideoPlayer videoPlayer = justAdded as UnityEngine.Video.VideoPlayer;
                            videoPlayer.clip = toAdd as VideoClip;
                        }
                        else if (justAdded is UnityEngine.UI.Text)
                        {
                            UnityEngine.UI.Text text = justAdded as UnityEngine.UI.Text;
                            text.font = toAdd as Font;
                        }
                        int curIndex = pendingOperation.targetObject.GetComponents<Component>().Length - 1;
                        HandleMoveComponent(justAdded, curIndex, pendingOperation.targetIndex, pendingOperation.targetObject.GetComponents<Component>());
                    }
                }
                PerformRedo();
                FinalizeMovementRecording(null, pendingOperation.targetObject, "Asset Dragged as Component");
                return;
            }
            if (pendingOperation == null || pendingOperation.targetIndex == -1 || pendingOperation.draggedComponent == null)
            {
                return;
            }
            int targetIndex = pendingOperation.targetIndex;
            int index = pendingOperation.sourceIndex;
            alreadyMovingComponent = true;
            Component draggedComponent = pendingOperation.draggedComponent;
            Component[] components = pendingOperation.targetObject.GetComponents<Component>();
            if (pendingOperation.isSelf)
            {
                if (pendingOperation.prefabError)
                {
                    pendingOperation.consumed = true;
                    pendingOperation = null;
                    EditorUtils.ShowPrefabFailedMessage();
                    return;
                }
                if (pendingOperation.errored)
                {
                    pendingOperation.consumed = true;
                    pendingOperation = null;
                    EditorUtils.ShowPasteFailedMessage();
                    return;
                }
                string operation = "Moved Component";
                if (pendingOperation.isCopy)
                {
                    ComponentUtility.CopyComponent(draggedComponent);
                    int numComponents = components.Length;
                    operation = "Cloned Component";
                    if (ComponentUtility.PasteComponentAsNew(pendingOperation.targetObject))
                    {
                        Repaint();
                        components = pendingOperation.targetObject.GetComponents<Component>();
                        if (numComponents == components.Length)
                        {
                            pendingOperation.consumed = true;
                            pendingOperation = null;
                            EditorUtils.ShowPasteFailedMessage();
                            return;
                        }
                        draggedComponent = components[components.Length - 1];
                        index = components.Length - 1;
                        GetActiveTab().SaveFoldoutToMap(draggedComponent, pendingOperation.foldoutOrigin, null, true);
                        HandleMoveComponent(draggedComponent, index, targetIndex, components);
                        FinalizeMovementRecording(draggedComponent, pendingOperation.targetObject, operation);
                        return;
                    }
                    else
                    {
                        pendingOperation.consumed = true;
                        pendingOperation = null;
                        EditorUtils.ShowPasteFailedMessage();
                        return;
                    }
                }
                else
                {
                    GetActiveTab().SaveFoldoutToMap(draggedComponent, pendingOperation.foldoutOrigin, null, true);
                    HandleMoveComponent(draggedComponent, index, targetIndex, components);
                    FinalizeMovementRecording(draggedComponent, pendingOperation.targetObject, operation);
                }
            }
            else
            {
                if (pendingOperation.errored)
                {
                    pendingOperation.consumed = true;
                    pendingOperation = null;
                    EditorUtils.ShowPasteFailedMessage();
                    return;
                }
                bool removeAfter = pendingOperation.removeAfter;
                ComponentUtility.CopyComponent(draggedComponent);
                if (ComponentUtility.PasteComponentAsNew(pendingOperation.targetObject))
                {
                    string operation = "Cloned Component";
                    components = pendingOperation.targetObject.GetComponents<Component>();
                    if (removeAfter)
                    {
                        Undo.DestroyObjectImmediate(draggedComponent);
                        operation = "Dragged Component";

                    }
                    draggedComponent = components[components.Length - 1];
                    index = components.Length - 1;
                    operation += " to " + pendingOperation.targetObject.name;
                    GetActiveTab().SaveFoldoutToMap(draggedComponent, pendingOperation.foldoutOrigin, null, true);
                    HandleMoveComponent(draggedComponent, index, targetIndex, components);
                    FinalizeMovementRecording(draggedComponent, pendingOperation.targetObject, operation);
                    return;
                }
                else if (pendingOperation.targetObject.GetComponent(draggedComponent.GetType()) != null)
                {
                    pendingOperation.consumed = true;
                    pendingOperation = null;
                    EditorUtils.ShowPasteFailedMessage();
                    return;
                }
                else
                {
                    pendingOperation.consumed = true;
                    pendingOperation = null;
                    EditorUtils.ShowPasteFailedMessageError();
                    return;
                }
            }
            Repaint();
        }
        private MaterialMapManager PrefabMaterialMapManager
        {
            get
            {
                if (prefabMaterialManager == null)
                {
                    prefabMaterialManager = new MaterialMapManager();
                }
                return prefabMaterialManager;
            }
        }

        private void DrawComponentInspectors(GameObject gameObject)
        {
            Component[] components = gameObject.GetComponents<Component>();
            GetActiveTab().CleanMap(components);
            if (componentEditors == null || components == null || componentEditors.Length != components.Length)
            {
                // Debug.Log();
                ReinitializeComponentEditors(false);
            }
            else if (components != null && componentEditors != null)
            {
                if (components.Length > componentEditors.Length)
                {
                    ReinitializeComponentEditors(false);
                    scrollPosition.y += 1000;
                }
            }

            Editor[] runtimeEditors = componentEditors;
            for (int i = 0; i < components.Length; i++)
            {
                if (i >= runtimeEditors.Length)
                {
                    break;
                }
                if (runtimeEditors[i] == null || !runtimeEditors[i].target || runtimeEditors[i].target != components[i])
                {
                    if (runtimeEditors[i] != null)
                    {
                        DestroyImmediate(runtimeEditors[i]);
                        ReinitializeComponentEditors(false);
                    }
                }
                if (IsComponentFilteredInTab(components[i], runtimeEditors[i]) || !AreWeFilteringComponents())
                {
                    if (runtimeEditors[i] != null)
                    {
                        DrawComponentSection(components[i], runtimeEditors[i], i, components);
                    }
                    if (components[i] == null)
                    {
                        DrawComponentSection(components[i], runtimeEditors[i], i, components);
                    }
                    AddIfNecessary(runtimeEditors[i]);
                }
                else if (i == components.Length - 1)
                {
                    EditorGUILayout.BeginVertical();
                    GUILayout.Space(0);
                    EditorGUILayout.EndVertical();
                    EditorUtils.DrawUnderLastComponent();
                }
            }
            GetActiveTab().RebuildMaterialsIfNecessary();
        }

        internal bool IsComponentFilteredInTab(Component component, Editor editor)
        {
            if (GetActiveTab() == null)
            {
                return false;
            }
            ComponentMap componentMap = GetActiveTab().GetFoldoutMapForComponent(component, editor);
            if (componentMap == null)
            {
                return false;
            }
            if (!AreWeFilteringComponents())
            {
                componentMap.isFilteredOut = false;
                return true;
            }
            if (GetActiveTab().filterString == "" || GetActiveTab().filterString == null)
            {
                componentMap.isFilteredOut = false;
                return true;
            }
            if (component == null)
            {
                return false;
            }
            componentMap.isFilteredOut = true;
            if (component.GetType().Name.ToLower().Contains(GetActiveTab().filterString.ToLower()))
            {
                componentMap.isFilteredOut = false;
                return true;
            }
            return false;
        }
        internal bool AreWeFilteringComponents()
        {
            if (!filteringComponents)
            {
                return false;
            }
            if (GetActiveTab() == null)
            {
                return false;
            }
            if (GetActiveTab().filterString == "" || GetActiveTab().filterString == null)
            {

                return false;
            }
            return true;
        }

        private bool DrawDragSpace(int index)
        {
            int hoverIndex = index;
            if (index > 0)
            {
                hoverIndex--;
                if (pendingOperation != null && pendingOperation.targetIndex == hoverIndex)
                {
                    GUILayout.Space(5);
                    return true;
                }
                else
                {
                    GUILayout.Space(0);
                    return false;
                }
            }
            return false;
        }
        private void DrawAfterComponentBar(bool isDragged, bool isCopied, ComponentMap componentMap, Rect compHeaderRect, int index, bool flag)
        {
            if (isDragged)
            {
                if (!flag)
                {
                    Rect rect = new Rect(compHeaderRect)
                    {
                        width = 10
                    };
                    EditorGUI.DrawRect(rect, CustomColors.DefaultInspector);
                }
                if (!isCopied)
                {
                    if (EditorUtils.IsLightSkin())
                    {
                        EditorUtils.DrawFadeToLeft(compHeaderRect, CustomColors.CustomBlue * 0.4f);
                    }
                    else
                    {
                        EditorUtils.DrawFadeToLeft(compHeaderRect, CustomColors.CustomSubtleBlue);
                    }
                }
                else
                {
                    if (EditorUtils.IsLightSkin())
                    {
                        EditorUtils.DrawFadeToLeft(compHeaderRect, CustomColors.CustomGreen * 0.6f);
                    }
                    else
                    {
                        EditorUtils.DrawFadeToLeft(compHeaderRect, CustomColors.SubtleGreen);
                    }
                }
            }
            if (componentMap.focusAfter && GUI.GetNameOfFocusedControl() != "Component" + index)
            {
                {
                    componentMap.focusAfter = false;
                    GUI.FocusControl("Component" + index);
                }
            }
            if (Event.current.type is EventType.Repaint && componentMap.awaitingScroll)
            {
                componentMap.awaitingScroll = false;
                scrollPosition.y = compHeaderRect.y - 30;
                if (scrollPosition.y < 0)
                {
                    scrollPosition.y = 0;
                }
            }
            Rect rect1 = new Rect(compHeaderRect);
            rect1.x = 0;
            EditorUtils.DrawLineOverRect(rect1, -1);
            if (!flag)
            {
                EditorUtils.DrawLineUnderRect(rect1, CustomColors.SoftShadow, -1, 2);
            }
            else
            {
                EditorUtils.DrawLineUnderRect(rect1, CustomColors.MediumShadow);
                Rect underRect = new Rect(rect1);
                underRect.y += 22;
                underRect.height = 4;
                EditorUtils.DrawFadeToBottom(underRect, CustomColors.SoftShadow);
            }
            if (!flag)
            {
                EditorUtils.DrawLineUnderRect(rect1);
            }
        }

        float GetComponentHeightForMap(ComponentMap componentMap)
        {
            float currentHeight = 0;
            if (GetActiveTab() != null)
            {
                foreach (var map in GetActiveTab().componentMaps)
                {
                    if (map.index < componentMap.index && !map.isFilteredOut)
                    {
                        currentHeight += map.height;
                    }
                }
            }
            return currentHeight;
        }
        internal bool IsComponentCulled(ComponentMap componentMap, float yPosition = 0)
        {
            if (componentMap == null)
            {
                return false;
            }
            if (!componentCulling || componentMap.height == -1 || componentMap.height == 22)
            {
                return false;
            }
            if (componentMap.isCulled)
            {
                GUILayout.Space(componentMap.height - 22);
                return true;
            }
            return false;
        }

        internal void UpdateComponentScrollRect(float startY, float endY)
        {
            if (startY == 1 || endY == 1)
            {
                return;
            }
            componentScrollRect = new Rect(0, scrollPosition.y, position.width, endY - startY);
        }
        private bool DrawComponentIsDragged(ComponentMap componentMap, Component component, int index, out bool isDragged, out bool isCopied)
        {
            isDragged = false;
            isCopied = false;
            if (pendingOperation != null && pendingOperation.draggedComponent == component)
            {
                isDragged = true;
                if (!componentMap.foldout)
                {
                    GUILayout.Space(10);

                }
                if (EditorUtils.IsCtrlHeld())
                {
                    isCopied = true;
                }
            }
            else
            {
                GUILayout.Space(0);
            }
            GUI.SetNextControlName("Component" + index);
            GUI.enabled = true;
            if (isDragged && !isCopied)
            {
                GUI.enabled = false;
            }
            return isDragged;
        }
        private void DrawComponentSection(Component component, Editor editor, int index, Component[] components)
        {
            if (component == null)
            {
                ShowMissingComponent();
                return;
            }
            else if (editor == null)
            {
                return;
            }
            if (componentFoldouts_ == null || componentFoldouts_.Length != componentEditors.Length)
            {
                componentFoldouts_ = new bool[componentEditors.Length];
            }
            bool isLastComponent = index == components.Length - 1;
            EditorGUILayout.BeginVertical();
            DrawDragSpace(index);
            EditorGUILayout.BeginHorizontal(CustomGUIStyles.InspectorButtonStyle);
            ComponentMap componentMap = GetActiveTab().GetFoldoutMapForComponent(component, editor);
            componentMap.index = index;
            bool componentHidden = !EditorUtils.HasVisibleFields(editor);
            bool isDragging = DrawComponentIsDragged(componentMap, component, index, out bool isDragged, out bool isCopied);
            Behaviour behaviour = component as Behaviour;
            bool isBehaviour = behaviour != null;
            bool wasEnabled = isBehaviour && behaviour.enabled;
            if (isBehaviour)
            {
                EditorGUI.BeginChangeCheck();
            }
            bool flag = EditorGUILayout.InspectorTitlebar(componentMap.foldout, editor.serializedObject.targetObject, !componentHidden);
            if (isBehaviour)
            {
                if (flag)
                {
                    isDragging = false;
                }
                if (DrawComponentEnabledToggle(editor, behaviour, wasEnabled, isDragging))
                {
                    flag = componentMap.foldout;
                }
            }

            if (componentHidden)
            {
                flag = true;
            }

            EditorGUILayout.EndHorizontal();
            Rect compHeaderRect = GUILayoutUtility.GetLastRect();
            DrawAfterComponentBar(isDragged, isCopied, componentMap, compHeaderRect, index, flag);
            bool changed = flag != componentFoldouts_[index];
            if (changed)
            {
                bool shouldIgnore = (DragAndDrop.objectReferences.Length > 0 && Event.current.mousePosition.x > 60) || triggeringADrag;
                if (!shouldIgnore)
                {
                    componentFoldouts_[index] = flag;
                    componentMap.foldout = flag;
                }
            }
            if (componentMap.foldout && !componentHidden)
            {
                if (!IsComponentCulled(componentMap))
                {
                    EditorGUILayout.BeginVertical(CustomGUIStyles.CompStyle);

                    if (!GetActiveTab().multiEditMode)
                    {
                        DrawComponentEditorTools(component, compHeaderRect);
                    }
                    EditorGUI.BeginChangeCheck();
                    if (!GetActiveTab().debug && !HasUnityEvent(editor))
                    {
                        editor.OnInspectorGUI();
                    }
                    else
                    {
                        editor.DrawDefaultInspector();
                    }
                    if (EditorGUI.EndChangeCheck() && EditorUtils.IsMaterialComponent(editor.target as Component))
                    {
                        GetActiveTab().MarkMaterialsForRebuild();
                        Repaint();
                    }
                    EditorGUILayout.EndVertical();

                    MaterialMap materialMap = GetActiveTab().GetMaterialMapForComponent(component);
                    List<Material> materialsList = materialMap.materials;
                    if (materialsList.Count > 0)
                    {
                        EditorUtils.DrawMaterials(materialMap, component);
                    }
                    else
                    {
                        EditorGUILayout.BeginVertical();
                        GUILayout.Space(2);
                        EditorGUILayout.EndVertical();
                    }
                }
            }

            EditorGUILayout.EndVertical();
            UpdateComponentHeight(componentMap);
            if (isLastComponent)
            {
                EditorUtils.DrawUnderLastComponent();
            }
            /*
            float height = componentRect.height;
            if (Event.current.type == EventType.Repaint)
            {
                if (componentMap.foldout && height < 30)
                {
                    componentMap.foldout = false;
                    componentMap.hidden = true;
                    Repaint();
                }
                else if (componentMap.foldout)
                {
                    componentMap.hidden = false;
                }
            } */
            GUI.enabled = true;
            HandleDragOperations(index, component, components, editor);
        }

        internal static void UpdateComponentHeight(ComponentMap componentMap)
        {
            Rect componentRect = GUILayoutUtility.GetLastRect();
            if ((componentMap.height == -1 || componentMap.height != componentRect.height) && componentRect.height != 1 && componentMap.foldout)
            {
                componentMap.height = componentRect.height;
            }
            else if (!componentMap.foldout && componentMap.height != 22)
            {
                componentMap.height = 22;
                componentMap.isCulled = false;
            }
        }

        internal static bool HasUnityEvent(Editor editor)
        {
            if (!odinInspectorPresent)
            {
                return false;
            }
            if (editor == null || editor.serializedObject == null)
                return false;

            SerializedProperty iterator = editor.serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (iterator.propertyType == SerializedPropertyType.Generic &&
                    iterator.type == "UnityEvent")
                {
                    return true;
                }
            }
            return false;
        }
        internal static void UpdateLabelSize()
        {
            if (!MainCoInspector)
            {
                return;
            }
            if (MainCoInspector)
            {
                EditorGUIUtility.wideMode = true;
                EditorGUIUtility.labelWidth = MainCoInspector.position.width / 2.7f;

            }
        }
        private void HandleDragOperations(int index, Component component, Component[] components, Editor editor)
        {
            bool areWeDragging = DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0 && !triggeringADrag && !DraggingPrefabComponents();
            if (!areWeDragging)
            {
                return;
            }
            bool escape = Event.current.keyCode == KeyCode.Escape;
            if (escape)
            {
                performPasteComponent = -1;
                pendingComponentDrag = false;
                pendingOperation = null;
                ignoreNextDragEvent = true;
                DragAndDrop.objectReferences = null;
                return;
            }
            Rect componentRect = GUILayoutUtility.GetLastRect();
            componentRect.height += 5;
            UnityObject[] draggedObjects = DragAndDrop.objectReferences;
            Rect adjustedRect = new Rect(componentRect);
            int threshold;
            {
                if (componentFoldouts_[index] == true)
                {
                    threshold = 20;
                    adjustedRect.height -= 20;
                    adjustedRect.y += 10;
                }
                else
                {
                    threshold = 20;
                    adjustedRect.height -= 10;
                    adjustedRect.y -= 5;
                }
            }
            float distanceFromBottom = Event.current.mousePosition.y - (adjustedRect.y + adjustedRect.height);
            bool isNegative = adjustedRect.height < 0;
            Rect realInfluenceRect = new Rect(adjustedRect.x, mousePosition.y - 10, adjustedRect.width, 20);
            bool isMouseBelowRect = !escape && EditorWindow.mouseOverWindow == this && distanceFromBottom >= 0 && distanceFromBottom <= threshold && !isNegative;
            if (draggedObjects.Length == 1 && draggedObjects[0] is Component)
            {
                Component draggedComponent = draggedObjects[0] as Component;
                if (draggedComponent is Transform || draggedComponent is RectTransform)
                {
                    return;
                }
                GameObject owner = draggedComponent.gameObject;
                bool isSelf = owner == component.gameObject;
                if (isMouseBelowRect)
                {
                    StartOrUpdateDragOperation(index, component, draggedComponent, realInfluenceRect, editor);
                    bool copy = EditorUtils.IsCtrlHeld();
                    bool prefabCantModify = !IsGameObjectInPrefabMode(component.gameObject) && PrefabUtility.IsPartOfAnyPrefab(component.gameObject) && !copy;
                    if (prefabCantModify)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        EditorUtils.DrawLineUnderRect(componentRect, CustomColors.CustomRed, -5, 5);
                        performPasteComponent = 4;
                    }
                    else
                    {
                        bool canCopy;
                        if (copy)
                        {
                            canCopy = CanAddMultipleTimes(draggedComponent) && !(draggedComponent is Graphic) && !(draggedComponent is Renderer);
                            if (!canCopy && !isSelf)
                            {
                                canCopy = true;
                            }
                            if (canCopy)
                            {
                                alreadyMovingComponent = false;
                                EditorUtils.DrawLineUnderRect(componentRect, CustomColors.CustomGreen, -5, 5);
                                performPasteComponent = 2;
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            }
                            else
                            {
                                alreadyMovingComponent = false;
                                EditorUtils.DrawLineUnderRect(componentRect, CustomColors.CustomRed, -5, 5);
                                performPasteComponent = 1;
                                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                            }
                        }
                        else
                        {
                            alreadyMovingComponent = false;
                            EditorUtils.DrawLineUnderRect(componentRect, CustomColors.CustomBlue, -5, 5);
                            performPasteComponent = 0;
                            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                        }
                    }
                }
            }
            else if (draggedObjects.Length > 0)
            {
                List<UnityObject> validAssets = new List<UnityObject>();
                List<GameObject> invalidGameObjects = new List<GameObject>();
                bool repeatingAsset = false;
                foreach (UnityObject draggedObject in draggedObjects)
                {
                    if (EditorUtils.IsValidAssetType(draggedObject, component.gameObject, out repeatingAsset) && !(draggedObject is GameObject))
                    {
                        validAssets.Add(draggedObject);
                    }
                    else if (draggedObject is GameObject)
                    {
                        invalidGameObjects.Add(draggedObject as GameObject);
                    }
                }
                if (isMouseBelowRect)
                {
                    if (validAssets.Count > 0)
                    {
                        alreadyMovingComponent = false;
                        if (!PrefabUtility.IsPartOfAnyPrefab(component.gameObject))
                        {
                            EditorUtils.DrawLineUnderRect(componentRect, CustomColors.CustomGreen, -5, 5);
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        }
                        else
                        {
                            EditorUtils.DrawLineUnderRect(componentRect, CustomColors.CustomRed, -5, 5);
                            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        }
                        pendingComponentDrag = true;
                        pendingOperation = new ComponentDragOperation
                        {
                            isMouseBelowRect = true,
                            isAsset = true,
                            assets = validAssets,
                            targetIndex = index,
                            targetObject = component.gameObject,
                            mouseOverRect = realInfluenceRect
                        };
                    }
                    else if (invalidGameObjects.Count < draggedObjects.Length)
                    {
                        if (repeatingAsset)
                        {
                            pendingComponentDrag = true;
                            pendingOperation = new ComponentDragOperation
                            {
                                isMouseBelowRect = true,
                                errored = true,
                                targetIndex = index,
                                draggedComponent = component,
                                targetObject = component.gameObject,
                                mouseOverRect = realInfluenceRect
                            };
                        }
                        alreadyMovingComponent = false;
                        EditorUtils.DrawLineUnderRect(componentRect, CustomColors.CustomRed, -5, 5);
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    }
                }
            }
            if (pendingOperation != null && pendingOperation.targetIndex == index && index == components.Length - 1)
            {
                GUILayout.Space(5);
            }
            else
            {
                GUILayout.Space(0);
            }
        }
        private void StartOrUpdateDragOperation(int index, Component component, Component draggedComponent, Rect realInfluenceRect, Editor editor)
        {
            pendingComponentDrag = true;
            if (pendingOperation == null)
            {
                pendingOperation = new ComponentDragOperation();
            }
            pendingOperation.targetObject = component.gameObject;
            pendingOperation.isMouseBelowRect = true;
            pendingOperation.draggedEditor = editor;
            pendingOperation.sourceIndex = Array.IndexOf(component.gameObject.GetComponents<Component>(), draggedComponent);
            pendingOperation.targetIndex = index;
            pendingOperation.draggedComponent = draggedComponent;
            if (pendingOperation.sourceTabIndex == -1)
            {
                pendingOperation.sourceTabIndex = activeIndex;
            }
            pendingOperation.foldoutOrigin = tabs[pendingOperation.sourceTabIndex].GetFoldoutForComponent(draggedComponent, editor);
            pendingOperation.isSelf = component.gameObject == draggedComponent.gameObject;
            pendingOperation.errored = performPasteComponent == 1;
            pendingOperation.removeAfter = performPasteComponent == 0;
            pendingOperation.isCopy = performPasteComponent == 2;
            pendingOperation.prefabError = performPasteComponent == 4;
            pendingOperation.mouseOverRect = realInfluenceRect;
            GUI.changed = true;
            DragAndDrop.visualMode = DragAndDropVisualMode.Link;
        }

        void ShowMissingComponent(bool multi = false)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Missing Script!");
            EditorGUILayout.EndHorizontal();
            Rect rect = EditorUtils.GetLastLineRect();
            EditorUtils.DrawLineOverRect(rect, 0);
            EditorUtils.DrawLineOverRect(rect, CustomColors.MediumShadow, 1);
            EditorUtils.DrawLineUnderRect(rect, 0);
            EditorUtils.DrawLineUnderRect(rect, CustomColors.SoftShadow, 1, 2);
            EditorGUILayout.Space(10);
            string message = "The associated script can not be loaded.\nPlease fix any compile errors or assign a valid script in the regular Inspector.";
            if (multi)
            {
                message = "One or more Components have missing scripts.\nPlease fix any compile errors or assign a valid script in the regular Inspector.";
            }
            EditorGUILayout.HelpBox(message, MessageType.Warning);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIContent content = CustomGUIContents.MissingComponentContent;
            content.image = CustomGUIContents.EditContentDefault.image;
            if (GUILayout.Button(content, GUILayout.Width(130)))
            {
                if (IsActiveTabValidMulti())
                {
                    PopUpInspectorWindow(GetActiveTab().targets, false);
                }
                else
                {
                    PopUpInspectorWindow(new GameObject[] { GetActiveTab().target }, false);
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        void ShowPrefabMissingComponent(bool multi = false)
        {
            string message = "Prefab has missing scripts. Open Prefab in regular Inspector to fix the issue.";
            if (multi)
            {
                message = "One or more Prefabs have missing scripts.\nOpen them individually in regular Inspector to fix the issue.";
            }
            EditorGUILayout.HelpBox(message, MessageType.Warning);
            if (multi)
            {
                GUI.enabled = false;
            }
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIContent content = CustomGUIContents.MissingComponentContent;
            if (GUILayout.Button(content, GUILayout.Width(130)))
            {
                PopUpInspectorWindow(new GameObject[] { targetObject as GameObject }, false);
                OpenPrefab();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (multi)
            {
                GUILayout.Space(10);
            }
            GUI.enabled = true;
        }
        static bool DraggingPrefabComponents()
        {
            UnityObject[] draggedObjects = DragAndDrop.objectReferences;
            if (draggedObjects == null || draggedObjects.Length == 0)
            {
                return false;
            }
            foreach (UnityObject draggedObject in draggedObjects)
            {
                if (draggedObject is Component)
                {
                    Component component = draggedObject as Component;
                    if (component.gameObject != null && EditorUtils.IsAPrefabAsset(component.gameObject))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private void DrawPrefabComponentInspectors(GameObject gameObject)
        {
            if (CanSkipRefresh())
            {
                return;
            }
            if (gameObject == null)
            {
                return;
            }
            Component[] components = gameObject.GetComponents<Component>();
            if (components == null || components.Any(c => c == null))
            {
                ShowPrefabMissingComponent();
                return;
            }
            if (runtimePrefabComponents != null && runtimePrefabComponents.Length > 0)
            {
                if (components != null)
                {
                    if (components.Length != runtimePrefabComponents.Length)
                    {
                        ReinitializePrefabComponentEditors();
                        Repaint();
                    }
                    else
                    {
                        for (int i = 0; i < components.Length; i++)
                        {
                            if ((components[i] == null && runtimePrefabComponents[i] == null) || components[i].GetType() != runtimePrefabComponents[i].GetType())
                            {
                                ReinitializePrefabComponentEditors();
                                Repaint();
                                break;
                            }
                        }
                    }
                }
            }
            runtimePrefabComponents = new Component[components.Length];
            Array.Copy(components, runtimePrefabComponents, components.Length);
            if (prefabEditors == null || prefabEditors.Length != components.Length)
            {
                ReinitializePrefabComponentEditors();
            }
            else if (components != null && prefabEditors != null)
            {
                if (components.Length > prefabEditors.Length)
                {
                    ReinitializePrefabComponentEditors();
                    scrollPosition.y += 1000;
                }
            }
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < components.Length; i++)
            {
                if (prefabEditors == null || i >= prefabEditors.Length)
                {
                    ReinitializePrefabComponentEditors();
                    break;
                }
                if (prefabEditors[i] == null || !prefabEditors[i].target || prefabEditors[i].target != components[i])
                {
                    if (prefabEditors[i] != null)
                    {
                        DestroyIfNotNull(prefabEditors[i]);
                    }
                    else
                    {
                        ReinitializePrefabComponentEditors();
                    }
                }
                if (prefabEditors[i] != null)
                {
                    DrawPrefabComponentSection(components[i], prefabEditors[i], i);
                }
            }
            EditorGUILayout.EndVertical();
            EditorUtils.DrawLineUnderRect(CustomColors.HardShadow);
            GUILayout.Space(1);

            if (prefabFoldouts_ != null && prefabFoldoutsChangeTracker_ != null)
            {
                if (EditorUtils.HaveBoolArraysChanged(prefabFoldouts_, prefabFoldoutsChangeTracker_))
                {
                    ResetAssetViewSize();
                    prefabFoldoutsChangeTracker_ = new bool[prefabFoldouts_.Length];
                    Array.Copy(prefabFoldouts_, prefabFoldoutsChangeTracker_, prefabFoldouts_.Length);
                }
            }

            PrefabMaterialMapManager.RebuildIfNecessary();
        }

        private void DrawPrefabComponentSection(Component component, Editor editor, int index)
        {
            if (component == null || editor == null || editor.target == null || prefabEditors == null)
            {
                return;
            }
            Color color = GUI.color;
            if (prefabFoldouts_ == null || prefabFoldouts_.Length != prefabEditors.Length)
            {
                prefabFoldouts_ = new bool[prefabEditors.Length];
                for (int i = 0; i < prefabFoldouts_.Length; i++)
                {
                    prefabFoldouts_[i] = true;
                }
            }
            GUI.enabled = !EditorUtils.IsAnImportedObject(component.gameObject);
            bool componentHidden = !EditorUtils.HasVisibleFields(editor);
            Behaviour behaviour = component as Behaviour;
            bool isBehaviour = behaviour != null;
            bool wasEnabled = isBehaviour && behaviour.enabled;
            if (isBehaviour)
            {
                EditorGUI.BeginChangeCheck();
            }
            bool flag = EditorGUILayout.InspectorTitlebar(prefabFoldouts_[index], component, !componentHidden);
            if (isBehaviour)
            {
                if (DrawComponentEnabledToggle(editor, behaviour, wasEnabled))
                {
                    flag = prefabFoldouts_[index];
                }
            }
            prefabFoldouts_[index] = flag;
            if (componentHidden)
            {
                prefabFoldouts_[index] = !collapsePrefabComponents;
            }
            GUI.enabled = true;
            Rect compHeaderRect = GUILayoutUtility.GetLastRect();
            EditorUtils.DrawLineOverRect(compHeaderRect, -1);
            EditorUtils.DrawLineUnderRect(compHeaderRect, CustomColors.SoftShadow, -1, 2);
            GUI.color = color;
            if (prefabFoldouts_[index])
            {
                EditorGUILayout.BeginVertical(CustomGUIStyles.CompStyle);
                EditorGUI.BeginChangeCheck();
                if (!debugAsset && !HasUnityEvent(editor))
                {
                    editor.OnInspectorGUI();
                }
                else
                {
                    editor.DrawDefaultInspector();
                }
                if (EditorGUI.EndChangeCheck() && EditorUtils.IsMaterialComponent(editor.target as Component))
                {
                    PrefabMaterialMapManager.timeToRebuild = true;
                    Repaint();
                }
                EditorGUILayout.EndVertical();
                MaterialMap materialMap = PrefabMaterialMapManager.GetMaterialMapForComponent(component);
                if (materialMap.materials.Count > 0)
                {
                    EditorUtils.DrawMaterials(materialMap, component, true);
                }
                GUILayout.Space(5);
            }
        }
        internal void DrawMultiPrefabInspector()
        {
            if (targetObjects == null || targetObjects.Length == 0)
            {
                return;
            }
            if (CanSkipRefresh())
            {
                return;
            }
            if (prefabEditors == null)
            {
                //   Debug.LogAssertionFormat("No component editors");
                ReinitializePrefabComponentEditors();
                return;
            }
            Component[] components = null;
            GameObject runtimePrefab = targetObjects[0] as GameObject;
            if (runtimePrefab != null)
            {
                components = runtimePrefab.GetComponents<Component>();
            }
            if (components == null || components.Any(c => c == null))
            {
                ShowPrefabMissingComponent(true);
                return;
            }
            if (runtimePrefabComponents != null && runtimePrefabComponents.Length > 0)
            {
                if (runtimePrefabComponents != null && runtimePrefabComponents.Length > 0)
                {
                    if (components != null)
                    {
                        if (components.Length != runtimePrefabComponents.Length)
                        {
                            ReinitializePrefabComponentEditors();
                            Repaint();
                        }
                        else
                        {
                            for (int i = 0; i < components.Length; i++)
                            {
                                if (components[i].GetType() != runtimePrefabComponents[i].GetType())
                                {
                                    ReinitializePrefabComponentEditors();
                                    Repaint();
                                    break;
                                }
                            }
                        }
                    }
                }
                runtimePrefabComponents = new Component[components.Length];
                Array.Copy(components, runtimePrefabComponents, components.Length);
            }
            for (int i = 0; i < prefabEditors.Length; i++)
            {
                if (prefabEditors[i] == null)
                {
                    ReinitializePrefabComponentEditors();
                    break;
                }
                bool hasDestroyedTarget = false;
                foreach (var target in prefabEditors[i].targets)
                {
                    if (!target)
                    {
                        hasDestroyedTarget = true;
                        break;
                    }
                }
                if (hasDestroyedTarget)
                {
                    ReinitializePrefabComponentEditors();
                    break;
                }
                if (prefabFoldouts_ == null || prefabFoldouts_.Length != prefabEditors.Length)
                {
                    prefabFoldouts_ = new bool[prefabEditors.Length];
                    for (int u = 0; u < prefabFoldouts_.Length; u++)
                    {
                        prefabFoldouts_[u] = true;
                    }
                }
                GUI.enabled = !EditorUtils.IsAnImportedObject(runtimePrefab);
                bool componentHidden = !EditorUtils.HasVisibleFields(prefabEditors[i]);
                Behaviour behaviour = prefabEditors[i].targets[0] as Behaviour;
                bool isBehaviour = behaviour != null;
                bool wasEnabled = isBehaviour && behaviour.enabled;
                if (isBehaviour)
                {
                    EditorGUI.BeginChangeCheck();
                }
                bool flag = EditorGUILayout.InspectorTitlebar(prefabFoldouts_[i], prefabEditors[i].targets, !componentHidden);
                if (isBehaviour)
                {
                    if (DrawMultiComponentEnabledToggle(prefabEditors[i], wasEnabled))
                    {
                        flag = prefabFoldouts_[i];
                    }
                }
                prefabFoldouts_[i] = flag;
                if (componentHidden)
                {
                    prefabFoldouts_[i] = !collapsePrefabComponents;
                }
                GUI.enabled = true;
                EditorUtils.DrawLineOverRect(GUILayoutUtility.GetLastRect(), -1);
                EditorUtils.DrawLineUnderRect(GUILayoutUtility.GetLastRect(), CustomColors.SoftShadow, -1, 2);
                if (prefabFoldouts_[i])
                {
                    EditorGUILayout.BeginVertical(CustomGUIStyles.CompStyle);
                    EditorGUI.BeginChangeCheck();
                    if (debugAsset || HasUnityEvent(prefabEditors[i]))
                    {
                        prefabEditors[i].DrawDefaultInspector();
                    }
                    else
                    {
                        prefabEditors[i].OnInspectorGUI();
                    }
                    if (EditorGUI.EndChangeCheck() && EditorUtils.IsMaterialComponent(prefabEditors[i].targets[0] as Component))
                    {
                        PrefabMaterialMapManager.timeToRebuild = true;
                        Repaint();
                    }
                    EditorGUILayout.EndVertical();
                    MaterialMap materialMap = PrefabMaterialMapManager.GetMaterialMapForComponent(prefabEditors[i].targets[0] as Component, prefabEditors[i]);

                    if (materialMap.materials.Count > 0)
                    {
                        EditorUtils.DrawMaterials(materialMap, prefabEditors[i].targets[0] as Component, true);
                    }
                    GUILayout.Space(2);
                }
            }
            PrefabMaterialMapManager.RebuildIfNecessary();
            EditorUtils.MultiEditFooter(differentPrefabComponents);

            if (prefabFoldouts_ != null && prefabFoldoutsChangeTracker_ != null)
            {
                if (EditorUtils.HaveBoolArraysChanged(prefabFoldouts_, prefabFoldoutsChangeTracker_))
                {
                    alreadyCalculatedHeight = false;
                    userHeight = suggestedHeight;
                    prefabFoldoutsChangeTracker_ = new bool[prefabFoldouts_.Length];
                    Array.Copy(prefabFoldouts_, prefabFoldoutsChangeTracker_, prefabFoldouts_.Length);
                    Repaint();
                }
            }
        }
        private void RebuildPrefabMultiComponentEditors()
        {
            if (prefabEditors != null)
            {
                for (int i = 0; i < prefabEditors.Length; i++)
                {
                    if (prefabEditors[i] != null)
                    {
                        DestroyImmediate(prefabEditors[i]);
                    }
                }
            }
            differentPrefabComponents = false;
            GameObject[] prefabs = new GameObject[targetObjects.Length];
            for (int i = 0; i < targetObjects.Length; i++)
            {
                prefabs[i] = targetObjects[i] as GameObject;
            }
            Dictionary<Type, List<List<Component>>> map = EditorUtils.OrderedComponentMap(prefabs, this, true);
            if (map == null)
            {
                return;
            }
            List<Editor> editorList = new List<Editor>();
            foreach (var entry in map)
            {
                List<List<Component>> componentLists = entry.Value;
                int componentCount = componentLists[0].Count;
                for (int i = 0; i < componentCount; i++)
                {
                    Component[] editorTargets = componentLists.Select(list => list[i]).ToArray();
                    Editor editor;
                    if (editorTargets.Length > 1)
                    {
                        editor = Editor.CreateEditor(editorTargets);
                    }
                    else
                    {
                        editor = Editor.CreateEditor(editorTargets[0]);
                    }
                    editorList.Add(editor);
                }
            }
            prefabEditors = editorList.ToArray();
            prefabFoldouts_ = new bool[prefabEditors.Length];
            prefabFoldoutsChangeTracker_ = new bool[prefabEditors.Length];
            for (int i = 0; i < prefabEditors.Length; i++)
            {
                prefabFoldouts_[i] = !collapsePrefabComponents;
                prefabFoldoutsChangeTracker_[i] = !collapsePrefabComponents;
            }
            SetAllAssetEditorsDebugTo(debugAsset);
        }

        void ShowImportSettingsMessage()
        {
            EditorGUILayout.HelpBox("If Apply/Revert don't show up, Apply changes with the toolbar icon!", MessageType.Warning);
            GUILayout.Space(5);
        }
        private bool IsAnyAssetDirty(UnityObject[] objects)
        {
            if (objects == null)
            {
                return false;
            }
            foreach (var obj in objects)
            {
                if (EditorUtility.IsDirty(obj))
                {
                    return true;
                }
                if (IsEditorValid(assetImportSettingsEditor))
                {
                    if (assetImportSettingsEditor.HasModified())
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        private bool IsAssetDirty(UnityObject obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (EditorUtility.IsDirty(obj))
            {
                return true;
            }
            if (IsEditorValid(assetImportSettingsEditor))
            {
                if (assetImportSettingsEditor.HasModified())
                {
                    return true;
                }
            }
            return false;
        }

        internal bool IsAssetImportSettingsDirty()
        {
            if (assetImportSettingsEditor == null || assetImportSettingsEditor.targets == null || assetImportSettingsEditor.targets.Length == 0)
            {
                return false;
            }

            if (assetImportSettingsEditor.HasModified())
            {
                return true;
            }
            return false;
        }

        bool CheckForApplyRevertOnClose()
        {
            if (IsAssetImportSettingsDirty())
            {
                int result = EditorUtils.ShowUnappliedImportSettings(assetImportSettingsEditor);
                if (result == 1)
                {
                    return false;
                }
                if (result == 0)
                {
                    Reflected.ApplyChanges(assetImportSettingsEditor);
                }
                if (result == 2)
                {
                    Reflected.DiscardChanges(assetImportSettingsEditor);
                }

            }
            return true;
            /*
            if (assetEditor == null || assetEditor.target == null || assetImportSettingsEditor == null || assetImportSettingsEditor.target == null || Reflected.checkForCloseMethod == null)
            {
                Debug.Log(Reflected.checkForCloseMethod == null);
                return true;
            }
            if (!IsEditorValid(assetImportSettingsEditor))
            {
                                Debug.Log("Asset Editor or Asset Import Settings Editor is null!");

                return true;
            }
            if (!assetImportSettingsEditor.HasModified())
            {
                                Debug.Log("Asset Editor or Asset Import Settings Editor is null!");

                return true;
            }
            List<string> assetPaths = new List<string>();
            if (targetObject && targetObjects == null)
            {
                if (targetObject)
                {
                    assetPaths.Add(AssetDatabase.GetAssetPath(targetObject));
                }
            }
            else if (targetObjects != null)
            {
                foreach (var target in targetObjects)
                {
                    assetPaths.Add(AssetDatabase.GetAssetPath(target));
                }
            }
            if (assetPaths.Count == 0)
            {
                return true;
            }
            bool result = (bool)Reflected.checkForCloseMethod.Invoke(assetImportSettingsEditor, new object[] { assetPaths });
            return result;*/
        }
        internal void DrawTextAssetPreview(UnityObject textAsset)
        {
            if (textAsset != null)
            {
                Shader shader = textAsset as Shader;
                string text = "";
                if (shader != null)
                {
                    text = File.ReadAllText(AssetDatabase.GetAssetPath(shader));
                }
                else
                {
                    TextAsset asset = textAsset as TextAsset;
                    if (asset != null)
                    {
                        text = asset.text;
                    }
                }
                EditorGUILayout.BeginVertical();
                Color color = GUI.color;
                GUI.color = CustomColors.TextPreviewColor;
                {
                    EditorGUILayout.TextArea(text, CustomGUIStyles.TextStyle());
                }
                GUI.color = color;
                EditorGUILayout.EndVertical();
            }
        }
        internal void HandleAssetHistory(UnityEngine.Object[] newTargets)
        {
            if (newTargets == null || newTargets.Length == 0)
            {
                return;
            }
            UnityObject[] objects = new UnityObject[newTargets.Length];
            Array.Copy(newTargets, objects, newTargets.Length);
            var newHistoryAsset = new HistoryAssets(objects);
            CleanHistoryAssets();
            var existingIndex = historyAssets.FindIndex(entry =>
                entry.assetGUIDs.OrderBy(guid => guid).SequenceEqual(newHistoryAsset.assetGUIDs.OrderBy(guid => guid)));
            if (existingIndex != -1)
            {
                historyAssets.RemoveAt(existingIndex);
            }
            historyAssets.Insert(0, newHistoryAsset);
            if (historyAssets.Count > 11)
            {
                historyAssets.RemoveAt(11);
            }
        }
        private void CleanHistoryAssets()
        {
            if (historyAssets == null)
            {
                historyAssets = new List<HistoryAssets>();
                return;
            }
            foreach (var entry in historyAssets)
            {
                entry.assetGUIDs = entry.assetGUIDs
                    .Where(guid => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(AssetDatabase.GUIDToAssetPath(guid)) != null)
                    .ToArray();
            }
            historyAssets.RemoveAll(entry => entry.assetGUIDs.Length == 0);
        }
        private UnityEngine.Object FindAssetByGUID(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
        }
        private void BackToPreviousAsset()
        {
            CleanHistoryAssets();
            if (historyAssets.Count == 0)
            {
                return;
            }
            int currentIndex = 0;
            if (EditorUtils.AssetAlreadyTarget(FindAssetByGUID(historyAssets[0].assetGUIDs[0]), this))
            {
                currentIndex = 1;
            }
            if (currentIndex >= historyAssets.Count)
            {
                return;
            }
            var historyEntry = historyAssets[currentIndex];
            if (historyEntry.assetGUIDs.Length == 1)
            {
                var asset = FindAssetByGUID(historyEntry.assetGUIDs[0]);
                if (asset != null)
                {
                    SetTargetAsset(asset, true);
                }
            }
            else if (historyEntry.assetGUIDs.Length > 1)
            {
                var assets = historyEntry.assetGUIDs.Select(FindAssetByGUID).Where(a => a != null).ToArray();
                SetTargetAssets(assets, true);
            }
        }
        internal void ShowHistoryContextMenu()
        {
            if (Event.current.button == 1)
            {
                BackToPreviousAsset();
                return;
            }
            GenericMenu menu = new GenericMenu();
            CleanHistoryAssets();
            bool firstIsCurrent = EditorUtils.AssetsAlreadyTargets(historyAssets[0].assetGUIDs.Select(FindAssetByGUID).ToArray(), this);
            int adjustIndex = 1;
            if (firstIsCurrent)
            {
                adjustIndex = 0;
            }
            if (historyAssets.Count < 1)
            {
                menu.AddDisabledItem(new GUIContent("No previous selections found!"));
            }
            else
            {
                for (int i = 0; i < historyAssets.Count; i++)
                {
                    if (i == 1 && firstIsCurrent)
                    {
                        menu.AddSeparator("");
                    }
                    bool status = false;
                    var historyEntry = historyAssets[i];
                    if (historyEntry.assetGUIDs.Length == 1)
                    {
                        var asset = FindAssetByGUID(historyEntry.assetGUIDs[0]);
                        status = EditorUtils.AssetAlreadyTarget(asset, this);
                        if (asset != null)
                        {
                            string preName = i + adjustIndex + ". ";
                            if (status)
                            {
                                preName = "✓ ";
                            }
                            string assetPath = AssetDatabase.GetAssetPath(asset);
                            bool isFolder = AssetDatabase.IsValidFolder(assetPath);
                            string assetNameWithExtension = !isFolder ?
                                                            CleanSlash(asset.name) + Path.GetExtension(assetPath).ToLower() :
                                                            asset.name;
                            menu.AddItem(new GUIContent(preName + assetNameWithExtension), false, () => SetTargetAsset(asset, true));
                        }
                    }
                    else if (historyEntry.assetGUIDs.Length > 1)
                    {
                        var assets = historyEntry.assetGUIDs.Select(FindAssetByGUID).Where(a => a != null).ToArray();
                        status = EditorUtils.AssetsAlreadyTargets(assets, this);
                        string menuPath = $"({historyEntry.assetGUIDs.Length}) Assets/";
                        string preName = i + adjustIndex + ". ";
                        if (status)
                        {
                            preName = "✓ ";
                        }
                        menuPath = preName + menuPath;
                        menu.AddItem(new GUIContent(menuPath + "- Re-select All -"), false, () =>
                        {
                            SetTargetAssets(assets, true);
                        });
                        for (int j = 0; j < historyEntry.assetGUIDs.Length; j++)
                        {
                            var guid = historyEntry.assetGUIDs[j];
                            var asset = FindAssetByGUID(guid);
                            if (asset != null)
                            {
                                status = EditorUtils.AssetAlreadyTarget(asset, this);
                                string _preName = j + 1 + ". ";
                                string assetPath = AssetDatabase.GetAssetPath(asset);
                                bool isFolder = AssetDatabase.IsValidFolder(assetPath);
                                string assetNameWithExtension = !isFolder ?
                                                                _preName + CleanSlash(asset.name) + Path.GetExtension(assetPath).ToLower() :
                                                                _preName + CleanSlash(asset.name);
                                menu.AddItem(new GUIContent(menuPath + assetNameWithExtension), status, () => SetTargetAsset(asset, true));
                            }
                        }
                    }
                }
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Wipe Asset History"), false, WipeAssetHistory);
            float height = 20 * (historyAssets.Count + 1);
            if (position.height - Event.current.mousePosition.y < height)
            {
                menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y - height, 0, 0));
            }
            else
            {
                menu.ShowAsContext();
            }
        }

        static string CleanSlash(string path)
        {
            return path.Replace("/", " ∕ ");
        }
        private void WipeAssetHistory()
        {
            if (EditorUtility.DisplayDialog("Wipe Asset History",
                "You sure about this?", "Yes", "No"))
            {
                historyAssets.Clear();
            }
        }
        static bool isText(UnityObject obj)
        {
            if (obj is TextAsset || obj is Shader)
            {
                return true;
            }
            return false;
        }
        public bool IsLastRectHovered()
        {
            if (Event.current == null)
            {
                return false;
            }
            Rect lastRect = GUILayoutUtility.GetLastRect();
            var position = Event.current.mousePosition;
            if (dragging && !GOdragging)
            {
                position = new Vector2(position.x, lastRect.y);
                int xLimit = 2;
                if (showHistory)
                {
                    xLimit = 45;
                }
                if (position.x < xLimit)
                {
                    dragTargetIndex = 0;
                    position.x = xLimit;
                }
            }
            return lastRect.Contains(position);
        }
        void ShowTabContextMenu(Rect buttonRect, int i, int click, GameObject item)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Tab to the Right"), false, () =>
                {
                    int previousIndex = activeIndex;
                    activeIndex = click;
                    AddTabNext(null);

                });
            menu.AddItem(new GUIContent("Duplicate Tab"), false, () =>
            {
                DuplicateTabNext(click);
            });
            menu.AddSeparator("");
            if (item || tabs[i].multiEditMode)
            {
                if (tabs[i].locked)
                {
                    menu.AddItem(new GUIContent("Unlock Tab"), false, () =>
                    {
                        tabs[click].locked = false;
                        UpdateAllWidths();
                    });
                }
                else
                {
                    menu.AddItem(new GUIContent("Lock Tab"), false, () =>
                    {
                        tabs[click].locked = true;
                        UpdateAllWidths();
                    });
                }
                if (!tabs[click].IsValidMultiTarget() && PrefabUtility.IsPartOfAnyPrefab(item))
                {
                    menu.AddSeparator("");
                    string typeGO = "Prefab Asset";
                    UnityObject go = PrefabUtility.GetCorrespondingObjectFromSource(item);
                    if (PoolCache.IsAnImportedObject(go))
                    {
                        typeGO = "Imported Object";
                        menu.AddItem(new GUIContent("Edit Imported Object"), false, () =>
                    {
                        UnityObject prefab = go as GameObject;
                        if (prefab)
                        {
                            string path = AssetDatabase.GetAssetPath(prefab);
                            OpenAsset(path);
                        }
                    });
                    }
                    else
                    {
                        menu.AddItem(new GUIContent("Open Root Prefab Asset in Scene Context"), false, () =>
                        {
                            UnityObject prefab = go as GameObject;
                            if (prefab)
                            {
                                string path = AssetDatabase.GetAssetPath(prefab);
                                DoOpenPrefabInScene(path, item);
                            }
                        });
                        menu.AddItem(new GUIContent("Open Root Prefab Asset in Isolation"), false, () =>
                        {
                            UnityObject prefab = go as GameObject;
                            if (prefab)
                            {
                                string path = AssetDatabase.GetAssetPath(prefab);
                                DoOpenPrefab(path);
                            }
                        });
                    }
                    menu.AddItem(new GUIContent("Ping " + typeGO), false, () =>
                    {
                        UnityObject prefab = go as GameObject;
                        if (prefab)
                        {
                            EditorWindow.GetWindow(Reflected.GetProjectWindowType());
                            EditorGUIUtility.PingObject(prefab);
                        }
                    });
                }
                else if (IsGameObjectInPrefabMode(item))
                {

                    GameObject prefab = GetPrefabStageRoot();
                    if (prefab)
                    {
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Ping Prefab Asset"), false, () =>
                        {
                            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(GetPrefabStageRootPath());
                            if (prefab)
                            {
                                EditorGUIUtility.PingObject(prefab);
                            }
                        });
                    }
                }
                menu.AddSeparator("");
                if (tabs[click].IsValidMultiTarget())
                {
                    if (!IsAlreadySelected(tabs[click].targets))
                    {
                        menu.AddItem(new GUIContent("Select in Hierarchy"), false, () =>
                        {
                            ignoreSelection = tabs[click].targets;
                            Selection.objects = tabs[click].targets;
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Select in Hierarchy"));
                    }
                }
                else if (item != Selection.activeGameObject)
                {
                    menu.AddItem(new GUIContent("Select in Hierarchy"), false, () =>
                    {
                        ignoreSelection = new GameObject[] { item };
                        Selection.objects = ignoreSelection;
                        Selection.activeGameObject = item;
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Select in Hierarchy"));
                }
                menu.AddItem(new GUIContent("Focus on Scene View"), false, () =>
               {
                   if (tabs[click].IsValidMultiTarget())
                   {
                       FocusOnSceneView(tabs[click].targets);
                   }
                   else
                   {
                       FocusOnSceneView(item);
                   }
               });
                menu.AddItem(new GUIContent("Ping in Hierarchy"), false, () =>
            {
                if (tabs[click].IsValidMultiTarget())
                {
                    EditorGUIUtility.PingObject(tabs[click].targets[0]);
                }
                else
                {
                    EditorGUIUtility.PingObject(item);
                }
            });
                if (!tabs[click].IsValidMultiTarget() && item != null)
                {
                    menu.AddItem(new GUIContent("Show In Local Hierarchy View"), false, () =>
                {
                    HierarchyPopup.ShowWindow(tabs[click].target, this, clickMousePosition);
                });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Show In Local Hierarchy View"));
                }
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Lock Tab"));
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("Select in Hierarchy"));
                menu.AddDisabledItem(new GUIContent("Ping in Hierarchy"));
            }
            menu.AddSeparator("");
            if (tabs.Count > 1)
            {
                menu.AddItem(new GUIContent("Close Tab"), false, () =>
            {
                CloseTab(click);
                targetGameObject = GetActiveTab().target;
            });
                menu.AddItem(new GUIContent("Close all other Tabs"), false, () =>
                {
                    SaveSettings();
                    List<TabInfo> _tabs = new List<TabInfo>(tabs);
                    TabInfo tab = _tabs[click];
                    foreach (var _tab in _tabs)
                    {
                        if (_tab != tab)
                        {
                            FixActiveIndex();
                            CloseTab(tabs.IndexOf(_tab), true, true);
                        }
                    }
                    activeIndex = 0;
                    FocusTab(activeIndex);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Close Tab"));
                menu.AddDisabledItem(new GUIContent("Close All Other Tabs"));
            }
            menu.AddSeparator("");
            if (AreThereClosedTabs())
            {
                menu.AddItem(new GUIContent("Restore Closed Tab"), false, () =>
                {
                    RestoreClosedTab();
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Restore Closed Tab"));
            }
            if (IsThereAPreviousSession())
            {
                menu.AddItem(new GUIContent("Restore Last Saved Session"), false, () =>
                {
                    ShowRecoverSessionDialogue();
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Restore Last Saved Session"));
            }
            menu.AddSeparator("");
            if (!tabs[click].newTab && tabs[click] != null)
            {
                menu.AddItem(new GUIContent("Normal"), !tabs[click].debug, () =>
                {
                    tabs[click].debug = false;
                    if (click == activeIndex)
                    {
                        ManageDebugMode(false);
                        Repaint();
                    }
                });
                menu.AddItem(new GUIContent("Debug"), tabs[click].debug, () =>
                {
                    tabs[click].debug = true;
                    if (click == activeIndex)
                    {
                        ManageDebugMode(true);
                        Repaint();
                    }
                });
                menu.AddSeparator("");
            }
            ShowSettingsMenu(menu, true);
            float accumulatedWidth = 0;
            for (int j = 0; j < click; j++)
            {
                accumulatedWidth += totalWidth[j];
            }
            Rect _rect = new Rect(accumulatedWidth, buttonRect.y + 25, 0, 0);
            menu.DropDown(_rect);
        }
        void DoPrefixLabel(GUIContent label, GUIStyle style)
        {
            var rect = GUILayoutUtility.GetRect(label, style, GUILayout.ExpandWidth(false));
            rect.height = Math.Max(20, rect.height);
            GUI.Label(rect, label, style);
        }
        internal void UpdateAllTabPaths()
        {
            if (tabs == null || tabs.Count == 0)
            {
                return;
            }
            foreach (var tab in tabs)
            {
                tab.UpdatePath();
            }
            if (closedTabs == null || closedTabs.Count == 0)
            {
                return;
            }
            foreach (var tab in closedTabs)
            {
                tab.UpdatePath();
            }
        }
        bool IsThereValidSession()
        {
            if (lastSessionData != null)
            {
                if (lastSessionData.tabs == null || lastSessionData.tabs.Count == 0)
                {
                    lastSessionData = null;
                    return false;
                }
                return true;
            }
            return false;
        }

        void RepopulateAllTabHistories()
        {
            if (tabs != null)
            {
                foreach (var tab in tabs)
                {
                    tab.TryRepopulateHistory();
                }
            }
            if (closedTabs != null)
            {
                foreach (var tab in closedTabs)
                {
                    tab.TryRepopulateHistory();
                }
            }
        }

        void TrySaveLastAssets()
        {
            if (settingsData != null && settingsData.sessions != null && settingsData.sessions.Any())
            {
                var mostRecentSession = settingsData.sessions
                    .OrderByDescending(session => session.GetSaveTime())
                    .FirstOrDefault();
                if (mostRecentSession != null)
                {
                    mostRecentSession.SaveAssets(this);
                }
            }
        }

        void RestoreLastAssets()
        {
            if (settingsData != null && settingsData.sessions != null && settingsData.sessions.Any())
            {

                var mostRecentSession = settingsData.sessions
                                       .OrderByDescending(session => session.GetSaveTime())
                                       .FirstOrDefault();
                if (mostRecentSession != null)
                {
                    targetObject = mostRecentSession.targetObject;
                    targetObjects = mostRecentSession.targetObjects;
                    if (targetObjects != null || targetObject != null)
                    {
                        assetsCollapsed = mostRecentSession.assetsCollapsed;
                        userHeight = mostRecentSession.userHeight;
                        alreadyCalculatedHeight = false;
                    }
                    if (targetObjects != null && targetObjects.Length > 0)
                    {
                        SetTargetAssets(targetObjects, true);
                    }
                    else if (targetObject != null)
                    {
                        SetTargetAsset(targetObject, true);
                    }
                    lockedAsset = mostRecentSession.lockedAsset;
                    maximizedAssetView = mostRecentSession.maximizedAssetView;
                    showImportSettings = mostRecentSession.showImportSettings;
                    if (mostRecentSession.historyAssets != null)
                    {
                        historyAssets = mostRecentSession.historyAssets
                                      .Where(item => item != null)
                                      .Select(item => item.Clone())
                                      .ToList();
                    }
                    /* if (targetObject && !String.IsNullOrEmpty(mostRecentSession.lastSelectedAsset))
                     {
                         if (AssetDatabase.GetAssetPath(targetObject) == mostRecentSession.lastSelectedAsset)
                         {
                             rawUserHeight = mostRecentSession.rawUserHeight;
                             userHeight = mostRecentSession.userHeight;
                             alreadyCalculatedHeight = true;
                             lastKnownHeight = mostRecentSession.lastKnownHeight;
                         }
                     }*/

                }
            }
        }
        void RestoreSession(bool light = false)
        {
            if (light)
            {
                RepopulateAllTabHistories();
            }
            if (lastSessionData != null && lastSessionData.tabs != null)
            {
                tabs = EditorUtils.CloneTabList(lastSessionData.tabs);
                if (tabs.Count == 0)
                {
                    tabs.Add(new TabInfo(null, 0, this));
                }
                scrollPosition = lastSessionData.scrollPosition;
                toolbarScrollPosition = lastSessionData.toolbarScrollPosition;
                lastValidTabWidth = lastSessionData.lastValidTabWidth;
                lastClickedTab = lastSessionData.lastClickedTab;
                closedTabs = EditorUtils.CloneTabList(lastSessionData.closedTabs);
                tracker = new GameObjectTracker(lastSessionData.tracker);
                activeIndex = lastSessionData.activeIndex;
                alreadyCalculatedHeight = false;
                if (activeIndex > tabs.Count - 1)
                {
                    activeIndex = tabs.Count - 1;
                }
                if (activeIndex < 0)
                {
                    activeIndex = 0;
                }
                if (GetActiveTab() != null && GetActiveTab().IsValidMultiTarget())
                {
                    SetTargetGameObjects(GetActiveTab().targets);
                }
                else if (GetActiveTab() != null && GetActiveTab().target != null)
                {
                    SetTargetGameObject(GetActiveTab().target);
                }
                userHeight = 0;
                lastKnownHeight = 0;
                rawUserHeight = 0;
                previewRect = new Rect(0, 0, 0, 0);
                lastSessionData = null;
            }
            UpdateAllWidths();
            activeScene = SceneInfo.FromActiveScene();
            ReinitializeComponentEditors();
            FocusTab();
        }
        /*
        internal TabInfo GetActiveTab()
        {
            if (tabs != null && tabs.Count > 0 && activeIndex < tabs.Count && activeIndex >= 0)
            {
                return tabs[activeIndex];
            }
            return null;
        }*/
        internal TabInfo GetActiveTab()
        {
            if (activeIndex != lastActiveIndex)
            {
                if (tabs != null && tabs.Count > 0 && activeIndex < tabs.Count && activeIndex >= 0)
                {
                    lastActiveIndex = activeIndex;
                    return tabs[activeIndex];
                }
                else
                {
                    lastActiveIndex = -1;
                    return null;
                }
            }
            return lastActiveIndex >= 0 ? tabs[lastActiveIndex] : null;
        }
        bool IsActiveTabNew()
        {
            if (tabs != null && tabs.Count > 0 && activeIndex < tabs.Count)
            {
                return GetActiveTab().newTab;
            }
            return false;
        }
        bool IsActiveTabValidMulti()
        {
            if (tabs != null && tabs.Count > 0 && activeIndex < tabs.Count)
            {
                if (GetActiveTab().multiEditMode && GetActiveTab().targets != null && GetActiveTab().targets.Length > 0)
                {
                    return true;
                }
            }
            return false;
        }
        bool IsActiveTabValid()
        {
            if (tabs != null && tabs.Count > 0 && activeIndex < tabs.Count)
            {
                TabInfo tab = GetActiveTab();
                if (tab.newTab)
                {
                    return true;
                }
                if (tab != null && ((!tab.multiEditMode && tab.target != null) || (tab.multiEditMode && tab.targets != null && tab.targets.Length > 0)))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsActiveTabLocked()
        {
            if (tabs != null && tabs.Count > 0 && activeIndex < tabs.Count)
            {
                return GetActiveTab().locked;
            }
            return false;
        }

        private void OnDestroy()
        {
            if (!settingsData)
            {
                settingsData = AutoCreateSettings();
            }
            UpdateAllTabPaths();
            CleanAllEditors();
            justOpened = false;
            instances.Count();
            instances.Remove(this);
        }
        internal static bool IsNamespacePresent(string namespaceName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                if (types.Any(type => type.Namespace != null && type.Namespace.Contains(namespaceName)))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsOdinInspectorPresent()
        {
            return IsNamespacePresent("Sirenix.OdinInspector");
        }
        internal static bool IsAnyNamespacePresent(List<string> namespaceNames)
        {
            foreach (var name in namespaceNames)
            {
                if (IsNamespacePresent(name))
                {
                    return true;
                }
            }
            return false;
        }
    }

}
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.EditorTools;

namespace CoInspector

{
    internal static class Reflected

    {
#if UNITY_2021_2_OR_NEWER
#else
        private static Type prefabStageUtilityType;
        private static MethodInfo openPrefabMethod;
        private static MethodInfo openPrefabWithInstanceMethod;
#endif
        private static Type addComponentWindowType;
        private static Type inspectorWindowType;
        private static Type avatarPreviewType;

         private static FieldInfo avatarPreviewField;
        private static FieldInfo timeControlField;
        private static PropertyInfo playingProperty;
        private static Type propertyWindowType;
        private static Type projectWindowType;
        private static Type prefabImporterType;
        private static Type showLabelType;
        private static Type showAssetBundleNameType;
        private static Type containerWindowType;
        private static Type assetImporterEditorType;
        private static Type timeControlType;
        private static MethodInfo setAssetImporterMethod;
        private static MethodInfo openPropertyEditorMethod;
        private static MethodInfo openAddComponentWindowMethod;
        private static MethodInfo showLabelGUI;
        private static MethodInfo saveChangesMethod;
        private static MethodInfo discardChangesMethod;
        private static MethodInfo showAssetBundleNameMethod;
        private static MethodInfo isMainWindowMethod;
        private static PropertyInfo inspectorModeProperty;
        private static PropertyInfo isInspectorLockedProperty;
        private static PropertyInfo windowsProperty;
        private static PropertyInfo positionProperty;
        private static FieldInfo hideInspectorField;
        private static FieldInfo windowsField;
        internal static object labelGUIInstance;
        internal static object assetBundleNameGUIInstance;
       internal static object avatarPreview;
       internal static object timeControl;
       internal static bool timeControlGathered = false;
       //BECAUSE IT TAKES A COUPLE OF FRAMES FOR EDITORS TO ASSIGN THE TIMECONTROL AND AVATARPREVIEW
       private static int gatheringAttempts = 0;
        private const int MAX_GATHERING_ATTEMPTS = 10;


        private static Type ContainerWindowType
        {
            get
            {
                if (containerWindowType == null)
                {
                    containerWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ContainerWindow");
                    if (containerWindowType == null)
                        throw new Exception("ContainerWindow type not found");
                }
                return containerWindowType;
            }
        }

        private static PropertyInfo WindowsProperty
        {
            get
            {
                if (windowsProperty == null)
                {
                    windowsProperty = ContainerWindowType.GetProperty("windows", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                }
                return windowsProperty;
            }
        }

        private static FieldInfo WindowsField
        {
            get
            {
                if (windowsField == null)
                {
                    windowsField = ContainerWindowType.GetField("windows", BindingFlags.Static | BindingFlags.NonPublic);
                }
                return windowsField;
            }
        }

        private static MethodInfo IsMainWindowMethod
        {
            get
            {
                if (isMainWindowMethod == null)
                {
                    isMainWindowMethod = ContainerWindowType.GetMethod("IsMainWindow", BindingFlags.Instance | BindingFlags.Public);
                    if (isMainWindowMethod == null)
                        throw new Exception("IsMainWindow method not found");
                }
                return isMainWindowMethod;
            }
        }
        internal static void SetLockState(EditorWindow window, bool locked)
        {
            if (window)
            {
                var isLockedProp = GetIsInspectorLockedPropertyInfo();
                isLockedProp?.GetSetMethod().Invoke(window, new object[] { locked });
            }
        }

        private static PropertyInfo PositionProperty
        {
            get
            {
                if (positionProperty == null)
                {
                    positionProperty = ContainerWindowType.GetProperty("position", BindingFlags.Instance | BindingFlags.Public);
                    if (positionProperty == null)
                        throw new Exception("position property not found");
                }
                return positionProperty;
            }
        }

        internal static Rect GetMainWindowPosition()
        {
            try
            {
                IEnumerable<object> windows;
                if (WindowsProperty != null)
                {
                    windows = WindowsProperty.GetValue(null) as IEnumerable<object>;
                }
                else if (WindowsField != null)
                {
                    windows = WindowsField.GetValue(null) as IEnumerable<object>;
                }
                else
                {
                    throw new Exception("Cannot access windows collection");
                }

                if (windows == null)
                    throw new Exception("Failed to get ContainerWindow instances");

                foreach (var window in windows)
                {
                    bool isMainWindow = (bool)IsMainWindowMethod.Invoke(window, null);
                    if (isMainWindow)
                    {
                        return (Rect)PositionProperty.GetValue(window);
                    }
                }

                throw new Exception("Main window not found");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error in GetMainWindowPosition: {e.Message}");
                return new Rect(0f, 0f, 1000f, 600f);
            }
        }

#if UNITY_2021_2_OR_NEWER
#else
        internal static Type GetPrefabStageUtilityType()
        {
            if (prefabStageUtilityType != null)
            {
                return prefabStageUtilityType;
            }
            prefabStageUtilityType = typeof(EditorWindow).Assembly.GetType("UnityEditor.Experimental.SceneManagement.PrefabStageUtility");
            return prefabStageUtilityType;
        }
        internal static MethodInfo GetOpenPrefabMethod()
        {
            if (openPrefabMethod != null)
            {
                return openPrefabMethod;
            }
            Type prefabStageUtilityType = GetPrefabStageUtilityType();
            if (prefabStageUtilityType != null)
            {
                openPrefabMethod = prefabStageUtilityType.GetMethod("OpenPrefab", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(string) }, null);
                if (openPrefabMethod == null)
                {
                    Debug.LogError("OpenPrefab method not found.");
                }
                return openPrefabMethod;
            }
            else
            {
                return null;
            }
        }

        internal static MethodInfo GetOpenPrefabWithInstanceMethod()
        {
            if (openPrefabWithInstanceMethod != null)
            {
                return openPrefabWithInstanceMethod;
            }
            Type prefabStageUtilityType = GetPrefabStageUtilityType();
            if (prefabStageUtilityType != null)
            {
                openPrefabWithInstanceMethod = prefabStageUtilityType.GetMethod("OpenPrefab", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(string), typeof(GameObject) }, null);
                if (openPrefabWithInstanceMethod == null)
                {
                    Debug.LogError("OpenPrefab with instance method not found.");
                }
                return openPrefabWithInstanceMethod;
            }
            else
            {
                Debug.LogError("PrefabStageUtility type not found.");
                return null;
            }
        }
        public static object OpenPrefab(string prefabAssetPath)
        {
            MethodInfo method = GetOpenPrefabMethod();
            if (method != null)
            {
                return method.Invoke(null, new object[] { prefabAssetPath });
            }
            else
            {
                Debug.LogError("Failed to invoke OpenPrefab.");
                return null;
            }
        }

        public static object OpenPrefab(string prefabAssetPath, GameObject openedFromInstance)
        {
            MethodInfo method = GetOpenPrefabWithInstanceMethod();
            if (method != null)
            {
                return method.Invoke(null, new object[] { prefabAssetPath, openedFromInstance });
            }
            else
            {
                Debug.LogError("Failed to invoke OpenPrefab with instance.");
                return null;
            }
        }

#endif
        internal static Type GetAssetBundleNameType()
        {
            if (showAssetBundleNameType != null)
            {
                return showAssetBundleNameType;
            }
            else
            {
                showAssetBundleNameType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AssetBundleNameGUI");
                return showAssetBundleNameType;
            }
        }

        internal static Type GetProjectWindowType()
        {
            if (projectWindowType != null)
            {
                return projectWindowType;
            }
            else
            {
                projectWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
                if (projectWindowType == null)
                {
                    //      Debug.LogError("ProjectWindow type not found.");
                }

                return projectWindowType;
            }
        }


        internal static Type GetAddComponentWindowType()
        {
            if (addComponentWindowType != null)
            {
                return addComponentWindowType;
            }
            else
            {
                addComponentWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.AddComponent.AddComponentWindow");
                return addComponentWindowType;
            }
        }

        internal static PropertyInfo GetIsInspectorLockedPropertyInfo()
        {
            if (isInspectorLockedProperty != null)
            {
                return isInspectorLockedProperty;
            }
            if (GetInspectorWindowType() != null)
            {

                isInspectorLockedProperty = GetInspectorWindowType().GetProperty("isLocked", BindingFlags.Instance | BindingFlags.Public);
                if (isInspectorLockedProperty != null)
                {
                    return isInspectorLockedProperty;
                }
                else
                {
                    //   Debug.LogError("isLocked property not found.");
                    return null;
                }
            }
            else
            {
                //   Debug.LogError("InspectorWindow type not found.");
                return null;
            }
        }
        internal static MethodInfo GetAddComponentWindowShowMethod()
        {
            if (openAddComponentWindowMethod != null)
            {
                return openAddComponentWindowMethod;
            }
            Type windowType = GetAddComponentWindowType();
            if (windowType != null)
            {

                openAddComponentWindowMethod = windowType.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(Rect), typeof(GameObject[]) }, null);
                return openAddComponentWindowMethod;
            }
            else
            {
                //    Debug.LogError("AddComponentWindow type not found.");
                return null;
            }
        }

        internal static Type GetInspectorWindowType()
        {
            if (inspectorWindowType == null)
            {
                inspectorWindowType = Assembly.Load("UnityEditor").GetType("UnityEditor.InspectorWindow");
            }
            return inspectorWindowType;
        }

#if UNITY_2020_2_OR_NEWER
    internal static void UpdateCurrentApplyRevertMethod(UnityEditor.AssetImporters.AssetImporterEditor importEditor, Editor editor)
#else
        internal static void UpdateCurrentApplyRevertMethod(UnityEditor.Experimental.AssetImporters.AssetImporterEditor importEditor, Editor editor)
#endif
        {
            if (editor == null || importEditor == null)
            {
                return;
            }
            if (assetImporterEditorType == null)
            {
                CacheMethods();
            }
            if (assetImporterEditorType == null)
            {
                assetImporterEditorType = importEditor.GetType();
            }
            if (setAssetImporterMethod == null)
            {
                setAssetImporterMethod = assetImporterEditorType.GetMethod("InternalSetAssetImporterTargetEditor", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            if (setAssetImporterMethod != null)
            {
                setAssetImporterMethod.Invoke(importEditor, new object[] { editor });
            }
            
        }

        internal static MethodInfo GetShowAssetBundleNameMethod()
        {
            Type labelType = GetAssetBundleNameType();
            if (labelType != null)
            {
                if (showAssetBundleNameMethod != null)
                {
                    return showAssetBundleNameMethod;
                }
                showAssetBundleNameMethod = labelType.GetMethod("OnAssetBundleNameGUI", BindingFlags.Public | BindingFlags.Instance);
                return showAssetBundleNameMethod;
            }
            else
            {
                //     Debug.LogError("AssetBundleNameGUI type not found.");
                return null;
            }
        }

    public static void GatherTimeControl(Editor editor)
    {
        timeControl = null;
        avatarPreview = null;
        avatarPreviewField = null;
        timeControlGathered = false;
        ResetTimeControlGathering();
        if (editor == null)
        {
            return;
        }
        Type editorType = editor.GetType();
        if (avatarPreviewType == null)
        {
            avatarPreviewType = typeof(Editor).Assembly.GetType("UnityEditor.AvatarPreview");
            if (avatarPreviewType == null)
            {
                return;
            }
        }        
        avatarPreviewField = editorType.GetField("m_AvatarPreview", BindingFlags.NonPublic | BindingFlags.Instance);
        if (avatarPreviewField == null)
        {
            return;
        }        
    }

    internal static bool FinishGatheringTimeControl(Editor editor)
    {
        if (editor == null || avatarPreviewField == null)
        {
            return false;
        }        
        avatarPreview = avatarPreviewField.GetValue(editor);
        if (avatarPreview == null)
        {
            return false;
        }

        if (timeControlField == null)
        {
            timeControlField = avatarPreviewType.GetField("timeControl", BindingFlags.Public | BindingFlags.Instance);
            if (timeControlField == null)
            {
                return false;
            }
        }
        timeControl = timeControlField.GetValue(avatarPreview);
        if (timeControl == null)
        {
            return false;
        }
        if (playingProperty == null)
        {
            if (timeControlType == null)
            {
                timeControlType = timeControl.GetType();
                if (timeControlType == null)
                {
                    Debug.LogWarning("Failed to find TimeControl type.");
                    return false;
                }
            }
            playingProperty = timeControlType.GetProperty("playing", BindingFlags.Public | BindingFlags.Instance);
                if (playingProperty == null)
                {
                    Debug.Log("Failed to find playing property in TimeControl.");
                    return false;
                }
            }
            timeControlGathered = true;
            return true;
    }

    public static bool IsTimeControlPlaying(Editor editor)
    {
        if (editor == null || avatarPreviewField == null)
        {
            return false;
        }
        if (!timeControlGathered && avatarPreview == null && gatheringAttempts < MAX_GATHERING_ATTEMPTS)
        {
            gatheringAttempts++;
            if (!FinishGatheringTimeControl(editor))
            {
                return false;
            }
            else
            {
                gatheringAttempts = 0;
            }
        }      
        if (!timeControlGathered || editor == null || timeControl == null || playingProperty == null)
        {
            return false;
        }    
        return (bool)playingProperty.GetValue(timeControl, null);
    }
    public static void ResetTimeControlGathering()
    {
        timeControlGathered = false;
        avatarPreview = null;
        timeControl = null;
        gatheringAttempts = 0;
    }

    private static void CacheMethods()
        {
#if UNITY_2020_2_OR_NEWER
        assetImporterEditorType = typeof(UnityEditor.AssetImporters.AssetImporterEditor);
#else
            assetImporterEditorType = typeof(UnityEditor.Experimental.AssetImporters.AssetImporterEditor);
#endif

#if UNITY_2022_3_OR_NEWER
        saveChangesMethod = assetImporterEditorType.GetMethod("SaveChanges", BindingFlags.Public | BindingFlags.Instance);
        discardChangesMethod = assetImporterEditorType.GetMethod("DiscardChanges", BindingFlags.Public | BindingFlags.Instance);
#else
            saveChangesMethod = assetImporterEditorType.GetMethod("ApplyAndImport", BindingFlags.NonPublic | BindingFlags.Instance);
            discardChangesMethod = assetImporterEditorType.GetMethod("ResetValues", BindingFlags.NonPublic | BindingFlags.Instance);
#endif
        }

        internal static void ApplyChanges(Editor editor)
        {
            if (saveChangesMethod == null || assetImporterEditorType == null)
            {
                CacheMethods();
            }

            if (editor == null || !assetImporterEditorType.IsInstanceOfType(editor))
            {
                return;
            }

            saveChangesMethod?.Invoke(editor, null);
        }

        internal static void DiscardChanges(Editor editor)
        {
            if (discardChangesMethod == null || assetImporterEditorType == null)
            {
                CacheMethods();
            }

            if (editor == null || !assetImporterEditorType.IsInstanceOfType(editor))
            {
                return;
            }

            discardChangesMethod?.Invoke(editor, null);
        }

        internal static void ShowAddComponentWindow(Rect rect, GameObject[] gameObjects)
        {
            MethodInfo _showMethod = GetAddComponentWindowShowMethod();
            if (_showMethod != null)
            {
                _showMethod.Invoke(null, new object[] { rect, gameObjects });
            }
            else
            {
                //    Debug.LogError("AddComponentWindow.Show method not found.");
            }
        }

        internal static bool ComponentHasEditorTool(Component component)
        {
            return EditorToolCache.IsToolAvailableForComponent(component);
        }


        internal static Type GetPrefabImporterType()
        {
            if (prefabImporterType != null)
            {
                return prefabImporterType;
            }
            else
            {
                prefabImporterType = typeof(EditorWindow).Assembly.GetType("UnityEditor.PrefabImporterEditor");
                return prefabImporterType;
            }
        }

        private static Type editorToolManagerType = null;
        private static MethodInfo getActiveToolMethod = null;

        internal static Type GetEditorToolManagerType()
        {
            if (editorToolManagerType != null)
            {
                return editorToolManagerType;
            }
            else
            {
                editorToolManagerType = typeof(EditorWindow).Assembly.GetType("UnityEditor.EditorTools.EditorToolManager");
                return editorToolManagerType;
            }
        }

        public static MethodInfo onSceneGUIMethod = null;

        public static void _OnSceneGUI(Editor editor)
        {
            onSceneGUIMethod = editor.GetType().GetMethod("OnSceneGUI", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (onSceneGUIMethod != null)
            {
                //   Debug.Log("OnSceneGUI method was found.");
                onSceneGUIMethod.Invoke(editor, null);
            }

        }

        internal static MethodInfo GetActiveToolMethod()
        {
            if (getActiveToolMethod != null)
            {
                return getActiveToolMethod;
            }
            else
            {
                Type toolManagerType = GetEditorToolManagerType();
                if (toolManagerType != null)
                {
                    getActiveToolMethod = toolManagerType.GetMethod("GetActiveTool", BindingFlags.Public | BindingFlags.Static);
                    if (getActiveToolMethod == null)
                    {
                        //    Debug.LogError("GetActiveTool method not found.");
                    }
                    return getActiveToolMethod;
                }
                else
                {
                    //    Debug.LogError("EditorToolManager type not found.");
                    return null;
                }
            }
        }

        public static EditorTool CallGetActiveTool()
        {
            MethodInfo method = GetActiveToolMethod();
            if (method != null)
            {
                return (EditorTool)method.Invoke(null, null);
            }
            else
            {
                //    Debug.LogError("Failed to invoke GetActiveTool.");
                return null;
            }
        }

        internal static Type GetPropertyEditorWindowType()
        {
            if (propertyWindowType != null)
            {
                return propertyWindowType;
            }
            else
            {
                propertyWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.PropertyEditor");
                return propertyWindowType;
            }
        }

        internal static Type GetAssetLabelsType()
        {
            if (showLabelType != null)
            {
                return showLabelType;
            }
            else
            {
                showLabelType = typeof(EditorWindow).Assembly.GetType("UnityEditor.LabelGUI");
                return showLabelType;
            }
        }

        internal static MethodInfo GetShowAssetLabelMethod()
        {
            Type labelType = GetAssetLabelsType();
            if (labelType != null)
            {
                if (showLabelGUI != null)
                {
                    return showLabelGUI;
                }
                showLabelGUI = GetAssetLabelsType().GetMethod("OnLabelGUI", BindingFlags.Public | BindingFlags.Instance);
                return showLabelGUI;
            }
            else
            {
                //     Debug.LogError("LabelGUI type not found.");
                return null;
            }
        }

        internal static MethodInfo GetPropertyEditorWindowShowMethod()
        {
            Type windowType = GetPropertyEditorWindowType();
            if (windowType != null)
            {
                if (openPropertyEditorMethod != null)
                {
                    return openPropertyEditorMethod;
                }
                openPropertyEditorMethod = windowType.GetMethod("Show", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(UnityEngine.Object) }, null);
                return openPropertyEditorMethod;

            }
            else
            {
                //    Debug.LogError("PropertyEditorWindow type not found.");
                return null;
            }
        }
        internal static void ShowPropertyEditorWindow(UnityEngine.Object obj, bool showWindow = true)
        {

            MethodInfo showMethod = GetPropertyEditorWindowShowMethod();

            if (showMethod != null)
            {
                showMethod.Invoke(null, new object[] { obj, showWindow });
            }
            else
            {
                //     Debug.LogError("PropertyEditorWindow.Show method not found.");
            }
        }
        internal static void SetHideInspector(Editor editor, bool hideInspector)
        {
            if (GetHideInspectorFieldInfo() != null)
            {
                GetHideInspectorFieldInfo().SetValue(editor, hideInspector);
            }
            else
            {
                //     Debug.LogError("Could not find the 'hideInspector' field.");
            }
        }

        internal static FieldInfo GetHideInspectorFieldInfo()
        {
            if (hideInspectorField != null)
            {
                return hideInspectorField;
            }
            hideInspectorField = typeof(Editor).GetField("hideInspector", BindingFlags.NonPublic | BindingFlags.Instance);
            return hideInspectorField;
        }

        internal static PropertyInfo GetInspectorModePropInfo()
        {
            if (inspectorModeProperty != null)
            {
                return inspectorModeProperty;
            }
            inspectorModeProperty = typeof(Editor).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
            return inspectorModeProperty;
        }

        internal static void SetInspectorMode(Editor editor, InspectorMode mode)
        {

            if (GetInspectorModePropInfo() != null)
            {
                GetInspectorModePropInfo().SetValue(editor, mode, null);
            }
            else
            {
                //    Debug.LogError("Could not find the 'inspectorMode' property.");
            }
        }

    }
}
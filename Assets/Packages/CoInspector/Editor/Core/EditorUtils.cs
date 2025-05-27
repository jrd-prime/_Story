using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.Reflection;
using UnityObject = UnityEngine.Object;
using System;
using System.Text;
using System.Linq;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Audio;


namespace CoInspector
{   

    internal static class EditorUtils
    {
        internal static bool IsLightSkin()
        {
            return !EditorGUIUtility.isProSkin;
        }
        internal static bool NotDragging()
        {
            return DragAndDrop.objectReferences == null || DragAndDrop.objectReferences.Length == 0;
        }

internal static bool HasVisibleFields(Editor editor)
{
    if (editor == null) return false;

    SerializedProperty property = editor.serializedObject.GetIterator();

    if (property.NextVisible(true))
    {
        // If it's a custom script, we need to skip the script field
        if (property.name == "m_Script" && property.type == "PPtr<MonoScript>")
        {
            return property.NextVisible(false);
        }
        else
        {
            return true;
        }
    }
    return false;
}
      /*
        internal static string DebugTabInfos(List<TabInfo> tabInfos)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tabInfos.Count; i++)
            {
                sb.AppendLine($"Tab {i + 1}:");
                sb.AppendLine(DebugSingleTabInfo(tabInfos[i]));
                sb.AppendLine();
            }
            return sb.ToString();
        } */

        internal static string DebugSingleTabInfo(TabInfo tabInfo)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"  Name: {tabInfo.name}");
            sb.AppendLine($"  Total ComponentMaps: {tabInfo.componentMaps.Count}");
            sb.AppendLine($"  Foldouts open: {tabInfo.componentMaps.Count(cm => cm.foldout)}");

            foreach (var componentMap in tabInfo.componentMaps)
            {
                string componentName = componentMap.component != null ? componentMap.component.GetType().Name : "Null";
                sb.AppendLine($"    {componentName} ({(componentMap.foldout ? "open" : "closed")})");
            }

            return sb.ToString();
        }
        internal static bool CheckEditorsChanged(Editor[] editors)
        {
            if (editors == null)
            {
                return false;
            }
            foreach (var editor in editors)
            {
                if (editor && CheckEditorChanged(editor))
                {
                    return true;
                }
            }
            return false;
        }
        internal static bool CheckEditorChanged(Editor editor)
        {
            if (editor == null)
            {
                return false;
            }
            if (editor.target == null || editor.serializedObject == null)
            {
                UnityEngine.Object.DestroyImmediate(editor, true);
                return false;
            }
            if (editor.serializedObject != null)
            {
                if (editor.serializedObject.UpdateIfRequiredOrScript())
                {
                    editor.serializedObject.ApplyModifiedProperties();
                    if (CoInspectorWindow.MainCoInspector)
                    {
                        CoInspectorWindow.MainCoInspector.Repaint();
                    }
                    return true;
                }
            }
            return false;
        }
        internal static bool AreRectsOverlapping(Rect rect1, Rect rect2)
        {
            if (rect1 == Rect.zero || rect2 == Rect.zero)
            {
                return false;
            }
            if (rect1.x < rect2.x + rect2.width && rect1.x + rect1.width > rect2.x)
            {
                return true;
            }
            return false;
        }
        internal static bool IsCtrlHeld()
        {
            if (Event.current == null)
            {
                return false;
            }
            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                if (Event.current.command)
                {
                    return true;
                }
            }
            else
            {
                if (Event.current.control)
                {
                    return true;
                }
            }

            return false;
        }

       internal static bool HaveBoolArraysChanged(bool[] array1, bool[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return true;
            }
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool CompareArrays(GameObject[] a, GameObject[] b)
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
        internal static bool ContainsArray(List<GameObject[]> list, GameObject[] array)
        {
            if (list == null || array == null)
            {
                return false;
            }

            foreach (GameObject[] _array in list)
            {
                if (CompareArrays(_array, array))
                {
                    return true;
                }
            }
            return false;
        }

        internal static GameObject LoadGameObject(string path, bool prefabMode = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            Transform root = null;
            if (prefabMode)
            {
                root = CoInspectorWindow.GetPrefabStageRoot()?.transform;
                if (root == null)
                {
                    return null;
                }
            }
            string[] pathComponents = path.Split('/');
            Transform current = root;
            for (int i = 0; i < pathComponents.Length; i++)
            {
                string component = pathComponents[i];
                if (string.IsNullOrEmpty(component))
                {
                    continue;
                }
                string name = component;
                int index = 0;

                int indexStart = component.LastIndexOf('[');
                if (indexStart >= 0)
                {
                    int indexEnd = component.LastIndexOf(']');
                    if (indexEnd > indexStart)
                    {
                        string indexStr = component.Substring(indexStart + 1, indexEnd - indexStart - 1);
                        if (int.TryParse(indexStr, out int parsedIndex))
                        {
                            name = component.Substring(0, indexStart);
                            index = parsedIndex;
                        }
                    }
                }
                if (prefabMode)
                {
                    if (i == 0)
                    {
                        if (name != root.name)
                        {
                            return null;
                        }
                        if (pathComponents.Length == 1)
                        {
                            return root.gameObject;
                        }
                    }
                    else
                    {
                        current = FindChildByIndex(current, name, index);
                    }
                }
                else
                {
                    if (current == null)
                    {
                        current = GameObject.Find(name)?.transform;
                    }
                    else
                    {
                        current = FindChildByIndex(current, name, index);
                    }
                }
                if (current == null)
                {
                    return null;
                }
            }
            return current?.gameObject;
        }
        internal static Transform FindChildByIndex(Transform parent, string name, int index)
        {
            int currentIndex = 0;
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    if (currentIndex == index)
                    {
                        return child;
                    }
                    currentIndex++;
                }
            }
            return null;
        }

        internal static bool IsAPrefabAsset(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
            if (AssetDatabase.GetAssetPath(gameObject) == "")
            {
                return false;
            }
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(gameObject);
            return prefabAssetType != PrefabAssetType.NotAPrefab;
        }

        internal static bool IsAnyAPrefabAsset(GameObject[] gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                if (IsAPrefabAsset(gameObject))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool _AreAllPrefabs(GameObject[] gameObjects)
        {
            if (gameObjects == null)
            {
                return false;
            }
            foreach (var gameObject in gameObjects)
            {
                if (!PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
                {
                    return false;
                }
            }
            return true;
        }

        internal static List<string[]> HistoryPathsToList(List<HistoryPaths> history)
        {
            if (history == null)
            {
                return null;
            }
            List<string[]> historyList = new List<string[]>();
            foreach (var paths in history)
            {
                historyList.Add(paths.paths);
            }
            return historyList;
        }

        internal static string GatherGameObjectPath(GameObject obj)
        {
            if (obj == null)
            {
                return "";
            }

            List<string> pathComponents = new List<string>();
            Transform current = obj.transform;

            while (current != null)
            {
                string name = current.name;
                Transform parent = current.parent;

                if (parent != null)
                {
                    int siblingIndex = GetSiblingIndex(parent, current);
                    name += $"[{siblingIndex}]";
                }

                pathComponents.Insert(0, name);
                current = parent;
            }

            return string.Join("/", pathComponents);
        }

        internal static int GetSiblingIndex(Transform parent, Transform child)
        {
            int index = 0;
            foreach (Transform sibling in parent)
            {
                if (sibling == child)
                {
                    return index;
                }
                if (sibling.name == child.name)
                {
                    index++;
                }
            }
            return index;
        }

        internal static GameObject LoadSingleTabGameObject(string path, int id, bool prefab)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }
            GameObject go = null;
            if (id != 0)
            {
                go = EditorUtility.InstanceIDToObject(id) as GameObject;
                if (go != null && path.Contains(go.name))
                {
                  return go;
                }
            }
            go = LoadGameObject(path, prefab);              
            return go;
        }

        internal static GameObject LoadTabGameObject(TabInfo tab)
        {
            if (tab == null || string.IsNullOrEmpty(tab.path))
            {
                return null;
            }            
            return LoadSingleTabGameObject(tab.path, tab.id, tab.prefab);
        }

        internal static GameObject[] LoadTabGameObjects(TabInfo tab)
        {
            if (tab == null)
            {
                return null;
            }
            if (tab.paths != null || (tab.paths != null && tab.ids != null && tab.paths.Length == tab.ids.Length))
            {
                List<GameObject> gameObjects = new List<GameObject>();
                if (tab.ids == null)
                {
                    tab.ids = new int[tab.paths.Length];
                }
                for (int i = 0; i < tab.paths.Length; i++)
                {
                    GameObject go = LoadSingleTabGameObject(tab.paths[i], tab.ids[i], tab.prefab);
                    if (go != null)
                    {
                        gameObjects.Add(go);
                    }
                }
                return gameObjects.Count > 0 ? gameObjects.ToArray() : null;
            }
            return null;
        }

        internal static void TabPathsToGameObjects(TabInfo tab)
        {
            if (tab == null)
            {
                return;
            }            
            if (!string.IsNullOrEmpty(tab.path))
            {
                tab.target = LoadTabGameObject(tab);
            }           
            if (tab.target == null && tab.paths != null && tab.paths.Length > 0)
            {               
                tab.targets = LoadTabGameObjects(tab);               
            }
            if (tab.historyPaths != null)
            {
                tab.history = GetHistory(tab);
            }
        }
        internal static void ShowPasteFailedMessage()
        {
            EditorUtility.DisplayDialog("Operation Failed", "This Type cannot be added twice to the same GameObject!", "Ah, OK");
        }
        internal static void ShowPrefabFailedMessage()
        {
            EditorUtility.DisplayDialog("Cannot restructure Prefab instance", "Children of a Prefab instance cannot be deleted or moved, and components cannot be reordered.\n\nYou can open the Prefab in Prefab Mode to restructure the Prefab Asset itself, or unpack the Prefab instance to remove its Prefab connection. ", "Ah, OK");
        }
        internal static void ShowPasteFailedMessageError()
        {
            EditorUtility.DisplayDialog("Operation Failed", "This component cannot be added to the target GameObject.", "OK");
        }
        internal static void MultiEditFooter(bool differentComponents)
        {
            if (!differentComponents)
            {
                 EditorGUILayout.BeginHorizontal();
                 EditorGUILayout.EndHorizontal();
                DrawUnderLastComponent();
                return;
            }
            EditorGUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();            
            GUIStyle wrapStyle = CustomGUIStyles.WrapLabelStyle;
            EditorGUILayout.LabelField("Components that are only on some of the selected objects cannot be multi-edited.", wrapStyle);            
            EditorGUILayout.EndHorizontal();
            Rect rect = GetLastLineRect();
            EditorUtils.DrawLineOverRect(rect, 2);
            EditorUtils.DrawLineOverRect(rect, CustomColors.MediumShadow, 3);
            EditorUtils.DrawLineUnderRect(rect, CustomColors.HardShadow, -1);
        }

        internal static bool IsValidAssetType(UnityObject asset, GameObject gameObject, out bool repeatingAsset)
        {
            repeatingAsset = false;
            bool isUIObject = IsAnUIObject(gameObject);
            bool hasGraphic = GameObjectHasGraphic(gameObject);
            bool hasRenderer = GameObjectHasRenderer(gameObject);
            bool hasMeshRenderer = GameObjectHasMeshRenderer(gameObject);
            if (asset is Material)
            {
                if (!hasMeshRenderer)
                {
                    return true;
                }
                repeatingAsset = true;
                return false;
            }
            if (asset is MonoScript || asset is AudioClip || asset is AnimationClip || asset is VideoClip || asset is AudioMixerGroup || asset is UnityEditor.Animations.AnimatorController)
            {
                return true;
            }
            if (asset is Texture2D)
            {
                repeatingAsset = isUIObject && hasGraphic;
                return isUIObject && !hasGraphic;
            }
            if (asset is Font)
            {
                repeatingAsset = isUIObject && hasGraphic;
                return isUIObject && !hasGraphic;
            }
            if (asset is Sprite)
            {
                repeatingAsset = hasRenderer;
                if (isUIObject)
                {
                    repeatingAsset = hasGraphic;
                }
                return isUIObject ? !hasGraphic : !hasRenderer;
            }
            return false;
        }

        internal static bool IsAnUIObject(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
            return gameObject.GetComponent<RectTransform>() != null;
        }
        internal static bool GameObjectHasMeshRenderer(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
            return gameObject.GetComponent<MeshRenderer>() != null;
        }
        internal static bool GameObjectHasRenderer(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
            Renderer[] renderers = gameObject.GetComponents<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (!(renderer is SkinnedMeshRenderer) || !(renderer is ParticleSystemRenderer))
                {
                    return true;
                }
            }
            return false;
        }
        internal static bool GameObjectHasGraphic(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return false;
            }
            return gameObject.GetComponent<Graphic>() != null;
        }

        internal static void DrawMaterials(MaterialMap materialMap, Component component, bool isPrefab = false)
        {
            if (materialMap.materials != null && materialMap.materials.Count > 0)
            {
                List<Material> materials = materialMap.materials;
                if (materialMap != null && materialMap.editors != null && materialMap.editors.Count != 0 && materials.Count == materialMap.editors.Count)
                {
                    for (int i = 0; i < materialMap.editors.Count; i++)
                    {
                        if (materialMap.editors[i] != null)
                        {
                            EditorGUILayout.BeginVertical();
                            if (i == 0) GUILayout.Space(5);
                            float currentLabelWidth = EditorGUIUtility.labelWidth;
                            EditorGUILayout.BeginVertical(CustomGUIStyles.CollapsedCompStyle);
                            materialMap.editors[i].DrawHeader();
                            EditorGUILayout.EndVertical();
                            if (UnityEditorInternal.InternalEditorUtility.GetIsInspectorExpanded(materials[i]))
                            {
                                GUI.enabled = !EditorUtils.IsAssetBuiltIn(AssetDatabase.GetAssetPath(materials[i]));
                                EditorGUILayout.BeginVertical(CustomGUIStyles.CollapsedCompStyle);
                                materialMap.editors[i].OnInspectorGUI();
                                EditorGUILayout.EndVertical();
                            }
                            GUI.enabled = true;
                            EditorGUIUtility.labelWidth = currentLabelWidth;
                            EditorGUILayout.EndVertical();
                            if (i == 0)
                            {
                                EditorUtils.DrawLineOverRect(-5, 1);
                            }
                            else
                            {
                                EditorUtils.DrawLineOverRect(0, 1);
                            }
                        }
                    }
                    GUI.enabled = true;
                }
            }
        }
        internal static bool IsMaterialComponent(Component comp)
        {
            if (comp == null)
            {
                return false;
            }
            return comp is Renderer || comp is Graphic || comp is Mask;
        }
        internal static T CloneFrom<T>(object comp, T other) where T : class
        {
            Type type = comp.GetType();
            if (type != other.GetType()) return null;
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pInfos = type.GetProperties(flags);
            foreach (var pInfo in pInfos)
            {
                if (pInfo.CanWrite)
                {
                    try
                    {
                        pInfo.SetValue(comp, pInfo.GetValue(other, null), null);
                    }
                    catch { }
                }
            }
            FieldInfo[] fInfos = type.GetFields(flags);
            foreach (var fInfo in fInfos)
            {
                fInfo.SetValue(comp, fInfo.GetValue(other));
            }
            return comp as T;
        }

        internal static List<GameObject[]> GetHistory(TabInfo tab)
        {
            if (tab == null || tab.historyPaths == null)
            {
                return null;
            }
            List<GameObject[]> history = new List<GameObject[]>();

            for (int i = 0; i < tab.historyPaths.Count; i++)
            {
                var paths = tab.historyPaths[i];
                if (paths != null)
                {
                    if (paths.paths != null && (paths.instances == null || paths.instances.Length != paths.paths.Length))
                    {
                        paths.instances = new int[paths.paths.Length];
                    }
                    List<GameObject> gameObjects = new List<GameObject>();
                    for (int j = 0; j < paths.paths.Length; j++)
                    {                        
                       var go = LoadSingleTabGameObject(paths.paths[j], paths.instances[j], paths.prefab);
                        if (go != null)
                        {
                            gameObjects.Add(go);
                        }
                    }
                    history.Add(gameObjects.ToArray());
                }
            }
            return history;
        }

        internal static List<TabInfo> RebuildTabs(List<TabInfo> tabs, CoInspectorWindow reference)
        {
            List<TabInfo> newTabs = new List<TabInfo>();

            foreach (var tab in tabs)
            {
                if (tab == null /*|| tab.newTab*/)
                {
                    continue;
                }
                RebuildTab(tab, reference, false);
                if (tab != null && tab.IsTabValid())
                {
                    newTabs.Add(tab);
                }
            }
            reference.ReinitializeComponentEditors();
            return newTabs;
        }


        internal static void RebuildTab (TabInfo tab, CoInspectorWindow reference, bool single = true)
        {
            if (tab == null)
            {
                return;
            }
            tab.owner = reference;
            TabPathsToGameObjects(tab);
            tab.AutoSortTargets();
            if (!tab.IsTabValid())
            {
                tab.TrySetValidHistoryTarget();
            }
            
            if (!tab.newTab && ((tab.target == null) && (tab.targets == null || tab.targets.Length == 0)))
            {
                return;
            }
            if (tab.multiEditMode && tab.targets != null)
            {
                var validTargets = tab.targets.Where(target => target != null).ToArray();

                if (validTargets.Length > 1)
                {
                    tab.targets = validTargets;
                }
                else if (validTargets.Length == 1)
                {
                    tab.target = validTargets[0];
                    tab.targets = null;
                    tab.multiEditMode = false;
                }
                else
                {
                    return;
                }
            }
            else if (!tab.newTab && (tab.target == null || tab.target != null))
            {
                if (tab.target == null)
                {
                    return;
                }
            }
            if (tab != null)
            {
                tab.owner = reference;
                FixHistory(tab);
                FixComponentMaps(tab);
                if (single)
                {
                    if (tab == reference.GetActiveTab())
                    {
                        reference.ReinitializeComponentEditors();
                    }
                    else
                    {
                        reference.RefreshAllTabNames();
                        reference.RefreshAllIcons();
                        reference.UpdateAllTabPaths();
                    }
                }
            }
        }

        internal static void FixComponentMaps(TabInfo tab)
        {
            if (tab == null)
            {
                return;
            }
            if (tab.newTab || tab.componentMaps == null)
            {
                tab.componentMaps = new List<ComponentMap>();
                return;
            }
            Component[] components;
            if (tab.IsValidMultiTarget())
            {
                components = tab.targets[0].GetComponents<Component>();
            }
            else if (tab.target != null)
            {
                components = tab.target.GetComponents<Component>();
            }
            else
            {
                return;
            }
            if (components == null || components.Length == 0)
            {
                return;
            }
            if (components.Length != tab.componentMaps.Count)
            {
                if (components.Length > tab.componentMaps.Count)
                {
                    for (int i = tab.componentMaps.Count; i < components.Length; i++)
                    {
                        tab.componentMaps.Add(new ComponentMap());
                    }
                }
            }
            for (int i = 0; i < components.Length; i++)
            {
                tab.componentMaps[i].component = components[i];
            }
        }

        internal static void FixHistory(TabInfo tab)
        {
            if (tab == null || tab.history == null)
            {
                return;
            }
            tab.FixNulls();
        }

        internal static bool SceneExists(string sceneGUID)
        {
            return !string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(sceneGUID));
        }


        internal static Dictionary<Type, List<List<Component>>> OrderedComponentMap(GameObject[] gos, CoInspectorWindow window, bool prefab = false)
        {
            GameObject[] targets = gos;
            Dictionary<Type, List<List<Component>>> orderedComponentMap = new Dictionary<Type, List<List<Component>>>();
            if (targets == null || targets.Length == 0)
            {
                return orderedComponentMap;
            }

            Component[] components = targets[0].GetComponents<Component>();
            int firstObjectComponentCount = components.Length;

            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    continue;
                }
                Type type = comp.GetType();
                if (!orderedComponentMap.ContainsKey(type))
                {
                    orderedComponentMap[type] = new List<List<Component>>();
                    for (int i = 0; i < targets.Length; i++)
                    {
                        orderedComponentMap[type].Add(new List<Component>());
                    }
                }
            }
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] == null)
                {
                    continue;
                }
                Component[] currentComponents = targets[i].GetComponents<Component>();
                if (currentComponents.Length != firstObjectComponentCount)
                {
                    if (window != null)
                    {
                        if (prefab)
                        {
                            window.differentPrefabComponents = true;
                        }
                        else
                        {
                            window.differentComponents = true;
                        }
                    }
                }
                foreach (Component comp in currentComponents)
                {
                    if (comp == null)
                    {
                        continue;
                    }
                    Type type = comp.GetType();
                    if (orderedComponentMap.ContainsKey(type))
                    {
                        orderedComponentMap[type][i].Add(comp);
                    }
                }
            }
            foreach (var entry in orderedComponentMap.ToList())
            {
                int maxConsistentCount = 0;
                foreach (var list in entry.Value)
                {
                    int count = list.Count;
                    if (count > maxConsistentCount && entry.Value.All(l => l.Count >= count))
                    {
                        maxConsistentCount = count;
                    }
                }
                if (maxConsistentCount > 0)
                {
                    foreach (var list in entry.Value)
                    {
                        if (list.Count > maxConsistentCount)
                        {
                            list.RemoveRange(maxConsistentCount, list.Count - maxConsistentCount);
                        }
                    }
                }
                else
                {
                    orderedComponentMap.Remove(entry.Key);
                }
            }
            return orderedComponentMap;
        }
        internal static bool AreAllTargetsPrefabs(UnityEngine.Object[] targets)
        {
            if (targets == null)
            {
                return false;
            }
            foreach (var target in targets)
            {
                if (target)
                {
                    if (!IsAPrefabAsset(target))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        internal static Texture2D GetIconForComponent(Component component)
        {
            if (component == null)
            {
                return EditorGUIUtility.FindTexture("DefaultAsset Icon");
            }
            foreach (string typeName in prioritizedComponentTypes)
            {
                if (component.GetType().Name == typeName)
                {
                    return AssetPreview.GetMiniThumbnail(component);
                }
            }
            return EditorGUIUtility.FindTexture("DefaultAsset Icon");
        }

        internal static Texture2D GetBestFittingIconForGameObject(GameObject gameObject)
        {

            if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
            {
                if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
                {

                    UnityEngine.Object _root = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);
                    if (_root != null && PoolCache.IsAnImportedObject(_root))
                    {
                        return CustomGUIContents.ImportedIcon.image as Texture2D;
                    }
                }

                var root = PrefabUtility.GetCorrespondingObjectFromOriginalSource(gameObject);

                if (root && PoolCache.IsAnImportedObject(root))
                {
                    return CustomGUIContents.ImportedIcon.image as Texture2D;
                }
                return CustomGUIContents.PrefabIcon.image as Texture2D;
            }
            Component[] components = gameObject.GetComponents<Component>();

            if (components.Length == 1)
            {
                if (components[0] is RectTransform)
                {
                    return CustomGUIContents.EmptyRectTransformContent.image as Texture2D;
                }
                return CustomGUIContents.EmptyGameObjectContent.image as Texture2D;
            }

            foreach (Component component in components)
            {
                if (component == null) { continue; }

                Texture2D icon = GetIconForComponent(component);
                if (icon != null)
                {
                    return icon;
                }
            }
            Texture2D iconForGameObject = AssetPreview.GetMiniThumbnail(components[1]);
            if (iconForGameObject != null)
            {
                return iconForGameObject;
            }
            return AssetPreview.GetMiniThumbnail(components[0]);
        }

        internal static readonly string[] prioritizedComponentTypes =
           {
        "Animator", "Renderer", "AudioSource", "Light", "Camera",
        "ParticleSystem", "UnityEngine.Video.VideoPlayer", "UnityEngine.UI.Image", "UnityEngine.UI.Text"
    };

        internal static bool AreAllTargetsImportedObjects(UnityEngine.Object[] targets)
        {
            if (targets == null)
            {
                return false;
            }
            foreach (var target in targets)
            {
                if (target)
                {
                    if (!IsAnImportedObject(target))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        internal static bool IsAssetBuiltIn(string assetPath)
        {
            return !assetPath.StartsWith("Assets/");
        }
        internal static bool IsAnImportedObject(UnityObject _object)
        {
            if (_object == null)
            {
                return false;
            }
            string _extension = System.IO.Path.GetExtension(AssetDatabase.GetAssetPath(_object)).ToLower();
            if (_extension == "")
            {
                return false;
            }
            return _extension != ".prefab" && _object is GameObject;
        }
        internal static bool IsAPrefabAsset(UnityObject _object)
        {
            if (_object == null)
            {
                return false;
            }
            string _extension = System.IO.Path.GetExtension(AssetDatabase.GetAssetPath(_object)).ToLower();
            return _extension == ".prefab";
        }
        internal static bool AssetAlreadyTarget(UnityEngine.Object obj, CoInspectorWindow window)
        {
            if (window == null)
            {
                return false;
            }
            if (obj == null)
            {
                return false;
            }
            if (window.targetObject)
            {
                if (window.targetObject == obj)
                {
                    return true;
                }
            }
            if (window.targetObjects != null && window.targetObjects.Length > 0)
            {
                if (window.targetObjects.Contains(obj))
                {
                    return true;
                }
            }
            return false;
        }
        internal static bool AssetsAlreadyTargets(UnityEngine.Object[] assets, CoInspectorWindow window)
        {
            if (window == null)
            {
                return false;
            }
           
            if (assets == null || assets.Length == 0)
            {
                return false;
            }
            if (assets.Length == 1 && window.targetObjects != null && window.targetObjects.Length > 1)
            {
                return false;
            }
            if (assets.Length == 1 && window.targetObject != null)
            {
                return AssetAlreadyTarget(assets[0], window);
            }
            if (window.targetObjects == null || window.targetObjects.Length == 0)
            {
                return false;
            }
            if (assets.Length != window.targetObjects.Length)
            {
                return false;
            }
            for (int i = 0; i < assets.Length; i++)
            {
                if (!window.targetObjects.Contains(assets[i]))
                {
                    return false;
                }
            }
            return true;
        }
        internal static bool IsMainAsset(UnityObject unityObject)
        {
            if (unityObject == null)
            {
                return false;
            }
            string path = AssetDatabase.GetAssetPath(unityObject);
            if (unityObject == AssetDatabase.LoadMainAssetAtPath(path))
            {
                return true;
            }
            return false;
        }

        internal static List<TabInfo> CloneTabList(List<TabInfo> list)
        {
            List<TabInfo> newList = new List<TabInfo>();
            if (list == null)
            {
                return newList;
            }
            foreach (var tab in list)
            {
                newList.Add(new TabInfo(tab));
            }
            return newList;
        }

        internal static bool IsSelection(GameObject ob)
        {
            if (Selection.gameObjects != null && Selection.gameObjects.Length == 1)
            {
                if (Selection.gameObjects[0] == ob)
                {
                    return true;
                }
            }
            return false;
        }
        internal static void DrawBrokenPrefabMessage(bool multi = false)
        {
            if (!multi)
            {
                EditorGUILayout.HelpBox("    This Prefab seems to be broken, so it can't be opened!", MessageType.Warning);
            }
            else
            {
                EditorGUILayout.HelpBox("    One or more of the selected Prefabs seem to be broken, so they can't be opened!", MessageType.Warning);
            }
            GUILayout.Space(7);
        }

        internal static int ShowUnappliedImportSettings(Editor editor)
        {
            if (editor == null)
            {
                return 1;
            }
            if (editor.targets == null)
            {
                return 1;
            }
            int count = editor.targets.Length;
            if (count == 0)
            {
                return 1;
            }
            string message = "";
            string path = AssetDatabase.GetAssetPath(editor.target);
            if (count == 1)
            {
                message = "Unapplied import settings for '" + path + "'";
            }
            else
            {
                message = "Unapplied import settings for '" + count + "' files";
            }
            return EditorUtility.DisplayDialogComplex("Unapplied import settings", message, "Apply", "Cancel", "Revert");
            
        }

        internal static void SaveAsset (UnityObject asset, Editor editor)
        {
            if (asset == null)
            {
                return;
            }
           
            if (editor != null)
            {
                Reflected.ApplyChanges(editor);
            }
            EditorUtility.SetDirty(asset);
 #if UNITY_2020_3_OR_NEWER
            AssetDatabase.SaveAssetIfDirty(asset);
#else
            AssetDatabase.SaveAssets();
#endif
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
        }
        internal static void SaveAssets(UnityObject[] assets, Editor editor)
        {
            if (assets == null || assets.Length == 0)
            {
                return;
            }
            if (editor != null)
            {
                Reflected.ApplyChanges(editor);
            }
            foreach (UnityObject asset in assets)
            {
                if (asset == null)
                {
                    continue;
                }
               EditorUtility.SetDirty(asset);               
            }
        
            AssetDatabase.SaveAssets();
            foreach (UnityObject asset in assets)
            {
                if (asset == null)
                {
                    continue;
                }
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(asset));
            }          
        }
        internal static Rect GetLastLineRect()
        {
            return GetLastLineRect(GUILayoutUtility.GetLastRect());
        }
        internal static Rect GetLastLineRect(Rect rect)
        {
            if (rect == null)
            {
                rect = GUILayoutUtility.GetLastRect();
            }
            float x = rect.x;
            rect.x = 0;
            rect.width += x;
            return rect;
        }

        internal static void DrawLineOverRect(Color color, int padding = 0, int thickness = 1)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            DrawLineOverRect(rect, color, padding, thickness);
        }
        internal static void DrawLineUnderRect(Color color, int padding = 0, int thickness = 1)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            DrawLineUnderRect(rect, color, padding, thickness);
        }
        internal static void DrawLineOverRect(int padding = 0, int thickness = 1)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            DrawLineOverRect(rect, CustomColors.SimpleBright, padding, thickness);
        }
        internal static void DrawLineUnderRect(int padding = 0, int thickness = 1)
        {
            Rect rect = GUILayoutUtility.GetLastRect();
            DrawLineUnderRect(rect, CustomColors.SimpleShadow, padding, thickness);
        }
         internal static void DrawUnderLastComponent()
        {
            EditorUtils.DrawLineUnderRect();
            EditorUtils.DrawLineUnderRect(CustomColors.SimpleBright, 1);
        }
        internal static void DrawLineOverRect(Rect rect, int padding = 0, int thickness = 1)
        {
            DrawLineOverRect(rect, CustomColors.SimpleBright, padding, thickness);
        }
        internal static void DrawLineUnderRect(Rect rect, int padding = 0, int thickness = 1)
        {
            DrawLineUnderRect(rect, CustomColors.SimpleShadow, padding, thickness);
        }
        internal static void DrawLineOverRect(Rect rect, Color color, int padding = 0, int thickness = 1)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMin - padding, rect.width, thickness), color);
        }
        internal static void DrawLineUnderRect(Rect rect, Color color, int padding = 0, int thickness = 1)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height + padding, rect.width, thickness), color);
        }
        internal static void DrawFadeToLeft(Rect rect)
        {
            DrawFadeToLeft(rect, CustomColors.GradientShadow);
        }
        internal static void DrawFadeToLeft(Rect rect, Color color, float limit = 0)
        {
            float fadeLimit = (limit <= 0 || limit > rect.width) ? rect.width : limit;
            float alphaStep = color.a / fadeLimit;

            for (int x = 0; x < fadeLimit; x++)
            {
                float alpha = color.a - (alphaStep * x);
                Color fadedColor = new Color(color.r, color.g, color.b, alpha);
                EditorGUI.DrawRect(new Rect(rect.xMax - x - 1, rect.y, 1, rect.height), fadedColor);
            }
        }
        internal static void DrawFadeToRight(Rect rect)
        {
            DrawFadeToRight(rect, CustomColors.GradientShadow);
        }
        internal static void DrawFadeToRight(Rect rect, Color color, float limit = 0)
        {
            float fadeLimit = (limit <= 0 || limit > rect.width) ? rect.width : limit;
            float alphaStep = color.a / fadeLimit;

            for (int x = 0; x < fadeLimit; x++)
            {
                float alpha = color.a - (alphaStep * x);
                Color fadedColor = new Color(color.r, color.g, color.b, alpha);
                EditorGUI.DrawRect(new Rect(rect.x + x, rect.y, 1, rect.height), fadedColor);
            }
        }
        internal static void DrawFadeToTop(Rect rect)
        {
            DrawFadeToTop(rect, CustomColors.GradientShadow);
        }
        internal static void DrawFadeToTop(Rect rect, Color color, float limit = 0)
        {
            float fadeLimit = (limit <= 0 || limit > rect.height) ? rect.height : limit;
            float alphaStep = color.a / fadeLimit;

            for (int y = 0; y < fadeLimit; y++)
            {
                float alpha = color.a - (alphaStep * y);
                Color fadedColor = new Color(color.r, color.g, color.b, alpha);
                EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - y - 1, rect.width, 1), fadedColor);
            }
        }
        internal static void DrawFadeToBottom(Rect rect)
        {
            DrawFadeToBottom(rect, CustomColors.GradientShadow);
        }
        internal static void DrawFadeToBottom(Rect rect, Color color, float limit = 0)
        {
            float fadeLimit = (limit <= 0 || limit > rect.height) ? rect.height : limit;
            float alphaStep = color.a / fadeLimit;

            for (int y = 0; y < fadeLimit; y++)
            {
                float alpha = color.a - (alphaStep * y);
                Color fadedColor = new Color(color.r, color.g, color.b, alpha);
                EditorGUI.DrawRect(new Rect(rect.x, rect.y + y, rect.width, 1), fadedColor);
            }
        }
        internal static void DrawOutsideRectBorder(Rect rect, Color color, int thickness = 1)
        {
            float borderLeft = rect.x - 1;
            float borderTop = rect.y - 1;
            float borderRight = rect.x + rect.width;
            float borderBottom = rect.y + rect.height;
            EditorGUI.DrawRect(new Rect(borderLeft, borderTop, rect.width + 2, 1), color);
            EditorGUI.DrawRect(new Rect(borderLeft, borderTop, 1, rect.height + 2), color);
            EditorGUI.DrawRect(new Rect(borderLeft, borderBottom, rect.width + 2, 1), color);
            EditorGUI.DrawRect(new Rect(borderRight, borderTop, 1, rect.height + 2), color);
        }
        internal static void DrawRectBorder(Rect rect, Color color, int thickness = 1)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y + rect.height - thickness, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x + rect.width - thickness, rect.y, thickness, rect.height), color);
        }

        internal static void DrawTipSection(CoInspectorWindow window)
        {
            GUILayout.Space(15);
            CustomGUIStyles.StartBoxSection();
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("Pro Tip: ", CustomGUIStyles.BoldLabel);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            CustomGUIStyles.HelpBox(window.GetCurrentTip());
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (window.GetCurrentTip().Contains("Settings Window"))
            {
                Color color = GUI.backgroundColor;
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUI.backgroundColor = CustomColors.NewTabButton;
                if (IsLightSkin())
                {
                    GUI.backgroundColor = Color.blue / 6;
                }

                if (GUILayout.Button(CustomGUIContents.SettingsContent, GUILayout.Width(120), GUILayout.Height(24)))
                {
                    SettingsWindow.ShowWindow();
                }
                CustomGUIContents.DrawCustomButton();
                GUILayout.Space(10);
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = color;
            }
            GUILayout.Space(15);
            EditorGUILayout.EndVertical();
            CustomGUIStyles.EndBoxSection();
        }


    }

    internal class ComponentDragOperation
    {
        public Component draggedComponent;
        public Editor draggedEditor;
        public int targetIndex;
        public int sourceIndex;
        public int sourceTabIndex = -1;
        public GameObject targetObject;
        public bool isSelf;
        public bool errored;
        public bool prefabError;
        public bool removeAfter;
        public bool isCopy;
        public bool isAsset;
        public bool isMouseBelowRect;
        public bool consumed;
        public bool foldoutOrigin;
        public List<UnityObject> assets;
        public Rect mouseOverRect;
        public ComponentDragOperation()
        {
            CoInspectorWindow.alreadyMovingComponent = false;
        }
        public ComponentDragOperation(Component draggedComponent, int targetIndex, int sourceIndex, GameObject target, bool isSelf, bool removeAfter, bool prefabError)
        {
            this.draggedComponent = draggedComponent;
            this.targetIndex = targetIndex;
            this.sourceIndex = sourceIndex;
            this.targetObject = target;
            this.isSelf = isSelf;
            this.removeAfter = removeAfter;
            this.prefabError = prefabError;
            CoInspectorWindow.alreadyMovingComponent = false;
        }
    }
    [Serializable]
    internal class MaterialMapManager
    {
        [SerializeField] internal List<MaterialMap> materialMaps;
        [SerializeField] internal bool timeToRebuild;

        private List<Material> FetchMultiMaterials(Editor componentEditor)
        {
            HashSet<Material> allMaterials = new HashSet<Material>();
            foreach (var target in componentEditor.targets)
            {
                Component component = target as Component;
                List<Material> materials = FetchValidMaterials(component);
                if (materials != null && materials.Count > 0)
                {
                    foreach (Material mat in materials)
                    {
                        allMaterials.Add(mat);
                    }
                }
            }
            HashSet<Material> commonMaterials = new HashSet<Material>(allMaterials);
            foreach (var target in componentEditor.targets)
            {
                List<Material> materials = FetchValidMaterials(target as Component);
                if (materials != null && materials.Count > 0)
                {
                    HashSet<Material> targetMaterials = new HashSet<Material>(materials);
                    commonMaterials.IntersectWith(targetMaterials);
                }
            }
            return commonMaterials.ToList();
        }

        private List<Material> FetchValidMaterials(Component component)
        {
            List<Material> materialsList = new List<Material>();
            if (component is Renderer && !CoInspectorWindow.ContainsMask(component) && component.gameObject.GetComponents<Renderer>().Length == 1)
            {
                materialsList.AddRange((component as Renderer).sharedMaterials);
            }
            else if (component is Graphic)
            {
                bool isMasked = component is MaskableGraphic && component.gameObject.GetComponent<Mask>();

                if (!isMasked)
                {
                    materialsList.Add((component as Graphic).material);
                }
            }
            if (component is Mask)
            {
                Mask mask = component as Mask;
                if (mask.graphic != null)
                {
                    materialsList.Add(mask.GetModifiedMaterial(mask.graphic.material));
                }
            }
            materialsList.RemoveAll(x => x == null);
            materialsList = materialsList.Distinct().ToList();
            return materialsList;
        }


        public MaterialMap GetMaterialMapForComponent(Component component, Editor editor = null)
        {
            if (materialMaps == null)
            {
                materialMaps = new List<MaterialMap>();
            }
            CleanNullEditors();
            CleanEmptyMaps();
            foreach (var map in materialMaps)
            {
                if (map.component == component)
                {

                    return map;
                }
            }
            /*
            if (materials == null || materials.Count == 0)
            {
                return null;
            } */
            MaterialMap newMap = new MaterialMap
            {
                component = component
            };
            if (editor != null)
            {
                newMap.materials = FetchMultiMaterials(editor);
            }
            else
            {
                newMap.materials = FetchValidMaterials(component);
            }
            newMap.editors = new List<Editor>(newMap.materials.Count);
            foreach (var material in newMap.materials)
            {
                if (material == null)
                {
                    continue;
                }
                newMap.editors.Add(Editor.CreateEditor(material));
            }


            materialMaps.Add(newMap);
            return newMap;
        }
        public MaterialMap _GetMaterialMapForComponent(Component component)
        {
            if (materialMaps == null || materialMaps.Count == 0)
            {
                return null;
            }
            foreach (var map in materialMaps)
            {
                if (map.component == component)
                {
                    return map;
                }
            }
            return null;
        }

        public void DestroyIfPresent(Component component)
        {
            if (materialMaps == null)
            {
                return;
            }
            List<MaterialMap> _materialMaps = new List<MaterialMap>(materialMaps);
            foreach (var map in _materialMaps)
            {
                if (map.component == component)
                {
                    CoInspectorWindow.DestroyAllIfNotNull(map.editors.ToArray());
                    materialMaps.Remove(map);
                    return;
                }
            }
        }
        void CleanNullEditors()
        {
            if (materialMaps == null)
            {
                return;
            }
            foreach (var map in materialMaps)
            {
                if (map.editors == null)
                {
                    map.editors = new List<Editor>();
                }
                List<Editor> _editors = new List<Editor>();
                foreach (var editor in map.editors)
                {
                    if (editor != null && editor.target != null)
                    {
                        _editors.Add(editor);
                    }
                    else
                    {
                        CoInspectorWindow.DestroyIfNotNull(editor);
                    }
                }
                map.editors = _editors;
            }
        }
        public void RebuildIfNecessary()
        {
            if (timeToRebuild)
            {
                _DestroyAllMaterialMaps();
            }
        }
        public void _DestroyAllMaterialMaps()
        {
            DestroyAllMaterialMaps();
            timeToRebuild = false;

        }

        void CleanEmptyMaps()
        {
            if (materialMaps == null)
            {
                materialMaps = new List<MaterialMap>();
                return;
            }
            List<MaterialMap> _materialMaps = new List<MaterialMap>();

            foreach (var map in materialMaps)
            {
                if ((map.editors == null && map.materials != null) ||
                    (map.editors != null && map.materials != null && map.editors.Count != map.materials.Count))
                {
                    CoInspectorWindow.DestroyAllIfNotNull(map.editors?.ToArray());
                }
                else
                {
                    _materialMaps.Add(map);
                }
            }

            materialMaps = _materialMaps;

        }

        public void DestroyAllMaterialMaps()
        {
            if (materialMaps == null || materialMaps.Count == 0)
            {
                return;
            }
            foreach (var map in materialMaps)
            {
                CoInspectorWindow.DestroyAllIfNotNull(map.editors.ToArray());
            }
            TabInfo.DebugMaterialEditors();
            materialMaps = new List<MaterialMap>();
        }

        public bool IsMaterialMapValid(MaterialMap map, Component component, List<Material> materials)
        {
            if (map.component == component && map.materials != null && materials != null && map.materials.Count == materials.Count)
            {
                for (int i = 0; i < materials.Count; i++)
                {
                    if (map.materials[i] != materials[i])
                    {
                        return false;
                    }
                    if (map.editors[i] == null || map.editors[i].target != materials[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public void CleanMaterialMaps(Component[] components)
        {
            if (materialMaps == null)
            {
                materialMaps = new List<MaterialMap>();
                return;
            }
            CleanNullEditors();

            List<MaterialMap> _materialMaps = new List<MaterialMap>();
            foreach (var map in materialMaps)
            {

                if (components.Contains(map.component) && map.materials != null && map.materials.Count > 0)
                {
                    _materialMaps.Add(map);
                }
                else
                {
                    CoInspectorWindow.DestroyAllIfNotNull(map.editors.ToArray());
                }
            }
            materialMaps = _materialMaps;
        }
    }
    [Serializable]
    internal class MaterialMap
    {
        [SerializeField] internal Component component;
        [SerializeField] internal List<Material> materials;
        [SerializeField] internal List<Editor> editors;
    }
    [Serializable]
    internal class ComponentMap
    {
        [SerializeField] internal int index;
        [SerializeField] internal Component component;
        [SerializeField] internal bool foldout;
        [SerializeField] internal bool focusAfter;
        [SerializeField] internal bool awaitingScroll;
        [SerializeField] internal bool hidden;
        [NonSerialized] internal float height = -1;
        [NonSerialized] internal bool isCulled = false;
        [NonSerialized] internal bool isFilteredOut = false;
        /*
         [SerializeField] internal SerializedProperty[] infos;
         [SerializeField] internal SerializedObject serializedObject;
         */
        public ComponentMap(ComponentMap other)
        {
            this.component = other.component;
            this.foldout = other.foldout;
            this.focusAfter = other.focusAfter;
            this.awaitingScroll = other.awaitingScroll;
            this.hidden = other.hidden;
            this.height = -1;
        }
        public ComponentMap() { }
    }
    internal class FloatingTab
    {
        internal TabInfo linkedTab;
        internal CoInspectorWindow owner;
        internal Rect tabRect;
        internal bool showIcon;
        internal bool isSelected;
        internal bool isClosing;
        internal bool isOpening;
        internal Texture2D icon;
        internal float startX;
        internal float tabWidth;
        internal float targetTabX;
        internal float tabDragPoint = 0;
        internal int dragTargetIndex = -1;
        internal int dragIndex = -1;
        internal GUIStyle style;
        internal bool fallingTab = false;
        internal float startTime = -1f;
        internal float animationDuration = 0.2f;

        internal FloatingTab(CoInspectorWindow _owner)
        {
            this.owner = _owner;
        }

        internal void Reset(CoInspectorWindow _owner)
        {
            linkedTab = null;
            tabRect = Rect.zero;
            showIcon = false;
            isSelected = false;
            isClosing = false;
            isOpening = false;
            icon = null;
            startX = 0;
            targetTabX = 0;
            tabDragPoint = 0;
            dragTargetIndex = -1;
            dragIndex = -1;
            fallingTab = false;
            startTime = -1f;
            owner = _owner;
        }
        internal void StartOpeningTab(TabInfo tab)
        {
            if (!isOpening)
            {
                linkedTab = tab;
                isClosing = false;
                isOpening = true;
                startTime = Time.realtimeSinceStartup;
            }
        }
        internal float GetOpeningTabWidth()
        {
            tabWidth = linkedTab.tabWidth;
            if (Event.current == null)
            {
                return tabWidth;
            }
            if (!isOpening || startTime < 0f)
            {
                isOpening = false;
                return tabWidth;
            }
            float t = (Time.realtimeSinceStartup - startTime) / 0.1f;
            if (t >= 1f)
            {
                isOpening = false;
                return tabWidth;
            }
            float value = Mathf.Lerp(0f, tabWidth, t);
            owner.Repaint();
            return value;
        }

        internal void StartClosingTab(TabInfo tab)
        {
            if (!isClosing)
            {
                linkedTab = tab;
                isClosing = true;
                isOpening = false;
                startTime = Time.realtimeSinceStartup;
                tabWidth = tab.tabWidth;
            }
        }

        internal float GetClosingTabWidth()
        {
            if (Event.current == null)
            {
                return 0f;
            }

            if (!isClosing || startTime < 0f)
            {
                FinishClosing();
                return 0f;
            }

            float t = (Time.realtimeSinceStartup - startTime) / 0.07f;
            if (t >= 1f)
            {
                FinishClosing();
                return 0f;
            }

            float value = Mathf.Lerp(tabWidth, 0f, t);
            owner.Repaint();
            return value;
        }

        private void FinishClosing()
        {
            if (linkedTab != null && !linkedTab.markForDeletion)            
            {
                linkedTab.markForDeletion = true;
            }
            owner.Repaint();
        }
    }
    [Serializable]
    internal class HistoryPaths
    {
        [SerializeField] internal string[] paths;
        internal int[] instances;
        [SerializeField] internal bool prefab;

        public HistoryPaths(string[] history, int[] instanceIDs, bool _prefab = false)
        {
            paths = new string[history.Length];
            Array.Copy(history, paths, history.Length);
            instances = new int[instanceIDs.Length];
            Array.Copy(instanceIDs, instances, instanceIDs.Length);
            prefab = _prefab;
        }
    }

    [Serializable]
    internal class HistoryAssets
    {
        [SerializeField] internal string[] assetGUIDs;

        public HistoryAssets(string[] newGUIDs)
        {
            if (newGUIDs == null)
            {
                return;
            }
            assetGUIDs = new string[newGUIDs.Length];
            Array.Copy(newGUIDs, assetGUIDs, newGUIDs.Length);
        }

        public HistoryAssets(UnityEngine.Object[] assets)
        {
            if (assets == null)
            {
                return;
            }
            assetGUIDs = assets.Where(asset => asset != null)
                               .Select(asset => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset)))
                               .Where(guid => !string.IsNullOrEmpty(guid))
                               .ToArray();
        }
        public HistoryAssets Clone()
        {
            return new HistoryAssets(this.assetGUIDs);
        }
    }
    [Serializable]
    internal class SceneInfo
    {
        [SerializeField] private string sceneName;
        [SerializeField] private string scenePath;
        [SerializeField] private string sceneGUID;

        internal string SceneName => sceneName;
        internal string ScenePath => scenePath;
        internal string SceneGUID => sceneGUID;

        internal SceneInfo(string sceneName, string sceneGUID, string scenePath)
        {
            this.sceneName = sceneName;
            this.scenePath = scenePath;
            this.sceneGUID = sceneGUID;
        }

        internal bool IsValid()
        {
            return !string.IsNullOrEmpty(sceneName) && !string.IsNullOrEmpty(sceneGUID) && !string.IsNullOrEmpty(scenePath);
        }

        internal static SceneInfo FromActiveScene(SceneInfo previousValidScene = null)
        {
            Scene activeScene = SceneManager.GetActiveScene();

            if (!IsSceneValid(activeScene))
            {
                return previousValidScene ?? CreateInvalidSceneInfo();
            }

            string sceneGUID = AssetDatabase.AssetPathToGUID(activeScene.path);
            SceneInfo newSceneInfo = new SceneInfo(activeScene.name, sceneGUID, activeScene.path);

            return newSceneInfo.IsValid() ? newSceneInfo : (previousValidScene ?? CreateInvalidSceneInfo());
        }

        private static bool IsSceneValid(Scene scene)
        {
            return scene.IsValid() && !string.IsNullOrEmpty(scene.path);
        }

        private static SceneInfo CreateInvalidSceneInfo()
        {
            return new SceneInfo("Invalid Scene", string.Empty, string.Empty);
        }
    }

}
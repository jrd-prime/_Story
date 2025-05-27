using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
#if UNITY_2021_2_OR_NEWER
#else
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace CoInspector
{
    [Serializable]
    internal class TabInfo
    {
        public string path = "";
        public string[] paths;
        public float tabWidth = 0;
        [NonSerialized] public int id = 0;
        [NonSerialized] public int[] ids;
        public string name;
        public Texture2D icon;
        public string shortName;
        public string filterString = "";
        public bool newTab = false;
        internal bool markForDeletion = false;
        internal bool willBeDeleted = false;
        public float scrollPosition;
        public bool isSelected = false;
        public bool allCollapsed = false;
        public bool[] runtimeFoldouts;
        public Component[] runtimeMultiComponents;
        public GUIContent tabTextContent;
        public bool multiEditMode = false;
        public bool locked;
        public bool debug;
        public bool prefab = false;
        public bool multiFoldout = true;
        public GameObject target;
        public GameObject[] targets;
        public int index;
        public int historyPosition = 0;
        public List<GameObject[]> history = new List<GameObject[]>();
        public List<ComponentMap> componentMaps;
        private MaterialMapManager materialMapManager;
        public List<HistoryPaths> historyPaths;
        public CoInspectorWindow owner;
        internal bool zoomFocus = false;

        public TabInfo(GameObject target, int index, CoInspectorWindow owner, string _name = "")
        {
            this.owner = owner;
            this.index = index;
            this.filterString = "";
            this.newTab = true;
            this.name = "New Tab";
            if (_name == "" && target != null)
            {
                name = target.name;
            }
            else
            {
                name = _name;
            }
            this.target = target;
            this.newTab = !target;
            if (owner)
            {
                UpdateTabName();
                UpdatePath();
                UpdateTabWidth(true);
                owner.UpdateTabBar();
            }
        }
        public TabInfo(GameObject[] targets, int index, CoInspectorWindow owner)
        {
            this.owner = owner;
            this.index = index;
            this.newTab = true;
            this.name = "New Tab";
            this.filterString = "";
            if (targets != null && targets.Length > 0)
            {
                if (targets.Length == 1)
                {
                    GameObject gameObject = targets[0];
                    if (gameObject != null)
                    {
                        name = gameObject.name;
                    }
                    this.target = gameObject;
                    this.newTab = !gameObject;
                    return;
                }
                this.name = "(" + targets.Length + ") Objects";
                this.multiEditMode = true;
                this.targets = targets;
                this.newTab = false;
                if (owner)
                {
                    UpdateTabName();
                    UpdatePath();
                    UpdateTabWidth(true);
                    owner.UpdateTabBar();
                }
            }
        }

        public TabInfo(TabInfo reference)
        {
            this.path = reference.path;
            this.paths = reference.paths;
            this.name = reference.name;
            this.filterString = reference.filterString;
            this.icon = reference.icon;
            this.shortName = reference.shortName;
            this.newTab = reference.newTab;
            this.isSelected = reference.isSelected;
            this.runtimeFoldouts = reference.runtimeFoldouts;
            this.multiEditMode = reference.multiEditMode;
            this.locked = reference.locked;
            this.debug = reference.debug;
            this.prefab = reference.prefab;
            this.target = reference.target;
            this.targets = reference.targets;
            this.index = reference.index;
            this.historyPosition = reference.historyPosition;
            this.history = reference.history;
            this.owner = reference.owner;
            this.id = reference.id;
            this.ids = reference.ids;
            this.historyPaths = reference.historyPaths;
            this.componentMaps = reference.componentMaps?.Select(cm => new ComponentMap(cm)).ToList();
            this.multiFoldout = reference.multiFoldout;
            this.scrollPosition = reference.scrollPosition;
            this.allCollapsed = reference.allCollapsed;
            this.tabWidth = reference.tabWidth;
            this.runtimeMultiComponents = reference.runtimeMultiComponents;
            this.tabTextContent = reference.tabTextContent;
            this.markForDeletion = false;
            this.willBeDeleted = false;
        }
        internal void SetNotFiltering()
        {
            if (componentMaps == null)
            {
                foreach (var map in componentMaps)
                {
                    map.isFilteredOut = false;
                }
            }
        }



        public GUIContent TabTextContent
        {
            get
            {
                if (tabTextContent == null)
                {
                    tabTextContent = new GUIContent(shortName ?? string.Empty);
                }
                else
                {
                    tabTextContent.text = shortName ?? string.Empty;
                }
                return tabTextContent;
            }
        }

        internal void UpdateTabName()
        {
            if (owner == null)
            {
                return;
            }
            string _name = name;
            if (newTab)
            {
                _name = "New Tab";
            }
            _name = _name.TrimStart();
            _name = _name.TrimEnd();

            int charLimit = 15;
            if (CoInspectorWindow.tabCompactMode == 2)
            {
                charLimit = 18;
            }
            int tabCount = owner.tabs.Count;
            int tabLimit = 2;
            if (CoInspectorWindow.tabCompactMode == 2)
            {
                tabLimit = 3;
            }
            for (int j = 0; j < tabCount; j++)
            {
                if (j >= tabLimit)
                {
                    charLimit -= 2;
                }
                else
                {
                    charLimit -= 1;
                }
            }

            if (charLimit < 4)
            {
                charLimit = 4;
            }
            if (!newTab && _name.Length > charLimit && tabCount > 1 && _name.Length - charLimit > 1)
            {
                _name = _name.Substring(0, charLimit).TrimEnd() + "…";
            }

            if (shortName != _name || tabWidth == 0)
            {
                shortName = _name;
                UpdateTabWidth();
            }
        }

        public void UpdateTabWidth(bool force = false)
        {
            if (CustomGUIStyles.editorToolbarButton == null)
            {
                return;
            }
            if (TabTextContent != null)
            {
                if ((CoInspectorWindow.showIcons || locked) && !newTab)
                {
                    CustomGUIStyles.ToolbarButtonTabs.padding = owner.PaddingIcon;
                }
                else
                {
                    CustomGUIStyles.ToolbarButtonTabs.padding = owner.PaddingNoIcon;
                }
                int padding = 7;
                tabWidth = Mathf.RoundToInt(CustomGUIStyles.ToolbarButtonTabs.CalcSize(TabTextContent).x) + padding;
            }
        }

        public void AddToHistoryIfProceeds(GameObject[] newTargets)
        {
            zoomFocus = false;
            if (newTargets != null && newTargets.Length > 0)
            {

                if (newTargets != null && !EditorUtils.CompareArrays(newTargets, targets))
                {
                    AddToHistory(newTargets);
                }
                else if (newTargets != null && history == null || history.Count == 0)
                {
                    AddToHistory(newTargets);
                }
            }

        }

        public void AddToHistoryIfProceeds(GameObject newTarget)
        {
            zoomFocus = false;
            multiFoldout = true;
            if (newTarget != null)
            {

                if (newTarget != null && newTarget != target)
                {
                    AddToHistory(new GameObject[] { newTarget });

                }
                else if (newTarget != null && history == null || history.Count == 0)
                {
                    AddToHistory(new GameObject[] { newTarget });
                }
            }

        }


        public void ResetTab()
        {
            target = null;
            targets = null;
            locked = false;
            debug = false;
            multiEditMode = false;
            multiFoldout = true;
            newTab = true;
            prefab = false;
            name = "New Tab";
            path = "";
            paths = null;
            id = 0;
            ids = null;
            history = new List<GameObject[]>();
            historyPosition = 0;
            componentMaps = new List<ComponentMap>();
            historyPaths = new List<HistoryPaths>();
            zoomFocus = false;
            runtimeMultiComponents = null;
            runtimeFoldouts = null;
            scrollPosition = 0;
            allCollapsed = false;
            DestroyAllMaterialMaps();
            if (CoInspectorWindow.MainCoInspector)
            {
                if (CoInspectorWindow.MainCoInspector.GetActiveTab() == this)
                {
                    CoInspectorWindow.MainCoInspector.targetGameObject = null;
                }
            }
        }

       internal void SetPrefabMode()
       {
            if (newTab)
            {
                prefab = false;
                return;
            }
            if (IsValidMultiTarget())
            {
                prefab = CoInspectorWindow.AreAllGameObjectsInPrefabMode(targets);
            }
            else if (target != null)
            {
                prefab = CoInspectorWindow.IsGameObjectInPrefabMode(target);
            }

       }


        public void LoadTargetFromPath()
        {
            if (path != "" && !multiEditMode)
            {
                target = GameObject.Find(path);
                if (target != null)
                {
                    newTab = false;
                }
            }
            else if (IsValidMultiTarget() && paths != null && paths.Length > 0)
            {
                List<GameObject> _targets = new List<GameObject>();
                foreach (var _path in paths)
                {
                    GameObject _target = GameObject.Find(_path);
                    if (_target != null)
                    {
                        newTab = false;
                        _targets.Add(_target);
                    }
                }
                targets = _targets.ToArray();
            }
        }
        internal bool IsTabValid(bool excludeNew = false)
        {
            if (excludeNew && newTab)
            {
                return false;
            }
            if (newTab)
            {
                return true;
            }
            if (IsValidMultiTarget())
            {
                return true;
            }
            if (target != null)
            {
                return true;
            }
            return false;
        }
        internal void AutoSortTargets()
        {
            if (target == null && (targets == null || targets.Length == 0))
            {
                multiEditMode = false;
                target = null;
                targets = null;
                return;
            }
            if (target != null && (targets == null || targets.Length == 0))
            {
                multiEditMode = false;
                targets = null;
                return;
            }
            if (targets != null && targets.Length > 0)
            {
                if (targets.Length > 1)
                {
                    multiEditMode = true;
                    target = null;
                    return;
                }
                if (targets.Length == 1)
                {
                    multiEditMode = false;
                    target = targets[0];
                    targets = null;
                    return;
                }
            }
            multiEditMode = false;
            targets = null;
        }

        public bool IsValidMultiTarget(bool _debug = false)
        {
            if (targets != null && targets.Length > 0 && multiEditMode)
            {
                foreach (var _target in targets)
                {
                    if (_target == null)
                    {
                        return false;
                    }
                }
                return true;
            }
            /*
            if (debug)
            {
                if (targets == null)
                {
                    Debug.Log("Targets null");
                }
                else if (targets.Length == 0)
                {
                    Debug.Log("Targets empty");
                }
                if (!multiEditMode)
                {
                    Debug.Log("Not multi edit mode");
                }
            }*/

            return false;
        }

        public bool HasNullMultiTargets()
        {
            return targets?.Any(t => t == null) ?? false;
        }

        public void UpdatePath()
        {
            if (newTab)
            {
                path = "";
                paths = null;
                id = 0;
                ids = null;
                historyPaths = null;
                history = null;
                return;
            }
            if (target != null)
            {
                path = EditorUtils.GatherGameObjectPath(target);
                id = target.GetInstanceID();
                paths = null;
                ids = null;
            }
            else if (IsValidMultiTarget())
            {
                paths = targets.Select(EditorUtils.GatherGameObjectPath).ToArray();
                ids = targets.Select(t => t.GetInstanceID()).ToArray();
                path = "";
                id = 0;
               
            }
            if (IsTabValid() && history != null && history.Count > 0)
            {
                historyPaths = history.Select(_history =>
                {
                    var _paths = _history.Select(EditorUtils.GatherGameObjectPath).ToArray();
                    var _ids = _history.Select(h => h.GetInstanceID()).ToArray();
                    return new HistoryPaths(_paths, _ids, prefab);
                }).ToList();
               
            }
        }      

        public bool HasValidTargets()
        {
            if (targets != null && targets.Length > 0)
            {
                return RemoveNulls();
            }
            return false;
        }

        public bool RemoveNulls()
        {
            List<GameObject> _targets = new List<GameObject>();
            foreach (var _target in targets)
            {
                if (_target != null)
                {
                    _targets.Add(_target);
                }
            }
            targets = _targets.ToArray();
            return targets.Length > 0;
        }
        bool IsMulti(int _index)
        {
            if (_index < 0 || _index >= history.Count)
            {
                return false;
            }
            if (history[_index] != null && history[_index].Length > 1)
            {
                return true;
            }
            return false;
        }

        public List<string> NamesBack()
        {
            List<string> names = new List<string>();
            for (int i = historyPosition; i < history.Count; i++)
            {
                if (IsMulti(i))
                {
                    names.Add("(" + history[i].Length + ") Objects");
                }
                else
                {
                    names.Add(history[i][0].name);
                }
            }
            return names;
        }
        public List<string> NamesForward()
        {
            List<string> names = new List<string>();
            for (int i = historyPosition; i > 0; i--)
            {
                if (IsMulti(i))
                {
                    names.Add("(" + history[i].Length + ") Objects");
                }
                else
                {
                    names.Add(history[i][0].name);
                }
            }
            return names;
        }

        public void OnDestroy()
        {
            DestroyAllMaterialMaps();
        }      
        public void RefreshIcon()
        {
            if (target != null)
            {
                icon = EditorUtils.GetBestFittingIconForGameObject(target);
            }
        }       

        public bool IsAPrefabTab()
        {
            if (IsValidMultiTarget())
            {
                return EditorUtils._AreAllPrefabs(targets);
            }
            else if (target != null)
            {
                return PrefabUtility.IsAnyPrefabInstanceRoot(target);
            }
            return false;
        }
        public void AddToHistory(GameObject[] newTargets)
        {
            if (newTargets == null)
            {
                return;
            }
            if (newTargets.Length == 1)
            {
                _AddToHistory(newTargets[0]);
                FixNulls();
                return;
            }
            if (locked)
            {
                return;
            }
            FixNulls();
            CoInspectorWindow.justOpened = false;
            RemoveIfAlreadyInHistory(newTargets);
            if (historyPosition != 0 && !EditorUtils.CompareArrays(newTargets, targets))
            {
                history.RemoveRange(0, -historyPosition);
                historyPosition = 0;
            }
            if (history.Count > 10)
            {
                history.RemoveAt(history.Count - 1);
            }
            if (history.Count == 0 && targets != null && targets != newTargets)
            {
                history.Insert(0, targets);
            }
            history.Insert(0, newTargets);
            targets = newTargets;
            target = null;
            name = "(" + targets.Length + ") Objects";
            FixNulls();
        }
        bool AlreadyInHistory(GameObject newTarget)
        {
            for (int i = 0; i < history.Count; i++)
            {
                if (IsMulti(i))
                {
                    continue;
                }
                if (history[i][0] == newTarget)
                {
                    return true;
                }
            }
            return false;
        }

        void RemoveIfAlreadyInHistory(GameObject[] newTargets)
        {
            for (int i = 0; i < history.Count; i++)
            {
                if (!IsMulti(i) && EditorUtils.CompareArrays(history[i], newTargets))
                {
                    history.RemoveAt(i);
                    return;
                }
            }
        }

        void RemoveIfAlreadyInHistory(GameObject newTarget)
        {
            for (int i = 0; i < history.Count; i++)
            {
                if (!IsMulti(i) && history[i][0] == newTarget)
                {
                    history.RemoveAt(i);
                    return;
                }
            }
        }
        public void TrySetValidHistoryTarget()
        {
            FixNulls();
            if (history == null)
            {
                return;
            }
            if (history.Count > 0)
            {
                if (history[0] != null && history[0].Length > 0)
                {
                    if (history[0].Length > 1)
                    {
                        targets = history[0];
                        target = null;
                        name = "(" + targets.Length + ") Objects";
                        if (owner.GetActiveTab() != null && owner.GetActiveTab() == this)
                        {
                            owner.SetTargetGameObjects(targets);
                        }
                        else
                        {
                            RefreshIcon();
                        }
                        multiEditMode = true;
                        return;
                    }
                    else
                    {
                        target = history[0][0];
                        targets = null;
                        name = target.name;
                        if (owner.GetActiveTab() != null && owner.GetActiveTab() == this)
                        {
                            owner.SetTargetGameObject(target);
                        }
                        else
                        {
                            RefreshIcon();
                        }
                        multiEditMode = false;
                        return;
                    }
                }

                if (CanMoveBack())
                {
                    MoveBack();
                }
                else if (CanMoveForward())
                {
                    MoveForward();
                }
            }
        }

        public void _AddToHistory(GameObject newTarget)
        {
            newTab = false;
            if (newTarget == null)
            {
                return;
            }
            if (locked && !multiEditMode)
            {
                return;
            }
            else if (multiEditMode)
            {
                return;
            }
            FixNulls();
            CoInspectorWindow.justOpened = false;
            if (historyPosition != 0 && newTarget != target)

            {
                history.RemoveRange(0, -historyPosition);
                historyPosition = 0;
            }
            RemoveIfAlreadyInHistory(newTarget);
            if (history.Count > 10)
            {
                history.RemoveAt(history.Count - 1);
            }
            if (history.Count == 0 && target && target != newTarget)
            {
                history.Insert(0, new GameObject[] { target });
            }
            target = newTarget;
            targets = null;
            icon = EditorUtils.GetBestFittingIconForGameObject(newTarget);
            history.Insert(0, new GameObject[] { newTarget });
            name = target.name;
        }

        public void MoveBack(int movements = 1)
        {
            FixNulls();
            if (history.Count > -historyPosition + 1)
            {
                historyPosition -= movements;
                if (IsMulti(-historyPosition))
                {
                    targets = history[-historyPosition];
                    target = null;
                    name = "(" + targets.Length + ") Objects";
                    if (owner.GetActiveTab() == this)
                    {
                        owner.SetTargetGameObjects(targets);
                    }
                    else
                    {
                        RefreshIcon();
                    }
                }
                else
                {
                    target = history[-historyPosition][0];
                    targets = null;
                    name = target.name;
                    if (owner.GetActiveTab() == this)
                    {
                        owner.SetTargetGameObject(target, true);
                    }
                    else
                    {
                        RefreshIcon();
                    }
                }
            }
        }
        public void MoveForward(int movements = 1)
        {
            FixNulls();
            if (historyPosition < 0)
            {
                historyPosition += movements;
                if (IsMulti(-historyPosition))
                {
                    targets = history[-historyPosition];
                    target = null;

                    name = "(" + targets.Length + ") Objects";
                    if (owner.GetActiveTab() == this)
                    {
                        owner.SetTargetGameObjects(targets);
                    }
                }
                else
                {
                    target = history[-historyPosition][0];
                    targets = null;

                    name = target.name;
                    if (owner.GetActiveTab() == this)
                    {
                        owner.SetTargetGameObject(target, true);
                    }
                }
            }
        }
        public void RefreshName()
        {
            if (IsValidMultiTarget())
            {
                name = "(" + targets.Length + ") Objects";
                target = null;
            }
            else if (target != null)
            {
                name = target.name;
                targets = null;
            }
        }
        public void TryRepopulateHistory()
        {
            if (newTab)
            {
                return;
            }
            history = EditorUtils.GetHistory(this);
        }

        public void MoveForwardUntil(GameObject[] _target, int maxMovements = 10)
        {
            if (maxMovements == 0)
            {
                return;
            }
            if (EditorUtils.ContainsArray(ForwardHistory(), _target))

            {
                MoveForward();
                if (_target.Length == 1)
                {
                    if (this.target != _target[0])
                    {
                        MoveForwardUntil(_target, maxMovements - 1);
                    }
                }
                else
                {
                    if (!EditorUtils.CompareArrays(this.targets, _target))
                    {
                        MoveForwardUntil(_target, maxMovements - 1);
                    }
                }
            }
        }        

        public void MoveBackUntil(GameObject[] _target, int maxMovements = 10)
        {
            if (EditorUtils.ContainsArray(BackHistory(), _target))
            {
                if (maxMovements == 0)
                {
                    return;
                }
                MoveBack();
                if (_target.Length == 1)
                {
                    if (this.target != _target[0])
                    {
                        MoveBackUntil(_target, maxMovements - 1);
                    }
                }
                else
                {
                    if (!EditorUtils.CompareArrays(this.targets, _target))
                    {
                        MoveBackUntil(_target, maxMovements - 1);
                    }
                }
            }
        }
        public List<string> _ForwardHistory()
        {
            FixNulls();
            List<string> historyStrings = new List<string>();
            Dictionary<string, int> nameCounts = new Dictionary<string, int>();

            for (int i = 0; i < -historyPosition; i++)
            {
                string itemName;
                if (this.history[i].Length == 1 && this.history[i][0] != null)
                {
                    itemName = this.history[i][0].name;
                }
                else if (this.history[i].Length > 1)
                {
                    itemName = $"({this.history[i].Length}) Objects";
                }
                else
                {
                    continue;
                }

                if (nameCounts.ContainsKey(itemName))
                {
                    nameCounts[itemName]++;
                    itemName += new string(' ', nameCounts[itemName]);
                }
                else
                {
                    nameCounts[itemName] = 0;
                }

                historyStrings.Add(itemName);
            }

            historyStrings.Reverse();
            return historyStrings;
        }

        public List<string> _BackHistory()
        {
            FixNulls();
            List<string> historyStrings = new List<string>();
            Dictionary<string, int> nameCounts = new Dictionary<string, int>();

            for (int i = -historyPosition + 1; i < this.history.Count; i++)
            {
                string itemName;
                if (this.history[i].Length == 1 && this.history[i][0] != null)
                {
                    itemName = this.history[i][0].name;
                }
                else if (this.history[i].Length > 1)
                {
                    itemName = $"({this.history[i].Length}) Objects";
                }
                else
                {
                    continue;
                }

                if (nameCounts.ContainsKey(itemName))
                {
                    nameCounts[itemName]++;
                    itemName += new string(' ', nameCounts[itemName]);
                }
                else
                {
                    nameCounts[itemName] = 0;
                }

                historyStrings.Add(itemName);
            }

            return historyStrings;
        }

        public List<GameObject[]> ForwardHistory()
        {
            FixNulls();
            List<GameObject[]> forwardHistory = new List<GameObject[]>();
            for (int i = 0; i < -historyPosition; i++)
            {
                forwardHistory.Add(this.history[i]);
            }
            forwardHistory.Reverse();
            return forwardHistory;
        }
        public List<GameObject[]> BackHistory()
        {
            FixNulls();
            List<GameObject[]> backHistory = new List<GameObject[]>();
            for (int i = -historyPosition + 1; i < this.history.Count; i++)
            {
                backHistory.Add(this.history[i]);
            }
            return backHistory;
        }

        public bool CanMoveBack()
        {
            if (history == null)
            {
                return false;
            }
            int relativePosition = -historyPosition + 1;
            return history.Count > relativePosition && history.Count > 1;
        }

        public bool CanMoveForward()
        {
            if (history == null)
            {
                return false;
            }
            return historyPosition < 0;
        }

        public bool IsObjectInSamePrefabState(GameObject gameObject)
        {
            if (prefab)
            {
                return CoInspectorWindow.IsGameObjectInPrefabMode(gameObject);
            }
            return !CoInspectorWindow.IsGameObjectInPrefabMode(gameObject);
        }

        public void FixNulls()
        {
            if (history == null)
            {
                history = new List<GameObject[]>();
                historyPosition = 0;
                return;
            }
            /*
            foreach (var _history in history)
            {
                if (_history == null)
                {
                    Debug.Log(_history[0].name);
                }
            }*/

            Scene activeScene = SceneManager.GetActiveScene();
#if UNITY_2021_2_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
#else
    UnityEditor.Experimental.SceneManagement.PrefabStage prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif
            bool isPrefabStageActive = prefabStage != null;

            int currentIndex = history.Count + historyPosition;
            int removedBeforeCurrent = 0;
            int removedAfterCurrent = 0;

            List<GameObject[]> newHistory = new List<GameObject[]>();
            for (int i = 0; i < history.Count; i++)
            {
                GameObject[] entry = history[i];
                if (entry == null)
                {
                    if (i < currentIndex)
                        removedBeforeCurrent++;
                    else if (i > currentIndex)
                        removedAfterCurrent++;
                    continue;
                }

                GameObject[] validObjects = entry
                    .Where(go => IsValidGameObject(go, activeScene, isPrefabStageActive))
                    .ToArray();

                if (validObjects.Length > 0)
                    newHistory.Add(validObjects);
                else
                {
                    if (i < currentIndex)
                        removedBeforeCurrent++;
                    else if (i > currentIndex)
                        removedAfterCurrent++;
                }
            }
            history = newHistory.Distinct(new GameObjectArrayComparer()).ToList();
            int removedAtCurrent = (currentIndex >= 0 && currentIndex < newHistory.Count && !history.Contains(newHistory[currentIndex])) ? 1 : 0;
            historyPosition += removedBeforeCurrent;
            historyPosition -= removedAfterCurrent;
            historyPosition += removedAtCurrent;
            historyPosition = Mathf.Clamp(historyPosition, -history.Count, 0);
        }
        private class GameObjectArrayComparer : IEqualityComparer<GameObject[]>
        {
            public bool Equals(GameObject[] x, GameObject[] y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null || x.Length != y.Length)
                    return false;

                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i] != y[i])
                        return false;
                }
                return true;
            }

            public int GetHashCode(GameObject[] obj)
            {
                unchecked
                {
                    int hash = 17;
                    foreach (var gameObject in obj)
                    {
                        hash = hash * 23 + (gameObject != null ? gameObject.GetHashCode() : 0);
                    }
                    return hash;
                }
            }
        }

        private bool IsValidGameObject(GameObject go, Scene activeScene, bool isPrefabStageActive)
        {

            if (go != null && go.scene == activeScene)
            {
                return true;
            }
            if (isPrefabStageActive)
            {
#if UNITY_2021_2_OR_NEWER
                UnityEditor.SceneManagement.PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
#endif

                if (prefabStage != null && go != null)
                {
                    return prefabStage.scene == go.scene;
                }
            }
            return false;
        }

        private SerializedProperty[] GetSerializedProperties(SerializedObject serializedObject)
        {
            List<SerializedProperty> propertyList = new List<SerializedProperty>();
            SerializedProperty property = serializedObject.GetIterator();

            while (property.NextVisible(true))
            {
                propertyList.Add(property.Copy());
            }

            return propertyList.ToArray();
        }

        public ComponentMap SaveFoldoutToMap(Component component, bool foldout, Editor editor, bool focusAfter = false)
        {
            if (componentMaps == null)
            {
                componentMaps = new List<ComponentMap>();
            }
            else
            {
                foreach (var map in componentMaps)
                {
                    if (map.component == component)
                    {
                        map.foldout = foldout;
                        map.focusAfter = focusAfter;                       
                        return map;
                    }
                }
            }

            ComponentMap newMap = new ComponentMap
            {
                component = component,
                foldout = foldout,
                focusAfter = focusAfter

            };
            /*
             if (editor != null)
                {
                    newMap.serializedObject = editor.serializedObject;
                    newMap.infos = GetSerializedProperties(editor.serializedObject);
                } */

            componentMaps.Add(newMap);
            return newMap;
        }

        public bool GetFoldoutForComponent(Component component, Editor editor)
        {
            if (componentMaps == null || componentMaps.Count == 0)
            {

                SaveFoldoutToMap(component, !allCollapsed, editor);
                return true;
            }
            foreach (var map in componentMaps)
            {
                if (map.component == component)
                {
                    return map.foldout;
                }
            }
            return true;
        }

        public ComponentMap GetFoldoutMapForComponent(Component component, Editor editor)
        {
            if (componentMaps == null || componentMaps.Count == 0)
            {

                return SaveFoldoutToMap(component, !allCollapsed, editor);
            }
            foreach (var map in componentMaps)
            {
                if (map.component == component)
                {
                    if (map.component == null)
                    {
                        map.component = component;
                    }
                    /*
                    if (editor)
                    {
                        if (map.serializedObject == null)
                        {
                            map.serializedObject = editor.serializedObject;
                            map.infos = GetSerializedProperties(editor.serializedObject);
                        }
                       
                    }*/
                    return map;
                }
            }
            return SaveFoldoutToMap(component, !allCollapsed, editor);
        }

        public bool AreAllCollapsed()
        {
            if (componentMaps == null)
            {
                componentMaps = new List<ComponentMap>();
            }
            if (componentMaps.Count == 0)
            {

                return false;
            }
            foreach (var map in componentMaps)
            {
                if (map.foldout)
                {

                    return false;
                }
            }

            return true;
        }



        public bool AreAllExpanded()
        {
            if (componentMaps == null)
            {
                componentMaps = new List<ComponentMap>();
            }
            if (componentMaps.Count == 0)
            {
                return false;
            }
            foreach (var map in componentMaps)
            {
                if (!map.foldout)
                {
                    return false;
                }
            }
            return true;
        }
        internal void ResetCulling()
        {
            if (componentMaps == null)
            {
                componentMaps = new List<ComponentMap>();
            }
            foreach (var map in componentMaps)
            {
                map.isCulled = false;
                map.height = -1;
            }
        }

        public void CleanMap(Component[] components)
        {
            if (componentMaps == null)
            {
                componentMaps = new List<ComponentMap>();
                return;
            }
            List<ComponentMap> _componentMaps = new List<ComponentMap>();
            foreach (var map in componentMaps)
            {
                if (components.Contains(map.component))
                {
                    _componentMaps.Add(map);
                }
            }
            componentMaps = _componentMaps;
        }

        private MaterialMapManager _MaterialMapManager
        {
            get
            {
                if (materialMapManager == null)
                {
                    materialMapManager = new MaterialMapManager();
                }
                return materialMapManager;
            }
        }

        public MaterialMap GetMaterialMapForComponent(Component component, Editor editor = null)
        {
            return _MaterialMapManager.GetMaterialMapForComponent(component, editor);
        }

        public void DestroyIfPresent(Component component)
        {
            _MaterialMapManager.DestroyIfPresent(component);
        }

        public void DestroyAllMaterialMaps()
        {
            _MaterialMapManager.DestroyAllMaterialMaps();
        }
        public static void DebugMaterialEditors()
        {
            return;
            /*
           MaterialEditor[] materialEditors = Resources.FindObjectsOfTypeAll<MaterialEditor>();
               
                int numEditors = materialEditors.Length;
                Debug.Log("Material editors " + numEditors);
                for (int i = 0; i < numEditors; i++)
                {
                    Debug.Log(materialEditors[i].target);
                }
               // Debug.Log(CoInspector.instances[^1].asset)
             //   CoInspector.DestroyAllIfNotNull(materialEditors);*/
        }

        public void MarkMaterialsForRebuild()
        {
            _MaterialMapManager.timeToRebuild = true;
        }

        public void RebuildMaterialsIfNecessary()
        {
            _MaterialMapManager.RebuildIfNecessary();
        }


    }
    
}
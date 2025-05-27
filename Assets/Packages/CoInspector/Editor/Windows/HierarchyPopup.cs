using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CoInspector
{

    internal class HierarchyPopup : EditorWindow
    {
        private GameObject selectedGameObject;
        private GUIStyle labelStyle;
        private GUIStyle foldoutStyle;
        private GUIStyle boldLabelStyle;
        private GUIStyle boldFoldoutStyle;
        private Vector2 scrollPosition;
        private Vector2 startPosition;
        private float maxX = 0;
        private float countUntilTarget = 0;
        private bool reachedTarget = false;
        private bool resizedOnStart = false;
        private CoInspectorWindow owner;
        private Dictionary<GameObject, bool> expandedObjects = new Dictionary<GameObject, bool>();
        private Color colorSelected = new Color(0.58f, 0.58f, 0.90f, 0.30f);
        private Color lineColor;
        bool colorGrid = false;
        private float maxWidth = 0;
        private float maxHeight = 0;
        GameObject root;

        internal static void ShowWindow(GameObject gameObject, CoInspectorWindow _owner, Vector2 mousePosition)
        {
            if (gameObject == null)
            {
                return;
            }            
            HierarchyPopup window = GetWindow<HierarchyPopup>(true, "Local Hierarchy");
            window.lineColor = CustomColors.SimpleShadow;
            if (EditorGUIUtility.isProSkin)
            {
                window.lineColor = CustomColors.SimpleBright;
            }
            window.startPosition = new Vector2(_owner.position.x, _owner.position.y);
            window.startPosition.x += mousePosition.x;
            window.startPosition.y +=  mousePosition.y + 80;
            window.maxX = _owner.position.xMax;
            window.position = new Rect(window.startPosition.x, window.startPosition.y, 0, 0); 
            window.labelStyle = new GUIStyle(EditorStyles.label);
            window.foldoutStyle = new GUIStyle(EditorStyles.foldout);
            window.boldLabelStyle = new GUIStyle(CustomGUIStyles.BoldLabel);
            window.boldFoldoutStyle = new GUIStyle(CustomGUIStyles.BoldFoldoutStyle);
            window.selectedGameObject = gameObject;
            window.owner = _owner;
            window.root = window.FindContextualRoot(gameObject);
            window.titleContent = new GUIContent("Local Hierarchy of '" + gameObject.name + "'");
            window.Focus();
            window.ExpandPath(gameObject);

        }


        GameObject FindContextualRoot(GameObject gameObject)
        {
            Transform current = gameObject.transform;
            while (current.parent != null)
            {
                current = current.parent;
            }
            return current.gameObject;
        }

        void OnGUI()
        {
            if (selectedGameObject == null || root == null)
            {
                EditorGUILayout.LabelField("No GameObject selected.");
                return;
            }
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(3);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(3);
            DrawGameObject(root, 0);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
            if (!resizedOnStart)
            {            
                if (reachedTarget)
                {
                    float height = 500;
                    if (maxHeight < height)
                    {
                        height = maxHeight;
                    }
                    if (height > maxHeight)
                    {
                        height = maxHeight;  
                    }
                    Rect newRect = new Rect(startPosition.x - maxWidth/2, startPosition.y, maxWidth, height + 40);
                    if (newRect.xMax > maxX)
                    {
                        newRect.x = maxX - newRect.width;
                    }
                    if (newRect.x < 0)
                    {
                        newRect.x = 0;
                    }
                    this.position = newRect;
                    scrollPosition.y = countUntilTarget;
                    resizedOnStart = true;
                }
            }
        }

        private void OnLostFocus()
        {
            #if UNITY_2022_1_OR_OLDER
               Close();
            #endif
        }

        void DrawGameObject(GameObject obj, int indentLevel, int childIndex = 0)
        {
            if ((obj.hideFlags & HideFlags.HideInHierarchy) != 0)
            {
                return;
            }
            GUILayout.BeginHorizontal(CustomGUIStyles.InspectorButtonStyle, GUILayout.Height(18));
            GUILayout.Space(indentLevel * 20);
            if (indentLevel > 0)
            {
                Rect rect1 = GUILayoutUtility.GetLastRect();
                rect1.width = 20;
                rect1.y += 8;
                rect1.x = indentLevel * 20 - 7;
                EditorUtils.DrawLineUnderRect(rect1, lineColor);
                rect1.width = 1;
                rect1.y -= 18;
                rect1.height = 18;
                EditorGUI.DrawRect(rect1, lineColor);
            }

            Texture2D icon = EditorUtils.GetBestFittingIconForGameObject(obj);
            GUIContent content = new GUIContent(" " + obj.name, icon);
            bool isExpanded = expandedObjects.ContainsKey(obj) && expandedObjects[obj];
            bool drawFoldout = obj.transform.childCount > 0;
            GUIStyle _labelStyle = labelStyle;
            GUIStyle _foldoutStyle = foldoutStyle;

            if (obj == selectedGameObject)
            {
                _labelStyle = boldLabelStyle;
            }

            if (drawFoldout)
            {
                drawFoldout = false;
                foreach (Transform child in obj.transform)
                {
                    if ((child.gameObject.hideFlags & HideFlags.HideInHierarchy) == 0)
                    {
                        drawFoldout = true;
                        break;
                    }
                }
            }
            if (drawFoldout)
            {
                if (obj == selectedGameObject)
                {
                    _foldoutStyle = boldFoldoutStyle;
                }
                _foldoutStyle.margin.left = 4;
                _foldoutStyle.margin.top = 0;
                _foldoutStyle.fixedHeight = 16;
                _foldoutStyle.fixedWidth = 1;
                isExpanded = EditorGUILayout.Foldout(isExpanded, "", false, _foldoutStyle);
                expandedObjects[obj] = isExpanded;
            }

            GUILayout.EndHorizontal();
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.height += 2;
            rect.y -= 2;
            if (obj == selectedGameObject)
            {
                Rect rect1 = new Rect(rect);
                rect1.height = 18;
                rect1.y += 2;
                EditorGUI.DrawRect(rect1, colorSelected);
            }
            if (selectedGameObject.transform.parent == obj.transform)
            {
                reachedTarget = true;
            }
            else if (selectedGameObject.transform.parent == null)
            {
                reachedTarget = true;
            }

            if (!reachedTarget)
            {
                countUntilTarget += 18;
            }

            maxHeight += 18;
            float localWidth = EditorStyles.label.CalcSize(content).x + (indentLevel * 20);
            if (localWidth > maxWidth)
            {
                maxWidth = localWidth;
            }
            if (rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown)
                {

                    Event.current.Use();

                    if (Event.current.button == 0)
                    {
                        owner.SetTargetGameObject(obj);
                        Close();
                    }
                    else if (Event.current.button == 1)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Open in current Tab"), false, () =>

                        {
                            owner.SetTargetGameObject(obj);
                            Close();
                        });
                        menu.AddItem(new GUIContent("Open in new Tab"), false, () =>
                        {
                            owner.AddTabNext();
                            owner.SetTargetGameObject(obj);
                            Close();
                        });
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Select in Hierarchy"), false, () =>
                        {
                            Selection.activeGameObject = obj;
                            Close();
                        }
                        );
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Ping in Hierarchy"), false, () => EditorGUIUtility.PingObject(obj));
                        menu.AddItem(new GUIContent("Focus on Scene View"), false, () => CoInspectorWindow._FocusOnSceneView(obj));

                        menu.ShowAsContext();
                    }
                    else if (Event.current.button == 2)
                    {
                        owner.AddTabNext();
                        owner.SetTargetGameObject(obj);
                        Close();
                    }
                }
            }
            _labelStyle.padding.bottom = 0;
            _labelStyle.padding.top = 0;
            _labelStyle.contentOffset = new Vector2(2, 2);
            _labelStyle.margin = new RectOffset(0, 0, 0, 0);
            _labelStyle.padding.left = 14 + (indentLevel * 20);
            _labelStyle.fixedHeight = 16;
            GUI.Label(rect, content, _labelStyle);
            colorGrid = !colorGrid;

            int childCount = obj.transform.childCount;
            if (isExpanded)
            {

                int lastIndex = obj.transform.childCount - 1;
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    Transform child = obj.transform.GetChild(i);
                    DrawGameObject(child.gameObject, indentLevel + 1, lastIndex - i);
                }
            }
        }

        void ExpandPath(GameObject gameObject)
        {
            Transform current = gameObject.transform;
            while (current != null)
            {
                expandedObjects[current.gameObject] = true;
                current = current.parent;
            }
        }
    }
}
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CoInspector
{
    internal class SettingsWindow : EditorWindow
    {
        Vector2 scrollPos;
        internal UserSaveData userSaveData;
        bool[] foldouts = new bool[0];
        bool showSessions = false;
        private GUIContent documentation;
        static private GUIContent settingsLogo;
        private GUIContent DocumentationContent
        {
            get
            {
                if (documentation == null)
                {
                    Texture2D texture = EditorGUIUtility.IconContent("UnityEditor.ConsoleWindow").image as Texture2D;
                    documentation = new GUIContent("Documentation ", texture);
                }
                return documentation;
            }
        }

        [MenuItem("Window/CoInspector/Open CoInspector Settings")]
        internal static void ShowWindow()
        {
            if (CoInspectorWindow.MainCoInspector)
            {
                CoInspectorWindow.MainCoInspector.SaveSettings();
            }
            var window = GetWindow<SettingsWindow>("CoInspector Settings");
            settingsLogo = new GUIContent(CustomGUIContents.SettingsButtonImage);
            if (!EditorGUIUtility.isProSkin)
            {
                settingsLogo.image = EditorGUIUtility.IconContent("_Popup").image;
            }
            window.titleContent.image = settingsLogo.image;                    
            window.ManageFoldouts();
        }

        void DrawLogo()
        {
            if (EditorGUIUtility.isProSkin)
            {
                EditorGUILayout.Space(5);
            }
            Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(70), GUILayout.Width(position.width));
            rect.x = 0;
            if (!EditorGUIUtility.isProSkin)
            {
                EditorGUI.DrawRect(rect, new Color(0.9f, 0.9f, 0.9f, 1f));
            }
            else
            {
                EditorGUI.DrawRect(rect, new Color(0.8f, 0.8f, 0.8f, 1f));
            }
            _DrawLogo(rect);
             Color shadowColor = CustomColors.GradientShadow;
            if (!EditorGUIUtility.isProSkin)
            {
                EditorUtils.DrawRectBorder(rect, Color.gray);
                shadowColor = CustomColors.SimpleShadow;
            }
           
            if (scrollPos.y > 0)
            {
                rect.y = rect.y + rect.height - 1; 
                rect.height = 15;                  
                EditorUtils.DrawFadeToBottom(rect, shadowColor);
            }

        }
        void _DrawLogo(Rect rect)
        {
            GUI.Label(rect, CustomGUIContents.MainLogoImage, CustomGUIStyles.LogoStyle);
        }

        void ManageFoldouts()
        {
            if (userSaveData == null)
            {
                userSaveData = CoInspectorWindow.FindSettingsObject();
                if (userSaveData == null && CoInspectorWindow.MainCoInspector)
                {
                    userSaveData = CoInspectorWindow.MainCoInspector.AutoCreateSettings();
                }
                if (userSaveData == null)
                {
                   userSaveData = CoInspectorWindow._AutoCreateSettings();
                }

                if (userSaveData == null)
                {
                    EditorUtility.DisplayDialog("CoInspector Settings", "Could not find User Data file. Please make sure you have a folder named 'CoInspector' in your Assets folder.", "OK");
                    Close();
                    return;
                }
            }
            if (userSaveData.sessions == null)
            {
                return;
            }
            if (foldouts.Length != userSaveData.sessions.Count)
            {
                foldouts = new bool[userSaveData.sessions.Count];
            }
        }

        void DrawHeader(string _title)
        {
            _title = " " + _title;
            GUILayout.Space(5);
            EditorGUI.indentLevel = 0;
            GUILayout.Label(_title, CustomGUIStyles.BigBoldLabel);
            GUILayout.Space(5);
            EditorGUI.indentLevel = 1;
        }

        private void OnGUI()
        {
            DrawLogo();
            if (UserSaveData.settingsWindow == null)
            {
                UserSaveData.settingsWindow = this;
            }
            Color defaultColor = GUI.backgroundColor;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUIUtility.labelWidth = 200;
            GUILayout.Space(5);
            DrawHeader("General Settings");
            CoInspectorWindow.assetInspection = EditorGUILayout.Toggle("Enable Asset Inspection", CoInspectorWindow.assetInspection);
            CoInspectorWindow.sessionsMode = EditorGUILayout.Popup("Sessions Behavior", CoInspectorWindow.sessionsMode, new[] { "Always Ask", "Always Restore", "Disable Sessions" });
            CoInspectorWindow.rememberSessions = CoInspectorWindow.sessionsMode != 2;
            EditorGUILayout.Space();
            if (CoInspectorWindow.sessionsMode == 0)
            {
                CustomGUIStyles.HelpBox("You'll be offered to restore your workspace when opening CoInspector or switching scenes.");
            }
            else if (CoInspectorWindow.sessionsMode == 1)
            {
                CustomGUIStyles.HelpBox("CoInspector will automatically load your previous workspaces without asking.");
            }
            else
            {
                CustomGUIStyles.HelpBox("CoInspector will never remember previous Sessions.");

            }
            GUIContent button = CustomGUIContents.GetSavedSessionsButton(showSessions);
            GUI.enabled = userSaveData != null && userSaveData.sessions != null && userSaveData.sessions.Count > 0;
            GUILayout.Space(5);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = CustomColors.NewTabButton;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.backgroundColor = CustomColors.NewTabButton;
            }
            else 
            {
                GUI.backgroundColor *= Color.blue/4.5f;
            }
            if (GUILayout.Button(button, GUILayout.Width(160), GUILayout.Height(24)))
            {
                showSessions = !showSessions;
            }
            CustomGUIContents.DrawCustomButton(true, true);
            GUI.backgroundColor = defaultColor;
            GUILayout.Space(5);            
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            if (showSessions)
            {
                DrawSessions();
            }
            else
            {
                DrawSeparator();
            }

            DrawHeader("Tab Settings");
            CoInspectorWindow.autoFocus = EditorGUILayout.Toggle("Auto-focus Tab target on click?", CoInspectorWindow.autoFocus);
            string[] selectionOptions = { "Do nothing", "Select in Hierarchy" };
            int _selectedIndex = CoInspectorWindow.softSelection ? 0 : 1;
            int newSelectedIndex = EditorGUILayout.Popup("When a Tab is clicked:", _selectedIndex, selectionOptions);

            if (CoInspectorWindow.softSelection)
            {
                EditorGUILayout.Space();
                CustomGUIStyles.HelpBox("Remember that Editor Tools <i>(Move, Rotate…)</i> <b>will still use your active Selection</b>.\n\nDepending on your Unity Version, other Tool Buttons <i>(like Collider Editors)</i> will only show up when the target is selected.");
                GUILayout.Space(10);
            }
            if (newSelectedIndex != _selectedIndex)
            {
                CoInspectorWindow.softSelection = newSelectedIndex == 0;
                SaveSettings();
                Repaint();
            }
            CoInspectorWindow.doubleClickMode = EditorGUILayout.Popup("When a Tab is double-clicked:", CoInspectorWindow.doubleClickMode, new[] { "Lock or Unlock Tab", "Select targets in Hierarchy", "Focus targets on Scene", "Show in Local Hierarchy Pop-up" });
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Tab design", CustomGUIStyles.SettingsBoldLabel);
            GUILayout.Space(3);
            CoInspectorWindow.showHistory = EditorGUILayout.Toggle("Show Tab History buttons", CoInspectorWindow.showHistory);
            CoInspectorWindow.showIcons = EditorGUILayout.Toggle("Show Tab icons", CoInspectorWindow.showIcons);            
            string[] tabInfoOptions = { "Name", "Tree view", "Nothing" };
            int selectedIndex = 0;
            if (CoInspectorWindow.showTabName && CoInspectorWindow.showTabTree)
            {
                selectedIndex = 1;
            }
            else if (!CoInspectorWindow.showTabName && !CoInspectorWindow.showTabTree)
            {
                selectedIndex = 2;
            }
            int _newSelectedIndex = EditorGUILayout.Popup("Info on Tab hover:", selectedIndex, tabInfoOptions);
            if (_newSelectedIndex != selectedIndex)
            {
                switch (_newSelectedIndex)
                {
                    case 0:
                        CoInspectorWindow.showTabName = true;
                        CoInspectorWindow.showTabTree = false;
                        break;
                    case 1:
                        CoInspectorWindow.showTabName = true;
                        CoInspectorWindow.showTabTree = true;
                        break;
                    case 2:
                        CoInspectorWindow.showTabName = false;
                        CoInspectorWindow.showTabTree = false;
                        break;
                }
            }
            CoInspectorWindow.tabCompactMode = EditorGUILayout.IntPopup("Tab size:", CoInspectorWindow.tabCompactMode, new[] { "Compact", "Normal"}, new[] { 1, 2 });
            CoInspectorWindow.showScrollBar = EditorGUILayout.Toggle("Show Tab scrollbar", CoInspectorWindow.showScrollBar);
            CoInspectorWindow.showCollapseTool = EditorGUILayout.Toggle("Show Collapse button", CoInspectorWindow.showCollapseTool);
            CoInspectorWindow.showFilterBar = EditorGUILayout.Toggle("Show Component Filter bar", CoInspectorWindow.showFilterBar);
            if (!CoInspectorWindow.showFilterBar && CoInspectorWindow.MainCoInspector)
            {
                CoInspectorWindow.MainCoInspector.filteringComponents = false;
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Mouse wheel scroll", CustomGUIStyles.SettingsBoldLabel);
            GUILayout.Space(3);
            CoInspectorWindow.scrollSpeedY = EditorGUILayout.IntPopup("Vertical wheel speed", CoInspectorWindow.scrollSpeedY, new[] { "Normal", "Fast", "Fastest" }, new[] { 2, 3, 4 });
            CoInspectorWindow.scrollSpeedX = EditorGUILayout.IntPopup("Horizontal wheel speed", CoInspectorWindow.scrollSpeedX, new[] { "Normal", "Fast", "Fastest" }, new[] { 2, 3, 4 });
            EditorGUILayout.Space();
            CoInspectorWindow.scrollDirectionY = EditorGUILayout.Toggle("Invert vertical scroll", CoInspectorWindow.scrollDirectionY == -1) ? -1 : 1;
            CoInspectorWindow.scrollDirectionX = EditorGUILayout.Toggle("Invert horizontal scroll", CoInspectorWindow.scrollDirectionX == -1) ? -1 : 1;
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Experimental", CustomGUIStyles.SettingsBoldLabel);
            CoInspectorWindow.componentCulling = EditorGUILayout.Toggle("Component Culling", CoInspectorWindow.componentCulling);
            GUILayout.Space(3);
            CustomGUIStyles.HelpBox(
               "Improves performance <b>drastically</b>, but it's still under testing.\n\n" +
                "<i>(Basically… leave this enabled unless you encounter something weird.)</i>"
            );
            DrawSeparator();
            GUI.enabled = CoInspectorWindow.assetInspection;
            DrawHeader("Asset Settings");
            CoInspectorWindow.ignoreFolders = EditorGUILayout.Toggle("Ignore Folder Inspection", CoInspectorWindow.ignoreFolders);
            if (CoInspectorWindow.ignoreFolders)
            {
                EditorGUILayout.Space();
                CustomGUIStyles.HelpBox("Individual folders will not count as Assets.");
                EditorGUILayout.Space();
            }
            CoInspectorWindow.collapsePrefabComponents = EditorGUILayout.Toggle("Collapsed Prefab View", CoInspectorWindow.collapsePrefabComponents);
            CoInspectorWindow.showAssetLabels = EditorGUILayout.Toggle("Show AssetBundle Footer", CoInspectorWindow.showAssetLabels);
            GUI.enabled = true;
            if (GUI.changed)
            {
                SaveSettings();
            }
            GUILayout.Space(10);
            EditorGUILayout.EndScrollView();
            DrawSeparator(true);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.backgroundColor = CustomColors.NewTabButton;
            if (EditorGUIUtility.isProSkin)
            {
                GUI.backgroundColor = CustomColors.NewTabButton;
            }
            else
            {
                GUI.backgroundColor *= Color.blue / 4.5f;
            }
            if (GUILayout.Button("Tutorial", GUILayout.Width(70), GUILayout.Height(25)))
            {
               if (EditorUtility.DisplayDialog("CoInspector Tutorial", "Are you sure you want to start the Tutorial?\n\nYour current Tabs and Asset History will be lost.", "Yes", "No"))
                {                    
                    FirstInstallWindow.ShowWindow();
                }
            }
            CustomGUIContents.DrawCustomButton();
            GUILayout.Space(2);
            if (GUILayout.Button(DocumentationContent, GUILayout.Width(120), GUILayout.Height(25)))
            {
                UnityEngine.Object obj = CustomGUIContents.LoadCustomAsset("/Documentation/Manual.pdf", true);
                if (obj != null)
                {
                    CoInspectorWindow.OpenAsset(obj);
                }
            }
            CustomGUIContents.DrawCustomButton();
            GUILayout.Space(2);
            GUI.backgroundColor = CustomColors.ResetToDefault;
            if (!EditorGUIUtility.isProSkin)
            {                                       
                GUI.backgroundColor *= Color.red / 3.5f;
            }           
            if (GUILayout.Button(CustomGUIContents.ResetToDefaultContent, GUILayout.Width(124), GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Reset to Default", "Are you sure you want to reset all settings to default?", "Yes", "No"))
                {
                    CoInspectorWindow.ResetToDefault();
                    if (EditorUtility.DisplayDialog("Done!", "Do you want to also wipe your existing Sessions?\n\n(It will not affect your current workspace)", "Yes", "No"))
                    {
                        userSaveData.sessions.Clear();
                        if (CoInspectorWindow.MainCoInspector)
                        {
                            CoInspectorWindow.MainCoInspector.ResetAssetViewSize();
                        }
                    }
                    SaveSettings(true);
                }
            }
            CustomGUIContents.DrawCustomButton();
            GUI.backgroundColor = defaultColor;
            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        void DrawSeparator(bool skipPadding = false)
        {
            if (!skipPadding)
            {
                GUILayout.Space(8);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.EndHorizontal();
            Rect rect = GUILayoutUtility.GetLastRect();
            rect.width = position.width;
            rect.x = 0;
            EditorUtils.DrawLineUnderRect(rect, CustomColors.SimpleBright);
            EditorUtils.DrawLineUnderRect(rect,CustomColors.HardShadow, -1);            
            EditorGUILayout.Space();
            
        }

        private void SaveSettings(bool toDisk = false)
        {
            if (userSaveData != null)
            {
                userSaveData.SaveData(toDisk);
                if (CoInspectorWindow.MainCoInspector)
                {
                    CoInspectorWindow.MainCoInspector.UpdateAllWidths();
                }
            }
        }

        void OnDestroy()
        {
            if (userSaveData != null)
            {
                userSaveData.SaveData(true);
            }
        }

        bool AreAllInvalid(List<TabInfo> tabs)
        {
            if (tabs == null)
            {
                return true;
            }
            return tabs.All(t => t == null);
        }

        void DrawSessions()
        {
            if (userSaveData == null)
            {
                return;
            }

            if (userSaveData.sessions == null)
            {
                return;
            }
            if (userSaveData.sessions.Count == 0)
            {
                return;
            }
            ManageFoldouts();
            GUIStyle labelStyle = CustomGUIStyles.SettingsLabel;
            GUIStyle subLabelStyle = CustomGUIStyles.SettingsSubLabel;
            GUIStyle foldoutStyle = CustomGUIStyles.JustFoldoutStyle;
            GUILayout.Space(8);
            var orderedSessions = userSaveData.sessions.OrderByDescending(s => s.GetSaveTime()).ToList();
            for (int i = 0; i < orderedSessions.Count; i++)
            {
                TabSession session = orderedSessions[i];
                if (session == null || session.tabs == null || session.tabs.Count == 0 || AreAllInvalid(session.tabs))
                {
                    userSaveData.sessions.Remove(session);
                }

                if (session.tabs != null && session.tabs.Count > 0)
                {
                    string date = session.GetSaveTime().ToString(CultureInfo.InvariantCulture);
                    string fullName = "<b>" + session.sceneName + "</b>   (" + session.tabs.Count + " Tabs)";
                    Color color = GUI.backgroundColor;
                   
                    if (EditorGUIUtility.isProSkin)
                    {
                        GUI.backgroundColor += CustomColors.AssetBarBackColor;
                    }
                   
                    EditorGUILayout.BeginHorizontal(CustomGUIStyles.InspectorSectionStyle, GUILayout.Height(23));
                    EditorGUILayout.BeginVertical(CustomGUIStyles.InspectorButtonStyle, GUILayout.Height(22));
                    GUIContent placeholderContent = CustomGUIContents.EmptyContent;
                    placeholderContent.text = fullName;
                    foldouts[i] = EditorGUILayout.Foldout(foldouts[i], "", true, foldoutStyle);
                    Rect labelRect = GUILayoutUtility.GetLastRect();
                    labelRect.width = labelStyle.CalcSize(placeholderContent).x;
                    labelRect.x += 30;
                    labelRect.y -= 1;
                    GUI.Label(labelRect, fullName, labelStyle);
                    labelStyle.fontSize = 11;
                    placeholderContent.text = session.lastSaveTimePrint;
                    float dateLabelSize = labelStyle.CalcSize(placeholderContent).x;
                    labelStyle.fontSize = 12;
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();
                     if (EditorGUIUtility.isProSkin)
                    {
                    GUI.backgroundColor += Color.red * 1.5f;
                    }
                    else 
                    {
                        GUI.backgroundColor = Color.red/1.5f;
                    }
                    GUIContent deleteContent = CustomGUIContents.DeleteContent;
                    if (GUILayout.Button(deleteContent, CustomGUIStyles.InspectorButtonStyle, GUILayout.Width(23), GUILayout.Height(22)))
                    {
                        if (EditorUtility.DisplayDialog("Delete Session", "Are you sure you want to delete this session?", "I'm sure to be sure", "No"))
                        {
                            userSaveData.sessions.Remove(session);
                        }
                    }
                    GUI.backgroundColor = color;
                    Rect rect1 = GUILayoutUtility.GetLastRect();                    
                    rect1.width = position.width;
                    float lastRectX = rect1.x;
                    Rect dateRect = new Rect(lastRectX - 25 - dateLabelSize, labelRect.y, dateLabelSize + 25, labelRect.height);
                    if (!(dateRect.x < labelRect.x + labelRect.width))
                    {
                        GUI.Label(dateRect, "<i>" + date + "</i>", labelStyle);
                    }
                    EditorGUILayout.EndHorizontal();
                    rect1 = GUILayoutUtility.GetLastRect();
                    rect1.width -= 25;
                    if (rect1.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp && Event.current.button == 0)
                    {
                        foldouts[i] = !foldouts[i];
                        Repaint();
                    }
                    rect1.width = position.width;
                    rect1.x = 0;                    
                    EditorUtils.DrawLineOverRect(rect1, CustomColors.HarderBright);
                    if (i == userSaveData.sessions.Count - 1 || foldouts[i])
                    {
                        if (foldouts[i])
                        {
                            EditorUtils.DrawLineUnderRect(rect1, CustomColors.HardShadow, -1);
                            EditorUtils.DrawLineUnderRect(rect1, CustomColors.SoftShadow, 0, 2);
                            EditorUtils.DrawLineUnderRect(rect1, CustomColors.VerySoftShadow, 0, 5);
                        }
                    }
                    if (foldouts[i])
                    {
                        EditorGUILayout.BeginVertical();
                        EditorGUI.indentLevel++;
                        bool dark = false;
                        for (int j = 0; j < session.tabs.Count; j++)
                        {
                            EditorGUILayout.BeginHorizontal();                           
                            
                            TabInfo tab = session.tabs[j];
                            string tabName = tab.name;
                            if (tab.newTab)
                            {
                                tabName = "<i>(New Tab)</i>";
                            }
                            EditorGUILayout.LabelField(j + 1 + ".  " + tabName, subLabelStyle);
                            EditorGUILayout.EndHorizontal();
                            if (dark)
                            {                               
                                EditorUtils.DrawLineOverRect(CustomColors.SoftShadow, 2, 20);
                            }                            
                             dark = !dark;                           
                        }
                        EditorGUI.indentLevel--;
                        GUILayout.Space(2);
                        EditorGUILayout.EndVertical();
                        GUI.backgroundColor = color;
                        Rect rect = GUILayoutUtility.GetLastRect();
                        rect.width = position.width;
                        rect.x = 0;
                        EditorGUI.DrawRect(rect, CustomColors.SubtleBlue);
                    }

                    if (i == userSaveData.sessions.Count - 1 && foldouts[i])
                    {
                        rect1 = GUILayoutUtility.GetLastRect();
                        rect1.width = position.width;
                        rect1.x = 0;
                        EditorUtils.DrawLineUnderRect(rect1, CustomColors.SimpleBright, 2);
                        EditorUtils.DrawLineUnderRect(rect1, CustomColors.HardShadow, 1);
                    }
                }
            }
        }
    }
}
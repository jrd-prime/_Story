#if UNITY_EDITOR
using UnityEditor;

public class ToggleInspectorLock : UnityEditor.Editor
{
    [MenuItem("GameTools/Editor Hotkeys/Toggle Inspector Lock #e")]
    public static void ToggleLock() =>
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
}

#endif
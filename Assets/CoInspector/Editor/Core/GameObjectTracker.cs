using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CoInspector
{
    [Serializable]
    internal class GameObjectTracker
    {
        [Serializable]
        private class GameObjectEntry
        {
            [SerializeField] private List<string> paths;
            [SerializeField] private int count;

            public GameObjectEntry(List<string> paths, int count)
            {
                this.paths = new List<string>(paths);
                this.count = count;
            }

            public List<string> Paths => paths;
            public int Count => count;

            public void IncrementCount()
            {
                count++;
            }
        }
        [SerializeField] private List<GameObjectEntry> mostClicked = new List<GameObjectEntry>();
        [SerializeField] private List<GameObjectEntry> recentlyClicked = new List<GameObjectEntry>();
        [SerializeField] private List<GUIContent> mostClickedContents = new List<GUIContent>();
        [SerializeField] private List<GUIContent> recentlyClickedContents = new List<GUIContent>();

        public GameObjectTracker()
        {

        }

        public GameObjectTracker(GameObjectTracker other)
        {
            if (other == null)
            {
                return;
            }
            mostClicked = other.mostClicked.Select(entry => new GameObjectEntry(entry.Paths, entry.Count)).ToList();
            recentlyClicked = other.recentlyClicked.Select(entry => new GameObjectEntry(entry.Paths, entry.Count)).ToList();
        }

        public void UpdateClicked(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            UpdateClicked(new List<GameObject> { gameObject });

        }

        public void UpdateClicked(GameObject[] gameObjects)
        {
            if (gameObjects == null)
            {
                return;
            }
            UpdateClicked(gameObjects.ToList());

        }

        public void UpdateClicked(List<GameObject> gameObjects)
        {
            List<string> paths = gameObjects.Select(go => GameObjectToPath(go)).ToList();
            UpdateMostClicked(paths);
            UpdateRecentlyClicked(paths);
            UpdateContents();
            if (CoInspectorWindow.MainCoInspector && !CoInspectorWindow.MainCoInspector.exitingPlayMode)
            {
                CoInspectorWindow.MainCoInspector.SaveSettings();
            }
        }

        private void UpdateMostClicked(List<string> paths)
        {
            GameObjectEntry entry = mostClicked.Find(e => e.Paths.SequenceEqual(paths));
            if (entry != null)
            {
                entry.IncrementCount();
                mostClicked.Sort((a, b) => b.Count.CompareTo(a.Count));
            }
            else
            {
                if (mostClicked.Count >= 10)
                {
                    mostClicked.RemoveAt(mostClicked.Count - 1);
                }
                mostClicked.Add(new GameObjectEntry(paths, 1));
            }

        }

        private void UpdateRecentlyClicked(List<string> paths)
        {
            GameObjectEntry entry = recentlyClicked.Find(e => e.Paths.SequenceEqual(paths));
            if (entry != null)
            {
                recentlyClicked.Remove(entry);
            }
            else if (recentlyClicked.Count >= 10)
            {
                recentlyClicked.RemoveAt(recentlyClicked.Count - 1);
            }
            recentlyClicked.Insert(0, new GameObjectEntry(paths, 1));


        }

        public List<List<GameObject>> GetMostClicked()
        {
            return GetGameObjectsList(mostClicked);
        }

        public List<List<GameObject>> GetRecentlyClicked()
        {
            return GetGameObjectsList(recentlyClicked);
        }

        public void RemoveFromMost(List<GameObject> gameObjects)
        {
            List<string> paths = gameObjects.Select(go => GameObjectToPath(go)).ToList();
            GameObjectEntry entry = mostClicked.Find(e => e.Paths.SequenceEqual(paths));
            if (entry != null)
            {
                mostClicked.Remove(entry);
            }
            UpdateContents();
        }

        public void RemoveFromLast(List<GameObject> gameObjects)
        {
            List<string> paths = gameObjects.Select(go => GameObjectToPath(go)).ToList();
            GameObjectEntry entry = recentlyClicked.Find(e => e.Paths.SequenceEqual(paths));
            if (entry != null)
            {
                recentlyClicked.Remove(entry);
            }
            UpdateContents();
        }

        private List<List<GameObject>> GetGameObjectsList(List<GameObjectEntry> list)
        {
            List<List<GameObject>> gameObjectsList = new List<List<GameObject>>();
            foreach (GameObjectEntry entry in list)
            {
                List<GameObject> gameObjects = entry.Paths.Select(path => PathToGameObject(path)).Where(go => go != null).ToList();
                if (gameObjects.Count > 0)
                {
                    gameObjectsList.Add(gameObjects);
                }
            }
            return gameObjectsList;
        }
        private string GameObjectToPath(GameObject gameObject)
        {
            string path = EditorUtils.GatherGameObjectPath(gameObject);
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }
            return string.Empty;
        }
        private GameObject PathToGameObject(string path)
        {
            return EditorUtils.LoadGameObject(path);
        }
        internal void UpdateContents()
        {
            if (mostClickedContents == null)
            {
                mostClickedContents = new List<GUIContent>();
            }
            mostClickedContents.Clear();
            foreach (GameObjectEntry entry in mostClicked)
            {
                List<GameObject> gameObjects = entry.Paths.Select(path => PathToGameObject(path)).Where(go => go != null).ToList();
                if (gameObjects.Count > 0)
                {
                    GUIContent content;
                    if (gameObjects.Count > 1)
                    {
                        content = new GUIContent(CustomGUIContents.MultiTabButtonImage);
                        content.tooltip = string.Join("\n", gameObjects.ConvertAll(go => gameObjects.IndexOf(go) + 1 + ". " + go.name));
                    }
                    else
                    {
                        content = new GUIContent(EditorUtils.GetBestFittingIconForGameObject(gameObjects[0]));

                        content.tooltip = "Click: Select\nMiddle-Click: Open in new Tab\nRight-Click: More Options";
                    }
                    mostClickedContents.Add(content);
                }
            }
            if (recentlyClickedContents == null)
            {
                recentlyClickedContents = new List<GUIContent>();
            }
            recentlyClickedContents.Clear();
            foreach (GameObjectEntry entry in recentlyClicked)
            {
                List<GameObject> gameObjects = entry.Paths.Select(path => PathToGameObject(path)).Where(go => go != null).ToList();
                if (gameObjects.Count > 0)
                {
                    GUIContent content;
                    if (gameObjects.Count > 1)
                    {
                        content = new GUIContent(CustomGUIContents.MultiTabButtonImage);
                        content.tooltip = string.Join("\n", gameObjects.ConvertAll(go => gameObjects.IndexOf(go) + 1 + ". " + go.name));
                    }
                    else
                    {
                        content = new GUIContent(EditorUtils.GetBestFittingIconForGameObject(gameObjects[0]));
                        content.tooltip = "Click: Select\nMiddle Click: Open in a New Tab\nRight Click: More Options";
                    }
                    recentlyClickedContents.Add(content);
                }
            }
        }

        public GUIContent GetContentForMost(int index)
        {

            if (mostClickedContents == null)
            {

                mostClickedContents = new List<GUIContent>();
                UpdateContents();
            }
            if (index >= 0 && index < mostClickedContents.Count)
            {

                return mostClickedContents[index];
            }
            return null;
        }

        public GUIContent GetContentForLast(int index)
        {
            if (recentlyClickedContents == null)
            {
                recentlyClickedContents = new List<GUIContent>();
                UpdateContents();
            }
            if (index >= 0 && index < recentlyClickedContents.Count)
            {
                return recentlyClickedContents[index];
            }
            return null;
        }
    }
}
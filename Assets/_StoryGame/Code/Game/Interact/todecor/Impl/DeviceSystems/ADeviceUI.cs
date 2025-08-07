using UnityEngine;
using UnityEngine.UIElements;

namespace _StoryGame.Game.Interact.todecor.Impl.DeviceSystems
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class ADeviceUI : MonoBehaviour
    {
        private void Awake()
        {
            Debug.LogWarning("Awake called for " + gameObject.name + " " + GetType().Name);
        }

        public void ShowPanel()
        {
            
        }
        
        public void HidePanel()
        {
            
        }
    }
}

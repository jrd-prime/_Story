using Unity.Mathematics;
using UnityEngine;

namespace _StoryGame.Game.Extensions
{
    public static class ScreenHelper
    {
        public static float2 GetSafeZoneOffset(float targetWidth, float targetHeight)
        {
            Rect safeArea = Screen.safeArea;

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            float scaleX = screenWidth / targetWidth;
            float scaleY = screenHeight / targetHeight;
            float scale = Mathf.Min(scaleX, scaleY);

            float offsetX = safeArea.xMin / scale;
            float offsetY = safeArea.yMin / scale;

            return new float2(offsetX, offsetY);
        }
    }
}

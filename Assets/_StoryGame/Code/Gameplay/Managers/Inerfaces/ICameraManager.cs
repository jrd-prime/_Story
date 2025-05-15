using _StoryGame.Core.Character.Common.Interfaces;
using UnityEngine;

namespace _StoryGame.Gameplay.Managers.Inerfaces
{
    public interface ICameraManager
    {
        void SetTarget(IFollowable target);
        void RemoveTarget();
        Camera GetMainCamera();
        Vector3 GetCamEulerAngles();
        Quaternion GetCamRotation();
    }
}

using UnityEditor.Animations;

namespace _StoryGame.Data.Anim
{
    using UnityEngine;

    public static class AnimatorHelper
    {
        /// <summary>
        /// Пытается найти длину клипа, назначенного состоянию в Animator Controller.
        /// Этот метод предполагает, что состояние содержит только один клип.
        /// </summary>
        /// <param name="animator">Компонент Animator.</param>
        /// <param name="stateName">Имя состояния (string).</param>
        /// <param name="layerIndex">Индекс слоя (по умолчанию 0).</param>
        /// <returns>Длительность клипа состояния, или 0f, если не найдено.</returns>
        public static float GetClipLengthFromStateName(Animator animator, string stateName, int layerIndex = 0)
        {
            if (animator == null || animator.runtimeAnimatorController == null)
            {
                Debug.LogError("Animator or RuntimeAnimatorController is null.");
                return 0f;
            }

            // Получаем RuntimeAnimatorController (который может быть обычным Controller или Override Controller)
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;

            // Если это Animator Override Controller, нужно получить его Base Controller
            if (controller == null)
            {
                AnimatorOverrideController overrideController =
                    animator.runtimeAnimatorController as AnimatorOverrideController;
                if (overrideController != null)
                {
                    controller = overrideController.runtimeAnimatorController as AnimatorController;
                    if (controller == null)
                    {
                        Debug.LogError("Base AnimatorController not found in AnimatorOverrideController.");
                        return 0f;
                    }
                }
                else
                {
                    Debug.LogError(
                        "Animator Controller is not a standard AnimatorController or AnimatorOverrideController.");
                    return 0f;
                }
            }

            // Находим слой
            if (layerIndex < 0 || layerIndex >= controller.layers.Length)
            {
                Debug.LogError($"Layer index {layerIndex} is out of bounds for Animator Controller.");
                return 0f;
            }

            AnimatorControllerLayer layer = controller.layers[layerIndex];
            ChildAnimatorState[] states = layer.stateMachine.states;

            foreach (ChildAnimatorState state in states)
            {
                if (state.state.name == stateName)
                {
                    // Если состояние является BlendTree, это сложнее, т.к. там много клипов
                    if (state.state.motion is BlendTree)
                    {
                        Debug.LogWarning(
                            $"State '{stateName}' is a BlendTree. Cannot get single clip length directly.");
                        // Для BlendTree вам придется анализировать его motions, что сложнее
                        return 0f;
                    }
                    else if (state.state.motion is AnimationClip clip)
                    {
                        // Если это просто AnimationClip
                        return clip.length;
                    }
                }
            }

            Debug.LogWarning($"State '{stateName}' not found in layer {layerIndex} or has no simple AnimationClip.");
            return 0f;
        }
    }
}

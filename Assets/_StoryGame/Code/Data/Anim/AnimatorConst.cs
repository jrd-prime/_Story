using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace _StoryGame.Data.Anim
{
    [SuppressMessage("Domain reload", "UDR0001:Domain Reload Analyzer")]
    public static class AnimatorConst
    {
        public const string GatherHighTrigger = "gather_high_trigger";
        public const string GatherMidTrigger = "gather_mid_trigger";
        public const string GatherLowTrigger = "gather_low_trigger";
        public const string IsGatherHigh = "is_gather_high";

        // Switchable trigger
        public static readonly int TurnOff = Animator.StringToHash("TurnOff");
        public static readonly int TurnOn = Animator.StringToHash("TurnOn");
        public const string OffStateName = "OFFState";
        public const string OnStateName = "ONState";
        
        // MC movement
        public static readonly int IsBrakingParam = Animator.StringToHash("IsBraking");
        public const string BrakingState = "Braking";
        public const string VelocityParam = "_velocity";
    }
}

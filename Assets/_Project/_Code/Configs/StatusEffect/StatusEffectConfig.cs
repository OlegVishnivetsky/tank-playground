using Sirenix.OdinInspector;
using TankPlayground.Gameplay;
using UnityEngine;

namespace TankPlayground.Configs
{
    [CreateAssetMenu(fileName = "StatusEffect Config", menuName = "Configs/Gameplay/StatusEffect Config")]
    public class StatusEffectConfig : ScriptableObject
    {
        [BoxGroup("Meta")]
        public string Name;
        [BoxGroup("Meta")]
        public string Description;
        [BoxGroup("Meta")]
        public Sprite Icon;
        
        [Space(10f)]
        [BoxGroup("Parameters")]
        public StackingBehaviourType Stacking;
        [BoxGroup("Parameters")]
        public float Duration = -1f;
        [BoxGroup("Parameters")]
        [ShowIf("@Stacking == StackingBehaviourType.Stack || Stacking == StackingBehaviourType.StackDuration")]
        public int MaxStacks = 1;
        
        [BoxGroup("Behaviour")]
        [SerializeReference]
        public IStatusEffectBehaviour Behaviour;
    }
}
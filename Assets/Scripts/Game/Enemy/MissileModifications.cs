using System.Collections.Generic;
using Game.ModificationSystem;
using UnityEngine;

namespace Game.Enemy
{
    public interface IAllowAttackFriendlyModification
    {
        GameObject Player { get; set; }
        GameObject Target { get; set; }
        IReadOnlyList<GameObject> Friendlies { get; set; }
    }

    public class AttackFriendlyModification : IModification<IAllowAttackFriendlyModification>
    {
        public void ApplyOn(IAllowAttackFriendlyModification obj)
        {
            var target = obj.Friendlies.SelectRandom();
            if (target != null) obj.Target = target;
        }

        public void ResetOn(IAllowAttackFriendlyModification obj)
        {
            obj.Target = obj.Player;
        }
    }

    public interface IAllowSelfDestructModification
    {
        void SelfDestruct();
    }

    public class SelfDestructModification : IModification<IAllowSelfDestructModification>
    {
        public void ApplyOn(IAllowSelfDestructModification obj)
        {
            obj.SelfDestruct();
        }

        public void ResetOn(IAllowSelfDestructModification obj)
        {
        }
    }
}
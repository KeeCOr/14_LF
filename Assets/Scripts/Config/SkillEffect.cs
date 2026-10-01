using System;
namespace SlotDefense
{
    public enum SkillType { LightningArrow, PortalBomb }

    [Serializable]
    public struct SkillEffect
    {
        public SkillType type;
        public float damage;
        public float radius;
    }
}

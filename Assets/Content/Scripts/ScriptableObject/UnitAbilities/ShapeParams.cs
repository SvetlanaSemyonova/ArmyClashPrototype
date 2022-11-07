using UnityEngine;

namespace Content.Scripts.ScriptableObject.UnitAbilities
{
    [CreateAssetMenu(fileName = "Unit Shape SO", menuName = "Unit Shape SO", order = 54)]
    public class ShapeParams : UnityEngine.ScriptableObject
    {
        public UnitShape unitShape;
        public PrimitiveType type;
        public double attackPoints;
        public double healthPoints;
        public double movementSpeed;
        public double attackSpeed;
    }

    public enum UnitShape
    {
        CUBE,
        SPHERE
    }
}
using Content.Scripts.ScriptableObject.UnitAbilities;
using Content.Scripts.UI;
using UnityEngine;
using UnityEngine.Assertions;

namespace Content.Scripts.Units
{
    public class Fighter : Unit
    {
        private float unitRangeDistance = 2f;
        
        [SerializeField] private ObjectPool unitHealthFXPool;

        private void Awake()
        {
            Assert.IsNotNull(unitHealthFXPool);
            
            unitHealthFXPool.Initialize();
        }

        private Unit FindClosestEnemy()
        {
            Unit closestUnit = null;
            var minDist = Mathf.Infinity;

            foreach (var unit in enemies)
            {
                if (!unit.isActiveAndEnabled) continue;
                var dist = Vector3.Distance(unit.transform.position, transform.position);
                if (dist < minDist)
                {
                    closestUnit = unit;
                    minDist = dist;
                }
            }

            return closestUnit;
        }

        private Unit FindEnemy()
        {
            if (IsFigherDistance()) return currentEnemy;
            Unit poorUnit = null;
            var minHealth = double.MaxValue;
            
            foreach (var unit in enemies)
            {
                if (!unit.isActiveAndEnabled) continue;
                var health = unit.HealthPoints;
                
                if (health < minHealth)
                {
                    poorUnit = unit;
                    minHealth = health;
                }
            }

            return poorUnit;
        }

        public override void GetDamage(double damage)
        {
            ChangeHealth(-damage);
            ShowHealthChange(damage);
            
            currentEnemy = FindClosestEnemy();
            if (IsDied) Death();
        }

        private void ShowHealthChange(double damage)
        {
            var obj = unitHealthFXPool.GetObject();
            var health = obj.GetComponent<HealthView>();
            
            health.SetText($"-{(int)damage}");
            health.OnFinish += ReturnHealthViewObject;
        }

        private void ReturnHealthViewObject(HealthView hp)
        {
            unitHealthFXPool.ReturnObject(hp.gameObject);
            hp.OnFinish -= ReturnHealthViewObject;
        }

        protected override void MoveToEnemy()
        {
            currentEnemy = Type == UnitShape.CUBE ? FindClosestEnemy() : FindEnemy();
            if (currentEnemy == null || isAttack || IsFigherDistance()) return;
            var step = MovementSpeed / 2 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentEnemy.transform.position, (float)step);
        }

        private bool IsFigherDistance()
        {
            if (!currentEnemy) return false;
            var dist = Vector3.Distance(currentEnemy.transform.position, transform.position);
            return !(dist > unitRangeDistance);
        }

        public override void Attack()
        {
            if (!IsFigherDistance()) return;
            currentEnemy.GetDamage(AttackPoints);
        }

        private new void Death()
        {
            CancelInvoke($"Attack");
            base.PerformDeath();
        }
    }
}
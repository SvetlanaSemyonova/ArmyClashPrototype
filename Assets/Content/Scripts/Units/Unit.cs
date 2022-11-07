using System;
using System.Collections;
using System.Collections.Generic;
using Content.Scripts.Managers;
using Content.Scripts.ScriptableObject.UnitAbilities;
using Project.Scripts.ScriptableObject.UnitAbilities;
using UnityEngine;

namespace Content.Scripts.Units
{
    public abstract class Unit: MonoBehaviour
    {
        [Header("Unit Characteristics")]
        [SerializeField] private double attackPoints;
        [SerializeField] private double healthPoints;
        [SerializeField] private double movementSpeed;
        [SerializeField] private double attackSpeed;
        protected double AttackPoints => attackPoints;
        public double HealthPoints => healthPoints;
        protected double MovementSpeed => movementSpeed;

        [Space]
        private MeshRenderer fighterMeshRenderer;
        private GameObject shape;
        private Rigidbody _rigidbody;
        protected UnitShape Type { get; private set; }

        [Space]
        public Unit[] enemies;
        public bool isAttack;
        public bool isInitialized;
        protected bool IsDied => healthPoints <= 0;
        public Unit currentEnemy;
        public string enemyTag;

        public event Action<GameObject> OnDeath;

        private void ChangeDamage(double attack) => attackPoints += attack;
        
        internal void ChangeHealth(double health) => healthPoints += health;

        private void ChangeSpeed(double speed) => movementSpeed += speed;

        private void ChangeAttackSpeed(double attackSpeed) => this.attackSpeed += attackSpeed;

        private void OnEnable()
        {
            StartCoroutine(StartAttack());
        }

        internal void Init(ShapeParams shape, ColorParams color , SizeParams size)
        {
            SetBaseCharacteristics();
            if (this.shape) Destroy(this.shape);
            if (fighterMeshRenderer) fighterMeshRenderer = null;
            
            this.shape = GameObject.CreatePrimitive(shape.type);
            this.shape.transform.SetParent(gameObject.transform);
            this.shape.transform.localPosition= new Vector3(0,0,0);
            fighterMeshRenderer = this.shape.gameObject.GetComponent<MeshRenderer>();
            fighterMeshRenderer.material = color.coloredMaterial;
            Type = shape.unitShape;
            if (!_rigidbody) _rigidbody = GetComponent<Rigidbody>();

            gameObject.transform.localScale = size.unitScale;
            ChangeDamage(size.attackPoints + shape.attackPoints + color.attackPoints);
            ChangeHealth(size.healthPoints + shape.healthPoints + color.healthPoints);
            ChangeSpeed(size.movementSpeed + shape.movementSpeed + color.movementSpeed);
            ChangeAttackSpeed(size.attackSpeed + shape.attackSpeed + color.attackSpeed);

            isInitialized = true;
        }

        IEnumerator StartAttack()
        {
            yield return new WaitUntil(() => GameManager.IsGameStarted);
            
            InvokeRepeating("Attack", (float)attackSpeed, (float)attackSpeed);
        }

        void FixedUpdate()
        {
            if (!isInitialized || !GameManager.IsGameStarted) return;
            MoveToEnemy();
            _rigidbody.velocity = Vector3.zero;
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);//freeze rotation manually
        }

        private void OnDisable()
        {
            fighterMeshRenderer = null;
            isInitialized = false;
            Destroy(shape);
        }

        public void SetEnemyView(Material mat)
        {
            fighterMeshRenderer.material = mat;
        }

        public void SetBaseCharacteristics()
        {
            attackPoints = Constants.AttackValue;
            healthPoints = Constants.StartHealthValue;
            movementSpeed = Constants.StartMovementSpeed;
            attackSpeed = Constants.StartAttackSpeed;
        }
        
        public void SetEnemies(List<Unit> enemies, string enemyTag)
        {
            this.enemyTag = enemyTag;
            this.enemies = new Unit[enemies.Count];
            for (var i = 0; i < enemies.Count; i++)
                this.enemies[i] = enemies[i];
        }

        protected void PerformDeath() => OnDeath?.Invoke(gameObject);
        
        protected abstract void MoveToEnemy();
        
        public abstract void GetDamage(double damage);
        
        public abstract void Attack();
        
    }
}

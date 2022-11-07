using System.Collections.Generic;
using Content.Scripts.ScriptableObject.UnitAbilities;
using Content.Scripts.Units;
using Project.Scripts;
using Project.Scripts.ScriptableObject.UnitAbilities;
using UnityEngine;
using UnityEngine.Assertions;

namespace Content.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private ObjectPool userPool;
        [SerializeField] private ObjectPool aiPool;

        [SerializeField] private ColorParams[] colors;
        [SerializeField] private ShapeParams[] shapes;
        [SerializeField] private SizeParams[] sizes;

        [SerializeField] private UIManager uiManager;
        
        public int UserUnitCount = 16;
        public int AIUnitCount = 16;
        public float UnitSpawnOffset = 2f;
        public int GridWidth = 5; 
        
        private readonly List<Unit> userUnits = new();
        private readonly List<Unit> aiUnits = new();

        public static bool IsGameStarted;

        private void Awake()
        {
            Assert.IsNotNull(userPool);
            Assert.IsNotNull(aiPool);
            Assert.IsNotNull(colors);
            Assert.IsNotNull(shapes);
            Assert.IsNotNull(sizes);
            Assert.IsNotNull(uiManager);
            
            userPool.Initialize();
            aiPool.Initialize();
        }

        private void Start()
        {
            for (var i = 0; i < UserUnitCount; i++)
            {
                var fighter = userPool.GetObject().GetComponent<Fighter>();
                userUnits.Add(fighter);
            }
            
            for (var i = 0; i < AIUnitCount; i++)
            {
                var soldier = aiPool.GetObject().GetComponent<Fighter>();
                aiUnits.Add(soldier);
            }

            RandomizeSoldiers();
        }

        private void RandomizeSoldiers()
        {
            for (var i = 0; i < userUnits.Count; i++)
            {
                var shape = shapes[Random.Range(0, shapes.Length)];
                var color = colors[Random.Range(0, colors.Length)];
                var size = sizes[Random.Range(0, sizes.Length)];
                
                SetGridPosition(userUnits[i].transform, i);
                
                userUnits[i].Init(shape, color, size);
                userUnits[i].SetEnemies(aiUnits, "Enemy");
                userUnits[i].OnDeath += SetUserUnitToPool;
            }

            for (var i = 0; i < aiUnits.Count; i++)
            {
                var randomShape = shapes[Random.Range(0, shapes.Length)];
                var randomColor = colors[Random.Range(0, colors.Length)];
                var randomSize = sizes[Random.Range(0, sizes.Length)];
                
                SetGridPosition(aiUnits[i].transform, i);
                
                aiUnits[i].Init(randomShape, randomColor, randomSize);
                aiUnits[i].SetEnemies(userUnits, "Friendly");
                aiUnits[i].SetEnemyView(randomColor.coloredMaterialEnemy);
                aiUnits[i].OnDeath += SetAIUnitToPool;

            }
        }

        private void SetUserUnitToPool(GameObject obj)
        {
            userPool.ReturnObject(obj);
            CheckGameState();
        }

        private void SetAIUnitToPool(GameObject obj)
        {
            aiPool.ReturnObject(obj);
            CheckGameState();
        }

        private void CheckGameState()
        {
            float aliveUnitsCount = 0;
            float aliveEnemyCount = 0;
            
            foreach (var soldier in userUnits)
                if (soldier.gameObject.activeInHierarchy) aliveUnitsCount++;
            
            foreach (var soldier in aiUnits)
                if (soldier.gameObject.activeInHierarchy) aliveEnemyCount++;
            
            uiManager.RefreshView(
                aliveUnitsCount/ userUnits.Count, 
                aliveEnemyCount / aiUnits.Count);
            
            if (aliveEnemyCount <= 0)
                EndGame(true);
            else if (aliveUnitsCount <= 0)
                EndGame(false);
                
        }

        private void EndGame(bool state)
        {
            IsGameStarted = false;
            uiManager.OpenEndPanel(state);
        }

        public void RandomizeArmyButton()
        {
            foreach (var soldier in userUnits)
                soldier.OnDeath -= SetAIUnitToPool;
            
            foreach (var soldier in aiUnits)
                soldier.OnDeath -= SetAIUnitToPool;
            
            RandomizeSoldiers();
        }

        public void StartButton()
        {
            IsGameStarted = true;
            uiManager.EnableBottomPanel(false);
        }

        private void SetGridPosition(Transform tr, int id)
        {
            var z = id % GridWidth * UnitSpawnOffset;
            var x = id / GridWidth * UnitSpawnOffset;
            tr.localPosition = new Vector3(x, 0, z);
        }
    }
}

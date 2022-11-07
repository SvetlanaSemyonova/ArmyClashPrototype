using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Content.Scripts.UI
{
    public class HealthView : MonoBehaviour
    {
        [SerializeField] private TextMeshPro healthValue;
        
        private readonly Vector3 _textRotationToUser = new(-45, -90, 0);
        
        public event Action<HealthView> OnFinish;

        public void SetText(string text)
        {
            healthValue.text = text;
        }

        private void Update()
        {
            healthValue.transform.eulerAngles = _textRotationToUser;
        }
        
        [UsedImplicitly]
        public void FinishAnimation()
        {
            OnFinish?.Invoke(this);
        }
    }
}

using UnityEngine;

namespace TurnBasedSample.Utils
{
    public class VisualHighlight : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material highlightedMaterial;
        
        private void OnEnable()
        {
            SwitchHighlight(false);
        }

        public void SetDefaultMaterial(Material material)
        {
            defaultMaterial = material;
        }

        public void SwitchHighlight(bool isHighlighted)
        {
            meshRenderer.sharedMaterial = isHighlighted ? highlightedMaterial : defaultMaterial;
        }
    }
}
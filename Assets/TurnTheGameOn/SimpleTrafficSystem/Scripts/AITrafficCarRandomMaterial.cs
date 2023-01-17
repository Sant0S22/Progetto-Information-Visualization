namespace TurnTheGameOn.SimpleTrafficSystem
{
    using UnityEngine;

    public class AITrafficCarRandomMaterial : MonoBehaviour
    {
        public Material[] getmaterial;

        void OnEnable()
        {
            MeshRenderer mesh = GetComponent<MeshRenderer>();
            int materialIndex = Random.Range(0, getmaterial.Length);
            mesh.material = getmaterial[materialIndex];
        }
    }
}
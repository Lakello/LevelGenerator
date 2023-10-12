using UnityEngine;

namespace LevelGenerator
{
    internal class HideOnPlay : MonoBehaviour
    {
        private void Start() =>
            gameObject.SetActive(false);
    }
}

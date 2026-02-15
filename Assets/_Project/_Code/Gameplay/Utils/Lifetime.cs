using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class Lifetime : MonoBehaviour
    {
        [SerializeField] private float _lifeTime = 1f;
        
        private void Start() => Destroy(gameObject, _lifeTime);
    }
}
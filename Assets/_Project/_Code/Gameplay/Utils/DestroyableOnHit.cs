using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class DestroyableOnHit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other) => Destroy(gameObject);
    }
}
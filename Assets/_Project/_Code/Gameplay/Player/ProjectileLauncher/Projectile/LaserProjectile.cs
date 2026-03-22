using DG.Tweening;
using UnityEngine;

namespace TankPlayground.Gameplay
{
    public class LaserProjectile : ProjectileBase
    {
        [SerializeField] private LineRenderer _lineRenderer;

        private const float Lifetime = 0.7f;
        private const float FadeDuration = 0.65f;

        public void SetPositions(Vector2 start, Vector2 end)
        {
            _lineRenderer.SetPosition(0, start);
            _lineRenderer.SetPosition(1, end);
        }
        
        public override void Shoot(Collider2D ownerCollider, Vector2 direction)
        {
            float alpha = 1f;
            DOTween
                .To(() => alpha, x => alpha = x, 0f, FadeDuration)
                .OnUpdate(() =>
                {
                    _lineRenderer.startColor = new Color(1f, 1f, 1f, alpha);
                    _lineRenderer.endColor = new Color(1f, 1f, 1f, alpha);
                });
            Destroy(gameObject, Lifetime);
        }
    }
}
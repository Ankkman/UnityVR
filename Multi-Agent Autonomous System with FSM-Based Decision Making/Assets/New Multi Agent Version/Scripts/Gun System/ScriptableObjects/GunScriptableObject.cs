using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

[CreateAssetMenu(fileName = "Gun", menuName = "Guns/Gun", order = 0)]
public class GunScriptableObject : ScriptableObject
{
    public ImpactType ImpactType;
    public GunType Type;
    public string Name;

    public ShootConfigurationScriptableObject ShootConfig;
    public TrailConfigScriptableObject TrailConfig;

    private MonoBehaviour activeMonoBehaviour;
    private Transform muzzleTransform;

    private float lastShootTime;
    private ObjectPool<TrailRenderer> trailPool;

    [Header("Damage")]
    public int Damage = 10;


    public void Initialize(Transform muzzle, MonoBehaviour owner)
    {
        muzzleTransform = muzzle;
        activeMonoBehaviour = owner;

        lastShootTime = 0;
        trailPool = new ObjectPool<TrailRenderer>(CreateTrail);
    }

    public void Shoot(Vector3 direction)
    {
        if (muzzleTransform == null) return;

        Vector3 shootDirection = direction.normalized
            + new Vector3(
                Random.Range(-ShootConfig.Spread.x, ShootConfig.Spread.x),
                Random.Range(-ShootConfig.Spread.y, ShootConfig.Spread.y),
                Random.Range(-ShootConfig.Spread.z, ShootConfig.Spread.z)
            );

        shootDirection.Normalize();

        Vector3 origin = muzzleTransform.position;

        if (Physics.Raycast(origin, shootDirection, out RaycastHit hit, float.MaxValue, ShootConfig.HitMask))
        {
            activeMonoBehaviour.StartCoroutine(
                PlayTrail(origin, hit.point, hit)
            );
        }
        else
        {
            activeMonoBehaviour.StartCoroutine(
                PlayTrail(
                    origin,
                    origin + (shootDirection * TrailConfig.MissDistance),
                    new RaycastHit()
                )
            );
        }
    }


    private IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
    {
        TrailRenderer instance = trailPool.Get();
        instance.gameObject.SetActive(true);
        instance.transform.position = startPoint;

        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(startPoint, endPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            instance.transform.position = Vector3.Lerp(
                startPoint,
                endPoint,
                Mathf.Clamp01(1 - (remainingDistance / distance))
            );

            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;
            yield return null;
        }

        instance.transform.position = endPoint;

        if (hit.collider != null)
        {
            // Apply damage
            HealthComponent health = hit.collider.GetComponentInParent<HealthComponent>();

            if (health != null)
            {
                health.TakeDamage(10); // you can make this configurable later
            }

            SurfaceManager.Instance.HandleImpact(
                hit.transform.gameObject,
                endPoint,
                hit.normal,
                ImpactType,
                0
            );
        }


        yield return new WaitForSeconds(TrailConfig.Duration);

        instance.emitting = false;
        instance.gameObject.SetActive(false);
        trailPool.Release(instance);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();

        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;

        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        return trail;
    }
}

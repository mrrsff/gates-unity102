using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum FireMode
    {
        Single,
        Auto
    }
    
    [Header("Weapon Settings")]
    public FireMode fireMode;
    public ParticleSystem muzzleFlash;
    public Transform muzzle;
    public LineRenderer tracer;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public ParticleSystem hitImpact;
    
    private bool isFiring;
    [Header("Weapon Stats")]
    public float fireRate = 3;
    public float damage = 10;
    public float recoilAmount = 25;
    public float unrecoilSpeed = 10;
    public float spread = 0.1f;
    
    
    private float nextShootTime;
    private float shootCooldown;
    private bool shouldMoveToOriginalPosition;
    private Vector3 originalPosition;
    private Coroutine disableLineRendererCoroutine;
    private void Awake()
    {
        shootCooldown = 1f / fireRate;
        originalPosition = transform.localPosition;
        audioSource.clip = shootSound;
    }

    public void SetIsFiring(bool value)
    {
        isFiring = value;
    }
    private void Shoot()
    {
        if (muzzleFlash) muzzleFlash.Play();
        
        Recoil();
        audioSource.Play();
        
        if (disableLineRendererCoroutine != null)
        {
            StopCoroutine(disableLineRendererCoroutine);
        }
        disableLineRendererCoroutine = StartCoroutine(DisableLineRenderer());
        
        nextShootTime = Time.time + shootCooldown;
        var shootDirection = muzzle.forward + UnityEngine.Random.insideUnitSphere * spread;
        var ray = new Ray(muzzle.position, shootDirection);
        if (!Physics.Raycast(ray, out var hit, 100f))
        {
            SetLineRenderer(ray.GetPoint(100));
            return;
        }
        SetLineRenderer(hit.point);
    
        var zombie = hit.collider.GetComponentInParent<Zombie>();
        if (zombie)
        {
            zombie.TakeDamage(hit, damage);
        }   
        else if (hitImpact) 
        {
            var impact = Instantiate(hitImpact, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impact.gameObject, impact.main.duration);
        }
    }

    private bool IsWeaponReady()
    {
        return Time.time >= nextShootTime;
    }

    private void Update()
    {
        if (isFiring && IsWeaponReady())
        {
            Shoot();
            if (fireMode == FireMode.Single)
            {
                isFiring = false;
            }
        }
        if (shouldMoveToOriginalPosition)
        {
            Unrecoil();
        }
    }

    private void Recoil()
    {
        transform.localPosition -= Vector3.forward * recoilAmount / 100f;
        shouldMoveToOriginalPosition = true;
        CrosshairConfigurator.Instance.Recoil(recoilAmount);
    }
    private void Unrecoil()
    {
        float distance = Vector3.Distance(transform.localPosition, originalPosition);
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * (unrecoilSpeed * distance + 1f));
        if (Vector3.Distance(transform.localPosition, originalPosition) < 0.005f)
        {
            shouldMoveToOriginalPosition = false;
        }
    }
    
    private void SetLineRenderer(Vector3 hitPoint)
    {
        tracer.enabled = true;
        tracer.SetPosition(0, muzzle.position);
        tracer.SetPosition(1, hitPoint);
    }
    
    private IEnumerator DisableLineRenderer()
    {
        yield return new WaitForSeconds(0.1f);
        tracer.enabled = false;
    }
}

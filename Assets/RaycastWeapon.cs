using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public bool isFiring;

    public int fireRate = 25;

    public ParticleSystem muzzleFlash;

    public ParticleSystem impactEffect;

    public Transform raycastOrigin;

    public Transform raycastDestination;

    public TrailRenderer tracerEffect;

    Ray ray;

    RaycastHit hitInfo;

    float accumulatedTime;

    public void StartFiring()
    {
        accumulatedTime = 0f;
        isFiring = true;
    }

    public void UpdateFiring()
    {
        accumulatedTime += Time.deltaTime;
        float fireInterval = 1f / fireRate;
        while (accumulatedTime >= 0f)
        {
            FireWeapon();
            accumulatedTime -= fireInterval;
        }
    }

    public void StopFiring()
    {
        isFiring = false;
    }

    void FireWeapon()
    {
        muzzleFlash.Emit(1);

        ray.origin = raycastOrigin.position;
        ray.direction = raycastDestination.position - raycastOrigin.position;

        var tracer = Instantiate(tracerEffect, ray.origin, Quaternion.identity);
        tracer.AddPosition(ray.origin);

        if (Physics.Raycast(ray, out hitInfo))
        {
            impactEffect.transform.position = hitInfo.point;
            impactEffect.transform.forward = hitInfo.normal;
            impactEffect.Emit(1);
        }

        tracer.transform.position = hitInfo.point;
    }
}

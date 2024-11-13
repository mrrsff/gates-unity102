using System;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum FireMode
    {
        Single,
        Auto
    }
    public FireMode fireMode;
    private bool isFiring;
    // ..., add other variables as you like. For example, damage, ammo, etc.
    
    public void SetIsFiring(bool value)
    {
        isFiring = value;
    }
    private void Shoot()
    {
        // TODO: Use Raycast to check if the weapon hits something.
    }

    private bool IsWeaponReady()
    {
        // TODO: Implement the logic for cooldowns and ammo.
        return true;
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
    }
}

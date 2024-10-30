using DG.Tweening;
using UnityEngine;

public class DoubleBarrelShotgun : Weapon
{
    [Header("DoubleShotgun Specific Settings")]
    public Transform firePointLeft;
    public Transform firePointRight;
    public int pelletsPerShot;
    private bool isFirstBarrelFired = false;
    private int originalPelletsPerShot; // Сохраняем оригинальное значение
    private float originalBulletSpread; // Сохраняем оригинальное значение

    [Header("Super Ability Settings")]
    public GameObject incendiaryProjectilePrefab;
    public GameObject ReturnedprojectilePrefab;

    protected override void Start()
    {
        base.Start();
        originalPelletsPerShot = pelletsPerShot; // Сохраняем исходные параметры
        originalBulletSpread = bulletSpread;
    }

    protected override void Update()
    {
        if (Input.GetMouseButtonDown(1) && !isReloading)
        {
            FireBothBarrels();
        }
        else if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            FireSingleShot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isCooldownActive)
        {
            if (ScoreManager.Instance.currentScore >= 100)
            {
                ActivateSuperAbility();
            }
            else
            {
                Debug.Log("Недостаточно очков стиля для активации способности");
            }
        }
    }

    protected override void ApplySuperAbilityEffects()
    {
        isSuperAbilityActive = true;
        pelletsPerShot += 3;
        bulletSpread *= 1.5f;
        projectilePrefab = incendiaryProjectilePrefab;

        Invoke("DeactivateSuperAbility", superAbilityDuration); // Запускаем таймер деактивации
    }

    protected override void RemoveSuperAbilityEffects()
    {
        isSuperAbilityActive = false;
        pelletsPerShot = originalPelletsPerShot; // Восстанавливаем исходные значения
        bulletSpread = originalBulletSpread;
        projectilePrefab = ReturnedprojectilePrefab;
    }

    protected override void Shoot()
    {

    }

    private void FireSingleShot()
    {
        if (currentAmmoInMagazine <= 0)
        {
            Debug.Log("Нет патронов");
            Reload();
            return;
        }
        Transform firePoint = isFirstBarrelFired ? firePointRight : firePointLeft;
        isFirstBarrelFired = !isFirstBarrelFired;

        FirePellets(firePoint);
        currentAmmoInMagazine--;

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        ApplyWeaponRecoil(); // Для одиночного выстрела, вызывает перегрузку с параметром false
        ApplyCameraRecoil();
    }

    private void FireBothBarrels()
    {
        if (currentAmmoInMagazine < 2)
        {
            Debug.Log("Не достаточно пуль для дуплета");
            FireSingleShot();
            return;
        }

        FirePellets(firePointLeft);
        FirePellets(firePointRight);
        currentAmmoInMagazine -= 2;

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        ApplyWeaponRecoil(true); // Для дуплета отдача вдвое
        ApplyCameraRecoil();
    }

    private void FirePellets(Transform firePoint)
    {
        Vector3 targetPoint = GetTargetPoint();
        float distanceToTarget = Vector3.Distance(firePoint.position, targetPoint);

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 dirWithSpread = AddBulletSpread(targetPoint - firePoint.position, distanceToTarget);

            GameObject pellet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            pellet.transform.forward = dirWithSpread.normalized;
            pellet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForce, ForceMode.Impulse);
        }
    }

    protected override void ApplyWeaponRecoil()
    {
        ApplyWeaponRecoil(false); // Одиночная отдача, передаем false
    }

    protected void ApplyWeaponRecoil(bool isDoubleBarrel)
    {
        Vector3 recoilOffset = isDoubleBarrel
            ? new Vector3(weaponRecoilX * 2, weaponRecoilY * 2, weaponRecoilZ * 2)
            : new Vector3(weaponRecoilX, weaponRecoilY, weaponRecoilZ);

        transform.DOLocalMove(initialWeaponPosition + recoilOffset, weaponRecoilDuration)
            .OnComplete(() => transform.DOLocalMove(initialWeaponPosition, 0.2f).SetEase(Ease.OutBack));
    }
}

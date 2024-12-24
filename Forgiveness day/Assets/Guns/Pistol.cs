using UnityEngine;
using DG.Tweening;

public class Pistol : Weapon
{
    [Header("Pistol Settings")]
    public float fireRate;
    private float nextTimeToFire = 0f;

    private int superAbilityShotsRemaining = 8;

    protected override void Shoot()
    {
        if (isSuperAbilityActive)
        {
            PerformRaycastShot();
            return;
        }

        if (currentAmmoInMagazine <= 0)
        {
            Reload();
            return;
        }

        if (Time.time < nextTimeToFire) return;
        nextTimeToFire = Time.time + fireRate;

        Vector3 targetPoint = GetTargetPoint();
        Vector3 direction = AddBulletSpread(targetPoint - firePoint.position, Vector3.Distance(firePoint.position, targetPoint));

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        bullet.transform.forward = direction.normalized;
        bullet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);

        currentAmmoInMagazine--;

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        ApplyWeaponRecoil();
        ApplyCameraRecoil();
    }

    private void PerformRaycastShot()
    {
        if (superAbilityShotsRemaining <= 0) return;

        Ray ray = gunCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Transform enemyRoot = hit.collider.transform.root;
                Transform head = FindHead(enemyRoot);

                Vector3 hitPoint = head != null ? head.position : hit.point;

                GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                bullet.transform.forward = (hitPoint - firePoint.position).normalized;
                bullet.GetComponent<Rigidbody>().AddForce((hitPoint - firePoint.position).normalized * shootForce, ForceMode.Impulse);

                superAbilityShotsRemaining--;

                if (audioSource && shootSound)
                {
                    audioSource.PlayOneShot(shootSound);
                }

                ApplyWeaponRecoil();
                ApplyCameraRecoil();

                if (superAbilityShotsRemaining <= 0)
                {
                    DeactivateSuperAbility();
                }
            }
        }
    }

    private Transform FindHead(Transform enemyRoot)
    {
        foreach (Transform child in enemyRoot.GetComponentsInChildren<Transform>())
        {
            EnemyCollider enemyCollider = child.GetComponent<EnemyCollider>();
            if (enemyCollider != null && enemyCollider.isHeadCollider)
            {
                return child;
            }
        }
        return null; // Голова не найдена
    }

    protected override void ApplySuperAbilityEffects()
    {
        base.ApplySuperAbilityEffects();

        currentAmmoInMagazine = Mathf.Min(8, maxAmmoInMagazine);

        // Включаем замедление времени
        Time.timeScale = 0.001f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        fireRate = 0f;

        superAbilityShotsRemaining = 8;
    }

    protected override void RemoveSuperAbilityEffects()
    {
        base.RemoveSuperAbilityEffects();

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        fireRate = 0.3f;
    }

    protected override void ApplyWeaponRecoil()
    {
        Vector3 recoilOffset = new Vector3(weaponRecoilX, weaponRecoilY, weaponRecoilZ);
        transform.DOLocalMove(initialWeaponPosition + recoilOffset, weaponRecoilDuration)
            .SetUpdate(UpdateType.Normal, true)
            .OnComplete(() => {
                transform.DOLocalMove(initialWeaponPosition, 0.2f).SetEase(Ease.OutBack);
            });
    }
}
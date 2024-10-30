using System.Collections;
using UnityEngine;
using DG.Tweening;

public abstract class Weapon : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmoInMagazine;
    public int currentAmmoInMagazine;
    public int maxAmmoTotal;
    public int currentAmmoTotal;
    public float reloadTime;

    [Header("Shot Settings")]
    public float bulletSpread;
    public float shootForce;
    public float effectiveRange;

    [Header("Weapon Recoil Settings")]
    public float weaponRecoilX;
    public float weaponRecoilY;
    public float weaponRecoilZ;
    public float weaponRecoilDuration;

    [Header("Camera Recoil Settings")]
    public float cameraRecoilPunchX;
    public float cameraRecoilPunchY;
    public float cameraRecoilPunchZ;
    public float cameraRecoilDuration;

    [Header("References")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public Camera gunCamera;
    public Camera playerCamera;
    public Camera hitCamera;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioSource audioSource;

    protected bool isReloading = false;
    protected bool isSuperAbilityActive = false;
    protected bool isCooldownActive = false;
    protected Vector3 initialWeaponPosition;

    protected float superAbilityDuration = 5f;
    private float superAbilityEndTime;

    protected virtual void Start()
    {
        currentAmmoInMagazine = maxAmmoInMagazine;
        currentAmmoTotal = maxAmmoTotal;
        initialWeaponPosition = transform.localPosition;
        EventManager.Instance.Subscribe("WeaponSwitched", OnWeaponSwitched);
        EventManager.Instance.Subscribe("SuperAbilityReady", OnSuperAbilityReady);
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isCooldownActive)
        {
            TryActivateSuperAbility();
        }

        // Проверка на завершение суперспособности, игнорирующая Time.timeScale
        if (isSuperAbilityActive && Time.realtimeSinceStartup >= superAbilityEndTime)
        {
            DeactivateSuperAbility();
        }
    }

    protected abstract void Shoot();

    protected virtual Vector3 GetTargetPoint()
    {
        Ray ray = hitCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        else
        {
            return ray.GetPoint(100f);
        }
    }

    protected virtual Vector3 AddBulletSpread(Vector3 direction, float distanceToTarget)
    {
        float adjustedSpread = bulletSpread * Mathf.Lerp(0.1f, 1f, distanceToTarget / effectiveRange);
        Vector2 randomCircle = Random.insideUnitCircle * adjustedSpread;

        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, direction).normalized;
        Vector3 spread = right * randomCircle.x + up * randomCircle.y;

        return direction + spread;
    }

    protected abstract void ApplyWeaponRecoil();

    protected virtual void ApplyCameraRecoil()
    {
        Vector3 punch = new Vector3(cameraRecoilPunchX, cameraRecoilPunchY, cameraRecoilPunchZ);
        playerCamera.transform.DOPunchPosition(punch, cameraRecoilDuration, 10, 0.1f).SetUpdate(UpdateType.Normal, true);
    }

    protected void Reload()
    {
        if (isReloading || currentAmmoTotal <= 0 || currentAmmoInMagazine == maxAmmoInMagazine)
        {
            return;
        }

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;

        if (audioSource && reloadSound)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = maxAmmoInMagazine - currentAmmoInMagazine;
        int ammoToReload = Mathf.Min(ammoNeeded, currentAmmoTotal);

        currentAmmoInMagazine += ammoToReload;
        currentAmmoTotal -= ammoToReload;

        isReloading = false;
        Debug.Log("Перезаряжен");
    }

    protected void TryActivateSuperAbility()
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

    protected void ActivateSuperAbility()
    {
        isSuperAbilityActive = true;
        isCooldownActive = true;

        ApplySuperAbilityEffects();
        ScoreManager.Instance.StartSuperAbilityCooldown(OnSuperAbilityCooldownComplete);

        // Устанавливаем конец суперспособности с использованием реального времени
        superAbilityEndTime = Time.realtimeSinceStartup + superAbilityDuration;
    }

    protected virtual void ApplySuperAbilityEffects()
    {
        // Метод для реализации конкретных эффектов суперспособности
    }

    protected void DeactivateSuperAbility()
    {
        isSuperAbilityActive = false;
        RemoveSuperAbilityEffects();
    }

    protected virtual void RemoveSuperAbilityEffects()
    {
        // Метод для отмены эффектов суперспособности
    }

    protected void OnSuperAbilityCooldownComplete()
    {
        isCooldownActive = true;
    }

    protected void OnSuperAbilityReady()
    {
        isCooldownActive = false;
    }

    protected void OnWeaponSwitched()
    {
        DeactivateSuperAbility();
        if (isReloading)
        {
            StopCoroutine(ReloadCoroutine());
            isReloading = false;
            Debug.Log("Перезарядка прервана при смене оружия");
        }
    }

    protected virtual void OnDestroy()
    {
        EventManager.Instance.Unsubscribe("WeaponSwitched", OnWeaponSwitched);
        EventManager.Instance.Unsubscribe("SuperAbilityReady", OnSuperAbilityReady);
    }
}

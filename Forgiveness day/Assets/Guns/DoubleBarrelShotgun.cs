using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DoubleBarrelShotgun : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmoInMagazine = 2;
    public int currentAmmoInMagazine;
    public int maxAmmoTotal = 20;
    public int currentAmmoTotal;
    public float reloadTime = 3f;

    [Header("Shot Settings")]
    public float bulletSpread = 0.1f;
    public float shootForce = 1500f;
    public int pelletsPerShot = 5;

    [Header("Weapon Recoil Settings")]
    public float weaponRecoilX = 0.2f;
    public float weaponRecoilY = -0.15f;
    public float weaponRecoilZ = -0.2f;
    public float weaponRecoilDuration = 0.15f;

    [Header("Camera Recoil Settings")]
    public float cameraRecoilPunchX = 0.05f;
    public float cameraRecoilPunchY = 0.05f;
    public float cameraRecoilPunchZ = 0.05f;
    public float cameraRecoilDuration = 0.1f;

    public GameObject pelletPrefab;
    public Transform firePointLeft;  // Левый ствол
    public Transform firePointRight; // Правый ствол
    public Camera gunCamera;
    public Camera playerCamera;
    public Camera Hitcam;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioSource audioSource;

    private bool isReloading = false;
    private bool isFirstBarrelFired = false;
    private Vector3 initialWeaponPosition;
    private float nextTimeToFire = 0f;

    private void Start()
    {
        currentAmmoInMagazine = maxAmmoInMagazine;
        currentAmmoTotal = maxAmmoTotal;
        initialWeaponPosition = transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            FireSingleShot();
        }

        if (Input.GetMouseButtonDown(1) && !isReloading)
        {
            FireBothBarrels();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
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
        isFirstBarrelFired = !isFirstBarrelFired; // Другой ствол после выстрела

        FirePellets(firePoint);

        currentAmmoInMagazine--;

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        ApplyWeaponRecoil(false);
        ApplyCameraRecoil();
    }

    private void FireBothBarrels()
    {
        if (currentAmmoInMagazine < 2)
        {
            Debug.Log("Не достаточно пуль для дуплета");
            FireSingleShot(); // Запрещаю стрелять дуплетом если только одна пуля в ружье
            return;
        }

        FirePellets(firePointLeft);
        FirePellets(firePointRight);

        currentAmmoInMagazine -= 2;

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        ApplyWeaponRecoil(true); // Сильная отдача при дуплете
        ApplyCameraRecoil();
    }

    private void FirePellets(Transform firePoint)
    {
        Vector3 targetPoint = GetTargetPoint();
        float distanceToTarget = Vector3.Distance(firePoint.position, targetPoint); // расстояние до цели, умное

        for (int i = 0; i < pelletsPerShot; i++)
        {
            Vector3 dirWithoutSpread = targetPoint - firePoint.position;
            Vector3 dirWithSpread = AddBulletSpread(dirWithoutSpread, distanceToTarget);

            GameObject pellet = Instantiate(pelletPrefab, firePoint.position, Quaternion.identity);
            pellet.transform.forward = dirWithSpread.normalized;
            pellet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForce, ForceMode.Impulse);
        }
    }

    private Vector3 GetTargetPoint()
    {
        Ray ray = Hitcam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            return ray.GetPoint(100f);
        }
    }

    private Vector3 AddBulletSpread(Vector3 direction, float distanceToTarget)
    {
        float adjustedSpread = bulletSpread * Mathf.Lerp(0.1f, 1f, distanceToTarget / 35f);

        Vector2 randomCircle = Random.insideUnitCircle * adjustedSpread;

        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, direction).normalized; 

        Vector3 spread = right * randomCircle.x + up * randomCircle.y;

        return direction + spread;
    }


    private void ApplyWeaponRecoil(bool isDoubleBarrel)
    {
        Vector3 recoilOffset = isDoubleBarrel
            ? new Vector3(weaponRecoilX * 2, weaponRecoilY * 2, weaponRecoilZ * 2)
            : new Vector3(weaponRecoilX, weaponRecoilY, weaponRecoilZ);

        transform.DOLocalMove(initialWeaponPosition + recoilOffset, weaponRecoilDuration)
            .OnComplete(() => {
                transform.DOLocalMove(initialWeaponPosition, 0.2f).SetEase(Ease.OutBack);
            });
    }

    private void ApplyCameraRecoil()
    {
        Vector3 punch = new Vector3(cameraRecoilPunchX, cameraRecoilPunchY, cameraRecoilPunchZ);
        playerCamera.transform.DOPunchPosition(punch, cameraRecoilDuration, 10, 0.1f);
    }

    private void Reload()
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
}

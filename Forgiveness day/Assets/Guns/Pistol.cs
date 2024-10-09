using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Pistol : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int maxAmmoInMagazine = 10;
    public int currentAmmoInMagazine;
    public int maxAmmoTotal = 50;
    public int currentAmmoTotal;
    public float reloadTime = 2f;

    [Header("Shot Settings")]
    public float fireRate = 0.5f;
    public float bulletSpread = 0.05f;
    public float shootForce = 1000f;
    public float effectiveRange = 35f; 

    [Header("Weapon Recoil Settings")]
    public float weaponRecoilX = 0.1f;
    public float weaponRecoilY = -0.07f;
    public float weaponRecoilZ = -0.09f;
    public float weaponRecoilDuration = 0.2f;

    [Header("Camera Recoil Settings")]
    public float cameraRecoilPunchX = 0.05f;
    public float cameraRecoilPunchY = 0.05f;
    public float cameraRecoilPunchZ = 0.05f;
    public float cameraRecoilDuration = 0.1f;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera gunCamera;
    public Camera playerCamera;
    public Camera hitCamera;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioSource audioSource;

    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    private Vector3 initialWeaponPosition; 

    private void Start()
    {
        currentAmmoInMagazine = maxAmmoInMagazine;
        currentAmmoTotal = maxAmmoTotal;
        initialWeaponPosition = transform.localPosition; 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextTimeToFire && !isReloading)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    private void Shoot()
    {
        if (currentAmmoInMagazine <= 0)
        {
            Debug.Log("пустой");
            Reload();
            return;
        }

        nextTimeToFire = Time.time + fireRate;

        Vector3 targetPoint = GetTargetPoint();
        Vector3 dirWithoutSpread = targetPoint - firePoint.position;
        Vector3 dirWithSpread = AddBulletSpread(dirWithoutSpread, Vector3.Distance(firePoint.position, targetPoint));

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.transform.forward = dirWithSpread.normalized;
        bullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForce, ForceMode.Impulse);

        currentAmmoInMagazine--;

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        ApplyWeaponRecoil();
        ApplyCameraRecoil();
    }

    private Vector3 GetTargetPoint()
    {
        Ray ray = hitCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
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
        float adjustedSpread = bulletSpread * Mathf.Lerp(0.1f, 1f, distanceToTarget / effectiveRange);
        Vector2 randomCircle = Random.insideUnitCircle * adjustedSpread;
        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, direction).normalized;
        Vector3 spread = right * randomCircle.x + up * randomCircle.y;
        return direction + spread;
    }

    private void ApplyWeaponRecoil()
    {
        Vector3 recoilOffset = new Vector3(weaponRecoilX, weaponRecoilY, weaponRecoilZ);
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

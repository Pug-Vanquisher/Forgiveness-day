using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DualUzi : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int maxAmmoInMagazine = 30;
    public int currentAmmoInMagazine;
    public int maxAmmoTotal = 90;
    public int currentAmmoTotal;
    public float reloadTime = 2f;
    public float fireRate = 0.1f;
    public float shootForce = 1000f;

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpread = 0.1f;
    public float effectiveRange = 50f;

    [Header("Recoil Settings")]
    public float xrecoil = 0.05f;
    public float yrecoil = -0.03f;
    public float zrecoil = -0.05f;
    public float recoilDuration = 0.1f;

    [Header("Weapon Sway Settings")]
    public float weaponSwayAmount = 0.05f;
    public float weaponSwayDuration = 0.1f;

    [Header("Weapon Objects")]
    public Transform leftFirePoint;
    public Transform rightFirePoint;
    public GameObject leftWeaponObject;
    public GameObject rightWeaponObject;

    [Header("Audio Settings")]
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioSource audioSource;

    [Header("Camera Settings")]
    public Camera gunCamera;
    public Camera playerCamera;
    public Camera Hitcam;

    private bool isReloading = false;
    private float nextTimeToFire = 0f;
    private Vector3 initialLeftWeaponPosition;
    private Vector3 initialRightWeaponPosition;

    private void Start()
    {
        currentAmmoInMagazine = maxAmmoInMagazine;
        currentAmmoTotal = maxAmmoTotal;
        initialLeftWeaponPosition = leftWeaponObject.transform.localPosition;
        initialRightWeaponPosition = rightWeaponObject.transform.localPosition;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextTimeToFire && !isReloading)
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
            Reload();
            return;
        }

        nextTimeToFire = Time.time + fireRate;

        Vector3 targetPoint = GetTargetPoint();

        ShootFromPoint(leftFirePoint, targetPoint);
        ShootFromPoint(rightFirePoint, targetPoint);

        currentAmmoInMagazine -= 2;

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        ApplyRecoil();
        ApplyWeaponSway();
    }

    private void ShootFromPoint(Transform firePoint, Vector3 targetPoint)
    {
        Vector3 dirWithoutSpread = targetPoint - firePoint.position;
        float distanceToTarget = Vector3.Distance(firePoint.position, targetPoint); 

        Vector3 dirWithSpread = AddBulletSpread(dirWithoutSpread, distanceToTarget);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.transform.forward = dirWithSpread.normalized;
        bullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForce, ForceMode.Impulse);
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
        float adjustedSpread = bulletSpread * Mathf.Lerp(0.1f, 1f, distanceToTarget / effectiveRange);

        Vector2 randomCircle = Random.insideUnitCircle * adjustedSpread;

        Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
        Vector3 up = Vector3.Cross(right, direction).normalized;

        Vector3 spread = right * randomCircle.x + up * randomCircle.y;

        return direction + spread;
    }

    private void ApplyRecoil()
    {
        Vector3 recoilOffset = new Vector3(xrecoil, yrecoil, zrecoil);

        gunCamera.transform.DOPunchPosition(recoilOffset, recoilDuration, 10, 0.1f)
            .OnComplete(() => {
                gunCamera.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
            });

        gunCamera.transform.DOScale(Vector3.one * 1.05f, 0.05f).OnComplete(() =>
        {
            gunCamera.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        });

        playerCamera.transform.DOPunchPosition(recoilOffset, recoilDuration, 10, 0.1f)
            .OnComplete(() => {
                playerCamera.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
            });

        playerCamera.transform.DOScale(Vector3.one * 1.05f, 0.05f).OnComplete(() =>
        {
            playerCamera.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        });
    }

    private void ApplyWeaponSway()
    {
        Vector3 leftWeaponSway = new Vector3(
            Random.Range(-weaponSwayAmount, weaponSwayAmount),
            Random.Range(-weaponSwayAmount, weaponSwayAmount),
            0);

        Vector3 rightWeaponSway = new Vector3(
            Random.Range(-weaponSwayAmount, weaponSwayAmount),
            Random.Range(-weaponSwayAmount, weaponSwayAmount),
            0);

        leftWeaponObject.transform.DOLocalMove(initialLeftWeaponPosition + leftWeaponSway, weaponSwayDuration).SetEase(Ease.OutBack)
            .OnComplete(() => {
                leftWeaponObject.transform.DOLocalMove(initialLeftWeaponPosition, weaponSwayDuration).SetEase(Ease.OutBack);
            });

        rightWeaponObject.transform.DOLocalMove(initialRightWeaponPosition + rightWeaponSway, weaponSwayDuration).SetEase(Ease.OutBack)
            .OnComplete(() => {
                rightWeaponObject.transform.DOLocalMove(initialRightWeaponPosition, weaponSwayDuration).SetEase(Ease.OutBack);
            });
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
    }
}

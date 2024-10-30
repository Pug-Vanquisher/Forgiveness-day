using System.Collections;
using UnityEngine;
using DG.Tweening;

public class DualUzi : Weapon
{
    [Header("Dual Uzi Specific Settings")]
    public float fireRate = 0.1f;
    public Transform leftFirePoint;
    public Transform rightFirePoint;
    public GameObject leftWeaponObject;
    public GameObject rightWeaponObject;
    public float weaponSwayAmount = 0.05f;
    public float weaponSwayDuration = 0.1f;

    private float nextTimeToFire = 0f;
    private Vector3 initialLeftWeaponPosition;
    private Vector3 initialRightWeaponPosition;

    protected override void Start()
    {
        base.Start();
        initialLeftWeaponPosition = leftWeaponObject.transform.localPosition;
        initialRightWeaponPosition = rightWeaponObject.transform.localPosition;
    }

    protected override void Update()
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

    protected override void Shoot()
    {
        if (currentAmmoInMagazine <= 0)
        {
            Debug.Log("No ammo");
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

        ApplyWeaponRecoil();
        ApplyWeaponSway();
    }

    private void ShootFromPoint(Transform firePoint, Vector3 targetPoint)
    {
        Vector3 dirWithoutSpread = targetPoint - firePoint.position;
        float distanceToTarget = Vector3.Distance(firePoint.position, targetPoint);
        Vector3 dirWithSpread = AddBulletSpread(dirWithoutSpread, distanceToTarget);

        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        bullet.transform.forward = dirWithSpread.normalized;
        bullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForce, ForceMode.Impulse);
    }

    protected override void ApplyWeaponRecoil()
    {
        Vector3 recoilOffset = new Vector3(weaponRecoilX, weaponRecoilY, weaponRecoilZ);

        gunCamera.transform.DOPunchPosition(recoilOffset, weaponRecoilDuration, 10, 0.1f)
            .OnComplete(() => {
                gunCamera.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
            });

        playerCamera.transform.DOPunchPosition(recoilOffset, weaponRecoilDuration, 10, 0.1f)
            .OnComplete(() => {
                playerCamera.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
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
}

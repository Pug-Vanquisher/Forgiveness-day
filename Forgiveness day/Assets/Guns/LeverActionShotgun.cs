using System.Collections;
using UnityEngine;
using DG.Tweening;

public class LeverActionShotgun : Weapon
{
    [Header("Shotgun Settings")]
    public int pelletsPerShot = 8;
    public float fireBonusCooldown = 1.2f;
    public float fireNormalCooldown = 1.8f;
    public GameObject incendiaryPelletPrefab;
    public Transform censerTransform;
    public float censerSwingThreshold = 0.1f;

    private bool isInIncendiaryZone = false;
    private float nextTimeToFire = 0f;
    private Vector3 initialCenserPosition;

    protected override void Start()
    {
        base.Start();
        initialCenserPosition = censerTransform.localPosition;
        StartCenserSwinging();
    }

    protected override void Update()
    {
        base.Update();
        CheckCenserPosition();
    }

    private void StartCenserSwinging()
    {
        if (isSuperAbilityActive) return; 

        float swingDuration = 1f;
        float swingDistance = 0.2f;

        censerTransform.DOLocalMoveX(-swingDistance, swingDuration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                censerTransform.DOLocalMoveX(swingDistance, swingDuration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() => StartCenserSwinging());
            });
    }

    private void CheckCenserPosition()
    {
        float censerXPosition = censerTransform.localPosition.x;
        isInIncendiaryZone = Mathf.Abs(censerXPosition) < censerSwingThreshold;
    }

    protected override void ApplySuperAbilityEffects()
    {
        censerTransform.DOKill(); // Остановка текущей анимации
        censerTransform.DOLocalMove(initialCenserPosition, 0.3f).SetEase(Ease.OutSine); // Возврат в начальную позицию
    }

    protected override void RemoveSuperAbilityEffects()
    {
        StartCenserSwinging(); // Возобновление движения кадила
    }

    protected override void Shoot()
    {
        if (Time.time < nextTimeToFire) return;

        if (currentAmmoInMagazine <= 0)
        {
            Reload();
            return;
        }

        currentAmmoInMagazine--;

        Vector3 targetPoint = GetTargetPoint();
        for (int i = 0; i < pelletsPerShot; i++)
        {
            FirePellet(targetPoint);
        }

        ApplyWeaponRecoil();
        ApplyCameraRecoil();

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        nextTimeToFire = Time.time + (isInIncendiaryZone ? fireBonusCooldown : fireNormalCooldown);
    }

    private void FirePellet(Vector3 targetPoint)
    {
        Transform selectedFirePoint = firePoint;
        Vector3 direction = AddBulletSpread(targetPoint - selectedFirePoint.position, effectiveRange);

        GameObject pelletPrefab = isInIncendiaryZone ? incendiaryPelletPrefab : projectilePrefab;
        GameObject pellet = Instantiate(pelletPrefab, selectedFirePoint.position, Quaternion.identity);
        pellet.transform.forward = direction.normalized;
        pellet.GetComponent<Rigidbody>().AddForce(direction.normalized * shootForce, ForceMode.Impulse);
    }

    protected override void ApplyWeaponRecoil()
    {
        Vector3 recoilOffset = new Vector3(weaponRecoilX, weaponRecoilY, weaponRecoilZ);
        transform.DOPunchPosition(recoilOffset, weaponRecoilDuration, 10, 0.1f)
            .OnComplete(() => transform.DOLocalMove(initialWeaponPosition, weaponRecoilDuration).SetEase(Ease.OutBack));
    }
}

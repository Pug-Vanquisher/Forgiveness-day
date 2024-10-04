using System.Collections;
using UnityEngine;
using DG.Tweening; // эффектики хуектики

public class Pistol : MonoBehaviour
{
    public int maxAmmoInMagazine = 10;
    public int currentAmmoInMagazine;
    public int maxAmmoTotal = 50;
    public int currentAmmoTotal;
    public float reloadTime = 2f;
    public float fireRate = 0.5f;
    public float bulletSpread = 0.05f;
    public float shootForce = 1000f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Camera gunCamera; // камера пушки только с нее проверять точку для выстрела!!!!!!!!!!!!!!!!!!!
    public Camera playerCamera; // обычная камера
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public AudioSource audioSource;

    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    private void Start()
    {
        currentAmmoInMagazine = maxAmmoInMagazine;
        currentAmmoTotal = maxAmmoTotal;
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
            Debug.Log("Out of ammo!");
            Reload();
            return;
        }

        nextTimeToFire = Time.time + fireRate;

        Vector3 targetPoint = GetTargetPoint();
        Vector3 dirWithoutSpread = targetPoint - firePoint.position;
        Vector3 dirWithSpread = AddBulletSpread(dirWithoutSpread);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.transform.forward = dirWithSpread.normalized;
        bullet.GetComponent<Rigidbody>().AddForce(dirWithSpread.normalized * shootForce, ForceMode.Impulse);

        currentAmmoInMagazine--;

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        ApplyRecoil();
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
        Debug.Log("Reload complete!");
    }

    private Vector3 GetTargetPoint() // метод говна и мочи, но работает
    {
        Ray ray = gunCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            return ray.GetPoint(1000f);
        }
    }

    private Vector3 AddBulletSpread(Vector3 direction)
    {
        float spreadX = Random.Range(-bulletSpread, bulletSpread);
        float spreadY = Random.Range(-bulletSpread, bulletSpread);
        return direction + new Vector3(spreadX, spreadY, 0);
    }

    private void ApplyRecoil() // нереальный расколбас
    {
       
        Vector3 recoilOffset = new Vector3(0f, -0.05f, -0.1f); 
        float duration = 0.2f; 

        
        gunCamera.transform.DOPunchPosition(recoilOffset, duration, 10, 0.1f)
            .OnComplete(() => {
                
                gunCamera.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
            });

        gunCamera.transform.DOScale(Vector3.one * 1.1f, 0.1f).OnComplete(() =>
        {
            gunCamera.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        });

        playerCamera.transform.DOPunchPosition(recoilOffset, duration, 10, 0.1f)
            .OnComplete(() => {
                playerCamera.transform.DOLocalMove(Vector3.zero, 0.2f).SetEase(Ease.OutBack);
            });

        playerCamera.transform.DOScale(Vector3.one * 1.1f, 0.1f).OnComplete(() =>
        {
            playerCamera.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        });
    }


}

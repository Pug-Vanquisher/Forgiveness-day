using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public GameObject[] weapons;
    private int currentWeaponIndex = 0;

    private void Start()
    {
        SelectWeapon();
    }

    private void Update()
    {
        int previousWeaponIndex = currentWeaponIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1)) currentWeaponIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentWeaponIndex = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentWeaponIndex = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) currentWeaponIndex = 3;

        // Скроллинг мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Length;
        else if (scroll < 0f) currentWeaponIndex = (currentWeaponIndex - 1 + weapons.Length) % weapons.Length;

        if (previousWeaponIndex != currentWeaponIndex)
        {
            EventManager.Instance.TriggerEvent("WeaponSwitched");
            SelectWeapon();
        }
    }

    private void SelectWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == currentWeaponIndex);
        }
    }
}

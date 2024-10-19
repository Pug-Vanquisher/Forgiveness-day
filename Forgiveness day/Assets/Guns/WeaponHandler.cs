using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public GameObject[] weapons; 

    private int currentWeaponIndex = 0;

    void Start()
    {

        SwitchWeapon(currentWeaponIndex);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentWeaponIndex = 0;
            SwitchWeapon(currentWeaponIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentWeaponIndex = 1;
            SwitchWeapon(currentWeaponIndex);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentWeaponIndex = 2;
            SwitchWeapon(currentWeaponIndex);
        }
    }


    private void SwitchWeapon(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(i == weaponIndex);
            }
        }
    }
}

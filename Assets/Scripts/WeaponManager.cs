using UnityEngine;
using UnityEngine.Networking;

class WeaponManager : NetworkBehaviour {
    [SerializeField]
    private string weaponLayerName = "Weapon";
    [SerializeField]
    private PlayerWeapon primaryWeapon;
    [SerializeField]
    private Transform weaponHolder;
    private PlayerWeapon currentWeapon;
    private WeaponGraphics currentGraphics;

    private void Start()
    {
        EquipWeapon(primaryWeapon);
    }
    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;
        GameObject _weaponIns = Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);
        currentGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if(currentGraphics == null)
            Debug.LogError("No WeaponGraphics component attached to object: " + _weaponIns.name);
        if (isLocalPlayer)
        {
            Util.SetLayerRecursively(_weaponIns,LayerMask.NameToLayer(weaponLayerName));
        }
    }
    public PlayerWeapon getCurrentWeapon()
    {
        return currentWeapon;
    }
    public WeaponGraphics getWeaponGraphics()
    {
       return currentGraphics; 
    }
}

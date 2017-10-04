using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

    public string weaponName = "M1911";
    public float range = 100f;
    public float damage = 10f;

    public float fireRate = 0f;
    public GameObject graphics;
}

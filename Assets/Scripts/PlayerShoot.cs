using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour {

    private const string PLAYER_TAG = "Player";
    

    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;
    private void Start()
    {
        if (cam == null)
        {
            Debug.Log("Player Shoot: no camera attached!");
            this.enabled = false;
        }
        weaponManager = GetComponent<WeaponManager>();
    }
    private void Update()
    {
        currentWeapon = weaponManager.getCurrentWeapon();
        if(currentWeapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot",0f,1/currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }
    
    //Called on all clients for shooting effects
    [ClientRpc]
    void RpcDisplayShootEffects()
    {
    Debug.Log("RpcDisplayShootGraphis is being envoked");
        weaponManager.getWeaponGraphics().muzzleFlash.Play();
    }
    [ClientRpc]
    void RpcDisplayHitEffects(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = (GameObject) Instantiate(weaponManager.getWeaponGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffect, 2f);
    }

    
    //actual shooting, call of OnShoot() on server
    [Client]
    void Shoot()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        CmdOnShoot();
        Debug.Log("Pew-pew!");
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            if (_hit.transform.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
            }
            CmdOnHit(_hit.point, _hit.normal);
        }
    }
    
    [Command]
    void CmdPlayerShot(string _ID, float _damage)
    {
        Debug.Log(_ID + " has been shot");
        Player _player = GameManager.GetPlayer(_ID);
        _player.RpcTakeDamage(_damage);
    }
    [Command]
    void CmdOnHit(Vector3 _pos, Vector3 _normal)
    {
        RpcDisplayHitEffects(_pos, _normal);
    }
    //Called on the server with player shooting
    [Command]
    void CmdOnShoot()
    {
        RpcDisplayShootEffects();
    }
}

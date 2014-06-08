﻿using System;
using System.Collections;
using System.Collections.Generic;
using PathologicalGames;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    private SoundManager _soundManager;                           // reference to global sound manager  
    private float        _nextFire                  = 0;          // used to time the next shot
    private float        _playerSpeed               = 18f;
    private enum State  { Playing, Explosion, Invincible }
    private State        _state                     = State.Playing;
    private Transform    _playerSpawnPoint;                        // Finds spawn point in editor
    private float        _shipInvisibleTime         = 1.3f;
    private SpawnPool    _pool                      = null;
    private ParticleEffectsManager _particleManager = null;
    private const float DEFAULT_PLAYER_SPEED        = 18f;

    [HideInInspector]
    public Weapons weapons                          = null;
    [HideInInspector]
    public Transform xform;


    void Start()
    {
        xform                     = transform;                                     
        _playerSpawnPoint         = GameObject.Find("PlayerSpawnPoint").transform; // set reference to Spawn Point Object
        xform.position            = _playerSpawnPoint.position;                    // Set player pos to spawnPoint pos
        _pool                     = GameObject.Find("GameManager").GetComponent<GameManager>().BulletPool;
        _particleManager          = GameObject.Find("ParticleManager").GetComponent<ParticleEffectsManager>();
        _soundManager             = SoundManager.GetSingleton();
        weapons                   = GetComponent<Weapons>();
    }

    void Update()
    {   
        // Is the player isn't alive, return
        if (_state == State.Explosion) return;

        HandlePlayerMovement();
        CheckIfShooting();
        CheckIfSwitchingWeapon();
    }

    /// <summary>
    /// Checks for inputs from player handles player movement
    /// </summary>
    private void HandlePlayerMovement()
    { 
        var horizontalMove = (_playerSpeed * Input.GetAxis("Horizontal"))    * Time.deltaTime;
        var verticalMove   = (_playerSpeed * Input.GetAxis("Vertical"))      * Time.deltaTime;
        var moveVector     = new Vector3(horizontalMove, verticalMove, 0);
        // prevents the player moving above its max speed on diagonals
        moveVector         = Vector3.ClampMagnitude(moveVector, _playerSpeed * Time.deltaTime);
        moveVector         = Vector3.ClampMagnitude(moveVector, _playerSpeed * Time.deltaTime); 

        // move the player
        xform.Translate(moveVector);
    }

    private void CheckIfSwitchingWeapon()
    {
        if (Input.GetButtonDown("SwitchWeapon"))
        {
            weapons.SwitchToNextWeapon();
        }

    }

    /// <summary>
    /// Is the player shooting? Left-click for bullets, right-click for missiles
    /// </summary>
    private void CheckIfShooting()
    {
        if (Input.GetButton("Fire1") && Time.time > _nextFire && _state == State.Playing)
        {
            // delay the next shot by the firing rate
            _nextFire = Time.time + weapons.GetFireRate();
           // ShootSpreadWeapon();
            weapons.ShootWeapon();

        }
    }
    
    /// <summary>
    /// Check what we are colliding with
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Is the player doing anything but playing? Get out of here.
        if (_state != State.Playing) return;    

        // check if it was a bullet we hit, if so put it back on its stack
        if (other.CompareTag("EnemyBullet"))
        {
            // put the bullet back on the stack for later re-use
//            PoolManager.Pools[_bulletPool].Despawn(other.transform);
            KillPlayer(other);
        }
        // If it is a pickup, then spawn the correct option
        //if (other.CompareTag("Pickup"))
        //{
        //    other.gameObject.GetComponent<Pickup>().SpawnMainOption();
        //    // TODO: Add this back to the spawning pool, not deactivate it
        //    other.gameObject.SetActive(false);
        //}
        // if it was an enemy, just destroy it and kill the player
        if (other.CompareTag("Enemy"))
        {
            KillPlayer(other);
        }
    }

    /// <summary>
    /// Kill player, make invisible & invisible, spawn at spawn point, and create particles
    /// </summary>
    /// <param name="other"> Who are we colliding with?</param>
    public void KillPlayer(Collider other)
    {
        if (_state != State.Playing) return;

        // Call enemy's Explode function for particles / sfx
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().Explode();
        }
        GameManager.lives--;
        StartCoroutine(OnBecameInvisible());
    }

    /// <summary>
    /// Spawns object at SpawnPoint, which is set in the editor.
    /// Main Camera -> SpawnPoint (child of Main Camera)
    /// </summary>
    private IEnumerator OnBecameInvisible()
    {
        _state = State.Explosion;

        _particleManager.CreatePlayerExplosionEffects(xform.position);
        // Make player ship invisible & move player to PlayerSpawnPoint
        gameObject.renderer.enabled = false;
        xform.position              = new Vector3(_playerSpawnPoint.position.x, _playerSpawnPoint.position.y, xform.position.z);
        yield return new WaitForSeconds(_shipInvisibleTime);

        if (GameManager.lives > 0)
        {
            ResetDefaultValues();
            // Set player to invincible while flashing & create particle effect at spawn point
            _state = State.Invincible;;
            _particleManager.CreateSpawnEffects(xform.position);

            // Make player ship visible again
            gameObject.renderer.enabled = true;

            // Make ship flash
            StartCoroutine(gameObject.GetComponent<FlashingObject>().Flash());

            yield return new WaitForSeconds(2.2f);
        }
        // Not flashing anymore? Now you can take hits
        _state = State.Playing;
    }

    public float GetPlayerSpeed()
    {
        return _playerSpeed;
    }

    public void SetPlayerSpeed(float playerSpeed)
    {
        _playerSpeed = playerSpeed;
    }

    /// <summary>
    /// Resets all player variables for powerups, upon player death
    /// </summary>
    private void ResetDefaultValues()
    {
        _playerSpeed = DEFAULT_PLAYER_SPEED;
        weapons.SetDefaultFireRate();
        weapons.SetDefaultBulletVel();
        weapons.SetDefaultBulletDmg();
    }

}






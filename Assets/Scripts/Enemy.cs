﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int             HitPoints;						// assigned when the enemy spawns
    public Vector3         Motion;							// assigned when the enemy spawns
    private Transform      _myTransform;
    private GameObject     _gameManager;
    private float          _enemyBulletSpeed;

    void Start()
    {
        _myTransform = transform;				            // cached for performance
        _enemyBulletSpeed = 6;                              // How fast enemy bullets fly
        _gameManager = GameObject.Find("GameManager");      // store the game manager for accessing its functions
    }

    void Update()
    {
        _myTransform.position += (Motion * Time.deltaTime); // move
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet"))			    // hit by a bullet
        {
            TakeDamage(1);								    // take away 1 hit point

            // disable the bullet and put it back on its stack
            other.gameObject.SetActive(false); 
            GameManager.playerBulletStack.Push(other.GetComponent<Bullet>());
        }
    }

    void TakeDamage(int damage)
    {
        // subtract damage and check if it's dead
        HitPoints -= damage;
        if (HitPoints <= 0)
            Explode();
    }

    public void Explode() // destroy this enemy
    {
        // draw particle explosion effect
        // TODO:play sound
        Destroy(this.gameObject);

        // increment the score
        GameManager.score++;
    }


    public IEnumerator Shoot(float delay) // waits for 'delay' seconds, then shoots directly at the player
    {
        yield return new WaitForSeconds(delay);

        // get a bullet from the stack
        Bullet newBullet = GameManager.enemyBulletStack.Pop();

        // position and enable it
        newBullet.gameObject.transform.position = _myTransform.position;
        newBullet.gameObject.SetActive(true);

        // calculate the direction to the player
        var shootVector = _gameManager.GetComponent<GameManager>().player.transform.position - _myTransform.position;

        // normalize this vector (make its length 1)
        shootVector.Normalize();

        // scale it up to the correct speed
        shootVector      *= _enemyBulletSpeed;
        newBullet.motion = shootVector;
    }
}
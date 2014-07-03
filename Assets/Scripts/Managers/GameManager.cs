﻿/* The Game Manager is an invisible Game Object which manages generic stuff like 
 * keeping track of bullets, spawning enemies, scoring, the GUI etc...
 */

using UnityEngine;
using PathologicalGames;

public class GameManager : MonoBehaviour
{ 
    public Player                        player          = null;
    public AudioClip                     backgroundMusic = null;
    [HideInInspector] public static int  score           = 0;
    [HideInInspector] public static int  lives           = 4;          
    /// <summary> Globally accessible BulletPool for all game objects to reference </summary>
    [HideInInspector] public SpawnPool   BulletPool;
    /// <summary> Globally accessible Particle for all game objects to reference   </summary>
    [HideInInspector] public SpawnPool   ParticlePool;

    private string            _bulletPoolString           = "BulletPool";
    private string            _ParticlePoolString         = "ParticlePool";
    private int               _respawnTime                = 3;
    private SoundManager      _soundManager;
    private static GameObject _GMGameObj;
    
    //----------------------
    // States and difficulty
    //-----------------------

    /// <summary> Sets the game difficulty </summary>
    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
    public Difficulty difficulty                          = Difficulty.Medium;

    /// <summary> Manages the current state of the game </summary>
    public enum CurrentState
    {
          GameStart
        , Playing
        , GameOver  
        , PauseScreen
        , StartScreen
        , OptionsScreen
        , CreditsScreen
        , UpdateScore
        , UpdateLives
    };
    public CurrentState currentState = CurrentState.StartScreen;



    /// <summary>
    /// Sets up a static singleton instance of GameManager, which is accessible to everything
    /// </summary>
    /// <returns>GameMananger object</returns>
    public static GameManager GetSingleton()
    {
        // If a Game Manager exists, then return that
        if (_GMGameObj != null) return (GameManager) _GMGameObj.GetComponent(typeof (GameManager));

        // If one doesn't exist, create a new GameManager
        _GMGameObj = new GameObject();
        return (GameManager)_GMGameObj.AddComponent(typeof(GameManager));
    }   


    /// <summary>
    /// Set up all of the global properties: Player, bullet & particle pools & set game difficulty
    /// </summary>
    private void Start()
    {
        GameEventManager.GameStart += SetSoundMananger;
        GameEventManager.GameStart += SetObjectPools;

        //TODO Tie this into the menus 
        SetDifficulty();
    }

    /// <summary>
    /// Grab a sound manager and play a track
    /// </summary>
    private void SetSoundMananger()
    {
        _soundManager = SoundManager.GetSingleton();
        _soundManager.PlayClip(backgroundMusic, false);
    }

    /// <summary>
    /// Creates a reference to object pools in the game
    /// //TODO: Do I never need this here?
    /// </summary>
    private void SetObjectPools()
    {
        BulletPool   = PoolManager.Pools[_bulletPoolString];
        ParticlePool = PoolManager.Pools[_ParticlePoolString];
    }

    //-----------------------------------------------------------------------------------------------
    //------------------------------------- Difficulty Settings -------------------------------------

    /// <summary>
    /// Sets the game difficulty accordingly 
    /// </summary>
    public void SetDifficulty()
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                EasyDifficultySettings();
                break;
            case Difficulty.Medium:
                MediumDifficultySettings();
                break;
            case Difficulty.Hard:
                HardDifficultySettings();
                break;
            default:
                DebugUtils.Assert(false);
                break;
        }
    }

    private void EasyDifficultySettings()
    {
        // Insert logic
    }

    private void MediumDifficultySettings()
    {
        // Insert logic
    }

    private void HardDifficultySettings()
    {
        // Insert logic
    }






}
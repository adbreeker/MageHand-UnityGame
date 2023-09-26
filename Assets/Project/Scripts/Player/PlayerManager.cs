using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerParams //static player parameters accessible from every script
{
    public static class Objects //mostly GameObjects associated with player
    {
        public static Camera playerCamera;
        public static GameObject player;
        public static GameObject hand;
    }

    public static class Controllers //scripts associated with player
    {
        public static PlayerMovement playerMovement;
        public static Inventory inventory;
        public static Spellbook spellbook;
        public static PauseMenu pauseMenu;
        public static HandInteractions handInteractions;
        public static SpellCasting spellCasting;
    }

    public static class Variables //variables associated with player
    {
        public static Vector3 cameraStartingPosition;
        public static float playerStartingMovementSpeed;
        public static float playerStartingRotationSpeed;
        public static bool uiActive;
    }
}

public class PlayerManager : MonoBehaviour //assigning static PlayerParams on Awake
{
    [Header("Objects")]
    public Camera playerCamera;
    public GameObject hand;

    [Header("Controllers")]
    public PlayerMovement playerMovement;
    public Inventory inventory;
    public Spellbook spellbook;
    public PauseMenu pauseMenu;
    public HandInteractions handInteractions;
    public SpellCasting spellCasting;

    //[Header("Variables")] - no need yet

    void Awake()
    {
        //objects
        PlayerParams.Objects.playerCamera = playerCamera;
        PlayerParams.Objects.player = gameObject;
        PlayerParams.Objects.hand = hand;

        //Controllers
        PlayerParams.Controllers.playerMovement = playerMovement;
        PlayerParams.Controllers.inventory = inventory;
        PlayerParams.Controllers.spellbook = spellbook;
        PlayerParams.Controllers.pauseMenu = pauseMenu;
        PlayerParams.Controllers.handInteractions = handInteractions;
        PlayerParams.Controllers.spellCasting = spellCasting;

        //Variables
        PlayerParams.Variables.cameraStartingPosition = playerCamera.transform.localPosition;
        PlayerParams.Variables.playerStartingMovementSpeed = playerMovement.movementSpeed;
        PlayerParams.Variables.playerStartingRotationSpeed = playerMovement.rotationSpeed;
        PlayerParams.Variables.uiActive = false;
    }
}

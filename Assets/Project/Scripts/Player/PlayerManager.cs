using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerParams //static player parameters accessible from every script
{
    public static class Objects //mostly GameObjects associated with player
    {
        public static Camera playerCamera;
        public static Camera uiCamera;
        public static GameObject player;
        public static GameObject hand;
    }

    public static class Controllers //scripts associated with player
    {
        public static PlayerManager playerManager;
        public static PlayerMovement playerMovement;
        public static Inventory inventory;
        public static Spellbook spellbook;
        public static Journal journal;
        public static PointsManager pointsManager;
        public static PauseMenu pauseMenu;
        public static SpellsMenu spellsMenu;
        public static HandInteractions handInteractions;
        public static SpellCasting spellCasting;
    }

    public static class Variables //variables associated with player
    {
        public static Vector3 cameraStartingPosition;
        public static float playerStartingMovementSpeed;
        public static float playerStartingRotationSpeed;
        public static float startingManaRegen;
        public static bool uiActive;
    }
}

public class PlayerManager : MonoBehaviour //assigning static PlayerParams on Awake
{
    [Header("Objects")]
    public Camera playerCamera;
    public Camera uiCamera;
    public GameObject hand;

    [Header("Controllers")]
    public PlayerMovement playerMovement;
    public Inventory inventory;
    public Spellbook spellbook;
    public Journal journal;
    public PointsManager pointsManager;
    public PauseMenu pauseMenu;
    public SpellsMenu spellsMenu;
    public HandInteractions handInteractions;
    public SpellCasting spellCasting;

    //[Header("Variables")] - no need yet

    void Awake()
    {
        //Objects
        PlayerParams.Objects.playerCamera = playerCamera;
        PlayerParams.Objects.player = gameObject;
        PlayerParams.Objects.uiCamera = uiCamera;
        PlayerParams.Objects.hand = hand;

        //Controllers
        PlayerParams.Controllers.playerManager = this;
        PlayerParams.Controllers.playerMovement = playerMovement;
        PlayerParams.Controllers.inventory = inventory;
        PlayerParams.Controllers.spellbook = spellbook;
        PlayerParams.Controllers.journal = journal;
        PlayerParams.Controllers.pointsManager = pointsManager;
        PlayerParams.Controllers.pauseMenu = pauseMenu;
        PlayerParams.Controllers.spellsMenu = spellsMenu;
        PlayerParams.Controllers.handInteractions = handInteractions;
        PlayerParams.Controllers.spellCasting = spellCasting;

        //Variables
        PlayerParams.Variables.cameraStartingPosition = playerCamera.transform.localPosition;
        PlayerParams.Variables.playerStartingMovementSpeed = playerMovement.movementSpeed;
        PlayerParams.Variables.playerStartingRotationSpeed = playerMovement.rotationSpeed;
        PlayerParams.Variables.uiActive = false;
    }
}

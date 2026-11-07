using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BrewingRecipe
{
    public List<GameObject> ingredients;
    public GameObject result;
    public Color brewingColor;
}

public class CauldronBehavior : InteractableBehavior
{
    [Header("Point to look inside cauldron")]
    [SerializeField] Transform _inCauldronCameraTransform;

    [Header("Interaction colliders")]
    [SerializeField] List<Collider> _cauldronInteractionColliders;

    [Header("Fluid:")]
    [SerializeField] CauldronFluid _fluid;
    [SerializeField] ParticleSystem _particles;

    [Header("Brewing:")]
    [SerializeField] List<BrewingRecipe> _brewingRecipes;

    bool _isCauldronZoomed = false;
    GameObject mainCamera;
    PlayerMovement playerMovement;
    PauseMenu pauseMenu;

    protected override void Start() //get necessary components on start
    {
        base.Start();
        mainCamera = PlayerParams.Objects.playerCamera.gameObject;
        playerMovement = PlayerParams.Controllers.playerMovement;
        pauseMenu = PlayerParams.Controllers.pauseMenu;

        _fluid.OnSplashTriggered.AddListener(Brewing);
    }

    private void Update() //listen to chest close input if chest is open
    {
        if (_isCauldronZoomed)
        {
            pauseMenu.ableToInteract = false;
            if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.S)) && !PlayerParams.Variables.uiActive)
            {
                OnClick();
            }
        }
    }

    public void OnClick() //move camera to chest/player and start chest animation
    {
        if (!playerMovement.isMoving && !playerMovement.isRotating && !playerMovement.isLeaning)
        {
            StartCoroutine(MoveCamera());
        }
    }

    IEnumerator MoveCamera() //move camera to couldron or to player
    {
        if (_isCauldronZoomed) //if cauldron is zoomed in then move camera to player 
        {
            _isCauldronZoomed = false;
            pauseMenu.ableToInteract = false;

            while (mainCamera.transform.localPosition != PlayerParams.Variables.cameraStartingPosition || mainCamera.transform.localRotation != Quaternion.Euler(0, 0, 0))
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.localPosition = Vector3.MoveTowards(mainCamera.transform.localPosition, PlayerParams.Variables.cameraStartingPosition, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, Quaternion.Euler(0, 0, 0), 5);
            }

            foreach (Collider col in _cauldronInteractionColliders) { col.enabled = true; }
            playerMovement.stopMovement = false;
        }
        else //else move camera to cauldron and stop player movement
        {
            playerMovement.stopMovement = true;
            foreach (Collider col in _cauldronInteractionColliders) { col.enabled = false; }

            Vector3 pivot = _inCauldronCameraTransform.parent.position; // local (0,0,0)
            Vector3 from = Vector3.ProjectOnPlane(_inCauldronCameraTransform.position - pivot, Vector3.up);
            Vector3 to = Vector3.ProjectOnPlane(mainCamera.transform.position - pivot, Vector3.up);
            float angle = Mathf.Round(Vector3.SignedAngle(from, to, Vector3.up) / 90f) * 90f;
            _inCauldronCameraTransform.localPosition = Quaternion.Euler(0, angle, 0) * _inCauldronCameraTransform.localPosition;

            while (mainCamera.transform.position != _inCauldronCameraTransform.position || mainCamera.transform.localRotation != _inCauldronCameraTransform.localRotation)
            {
                yield return new WaitForFixedUpdate();
                mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, _inCauldronCameraTransform.position, 0.2f);
                mainCamera.transform.localRotation = Quaternion.RotateTowards(mainCamera.transform.localRotation, _inCauldronCameraTransform.localRotation, 4);
            }

            _isCauldronZoomed = true;
        }
    }

    void Brewing()
    {
        List<GameObject> submergedItems = _fluid.GetSubmergedItems();

        foreach (BrewingRecipe recipe in _brewingRecipes)
        {
            bool allIngredientsPresent = true;
            foreach (GameObject ingredient in recipe.ingredients)
            {
                if(!submergedItems.Contains(ingredient))
                {
                    allIngredientsPresent = false;
                    break;
                }
            }

            if(allIngredientsPresent && submergedItems.Count == recipe.ingredients.Count)
            {
                isInteractable = false; //disable cauldron interaction until brewing is finished
                StartCoroutine(BrewingSuccessRoutine(recipe));

                foreach (GameObject ingredient in recipe.ingredients)
                {
                    Destroy(ingredient);
                }

                break;
            }
        }
    }

    IEnumerator BrewingSuccessRoutine(BrewingRecipe brewingRecipe)
    {
        yield return _fluid.SetColor(brewingRecipe.brewingColor);
        if (_isCauldronZoomed) { OnClick(); } //unzoom cauldron if zoomed in

        yield return new WaitForSeconds(0.5f);
        _particles.gameObject.SetActive(true);
        _particles.GetComponent<ParticlesColor>().ChangeColorOfEffect(brewingRecipe.brewingColor);
        _particles.Play(true);

        yield return new WaitForSeconds(_particles.main.duration - 0.5f);
        if (PrefabCheckUtility.IsSceneInstance(brewingRecipe.result)) //if already spawned in scene, just move it to the cauldron and enable it
        {
            brewingRecipe.result.transform.position = _fluid.transform.position - Vector3.up * 0.2f;
            brewingRecipe.result.transform.rotation = Quaternion.identity;
            brewingRecipe.result.SetActive(true); 
        }
        else //else instantiate the result prefab in the cauldron
        {
            Instantiate(brewingRecipe.result, _fluid.transform.position - Vector3.up * 0.2f, Quaternion.identity);
        } 

        yield return new WaitForSeconds(1f);
        isInteractable = true; //re-enable cauldron interaction
        StartCoroutine(_fluid.SetColor_Base());
    }
}

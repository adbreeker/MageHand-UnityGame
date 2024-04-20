using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnteringEarthquake : MonoBehaviour
{
    [SerializeField] Light _enteringLight;
    [SerializeField] Vector3 _enteringPosition;
    [SerializeField] GameObject _fallingRock;
    [SerializeField] Vector3 _baseFallingPosition;
    [SerializeField] GameObject _firstTutorial;

    Vector3 _firstTilePos;
    AudioSource earthquakeSound;
    ProgressSaving progressSaving;

    void Start()
    {
        PlayerParams.Controllers.playerMovement.stopMovement = true;
        _firstTilePos = PlayerParams.Objects.player.transform.position;
        PlayerParams.Objects.player.transform.position = _enteringPosition;
        earthquakeSound = GameParams.Managers.soundManager.CreateAudioSource(SoundManager.Sound.SFX_Earthquake);

        progressSaving = GameParams.Managers.saveManager;
        progressSaving.SaveGameState(SceneManager.GetActiveScene().name, 0, 0, 0, 0, 0, 0, 0);
        progressSaving.SaveProgressToFile();

        StartCoroutine(EarthquakeSimulation());
    }

    IEnumerator EarthquakeSimulation()
    {
        Vector3 cameraStartPos = PlayerParams.Objects.playerCamera.transform.localPosition;
        while(PlayerParams.Objects.player.transform.position != _firstTilePos)
        {
            PlayerParams.Objects.player.transform.position = Vector3.MoveTowards(
                PlayerParams.Objects.player.transform.position, _firstTilePos, 5.0f * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        earthquakeSound.Play();
        Destroy(earthquakeSound, earthquakeSound.clip.length);
        yield return new WaitForSeconds(0.3f);

        float lightRemovingStep = _enteringLight.intensity / 150.0f;
        for(int i = 0; i<200; i++)
        {
            PlayerParams.Objects.playerCamera.transform.localPosition = Vector3.MoveTowards(
                PlayerParams.Objects.playerCamera.transform.localPosition,
                new Vector3(Random.Range(-1f, 1f), Random.Range(cameraStartPos.y - 0.2f, cameraStartPos.y + 0.2f), cameraStartPos.z),
                Random.Range(2f, 4f) * Time.fixedDeltaTime);

            PlayerParams.Objects.player.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-3f, 3f));

            _enteringLight.intensity -= lightRemovingStep;

            if((i+1)%10 == 0)
            {
                Vector3 fallingPos = new Vector3(
                    _baseFallingPosition.x + Random.Range(-1.9f, 1.9f),
                    _baseFallingPosition.y,
                    _baseFallingPosition.z + Random.Range(-4.0f, 5.5f));
                Instantiate(_fallingRock, fallingPos, Random.rotation);
            }

            yield return new WaitForFixedUpdate();
        }
        Destroy(_enteringLight.gameObject);
        PlayerParams.Objects.playerCamera.transform.localPosition = cameraStartPos;
        PlayerParams.Objects.player.transform.rotation = Quaternion.identity;

        yield return new WaitForSeconds(0.5f);
        _firstTutorial.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        PlayerParams.Controllers.playerMovement.stopMovement = false;
    }
}

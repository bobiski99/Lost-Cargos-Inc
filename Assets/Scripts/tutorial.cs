using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class tutorial : MonoBehaviour
{
    [SerializeField] private ContractSequence contractsequenceer;
    [SerializeField] private Transform player;
    [SerializeField] private ContractFPSController_sc playerController;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float moveDuration = 1.8f;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private VideoClip tutorialVideo;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip turkishAudio;
    [SerializeField] private AudioClip englishAudio;
    [SerializeField] private TutorialLanguage currentLanguage = TutorialLanguage.Turkish;
    [SerializeField] private string nextSceneName;
    [SerializeField] private SceneFadeTransition sceneFadeTransition;

    public enum TutorialLanguage
    {
        Turkish,
        English
    }

    private bool playerNearby;
    private bool interactionStarted;
    private bool tutorialPlaying;

    private void Awake()
    {
        videoPlayer.playOnAwake = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
            
    }

    private void Update()
    {
        if (!playerNearby || interactionStarted)
        {
            return;
        }

        if (!contractsequenceer.tutorial_ready)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartTutorial();
        }
    }

    private void StartTutorial()
    {
        interactionStarted = true;

        playerController.SetControllerEnabled(false);
        Cursor.lockState = CursorLockMode.None;

        Vector3 targetRotation = new Vector3(player.eulerAngles.x,0f,player.eulerAngles.z);

        DOTween.Sequence().Join(player.DOMove(playerTarget.position, moveDuration).SetEase(Ease.InOutSine)).Join(player.DORotate(targetRotation, moveDuration).SetEase(Ease.InOutSine));

        if (playerCamera != null)
        {
            playerCamera.DOLocalRotate(Vector3.zero, moveDuration).SetEase(Ease.InOutSine);
            playerController.SetCameraPitch(0f);
        }

        PlayTutorialVideo();
    }

    private void PlayTutorialVideo()
    {
        if (tutorialVideo == null)
        {
            Debug.LogWarning("Tutorial video is missing!");
            return;
        }

        tutorialPlaying = true;
        videoPlayer.prepareCompleted -= OnVideoPrepared;
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.Stop();
        audioSource.Stop();
        videoPlayer.clip = tutorialVideo;
        videoPlayer.isLooping = false;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        audioSource.clip = currentLanguage == TutorialLanguage.Turkish? turkishAudio: englishAudio;
        audioSource.time = 0f;
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnVideoPrepared;

        if (!tutorialPlaying)
        {
            return;
        }
        vp.Play();
        if (audioSource.clip != null)
        {

            audioSource.Play();
        }
    }
    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!tutorialPlaying)
            return;

        tutorialPlaying = false;
        audioSource.Stop();

        StartCoroutine(FinishTutorial());
    }
    private IEnumerator FinishTutorial()
    {
        // Video bittikten sonra 1 saniye bekle
        yield return new WaitForSecondsRealtime(1f);

        if (sceneFadeTransition != null)
        {
            sceneFadeTransition.LoadSceneWithFade("Fabrika", 1.5f, 0.5f);
        }
        else
        {
            Debug.LogError("SceneFadeTransition reference is missing!");
        }
    }

    public void SetLanguage(TutorialLanguage language)
    {
        currentLanguage = language;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }
}

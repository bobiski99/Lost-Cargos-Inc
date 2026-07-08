using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private bool isDangerDoor;
    [SerializeField] private DangerDoorButton dangerButton;
    public bool state = true;
    public float ok1,ok2, ck1, ck2;


    public Transform kapi1, kapi2;

    [SerializeField] private int color, number;

    [Header("Ses Ayarlarý")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [Range(0.1f, 0.5f)][SerializeField] private float pitchRandomness = 0.2f;

    public bool CanAcceptBox()
    {
        if (!isDangerDoor)
            return true;

        return dangerButton != null && dangerButton.dangerOpen;
    }

    public void TryBox(int _color, int _number, bool _danger = false)
    {

        if (_color != color || _number != number)
        {
            CargoCoreManager.instance.takeDamage(); return; 
        }
       
        CargoCoreManager.instance.GivePoint(31);

    }
    public void OpenDoor()
    {
        kapi1.DOKill(true);
        kapi2.DOKill(true);
        if (state)
        {
            PlayRandomizedSound(openSound);
            kapi1.DORotate(new Vector3(ok1, 0, 0), 0.3f).SetEase(Ease.OutBack).SetRelative(true);
            kapi2.DORotate(new Vector3(ok2, 0, 0), 0.3f).SetEase(Ease.OutBack).SetRelative(true);
        }                           
        else                        
        {
            PlayRandomizedSound(closeSound);
            kapi1.DORotate(new Vector3(ck1, 0, 0), 0.9f).SetEase(Ease.OutBounce).SetRelative(true);
            kapi2.DORotate(new Vector3(ck2, 0, 0), 0.9f).SetEase(Ease.OutBounce).SetRelative(true);
        }

        state = !state;
    }

    private void PlayRandomizedSound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.pitch = Random.Range(1f - pitchRandomness, 1f + pitchRandomness);

            audioSource.PlayOneShot(clip);
        }
    }
}
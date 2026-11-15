
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ContractSequence : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private ContractFPSController_sc playerController;
    [SerializeField] private Transform playerTarget;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float moveDuration = 1.8f;
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Transform contractDocument;
    [SerializeField] private Transform documentHandAnchor;
    [SerializeField] private SignatureDrawer contractDrawer;
    [SerializeField] private Transform petitionDocument;
    [SerializeField] private Transform petitionHandAnchor;
    [SerializeField] private SignatureDrawer petitionDrawer;
    [SerializeField] private GameObject signatureConfirmButton;
    [SerializeField] private GameObject petitionChoicePanel;
    [SerializeField] private float documentMoveDuration = 1.2f;
    private bool sequenceStarted;
    private bool waitingForClick;
    private bool documentMoving;
    private bool signingPetition;
    private int dialogueStep;
    private SignatureDrawer activeDrawer;
    private Transform activeDocument;
    private Vector3 contractOriginalPosition;
    private Quaternion contractOriginalRotation;
    private Vector3 contractOriginalScale;
    private Transform contractOriginalParent;
    private Vector3 petitionOriginalPosition;
    private Quaternion petitionOriginalRotation;
    private Vector3 petitionOriginalScale;
    private Transform petitionOriginalParent;
    public bool SignedPetition 
    {
        get; private set;
    }

    private void Start()
    {
        contractOriginalParent = contractDocument.parent;
        contractOriginalPosition = contractDocument.localPosition;
        contractOriginalRotation = contractDocument.localRotation;
        contractOriginalScale = contractDocument.localScale;
        petitionOriginalParent = petitionDocument.parent;
        petitionOriginalPosition = petitionDocument.localPosition;
        petitionOriginalRotation = petitionDocument.localRotation;
        petitionOriginalScale = petitionDocument.localScale;
        dialoguePanel.SetActive(false);
        signatureConfirmButton.SetActive(false);
        petitionChoicePanel.SetActive(false);
        contractDrawer.SetDrawingEnabled(false);
        petitionDrawer.SetDrawingEnabled(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !sequenceStarted)
        {
            StartSequence();
        }
    }
    private void StartSequence()
    {
        sequenceStarted = true;
        playerController.SetControllerEnabled(false);
        Cursor.lockState = CursorLockMode.None;
        Vector3 targetRotation = new Vector3(player.eulerAngles.x,90f,player.eulerAngles.z);
        Sequence sequence = DOTween.Sequence();
        sequence.Join(player.DOMove(playerTarget.position, moveDuration).SetEase(Ease.InOutSine));

        sequence.Join(player.DORotate(targetRotation, moveDuration).SetEase(Ease.InOutSine));

        if (playerCamera != null)
        {
            sequence.Join(playerCamera.DOLocalRotate(Vector3.zero, moveDuration).SetEase(Ease.InOutSine));
        }

        sequence.OnComplete(() =>
        {
            playerController.SetCameraPitch(0f);
            ShowFirstDialogue();
        });
    }
    private void ShowFirstDialogue()
    {
        dialoguePanel.SetActive(true);

        dialogueText.text = "Welcome to our warehouse, so you wanna work in this shitty position? I don't blame you.";
        dialogueStep = 0;
        waitingForClick = true;
    }

    private void Update()
    {
        if (!waitingForClick || documentMoving)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleDialogueClick();
        }
    }

    private void HandleDialogueClick()
    {
        switch (dialogueStep)
        {
            case 0:
                dialogueText.text = "Here, sign this.";
                dialogueStep = 1;
                break;

            case 1:
                waitingForClick = false;
                GiveContract();
                break;

            case 2:
                waitingForClick = false;
                FinishSequence();
                break;
        }
    }
    private void GiveContract()
    {
        dialoguePanel.SetActive(false);
        documentMoving = true;

        MoveDocumentToHand(contractDocument,documentHandAnchor,() =>
            {
                activeDocument = contractDocument;
                activeDrawer = contractDrawer;
                signingPetition = false;

                OpenSignature();
            }
        );
    }
    private void OpenSignature()
    {
        documentMoving = false;
        dialoguePanel.SetActive(false);
        signatureConfirmButton.SetActive(false);
        if (activeDrawer == null)
        {
            Debug.LogError("Active SignatureDrawer is missing!");
            return;
        }
        activeDrawer.OnFirstStroke = ShowConfirmButton;
        activeDrawer.SetDrawingEnabled(true);
        Debug.Log("Signature mode enabled: " + activeDrawer.name);
    }

    private void ShowConfirmButton()
    {
        signatureConfirmButton.SetActive(true);
    }

    public void ConfirmSignature()
    {
        if (activeDrawer == null || !activeDrawer.HasSignature())
        {
            return;
        }
        activeDrawer.SetDrawingEnabled(false);
        activeDrawer.OnFirstStroke = null;
        signatureConfirmButton.SetActive(false);
        documentMoving = true;
        if (signingPetition)
        {
            SignedPetition = true;

            ReturnDocumentToOriginal(activeDocument,petitionOriginalParent,petitionOriginalPosition,petitionOriginalRotation,petitionOriginalScale,ShowFinalDialogue);
        }
        else
        {
            ReturnDocumentToOriginal(activeDocument,contractOriginalParent,contractOriginalPosition,contractOriginalRotation,contractOriginalScale,ShowPetitionQuestion);
        }
    }
    private void ShowPetitionQuestion()
    {
        documentMoving = false;
        dialoguePanel.SetActive(true);
        dialogueText.text = "Would you like to sign the petition too?";
        petitionChoicePanel.SetActive(true);
    }

    public void ChoosePetition(bool accepted)
    {
        petitionChoicePanel.SetActive(false);
        if (accepted)
        {
            GivePetition();
        }
        else
        {
            ShowFinalDialogue();
        }
    }
    private void GivePetition()
    {
        dialoguePanel.SetActive(false);
        waitingForClick = false;
        documentMoving = true;
        signingPetition = true;
        MoveDocumentToHand(petitionDocument,petitionHandAnchor,() =>
            {
                activeDocument = petitionDocument;
                activeDrawer = petitionDrawer;
                OpenSignature();
            }
        );
    }
    private void ShowFinalDialogue()
    {
        documentMoving = false;
        dialoguePanel.SetActive(true);
        dialogueText.text = "Okay, now get to the TV.";
        dialogueStep = 2;
        waitingForClick = true;
    }

    private void FinishSequence()
    {
        dialoguePanel.SetActive(false);
        playerController.SetControllerEnabled(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void MoveDocumentToHand(Transform document,Transform handAnchor,System.Action onComplete)
    {
        documentMoving = true;
        document.DOKill();
        DOTween.Sequence().Join(document.DOMove(handAnchor.position, documentMoveDuration).SetEase(Ease.OutCubic)).Join(document.DORotateQuaternion(handAnchor.rotation,
                    documentMoveDuration).SetEase(Ease.OutCubic)).OnComplete(() =>
            {
                document.SetParent(handAnchor, true);
                document.localPosition = Vector3.zero;
                document.localRotation = Quaternion.identity;
                onComplete?.Invoke();
            });
    }

    private void ReturnDocumentToOriginal(Transform document,Transform originalParent,Vector3 originalPosition,Quaternion originalRotation,Vector3 originalScale,System.Action onComplete)
    {
        document.DOKill();
        Vector3 targetWorldPosition;
        Quaternion targetWorldRotation;
        if (originalParent != null)
        {
            targetWorldPosition = originalParent.TransformPoint(originalPosition);

            targetWorldRotation = originalParent.rotation * originalRotation;
        }
        else
        {
            targetWorldPosition = originalPosition;
            targetWorldRotation = originalRotation;
        }

        DOTween.Sequence().Join(document.DOMove(targetWorldPosition, documentMoveDuration).SetEase(Ease.InOutSine)
            )
            .Join(document.DORotateQuaternion(targetWorldRotation,documentMoveDuration).SetEase(Ease.InOutSine)).OnComplete(() =>
            {
                document.SetParent(originalParent, true);
                document.localPosition = originalPosition;
                document.localRotation = originalRotation;
                document.localScale = originalScale;
                onComplete?.Invoke();
            });
    }
}
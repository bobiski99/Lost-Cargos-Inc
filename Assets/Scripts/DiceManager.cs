using DG.Tweening;
using TMPro;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public static DiceManager Instance;

    [Header("Camera")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Transform cameraTarget;
    [SerializeField] float cameraMoveTime = .7f;

    Vector3 cameraStartPos;
    Quaternion cameraStartRot;

    [Header("UI")]
    [SerializeField] GameObject dicePanel;
    [SerializeField] GameObject questionPanel;
    [SerializeField] GameObject choicePanel;

    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text timerText;

    [Header("Timer")]
    [SerializeField] float questionTime = 5f;
    [SerializeField] float choiceTime = 5f;

    float timer;

    bool waitingQuestion;
    bool waitingChoice;

    GameObject currentDice;

    bool choseOdd;

    public bool IsDiceEvent { get; private set; }

    void Awake()
    {
        Instance = this;

        if (mainCamera == null)
            mainCamera = Camera.main;

        cameraStartPos = mainCamera.transform.position;
        cameraStartRot = mainCamera.transform.rotation;

        dicePanel.SetActive(false);
        questionPanel.SetActive(false);
        choicePanel.SetActive(false);

        resultText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    public void StartDiceEvent(GameObject dice)
    {
        CursorManager.Instance.SetNormal();

        currentDice = dice;

        mainCamera.transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(mainCamera.transform.DOMove(cameraTarget.position, cameraMoveTime));

        seq.Join(mainCamera.transform.DORotateQuaternion(cameraTarget.rotation, cameraMoveTime));

        seq.OnComplete(() =>
        {
            Time.timeScale = 0;

            IsDiceEvent = true;

            PauseManager.Instance.CanPause = false;

            dicePanel.SetActive(true);
            questionPanel.SetActive(true);

            waitingQuestion = true;
            waitingChoice = false;

            timer = questionTime;

            timerText.gameObject.SetActive(true);
            timerText.text = Mathf.Ceil(timer).ToString();
        });
    }

    public void Yes()
    {
        waitingQuestion = false;

        questionPanel.SetActive(false);
        choicePanel.SetActive(true);

        waitingChoice = true;
        timer = choiceTime;

        timerText.text = Mathf.Ceil(timer).ToString();
    }

    public void No()
    {
        waitingQuestion = false;
        waitingChoice = false;

        questionPanel.SetActive(false);
        timerText.gameObject.SetActive(false);

        CargoCoreManager.instance.Score =
            Mathf.Max(0, CargoCoreManager.instance.Score - 100);

        CargoCoreManager.instance.UpdateScore();

        FinishEvent();
    }

    public void Odd()
    {
        waitingChoice = false;
        choseOdd = true;
        RollDice();
    }

    public void Even()
    {
        waitingChoice = false;
        choseOdd = false;
        RollDice();
    }

    void RollDice()
    {
        timerText.gameObject.SetActive(false);

        choicePanel.SetActive(false);

        resultText.gameObject.SetActive(true);

        int value = Random.Range(1, 7);

        bool isOdd = value % 2 == 1;

        if (isOdd == choseOdd)
        {
            CargoCoreManager.instance.Score *= 2;

            resultText.text =
                $"YOU WON!\n\nDice : {value}\n\nScore x2";
        }
        else
        {
            CargoCoreManager.instance.Score /= 2;

            resultText.text =
                $"YOU LOST!\n\nDice : {value}\n\nScore /2";
        }

        CargoCoreManager.instance.UpdateScore();

        DOVirtual.DelayedCall(2f, FinishEvent, true);
    }

    public void FinishEvent()
    {
        waitingQuestion = false;
        waitingChoice = false;

        timerText.gameObject.SetActive(false);

        PauseManager.Instance.CanPause = true;

        dicePanel.SetActive(false);
        questionPanel.SetActive(false);
        choicePanel.SetActive(false);
        resultText.gameObject.SetActive(false);

        Time.timeScale = 1;

        mainCamera.transform.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(mainCamera.transform.DOMove(cameraStartPos, cameraMoveTime));

        seq.Join(mainCamera.transform.DORotateQuaternion(cameraStartRot, cameraMoveTime));

        seq.OnComplete(() =>
        {
            if (currentDice != null)
                Destroy(currentDice);
        });

        IsDiceEvent = false;
    }

    void Update()
    {
        if (!IsDiceEvent)
            return;

        if (waitingQuestion)
        {
            timer -= Time.unscaledDeltaTime;

            timerText.text = Mathf.Ceil(timer).ToString();

            if (timer <= 0f)
            {
                No();
            }
        }

        if (waitingChoice)
        {
            timer -= Time.unscaledDeltaTime;

            timerText.text = Mathf.Ceil(timer).ToString();

            if (timer <= 0f)
            {
                waitingChoice = false;

                choicePanel.SetActive(false);

                timerText.gameObject.SetActive(false);

                resultText.gameObject.SetActive(true);
                resultText.text = "TIME'S UP!\n\nYOU LOST";

                CargoCoreManager.instance.Score /= 2;
                CargoCoreManager.instance.UpdateScore();

                DOVirtual.DelayedCall(2f, FinishEvent, true);
            }
        }
    }
}
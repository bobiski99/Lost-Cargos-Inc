using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using static UnityEngine.GraphicsBuffer;

public class Hold : MonoBehaviour
{
    [Header("Ayarlar")]
    public float YHeight = .5f;
    public float moveSpeed = 15f;
    public Vector3 holdRot;
    [SerializeField] private DangerDoorButton dangerDoorButton;
    [SerializeField] private DeliverButton deliver;
    [SerializeField] private fes hat_wear;
    [SerializeField] private discoball_sc disco;
    public bool ItsSellable = true;
    private Transform hatTarget;
    public Vector3 StarterPoint;
    Vector3 StarterRot;
    private bool holded = false;
    private Camera cam;
    Vector3 startScale;
    Vector3 hoverScale;
    Vector3 xray_hoverscale;
    Vector3 cat_hoverscale;
    GameObject pivot;
    private Rigidbody rb;
    bool dont;
    private bool catBoxAnimating = false;
    public bool scanner_holding = false;
    private Transform catObject;
    private float catStartX;
    private bool catMoved = false;
    [SerializeField] private ScannerScreen scannerScreen;
    private bool scanning = false;
    private bool clickObject_Moving = false;

    void Start()
    {

        rb = GetComponent<Rigidbody>();

        dangerDoorButton = FindAnyObjectByType<DangerDoorButton>();
        deliver = FindAnyObjectByType<DeliverButton>();
        scannerScreen = FindAnyObjectByType<ScannerScreen>();
        hat_wear = FindAnyObjectByType<fes>();
        catbox cat = FindAnyObjectByType<catbox>();
        disco = FindAnyObjectByType<discoball_sc>();

        if (cat != null)
        {
            catObject = cat.transform;
            catStartX = catObject.position.x;
        }
        hatTarget = GameObject.FindGameObjectWithTag("HatTarget").transform;
        if (StarterPoint == Vector3.zero) StarterPoint = transform.position;
        StarterRot = transform.eulerAngles;
        cam = Camera.main;
        startScale = transform.localScale;
        hoverScale = startScale * 0.6f;
        cat_hoverscale = startScale * 0.8f;
        xray_hoverscale = startScale * 0.8f;
        
    }

    private void OnMouseOver()
    {
        if (!holded && Vector3.Distance(transform.position, StarterPoint) < 0.02f)
            transform.GetComponent<OutlineController>().IsOutlineActive = true;
    }

    private void OnMouseExit()
    {
        transform.GetComponent<OutlineController>().IsOutlineActive = false;

    }

    void OnMouseDown()
    {
       
        if (CompareTag("scanner"))
        {
            scanner_holding = true;
            holded = true;
            return;
        }
        if (CompareTag("coin"))
        {
            holded = true;
            return;
        }
        Box bx = GetComponent<Box>();
        bool hasBox = bx != null;
        
        if (bx.isOnTable == false)
        {
            Debug.Log("noldu lan hattayken tıklayamıyon mu artık hıyar :D");
            return;
        }
        if (bx.isOnTable == true)
        {
            if (CompareTag("hatto"))
            {
                clickObject_Moving = true;
                bx.isOnTable = false;
                transform.DOKill();

                transform.DORotate(hatTarget.eulerAngles, 1f);
                transform.DOMove(hatTarget.position, 1f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    
                    CargoCoreManager.instance.GivePoint(10);
                    hat_wear.transform.DOLocalMoveY(0.00542f, 0.2f).SetEase(Ease.OutBack);
                    transform.DOKill();
                    
                    DOVirtual.DelayedCall(3f, () =>
                    {
                        hat_wear.transform.DOLocalMoveY(0.0082f, 1f).SetEase(Ease.InOutQuart); 
                        transform.DOKill();
                        transform.DOKill(true);
                        Destroy(gameObject);

                    });
                    
                    
                });

                return;
            }
            if (CompareTag("discoball"))
            {
                clickObject_Moving = true;
                transform.DOMoveY(10.93f, 0.5f).SetEase(Ease.OutBack);
                CargoCoreManager.instance.GivePoint(15);
                disco.discotime();
                DOVirtual.DelayedCall(2f, () =>
                {
                    Destroy(gameObject);
                });
                return;
            }

            

            holded = true;
            if (Vector3.Distance(transform.position, StarterPoint) > 0.02f)
                return;
            transform.DOKill();
            transform.GetComponent<OutlineController>().IsOutlineActive = false;
            transform.DORotate(holdRot, 0.6f).SetEase(Ease.OutBack);
            transform.DOShakeScale(0.3f, .3f).SetEase(Ease.OutBack);
        }
        

    }
    public void PlayHatAnimation()
    {
        transform.DOLocalMoveY(0.00542f, 0.2f);

        DOVirtual.DelayedCall(3f, () =>
        {
            transform.DOLocalMoveY(0.0082f, 0.2f);
        });
    }
    void OnMouseUp()
    {

        holded = false;
        scanner_holding = false;
        Box bx = GetComponent<Box>();
        if (scanning)
        {
            scanning = false;
            scannerScreen.StopScan();
        }
        if (bx != null && bx.hat)
        {
            return;
        }
        transform.DORotate(StarterRot, 0.5f).SetEase(Ease.OutElastic);
        
        bool isCat = bx != null && bx.cat;
        
        if (pivot != null)
        {
            if (isCat && pivot.CompareTag("cat_pivot"))
            {
                holded = false;
                catMoved = false;

                catObject.DOKill();
                catObject.DOMoveX(catStartX, 0.3f).SetEase(Ease.OutBack);
                transform.DOMoveY(-10, 0.5f).SetRelative(true).SetEase(Ease.InBack);
                transform.DOKill();
                Destroy(gameObject);
                
                CargoCoreManager.instance.GivePoint(50);


            }


            //buraya bak
            if (pivot.CompareTag("dangerpivot") && dangerDoorButton.dangerOpen)
            {
                dont = true;

                transform.DOScale(new Vector3(-0.2f, -0.2f, -0.2f), 0.2f).SetRelative(true).SetEase(Ease.OutBack);
                transform.DOMoveY(-10, 0.5f).SetRelative(true).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        if (bx.Danger)
                        {
                            deliver.Awake();
                            deliver.ActivateObject();
                        }
                        else
                        {
                            CargoCoreManager.instance.takeDamage();
                        }

                        dangerDoorButton.toggle_case();

                        transform.DOKill();
                        Destroy(gameObject);
                    });

                return;
            }
            if (pivot.TryGetComponent<DoorOpener>(out DoorOpener eskiKapi))
            {
                dont = true;
                transform.DOScale(new Vector3(-0.2f, -0.2f, -0.2f), 0.2f).SetEase(Ease.OutBack).SetRelative(true);

                transform.DOMoveY(-10, .5f).SetRelative(true).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        eskiKapi.TryBox(bx.color, bx.number, bx.Danger);

                        if (bx.Danger)
                            CargoCoreManager.instance.takeDamage(1);

                        eskiKapi.OpenDoor();

                        transform.DOKill();
                        Destroy(gameObject);
                    });
            }

        }
        else holded = false;
        transform.GetComponent<OutlineController>().IsOutlineActive = false;

        if (isCat && catObject != null && !catBoxAnimating)
        {
            catMoved = false;
            

            catObject.DOKill();

            catObject.DOMoveX(catStartX, 0.3f).SetEase(Ease.OutBack);
        }
    }

    private void OnDisable()
    {
        holded = false;
    }

    void Update()
    {
        if (clickObject_Moving)
        {
            return;
        }
            
        if (holded && !Input.GetMouseButton(0))
        {
            OnMouseUp();
            return;
        }
        if (dont) return;
        if (holded)
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            Plane zeminDuzlemi = new Plane(Vector3.up, new Vector3(0, StarterPoint.y, 0));

            if (zeminDuzlemi.Raycast(ray, out float mesafe))
            {
                Vector3 mouse3DKonumu = ray.GetPoint(mesafe);

                Vector3 target = new Vector3(mouse3DKonumu.x, StarterPoint.y + YHeight, mouse3DKonumu.z);

                Collider[] blocked = Physics.OverlapSphere(mouse3DKonumu, 0.005f);

                foreach (Collider col in blocked)
                {
                    if (col.CompareTag("noholdhere"))
                    {
                        holded = false;
                        OnMouseUp();
                    }
                }

               

                Collider[] hitColliders = Physics.OverlapSphere(mouse3DKonumu, .2f);

                Collider pvt = null;
                Box bx = GetComponent<Box>();
                bool isCat = bx != null && bx.cat;
                
                if (ItsSellable)
                {
                    if (isCat && catObject != null && !catMoved && !catBoxAnimating)
                    {
                        catBoxAnimating = true;

                        catMoved = true;

                        catObject.DOKill();

                        catObject.DOMoveX(catStartX - 6f, 0.3f).SetEase(Ease.OutBack);



                    }
                    foreach (Collider col in hitColliders)
                    {

                        if (col.gameObject == gameObject) continue;

                        if (col.CompareTag("Pivot") && !isCat)
                        {
                            pvt = col;
                            target = col.transform.position;

                            if ((transform.position - target).magnitude > 0.1f) transform.DORotate(StarterRot, 0.2f).SetEase(Ease.OutBack);
                            break;

                        }
                    }
                    foreach (Collider col in hitColliders)
                    {

                        if (col.gameObject == gameObject) continue;

                        if (col.CompareTag("xray") && !isCat)
                        {
                            if (!scanning)
                            {
                                scanning = true;
                                scannerScreen.StartScan(GetComponent<Box>());
                            }

                            Debug.Log("xray taraması");

                            pvt = col;
                            target = col.transform.position;

                            if ((transform.position - target).magnitude > 0.1f)
                                transform.DORotate(StarterRot, 0.2f).SetEase(Ease.OutBack);

                            break;
                        }
                    }
                    foreach (Collider col in hitColliders)
                    {

                        if (col.gameObject == gameObject) continue;

                        if (col.CompareTag("cat_pivot") && isCat)
                        {
                            pvt = col;
                            target = col.transform.position;

                            if ((transform.position - target).magnitude > 0.1f) transform.DORotate(StarterRot, 0.2f).SetEase(Ease.OutBack);
                            break;
                            

                        }
                    }
                    foreach (Collider col in hitColliders)
                    {

                        if (col.gameObject == gameObject) continue;

                        if (col.CompareTag("dangerpivot") && dangerDoorButton.dangerOpen && !isCat)
                        {
                            Debug.Log("ananinami");
                            pvt = col;
                            target = col.transform.position;
                            break;
                        }
                    }

                }
                
                GameObject yeniHedef = (pvt != null) ? pvt.gameObject : null;


                if (yeniHedef != pivot)
                {
                    // Xray'den çıktıysak taramayı durdur
                    if (pivot != null && pivot.CompareTag("xray") && (yeniHedef == null || !yeniHedef.CompareTag("xray")))
                    {
                        scanning = false;
                        scannerScreen.StopScan();
                    }
                    if (scanning && (yeniHedef == null || !yeniHedef.CompareTag("xray")))
                     {
                         scanning = false;
                     }
                    if (yeniHedef != null)
                    {
                        if (yeniHedef.CompareTag("cat_pivot"))
                        {
                            transform.DOScale(cat_hoverscale, 0.2f).SetEase(Ease.OutBack);
                        }
                        else if (yeniHedef.CompareTag("xray"))
                        {
                            transform.DOScale(xray_hoverscale, 0.2f).SetEase(Ease.OutBack);
                        }
                        else if (!yeniHedef.CompareTag("dangerpivot"))
                        {
                            transform.DOScale(hoverScale, 0.2f).SetEase(Ease.OutBack);
                        }
                    }
                    else
                    {
                        transform.DOScale(startScale, 0.2f).SetEase(Ease.OutBack);
                    }

                    if (pivot != null)
                    {
                        if (pivot.TryGetComponent<DoorOpener>(out DoorOpener eskiKapi))
                        {
                            eskiKapi.OpenDoor();
                        }
                    }

                    if (yeniHedef != null)
                    {
                        if (yeniHedef.TryGetComponent<DoorOpener>(out DoorOpener yeniKapi))
                        {
                            yeniKapi.OpenDoor();
                        }
                    }

                    pivot = yeniHedef;
                }
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * moveSpeed);
                if (Mathf.Abs(transform.position.x - target.x) > 0.2f && pvt == null) transform.DORotate(new Vector3(0, (transform.position.x - target.x) * 12, 0) + holdRot, 0.2f).SetEase(Ease.OutBack);
            }
        }
        else
        {





            transform.position = Vector3.Lerp(transform.position, StarterPoint, Time.deltaTime * moveSpeed);
        }


    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using StarterAssets;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Cinemachine;

//🟪🟪🟪🟪🟪🟪🟪
//🟪🟪🟪🟪🟪🟪🟪
//⬜⬛🟪⬜⬛🟪🟪
//🟪🟪🟪🟪🟪🟪🟪
//🟪⬛⬛⬛⬛⬛🟪
//🟪🟪⬛⬛⬛🟪🟪
//   🟪🟪🟪🟪🟪 THIS CODE IS ASS i blame sleep deprivation

public class OfficeMechanics : MonoBehaviour
{
    // PC ABILITIES
    public bool SilentVentilationOn=false;
    public bool MotionDOn=false;
    public bool AudioLureOn=false;
    public bool PlayingAD=false;

    //PUBLIC VARIABLES
    private bool[] AdTrack = new bool[10];
    public bool[] ADACCEPTED = new bool[10];
    public float CASHMONEY = 2000f;
    public float rayDistance = 5f; // Max distance for the raycast
    public LayerMask layerMask; // Layer mask to filter specific layers (optional)
    public float NoiseLevel = 9f;
    public float NoiseTask = 0f;
    public float NoiseSilent = 0f;
    public float Temperature = 60f;
    public float TemperatureTimer = 0f;
    public float TasksDoneC1 = 0;
    public float TasksDoneC2 = 0;
    public float TasksDoneC3 = 0;
    public string MotionDLastRoom = "no";
    public Transform _actualPositionObject;

    public Audio_Lure _audioLureScript;

    public bool Upgrade1=false;
    public bool Upgrade2=false;
    public bool Upgrade3=false;

    // PAGES GAMEOBJECTS
    private GameObject _showTasks;
    private GameObject _showTask1;
    private GameObject _showTask2;
    private GameObject _showTask3;
    private GameObject _showTask4;
    private GameObject _showMotionD;
    private GameObject _showAudio;
    private GameObject _showVent;
    private GameObject _AudioAnimParent;
    private Image _SilentVentUI;

    private TextMeshProUGUI _blinkTextmeshpro;
    private Image _VentAnim;
    private Image _AudioAnim;
    private Image _MotionDAnim;

    private GameObject _VentButton;
    private GameObject _MotionDButton;
    private TextMeshProUGUI _MotionDText;
    private TextMeshProUGUI _VentText;
    private TextMeshProUGUI _AudioLureText;

    private Vignette _vignette;
    private Volume _volume;

    private GameObject _AudioPositionParent;

    private GameObject _MotionDSignalParent;
    //ADS

    public GameObject[] _AD = new GameObject[5];
    public float AdTimer = 0f;
    private GameObject lastAdGO=null;

    // FAN COMPUTER AND FLASHLIGHT UI
    private GameObject  _imageOverlay;
    private GameObject _closedComputer;
    public Image _computerColor;
    public bool ComputerOn = true;

    public Image _fanColor;
    public bool FanOn = true;

    public bool notdead = true;

    private Image _FlashLightColor;
    private bool FlashlightOn = false;

    public TextMeshProUGUI _TemperatureUI;

    //OTHER OBJECTS
    private GameObject Notice1;
    private GameObject Notice2;
    private GameObject Notice3;
    private GameObject LogOff;

    //AUDIO STUFF
    private AudioSource _AudioController;
    private AudioSource _FanController;
    private AudioSource _ComputerController;
    private AudioSource _PlayerAudioController;
    public AudioClip _boop;
    public AudioClip Tasktype1;
    public AudioClip Tasktype2;
    public AudioClip Tasktype3;
    public AudioClip Tasktype1Upgrade;
    public AudioClip Tasktype2Upgrade;

    public AudioClip beep1;
    public AudioClip beep2;
    public AudioClip beep3;

    public AudioClip ComputerOnAudio;
    public AudioClip ComputerOffAudio;
    public AudioClip FanOnAudio;
    public AudioClip FanOffAudio;
    public AudioClip AudioWork;
    public AudioClip AudioNotWork;
    public AudioClip CantDoThatNow;

    public AudioClip PlayingAd;

    public bool NotInVent=true;
    
    // IMPLEMENTATION VARIABLES
    private GameObject _spinningThingy;
    private GameObject _animationObject = null;
    private Coroutine TemperatureCoroutine = null;
    private Coroutine currentTaskCoroutine = null;
    private Coroutine BlinkingCoroutine = null;
    private Coroutine BlinkPowerCoroutine = null;
    private Night_Mechanics _nightScript;
    private FirstPersonController _firstpersoncontroller;
    private GameObject AudioCircle = null;

    void Start()
    {
        _AD[0]= GameObject.Find("AD1");_AD[1]= GameObject.Find("AD2");_AD[2]= GameObject.Find("AD3");_AD[3]= GameObject.Find("AD4");_AD[4]= GameObject.Find("AD5");
        ADACCEPTED[0]=true;
        ADACCEPTED[1]=false;
        ADACCEPTED[2]=false;
        ADACCEPTED[3]=false;
        ADACCEPTED[4]=false;
        _volume = GameObject.FindWithTag("Volume").GetComponent<Volume>();
        if (!_volume.profile.TryGet(out _vignette))
        {
            Debug.LogError("No Vignette effect found in the Volume profile!");
            return;
        }
        _PlayerAudioController = GameObject.FindWithTag("Player").GetComponent<AudioSource>();
        _imageOverlay = GameObject.Find("FadeOverlay");
        _firstpersoncontroller = GameObject.FindWithTag("Player").GetComponent<FirstPersonController>();
        _nightScript = GameObject.FindWithTag("NightTime").GetComponent<Night_Mechanics>();
        _audioLureScript = GameObject.Find("AudioLureMechanic").GetComponent<Audio_Lure>();
        _AudioController = GameObject.FindWithTag("Office").GetComponent<AudioSource>();
        _FanController = _AudioController.transform.Find("FanSound").gameObject.GetComponent<AudioSource>();
        _ComputerController = _AudioController.transform.Find("PcSound").gameObject.GetComponent<AudioSource>();
        _showTasks = GameObject.Find("ShowTasks");
        _showMotionD = GameObject.Find("ShowMotionD");
        _showAudio = GameObject.Find("ShowAudio");
        _showVent = GameObject.Find("ShowVent");
        _showTask1 = GameObject.Find("Task1");
        _showTask2 = GameObject.Find("Task2");
        _showTask3 = GameObject.Find("Task3");
        _showTask4 = GameObject.Find("Task4");
        Notice1 = GameObject.Find("TaskNotDone1");
        Notice2 = GameObject.Find("TaskNotDone2");
        Notice3 = GameObject.Find("TaskNotDone3");
        LogOff = GameObject.Find("LOG OFF");
        _AudioPositionParent = GameObject.Find("AudioPositionParent");
        _MotionDSignalParent = GameObject.Find("MotionDSignalParent");
        _AudioAnimParent = GameObject.Find("AudioAnimParent");
        _SilentVentUI = GameObject.Find("SilentVentUI").GetComponent<Image>();

        _VentButton = GameObject.Find("ActivateVent");
        _MotionDButton = GameObject.Find("ScanMotion");
        _MotionDText = GameObject.Find("MotionDText").GetComponent<TextMeshProUGUI>();
        _VentText = GameObject.Find("VentText").GetComponent<TextMeshProUGUI>();
        _AudioLureText = GameObject.Find("AudioLureText").GetComponent<TextMeshProUGUI>();

        _VentAnim = GameObject.Find("VentAnim").GetComponent<Image>();
        _AudioAnim = GameObject.Find("AudioAnim").GetComponent<Image>();
        _MotionDAnim = GameObject.Find("MotionDAnim").GetComponent<Image>();

        _blinkTextmeshpro = GameObject.Find("PleaseWait").GetComponent<TextMeshProUGUI>();

        _TemperatureUI = GameObject.Find("TemperatureUI").GetComponent<TextMeshProUGUI>();
        _closedComputer = GameObject.Find("ClosedComputer");
        _computerColor = GameObject.Find("PowerDownColor").GetComponent<Image>();
        _fanColor = GameObject.Find("FanOffColor").GetComponent<Image>();
        _FlashLightColor = GameObject.Find("FlashlightOffColor").GetComponent<Image>();

        _closedComputer.SetActive(false);
        LogOff.SetActive(false);
        _showTask1.SetActive(false);
        _showTask2.SetActive(false);
        _showTask3.SetActive(false);
        _showTask4.SetActive(false);
        _showTasks.SetActive(true);
        _showMotionD.SetActive(false);
        _showAudio.SetActive(false);
        _showVent.SetActive(false);
        _AD[0].SetActive(false);_AD[1].SetActive(false);_AD[2].SetActive(false);_AD[3].SetActive(false);_AD[4].SetActive(false);
    }

    void Update()
    {
        AdFunction();
        MotionD();
        UpdateTemperature();
        if (Input.GetMouseButtonDown(0))
        {
            // Get the center of the screen
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        
            // Get the main camera
            Camera mainCamera = Camera.main;

            // Create a ray from the screen center point
            Ray ray = mainCamera.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, rayDistance, layerMask, QueryTriggerInteraction.Collide))
            {
                // Check if the hit collider has a BoxCollider component
                BoxCollider boxCollider = hit.collider as BoxCollider;
                if (boxCollider != null)
                {
                    // Do something when the box collider is hit
                    ButtonUse(hit.collider);
                    Debug.Log("Hit Box Collider: " + hit.collider.name);
                }
            }

            // Optional: Debug visualization of the ray in the Scene view
            //Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red, 1.0f);
        }

        if((Input.GetKeyDown(KeyCode.Z) && NotInVent==false) || (Input.GetKeyDown(KeyCode.X) && NotInVent==false))
            _PlayerAudioController.PlayOneShot(CantDoThatNow);

        if (Input.GetKeyDown(KeyCode.Z) && ComputerOn==true && NotInVent==true && PlayingAD==false)
        {
            if (currentTaskCoroutine != null || BlinkPowerCoroutine!=null)
            {
                if(currentTaskCoroutine!=null)
                {
                    StopCoroutine(currentTaskCoroutine);
                    StopCoroutine(BlinkingCoroutine);
                    if(NoiseTask==1f)
                        NoiseTask-=1f;
                    _spinningThingy.GetComponent<AudioSource>().enabled=false;
                    _spinningThingy.GetComponent<Image>().enabled=false;
                    _spinningThingy.GetComponent<Animator>().enabled=false;
                }
                NoiseSilent=0f;
                if(BlinkPowerCoroutine!=null)
                    StopCoroutine(BlinkPowerCoroutine);
                
                for(int i=0;i<4;i++)_nightScript.AnimatronicsRST[0]=false;
                _audioLureScript.transform.position = new Vector3(-2.20000005f,-0.798399925f,45.0999985f);
                if(AudioCircle!=null)
                    AudioCircle.GetComponent<Image>().enabled=false;

                currentTaskCoroutine = null;
                BlinkingCoroutine = null;
                BlinkPowerCoroutine = null;
                _SilentVentUI.enabled=false;
                
                SilentVentilationOn=false;
                AudioLureOn=false;
                MotionDOn=false;

                _blinkTextmeshpro.enabled=false;
                _MotionDAnim.enabled=false;
                _AudioAnim.enabled=false;
                _VentAnim.enabled=false;

                _MotionDText.text = "Activate motion scanning?";
                _VentText.text = "Activate Silent Ventilation?";
                _AudioLureText.text = "Select a room to activate music.";

                _VentButton.SetActive(true);
                _MotionDButton.SetActive(true);
                if(_animationObject!=null)
                {
                    _animationObject.GetComponent<Animator>().enabled=false;
                    _animationObject.GetComponent<Image>().enabled=false;
                }
            }
            _ComputerController.Stop();
            _showTasks.SetActive(false);
            _showMotionD.SetActive(false);
            _showAudio.SetActive(false);
            _showVent.SetActive(false);
            _showTask1.SetActive(false);
            _showTask2.SetActive(false);
            _showTask3.SetActive(false);
            _showTask4.SetActive(false);
            _AudioController.PlayOneShot(ComputerOffAudio);
            _closedComputer.SetActive(true);
            _computerColor.color = new Color32(100,100,100,225);
            NoiseLevel-=4f;
            ComputerOn=false;
        }
        else
        if (Input.GetKeyDown(KeyCode.Z) && ComputerOn==false && NotInVent==true  && PlayingAD==false)
        {
            _AudioController.PlayOneShot(ComputerOnAudio);
            _ComputerController.Play();
            _closedComputer.SetActive(false);
            _computerColor.color = new Color32(35,94,25,255);
            NoiseLevel+=4f;
            ComputerOn=true;
        }

        if (Input.GetKeyDown(KeyCode.X) && FanOn==true && NotInVent==true)
        {
            _AudioController.PlayOneShot(FanOffAudio);
            _fanColor.color = new Color32(100,100,100,225);
            NoiseLevel-=4f;
            FanOn=false;
            _FanController.Stop();
        }
        else
        if (Input.GetKeyDown(KeyCode.X) && FanOn==false && NotInVent==true)
        {
            _AudioController.PlayOneShot(FanOnAudio);
            _fanColor.color = new Color32(38,141,154,255);
            NoiseLevel+=4f;
            FanOn=true;
            _FanController.Play();
        }

        if (Input.GetKeyDown(KeyCode.F) && FlashlightOn==true)
        {
            _FlashLightColor.color = new Color32(100,100,100,225);
            FlashlightOn=false;
        }
        else
        if (Input.GetKeyDown(KeyCode.F) && FlashlightOn==false)
        {
            _FlashLightColor.color = new Color32(178,159,51,255);
            FlashlightOn=true;
        }
    }

    private void ButtonUse(Collider Button)
    {
        if (currentTaskCoroutine != null)
        {
            StopCoroutine(currentTaskCoroutine);
            StopCoroutine(BlinkingCoroutine);
            _blinkTextmeshpro.enabled=false;
            _spinningThingy.GetComponent<AudioSource>().enabled=false;
            _spinningThingy.GetComponent<Image>().enabled=false;
            _spinningThingy.GetComponent<Animator>().enabled=false;
            if(NoiseTask==1f)
                NoiseTask-=1f;
            Debug.Log("Stopped coroutine -1 noise");
            currentTaskCoroutine = null;
            BlinkingCoroutine = null;
        }

        if(Button.name== "SkipButton")
        {
            NoiseLevel-=2f;
            _AudioController.Stop();
            PlayingAD=false;
            lastAdGO.SetActive(false);
            _AudioController.PlayOneShot(beep2);
            display(_showTasks);
        }
        if(Button.name=="LOG OFF")
        {
            _AudioController.PlayOneShot(beep2);
            Debug.Log("END NIGHT");
        }
        if(Button.name=="Helpy")
        {
            _AudioController.PlayOneShot(_boop);
        }
        if(Button.name=="Tasks" || Button.name=="BACK")
        {
            _AudioController.PlayOneShot(beep2);
            display(_showTasks);
            _showTasks.transform.Find("MONEY").gameObject.GetComponent<TextMeshProUGUI>().text = "$" + CASHMONEY;
        }
        if(Button.name=="MotionD")
        {
            _AudioController.PlayOneShot(beep2);
            display(_showMotionD);
        }
        if(Button.name=="Audio")
        {
            _AudioController.PlayOneShot(beep2);
            display(_showAudio);
        }
        if(Button.name=="Vent")
        {
            _AudioController.PlayOneShot(beep2);
            display(_showVent);
        }
        if(Button.name=="ORDER SUPPLIES")
        {
            _AudioController.PlayOneShot(beep1);
            display(_showTask1);
            _showTask1.transform.Find("MONEY").gameObject.GetComponent<TextMeshProUGUI>().text = "$" + CASHMONEY;
        }
        if(Button.name=="ADVERTISING")
        {
            _AudioController.PlayOneShot(beep1);
            display(_showTask2);
            _showTask2.transform.Find("MONEY").gameObject.GetComponent<TextMeshProUGUI>().text = "$" + CASHMONEY;
        }
        if(Button.name=="MAINTENANCE")
        {
            _AudioController.PlayOneShot(beep1);
            display(_showTask3);
            _showTask3.transform.Find("MONEY").gameObject.GetComponent<TextMeshProUGUI>().text = "$" + CASHMONEY;
        }
        if(Button.name=="EQUIPMENT")
        {
            _AudioController.PlayOneShot(beep1);
            display(_showTask4);
            _showTask4.transform.Find("MONEY").gameObject.GetComponent<TextMeshProUGUI>().text = "$" + CASHMONEY;
        }
        if(Button.name=="ORDER CUPS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=8.35f;
            if(Upgrade1==true)
            {
                TimeT=TimeT*4f/5f;
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1Upgrade,1,TimeT));
            }
            else
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1,1,TimeT));
        }
        if(Button.name=="ORDER PLATES")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=8.35f;
            if(Upgrade1==true)
            {
                TimeT=TimeT*4f/5f;
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1Upgrade,1,TimeT));
            }
            else
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1,1,TimeT));
        }
        if(Button.name=="ORDER NAPKINS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=8.35f;
            if(Upgrade1==true)
            {
                TimeT=TimeT*4f/5f;
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1Upgrade,1,TimeT));
            }
            else
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1,1,TimeT));
        }
        if(Button.name=="ORDER UTENSILS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=8.35f;
            if(Upgrade1==true)
            {
                TimeT=TimeT*4f/5f;
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1Upgrade,1,TimeT));
            }
            else
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1,1,TimeT));
        }
        if(Button.name=="ORDER PIZZA KITS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=8.35f;
            if(Upgrade1==true)
            {
                TimeT=TimeT*4f/5f;
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1Upgrade,1,TimeT));
            }
            else
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype1,1,TimeT));
        }
        if(Button.name=="PRINT FLYERS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=16.68f;
            if(Upgrade2==true)
            {
                TimeT=TimeT*4f/5f;
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype2Upgrade,2,TimeT));
            }
            else
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype2,2,TimeT));
        }
        if(Button.name=="PRINT POSTERS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=16.68f;
            if(Upgrade2==true)
            {
                TimeT=TimeT*4f/5f;
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype2Upgrade,2,TimeT));
            }
            else
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype2,2,TimeT));
        }
        if(Button.name=="PRINT MENUS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=16.68f;
            if(Upgrade2==true)
            {
                TimeT=TimeT*4f/5f;
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype2Upgrade,2,TimeT));
            }
            else
                currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype2,2,TimeT));
        }
        if(Button.name=="UNCLOG TOILETS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=12.52f;
            if(Upgrade3==true)
                TimeT=TimeT*4f/5f;
            currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype3,3,TimeT));
        }
        if(Button.name=="CLEAN OVENS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=12.52f;
            if(Upgrade3==true)
                TimeT=TimeT*4f/5f;
            currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype3,3,TimeT));
        }
        if(Button.name=="REPLACE BULBS")
        {
            _AudioController.PlayOneShot(beep3);
            _spinningThingy = Button.gameObject.transform.Find("NotDone").gameObject;
            float TimeT=12.52f;
            if(Upgrade3==true)
                TimeT=TimeT*4f/5f;
            currentTaskCoroutine = StartCoroutine(DoTask(Button.gameObject,_spinningThingy,Tasktype3,3,TimeT));
        }

        if(Button.name=="2X PRINTER" && CASHMONEY>=500f)
        {
            CASHMONEY-=500;
            Button.gameObject.SetActive(false);
            _AudioController.PlayOneShot(beep3);
            Upgrade2 = true;
            _showTask4.transform.Find("MONEY").gameObject.GetComponent<TextMeshProUGUI>().text = "$" + CASHMONEY;
        }
        if(Button.name=="HI-SPD UPLINK" && CASHMONEY>=500f)
        {
            CASHMONEY-=500;
            Button.gameObject.SetActive(false);
            _AudioController.PlayOneShot(beep3);
            Upgrade1 = true;
            _showTask4.transform.Find("MONEY").gameObject.GetComponent<TextMeshProUGUI>().text = "$" + CASHMONEY;
        }
        if(Button.name=="HIRE HANDYMAN" && CASHMONEY>=900f)
        {
            CASHMONEY-=900;
            Button.gameObject.SetActive(false);
            _AudioController.PlayOneShot(beep3);
            Upgrade3 = true;
            _showTask4.transform.Find("MONEY").gameObject.GetComponent<TextMeshProUGUI>().text = "$" + CASHMONEY;
        }

        if(Button.name=="ActivateVent")
        {
            NoiseSilent-=2f;
            _audioLureScript.transform.position = new Vector3(-2.20000005f,-0.798399925f,45.0999985f);
            _SilentVentUI.enabled=true;
            if(BlinkPowerCoroutine!=null)
                StopCoroutine(BlinkPowerCoroutine);
            MotionDOn=false;
            AudioLureOn=false;
            _MotionDAnim.enabled=false;
            _AudioAnim.enabled=false;
            _VentAnim.enabled=false;

            _VentButton.SetActive(false);
            _MotionDButton.SetActive(true);
            for(int i=0;i<4;i++)_nightScript.AnimatronicsRST[0]=false;

            if(AudioCircle!=null)
                    AudioCircle.GetComponent<Image>().enabled=false;

            _MotionDText.text = "Activate motion scanning?";
            _VentText.text = "Silent Ventilation active...";
            _AudioLureText.text = "Select a room to activate music.";
            SilentVentilationOn=!SilentVentilationOn;
            BlinkPowerCoroutine = StartCoroutine(BlinkingPowerup(_VentAnim));
            _AudioController.PlayOneShot(beep1);
            if(_animationObject!=null)
            {
                _animationObject.GetComponent<Animator>().enabled=false;
                _animationObject.GetComponent<Image>().enabled=false;
            }
        }
        if(Button.name=="ScanMotion")
        {
            NoiseSilent=0f;
            _audioLureScript.transform.position = new Vector3(-2.20000005f,-0.798399925f,45.0999985f);
            _SilentVentUI.enabled=false;
            if(BlinkPowerCoroutine!=null)
                StopCoroutine(BlinkPowerCoroutine);
            SilentVentilationOn=false;
            AudioLureOn=false;
            _MotionDAnim.enabled=false;
            _AudioAnim.enabled=false;
            _VentAnim.enabled=false;
            for(int i=0;i<4;i++)_nightScript.AnimatronicsRST[0]=false;

            _VentButton.SetActive(true);
            _MotionDButton.SetActive(false);

            if(AudioCircle!=null)
                    AudioCircle.GetComponent<Image>().enabled=false;

            _MotionDText.text = "SCANNING...";
            _VentText.text = "Activate Silent Ventilation?";
            _AudioLureText.text = "Select a room to activate music.";

            MotionDOn=!MotionDOn;
            BlinkPowerCoroutine = StartCoroutine(BlinkingPowerup(_MotionDAnim));
            _AudioController.PlayOneShot(beep1);
            if(_animationObject!=null)
            {
                _animationObject.GetComponent<Animator>().enabled=false;
                _animationObject.GetComponent<Image>().enabled=false;
            }
        }
        Transform[] children = _AudioPositionParent.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            if (child.name == Button.name)
            {
                NoiseSilent=0f;
                if(AudioCircle!=null)
                    AudioCircle.GetComponent<Image>().enabled=false;
                _SilentVentUI.enabled=false;
                if(BlinkPowerCoroutine!=null)
                    StopCoroutine(BlinkPowerCoroutine);
                MotionDOn=false;
                SilentVentilationOn=false;
                
                _MotionDAnim.enabled=false;
                _AudioAnim.enabled=false;
                _VentAnim.enabled=false;

                _VentButton.SetActive(true);
                _MotionDButton.SetActive(true);
                for(int i=0;i<4;i++)_nightScript.AnimatronicsRST[0]=false;

                _MotionDText.text = "Activate Motion Scanning?";
                _VentText.text = "Activate Silent Ventilation?";
                _AudioLureText.text = "Playing...";

                AudioLureOn=!AudioLureOn;
                BlinkPowerCoroutine = StartCoroutine(BlinkingPowerup(_AudioAnim));
                _AudioController.PlayOneShot(beep1);

                AudioCircle = _AudioAnimParent.transform.Find(child.name).gameObject.transform.Find("Image").gameObject;
                AudioCircle.GetComponent<Image>().enabled=true;

                _actualPositionObject = _nightScript.transform.Find("Room"+child.name);
                _audioLureScript._position = child.name;
                _audioLureScript.transform.SetParent(_actualPositionObject);
                _audioLureScript.transform.localPosition = Vector3.zero;

                if(_animationObject!=null)
                {
                    _animationObject.GetComponent<Animator>().enabled=false;
                    _animationObject.GetComponent<Image>().enabled=false;
                }
                _animationObject = _AudioAnimParent.transform.Find(child.name).gameObject;
                _animationObject.GetComponent<Animator>().enabled=true;
                _animationObject.GetComponent<Image>().enabled=true;
            }
        }
    }

    private IEnumerator DoTask(GameObject ButtonGO,GameObject Spinner,AudioClip TaskAudioClip,int typeOfTask,float TaskDuration)
    {
        BlinkingCoroutine = StartCoroutine(Blinking(_blinkTextmeshpro,TaskDuration));
        Spinner.GetComponent<Image>().enabled=true;
        Spinner.GetComponent<Animator>().enabled=true;
        Spinner.GetComponent<AudioSource>().enabled=true;
        Spinner.GetComponent<AudioSource>().PlayOneShot(TaskAudioClip);
        if(NoiseTask==0f)
            NoiseTask+=1f;
        yield return new WaitForSeconds(TaskDuration);
        if(NoiseTask==1f)
            NoiseTask-=1f;
        Debug.Log("Ended coroutine no stop -1 noise");
        Spinner.GetComponent<AudioSource>().enabled=false;
        Spinner.GetComponent<Image>().enabled=false;
        Spinner.GetComponent<Animator>().enabled=false;
        ButtonGO.SetActive(false);
        
        if(typeOfTask==1)TasksDoneC1+=1;
        if(typeOfTask==2)TasksDoneC2+=1;
        if(typeOfTask==3)TasksDoneC3+=1;

        if(TasksDoneC1==5)Notice1.GetComponent<Image>().enabled=false;
        if(TasksDoneC2==3)Notice2.GetComponent<Image>().enabled=false;
        if(TasksDoneC3==3)Notice3.GetComponent<Image>().enabled=false;

        if(TasksDoneC1==5 && TasksDoneC2==3 && TasksDoneC3==3)
        {
            LogOff.SetActive(true);
        }
    }

    private IEnumerator Blinking(TextMeshProUGUI _blinkText, float timer)
    {
        float elapsedTime = 0f;
        while (elapsedTime < timer)
        {
            for(int i=1;i<=2;i++)
            {
                _blinkText.enabled = true;
                yield return new WaitForSeconds(0.25f);
                _blinkText.enabled = false;
                yield return new WaitForSeconds(0.25f);
            }

            elapsedTime += 1f;
        }
        
        // Ensure text is off after blinking is complete
        _blinkText.enabled = false;
    }

    private IEnumerator BlinkingPowerup(Image _blinkText)
    {
        float elapsedTime = 0f;
        while (elapsedTime < 1000000)
        {
            for(int i=1;i<=2;i++)
            {
                _blinkText.enabled = true;
                yield return new WaitForSeconds(0.25f);
                _blinkText.enabled = false;
                yield return new WaitForSeconds(0.25f);
            }

            elapsedTime += 1f;
        }
        
        // Ensure text is off after blinking is complete
        _blinkText.enabled = false;
    }

    private void display(GameObject _displayDis)
    {
        _showTasks.SetActive(false);
        _showMotionD.SetActive(false);
        _showAudio.SetActive(false);
        _showVent.SetActive(false);
        _showTask1.SetActive(false);
        _showTask2.SetActive(false);
        _showTask3.SetActive(false);
        _showTask4.SetActive(false);

        _displayDis.SetActive(true);
    }

    private void UpdateTemperature()
    {
        TemperatureTimer+=Time.deltaTime;
        if(TemperatureTimer>1.0f)
        {
            TemperatureTimer=0f;
            if(ComputerOn==false && FanOn==false)
            {
                int cointoss = Random.Range(0, 2);
                if(cointoss==0)Temperature+=0.5f;
            }
            if(ComputerOn==true  && FanOn==false)
            {
                int cointoss = Random.Range(0, 2);
                if(cointoss==0)Temperature+=1.0f;
            }
            if(ComputerOn==false  && FanOn==true)
            {
                int cointoss = Random.Range(0, 2);
                if(cointoss==0)Temperature-=1.25f;
            }
            if(ComputerOn==false  && FanOn==true)
            {
                int cointoss = Random.Range(0, 2);
                if(cointoss==0)Temperature-=1.0f;
            }
            if(SilentVentilationOn==true && Temperature>70)
            {
                int cointoss = Random.Range(0, 2);
                if(cointoss==0)Temperature-=0.5f;
            }
            if(Temperature<60)
                Temperature=60f;
            if(Temperature>=60 && Temperature<80)
                _TemperatureUI.color = new Color32(212,212,212,255);
            if(Temperature>=80 && Temperature<100)
            {
                if(_vignette.intensity.value !=0f)
                    StartCoroutine(LerpVignette(0f,2f));
                if(TemperatureCoroutine!=null)
                    StopCoroutine(TemperatureCoroutine);
                _TemperatureUI.color =new Color32(188,124,75,255);
            }
            if(Temperature>=100 && Temperature<120)
            {
                TemperatureCoroutine = StartCoroutine(Blinking(_TemperatureUI,1000000f));
                _TemperatureUI.color =new Color32(183,16,20,255);
            }
            if(Temperature>120 && notdead==true)
            {
                notdead=false;
                DieFromHeat();
            }
            _TemperatureUI.text = (int)Temperature + "°";
        }
        if(Temperature>=100 && Temperature<120)
        {
            if(_vignette.intensity.value < 0.55)
                _vignette.intensity.value += Time.deltaTime*0.02f;
        }
    }

    private IEnumerator LerpVignette(float targetIntensity, float duration)
    {
        float startIntensity = _vignette.intensity.value;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            _vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _vignette.intensity.value = targetIntensity;
    }

    private void DieFromHeat()
    {
        _firstpersoncontroller.enabled=false;
        _nightScript.enabled=false;

        _imageOverlay.GetComponent<Image>().color = new Color32(175, 0, 0, 255);
        StartCoroutine(FadeOverlay(_imageOverlay,new Color32(175, 0, 0, 255), Color.black, 2f));
        StartCoroutine(LoadSceneAfterDelay("CompleteLabyrinth",5.0f));
    }

    private IEnumerator FadeOverlay(GameObject overlay, Color from, Color to, float duration)
    {
        Image image = overlay.GetComponent<Image>();
        float elapsed = 0f;
        while (elapsed < duration)
        {
            image.color = Color.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        image.color = to;
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    private void MotionD()
    {
        if(MotionDLastRoom!="no" && MotionDOn==true)
        {
            Transform[] children = _MotionDSignalParent.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == MotionDLastRoom)
                {
                    StartCoroutine(MotionDFlicker(child.gameObject.GetComponent<Image>()));
                }
            }
            MotionDLastRoom="no";
        }
    }

    private IEnumerator MotionDFlicker(Image _image)
    {
        _AudioController.PlayOneShot(AudioWork);
        for(int i=1;i<=5;i++)
        {
            _image.enabled = true;
            yield return new WaitForSeconds(0.05f);
            _image.enabled = false;
            yield return new WaitForSeconds(0.05f);
        }
    }

    private void AdFunction()
    {
        if(ComputerOn==true && PlayingAD==false)
            AdTimer+=Time.deltaTime;
        if(AdTimer>20f && ComputerOn==true)
        {
            AdTimer=0f;
            for(int i=0;i<5;i++)
            {
                int cointoss = Random.Range(0, 2);
                if(cointoss==0)
                {
                    if(ADACCEPTED[i]==true && AdTrack[i]==false)
                    {
                        AdTrack[i]=true;
                        lastAdGO=_AD[i];
                        StartCoroutine(PlayAd(_AD[i]));
                        break;
                    }
                }
            }
        }
    }

    private IEnumerator PlayAd(GameObject _AdParent)
    {
        NoiseSilent=0f;
        NoiseLevel+=2f;
        _AudioController.clip=PlayingAd;
        _AudioController.Play();
        PlayingAD=true;
        _showTasks.SetActive(false);
        _showMotionD.SetActive(false);
        _showAudio.SetActive(false);
        _showVent.SetActive(false);
        _showTask1.SetActive(false);
        _showTask2.SetActive(false);
        _showTask3.SetActive(false);
        _showTask4.SetActive(false);
        // KILL ALL OTHER THINGS THAT HAPPEN
        if(currentTaskCoroutine!=null)
        {
            StopCoroutine(currentTaskCoroutine);
            StopCoroutine(BlinkingCoroutine);
            if(NoiseTask==1f)
                NoiseTask-=1f;
            _spinningThingy.GetComponent<AudioSource>().enabled=false;
            _spinningThingy.GetComponent<Image>().enabled=false;
            _spinningThingy.GetComponent<Animator>().enabled=false;
        }
        if(BlinkPowerCoroutine!=null)
            StopCoroutine(BlinkPowerCoroutine);
        
        for(int i=0;i<4;i++)_nightScript.AnimatronicsRST[0]=false;
        _audioLureScript.transform.position = new Vector3(-2.20000005f,-0.798399925f,45.0999985f);
        if(AudioCircle!=null)
            AudioCircle.GetComponent<Image>().enabled=false;

        currentTaskCoroutine = null;
        BlinkingCoroutine = null;
        BlinkPowerCoroutine = null;
        _SilentVentUI.enabled=false;
        
        SilentVentilationOn=false;
        AudioLureOn=false;
        MotionDOn=false;

        _blinkTextmeshpro.enabled=false;
        _MotionDAnim.enabled=false;
        _AudioAnim.enabled=false;
        _VentAnim.enabled=false;

        _MotionDText.text = "Activate motion scanning?";
        _VentText.text = "Activate Silent Ventilation?";
        _AudioLureText.text = "Select a room to activate music.";

        _VentButton.SetActive(true);
        _MotionDButton.SetActive(true);
        if(_animationObject!=null)
        {
            _animationObject.GetComponent<Animator>().enabled=false;
            _animationObject.GetComponent<Image>().enabled=false;
        }

        /// THE ACTUAL AD
        _AdParent.SetActive(true);
        TextMeshProUGUI _Countdown = _AdParent.transform.Find("Countdown").gameObject.GetComponent<TextMeshProUGUI>();
        Image WaitImage = _AdParent.transform.Find("CountdownText").gameObject.GetComponent<Image>();
        Image SkipButton = _AdParent.transform.Find("SkipButton").gameObject.GetComponent<Image>();
        Collider SkipButtonCollider = _AdParent.transform.Find("SkipButton").gameObject.GetComponent<Collider>();
        _Countdown.text="5";
        yield return new WaitForSeconds(1.0f);
        _Countdown.text="4";
        yield return new WaitForSeconds(1.0f);
        _Countdown.text="3";
        yield return new WaitForSeconds(1.0f);
        _Countdown.text="2";
        yield return new WaitForSeconds(1.0f);
        _Countdown.text="1";
        yield return new WaitForSeconds(1.0f);
        _Countdown.enabled=false;
        WaitImage.enabled=false;
        SkipButton.enabled=true;
        SkipButtonCollider.enabled=true;
    }
}

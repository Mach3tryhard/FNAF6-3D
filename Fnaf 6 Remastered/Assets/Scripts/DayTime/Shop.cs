using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    public string[] itemName;
    public Sprite[] itemImage;
    public GameObject[] itemPrefab;
    public int[] itemPrice;
    public int[] itemStats;
    public bool[] itemBought;

    public int I=1,J=0;

    public float rayDistance = 5f; // Max distance for the raycast
    public LayerMask layerMask;

    // GAME

    private BuildManager BuildManagerScript;
    private Daytime_Mechnanics DaytimeScript;
    private int[] PozforI;
    private string[] DayString;

    ///UI 

    private TextMeshProUGUI DayText;

    private Image Background1;
    private Image Background2;
    private Image Background3;

    private Animator[] ShopAnimators;

    private GameObject[] BlockShop;

    private GameObject[] NewShopParticles;
    private Image NewShopImage;
    private GameObject[] NewShopAnimation;
    private TextMeshProUGUI[] StatDisplayer;
    private Slider[] StatBarDisplay;
    private GameObject RiskParent;
    private TextMeshProUGUI MoneyDisplayer;
    private TextMeshProUGUI NameDisplayer;
    private Image ImageDisplayer;
    private GameObject SoldOutImage;

    private GameObject BuyButtonDisplay;
    private GameObject ButtonArrowRight;
    private GameObject ButtonArrowLeft;

    ///SOUNDS
    public AudioClip ShopChoose;
    public AudioClip ItemChoose;
    public AudioClip ItemBuy;
    public AudioClip ShopUnlocked;

    private AudioSource PlayCatalogEffects;

    ///ANIMATIONS
    private int HelpyAnimIndex=0;
    private Animator HelpyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        NewShopParticles = new GameObject[3];
        NewShopAnimation = new GameObject[3];
        DayString = new string[8];
        DayString[1]="Monday";DayString[2]="Tuesday";DayString[3]="Wednesday";DayString[4]="Thursday";DayString[5]="Friday";DayString[6]="Saturday";
        ShopAnimators = new Animator[4];
        PozforI = new int[4];
        PozforI[0]=14;PozforI[1]=29;PozforI[2]=44;PozforI[3]=59;
        StatDisplayer = new TextMeshProUGUI[6];
        StatBarDisplay = new Slider[6];
        BlockShop = new GameObject[3];

        BuildManagerScript = GameObject.Find("Building_Manager").GetComponent<BuildManager>();
        DaytimeScript = GameObject.Find("DayTimeMechanics").GetComponent<Daytime_Mechnanics>();
        StatDisplayer = new TextMeshProUGUI[60];

        //UI
        NewShopImage = GameObject.Find("NewCatalogUnlocked").GetComponent<Image>();
        BuyButtonDisplay = GameObject.Find("BuyButtonDisplay");
        MoneyDisplayer = GameObject.Find("ItemPriceDisplay").GetComponent<TextMeshProUGUI>();
        NameDisplayer = GameObject.Find("ItemNameDisplay").GetComponent<TextMeshProUGUI>();
        ImageDisplayer = GameObject.Find("ItemImageDisplay").GetComponent<Image>(); 
        SoldOutImage =  GameObject.Find("SoldOutDisplay");

        ButtonArrowRight = GameObject.Find("ButtonArrowRight");
        ButtonArrowLeft = GameObject.Find("ButtonArrowLeft");
        ButtonArrowLeft.SetActive(false);

        SoldOutImage.SetActive(false);

        Background1 = GameObject.Find("BackgroundImage1").GetComponent<Image>();
        Background2 = GameObject.Find("BackgroundImage2").GetComponent<Image>();

        for(int i=2;i<=4;i++)
        {
            NewShopParticles[i-2] = GameObject.Find("ShopUnlockPart"+i);
            NewShopParticles[i-2].SetActive(false);
        }
        for(int i=2;i<=4;i++)
        {
            NewShopAnimation[i-2] = GameObject.Find("AnimShop"+i);
        }
        for(int i=1;i<=6;i++)
        {
            StatDisplayer[i-1] = GameObject.Find("StatDisplay"+i).GetComponent<TextMeshProUGUI>();
        }
        for(int i=1;i<=6;i++)
        {
            StatBarDisplay[i-1] = GameObject.Find("SliderStat"+i).GetComponent<Slider>();
        }
        for(int i=1;i<=3;i++)
        {
            BlockShop[i-1] = GameObject.Find("BlockShop"+i);
        }
        DayText = GameObject.Find("DayText").GetComponent<TextMeshProUGUI>();
        RiskParent = GameObject.Find("RiskShower");
        RiskParent.SetActive(false);
        ///SOUNDS

        PlayCatalogEffects = GameObject.Find("PlayCatalogEffects").GetComponent<AudioSource>();

        ///ANIMATIONS

        for(int i=1;i<=4;i++)
        {
            ShopAnimators[i-1] = GameObject.Find("ButtonShop"+i).GetComponent<Animator>();
        }

        HelpyAnimator = GameObject.Find("HelpyDisplay").GetComponent<Animator>();

        ///GAME SETUP

        DayText.text = DayString[DaytimeScript.CurrentDay];
        StartCoroutine(HelpyAnimationTimer());
        ChangeShop(0,ShopAnimators[0].gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);

            Camera mainCamera = Camera.main;

            Ray ray = mainCamera.ScreenPointToRay(screenCenter);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayDistance, layerMask, QueryTriggerInteraction.Collide))
            {
                BoxCollider boxCollider = hit.collider as BoxCollider;
                if (boxCollider != null)
                {
                    ButtonUse(hit.collider);
                    Debug.Log("Hit Box Collider: " + hit.collider.name);
                }
            }

        }
    }

    private void ButtonUse(Collider Button)
    {
        for(int i=1;i<=4;i++)
        {
            if(Button.name== "ButtonShop"+i)
            {
                ChangeShop(i-1,Button.gameObject);
            }
        }
        if(Button.name == "ButtonArrowLeft")
        {
            J--;
            PlayCatalogEffects.PlayOneShot(ItemChoose);
            ///Change Statistics
            DisplayStatsFunc(I*15+J);
            DisplayNamePriceImage(I*15+J);
            DisplayBuyUpdate(I*15+J);

            ButtonArrowRight.SetActive(true);
            if(J==0)
            {
                ButtonArrowLeft.SetActive(false);
            }
        }
        if(Button.name == "ButtonArrowRight")
        {
            J++;
            PlayCatalogEffects.PlayOneShot(ItemChoose);
            ///Change Statistics
            DisplayStatsFunc(I*15+J);
            DisplayNamePriceImage(I*15+J);
            DisplayBuyUpdate(I*15+J);

            ButtonArrowLeft.SetActive(true);
            if(J+I*15==PozforI[I])
            {
                ButtonArrowRight.SetActive(false);
            }
        }
        if(Button.name == "BuyButtonDisplay")
        {
            BuyMechanic(I*15+J);
            DisplayBuyUpdate(I*15+J);
        }
    }

    private void BuyMechanic(int index)
    {
        PlayCatalogEffects.clip = ItemBuy;
        PlayCatalogEffects.Play();
        DaytimeScript.SpentMoney+=itemPrice[index];
        DaytimeScript.CashMoney-=itemPrice[index];
        BuildManagerScript.OwnedObjects = BuildManagerScript.AddObject(BuildManagerScript.OwnedObjects,itemPrefab[index]);
        BuildManagerScript.ChangeddisplayPictures = BuildManagerScript.AddObjectSprite(BuildManagerScript.ChangeddisplayPictures,itemPrefab[index].GetComponent<Image>().sprite);
        itemBought[index]=true;

        ///FIX INDEX POSITION FOR IMAGE
        if(BuildManagerScript.OwnedObjects.Length>1)
            BuildManagerScript.index = 1;
        else
            BuildManagerScript.index = 0;

        ///UPGRADE SHOP STATE
        if(DaytimeScript.SpentMoney>=100 && BlockShop[0].activeInHierarchy)
        {
            PlayCatalogEffects.PlayOneShot(ShopUnlocked);
            BlockShop[0].SetActive(false);
            StartCoroutine(ShopUnlockAnim(0));
        }
        if(DaytimeScript.SpentMoney>=1000 && BlockShop[1].activeInHierarchy)
        {
            PlayCatalogEffects.PlayOneShot(ShopUnlocked);
            BlockShop[1].SetActive(false);
            StartCoroutine(ShopUnlockAnim(1));
        }
        if(DaytimeScript.SpentMoney>=3000 && BlockShop[2].activeInHierarchy)
        {
            PlayCatalogEffects.PlayOneShot(ShopUnlocked);
            BlockShop[2].SetActive(false);
            StartCoroutine(ShopUnlockAnim(2));
        }
    }

    private void DisplayBuyUpdate(int index)
    {
        if(itemBought[index]==true)
        {
            SoldOutImage.SetActive(true);
            BuyButtonDisplay.SetActive(false);
        }
        else
        {
            SoldOutImage.SetActive(false);
            BuyButtonDisplay.SetActive(true);
        }
        if( DaytimeScript.CashMoney<itemPrice[index])
        {
            BuyButtonDisplay.SetActive(false);
        }
    }

    private void ChangeShop(int index,GameObject hitobject)
    {
        ///Buton animation
        if(index!=I)
        {
            J=0;
            PlayCatalogEffects.PlayOneShot(ShopChoose);
            int animint=I+1;
            ShopAnimators[I].CrossFade("ButtonSlideBack"+animint,0f);
            //ShopAnimators[I].SetInteger("ButtonAnimState"+animint,0);
            animint=index+1;
            ShopAnimators[index].CrossFade("ButtonSlide"+animint,0f);
            //ShopAnimators[index].SetInteger("ButtonAnimState"+animint,1);
            I=index;
        }
        ///Change Background
        Background1.color = hitobject.GetComponent<Image>().color;
        Background2.color = hitobject.GetComponent<Image>().color;
        BuyButtonDisplay.GetComponent<Image>().color = hitobject.GetComponent<Image>().color;
        ///Change Statistics
        DisplayStatsFunc(I*15+J);
        DisplayNamePriceImage(I*15+J);
        DisplayBuyUpdate(I*15+J);
        UpdateArrows();
    }

    private void DisplayNamePriceImage(int index)
    {
        MoneyDisplayer.text = "$"+itemPrice[index];
        NameDisplayer.text = itemName[index];
        ImageDisplayer.sprite = itemImage[index];
    }

    private void UpdateArrows()
    {
        ButtonArrowRight.SetActive(true);
        ButtonArrowLeft.SetActive(true);
        if(J==0)
        {
            ButtonArrowLeft.SetActive(false);
        }
        if(J+I*15==PozforI[I])
        {
            ButtonArrowRight.SetActive(false);
        }
    }

    private void DisplayStatsFunc(int index)
    {
        int Statistics = itemStats[index];
        for(int i=5;i>=0;i--)
        {
            StatDisplayer[i].text = ""+Statistics%10;
            StatBarDisplay[i].value = Statistics%10;
            Statistics=Statistics/10;
        }
        if(StatDisplayer[5].text=="0")
        {
            RiskParent.SetActive(false);
        }
        else
        {
            RiskParent.SetActive(true);
        }
    }

    private IEnumerator HelpyAnimationTimer()
    {
        yield return new WaitForSeconds(5f);
        HelpyAnimIndex = (HelpyAnimIndex+1)%5;
        HelpyAnimator.SetInteger("CurrentDance", HelpyAnimIndex);
        StartCoroutine(HelpyAnimationTimer());
    }

    private IEnumerator ShopUnlockAnim(int index)
    {
        NewShopParticles[index].SetActive(true);
        NewShopParticles[index].GetComponent<ParticleSystem>().Play();
        NewShopImage.enabled=true;
        NewShopAnimation[index].GetComponent<Image>().enabled=true;
        int caca=index+2;
        NewShopAnimation[index].GetComponent<Animator>().Play("ShopAnim"+caca);
        yield return new WaitForSeconds(2f);
        NewShopAnimation[index].GetComponent<Image>().enabled=false;
        NewShopImage.enabled=false;
        NewShopParticles[index].SetActive(false);
    }
}

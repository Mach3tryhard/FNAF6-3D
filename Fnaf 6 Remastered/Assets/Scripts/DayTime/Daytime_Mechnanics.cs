using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Daytime_Mechnanics : MonoBehaviour
{
    public int FazRating = 0;
    public int CashMoney = 1000000;
    public int SpentMoney=0;
    public int playTokens=0;
    public int TOTALAtmosphere=0;
    public int TOTALHealthANDSafety=0;
    public int TOTALEntertainment=0;
    public int TOTALBonusRevenue=0;
    public int TOTALRisk=0;
    public int TOTALHappiness=0;
    public int Upgrades=1;
    public int CurrentDay=1;
    public int[] TakenAds;

    public Material[] FloorMaterials;
    public Material[] WallMaterials;

    public GameObject[] Table2Package;
    public GameObject[] Table3Package;

    public GameObject Confetti;
    public GameObject DecorativeBaloons;

    private MeshRenderer[] WallsOfMainRoom;
    private MeshRenderer FloorRoomMain;

    private BuildManager BuildingManager;
    private Daytime_Mechnanics DaytimeScript;

    private TextMeshProUGUI[] StatDisplay;
    
    //UI ELEMENTS

    private GameObject AdAcceptButton;
    private int[] AdPaymentSum;
    private TextMeshProUGUI FloorplanCost;
    private GameObject UpgradeParent;
    private TextMeshProUGUI MoneyUI;
    private TextMeshProUGUI PlayTokensUI;

    private AudioSource PlayBuildingEffects;
    public AudioClip UpgradingFloor;
    public AudioClip TwinkeAudio;

    void Start()
    {
        AdPaymentSum = new int[5];
        AdPaymentSum[0] = 250;AdPaymentSum[1] = 500;AdPaymentSum[2] = 1000;AdPaymentSum[3] = 2500;AdPaymentSum[4] = 5000;
        TakenAds = new int[5];
        PlayBuildingEffects = GameObject.Find("PlayBuildingEffects").GetComponent<AudioSource>();
        StatDisplay = new TextMeshProUGUI[6];
        WallsOfMainRoom = new MeshRenderer[5];

        FloorRoomMain = GameObject.Find("FloorBuilding").GetComponent<MeshRenderer>();
        AdAcceptButton = GameObject.Find("AdAcceptButton");
        FloorplanCost = GameObject.Find("FloorplanCost").GetComponent<TextMeshProUGUI>();
        UpgradeParent = GameObject.Find("UpgradeParent");
        PlayTokensUI = GameObject.Find("TokenNumber").GetComponent<TextMeshProUGUI>();
        MoneyUI = GameObject.Find("MoneyTextDisplay").GetComponent<TextMeshProUGUI>();
        BuildingManager = GameObject.Find("Building_Manager").GetComponent<BuildManager>();

        for(int i=1;i<=6;i++)
        {
            StatDisplay[i-1] = GameObject.Find("TotalStat"+i).GetComponent<TextMeshProUGUI>();
        }
        for(int i=1;i<=5;i++)
        {
            WallsOfMainRoom[i-1] = GameObject.Find("WallBuilding"+i).GetComponent<MeshRenderer>();
        }

        if(CurrentDay!=6)
        {
            AdAcceptButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMoney();
        UpdatePlayTokens();
        if(Input.GetKeyDown(KeyCode.M) && TakenAds[CurrentDay-1]==0 && CurrentDay!=6)
        {
            PlayBuildingEffects.PlayOneShot(TwinkeAudio);
            AdAcceptButton.SetActive(false);
            TakenAds[CurrentDay-1]=1;
            CashMoney+=AdPaymentSum[CurrentDay-1];
        }
        int comp=0;
        if(Upgrades==1)comp=250;
        if(Upgrades==2)comp=1000;
        if(Input.GetKeyDown(KeyCode.U) && CashMoney>comp && Upgrades<3)
        {
            Upgrades++;
            if(Upgrades==2)
            {
                Confetti.SetActive(true);
                for(int i=0;i<Table2Package.Length;i++)
                {
                    BuildingManager.OwnedObjects = BuildingManager.AddObject(BuildingManager.OwnedObjects,Table2Package[i]);
                    BuildingManager.ChangeddisplayPictures = BuildingManager.AddObjectSprite(BuildingManager.ChangeddisplayPictures,Table2Package[i].GetComponent<Image>().sprite);
                    BuildingManager.index++;
                }
                FloorplanCost.text = "COST: $"+1000;
                CashMoney-=250;
                SpentMoney+=250;
                PlayBuildingEffects.PlayOneShot(UpgradingFloor);
            }
            if(Upgrades==3)
            {
                DecorativeBaloons.SetActive(true);
                for(int i=0;i<Table3Package.Length;i++)
                {
                    BuildingManager.OwnedObjects = BuildingManager.AddObject(BuildingManager.OwnedObjects,Table3Package[i]);
                    BuildingManager.ChangeddisplayPictures = BuildingManager.AddObjectSprite(BuildingManager.ChangeddisplayPictures,Table3Package[i].GetComponent<Image>().sprite);
                    BuildingManager.index++;
                }
                CashMoney-=1000;
                SpentMoney+=Upgrades*200;
                PlayBuildingEffects.PlayOneShot(UpgradingFloor);
            }
            UpgradeFloor();
            if(Upgrades==3)
            {
                UpgradeParent.SetActive(false);
            }
        }
    }

    public void UpdateMoney()
    {
        MoneyUI.text = ""+CashMoney;
    }

    public void UpdatePlayTokens()
    {
        PlayTokensUI.text = ""+playTokens;
    }

    public void UpdateTotalStats()
    {
        TOTALAtmosphere=0;
        TOTALHealthANDSafety=0;
        TOTALEntertainment=0;
        TOTALBonusRevenue=0;
        TOTALRisk=0;
        TOTALHappiness=0;
        int ConvertBool;
        for(int i=0;i<BuildingManager.PlacedObjects.Length;i++)
        {
            ObjectInfo IndexObjectInfo = BuildingManager.PlacedObjects[i].GetComponent<ObjectInfo>();
            ConvertBool = IndexObjectInfo.PolishandClean ? 1 : 0;
            TOTALAtmosphere=TOTALAtmosphere+IndexObjectInfo.Atmosphere+ConvertBool;
            TOTALHealthANDSafety+=IndexObjectInfo.HealthANDSafety;
            TOTALEntertainment+=IndexObjectInfo.Entertainment;
            ConvertBool = IndexObjectInfo.AddedCoinslot ? 1 : 0;
            TOTALBonusRevenue=TOTALBonusRevenue+IndexObjectInfo.BonusRevenue+ConvertBool;
            ConvertBool = IndexObjectInfo.AddedSfateryStraps ? 1 : 0;
            if(IndexObjectInfo.Risk-ConvertBool<0)
                TOTALRisk+=0;
            else
                TOTALRisk=TOTALRisk+IndexObjectInfo.Risk-ConvertBool;
            TOTALHappiness+=IndexObjectInfo.Happiness;
        }
        StatDisplay[0].text=""+TOTALAtmosphere;
        StatDisplay[1].text=""+TOTALHealthANDSafety;
        StatDisplay[2].text=""+TOTALEntertainment;
        StatDisplay[3].text=""+TOTALBonusRevenue;
        StatDisplay[4].text=""+TOTALHappiness;
        StatDisplay[5].text=""+TOTALRisk;
    }

    private void UpgradeFloor()
    {
        // Modify the floor material
        Material[] floorMaterials = FloorRoomMain.materials;  // Clone the floor's material array
        floorMaterials[0] = FloorMaterials[Upgrades - 1];     // Apply the upgraded material
        FloorRoomMain.materials = floorMaterials;             // Reassign the modified array

        // Modify the first wall
        Material[] materialsarray = WallsOfMainRoom[0].materials;  // Clone the materials
        materialsarray[1] = WallMaterials[Upgrades - 1];          // Modify the material
        WallsOfMainRoom[0].materials = materialsarray;             // Reassign the modified array

        // Modify the second wall
        materialsarray = WallsOfMainRoom[1].materials;        // Clone the materials
        materialsarray[1] = WallMaterials[Upgrades - 1];     // Modify the material
        WallsOfMainRoom[1].materials = materialsarray;        // Reassign the modified array

        // Modify walls from index 2 to 8
        for (int i = 2; i < 5; i++)
        {
            materialsarray = WallsOfMainRoom[i].materials;    // Clone the materials
            materialsarray[0] = WallMaterials[Upgrades - 1]; // Modify the material
            WallsOfMainRoom[i].materials = materialsarray;    // Reassign the modified array
        }
    }
}

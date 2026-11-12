using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectManager : MonoBehaviour
{
    public GameObject selectedObject;
    private BuildManager buildingManager;
    private Daytime_Mechnanics DaytimeScript;

    public GameObject SmokePoof;

    private Collider ItemCollider;
    private MeshRenderer ItemSizeViewer;

    ///UI STUFF

    private ParticleSystem CleanAndPolishPart;
    private ParticleSystem UpgradesPart;

    public Button[] HoverButtons;
    public GameObject HoverPARENT;
    private TextMeshProUGUI[] HoverText;

    private AudioSource PlayBuildingEffects;
    public AudioClip SelectingAudio;
    public AudioClip UpgradeAudio;
    public AudioClip TwinkeAudio;
    public AudioClip RemoveAudio;

    void Start()
    {
        PlayBuildingEffects = GameObject.Find("PlayBuildingEffects").GetComponent<AudioSource>();

        DaytimeScript = GameObject.Find("DayTimeMechanics").GetComponent<Daytime_Mechnanics>();
        buildingManager = GameObject.Find("Building_Manager").GetComponent<BuildManager>();

        ///UI STUFF
        HoverButtons = new Button[6];
        HoverText = new TextMeshProUGUI[5];

        for(int i=1;i<=6;i++)
        {
            HoverButtons[i-1] = GameObject.Find("ButtonHover"+i).GetComponent<Button>();
        }
        for(int i=1;i<=5;i++)
        {
            HoverText[i-1] = GameObject.Find("HoverInfo"+i).GetComponent<TextMeshProUGUI>();
        }

        HoverPARENT = GameObject.Find("HoverParent");
        HoverPARENT.SetActive(false);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,1000))
        {
            if(hit.collider.gameObject.CompareTag("Placeable") && buildingManager.isBuidling==false)
            {
                Select(hit.collider.gameObject);
                SelectUI();
            }
            else
            {
                HoverPARENT.SetActive(false);
                Deselect();
            }
        }
        if( Input.GetKeyDown(KeyCode.F) && buildingManager.isBuidling==false && DaytimeScript.CashMoney>=100 && selectedObject!=null)
        {
            for(int i=0;i<buildingManager.PlacedObjects.Length;i++)
            {
                if(buildingManager.PlacedObjects[i].name+"(Clone)"==selectedObject.name)
                {
                    PlayBuildingEffects.PlayOneShot(TwinkeAudio);
                    buildingManager.PlacedObjects[i].GetComponent<ObjectInfo>().PolishandClean=true;
                }
            }
            StartCoroutine(CleanParticles());
            DaytimeScript.CashMoney-=100;
            selectedObject.GetComponent<ObjectInfo>().PolishandClean=true;
            HoverButtons[0].interactable = false;
            SelectUI();
            DaytimeScript.UpdateTotalStats();
        }
        if( Input.GetKeyDown(KeyCode.C) && buildingManager.isBuidling==false && DaytimeScript.CashMoney>=100 && selectedObject!=null)
        {
            for(int i=0;i<buildingManager.PlacedObjects.Length;i++)
            {
                if(buildingManager.PlacedObjects[i].name+"(Clone)"==selectedObject.name)
                {
                    PlayBuildingEffects.PlayOneShot(UpgradeAudio);
                    buildingManager.PlacedObjects[i].GetComponent<ObjectInfo>().AddedCoinslot=true;
                }
            }
            StartCoroutine(UpgradeParticles());
            DaytimeScript.CashMoney-=100;
            selectedObject.GetComponent<ObjectInfo>().AddedCoinslot=true;
            HoverButtons[1].interactable = false;
            SelectUI();
            DaytimeScript.UpdateTotalStats();
        }
        if( Input.GetKeyDown(KeyCode.V) && buildingManager.isBuidling==false  && DaytimeScript.CashMoney>=50 && selectedObject!=null)
        {
            for(int i=0;i<buildingManager.PlacedObjects.Length;i++)
            {
                if(buildingManager.PlacedObjects[i].name+"(Clone)"==selectedObject.name)
                {
                    PlayBuildingEffects.PlayOneShot(UpgradeAudio);
                    buildingManager.PlacedObjects[i].GetComponent<ObjectInfo>().AddedSfateryStraps=true;
                }
            }
            StartCoroutine(UpgradeParticles());
            DaytimeScript.CashMoney-=50;
            selectedObject.GetComponent<ObjectInfo>().AddedSfateryStraps=true;
            HoverButtons[2].interactable = false;
            SelectUI();
            DaytimeScript.UpdateTotalStats();
        }
        if( Input.GetKeyDown(KeyCode.Z) && buildingManager.isBuidling==false && selectedObject!=null)
        {
            PlayBuildingEffects.PlayOneShot(RemoveAudio);

            //Transform childTransform = selectedObject.transform.Find("SmokePoof");
            //SmokePoof = childTransform.gameObject.GetComponent<ParticleSystem>();
            StartCoroutine(RemoveObjectWithEffect(selectedObject,SmokePoof));
            //SmokePoof.Play();

            Destroy(buildingManager.pendingObject);
            buildingManager.isBuidling=true;

            Move();

            Transform childTransform = selectedObject.transform.Find("ColliderAndSize");
            if(childTransform!=null)
            {
                ItemCollider = childTransform.gameObject.GetComponent<BoxCollider>();
                ItemCollider.enabled=false;
            }

            for(int i=0;i<buildingManager.PlacedObjects.Length;i++)
            {
                
                if(buildingManager.PlacedObjects[i].name+"(Clone)"==selectedObject.name)
                {
                    buildingManager.OwnedObjects = buildingManager.AddObject(buildingManager.OwnedObjects,buildingManager.PlacedObjects[i]);
                    buildingManager.PlacedObjects = buildingManager.RemoveObject(buildingManager.PlacedObjects,buildingManager.PlacedObjects[i]);
                }
            }

            buildingManager.index = buildingManager.OwnedObjects.Length-1;
            
            buildingManager.ChangeddisplayPictures = buildingManager.AddObjectSprite(buildingManager.ChangeddisplayPictures,selectedObject.GetComponent<Image>().sprite);

            DaytimeScript.UpdateTotalStats();
            buildingManager.BuildingDisplay();
            buildingManager.UpdateDisplay();
        }
        if( Input.GetKeyDown(KeyCode.X) && buildingManager.isBuidling==false && selectedObject!=null)
        {
            PlayBuildingEffects.PlayOneShot(RemoveAudio);

            //Transform childTransform = selectedObject.transform.Find("SmokePoof");
            //SmokePoof = childTransform.gameObject.GetComponent<ParticleSystem>();
            StartCoroutine(RemoveObjectWithEffect(selectedObject,SmokePoof));
            //SmokePoof.Play();

            for(int i=0;i<buildingManager.PlacedObjects.Length;i++)
            {
                
                if(buildingManager.PlacedObjects[i].name+"(Clone)"==selectedObject.name)
                {
                    buildingManager.OwnedObjects = buildingManager.AddObject(buildingManager.OwnedObjects,buildingManager.PlacedObjects[i]);
                    buildingManager.PlacedObjects = buildingManager.RemoveObject(buildingManager.PlacedObjects,buildingManager.PlacedObjects[i]);
                }
            }

            buildingManager.index = buildingManager.OwnedObjects.Length-1;
            
            buildingManager.ChangeddisplayPictures = buildingManager.AddObjectSprite(buildingManager.ChangeddisplayPictures,selectedObject.GetComponent<Image>().sprite);

            DaytimeScript.UpdateTotalStats();
            Delete();
        }
    }

    private void Select(GameObject obj)
    {
        if(obj == selectedObject)
            return ;
        else
            PlayBuildingEffects.PlayOneShot(SelectingAudio);
        if(selectedObject!=null)
            Deselect();
        Outline outline = obj.GetComponent<Outline>();
        if(outline == null) obj.AddComponent<Outline>();
        else outline.enabled = true;

        selectedObject = obj;
    }

    private void SelectUI()
    {
        ObjectInfo InfoScript = selectedObject.GetComponent<ObjectInfo>();
        HoverPARENT.SetActive(true);
        int ConvertBool;
        ConvertBool = InfoScript.AddedSfateryStraps ? 1 : 0;
        if(InfoScript.Risk==0)
            HoverText[0].text = "RISK:  " + InfoScript.Risk;
        else
            HoverText[0].text = "RISK:  " + (InfoScript.Risk-ConvertBool);
        ConvertBool = InfoScript.AddedCoinslot ? 1 : 0;
        HoverText[1].text = "REVE:  " + (InfoScript.BonusRevenue+ConvertBool);
        HoverText[2].text = "ENTE:  " + InfoScript.Entertainment;
        HoverText[3].text = "H&SA:  " + InfoScript.HealthANDSafety;
        ConvertBool = InfoScript.PolishandClean ? 1 : 0;
        HoverText[4].text = "ATMO:  " + (InfoScript.Atmosphere+ConvertBool);

        HoverButtons[0].interactable = !InfoScript.PolishandClean;
        HoverButtons[1].interactable = !InfoScript.AddedCoinslot;
        HoverButtons[2].interactable = !InfoScript.AddedSfateryStraps;
    }

    private void Deselect()
    {
        if(selectedObject!=null)
            selectedObject.GetComponent<Outline>().enabled=false;
        selectedObject = null;
    }

    public void Move()
    {
        Transform childTransform = selectedObject.transform.Find("ColliderAndSize");
        if(childTransform!=null)
        {
            ItemCollider = childTransform.gameObject.GetComponent<BoxCollider>();
            ItemSizeViewer = childTransform.gameObject.GetComponent<MeshRenderer>();
            ItemSizeViewer.enabled=true;
        }
        // Use the prefab reference instead of the instantiated clone
        GameObject objectPrefab = selectedObject;  // Replace with your logic to get the prefab reference
        buildingManager.pendingObject = objectPrefab;
    }

    public void Delete()
    {
        GameObject objToDestroy = selectedObject;
        Deselect();
        Destroy(objToDestroy);
    }

    private IEnumerator RemoveObjectWithEffect(GameObject objectToRemove, GameObject particleEffect)
    {
        // Clone the ParticleSystem at the object's position
        GameObject clonedEffect = Instantiate(particleEffect, objectToRemove.transform.position, Quaternion.identity);

        clonedEffect.GetComponent<ParticleSystem>().Play();

        // Wait for the particle effect to finish
        yield return new WaitForSeconds(clonedEffect.GetComponent<ParticleSystem>().main.duration/* + clonedEffect.main.startLifetime.constantMax*/);

        // Destroy the cloned particle effect once it has finished
        Destroy(clonedEffect);
    }

    private IEnumerator CleanParticles()
    {
        Transform childTransform = selectedObject.transform.Find("CleanAndPolishPart");
        CleanAndPolishPart = childTransform.gameObject.GetComponent<ParticleSystem>();
        CleanAndPolishPart.Play();
        yield return new WaitForSeconds(2f);
        CleanAndPolishPart.Stop();
    }

    private IEnumerator UpgradeParticles()
    {
        Transform childTransform = selectedObject.transform.Find("UpgradesPart");
        UpgradesPart = childTransform.gameObject.GetComponent<ParticleSystem>();
        UpgradesPart.Play();
        yield return new WaitForSeconds(2f);
        UpgradesPart.Stop();
    }
}

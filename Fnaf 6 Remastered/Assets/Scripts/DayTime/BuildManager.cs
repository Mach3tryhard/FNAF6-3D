using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildManager : MonoBehaviour
{
    public GameObject[] OwnedObjects;
    public GameObject[] PlacedObjects;
    public Sprite[] ChangeddisplayPictures;

    public GameObject pendingObject;
    public int index = 1;

    private Vector3 pos;

    private RaycastHit hit;
    [SerializeField] private LayerMask layerMask;

    public float rotateAmount = 45;
    public float gridSize;
    bool gridOn = true;

    public bool canPlace = true;

    public bool isBuidling = true;

    private bool isRatingAnimating = false;

    public Material HitBoxMat;

    private Image[] ImageDisplay;
    private Image[] BorderDisplay;
    private Image buildCrosshair;
    private Image NormalCrosshair;
    private TextMeshProUGUI buildText;
    private TextMeshProUGUI GeneralText;

    private BoxCollider ItemCollider;
    private MeshRenderer ItemSizeViewer;

    private Daytime_Mechnanics DaytimeScript;

    /// UI BULLSHIT

    private GameObject PerformanceBonusDisplay;

    private TextMeshProUGUI FazratingTextDisplay;
    private GameObject FazRatingImageDisplay;

    private TextMeshProUGUI BadWarning;
    private TextMeshProUGUI GoodWarning;
    private TextMeshProUGUI MidWarning;

    private GameObject FireWorksParticles;
    private ParticleSystem StartParticles1000;

    private ParticleSystem StartParticlesItem;

    private AudioSource PlayBuildingEffects;
    public AudioClip SwitchingAudio;
    public AudioClip PlaceAudio;
    public AudioClip NotPossibleAudio;

    void Start()
    {
        PerformanceBonusDisplay = GameObject.Find("PerformanceBonusDisplay");
        PerformanceBonusDisplay.SetActive(false);
        FireWorksParticles = GameObject.Find("FireWorksParticles");
        FireWorksParticles.SetActive(false);
        StartParticles1000 = GameObject.Find("StartParticles1000").GetComponent<ParticleSystem>();
        
        FazratingTextDisplay = GameObject.Find("TextFazRatingDisplay").GetComponent<TextMeshProUGUI>();
        FazRatingImageDisplay = GameObject.Find("FazRatingDisplay");

        PlayBuildingEffects = GameObject.Find("PlayBuildingEffects").GetComponent<AudioSource>();
        DaytimeScript = GameObject.Find("DayTimeMechanics").GetComponent<Daytime_Mechnanics>();
        BorderDisplay = new Image[3];
        ImageDisplay = new Image[3];
        GeneralText = GameObject.Find("GeneralInfo").GetComponent<TextMeshProUGUI>();
        buildText = GameObject.Find("BuildInfo").GetComponent<TextMeshProUGUI>();
        buildCrosshair = GameObject.Find("BuildCrosshair").GetComponent<Image>();
        NormalCrosshair = GameObject.Find("Crosshair").GetComponent<Image>();
        BorderDisplay[0] = GameObject.Find("Border1").GetComponent<Image>();
        BorderDisplay[1] = GameObject.Find("Border2").GetComponent<Image>();
        BorderDisplay[2] = GameObject.Find("Border3").GetComponent<Image>();
        ImageDisplay[0] = GameObject.Find("ObjectImage1").GetComponent<Image>();
        ImageDisplay[1] = GameObject.Find("ObjectImage2").GetComponent<Image>();
        ImageDisplay[2] = GameObject.Find("ObjectImage3").GetComponent<Image>();

        ///UI BULLSHIT
        BadWarning = GameObject.Find("WarningBad").GetComponent<TextMeshProUGUI>();
        MidWarning = GameObject.Find("WarningMid").GetComponent<TextMeshProUGUI>();
        GoodWarning = GameObject.Find("WarningGood").GetComponent<TextMeshProUGUI>();

        FazRatingImageDisplay.SetActive(false);
    }

    void Update()
    {
        // Toggle build mode
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isBuidling = !isBuidling;
            BuildingDisplay();
            UpdateDisplay();

            if (isBuidling)
            {
                // Reinstantiate the visual representation when re-entering build mode
                if (pendingObject == null)
                {
                    SelectObject(index);
                }
            }
            else
            {
                canPlace = true;
                // If exiting build mode, remove the visual representation
                if (pendingObject != null)
                {
                    Destroy(pendingObject);
                    pendingObject = null;
                }
                return;  // Exit the method early to avoid placing objects
            }
        }

        if (isBuidling)
        {
            if (pendingObject != null)
            {
                if (gridOn)
                {
                    pendingObject.transform.position = new Vector3(
                        RoundToNearestGrid(pos.x),
                        pos.y,
                        RoundToNearestGrid(pos.z)
                    );
                }
                else
                {
                    pendingObject.transform.position = pos;
                }

                if (Input.GetKeyDown(KeyCode.E) && canPlace && index > 0)
                {
                    PlaceObject();
                }
                if (Input.GetKeyDown(KeyCode.E) && !canPlace)
                {
                    PlayBuildingEffects.PlayOneShot(NotPossibleAudio);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow) && index < OwnedObjects.Length - 1)
            {
                PlayBuildingEffects.PlayOneShot(SwitchingAudio);
                index++;
                UpdateDisplay();
                SwitchObject(index);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) && index > 1)
            {
                PlayBuildingEffects.PlayOneShot(SwitchingAudio);
                index--;
                UpdateDisplay();
                SwitchObject(index);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                gridOn = !gridOn;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateObject();
            }

            UpdateWarnings();
        }
    }

    public void PlaceObject()
    {
        if(ItemSizeViewer!=null)
        {
            ItemSizeViewer.enabled=false;
        }
        Transform childTransform = pendingObject.transform.Find("ColliderAndSize");
        if(childTransform!=null)
        {
            ItemCollider = childTransform.gameObject.GetComponent<BoxCollider>();
            ItemSizeViewer = childTransform.gameObject.GetComponent<MeshRenderer>();
            ItemSizeViewer.enabled=false;
        }
        if(OwnedObjects[index].GetComponent<ObjectInfo>().placed==false)
        {
            float initialFazRating = DaytimeScript.FazRating; 
            DaytimeScript.FazRating = DaytimeScript.FazRating + OwnedObjects[index].GetComponent<ObjectInfo>().price;
            OwnedObjects[index].GetComponent<ObjectInfo>().placed=true;
            StartCoroutine(RatingAnimation(initialFazRating,DaytimeScript.FazRating));
        }

        childTransform = pendingObject.transform.Find("StartParticlesItem");
        StartParticlesItem = childTransform.gameObject.GetComponent<ParticleSystem>();
        StartParticlesItem.Play();

        PlayBuildingEffects.PlayOneShot(PlaceAudio);
        pendingObject.GetComponent<ActivateObjCol>().JustPlaced=true;

        pendingObject = null;

        ChangeddisplayPictures = RemoveObjectSprite(ChangeddisplayPictures,ChangeddisplayPictures[index]);
        PlacedObjects = AddObject(PlacedObjects,OwnedObjects[index]);
        OwnedObjects = RemoveObject(OwnedObjects,OwnedObjects[index]);

        if (index >= OwnedObjects.Length)
        {
            if(OwnedObjects.Length>1)
                index = 1;
            else
                index = 0;
        }

        UpdateDisplay();
        DaytimeScript.UpdateTotalStats();
        SelectObject(index);
        UpdateWarnings();
        ChangeBuildState();
    }

    public void RotateObject()
    {
        pendingObject.transform.Rotate(Vector3.up, rotateAmount);
    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            pos = hit.point;
        }
    }

    void UpdateWarnings()
    {
        if(canPlace && OwnedObjects.Length==1)
        {
            MidWarning.enabled=true;
            BadWarning.enabled=false;
            GoodWarning.enabled=false;
        }
        else
        if (canPlace)
        {
            BadWarning.enabled=false;
            MidWarning.enabled=false;
            GoodWarning.enabled=true;
            HitBoxMat.SetColor("_EmissionColor", new Color32(0, 255, 0, 5));
            HitBoxMat.color = new Color32(0, 255, 0, 5);
        }
        else
        {
            GoodWarning.enabled=false;
            MidWarning.enabled=false;
            BadWarning.enabled=true;
            HitBoxMat.SetColor("_EmissionColor", new Color32(255, 0, 0, 5));
            HitBoxMat.color = new Color32(255, 0, 0, 5);
        }
        if(isBuidling==false)
        {
            BadWarning.enabled=false;
            GoodWarning.enabled=false;
            MidWarning.enabled=false;
        }
    }

    public void SwitchObject(int newIndex)
    {
        if (pendingObject != null)
        {
            Destroy(pendingObject);  // Remove the current visual representation
        }

        SelectObject(newIndex);  // Automatically create a new visual representation of the selected object

        CheckPlacementState();
    }

    public void CheckPlacementState()
    {
        // Call this method to verify placement validity
        canPlace = true; // Start with assuming placement is valid

        // Now check if the current pending object overlaps with any placed objects
        foreach (GameObject obj in PlacedObjects)
        {
            // Assuming you have a collider on your objects
            if (obj.GetComponent<Collider>().bounds.Intersects(pendingObject.GetComponent<Collider>().bounds))
            {
                canPlace = false;
                break; // Exit as soon as we find a conflict
            }
        }
    }

    public void SelectObject(int index)
    {
        pendingObject = Instantiate(OwnedObjects[index], pos, transform.rotation);
        Transform childTransform = pendingObject.transform.Find("ColliderAndSize");
        if(childTransform!=null)
        {
            ItemCollider = childTransform.gameObject.GetComponent<BoxCollider>();
            ItemSizeViewer = childTransform.gameObject.GetComponent<MeshRenderer>();
            ItemSizeViewer.enabled=true;
        }
    }

    float RoundToNearestGrid(float pos)
    {
        float roundedPos = Mathf.Round(pos / gridSize) * gridSize;
        return roundedPos + (gridSize / 2);  // Adding 0.5 units assuming gridSize = 1
    }

    public void BuildingDisplay()
    {
        for(int i=0;i<3;i++)
        {
            ImageDisplay[i].enabled=!ImageDisplay[i].enabled;
        }
        for(int i=0;i<3;i++)
        {
            BorderDisplay[i].enabled=!BorderDisplay[i].enabled;
        }
        GeneralText.enabled = !GeneralText.enabled;
        buildCrosshair.enabled = !buildCrosshair.enabled;
        buildText.enabled = !buildText.enabled;
        NormalCrosshair.enabled = !NormalCrosshair.enabled;

        if(isBuidling==false)
        {
            BadWarning.enabled=false;
            GoodWarning.enabled=false;
            MidWarning.enabled=false;
        }
    }

    public void UpdateDisplay()
    {
        if(index==0 && OwnedObjects.Length==1)
        {
            ImageDisplay[0].sprite = ChangeddisplayPictures[0];
            ImageDisplay[1].sprite = ChangeddisplayPictures[0];
            ImageDisplay[2].sprite = ChangeddisplayPictures[0];
        }
        else
        if(OwnedObjects.Length==1)
        {
            ImageDisplay[0].sprite = ChangeddisplayPictures[0];
            ImageDisplay[1].sprite = ChangeddisplayPictures[index];
            ImageDisplay[2].sprite = ChangeddisplayPictures[0];
        }
        else
        if(index==OwnedObjects.Length-1 && index!=0)
        {
            ImageDisplay[0].sprite = ChangeddisplayPictures[OwnedObjects.Length-2];
            ImageDisplay[1].sprite = ChangeddisplayPictures[OwnedObjects.Length-1];
            ImageDisplay[2].sprite = ChangeddisplayPictures[0];
        }
        else
        {
            ImageDisplay[0].sprite = ChangeddisplayPictures[index-1];
            ImageDisplay[1].sprite = ChangeddisplayPictures[index];
            ImageDisplay[2].sprite = ChangeddisplayPictures[index+1];
        }
    }

    public IEnumerator RatingAnimation(float startFazRating, float endFazRating)
    {
        if (isRatingAnimating) yield break; // Prevent starting the animation if it's already running
        isRatingAnimating = true; // Set the flag to true

        FazRatingImageDisplay.SetActive(true);
        StartParticles1000.Play();
        FazRatingImageDisplay.GetComponent<AudioSource>().enabled = true;
        Animator RatingAnimator = FazRatingImageDisplay.GetComponent<Animator>();
        RatingAnimator.Play("FazRating");

        int displayedNumber = (int)startFazRating; // Start from the initial FazRating
        float totalSteps = endFazRating - startFazRating; // Steps to reach new FazRating

        // Calculate total animation duration based on the score
        float animationDuration = 1f;
        float waitTimePerStep = animationDuration / totalSteps;

        Debug.Log(waitTimePerStep);

        yield return new WaitForSeconds(0.1f);

        while (displayedNumber < endFazRating)
        {
            displayedNumber++;
            if (displayedNumber % 1000 == 0)
            {
                DaytimeScript.CashMoney+=100;
                StartCoroutine(FireworksAnim());
            }
            FazratingTextDisplay.text = displayedNumber.ToString();
            yield return new WaitForSeconds(waitTimePerStep); // Wait based on score
        }

        FazRatingImageDisplay.GetComponent<AudioSource>().enabled = false;
        FazratingTextDisplay.text = endFazRating.ToString();
        yield return new WaitForSeconds(1.0f);
        FazRatingImageDisplay.SetActive(false);

        isRatingAnimating = false; // Reset the flag when done
    }

    public IEnumerator FireworksAnim()
    {
        PerformanceBonusDisplay.SetActive(true);
        PerformanceBonusDisplay.GetComponent<Animator>().Play("PerformanceBonus");
        FireWorksParticles.SetActive(true);
        FireWorksParticles.GetComponent<AudioSource>().enabled=true;
        FireWorksParticles.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        PerformanceBonusDisplay.SetActive(false);
        FireWorksParticles.GetComponent<Animator>().Play("FireworksAnim");
        yield return new WaitForSeconds(0.75f);
        FireWorksParticles.GetComponent<AudioSource>().enabled=false;
        FireWorksParticles.SetActive(false);
    }

    public void ChangeBuildState()
    {
        isBuidling = !isBuidling;
        BuildingDisplay();
        UpdateDisplay();

        if (isBuidling)
        {
            // Reinstantiate the visual representation when re-entering build mode
            if (pendingObject == null)
            {
                SelectObject(index);
            }
        }
        else
        {
            canPlace = true;
            // If exiting build mode, remove the visual representation
            if (pendingObject != null)
            {
                Destroy(pendingObject);
                pendingObject = null;
            }
        }
    }
    
    public Sprite[] AddObjectSprite(Sprite[] array, Sprite newObject)
    {
        // Convert array to List
        List<Sprite> objectList = new List<Sprite>(array);

        // Add the new object to the List
        objectList.Add(newObject);

        // Convert List back to array and return
        return objectList.ToArray();
    }

     // Method to add an object to the specified array
    public GameObject[] AddObject(GameObject[] array, GameObject newObject)
    {
        // Convert array to List
        List<GameObject> objectList = new List<GameObject>(array);

        // Add the new object to the List
        objectList.Add(newObject);

        // Convert List back to array and return
        return objectList.ToArray();
    }

    public Sprite[] RemoveObjectSprite(Sprite[] array,Sprite objectToRemove)
    {
        // Convert array to List
        List<Sprite> objectList = new List<Sprite>(array);

        // Remove the object from the List
        if (objectList.Contains(objectToRemove))
        {
            objectList.Remove(objectToRemove);
        }
        else
        {
            Debug.LogWarning("Object not found in the array.");
        }

        // Convert List back to array and return
        return objectList.ToArray();
    }

    // Method to remove an object from the specified array
    public GameObject[] RemoveObject(GameObject[] array, GameObject objectToRemove)
    {
        // Convert array to List
        List<GameObject> objectList = new List<GameObject>(array);

        // Remove the object from the List
        if (objectList.Contains(objectToRemove))
        {
            objectList.Remove(objectToRemove);
        }
        else
        {
            Debug.LogWarning("Object not found in the array.");
        }

        // Convert List back to array and return
        return objectList.ToArray();
    }
}

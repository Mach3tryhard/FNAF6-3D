using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using UnityEngine.UI;
using StarterAssets;

public class JumpScare : MonoBehaviour
{
    public float lerpDuration = 2f;
    public AudioClip JumpScareSound;

    private Animator _animator;
    private GameObject _DeleteMainCamera;
    private GameObject _facePosition;
    private AudioSource _audioSource;
    private NavMeshAgent _agent;
    private GameObject jumpscarePos;
    private FirstPersonController firstpersoncontroller;
    private Night_Mechanics _NightScript;
    private GameObject _player;
    public GameObject  _imageOverlay;
    private Light _flashLight;
    private FlashlightMechanics _lightScript;
    private GameObject _maincamera;
    public bool jumpscare=false;
    private NavMeshAgent _SpringTrap;
    private NavMeshAgent _Lefty;
    private NavMeshAgent _ScrapBaby;
    private NavMeshAgent _MoltenFreddy;
    private ChaseMechanic _chaseScript;

    // Start is called before the first frame update
    void Start()
    {
        // GET CANVAS COMPONENTS
        _player = GameObject.FindWithTag("Player");

        // GET OTHER STUFF
        foreach (Transform child in transform)
        {
            if (child.name == "Face")
            {
                _facePosition = child.gameObject;
                break;
            }
        }
        _chaseScript = GameObject.FindWithTag("Player").transform.Find("AnimatronicChase").gameObject.GetComponent<ChaseMechanic>();
        _maincamera = GameObject.FindGameObjectWithTag("CinemachineTarget");
        _lightScript = GameObject.FindGameObjectWithTag("Flashlight").GetComponent<FlashlightMechanics>();
        _flashLight = GameObject.FindGameObjectWithTag("Flashlight").GetComponent<Light>();
        jumpscarePos = GameObject.FindGameObjectWithTag("JumpscarePos");
        _NightScript =  GameObject.FindGameObjectWithTag("NightTime").GetComponent<Night_Mechanics>();
        _SpringTrap = GameObject.FindGameObjectWithTag("NightTime").transform.Find("SpringTrap").gameObject.GetComponent<NavMeshAgent>();
        _Lefty = GameObject.FindGameObjectWithTag("NightTime").transform.Find("Lefty").gameObject.GetComponent<NavMeshAgent>();
        _ScrapBaby = GameObject.FindGameObjectWithTag("NightTime").transform.Find("ScrapBaby").gameObject.GetComponent<NavMeshAgent>();
        _MoltenFreddy = GameObject.FindGameObjectWithTag("NightTime").transform.Find("MoltenFreddy").gameObject.GetComponent<NavMeshAgent>();
        _agent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        _DeleteMainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        _animator = GetComponent<Animator>();
        firstpersoncontroller = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && jumpscare==false)
        {
            _chaseScript.enabled=false;
            jumpscare=true;
            Debug.Log("jumpscare");
            StartCoroutine(PerformJumpscare());
        }
    }

    private IEnumerator PerformJumpscare()
    {
        _chaseScript.enabled=false;
        _SpringTrap.speed = 0f;_Lefty.speed=0f;_ScrapBaby.speed=0f;_MoltenFreddy.speed=0f;
        firstpersoncontroller.enabled=false;
        _NightScript.enabled=false;
        GetComponent<NavMeshAgent>().enabled=false;
        _lightScript.enabled=false;
        _flashLight.enabled=true;

        _facePosition.transform.parent = null;
        transform.SetParent(_facePosition.transform);

        Vector3 _faceOnlyY = new Vector3(_facePosition.transform.position.x,_player.transform.position.y,_facePosition.transform.position.z);
        StartCoroutine(LerpPlayerLookAt(_player, _faceOnlyY, 0.1f));
        StartCoroutine(LerpCameraRotation(0.1f));
        yield return new WaitForSeconds(0.1f);
        //_player.transform.LookAt(_faceOnlyY);
       //_maincamera.transform.localEulerAngles = new Vector3(0, _maincamera.transform.localEulerAngles.y, _maincamera.transform.localEulerAngles.z);

        Vector3 _playerOnlyY = new Vector3(_player.transform.position.x,_facePosition.transform.position.y,_player.transform.position.z);
        _facePosition.transform.LookAt(_playerOnlyY);

        yield return new WaitForSeconds(0.1f);

        _facePosition.transform.position = jumpscarePos.transform.position;

        _audioSource.PlayOneShot(JumpScareSound);
        _animator.Play("ZombieScream");

        Vector3 newRotation = _facePosition.transform.rotation.eulerAngles;
        newRotation.y += 25f;
        _facePosition.transform.rotation = Quaternion.Euler(newRotation);

        yield return new WaitForSeconds(1.5f);

        _imageOverlay.GetComponent<Image>().color = new Color32(175, 0, 0, 255);

        StartCoroutine(FadeOverlay(_imageOverlay,new Color32(175, 0, 0, 255), Color.black, 2f));

        StartCoroutine(LoadSceneAfterDelay("CompleteLabyrinth",5.0f));
    }

    private IEnumerator LerpCameraRotation(float duration)
    {
        Vector3 startRotation = _maincamera.transform.localEulerAngles;
        Vector3 endRotation = new Vector3(0, startRotation.y, startRotation.z);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            _maincamera.transform.localEulerAngles = Vector3.Lerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _maincamera.transform.localEulerAngles = endRotation;
    }

    private IEnumerator LerpPlayerLookAt(GameObject player, Vector3 targetPosition, float duration)
    {
        Quaternion startRotation = player.transform.rotation;
        Quaternion endRotation = Quaternion.LookRotation(targetPosition - player.transform.position);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            player.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.transform.rotation = endRotation;
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
}

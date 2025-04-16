using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private CameraController cameraController;
    private Movement playerMovement;

    [SerializeField]
    private TextMeshProUGUI dialogueName;
    [SerializeField]
    private GameObject dialoguePanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cameraController = FindAnyObjectByType<CameraController>();
        playerMovement = FindAnyObjectByType<Movement>();
    }

    public void DialogueStarter(Transform lookAtPoint, float lookFov, float lookDuration, string name)
    {
        playerMovement.SetMovementLock(true);
        dialogueName.text = name;
        dialoguePanel.SetActive(true);
        cameraController.LookAtPoint(lookAtPoint.position, lookFov, lookDuration);
    }

    public void DialogueEnder()
    {
        cameraController.UnlockCamera();
        playerMovement.SetMovementLock(false);
        dialoguePanel.SetActive(false);
    }
}

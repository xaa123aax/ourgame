using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

#pragma warning disable 649

public class PlayerUI : MonoBehaviourPun
{
    #region Private Fields
    public static PlayerUI Instance;

    [Tooltip("生成位置")]
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f,500f, 0f);

    [Tooltip("玩家暱稱")]
    [SerializeField]
    public Text playerNameText;

    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private RectTransform playerHealthSlider;

    public PlayerManager target;

    float characterControllerHeight;

    Transform targetTransform;

    Renderer targetRenderer;

    Vector3 targetPosition;


    #endregion

    #region MonoBehaviour Messages

    void Awake()
    {

        Instance = this;

        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    private void Start()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        playerHealthSlider.sizeDelta = new Vector2(target.PlayerHp, playerHealthSlider.sizeDelta.y);


    }

    void LateUpdate()
    {
        // Do not show the UI if we are not visible to the camera, thus avoid potential bugs with seeing the UI, but not the player itself.
        if (targetRenderer != null)
        {
            this.gameObject.SetActive(targetRenderer.isVisible);
        }

        // #Critical
        // Follow the Target GameObject on screen.
        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += characterControllerHeight;

            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }

    }
    #endregion

    #region Public Methods

    /// <param name="target">Target.</param>
    public void SetTarget(PlayerManager _target)
    {

        if (_target == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        // Cache references for efficiency because we are going to reuse them.
        this.target = _target;
        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();
        gameObject.name = "PlayerUI" + target.photonView.Owner.NickName + target.photonView.ViewID;

        CapsuleCollider _characterController = this.target.GetComponent<CapsuleCollider>();

        // Get data from the Player that won't change during the lifetime of this Component
        if (_characterController != null)
        {
            characterControllerHeight = _characterController.height;
        }

        if (playerNameText != null)
        {
            playerNameText.text = this.target.photonView.Owner.NickName;
        }
    }

    #endregion
}
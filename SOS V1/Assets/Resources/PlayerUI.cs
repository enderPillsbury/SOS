using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Com.MysticVentures.SOS{
    public class PlayerUI : MonoBehaviour{
        #region Private Fields
        [SerializeField]
        private Text playerNameText;

        [SerializeField]
        private Slider playerHealthSlider;

        private PlayerManager target;
        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;
        #endregion

        #region MonoBehavior Callbacks
        void Update(){
            if(target == null){
                Destroy(this.gameObject);
                return;
            }
            if (playerHealthSlider != null){
                playerHealthSlider.value = target.Health;
            }
        }
        void Awake(){
            _canvasGroup = this.GetComponent<CanvasGroup>();
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }
        void LateUpdate(){
            if (targetRenderer!= null){
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }
            if (targetTransform != null){
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint (targetPosition) + screenOffset;
            }
        }
        #endregion

        #region Public Methods
        public void SetTarget(PlayerManager _target){
            if (_target == null){
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager targer for PlayerUI.SetTarget", this);
                return;
            }
            this.target = _target;
            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController _characterController = _target.GetComponent<CharacterController>();
            if (_characterController != null){
                characterControllerHeight = _characterController.height;
            }
            if (playerNameText != null){
                playerNameText.text = this.target.photonView.Owner.NickName;
            }
        }
        [SerializeField]
        private Vector3 screenOffset = new Vector3 (0f, -1f, 0f);
        #endregion
    }
}
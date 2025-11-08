using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using System.Collections;

namespace Com.MysticVentures.SOS{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable{
        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
            if(stream.IsWriting){
                stream.SendNext(isFiring);
                stream.SendNext(Health);
            }
            else{
                this.isFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
            }
        #endregion 
        #region Private Fields
        [SerializeField]
        private GameObject attackZone;
        private float cooldown = 0f;
        bool isFiring;
        public float Health = 1f;
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode){
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
        #endregion

        #region MonoBehaviour Callbacks
        void Start(){
            if (PlayerUiPrefab != null){
                GameObject _uiGo =  Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else{
                Debug.LogWarning("<Color=Red><a>Warning</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void Awake(){
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            if (photonView.IsMine){
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            DontDestroyOnLoad(this.gameObject);
            if (attackZone == null){
                Debug.LogError("<Color=Red><a>Missing</a></Color> Attack Zone Reference.", this);
            }
            else
            {
                attackZone.SetActive(false);
            }
        }
        void Update(){
            if (photonView.IsMine){
                ProcessInputs();
                if (Health <= 0f){
                    GameManager.Instance.LeaveRoom();
                }
            }
            if(attackZone !=null && isFiring != attackZone.activeInHierarchy){
                attackZone.SetActive(isFiring);
            }
        }

        void OnTriggerEnter(Collider other){
            if (!photonView.IsMine){
                return;
            }
            if (!other.name.Contains("Attack")){
                return;
            }
            Health -= 0.1f;
        }

        void OnTriggerStay(Collider other){
            if (!photonView.IsMine){
                return;
            }
            if (!other.name.Contains("Attack")){
                return;
            }
            Health -= 0.1f * Time.deltaTime;
        }

        void OnLevelWasLoaded(int level){
            this.CalledOnLevelWasLoaded(level);
        }

        void CalledOnLevelWasLoaded(int level){
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f)){
                transform.position = new Vector3(0f, 5f, 0f);
            }
            GameObject _uiGo = Instantiate(this.PlayerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        
        public override void OnDisable(){
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion
        #region Custom
        public static GameObject LocalPlayerInstance;
        [SerializeField]
        public GameObject PlayerUiPrefab;
        void ProcessInputs(){
            if(Mouse.current.leftButton.wasPressedThisFrame){
                if(!isFiring){
                    isFiring = true;
                }
            }
            else{
                if(isFiring){
                    cooldown += Time.deltaTime;
                    if(cooldown >=1){
                        isFiring = false;
                        cooldown = 0;
                    }
                }
            }
        }
        #endregion
    }
}

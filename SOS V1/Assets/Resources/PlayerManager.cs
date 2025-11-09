using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using System.Collections;

namespace Com.MysticVentures.SOS{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable{
        #region IPunObservable implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
            if(stream.IsWriting){
                stream.SendNext(this.isFiring);
                stream.SendNext(this.Health);
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

        public static GameObject LocalPlayerInstance;

        private float cooldown = 0f;

        bool isFiring;
        public float Health = 1f;
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode){
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
        #endregion

        #region MonoBehaviour Callbacks
        void Start(){
            if (this.PlayerUiPrefab != null){
                GameObject _uiGo =  Instantiate(PlayerUiPrefab);
                _uiGo.SendMessage ("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else{
                Debug.LogWarning("<Color=Red><a>Warning</a></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        void Awake(){
            if (attackZone == null){
                Debug.LogError("<Color=Red><a>Missing</a></Color> Attack Zone Reference.", this);
            }
            else
            {
                attackZone.SetActive(false);
            }
            DontDestroyOnLoad(this.gameObject);
            if (photonView.IsMine){
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            
        }
        private bool leavingRoom;
        void Update(){
            if (photonView.IsMine){
                this.ProcessInputs();
                if (this.Health <= 0f && !this.leavingRoom){
                    this.leavingRoom = PhotonNetwork.LeaveRoom();
                }
            }
            if(this.attackZone !=null && this.isFiring != this.attackZone.activeInHierarchy){
                this.attackZone.SetActive(this.isFiring);
            }
        }
        public override void OnLeftRoom(){
            this.leavingRoom = false;
        }
        void OnTriggerEnter(Collider other){
            if (!photonView.IsMine){
                return;
            }
            if (!other.name.Contains("Attack")){
                return;
            }
            this.Health -= 0.1f;
        }

        void OnTriggerStay(Collider other){
            if (!photonView.IsMine){
                return;
            }
            if (!other.name.Contains("Attack")){
                return;
            }
            this.Health -= 0.1f * Time.deltaTime;
        }

        void OnLevelWasLoaded(int level){
            this.CalledOnLevelWasLoaded(level);
        }

        void CalledOnLevelWasLoaded(int level){
            if (!Physics.Raycast(transform.position, -Vector3.up, 0f)){
                transform.position = new Vector3(1.5f, 1f, 0f);
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

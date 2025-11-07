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
        #endregion

        #region MonoBehaviour Callbacks
        void Awake(){
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
            }
            if (Health <= 0f){
                {
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

        #endregion
        #region Custom
        public static GameObject LocalPlayerInstance;
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

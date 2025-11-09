using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MysticVentures.SOS{
    public class GameManager : MonoBehaviourPunCallbacks{
        #region Photon Callbacks
        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom(){
            SceneManager.LoadScene(0);
        }

        public override void OnJoinedRoom(){
            if (PlayerManager.LocalPlayerInstance == null){
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(1.5f, 1f, 0f), Quaternion.identity, 0);
            }
        }

        public override void OnPlayerEnteredRoom(Player other){
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); //not seen if you're the player connecting
            if (PhotonNetwork.IsMasterClient){
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); //called before OnPlayerLeftRoom
                LoadArena();
            }

        }

        public override void OnPlayerLeftRoom(Player other){
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); //seen when other disconnects

            if (PhotonNetwork.IsMasterClient){
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); //Called before OnPlayerLeftRoom
                LoadArena(); 
            }
        }
        #endregion

        #region Public Methods
        public static GameManager Instance;
        public GameObject playerPrefab;
        public void LeaveRoom(){
            PhotonNetwork.LeaveRoom();
        }
        void Start(){
            Instance = this;
            if (playerPrefab == null){
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else{
                if(PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null){
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManager.GetActiveScene().path);
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(1.5f, 1f, 0f), Quaternion.identity, 0);
                }
                else{
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManager.GetActiveScene().path);
                }
                
            }
        }
        #endregion

        #region Private Methods
        void LoadArena(){
            if (!PhotonNetwork.IsMasterClient){
                Debug.LogError("PhotonNetwork: Trying to Load a level but we are not the master Client");
                return;
            }
            Debug.LogFormat("PhotonNetwork: Loading level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        #endregion
    }
}
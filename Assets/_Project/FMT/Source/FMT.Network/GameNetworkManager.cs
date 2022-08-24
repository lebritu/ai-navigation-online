using System.Collections;
using System.Collections.Generic;
using FMT.Gameplay;
using FMT.Player;
using UnityEngine;
using Mirror;

namespace FMT.Network
{
    public class GameNetworkManager : NetworkManager
    {
        [SerializeField]
        private PlayerMotor _playerPrefab;

        private SpawnPoint[] _spawnPoints;

        public static GameNetworkManager Instance => singleton as GameNetworkManager;

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            _spawnPoints = FindObjectsOfType<SpawnPoint>();
            int randomNumber = Random.Range(0, _spawnPoints.Length - 1);

            PlayerMotor player = Instantiate(_playerPrefab, _spawnPoints[randomNumber].Position, _spawnPoints[randomNumber].Rotation);

            NetworkServer.AddPlayerForConnection(conn, player.gameObject);

            Debug.Log("Player " + numPlayers + " Connected");

            SoundPool.PlayMusicClip(SoundPool.AudioLibrary.Music);
        }
    }
}

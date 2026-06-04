using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : NetworkBehaviour
{
    public static RoundManager Instance;

    [SerializeField] private Text _text;

    [SerializeField] private int _winScore = 12;
    [SerializeField] private float _roundRestartDelay = 3f;

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private Transform[] _spawnPointsRed;
    [SerializeField] private Transform[] _spawnPointsBlue;

    [SyncVar] private int _redScore;
    [SyncVar] private int _blueScore;

    private Dictionary<NetworkConnectionToClient, Teams> _players = new Dictionary<NetworkConnectionToClient, Teams>();

    private int _aliveRed;
    private int _aliveBlue;

    private bool _isRoundActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        _isRoundActive = true;

        NetworkServer.OnDisconnectedEvent += OnClientDisconnected;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        NetworkServer.OnDisconnectedEvent -= OnClientDisconnected;
    }

    private void OnClientDisconnected(NetworkConnectionToClient conn)
    {
        if (_players.ContainsKey(conn))
        {
            Teams team = _players[conn];

            _players.Remove(conn);

            if (_isRoundActive)
            {
                if (team == Teams.Red)
                {
                    _aliveRed--;
                }
                else
                {
                    _aliveBlue--;
                }

                _aliveRed = Mathf.Max(_aliveRed, 0);
                _aliveBlue = Mathf.Max(_aliveBlue, 0);

                CheckRoundEnd();
            }
        }
    }

    [Server]
    public void RegisterPlayer(NetworkConnectionToClient conn, Teams team)
    {
        if (_players.ContainsKey(conn))
        {
            if (team == Teams.Red)
            {
                _aliveRed++;
            }
            else
            {
                _aliveBlue++;
            }

            Debug.Log($"Респавн {conn.connectionId} ({team}). Живых: R = {_aliveRed} B = {_aliveBlue}");
            return;
        }

        _players[conn] = team;

        if (team == Teams.Red)
        {
            _aliveRed++;
        }
        else
        {
            _aliveBlue++;
        }

        Debug.Log($"Новый игрок {conn.connectionId} ({team}). Живых: R = {_aliveRed} B = {_aliveBlue}");
    }

    [Server]
    public void OnPlayerDeath(NetworkConnectionToClient conn, Teams team)
    {
        if (!_isRoundActive)
        {
            return;
        }
        if (team == Teams.Red)
        {
            _aliveRed--;
        }
        else
        {
            _aliveBlue--;
        }

        _aliveRed = Mathf.Max(_aliveRed, 0);
        _aliveBlue = Mathf.Max(_aliveBlue, 0);

        Debug.Log($"Смерть {conn.connectionId} ({team}). Живых: R = {_aliveRed} B = {_aliveBlue}");

        CheckRoundEnd();
    }

    private void CheckRoundEnd()
    {
        if (_aliveRed <= 0)
        {
            EndRound(Teams.Blue);
        }
        else if (_aliveBlue <= 0)
        {
            EndRound(Teams.Red);
        }
    }

    private void EndRound(Teams winner)
    {
        if (!_isRoundActive)
        {
            return;
        }

        _isRoundActive = false;

        if (winner == Teams.Red)
        {
            _redScore++;
        }
        else
        {
            _blueScore++;
        }

        RpcRoundWinner(winner, _redScore, _blueScore);

        Debug.Log($"Раунд выиграла {winner}. Счёт: {_redScore}:{_blueScore}");

        if (_redScore >= _winScore)
        {
            EndMatch(Teams.Red);
        }
        else if (_blueScore >= _winScore)
        {
            EndMatch(Teams.Blue);
        }
        else
        {
            Invoke(nameof(RestartRound), _roundRestartDelay);
        }
    }

    [Server]
    private void RestartRound()
    {
        foreach (var conn in _players.Keys)
        {
            if (conn.identity != null)
            {
                NetworkServer.Destroy(conn.identity.gameObject);
            }
        }

        _aliveRed = 0;
        _aliveBlue = 0;

        foreach (var kvp in _players)
        {
            NetworkConnectionToClient conn = kvp.Key;
            Teams team = kvp.Value;
            Transform spawnPoint = (team == Teams.Red) ? GetRandomSpawn(_spawnPointsRed) : GetRandomSpawn(_spawnPointsBlue);

            GameObject newPlayer = Instantiate(_playerPrefab, spawnPoint.position, spawnPoint.rotation);

            NetworkServer.ReplacePlayerForConnection(conn, newPlayer, true);
        }

        _isRoundActive = true;
    }

    private Transform GetRandomSpawn(Transform[] arr)
    {
        return arr[Random.Range(0, arr.Length)];
    }

    private void EndMatch(Teams winner)
    {
        RpcMatchWinner(winner);
    }

    [ClientRpc]
    private void RpcRoundWinner(Teams winner, int redScore, int blueScore)
    {
        _text.text = $"{redScore} : {blueScore}";
    }

    [ClientRpc]
    private void RpcMatchWinner(Teams winner)
    {
        _text.text = $"Победители - {winner}!";
    }
}
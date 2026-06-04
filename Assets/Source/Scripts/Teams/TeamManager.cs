using Mirror;
using UnityEngine;

public class TeamManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnTeamChanged))] private Teams _team;

    [SerializeField] private SpriteRenderer _spriteRenderer;

    public Teams Team => _team;

    public override void OnStartServer()
    {
        if (transform.position.y > 0)
        {
            _team = Teams.Red;
            Debug.Log("Red");
        }
        else
        {
            _team = Teams.Blue;
            Debug.Log("Blue");
        }

        if (connectionToClient != null)
        {
            if (RoundManager.Instance != null)
            {
                RoundManager.Instance.RegisterPlayer(connectionToClient, _team);
            }
        }
    }

    public override void OnStartClient()
    {
        ApplyTeamVisuals(_team);
    }

    private void OnTeamChanged(Teams oldTeam, Teams newTeam)
    {
        ApplyTeamVisuals(newTeam);
    }

    private void ApplyTeamVisuals(Teams newTeam)
    {
        int layerIndex;

        switch (_team)
        {
            case Teams.Red:
                _spriteRenderer.color = Color.red;
                layerIndex = LayerMask.NameToLayer("Red");
                gameObject.layer = layerIndex;
                break;
            case Teams.Blue:
                _spriteRenderer.color = Color.cyan;
                layerIndex = LayerMask.NameToLayer("Blue");
                gameObject.layer = layerIndex;
                break;
        }
    }
}

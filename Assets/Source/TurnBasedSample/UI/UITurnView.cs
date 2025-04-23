using TurnBasedSample.Core;
using TurnBasedSample.Gameplay;
using TMPro;
using UnityEngine;

namespace TurnBasedSample.UI
{
    public class UITurnView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        private void Start()
        {
            GameManager.Instance.OnNextTurnEvent += OnNextTurnEvent;
            GameManager.Instance.OnGameOverEvent += OnGameOver;
            OnNextTurnEvent(GameManager.Instance.CurrentTeam);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.OnNextTurnEvent -= OnNextTurnEvent;
                GameManager.Instance.OnGameOverEvent -= OnGameOver;
            }
        }

        private void OnNextTurnEvent(UnitTeam team)
        {
            label.text = $"Turn: {team.ToString()}";
        }

        private void OnGameOver(UnitTeam team)
        {
            label.text = $"{team.ToString()} has won!";
        }
    }
}
using TurnBasedSample.Core;
using TurnBasedSample.Input;
using TurnBasedSample.Utils;
using UnityEngine;

namespace TurnBasedSample.Gameplay
{
    public class Tile : MonoBehaviour, IClickable
    {
        [SerializeField] private VisualHighlight visualHighlight;

        public Unit OccupyingUnit { get; set; }
        public TileGrid TileGrid { get; private set; }
        public Vector2Int Position { get; private set; }

        public void Init(TileGrid tileGrid, Vector2Int position)
        {
            TileGrid = tileGrid;
            Position = position;
        }

        public void SwitchHighlight(bool isHighlighted)
        {
            visualHighlight.SwitchHighlight(isHighlighted);
        }
    }
}
using System;
using System.Collections.Generic;
using TurnBasedSample.Input;
using TurnBasedSample.Utils;
using UnityEngine;

namespace TurnBasedSample.Gameplay
{
    public enum UnitTeam
    {
        TeamRed,
        TeamBlue
    }
    
    public class Unit : MonoBehaviour, IClickable
    {
        [SerializeField] private UnitTeamConfig[] unitTeamConfigs;
        [SerializeField] private VisualHighlight visualHighlight;
        [SerializeField] private Vector3 positionOffset = new Vector3(0.0f, 1.0f, 0.0f);
        [SerializeField] private int moveRange = 2;
        
        public UnitTeam Team { get; private set; }
        public Tile OccupiedTile { get; private set; }

        public void SetTeam(UnitTeam team)
        {
            Team = team;
    
            var config = Array.Find(unitTeamConfigs, c => c.Team == team);
            visualHighlight.SetDefaultMaterial(config.DefaultMaterial);
            visualHighlight.SwitchHighlight(false);
            
            gameObject.layer = LayerMask.NameToLayer(team.ToString());
        }

        public void SetTile(Tile tile)
        {
            if (OccupiedTile != null)
            {
                OccupiedTile.OccupyingUnit = null;
                OccupiedTile = null;
            }
            
            OccupiedTile = tile;

            if (OccupiedTile != null)
            {
                OccupiedTile.OccupyingUnit = this;
                transform.position = tile.transform.position + positionOffset;
            }
            else
            {
                transform.position = Vector3.zero;
            }
        }
        
        public void SwitchHighlight(bool isHighlighted)
        {
            visualHighlight.SwitchHighlight(isHighlighted);
        }

        public List<Tile> GetTilesForMovement()
        {
            var tiles = new List<Tile>();
    
            // Vertical line - top
            for (var i = 1; i <= moveRange; i++)
            {
                var shouldBreak = CheckTile(0, i, tiles);
                if (shouldBreak)
                    break;
            }
            
            // Vertical line - bottom
            for (var i = -1; i >= -moveRange; i--)
            {
                var shouldBreak = CheckTile(0, i, tiles);
                if (shouldBreak)
                    break;
            }
    
            // Horizontal line - top
            for (var i = 1; i <= moveRange; i++)
            {
                var shouldBreak = CheckTile(i, 0, tiles);
                if (shouldBreak)
                    break;
            }
            
            // Horizontal line - bottom
            for (var i = -1; i >= -moveRange; i--)
            {
                var shouldBreak = CheckTile(i, 0, tiles);
                if (shouldBreak)
                    break;
            }
    
            // Diagonal line - top-right
            for (var i = 1; i <= moveRange; i++)
            {
                var shouldBreak = CheckTile(i, i, tiles);
                if (shouldBreak)
                    break;
            }
    
            // Diagonal line - top-left
            for (var i = 1; i <= moveRange; i++)
            {
                var shouldBreak = CheckTile(-i, i, tiles);
                if (shouldBreak)
                    break;
            }
    
            // Diagonal line - bottom-right
            for (var i = 1; i <= moveRange; i++)
            {
                var shouldBreak = CheckTile(i, -i, tiles);
                if (shouldBreak)
                    break;
            }
    
            // Diagonal line - bottom-left
            for (var i = 1; i <= moveRange; i++)
            {
                var shouldBreak = CheckTile(-i, -i, tiles);
                if (shouldBreak)
                    break;
            }
    
            return tiles;
        }

        private bool CheckTile(int xOffset, int yOffset, List<Tile> tiles)
        {
            var tile = OccupiedTile.TileGrid.GetTile(OccupiedTile.Position + new Vector2Int(xOffset, yOffset));
                
            // Reached edge of the grid
            if (!tile)
                return true;

            if (tile.OccupyingUnit)
            {
                // Allow movement onto tiles occupied by enemies (attacking)
                if (tile.OccupyingUnit.Team != Team)
                {
                    tiles.Add(tile);
                }
                    
                // Don't continue checking, if a tile is occupied
                return true;
            }
                
            tiles.Add(tile);
            return false;
        }

        [Serializable]
        public class UnitTeamConfig
        {
            public UnitTeam Team;
            public Material DefaultMaterial;
        }
    }
}
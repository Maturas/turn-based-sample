using System;
using TurnBasedSample.Gameplay;
using UnityEngine;

namespace TurnBasedSample.Core
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TurnBasedSample/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject
    {
        public Vector2Int GridSize;
        public UnitsPlacementConfig[] UnitsPlacements;

        [Serializable]
        public class UnitsPlacementConfig
        {
            public UnitTeam Team;
            public Vector2Int[] Positions;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (UnitsPlacements == null || UnitsPlacements.Length == 0)
                return;

            // Check for duplicate teams
            for (int i = 0; i < UnitsPlacements.Length; i++)
            {
                if (UnitsPlacements[i] == null)
                    continue;
            
                for (int j = i + 1; j < UnitsPlacements.Length; j++)
                {
                    if (UnitsPlacements[j] == null)
                        continue;
                
                    if (UnitsPlacements[i].Team == UnitsPlacements[j].Team)
                    {
                        Debug.LogError($"Duplicate team found in GameConfig: {UnitsPlacements[i].Team}. Teams should be unique.");
                    }
                }
            }

            // Check for duplicate positions
            for (var i = 0; i < UnitsPlacements.Length; i++)
            {
                if (UnitsPlacements[i] == null || UnitsPlacements[i].Positions == null)
                    continue;
        
                for (var posI = 0; posI < UnitsPlacements[i].Positions.Length; posI++)
                {
                    Vector2Int position = UnitsPlacements[i].Positions[posI];
        
                    // Check against all positions in this team (except itself)
                    for (var posJ = posI + 1; posJ < UnitsPlacements[i].Positions.Length; posJ++)
                    {
                        if (position == UnitsPlacements[i].Positions[posJ])
                        {
                            Debug.LogError($"GameConfig: Duplicate unit placement found for team {UnitsPlacements[i].Team} at position {position}.");
                        }
                    }
        
                    // Check against all positions in other teams
                    for (var j = i + 1; j < UnitsPlacements.Length; j++)
                    {
                        if (UnitsPlacements[j] == null || UnitsPlacements[j].Positions == null)
                            continue;
                
                        foreach (var otherPosition in UnitsPlacements[j].Positions)
                        {
                            if (position == otherPosition)
                            {
                                Debug.LogError($"GameConfig: Duplicate unit placement found at position {position} between teams {UnitsPlacements[i].Team} and {UnitsPlacements[j].Team}.");
                            }
                        }
                    }
                }
            }

            // Check if positions are within grid bounds
            foreach (var placement in UnitsPlacements)
            {
                if (placement == null || placement.Positions == null)
                    continue;
        
                foreach (var position in placement.Positions)
                {
                    if (position.x < 0 || position.x >= GridSize.x ||
                        position.y < 0 || position.y >= GridSize.y)
                    {
                        Debug.LogError($"GameConfig: Unit placement for team {placement.Team} at {position} is outside the grid bounds of {GridSize}.");
                    }
                }
            }
        }
#endif
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using TurnBasedSample.Gameplay;
using TurnBasedSample.Input;
using TurnBasedSample.Utils;
using UnityEngine;
using UnityEngine.Assertions;

namespace TurnBasedSample.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [SerializeField] private Unit unitPrefab;
        [SerializeField] private TileGrid tileGrid;
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private ClickSelector clickSelector;
        
        [SerializeField] private InputController inputController;
        public InputController InputController => inputController;
        
        [SerializeField] private Camera mainCamera;
        public Camera MainCamera => mainCamera;
        
        public UnitTeam CurrentTeam => _allTeams[_currentTeamIndex];

        public event Action<UnitTeam> OnNextTurnEvent;
        public event Action<UnitTeam> OnGameOverEvent;
        
        private ObjectPool<Unit> _unitPool;
        private UnitTeam[] _allTeams;
        private int _currentTeamIndex;
        
        private Unit _selectedUnit;
        private Tile[] _selectedTiles;
        
        private Dictionary<UnitTeam, List<Unit>> _units;
            
        private void Awake()
        {
            Instance = this;
            Init();
        }

        private void OnDestroy()
        {
            Instance = null;
            clickSelector.OnClickedEvent -= OnClickedEvent;
        }

        private void Init()
        {
            _allTeams = Enum.GetValues(typeof(UnitTeam)).Cast<UnitTeam>().ToArray();
            _units = new Dictionary<UnitTeam, List<Unit>>();
            foreach (var team in _allTeams)
            {
                _units[team] = new List<Unit>();
            }
            
            // Init tile grid
            tileGrid.Init(gameConfig.GridSize.x, gameConfig.GridSize.y);
            
            // Init units
            var initialUnitsAmount = gameConfig.UnitsPlacements.Sum(x => x.Positions.Length);
            _unitPool = ObjectPool<Unit>.Create(unitPrefab, initialUnitsAmount, true);
            
            foreach (var unitPlacementConfig in gameConfig.UnitsPlacements)
            {
                var team = unitPlacementConfig.Team;
                foreach (var position in unitPlacementConfig.Positions)
                {
                    var unit = _unitPool.Get();
                    unit.SetTeam(team);
                    
                    var tile = tileGrid.GetTile(position);
                    unit.SetTile(tile);
                    
                    _units[team].Add(unit);
                }
            }
            
            StartGame();
        }

        private void UnhighlightAllUnits()
        {
            foreach (var units in _units.Values)
            {
                foreach (var unit in units)
                {
                    unit.SwitchHighlight(false);
                }
            }
        }

        private void StartGame()
        {
            _currentTeamIndex = 0;
            clickSelector.OnClickedEvent += OnClickedEvent;
        }

        private void OnTurnEnd()
        {
            // Check if only one team has any units remaining
            var liveTeams = 0;
            foreach (var units in _units.Values)
            {
                if (units.Count > 0)
                    liveTeams++;
                
                if (liveTeams > 1)
                    break;
            }

            if (liveTeams > 1)
            {
                NextTurn();
            }
            else
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            // Disable clicking system
            ResetClicking();
            clickSelector.CurrentSelectionMode = SelectionMode.None;
            clickSelector.OnClickedEvent -= OnClickedEvent;
            
            var winningTeam = _units.First(x => x.Value.Count > 0).Key;
            OnGameOverEvent?.Invoke(winningTeam);
        }

        private void NextTurn()
        {
            // Sanity check to prevent infinite loops
            Assert.IsTrue(_units.Any(x => x.Value.Count > 0));
            
            // Skip dead teams
            do
            {
                _currentTeamIndex = (_currentTeamIndex + 1) % _allTeams.Length;
            }
            while (_units[CurrentTeam].Count == 0);
            
            OnNextTurnEvent?.Invoke(CurrentTeam);
            ResetClicking();
        }

        private void OnClickedEvent(IClickable clickable)
        {
            switch (clickable)
            {
                case null:
                {
                    ResetClicking();
                    break;
                }

                case Tile tile:
                {
                    OnTileClicked(tile);
                    break;
                }

                case Unit unit:
                {
                    OnUnitClicked(unit);
                    break;
                }
            }
        }

        private void ResetClicking()
        {
            // Reset to state before selecting the unit
            tileGrid.UnhighlightAllTiles();
            UnhighlightAllUnits();
            clickSelector.CurrentSelectionMode = CurrentTeam switch
            {
                UnitTeam.TeamRed => SelectionMode.UnitsTeamRed,
                UnitTeam.TeamBlue => SelectionMode.UnitsTeamBlue,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _selectedUnit = null;
            _selectedTiles = null;
        }

        private void OnUnitClicked(Unit unit)
        {
            unit.SwitchHighlight(true);
            
            var tiles = unit.GetTilesForMovement();
            foreach (var tile in tiles)
            {
                tile.SwitchHighlight(true);
            }
            
            _selectedUnit = unit;
            _selectedTiles = tiles.ToArray();
            
            clickSelector.CurrentSelectionMode = SelectionMode.Tiles;
        }

        private void OnTileClicked(Tile tile)
        {
            if (!_selectedTiles.Contains(tile))
            {
                ResetClicking();
                return;
            }
            
            // Destroy occupying tile
            if (tile.OccupyingUnit)
            {
                var enemyUnit = tile.OccupyingUnit;
                tile.OccupyingUnit = null;
                enemyUnit.SetTile(null);

                _unitPool.Return(enemyUnit);
                _units[enemyUnit.Team].Remove(enemyUnit);
            }
            
            _selectedUnit.SetTile(tile);
            OnTurnEnd();
        }
    }
}
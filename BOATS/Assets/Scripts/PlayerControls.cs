using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum MenuState {
    Idle,
    HoldingNewShip,
    SelectingDefender1,
    SelectingDefender2,
    SelectingDefender3,
}

public class PlayerControls : MonoBehaviour
{
    public GameObject selectedObject;
    private MenuInfoController _controls;
    private MenuState _state;
    public MenuState state
    {
        get => _state;
        set
        {
            _state = value;
            if (_controls != null)
            {
                _controls.updateInfoText(value);
            }
        }
    }
    private TileSystem _tileSystem;
    public Vector3 worldMousePosition => Camera.main.ScreenToWorldPoint(Pointer.current.position.ReadValue());

    // pause menu variables
    public GameObject pauseMenuPrefab;
    private GameObject _pauseMenuParent;
    private GameObject _pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        _tileSystem = FindObjectOfType<TileSystem>();
        _controls = FindObjectOfType<MenuInfoController>();
        state = MenuState.Idle;
        _pauseMenuParent = GameObject.FindGameObjectWithTag("Pause Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseMoved(InputValue positionValue)
    {
        // "Ghost" ship underneath the player's mouse when selected to place
        if (state == MenuState.HoldingNewShip)
        {
            Vector2 position = positionValue.Get<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            selectedObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, -1);

            // Update Selected Tiles
            Vector2Int tilePosition = _tileSystem.WorldToTilePoint(worldPosition);
            TileOccupier occupier = selectedObject.GetComponent<TileOccupier>();
            BoatBehavior behavior = selectedObject.GetComponent<BoatBehavior>();
            _tileSystem.SelectTiles(occupier.GetTilesOccupied(tilePosition), behavior.GetFiringRange(occupier.GetFocusCoordinate(tilePosition)));
        }
    }

    public void OnPrimaryClick()
    {
        // Ship Placement
        Vector2Int tilePosition = _tileSystem.WorldToTilePoint(worldMousePosition);
        switch (state)
        {
            case MenuState.HoldingNewShip:
                
                if (_tileSystem.PlaceFriendlyShip(selectedObject, tilePosition))
                {
                    state = MenuState.Idle;
                    Destroy(selectedObject);
                    _tileSystem.SelectTiles(new Vector2Int[0], new Vector2Int[0, 0]);
                }
                break;
            case MenuState.Idle:
                // if you aren't holding something, you probably want to see info about a ship
                if (_tileSystem.IsTilePointInBounds(tilePosition))
                {
                    TileOccupier selectedShip = _tileSystem.GetSelectedShip(tilePosition);
                    if (selectedShip != null)
                    {
                        selectedObject = selectedShip.gameObject;
                        BoatBehavior boatBehavior = selectedShip.GetComponent<BoatBehavior>();
                        switch (boatBehavior.GetType().Name)
                        {
                            case "Defender1Behaviour":
                                state = MenuState.SelectingDefender1;
                                break;
                            case "Defender2Behaviour":
                                state = MenuState.SelectingDefender2;
                                break;
                            case "Defender3Behaviour":
                                state = MenuState.SelectingDefender3;
                                break;
                            case "Enemy1Behaviour":
                                break;
                            case "BaseBehaviour":
                                break;
                            default:
                                throw new System.Exception("Can't find boat type");

                        }
                    }
                }
                
                break;
            default:
                // this is for clicking out of the menu for a selected ship
                if (_tileSystem.IsTilePointInBounds(tilePosition) && !_tileSystem.isPaused)
                {
                    state = MenuState.Idle;
                }
                break;
        }
        
    }

    public void OnSecondaryClick()
    {
        // Rotate the ship
        if (state == MenuState.HoldingNewShip)
        {
            TileOccupier occupier = selectedObject.GetComponent<TileOccupier>();
            occupier.rotation = (OccupierRotation) (((int)occupier.rotation + 90) % 360);
            occupier.gameObject.transform.Rotate(Vector3.back, 90);

            // Update Selected Tiles
            Vector2Int tilePosition = _tileSystem.WorldToTilePoint(worldMousePosition);
            BoatBehavior behavior = selectedObject.GetComponent<BoatBehavior>();
            _tileSystem.SelectTiles(occupier.GetTilesOccupied(tilePosition), behavior.GetFiringRange(occupier.GetFocusCoordinate(tilePosition)));
        }
    }

    // toggle pause menu
    public void OnMenuPress()
    {
        // pause
        if (!_tileSystem.isPaused)
        {
            _tileSystem.isPaused = true;
            if (_pauseMenu == null)
            {
                _pauseMenu = Instantiate(pauseMenuPrefab, _pauseMenuParent.transform);
            } else
            {
                _pauseMenu.SetActive(true);
            }
        }
        // resume
        else
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        _pauseMenu.SetActive(false);
        _tileSystem.isPaused = false;
    }
}

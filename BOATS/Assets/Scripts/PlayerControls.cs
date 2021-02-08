using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControls : MonoBehaviour
{
    public GameObject heldObject;

    private TileSystem _tileSystem;

    // Start is called before the first frame update
    void Start()
    {
        _tileSystem = FindObjectOfType<TileSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseMoved(InputValue positionValue)
    {
        // "Ghost" ship underneath the player's mouse when selected to place
        if (heldObject != null)
        {
            Vector2 position = positionValue.Get<Vector2>();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            heldObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, -1);
        }
    }

    public void OnPrimaryClick()
    {
        if (heldObject != null)
        {
            Vector2 position = Pointer.current.position.ReadValue();
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
            Vector2Int tilePosition = _tileSystem.TranslateCoordinatesToTile(worldPosition);

            _tileSystem.PlaceShip(heldObject, tilePosition);
        }
    }

    public void OnSecondaryClick()
    {
        // Rotate the ship
        if (heldObject != null)
        {
            TileOccupier occupier = heldObject.GetComponent<TileOccupier>();
            occupier.rotation = (OccupierRotation) (((int)occupier.rotation + 90) % 360);
            occupier.gameObject.transform.Rotate(Vector3.back, 90);
        }
    }
}

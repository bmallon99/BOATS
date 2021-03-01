using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defender6Behaviour : BoatBehavior
{
    public float probability;
    public GameObject moneyPrefab;

    private TileOccupier tileOccupier;
    private GameObject moneyParent;
    private GameObject lastSpawnedMoney;

    protected override bool Move() => false;
    protected override bool Attack() => false;

    protected void Start()
    {
        tileOccupier = GetComponent<TileOccupier>();
        moneyParent = GameObject.FindGameObjectWithTag("Money Layer");
    }

    public override void Turn()
    {
        if (Random.Range(0f, 1f) < probability && lastSpawnedMoney == null)
        {
            Debug.Log(MoneyPosition());
            lastSpawnedMoney = Instantiate(moneyPrefab, MoneyPosition(), new Quaternion(), moneyParent.transform);
        }
    }

    public override Vector2Int[,] GetFiringRange(Vector2Int position)
    {
        return new Vector2Int[0,0];
    }

    private Vector3 MoneyPosition()
    {
        switch (tileOccupier.rotation)
        {
            case OccupierRotation.Zero:
                return new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);

            case OccupierRotation.Clockwise90:
                return new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);

            case OccupierRotation.Clockwise180:
                return new Vector3(transform.position.x - 2, transform.position.y - 1, transform.position.z);

            case OccupierRotation.Clockwise270:
                return new Vector3(transform.position.x - 1, transform.position.y + 1, transform.position.z);

            default:
                throw new System.Exception($"Invalid TileOccupier Rotation {tileOccupier.rotation}");
        }
    }
}

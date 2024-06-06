using UnityEngine;

public sealed class TileRow : MonoBehaviour
{
    [SerializeField]
    private TileCell[] cells;

    public TileCell[] Cells => cells;
}

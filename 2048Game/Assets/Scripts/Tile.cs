using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class Tile : MonoBehaviour
{
    [SerializeField]
    private TileState tileState;

    [SerializeField]
    private TileCell cell;

    [SerializeField]
    private int number;

    [SerializeField]
    private Image background;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private bool locked;

    public TileState TileState => tileState;
    public TileCell Cell => cell;
    public int Number => number;
    public bool Locked { get { return locked; } set { locked = value; } }

    public void SetState(TileState state, int number)
    {
        tileState = state;
        this.number = number;

        text.text = number.ToString();

        background.color = state.backgroundColor;
        text.color = state.textColor;
    }

    public void Spawn(TileCell cell)
    {
        if (this.cell != null)
            this.cell.tile = null;

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public void MoveToCell(TileCell cell)
    {
        if (this.cell != null)
            this.cell.tile = null;

        this.cell = cell;
        this.cell.tile = this;

        StartCoroutine(Animate(cell.transform.position, false));
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
            this.cell.tile = null;

        this.cell = null;
        cell.tile.locked = true;

        StartCoroutine(Animate(cell.transform.position, true));
    }

    private IEnumerator Animate(Vector3 destination, bool merging)
    {
        float elapsedTime = 0f;
        float duration = 0.1f;

        Vector3 start = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, destination, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;

        if (merging) Destroy(gameObject);
    }
}
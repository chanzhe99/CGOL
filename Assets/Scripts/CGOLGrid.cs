using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CGOLGrid : MonoBehaviour
{
    Vector2 GridStartPosition = new Vector2(-5.0f, -5.0f); // Start position for first cell
    float CellSize; // Size of individual cell

    #region Grid
    [Header("Grid")]
    [Space(5)]
    [SerializeField] int GridCountMin = 50;
    [SerializeField] int GridCountMax = 150;
    [SerializeField] [Range(50, 150)] int GridCountSize = 50;
    [SerializeField] [Range(50, 150)] int GridCountCurrent = 50;
    [SerializeField] GameObject CellPrefab;
    Cell[,] CellArray;
    bool[,] NextCellArray;
    bool rand;
    #endregion

    #region Sim Speed
    [Header("Simulation Speed")]
    [Space(5)]
    [SerializeField] float SimSpeedMin = 1.0f;
    [SerializeField] float SimSpeedMax = 60.0f;
    [SerializeField] [Range(1.0f, 60.0f)] float SimSpeed = 7.0f;
    bool IsPaused = true;
    bool clickedAlive = false;
    float Elapsed = 0.0f;
    #endregion

    #region UI
    [Header("UI")]
    [Space(5)]
    [SerializeField] TMP_Text PlayPauseText;

    [SerializeField] Slider SimSpeedSlider;
    [SerializeField] TMP_Text SimSpeedText;

    [SerializeField] Slider GridSizeSlider;
    [SerializeField] TMP_Text GridSizeText;
    [SerializeField] TMP_Text GridApplyText;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        CellArray = new Cell[GridCountSize, GridCountSize];
        NextCellArray = new bool[GridCountSize, GridCountSize];
        GridCountCurrent = GridCountSize;
        InitialiseGrid(GridCountSize);

        #region UI
        PlayPauseText.SetText("Play");

        GridSizeSlider.minValue = GridCountMin;
        GridSizeSlider.maxValue = GridCountMax;
        GridSizeSlider.value = GridCountSize;
        GridSizeSlider.onValueChanged.AddListener(delegate { ChangedGridSize(); });
        GridSizeText.SetText(GridCountCurrent.ToString("New Size: 0"));
        GridApplyText.SetText(GridCountCurrent.ToString("Size: 0\nApply"));

        SimSpeedSlider.minValue = SimSpeedMin;
        SimSpeedSlider.maxValue = SimSpeedMax;
        SimSpeedSlider.value = 7.0f;
        SimSpeedText.SetText(SimSpeed.ToString("Sim Speed: 0.0/s"));
        #endregion
    }

    void InitialiseGrid(int GridCount)
    {
        CellSize = 10.0f / GridCount;
        for (int y = 0; y < GridCount; y++)
        {
            for (int x = 0; x < GridCount; x++)
            {
                CellArray[x, y] = Instantiate(CellPrefab, GridStartPosition + new Vector2(CellSize * x, CellSize * y), Quaternion.identity).GetComponent<Cell>();
                //CellArray[x, y].SetScale(CellSize);
                CellArray[x, y].transform.localScale = new Vector2(CellSize, CellSize);

                rand = (Random.value >= 0.5) ? false : true;
                CellArray[x, y].SetIsAlive(rand);
                //CellArray[x, y].SetIsAlive(false);
                NextCellArray[x, y] = rand;
            }
        }
    }

    int GetLivingNeighbours(int x, int y)
    {
        int count = 0;
        
        // Check cell on the top left
        if (x != 0 && y != GridCountCurrent - 1)
            if (CellArray[x - 1, y + 1].GetIsAlive())
                count++;

        // Check cell on the top center
        if (y != GridCountCurrent - 1)
            if (CellArray[x, y + 1].GetIsAlive())
                count++;

        // Check cell on the top right
        if (x != GridCountCurrent - 1 && y != GridCountCurrent - 1)
            if(CellArray[x + 1, y + 1].GetIsAlive())
                count++;

        // Check cell on the left
        if (x != 0)
            if (CellArray[x - 1, y].GetIsAlive())
                count++;

        // Check cell on the right
        if (x != GridCountCurrent - 1)
            if (CellArray[x + 1, y].GetIsAlive()) 
                count++;

        // Check cell on the bottom left
        if (x != 0 && y != 0)
            if (CellArray[x - 1, y - 1].GetIsAlive()) 
                count++;

        // Check cell on the bottom center
        if (y != 0)
            if (CellArray[x, y - 1].GetIsAlive())
                count++;

        // Check cell on the bottom right
        if (x != GridCountCurrent - 1 && y != 0)
            if (CellArray[x + 1, y - 1].GetIsAlive()) 
                count++;

        return count;
    }

    void SetNextState(int xCount, int yCount)
    {
        for (int y = 0; y < yCount; y++)
        {
            for (int x = 0; x < xCount; x++)
            {
                CellArray[x, y].SetIsAlive(NextCellArray[x, y]);
            }
        }
    }

    void ChangeGridSize(int GridCountNew)
    {
        for (int y = 0; y < GridCountCurrent; y++)
        {
            for (int x = 0; x < GridCountCurrent; x++)
            {
                Destroy(CellArray[x, y].gameObject);
                CellArray[x, y] = null;

                NextCellArray[x, y] = false;
            }
        }

        CellArray = new Cell[GridCountNew, GridCountNew];
        NextCellArray = new bool[GridCountNew, GridCountNew];
        InitialiseGrid(GridCountNew);
        GridCountCurrent = GridCountNew;
        GridApplyText.SetText(GridCountCurrent.ToString("Size: 0\nApplied"));
    }

    void ChangedGridSize()
    {
        GridApplyText.SetText(GridCountCurrent.ToString("Size: 0\nApply"));
    }

    public void ButtonPlayPause()
    {
        if (!IsPaused)
        {
            IsPaused = true;
            PlayPauseText.SetText("Play");
        }
        else
        {
            IsPaused = false;
            PlayPauseText.SetText("Pause");
        }
    }

    public void ButtonMapRandomise()
    {
        for (int y = 0; y < GridCountCurrent; y++)
        {
            for (int x = 0; x < GridCountCurrent; x++)
            {
                rand = (Random.value >= 0.5) ? false : true;
                CellArray[x, y].SetIsAlive(rand);
                NextCellArray[x, y] = rand;
            }
        }
    }

    public void ButtonMapReset()
    {
        for (int y = 0; y < GridCountCurrent; y++)
        {
            for (int x = 0; x < GridCountCurrent; x++)
            {
                CellArray[x, y].SetIsAlive(false);
                NextCellArray[x, y] = false;
            }
        }
    }

    public void SliderGridSize()
    {
        GridCountSize = (int)GridSizeSlider.value;
        GridSizeText.SetText(GridCountSize.ToString("New Size: 0"));
    }

    public void ButtonGridResize()
    {
        ChangeGridSize(GridCountSize);
    }

    public void SliderSimSpeed()
    {
        SimSpeed = SimSpeedSlider.value;
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        
        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                if (!hit.collider.gameObject.GetComponent<Cell>().GetIsAlive())
                {
                    hit.collider.gameObject.GetComponent<Cell>().SetIsAlive(true);
                    clickedAlive = false;
                }
                else
                {
                    hit.collider.gameObject.GetComponent<Cell>().SetIsAlive(false);
                    clickedAlive = true;
                }
            }
        }

        if(Input.GetMouseButton(0) && !clickedAlive)
        {
            if(hit.collider != null)
            {
                hit.collider.gameObject.GetComponent<Cell>().SetIsAlive(true);
            }
        }
        else if(Input.GetMouseButton(0) && clickedAlive)
        {
            if (hit.collider != null)
            {
                hit.collider.gameObject.GetComponent<Cell>().SetIsAlive(false);
            }
        }

        SimSpeedText.SetText(SimSpeed.ToString("Sim Speed: 0.0/s"));

        // Slows down update cycle for human eyes
        Elapsed += Time.deltaTime;
        if (Elapsed < 1/SimSpeed) return;
        Elapsed -= 1/SimSpeed;
        
        if(!IsPaused)
        {
            for (int y = 0; y < GridCountCurrent; y++)
            {
                for (int x = 0; x < GridCountCurrent; x++)
                {
                    // Check the cell's current state, and counts it's living neighbours
                    bool alive = CellArray[x, y].GetIsAlive();
                    int count = GetLivingNeighbours(x, y);
                    bool result = false;

                    // Apply CGOL rules and set the next state
                    if (alive && count < 2)
                        result = false;
                    else if (alive & (count == 2 || count == 3))
                        result = true;
                    else if (alive && count > 3)
                        result = false;
                    else if (!alive && count == 3)
                        result = true;

                    NextCellArray[x, y] = result;
                }
            }

            SetNextState(GridCountCurrent, GridCountCurrent);
        }
    }

}

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AStar
/// 启发式搜索
/// 为每个节点计绑定了一个估计值 遍历时取最优的
/// </summary>
public class AStar : MonoBehaviour
{
    public int xCount = 10;
    public int zCount = 10;
    public float space = 0.5f;
    // 支持斜方向
    public bool oblique;
    // 斜方向有block是否可以走
    public bool obliqueBlock;
    public bool showUIPos;
    public GameObject uiPos;
    public Transform uiPosRoot;

    Grid start, end;
    List<Grid> grids, blocks;
    List<Grid> open = new List<Grid>();
    Queue<Grid> close = new Queue<Grid>();

    class Grid : MonoBehaviour
    {
        public int x, y, z;
        public bool block;
        public Grid parent;
        public float G;
        public float H;
        public float F => G + H;
        // 绘制探索区域，共有的邻居只会被一个节点引用防止重复绘制
        public Queue<Grid> neighborRecord = new Queue<Grid>(4);

        public void setColor(Color color)
        {
            gameObject.GetComponent<Renderer>().material.color = color;
        }

        public void Reset()
        {
            G = 0;
            H = 0;
            block = false;
            setColor(Color.white);
            neighborRecord.Clear();
        }

        public void info()
        {
            Debug.Log($"({x}, {y}, {z}) [G = {G}, H = {H}, F = {F}]", this);
        }
    }

    private void Start()
    {
        // init
        grids = new List<Grid>(xCount * zCount);
        blocks = new List<Grid>(xCount * zCount / 2 / 2);
        // generate grids
        GameObject gridsObj = new GameObject("Grids");
        gridsObj.transform.position = Vector3.zero;
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < zCount; j++)
            {
                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.parent = gridsObj.transform;
                go.transform.localPosition = new Vector3(i * (1 + space), 0, j * (1 + space));
                go.name = $"({j},0,{i})";
                Grid grid = go.AddComponent<Grid>();
                grid.x = i;
                grid.z = j;
                grid.setColor(Color.white);
                grids.Add(grid);
            }
        }
        // set camera position
        var camera = Camera.main;
        float offset = 0.5f;
        float screenScale = 1f * Screen.height / Screen.width;
        float miniMapLeftX = getGrid(0, 0, 0).transform.position.x - offset;
        float miniMapLeftZ = getGrid(0, 0, 0).transform.position.z - offset;
        float rightX = getGrid(xCount - 1, 0, zCount - 1).transform.position.x + offset;
        float rightZ = getGrid(xCount - 1, 0, zCount - 1).transform.position.z + offset;
        float width = Mathf.Abs(miniMapLeftX) + Mathf.Abs(rightX);
        float height = Mathf.Abs(miniMapLeftZ) + Mathf.Abs(rightZ);
        float mapScale = height / width;
        camera.transform.position = new Vector3((miniMapLeftX + rightX) / 2, (width > height ? width : height) / 2, (miniMapLeftZ + rightZ) / 2);
        if (mapScale >= screenScale)
        {
            camera.orthographicSize = height / 2;
        }
        else
        {
            // camera.orthographicSize = width / 2 * mapScale;
            camera.orthographicSize = width / 2 * screenScale;
        }

        // show ui pos
        if (showUIPos && uiPos && uiPosRoot)
        {
            uiPos.SetActive(true);
            grids.ForEach((grid) =>
            {
                var uiPosGo = Instantiate(uiPos, uiPosRoot);
                uiPosGo.transform.position = RectTransformUtility.WorldToScreenPoint(camera, grid.transform.position);
                uiPosGo.GetComponent<Text>().text = grid.name;
                uiPosGo.GetComponent<Text>().fontSize = (int)(120 / camera.orthographicSize);
                // uiPosGo.GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;
            });
            uiPos.SetActive(false);
        }
    }

    // cache current clicking block
    private Grid lastBlockClickGrid;

    private void Update()
    {
        handlerGridInit();
    }

    private void handlerGridInit()
    {
        // s button and left mouse button click is start grid
        if (Input.GetKey(KeyCode.S) && Input.GetMouseButtonDown(0))
        {
            Grid grid = getMouseClickGrid();
            if (grid == null || start != null || grid == end)
                return;
            start = grid;
            if (blocks.Contains(start))
            {
                start.block = false;
                blocks.Remove(start);
            }
            grid.setColor(Color.green);

        }
        // e button and left mouse button click is end grid
        else if (Input.GetKey(KeyCode.E) && Input.GetMouseButtonDown(0))
        {
            Grid grid = getMouseClickGrid();
            if (grid == null || end != null || grid == start)
                return;
            end = grid;
            if (blocks.Contains(end))
            {
                end.block = false;
                blocks.Remove(end);
            }
            grid.setColor(Color.red);
        }
        // left mouse button click is block grid
        else if (Input.GetMouseButton(0))
        {
            Grid grid = getMouseClickGrid();
            if (grid == null || grid == start || grid == end || grid == lastBlockClickGrid)
                return;
            grid.block = !grid.block;
            if (blocks.Contains(grid))
            {
                blocks.Remove(grid);
                grid.setColor(Color.white);
            }
            else
            {
                blocks.Add(grid);
                grid.setColor(Color.black);
            }
            lastBlockClickGrid = grid;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            lastBlockClickGrid = null;
        }
        // control and r button is clear start and end grid state
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.R))
        {
            clearPath();
            clearStartAndEnd();
        }
        // alt and r button is clear blocks grid state
        else if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.R))
        {
            clearPath();
            clearBlocks();
        }
        // r button is clear all grid state
        else if (Input.GetKeyDown(KeyCode.R))
        {
            clearPath();
            clearBlocks();
            clearStartAndEnd();
        }
        // space button is start find path
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (start == null || end == null)
                return;
            clearPath();
            calcuPath();
        }
    }

    private Grid getMouseClickGrid()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit))
            return rayHit.collider.gameObject.GetComponent<Grid>();
        return null;
    }

    private void calcuPath()
    {
        open.Add(start);
        while (open.Count > 0)
        {
            Grid current = getMinFGrid();
            open.Remove(current);
            close.Enqueue(current);
            // find neighbor
            // [-1, 1] [0, 1] [1, 1]
            // [-1, 0] [0, 0] [1, 0]
            // [-1,-1] [0,-1] [1,-1]
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    // 支持斜方向
                    if (!oblique && MathF.Abs(i) == MathF.Abs(j))
                        continue;
                    // print($"{i}, {j}");
                    // 邻居获取顺序  → ↑ ↓ ← 
                    Grid neighbor = getGrid(current.x + i * -1, 0, current.z + j * -1);
                    if (neighbor == null || blocks.Contains(neighbor) || close.Contains(neighbor))
                        continue;
                    // 斜方向两边是否有block
                    if (oblique && !obliqueBlock &&
                        (blocks.Contains(getGrid(current.x, 0, neighbor.z)) || blocks.Contains(getGrid(neighbor.x, 0, current.z))))
                        continue;
                    if (neighbor == end)
                    {
                        end.parent = current;
                        showPath();
                        return;
                    }
                    // 移动一次需要的消耗，todo 如果为斜着走(路更艰难)消耗更大
                    int moveConsume = 1;
                    // 已经在待搜寻列表且从当前到邻格子的F优于之前的F
                    if (open.Contains(neighbor))
                    {
                        if (current.G + moveConsume + neighbor.H < neighbor.F)
                        {
                            neighbor.G = current.G + moveConsume;
                            neighbor.parent = current;
                        }
                        // ignore repeat
                        continue;
                    }
                    else
                    {
                        neighbor.G = current.G + moveConsume;
                        neighbor.H = getToEndDist(neighbor);
                        neighbor.parent = current;
                        open.Add(neighbor);
                    }
                    // ignore end
                    // ignore repeat
                    current.neighborRecord.Enqueue(neighbor);
                }
            }
        }
        Debug.LogWarning("finder failed");
        showPath();
    }

    private float getToEndDist(Grid start)
    {
        return MathF.Abs(end.x - start.x) + MathF.Abs(end.z - start.z);
    }

    private Grid getMinFGrid()
    {
        Grid min = open[0];
        for (int i = open.Count - 1; i > 0; i--)
        {
            if (open[i].F < min.F)
                min = open[i];
        }
        return min;
    }

    private Grid getGrid(int x, int y, int z)
    {
        return grids.Find((g) => g.x == x && g.y == y && g.z == z);
    }

    private CancellationTokenSource cts;
    private List<Grid> waitClearGrid = new List<Grid>();
    private async void showPath()
    {
        waitClearGrid.Clear();
        Stack<Grid> stack = new Stack<Grid>();
        Grid lastNode = end.parent;
        while (lastNode != start && lastNode != null)
        {
            stack.Push(lastNode);
            lastNode = lastNode.parent;
        }

        // tween show
        cts = new CancellationTokenSource();
        // show search path
        while (close.Count > 0)
        {
            var neighbor = close.Dequeue().neighborRecord;
            while (neighbor.Count > 0)
            {
                var grid = neighbor.Dequeue();
                grid.setColor(Color.gray);
                waitClearGrid.Add(grid);
                var isCanceled = await UniTask.Delay(200, cancellationToken: cts.Token).SuppressCancellationThrow();
                if (isCanceled)
                    return;
            }
        }
        // show path
        while (stack.Count > 0)
        {
            var grid = stack.Pop();
            grid.setColor(Color.yellow);
            waitClearGrid.Add(grid);
            var isCanceled = await UniTask.Delay(200, cancellationToken: cts.Token).SuppressCancellationThrow();
            if (isCanceled)
                return;
        }
    }

    private void clearBlocks()
    {
        blocks.ForEach(grid => grid.Reset());
        blocks.Clear();
    }

    private void clearStartAndEnd()
    {
        start?.Reset();
        end?.Reset();
        start = null;
        end = null;
    }

    private void clearPath()
    {
        open.Clear();
        close.Clear();
        cts?.Cancel();
        waitClearGrid.ForEach(grid => grid.Reset());
    }
}

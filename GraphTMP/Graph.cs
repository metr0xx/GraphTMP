namespace GraphTMP;

public class Graph
{
    private readonly List<List<int>> _adjacencyMatrix;
    private readonly List<List<MatrixItem>> _vertexMatrix = [];
    private readonly List<Arc> _arcs = [];

    public Graph(List<List<int>> adjacencyMatrix)
    {
        if (adjacencyMatrix.Count == 0 || adjacencyMatrix.Any(row => row.Count != adjacencyMatrix.Count)
            || adjacencyMatrix.Find(e => e.Any(item => item < -1 || item > 1)) != null) throw new Exception("Задана некорректная матрица смежности");

        _adjacencyMatrix = adjacencyMatrix;
        FillVertexMatrix();
        FillArcs();
    }

    /// <summary>
    /// возвращает индекс первой вершины, смежной с вершиной v. Если вершина v не имеет смежных вершин, то возвращается "нулевая" вершина
    /// </summary>
    public int First(string vertexName)
    {
        var foundRow = _vertexMatrix.FirstOrDefault(r => r[0].RowName == vertexName) ?? throw new Exception("Вершина не найдена");

        for (int i = 0; i < foundRow.Count; i++)
        {
            if (foundRow[i].Value != 0) return GetIndex(foundRow[i].ColumnName);
        }

        return 0;
    }

    /// <summary>
    /// возвращает индекс вершины, смежной с вершиной v, следующий за индексом i. Если i — это индекс последней вершины, смежной с вершиной v, то возвращается "нулевая" вершина
    /// </summary>
    public int Next(string vertexName, int i)
    {
        var foundRow = _vertexMatrix.FirstOrDefault(r => r[0].RowName == vertexName) ?? throw new Exception("Вершина не найдена");
        bool hasVertex = false;
        for (int j = 0; j < foundRow.Count; j++)
        {
            if (hasVertex && foundRow[j].Value != 0) return GetIndex(foundRow[j].ColumnName);
            hasVertex = (foundRow[j].Value != 0 && GetIndex(foundRow[j].ColumnName) == i);
        }

        return 0;
    }

    /// <summary>
    /// возвращает вершину с индексом i из множества вершин, смежных с v
    /// </summary>
    public string Vertex(string vertexName, int i)
    {
        var foundRow = _vertexMatrix.FirstOrDefault(r => r[0].RowName == vertexName) ?? throw new Exception("Вершина не найдена");
        for (int j = 0; j < foundRow.Count; j++)
        {
            if (foundRow[j].Value != 0 && GetIndex(foundRow[j].ColumnName) == i) return foundRow[j].ColumnName;
        }

        throw new Exception($"Смежная вершина с индексом {i} не найдена");
    }

    /// <summary>
    /// добавить узел
    /// </summary>
    public void AddVertex(string vertexName)
    {
        if (_vertexMatrix.FirstOrDefault(r => r[0].RowName == vertexName) != null) throw new Exception("Узел уже существует");

        List<int> newRow = [];
        var r = new Random();

        _adjacencyMatrix.ForEach(_ => newRow.Add(r.Next(-1, 1)));
        newRow.Add(0);

        for (int i = 0; i < _adjacencyMatrix.Count; i++)
        {
            _adjacencyMatrix[i].Add(newRow[i] * -1);
        }

        _adjacencyMatrix.Add(newRow);

        for (int i = 0; i < _vertexMatrix.Count; i++)
        {
            _vertexMatrix[i].Add(new MatrixItem()
            {
                RowName = _vertexMatrix[i][0].RowName,
                ColumnName = vertexName,
                Value = _adjacencyMatrix[^1][i]
            });
        }

        _vertexMatrix.Add([]);

        for (int i = 0; i < _adjacencyMatrix.Count; i++)
        {
            _vertexMatrix[^1].Add(new MatrixItem()
            {
                RowName = vertexName,
                ColumnName = _vertexMatrix[0][i].ColumnName,
                Value = newRow[i]
            });
        }

        foreach (var col in _vertexMatrix[^1])
        {
            _arcs.Add(new Arc()
            {
                From = col.Value == 1 ? col.RowName : col.ColumnName,
                To = col.Value == 1 ? col.ColumnName : col.RowName,
                Weight = 1
            });
        }
    }

    /// <summary>
    /// добавить дугу
    /// </summary>
    public void AddArc(string from, string to, int weight)
    {
        if (_arcs.FirstOrDefault(arc => arc.From == from && arc.To == to) != null) throw new Exception("Дуга уже существует");

        for (int i = 0; i < _vertexMatrix.Count; i++)
        {
            if (_vertexMatrix[i][0].RowName == from)
            {
                for (int j = 0; j < _vertexMatrix[i].Count; j++)
                {
                    if (_vertexMatrix[i][j].ColumnName == to)
                    {
                        _vertexMatrix[i][j].Value = 1;
                        _vertexMatrix[j][i].Value = -1;
                        _arcs.Add(new Arc()
                        {
                            From = from,
                            To = to,
                            Weight = weight
                        });

                        return;
                    }
                }
            }
        }

        throw new Exception("Узел не найден");
    }

    /// <summary>
    /// удалить узел
    /// </summary>
    public void DeleteVertex(string vertexName)
    {
        _vertexMatrix.Remove(_vertexMatrix.Find(row => row[0].RowName == vertexName) ?? throw new Exception("Узел не найден"));
        _vertexMatrix.ForEach(row => row.Remove(row.Find(e => e.ColumnName == vertexName) ?? throw new Exception("Узел не найден")));
        _arcs.RemoveAll(arc => arc.From == vertexName || arc.To == vertexName);
    }

    /// <summary>
    /// удалить дугу
    /// </summary>
    public void DeleteArc(string from, string to)
    {
        _arcs.Remove(_arcs.Find(arc => arc.From == from && arc.To == to) ?? throw new Exception("Дуга не найдена"));

        for (int i = 0; i < _vertexMatrix.Count; i++)
        {
            if (_vertexMatrix[i][0].RowName == from)
            {
                for (int j = 0; j < _vertexMatrix[i].Count; j++)
                {
                    if (_vertexMatrix[i][j].ColumnName == to)
                    {
                        _vertexMatrix[i][j].Value = _vertexMatrix[j][i].Value == 1 ? -1 : 0;
                        _vertexMatrix[j][i].Value = _vertexMatrix[i][j].Value == 0 ? 0 : 1;
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// изменить метку (маркировку) узла
    /// </summary>
    public void EditVertex(string name, string newName)
    {
        _vertexMatrix.ForEach(row => (row.Find(e => e.ColumnName == name) ?? throw new Exception("Узел не найден")).ColumnName = newName);
        (_vertexMatrix.Find(row => row[0].RowName == name) ?? throw new Exception("Узел не найден")).ForEach(v => v.RowName = newName);
        _arcs.ForEach(arc => arc.From = arc.From == name ? newName : arc.From);
        _arcs.ForEach(arc => arc.To = arc.To == name ? newName : arc.To);
    }

    /// <summary>
    /// изменить вес дуги
    /// </summary>
    public void EditArcWeight(string from, string to, int weight) => (_arcs.Find(arc => arc.From == from && arc.To == to) ?? throw new Exception("Дуга не найдена")).Weight = weight;

    /// <summary>
    /// Определить, есть ли какой-либо путь, проходящий через ВСЕ вершины орграфа,
    /// причем через вершину можно проходить только один раз,
    /// а начальная и конечная вершины не должны быть смежными, и вывести его на экран
    /// </summary>
    public bool HasWay()
    {
        var graph = GetGraph();
        var destinations = Enumerable.Range(0, _vertexMatrix[0].Count).ToList();

        foreach (var from in graph.Keys)
        {
            foreach (var to in destinations)
            {
                if (graph[from].Contains(to) || from == to) continue;
                if (DFS(graph, from, to)) return true;
            }
        }

        return false;
    }

    /// <summary>
    /// алгоритм обхода графа в глубину
    /// </summary>
    private bool DFS(Dictionary<int, List<int>> graph, int from, int to, List<int>? visited = null)
    {
        if (visited is null)
        {
            visited = [];
            _vertexMatrix[0].ForEach(_ => visited.Add(0));
        }

        if (from == to && visited.Sum() == _vertexMatrix[0].Count) return true;
        if (visited[from] == 1) return false;

        visited[from]++;

        foreach (var neighbor in graph[from])
        {
            if (DFS(graph, neighbor, to, visited)) return true;
        }

        return false;
    }

    /// <summary>
    /// получить представление графа
    /// </summary>
    public Dictionary<int, List<int>> GetGraph()
    {
        Dictionary<int, List<int>> graph = [];

        for (int i = 0; i < _vertexMatrix[0].Count; i++)
        {
            graph.Add(i, []);

            for (int j = 0; j < _vertexMatrix[0].Count; j++)
            {
                if (_vertexMatrix[i][j].Value == 1) graph[i].Add(j);
            }

            if (graph[i].Count == 0) graph.Remove(i);
        }

        return graph;
    }

    /// <summary>
    /// вывести граф
    /// </summary>
    public void PrintGraph()
    {
        var graph = GetGraph();

        foreach (var val in graph)
        {
            Console.Write($"{val.Key}: ");

            foreach (var item in val.Value) Console.Write($"{item}; ");
            Console.WriteLine();
        }
    }

    /// <summary>
    /// вывести все дуги графа
    /// </summary>
    public void PrintArcs()
    {
        foreach (var arc in _arcs) Console.WriteLine($"from {arc.From} to {arc.To} with weight = {arc.Weight}");
    }

    /// <summary>
    /// получить индекс вершины
    /// </summary>
    private int GetIndex(string vertexName) => (_vertexMatrix.Find(r => r[0].RowName == vertexName) ?? throw new Exception("Вершина не найдена")).Sum(v => v.Value);

    /// <summary>
    /// инициализация матрицы графа
    /// </summary>
    private void FillVertexMatrix()
    {
        for (int i = 0; i < _adjacencyMatrix.Count; i++)
        {
            _vertexMatrix.Add([]);

            for (int j = 0; j < _adjacencyMatrix[i].Count; j++)
            {
                if (j == i && _adjacencyMatrix[i][j] != 0 || Math.Abs(_adjacencyMatrix[i][j]) == 1 && _adjacencyMatrix[j][i] == 0) throw new Exception("Задана некорректная матрица смежности");
                _vertexMatrix[i].Add(new MatrixItem()
                {
                    RowName = ((char)(i + 97)).ToString(),
                    ColumnName = ((char)(j + 97)).ToString(),
                    Value = _adjacencyMatrix[i][j]
                });
            }
        }
    }

    /// <summary>
    /// инициализация дуг графа
    /// </summary>
    private void FillArcs()
    {
        _arcs.Clear();
        foreach (var row in _vertexMatrix)
        {
            foreach (var col in row)
            {
                if (col.Value == 0) continue;

                _arcs.Add(new Arc()
                {
                    From = col.Value == 1 ? col.RowName : col.ColumnName,
                    To = col.Value == 1 ? col.ColumnName : col.RowName,
                    Weight = 1
                });
            }
        }
    } 
}

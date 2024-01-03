using GraphTMP;

//var g = new Graph([
//    [0, 0, 0, 0, 1],
//    [1, 0, 0, 0, 0],
//    [1, 0, 0, 0, 1],
//    [0, 1, 0, 0, 1],
//    [1, 1, 1, 0, 0]]);


// ПРИМЕР 1
var g1 = new Graph([
    [0, 1, 1, 1],
    [1, 0, 1, 0],
    [1, 1, 0, 0],
    [1, 0, 0, 0]]);

// ПРИМЕР 2
var g2 = new Graph([
    [0, 1, 1, 1, 1],
    [1, 0, 1, 0, 0],
    [1, 1, 0, 0, 0],
    [1, 0, 0, 0, 0],
    [1, 0, 0, 0, 0]]);

g1.PrintGraph();
Console.WriteLine(g1.HasWay());
g2.PrintGraph();
Console.WriteLine(g2.HasWay());

//Console.WriteLine(g.First("b"));
//Console.WriteLine(g.Next("e", 2));

//Console.WriteLine(g.Vertex("e", 2));
//g.AddVertex("q");
//g.AddArc("a", "b", 11);
//g.PrintArcs();
//g.DeleteVertex("q");
//g.Print();
//g.DeleteArc("a", "b");
//g.AddArc("a", "c", 5);
//g.PrintArcs();
//g.EditVertex("a", "w");
//g.Print();
//g.EditArcWeight("w", "c", 99);
//g.Print();

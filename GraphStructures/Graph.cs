// ===== Role =====

// This is a consumer-facing class that manages collections
// of GraphNode<T> instances in a Graph structure.

// The relationship between GraphNode<T> and Graph<T> is in part
// analogous to:

// SimpleNode<T> and SinglyLinkedList<T>.
// ComplexNode<T> and DoublyLinkedList<T>.
// GeneralTreeNode<T> and GeneralTree<T>.
// BinaryTreeNode<T> and BinaryTree<T>.

// In each case, we have a dumb, low-level container that is wholly managed
// by another class. The container contains merely a Value and pointers
// to other nodes of the same type (though the nature of the pointers
// varies in arity and directionality across the different kinds of node).
// A node class instance is fully initialised at creation,
// and its Value is non-nullable. 
// Each node class instance belongs to at most one instance of the other class, and
// node insertion always generates a new node instance. Removed
// nodes become unowned and cannot be added to another instance of the other class.

// However, there is an important disanalogy:

// The above four classes that wholly manage a certain node class
// are all internal mechanisms to manage collections
// of the dumb, low-level container nodes. The classes are not 
// intended for direct consumer use, but rather provide a mechanism
// on which higher-level, consumer-facing classes are built.
// The API of these higher-level, consumer-facing classes is only 
// concerned with T value management. The consumer does not have the 
// ability to interact directly with the nodes on which the consumer-facing
// class is built.

// In contrast, Graph<T> provides both the management of GraphNode<T> 
// instances and graph mechanisms, whilst also being the consumer-facing product.

// Consumers *do* have the ability to interact directly with GraphNode<T>
// instances via Graph<T> methods. This is the standard way
// that Graph structures are presented.
// Graph<T> is used by a consumer whenever they want to represent 
// a collection of entities and the connections between them. 
// For example: road maps, computer networks, game NPC pathfinding.

// The nodes themselves are of central importance here, 
// in particular, the topology of the graph.
// T values stored by nodes are less important. They are better considered
// as pieces of metadata.

// As a result, our API methods will have many more checks built into them
// that weren't required for the internal mechanism methods of classes
// like BinaryTree<T>, where I ensured that my calls to those methods were in
// contexts where additional checks are not required.


// Representing Graphs (repeated from GraphNode.cs):

// Graph<T> is node-centric:
// Graph<T> represents a Graph by (i) storing a collection of GraphNode<T> objects
// in a simple HashSet<GraphNode<T>>, where (ii) a GraphNode<T> stores a collection
// of edges as a Dictionary<GraphNode<T>, int> object.

// This approach to representing Graphs supports
// both directed and undirected, and weighted and
// unweighted, graphs using only the Graph<T> class:

// Graph<T> will contain the bool
// properties IsWeighted and IsDirected. 
// It will contain two separate edge adding methods: 
// AddWeightedEdge(GraphNode<T> node1, GraphNode<T> node2, int weight); and 
// AddUnweightedEdge(GraphNode<T> node1, GraphNode<T> node2).

// AddWeightedEdge() will throw an exception if the graph is not weighted.
// AddUnweightedEdge() will throw an exception if the graph is weighted.
// For each method, when an exception is not thrown, the method additionally checks
// whether the graph is directed. If the graph is directed, only node1.Neighbours
// has an entry added.
// If the graph is not directed, both node1.Neighbours and node2.Neighbours have
// an entry added.
// In the case of a non-directed graph, I still classify this as only adding one
// edge in the graph (despite adjusting each of the nodes), in terms of edge count.
// In AddUnweightedEdge(), new edge entries are assigned a weight of 1.

// For generality, Graph<T> also allows for graphs with non-positive edge weights.
// A PositiveEdgesOnly bool is set during Graph<T> instance creation.
// Calling certain algorithms on a Graph<T> instance with a false PositiveEdgesOnly
// will throw an exception. For example, Dijkstra-based algorithms.



// ===== Invariants =====

// Properties:

// IsDirected, IsWeighted, and PositiveEdgesOnly are immutable bools.
// Any combination of values for these properties is permissible,
// with one important exception:
// if IsWeighted == false, then PositiveEdgesOnly == true.
// An exception is thrown is a Graph<T> constructor call is made
// with an inappropriate combination of bool values.
// By default, the constructor sets all three of these properties to true.

// The Nodes property references a HashSet<GraphNode<T>>. 
// Nodes is reference immutable.
// The stored HashSet instance is mutable, but only through calling 
// Graph<T> methods on a Graph<T> instance. A consumer cannot get the HashSet
// directly.

// NodesCount and EdgesCount straightforwardly correspond to the number
// of nodes and edges in the Graph, respectively.


// Value Duplication:

// A Graph<T> is allowed to logically contain multiple GraphNode<T>
// instances with the same Value.
// However, a Graph<T> cannot contain the same GraphNode<T>
// instance in two separate locations.


// Edges:

// At most one edge from node1 to node2.
// If unweighted: weight == 1.
// If weighted: weight inline with PositiveEdgesOnly.
// If undirected: symmetric edges. 
// No loops (an edge from a node to itself).



// ===== Methods =====

// == Graph Management ==

// Clear().
// Removes all nodes from the Graph and sets the NodesCount
// and EdgesCount to 0.
// Time complexity O(n) as we call Clear on the
// HashSet<GraphNode<T>>, which require an iteration 
// through the HashSet to remove references.
// n: total number of nodes in the graph.

// GetNodes().
// Returns the nodes of the Graph in a read-only form.
// Time complexity O(1).

// Contains(GraphNode<T> node).
// Checks whether node is logically part of the Graph.
// Time complexity: average case O(1), worst case O(n), as we 
// call Contains() on the HashSet<GraphNode<T>> this.Nodes,
// which uses a hash-based lookup.

// ContainsEdge(GraphNode<T> node1, GraphNode<T> node2).
// Checks both whether node1 and node2 are in the Graph,
// then also whether node1.Neighbours.ContainsKey(node2).
// Returns true if these all hold, false otherwise.
// Time complexity: average case O(1), worst case O(n), as we 
// call ContainsKey() on the Dictionary<GraphNode<T>, int>
// node1.Neighbours, which uses a hash-based lookup.


// AddNode(T value).
// Adds a node of Value value to the Graph and adjusts NodeCount.
// Returns the created node.
// Time complexity O(1).

// AddWeightedEdge(GraphNode<T> node1, GraphNode<T> node2, int).
// Throws an exception if IsWeighted == false.
// Throws an exception if node1 or node2 is not in the Graph.
// Throws an exception if node1 == node2.
// Throws an exception if node1.Neighbours.ContainsKey(node2) == true.
// Throws an exception if PositiveEdgesOnly == true and int < 1.
// If IsDirected == true, sets node1.Neighbours[node2] to int.
// If IsDirected == false, sets node1.Neighbours[node2] and
// node2.Neighbours[node1] to int.
// Increments EdgesCount by 1.
// Time complexity O(1) average case, O(n) worst case given the
// exception checks.


// AddUnweightedEdge(GraphNode<T> node1, GraphNode<T> node2).
// Throws an exception if IsWeighted == true.
// Throws an exception if node1 or node2 is not in the Graph.
// Throws an exception if node1 == node2.
// Throws an exception if node1.Neighbours.ContainsKey(node2) == true.
// If IsDirected == true, sets node1.Neighbours[node2] to 1.
// If IsDirected == false, sets node1.Neighbours[node2] and
// node2.Neighbours[node1] to 1.
// Increments EdgesCount by 1.
// Time complexity O(1) average case, O(n) worst case given the
// exception checks.

// RemoveEdge(GraphNode<T> node1, GraphNode<T> node2).
// Throws an exception if node1 or node2 are not in the Graph.
// Throws an exception if node1.Neighbours.ContainsKey(node2) == false.
// Removes the edge from node1 to node2.
// If IsDirected == false, also removes the edge from node2 to node1.
// Decrements EdgesCount by 1.
// Time complexity O(1) average case, O(n) worst case given the
// exception checks.

// RemoveNode(GraphNode<T> node).
// Throws an exception if node is not in the Graph.
// Removes all edges that attach to node, then removes
// node from this.Nodes.
// Decrements NodesCount and EdgesCount appropriately.
// Time complexity: average case O(n), worst case O(n^2),
// as we iterate through all nodes and perform one hash-based
// removal for each one.




// == Traversal Algorithms ==

// The class contains two API traversal methods.
// Note: the methods are not deterministic (no guaranteed traversal order),
// as each involves iterating through currentNode.Neighbours.Keys. which
// has no guaranteed order. Traversal order can be fixed by alternatively
// iterating over an ordered version of currentNode.Neighbours.Keys. See the 
// method bodies for details.

// BFS(GraphNode<T> startNode).
// Returns an IEnumerable<GraphNode<T>> starting at startNode.
// Employs my own Queue<T> class.
// Time complexity O(v + e), where 
// v is the number of nodes reachable from startNode.
// e is the number of edges between reachable nodes ie
// the number of traversable edges.
// Space complexity O(v).

// DFS(GraphNode<T> startNode).
// Returns an IEnumerable<GraphNode<T>> starting at startNode.
// Employs my own Stack<T> class explicitly.
// Time complexity: O(v + e).
// Space complexity O(v).

// The class also contains a private traversal method
// for educational purposes:

// DFSRecursive(GraphNode<T> startNode)
// Returns an IEnumerable<GraphNode<T>> starting at startNode.
// Implicitly relies on the call stack by using a recursive 
// helper method.
// Time complexity: O(v + e).
// Space complexity O(v).
// The method is typically less space-intensive than DFS()
// (see the discussion in the method).



// == Shortest Path Algorithms ==


// There are two rich private Dijkstra methods:

// NaiveDijkstra(GraphNode<T> startNode).
// A naive implementation of Dijkstra's algorithm that stores the unvisited
// nodes in an unordered structure. Each loop, we must manually determine
// next currentNode (the unvisited node with the lowest finite tentative distance).
// Returns a (Dictionary<GraphNode<T>, int>, Dictionary<GraphNode<T>, GraphNode<T>>),
// representing (i) a Dictionary of the shortest path weight for each node from startNode, 
// and (ii) a Dictionary of the shortest path for each node from startNode.
// Time complexity O(v^2).

// Dijkstra(GraphNode<T> startNode).
// This has the same signature as NaiveDijkstra(), but is a more efficient 
// implementation of the algorithm. In particular, we use a PriorityQueue to 
// manage the process of selecting the next currentNode.
// Time complexity O(e * log v).


// There are four public Dijkstra methods, which each employ the rich Dijkstra():

// GetShortestDistance(GraphNode<T> startNode, GraphNode<T> endNode).
// Returns an int.

// GetAllShortestDistances(GraphNode<T> startNode).
// Returns a Dictionary<GraphNode<T>, int>.

// GetShortestPath(GraphNode<T> startNode, GraphNode<T> endNode).
// Returns an IEnumerable<GraphNode<T>> by lazily yielding.

// GetAllShortestPaths(GraphNode<T> startNode).
// Returns a Dictionary<GraphNode<T>, List<GraphNode<T>>>.



// As with my implementation of Dijkstra's algorithm, for AStar,
// I include a rich private method, which public methods will employ.
// Private method:

// AStar(GraphNode<T> startNode, GraphNode<T> endNode, Func<GraphNode<T>, GraphNode<T>, int> heuristic)
// This returns a (Dictionary<GraphNode<T>, int>, Dictionary<GraphNode<T>, GraphNode<T>>).
// Time complexity O(e * log v).

// Public methods:

// GetShortestDistanceAStar(GraphNode<T> startNode, GraphNode<T> endNode, Func<GraphNode<T>, GraphNode<T>, int> heuristic).
// Returns an int.

// GetShortestPathAStar(GraphNode<T> startNode, GraphNode<T> endNode, Func<GraphNode<T>, GraphNode<T>, int> heuristic).
// Returns an IEnumerable<GraphNode<T>> by lazily yielding.



// ===== Design Discussion =====

// Node-centric vs Edge-centric (repeated from GraphNode.cs):

// Graph<T> represents a Graph by storing 
// a collection of GraphNode<T> objects.
// Call this approach Option 1. It is node-centric.

// An alternative approach to representing a Graph is to keep a GraphNode<T> as
// merely a container for a Value reference. Then we have two options:

// Option 2: Graph<T> stores an Edges property whose value is
// a data structure that holds (GraphNode<T>, GraphNode<T>, int) tuples, 
// representing edges.

// Option 3: We have a separate GraphEdge<T> class that stores the properties:
// StartNode, EndNode, Weight. Then Graph<T> stores a 
// collection of GraphEdge<T> instances.

// For Options 2 and 3, Graph<T> now stores a collection of 
// (GraphNode<T>, GraphNode<T>, int) tuples.
// This is the edge-centric approach.

// However, with Option 1, specifying a collection of
// nodes is sufficient to specify the graph. This approach is fully general.
// Any kind of topology can be specified.
// With Options 2 and 3, we can specify most kinds of graph by
// specifying a collection of (GraphNode<T>, GraphNode<T>, int) tuples, but
// there is one important exception: 
// we cannot represent a graph with an isolated node.

// So with Option 1, a Graph can be implemented without an explicit Edges
// property. With Options 2 and 3, a Graph can only be implemented
// with both explicit Nodes and Edges properties.

// However, even with Option 1, there is a case for including 
// an Edges property: some Graph algorithms require a list of edges.
// This can be computed ad hoc if we only store a collection of nodes,
// by accessing the Neighbours property of each GraphNode<T> instance,
// but the implementation of the algorithm that requires this 
// ad hoc computation will have an increased time complexity.

// I have chosen Option 1 (to include the Neighbours property in a node),
// to provide symmetry with my other node classes. They all now store 
// a Value reference and pointers to other nodes of the same kind
// (although those pointers are of various arities and directionalities). 
// Further, I will not add an Edges property in Graph<T> as this
// costs extra memory without real reward for my implementation:
// the algorithms I will implement in Graph<T> employ node-centric 
// representation.
// The Graph<T> class can be straightforwardly extended to include an
// Edges property if edge-centric algorithms need to be implemented.

// Access Modifiers:

// The class and all its API methods are public as they are consumer-facing.
// Any purely educational methods are private.

// The bool properties IsDirected, IsWeighted, and PositiveEdgesOnly
// are immutable so have no setter, but they have public getters as consumers
// may want to access these property values.

// NodesCount and EdgesCount have public getters but private setters, as they 
// should only be mutable by the methods of this class.

// Nodes has no setter because it should be reference immutable: 
// we don't want to be able to change which HashSet<GraphNode<T>> the
// Nodes reference points to. 

// Nodes has a private getter so that methods of this class can access the 
// HashSet<GraphNode<T>>. However, we do not want consumers to access this 
// HashSet directly, as they could mutate the object independently of calling a 
// Graph<T> method.
// Instead, I have included a public GetNodes() method that returns
// Nodes as an IReadOnlyCollection<GraphNode<T>>.



using System;
using System.Collections.Generic;


namespace DataStructures
{

public class Graph<T>
{
    private HashSet<GraphNode<T>> Nodes {get; }
    
    // We do not want to expose the Nodes getter to the public, as with just a 
    // getter (no setter), a consumer could internally mutate the HashSet 
    // instance independently of a Graph<T> call on the Graph<T> instance,
    // even though Nodes is reference immutable.
    // Instead, I have included a GetNodes() method with an
    // IReadOnlyCollection<GraphNode<T>> return type.
    public bool IsDirected {get;}   
    public bool IsWeighted {get;}   
    public bool PositiveEdgesOnly {get;} 
    public int NodesCount {get; private set;}
    public int EdgesCount {get; private set;}

    public Graph(bool isDirected = true, bool isWeighted = true, bool positiveEdgesOnly = true)
        {
            if (!isWeighted && !positiveEdgesOnly)
            {
                throw new ArgumentException("PositiveEdgesOnly cannot be false when IsWeighted is false.");
            }
            this.Nodes = new HashSet<GraphNode<T>>();
            this.IsDirected = isDirected;
            this.IsWeighted = isWeighted;
            this.PositiveEdgesOnly = positiveEdgesOnly;
            this.NodesCount = 0;
            this.EdgesCount = 0;
        }

    // Graph Management:
    public void Clear()
        {
            this.Nodes.Clear();
            this.NodesCount = 0;
            this.EdgesCount = 0;
        }

    public IReadOnlyCollection<GraphNode<T>> GetNodes()
        {
            return this.Nodes;
        }

    public bool Contains(GraphNode<T> node)
        {
            return this.Nodes.Contains(node);
        }

    public bool ContainsEdge(GraphNode<T> node1, GraphNode<T> node2)
        {
            return this.Contains(node1) && this.Contains(node2) && node1.Neighbours.ContainsKey(node2);
        }

    public GraphNode<T> AddNode(T value)
        {
            GraphNode<T> newNode = new GraphNode<T>(value);
            this.Nodes.Add(newNode);
            this.NodesCount += 1;
            return newNode;
        }

    public void AddWeightedEdge(GraphNode<T> node1, GraphNode<T> node2, int weight)
        {
            if (!this.IsWeighted)
            {
                throw new InvalidOperationException("This graph is not weighted.");
            }

            if (node1 == node2)
            {
                throw new InvalidOperationException("Self-loops are not allowed.");
            }

            if (!this.Contains(node1))
            {
                throw new InvalidOperationException("The first node is not in the graph.");
            }

            if (!this.Contains(node2))
            {
                throw new InvalidOperationException("The second node is not in the graph.");
            }

            if (weight < 1 && this.PositiveEdgesOnly)
            {
                throw new InvalidOperationException("This graph supports positive edge weights only.");
            }

            if (node1.Neighbours.ContainsKey(node2))
            {
                throw new InvalidOperationException("There is already an edge from that start node to that end node.");
            }

            node1.Neighbours[node2] = weight;

            if (!this.IsDirected)
            {
                // No need to check whether there is an edge from node2 to node1.
                // If the graph is not directed, then edge insertion logic guarantees
                // node1.Neighbours.ContainsKey(node2) == node2.Neighbours.ContainsKey(node1).

                node2.Neighbours[node1] = weight;
            }

            this.EdgesCount += 1;
        }

    public void AddUnweightedEdge(GraphNode<T> node1, GraphNode<T> node2)
        {
            if (this.IsWeighted)
            {
                throw new InvalidOperationException("This graph is weighted.");
            }
             
            if (node1 == node2)
            {
                throw new InvalidOperationException("Self-loops are not allowed.");
            }

            if (!this.Contains(node1))
            {
                throw new InvalidOperationException("The first node is not in the graph.");
            }

            if (!this.Contains(node2))
            {
                throw new InvalidOperationException("The second node is not in the graph.");
            }

            if (node1.Neighbours.ContainsKey(node2))
            {
                throw new InvalidOperationException("There is already an edge from that start node to that end node.");
            }

            node1.Neighbours[node2] = 1;

            if (!this.IsDirected)
            {
                node2.Neighbours[node1] = 1;
            }

            this.EdgesCount += 1;
        }


    public void RemoveEdge(GraphNode<T> node1, GraphNode<T> node2)
        {
            if (!this.Contains(node1))
            {
                throw new InvalidOperationException("The first node is not in the graph.");
            }

            if (!this.Contains(node2))
            {
                throw new InvalidOperationException("The second node is not in the graph.");
            }

            if (!node1.Neighbours.ContainsKey(node2))
            {
                throw new InvalidOperationException("There is no edge from the first node to the second in the graph.");
            }

            node1.Neighbours.Remove(node2);

            if (!this.IsDirected)
            {
                node2.Neighbours.Remove(node1);
            }

            this.EdgesCount -= 1;
        }

    public void RemoveNode(GraphNode<T> node)
        {
            if (!this.Contains(node))
            {
                throw new InvalidOperationException("The node is not in the graph.");
            }

            // Two steps:
            // (1) Remove all edges attached to node, adjusting this.EdgesCount.
            // (2) Remove node from this.Nodes and decrement this.NodesCount.

            // (1)

            // Remove incoming edges:

           foreach (GraphNode<T> n in this.Nodes)
            {
                if (n.Neighbours.ContainsKey(node))
                {
                    n.Neighbours.Remove(node);
                    this.EdgesCount -= 1;
                }
            }

            // If the graph is direct, we need to consider
            // outgoing edges to correctly adjust this.EdgesCount.

            if (this.IsDirected)
            {
                this.EdgesCount -= node.Neighbours.Count;
            }

            // (2)

            this.Nodes.Remove(node);
            this.NodesCount -= 1;

            // For hygiene, we may optionally add:
            // node.Neighbours.Clear();
        }   


    
    
    // ===== Traversal Algorithms =====

    public IEnumerable<GraphNode<T>> BFS(GraphNode<T> startNode)
        {
            // Time complexity: O(v + e).

            // Where:
            // v is the number of nodes reachable from startNode.
            // e is the number of edges between reachable nodes ie
            // the number of traversable edges.
            // In a non-directed graph, each edge will
            // still be considered from each side ie each reachable 
            // edge will be considered exactly twice, but this still reduces to 
            // O(v + e).

            // Space complexity O(v).

            if (!this.Contains(startNode))
            {
                throw new InvalidOperationException("The start node is not in the graph.");
            }

            // We execute our BFS with my Queue<T> class.

            // We track visited nodes to ensure we don't revisit a node.
            // As we don't need to store these nodes in an order, we
            // use a HashSet<GraphNode<T>> object.

            HashSet<GraphNode<T>> visited = new HashSet<GraphNode<T>>();
            Queue<GraphNode<T>> queue = new DataStructures.Queue<GraphNode<T>>();

            visited.Add(startNode);
            queue.Enqueue(startNode);

            while (queue.Size > 0)
            {
                GraphNode<T> currentNode = queue.Dequeue();
                yield return currentNode;

                foreach (GraphNode<T> neighbour in currentNode.Neighbours.Keys)
                {
                    if (!visited.Contains(neighbour))
                    {
                        visited.Add(neighbour);
                        queue.Enqueue(neighbour);
                    }
                }
            }

            // As the Neighbours property of a node is a Dictionary, 
            // when we iterate through currentNode.Neighbours.Keys,
            // this iteration
            // has no guaranteed order. 
            // As such, there is no guaranteed order in which nodes are
            // enqueued.
            // This means that two calls of BFS() with the
            // same argument may return a different IEnumerable.

            // If we need deterministic order, we can 
            // sort currentNode.Neighbours.Keys
            // before iterating over it.
            // For example, we might sort by edge weight if the graph is weighted:
            // In this case, the inner foreach block becomes:
            // foreach (GraphNode<T> neighbour in currentNode.Neighbours.Keys.OrderBy(n => n.Value))
            // {...}
        }

    public IEnumerable<GraphNode<T>> DFS(GraphNode<T> startNode)
        {
            // Again: 
            // Time complexity: O(v + e).
            // Space complexity O(v).

            if (!this.Contains(startNode))
            {
                throw new InvalidOperationException("The start node is not in the graph.");
            }


            // We execute our DFS with my Stack<T> class.

            // Again, we track visited nodes to ensure we don't revisit a node.
            // As we don't need to store these nodes in an order, we
            // use a HashSet<GraphNode<T>> object.

            HashSet<GraphNode<T>> visited = new HashSet<GraphNode<T>>();
            Stack<GraphNode<T>> stack = new DataStructures.Stack<GraphNode<T>>();


            visited.Add(startNode);
            stack.Push(startNode);

            while (stack.Size > 0)
            {
                GraphNode<T> currentNode = stack.Pop();
                yield return currentNode;

                foreach (GraphNode<T> neighbour in currentNode.Neighbours.Keys)
                {
                    if (!visited.Contains(neighbour))
                    {
                        visited.Add(neighbour);
                        stack.Push(neighbour);
                    }
                }
            }

            // As with BFS(), the Neighbours property of a node is a Dictionary, 
            // when we iterate through currentNode.Neighbours.Keys,
            // this iteration
            // has no guaranteed order. 
            // As such, there is no guaranteed order in which nodes are
            // enqueued.
            // This means that two calls of DFS() with the
            // same argument may return a different IEnumerable.
            // This can be adjusted in a similar fashion to with BFS().
        }

    
    // Rather than explicitly appealing to my Stack<T> class in a DFS
    // implementation, we can implicitly rely on the call stack by using
    // a recursive method.

    // Again: 
    // Time complexity: O(v + e).
    // Space complexity O(v).

    // DFSRecursive() still requires a HashSet<GraphNode<T>> of size v.
    // However, the stack portion of the method will typically
    // be less space intensive
    // than DFS(). This is because in DFS(), when traversing, and looking
    // through neighbours, we immediately push 
    // all neighbours to
    // the Stack<GraphNode<T>> object, then process them later. 
    // In DFSRecursive(), we delay pushing to the implicit stack until
    // the point of processing.

    // However, DFSRecursive() has a stack overflow risk, as the call stack
    // has a fixed size. A deep graph may exceed its capacity.

    // A similar situation holds for DFS traversal of a tree
    // with appeal to an explicit Stack<T> versus implicit appeal to the call stack.
    // See my GeneralTree<T> in this repository.


    private IEnumerable<GraphNode<T>> DFSRecursive(GraphNode<T> startNode)
        {
            if (!this.Contains(startNode))
            {
                throw new InvalidOperationException("The start node is not in the graph.");
            }

            HashSet<GraphNode<T>> visited = new HashSet<GraphNode<T>>();
            return DFSRecursiveHelper(startNode, visited);
        }

    private IEnumerable<GraphNode<T>> DFSRecursiveHelper(GraphNode<T> currentNode, HashSet<GraphNode<T>> visited)
        {
            visited.Add(currentNode);
            yield return currentNode;

            foreach (GraphNode<T> neighbour in currentNode.Neighbours.Keys)
            {
                if (!visited.Contains(neighbour))
                {
                    foreach (GraphNode<T> node in DFSRecursiveHelper(neighbour, visited))
                    {
                        yield return node;
                    }
                }
            }
        }

    // To see how DFSRecursive works, consider a graph A -> B -> C.
    // Suppose we call DFSRecursive(A).
    // Frame 1: DFSRecursive(A) call.
    // HashSet object created. 

    // Frame 2: DFSRecursiveHelper(A, {}) call.
    // visited = {A}.
    // Set the first element of the returning IEnumerable in this call as A.
    // Iterate over unvisited neighbours.
    // Unvisited neighbour B selected.
    // Inner foreach block execution begins.
  
    // Frame 3: DFSRecursiveHelper(B, {A}) call.
    // visited = {A, B}.
    // Set the first element of the returning IEnumerable in this call as B.
    // Iterate over unvisited neighbours.
    // Unvisited neighbour C selected.
    // Inner foreach block execution begins.

    // Frame 4: DFSRecursiveHelper(C, {A, B}) call.
    // visited = {A, B, C}.
    // Set the first element of the returning IEnumerable in this call as C.
    // Iterate over unvisited neighbours.
    // There are none.

    // Frame 4 returns the IEnumerable with only C.

    // Frame 3 now resolves. The first element of the returning IEnumerable
    // was already set to B. Now the inner foreach block iterates over the 
    // IEnumerable returned by frame 4. This IEnumerable contained only C.
    // So C is added to the returning IEnumerable for frame 3.
    // Frame 3 returns the IEnumerable with B then C.

    // Frame 2 now resolves. The first element of the returning IEnumerable
    // was already set to A. Now the inner foreach block iterates over the 
    // IEnumerable returned by frame 3, which contained B then C.
    // So B then C is added to the returning IEnumerable for frame 2.
    // Frame 2 returns the IEnumerable with A, then B, then C.

    // Frame 1 now resolves. It returns the IEnumerable returned by frame 2.
    // That is, the IEnumerable with A, then B, then C.

    // In this way, the IEnumerable that a DFSRecursive() call returns is built
    // in a backwards fashion.

    
    
    // For another example, consider the graph A -> B - > C and A -> D.
    // Suppose we call DFSRecursive(A).

    // Frame 1: DFSRecursive(A) call.
    // Hashset object created.

    // Frame 2: DFSRecursiveHelper(A, {}) call.
    // visited = {A}.
    // Set the first element of the returning IEnumerable in this call as A.
    // Iterate over unvisited neighbours.
    // We have 2 (B and D).
    // Let us suppose B is selected, though as Neighbours is a Dictionary, it is
    // unordered, so either B or D may be selected first.
    // Inner foreach block execution begins.
  
    // Frame 3: DFSRecursiveHelper(B, {A}) call.
    // visited = {A, B}.
    // Set the first element of the returning IEnumerable in this call as B.
    // Iterate over unvisited neighbours.
    // Unvisited neighbour C selected.
    // Inner foreach block execution begins.

    // Frame 4: DFSRecursiveHelper(C, {A, B}) call.
    // visited = {A, B, C}.
    // Set the first element of the returning IEnumerable in this call as C.
    // Iterate over unvisited neighbours.
    // There are none.

    // Frame 4 returns the IEnumerable with only C.

    // Frame 3 now resolves. The first element of the returning IEnumerable
    // was already set to B. Now the inner foreach block iterates over the 
    // IEnumerable returned by frame 4. This IEnumerable contained only C.
    // So C is added to the returning IEnumerable for frame 3.
    // Frame 3 returns the IEnumerable with B then C.

    // Frame 2 now continues. Recall that the first element of the returning 
    // IEnumerable in this call was set to A. Now, we complete the first
    // loop of the outer foreach block by adding B then C to the returning
    // IEnumerable.
    // Next, unvisited neighbour D is selected.
    // The inner foreach block execution begins.

    // Frame 5: DFSRecursiveHelper(D, {A, B, C}) call.
    // visited = {A, B, C, D}.
    // Set the first element of the returning IEnumerable in this call as D.
    // Iterate over unvisited neighbours.
    // There are none.

    // Frame 5 returns the IEnumerable with only D.

    // Frame 2 now resolves. Recall our current returning IEnumerable for this
    // call is A, then B, then C. Now we are able to complete the inner foreach
    // block of the second loop of the outer foreach block. 
    // This iterates over the IEnumerable with only D (the one returned
    // by frame 5). So, we add D to the returning IEnumerable of frame 2. This gives us
    // the IEnumerable A then B then C then D.

    // Frame 1 resolves, returning the IEnumerable A then B then C then D.






    // ===== Shortest Path Algorithms =====


    // == Dijkstra ==

    // Conceptual Intro:

    // The algorithm takes a GraphNode<T> startNode as input.
    // We keep a record of both (i) the unvisited nodes, and (ii) for each node n, 
    // the tentative min distance from the startNode to n.
    // The algorithm involves visiting the nodes of the graph, starting at startNode,
    // updating (i) and (ii) as we go.
    // We can optionally also track (iii) for each node n,
    // the tentative min path from startNode to n.

    
    // Set the tentative distance to infinity for every node other than startNode.
    // Set the tenative distance to 0 for startNode.

    // The main loop is as follows:

    // Select currentNode: this is the unvisited node with the smallest
    // finite tentative distance from startNode.

    // For each node n that is a neighbour of currentNode, we compare
    // (the tentative min distance to currentNode + 
    // the weight edge from currentNode to n)
    // with (the tentative min distance to n).
    // If the former is less than the latter, we update the 
    // tentative min distance to n as the former value.

    // We mark currentNode as visited and the loop repeats until
    // a currentNode can no longer be selected. That is, there are no
    // finitely close unvisited nodes.

    
    // Implementation:

    // There are different implementations depending on exactly 
    // what the method returns. My approach is to implement a rich 
    // private method that takes startNode as input
    // and returns for each node n, both (a) the weight of the
    // shortest path from startNode to n, and
    // (b) the actual shortest path from startNode to n.

    // (a) is straightforwardly represented as a Dictionary<GraphNode<T>, int>
    // weightMap that
    // maps a node n to the weight of the shortest path from startNode to n.
    // If n is unreachable from startNode, this value is Int32.MaxValue.

    // (b) is represented indirectly by a Dictionary<GraphNode<T>, GraphNode<T>> 
    // prevMap that
    // maps a node n to the previous node m from n in the shortest path from 
    // startNode to n.
    // If n is unreachable from startNode, it does not feature in the map.
    // startNode itself is mapped to startNode in the map.
    // In this way, we unambiguously capture three separate states:
    // (i) prevMap[n] == n (n is the startNode).
    // (ii) prevMap.ContainsKey(n) && prevMap[n] != n (n is a reachable non-start node).
    // (iii) !prevMap.ContainsKey(n) (n is unreachable).

    
    // With this private method, we can then implement various public methods
    // with different return types, for different consumer purposes.



    // With regard to our rich private implementation, 
    // one implementation decision we need to make is how to
    // select the next currentNode when a new loop begins.
    // On this, it is informative to
    // consider a naive private implementation, which will store
    // a record of unvisited nodes in an unordered structure and
    // manually update currentNode.

    private (Dictionary<GraphNode<T>, int>, Dictionary<GraphNode<T>, GraphNode<T>>) NaiveDijkstra(GraphNode<T> startNode)
        {
            if (!this.Contains(startNode))
            {
                throw new InvalidOperationException("The start node is not in the graph.");
            }

            if (!this.IsWeighted)
            {
                throw new InvalidOperationException("The graph must be weighted (it is not).");
            }

            if (!this.PositiveEdgesOnly)
            {
                throw new InvalidOperationException("The algorithm cannot execute correctly if non-positive edge weights are allowed (they are).");
            }


            // Initialise weightMap, prevMap, and unvisited.

            Dictionary<GraphNode<T>, int> weightMap = new Dictionary<GraphNode<T>, int>();
            Dictionary<GraphNode<T>, GraphNode<T>> prevMap = new Dictionary<GraphNode<T>, GraphNode<T>>();
            HashSet<GraphNode<T>> unvisited = new HashSet<GraphNode<T>>();

            foreach (GraphNode<T> node in this.Nodes)
            {
                if (node == startNode)
                {
                    weightMap[node] = 0;
                    prevMap[node] = node;
                }
                else
                {
                    weightMap[node] = Int32.MaxValue;
                }

                unvisited.Add(node);
            }

            // Main loop:

            while (unvisited.Count > 0)
            {

                // Set currentNode as the closest
                // finitely close node that hasn't been visited:

                int lowestDistance = Int32.MaxValue;
                GraphNode<T>? currentNode = null;

                foreach (GraphNode<T> node in unvisited)
                {
                    if (weightMap[node] < lowestDistance)
                    {
                        lowestDistance = weightMap[node];
                        currentNode = node;
                    }
                }

                // If there isn't such a node, the loop ends:

                if (lowestDistance == Int32.MaxValue)
                {
                    break;
                }

                // We update the weightMap values for the neighbours of
                // currentNode:

                foreach (KeyValuePair<GraphNode<T>, int> kvp in currentNode.Neighbours)
                // We iterate over KVPs rather than keys alone as we will
                // need the values, and this saves having to
                // look up the value that corresponds to the key in the block.
                    {
                        GraphNode<T> neighbour = kvp.Key;
                        int edgeWeight = kvp.Value;

                        int altWeight = weightMap[currentNode] + edgeWeight;

                        if (altWeight < weightMap[neighbour])
                        {
                            weightMap[neighbour] = altWeight;
                            prevMap[neighbour] = currentNode;
                        }
                    }

                // We mark currentNode as visited:

                unvisited.Remove(currentNode);
            }

            // Once the traversal is complete and weightMap and prevMap have been
            // set, we return them:

            return (weightMap, prevMap);
        }

    
    // NaiveDijkstra is inefficient.
    // In the main loop, the initial scan over our HashSet<GraphNode<T>> unvisited 
    // is time complexity O(v), and the loops runs at most v times. This alone
    // gives us time complexity O(v^2).
    // However, in each loop, we also scan through the edges from currentNode. 
    // Across the whole algorithm, this 
    // is a total of max e checks (where e is the total number of edges in the graph).
    // As such, NaiveDijkstra() is time complexity O(v^2 + e).
    // Since e <= v^2, this is usually written as O(v^2).

    // The main inefficiency here is how currentNode is selected.
    // What we want is for our unvisited structure 
    // to hold node n and weightMap[n] pairs
    // in an ordered fashion so that we can simply request the "next 
    // pair", and this will return the n + weightMap[n] pair with the lowest
    // weightMap value (and where this value is not Int32.MaxValue).

    // In a simple world, we would initialise our unvisited structure with 
    // every node n + weightMap[n] pair at the start of the algorithm,
    // then the second value of the pairs would be adjusted (and hence the order
    // of the structure is adjusted) as we work through the algorithm and
    // weightMap is adjusted.

    // However, these adjustments may themselves be inefficient.

    // The approach I shall employ here is to use PriorityQueue<T,Q>.

    // PriorityQueue<T,Q> does not support efficient updates to elements.
    // As such, the approach here will be to allow "duplicate" elements
    // in the PriorityQueue, where duplication is
    // understood as two elements with the same GraphNode<T>
    // instance.
    
    // When a node n has its weightMap value adjusted to i, we enqueue n with
    // priority i.
    // For duplicates (n, priority i) and (n, priority j) in the PriorityQueue, 
    // where i > j, 
    // the (n, priority i) element is "stale". To handle stale elements, 
    // when an element (n, priority i) is dequeued, 
    // we need to check that i < weightMap[n].
    // If this doesn't hold, the element is stale and can be ignored.

    // However, once we dequeue an element from the PriorityQueue, we lose
    // access to its priority. 
    // As such, rather than simply using a PriorityQueue<GraphNode<T>, int> 
    // instance, we will use a 
    // PriorityQueue<(GraphNode<T>, int), int> instance, with
    // elements ((n, i), i).
    // So when we dequeue an element, we have the (node, priority) tuple. 
    // This allows us to straightforwardly execute the staleness check.

    // Finally, with this approach, there is no need to enqueue an element
    // for every node n before the main loop begins. We need only enqueue
    // (startNode, 0) with priority 0. 
    // Then we only enqueue elements when an entry in weightMap is 
    // updated.
    // With this approach, every reachable node will be visited.


    private (Dictionary<GraphNode<T>, int>, Dictionary<GraphNode<T>, GraphNode<T>>) Dijkstra(GraphNode<T> startNode)
        {
            if (!this.Contains(startNode))
            {
                throw new InvalidOperationException("The start node is not in the graph.");
            }

            if (!this.IsWeighted)
            {
                throw new InvalidOperationException("The graph must be weighted (it is not).");
            }

            // Initialise weightMap, prevMap, and unvisited.

            Dictionary<GraphNode<T>, int> weightMap = new Dictionary<GraphNode<T>, int>();
            Dictionary<GraphNode<T>, GraphNode<T>> prevMap = new Dictionary<GraphNode<T>, GraphNode<T>>();
            PriorityQueue<(GraphNode<T>, int), int> unvisited = new PriorityQueue<(GraphNode<T>, int), int>();

            foreach (GraphNode<T> node in this.Nodes)
            {
                if (node == startNode)
                {
                    weightMap[node] = 0;
                    prevMap[node] = node;
                }
                else
                {
                    weightMap[node] = Int32.MaxValue;
                }
            }
            
            unvisited.Enqueue((startNode, 0), 0);

            // Main loop:

            while (unvisited.Count > 0)
            {
                (GraphNode<T> currentNode, int distance) = unvisited.Dequeue();
                
                // By the enqueue logic, weightMap[currentNode] < Int32.MaxValue.
                
                // Staleness check:

                if (distance > weightMap[currentNode])
                {
                    continue;
                }
                                

                foreach (KeyValuePair<GraphNode<T>, int> kvp in currentNode.Neighbours)
                // We iterate over KVPs rather than keys alone as we will
                // need the values, and this saves having to
                // look up the value that corresponds to the key in the block.
                {
                    GraphNode<T> neighbour = kvp.Key;
                    int edgeWeight = kvp.Value;

                    int altWeight = weightMap[currentNode] + edgeWeight;

                    if (altWeight < weightMap[neighbour])
                    {
                        weightMap[neighbour] = altWeight;
                        prevMap[neighbour] = currentNode;

                        unvisited.Enqueue((neighbour, altWeight), altWeight);
                    }
                }
            }
            
            // Return weightMap and prevMap:

            return (weightMap, prevMap);
        }

    // With a PriorityQueue, insertion and extraction are each O(log v).
    // Each loop: executes Dequeue() at O(log v); executes a staleness check O(1);
    // and iterates over the neighbours of currentNode.

    // Across the whole algorithm, each edge is checked (from each side) at most once.
    // In the worst case, each check results in an update to weightMap.
    // So there are at most e enqueues (plus one for the initial startNode enqueue).
    // There is one dequeue for every enqueue, so at most e dequeues.
    // And at most e adjustments to weightMap.

    // So: 
    // e many enqueues with cost O(log v). Total O(e * log v)
    // e many dequeues with cost O(log v). Total O(e * log v)
    // e many adjustments to weightMap. Total O(e).

    // This reduces to complexity O(e * log v).

    
    // Now we use our rich private method to implement our various
    // consumer-facing methods:

    public int GetShortestDistance(GraphNode<T> startNode, GraphNode<T> endNode)
        {
            if (!this.Contains(endNode))
            {
                throw new InvalidOperationException("The end node is not in the graph.");
            }

            (Dictionary<GraphNode<T>, int> weightMap, Dictionary<GraphNode<T>, GraphNode<T>> prevMap) = this.Dijkstra(startNode);
            
            if (weightMap[endNode] == Int32.MaxValue)
            {
                throw new InvalidOperationException("There is no path from the start node to the end node.");
            }
            return weightMap[endNode];
        }

    public Dictionary<GraphNode<T>, int> GetAllShortestDistances(GraphNode<T> startNode)
        {
            (Dictionary<GraphNode<T>, int> weightMap, Dictionary<GraphNode<T>, GraphNode<T>> prevMap) = this.Dijkstra(startNode);
            return weightMap;
        }

    public IEnumerable<GraphNode<T>> GetShortestPath(GraphNode<T> startNode, GraphNode<T> endNode)
        {
            if (!this.Contains(endNode))
            {
                throw new InvalidOperationException("The end node is not in the graph.");
            }
            
            (Dictionary<GraphNode<T>, int> weightMap, Dictionary<GraphNode<T>, GraphNode<T>> prevMap) = this.Dijkstra(startNode);

            if (weightMap[endNode] == Int32.MaxValue)
            {
                throw new InvalidOperationException("There is no path from the start node to the end node.");
            }

            Stack<GraphNode<T>> path = new DataStructures.Stack<GraphNode<T>>();
            GraphNode<T> currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Push(currentNode);
                currentNode = prevMap[currentNode];
            }

            path.Push(startNode);

            while (path.Size > 0)
            {
                yield return path.Pop();
            }
        }

    public Dictionary<GraphNode<T>, List<GraphNode<T>>> GetAllShortestPaths(GraphNode<T> startNode)
        {
            (Dictionary<GraphNode<T>, int> weightMap, Dictionary<GraphNode<T>, GraphNode<T>> prevMap) = this.Dijkstra(startNode);

            // Initiate the Dictionary to be returned:

            Dictionary<GraphNode<T>, List<GraphNode<T>>> paths = new Dictionary<GraphNode<T>, List<GraphNode<T>>>();
            
            // The returned Dictionary will only contain values for nodes that are
            // reachable from startNode. That is, ones that are keys in prevMap.
            // So, we iterate over these nodes, for each, we generate a List<GraphNode<T>>
            // that represents the path.

            foreach (GraphNode<T> node in prevMap.Keys)
            {
                Stack<GraphNode<T>> path = new DataStructures.Stack<GraphNode<T>>();
                List<GraphNode<T>> completePath = new List<GraphNode<T>>();
                GraphNode<T> currentNode = node;

                while (currentNode != startNode)
                {
                    path.Push(currentNode);
                    currentNode = prevMap[currentNode];
                }

                path.Push(startNode);

                while (path.Size > 0)
                {
                    completePath.Add(path.Pop());
                }

                paths[node] = completePath;

            }

            return paths;
        }



    // == A* Algorithm ==

    // Conceptual Intro:

    // Dijkstra’s algorithm is efficient when we are interested in discovering 
    // for *every* node n, the shortest distance from our start node to n. 
    // However, it is not the best when we are just interested in the 
    // shortest distance from a single node to a single node.

    // The A* algorithm is a modification to Dijkstra’s algorithm which 
    // improves performance for this function by incorporating additional information.
    
    // The algorithm takes as input for each node n, 
    // an estimated distance from n to endNode. 
    // This estimated distance is the heuristic: h(n).

    // As with Dijkstra's, we additionally track g(n): the known shortest distance
    // from startNode to n.

    // In the algorithm, we move through the nodes as before, but currentNode
    // is selected as the node with the lowest f(n):

    // f(n) = g(n) + h(n).

    // That is, the distance from startNode to n, plus the heuristic from n to
    // endNode.

    // Dijkstra is a special case of A*: where h(n) = 0 for every n.


    // Implementation:

    // In keeping with the implementation of Dijkstra's algorithm,
    // we will implement a private rich method, then implement
    // multiple consumer-facing methods that employ the rich method.

    // In keeping with Dijkstra(), the rich method will return a 
    // tuple of dictionaries, weightMap and prevMap,
    // but these will only be completed up to endNode.

    // The method will take as input startNode, endNode, and a heuristic.
    // We will represent the heuristic using Func<...>.
    // Func is a delegate type: a type that represents a reference to a method.
    // Func takes multiple type parameters, where the last one 
    // specifies the return type.
    // We will use Func<GraphNode<T>, GraphNode<T>, int>. The delegate we
    // pass into an AStar() call will 
    // represent a particular heuristic for every node n,
    // not just our endNode.
    // Note: the delegate doesn't store values for every node, but it's a function
    // that can be called for any node.
    // In our method block, we will only call it with endNode as the second input.

    // For example, for a node n:
    // f(n) = weightMap[n] + heuristic(n, endNode).
    

    private (Dictionary<GraphNode<T>, int>, Dictionary<GraphNode<T>, GraphNode<T>>) AStar(GraphNode<T> startNode, GraphNode<T> endNode, Func<GraphNode<T>, GraphNode<T>, int> heuristic)
        {
            if (!this.Contains(startNode))
                {
                throw new InvalidOperationException("The start node is not in the graph.");
                }

            if (!this.Contains(endNode))
                {
                throw new InvalidOperationException("The end node is not in the graph.");
                }

            if (!this.IsWeighted)
                {
                throw new InvalidOperationException("The graph must be weighted (it is not).");
                }

            if (!this.PositiveEdgesOnly)
                {
                    throw new InvalidOperationException("The algorithm cannot execute correctly if non-positive edge weights are allowed (they are).");
                }


            // Initialise weightMap, prevMap, and unvisited.
            // In unvisited, when we enqueue a node n, we will use
            // f(n) as the priority score, but maintain (n, g(n)) as the
            // item that gets returned on the dequeue. This allows us to
            // perform a staleness check.

            Dictionary<GraphNode<T>, int> weightMap = new Dictionary<GraphNode<T>, int>();
            Dictionary<GraphNode<T>, GraphNode<T>> prevMap = new Dictionary<GraphNode<T>, GraphNode<T>>();
            PriorityQueue<(GraphNode<T>, int), int> unvisited = new PriorityQueue<(GraphNode<T>, int), int>();

            foreach (GraphNode<T> node in this.Nodes)
                {
                    if (node == startNode)
                    {
                        weightMap[node] = 0;
                        prevMap[node] = node;
                    }
                    else
                    {
                        weightMap[node] = Int32.MaxValue;
                    }
                }

            unvisited.Enqueue((startNode, 0), heuristic(startNode, endNode));

            // Main loop:

            while (unvisited.Count > 0)
            {
                (GraphNode<T> currentNode, int distance) = unvisited.Dequeue();
                
                // By the enqueue logic, weightMap[currentNode] < Int32.MaxValue.

                // Check if it's time to stop:

                if (currentNode == endNode)
                    {
                        break;
                    }
                
                // Staleness check:

                if (distance > weightMap[currentNode])
                {
                    continue;
                }
                                

                foreach (KeyValuePair<GraphNode<T>, int> kvp in currentNode.Neighbours)
                // We iterate over KVPs rather than keys alone as we will
                // need the values, and this saves having to
                // look up the value that corresponds to the key in the block.
                {
                    GraphNode<T> neighbour = kvp.Key;
                    int edgeWeight = kvp.Value;

                    int altWeight = weightMap[currentNode] + edgeWeight;

                    if (altWeight < weightMap[neighbour])
                    {
                        weightMap[neighbour] = altWeight;
                        prevMap[neighbour] = currentNode;

                        // Check our heuristic is valid!

                        int newHeuristic = heuristic(neighbour, endNode);
                        // We could optionally cache the heuristic for each node
                        // the first time it is calculated, to prevent recalculation.

                        if (newHeuristic < 0)
                            {
                                throw new InvalidOperationException("Heuristic must be non-negative.");
                            } 

                        unvisited.Enqueue((neighbour, altWeight), altWeight + newHeuristic);
                    }
                }
            }
            
            // Return weightMap and prevMap:

            return (weightMap, prevMap);
        }

    // With a PriorityQueue, insertion and extraction are each O(log v).
    // Each loop: executes Dequeue() at O(log v); executes a staleness check O(1);
    // and iterates over the neighbours of currentNode.

    // Across the whole algorithm, each edge is checked (from each side) at most once.
    // In the worst case, each check results in an update to weightMap.
    // So there are at most e enqueues (plus one for the initial startNode enqueue).
    // There is at most one dequeue for every enqueue, so at most e dequeues.
    // And at most e adjustments to weightMap.

    // So: 
    // e many enqueues with cost O(log v). Total O(e * log v)
    // e many dequeues with cost O(log v). Total O(e * log v)
    // e many adjustments to weightMap. Total O(e).

    // This reduces to complexity O(e * log v).
    

    // The worst case AStar() and Dijkstra() are the same. AStar() is only
    // more helpful than Dijkstra() when the heuristic is admissible (it never
    // overestimates) and informative.

    
    // Now we implement consumer-facing methods in a similar way we did with Dijkstra:

    public int GetShortestDistanceAStar(GraphNode<T> startNode, GraphNode<T> endNode, Func<GraphNode<T>, GraphNode<T>, int> heuristic)
        {
            (Dictionary<GraphNode<T>, int> weightMap, Dictionary<GraphNode<T>, GraphNode<T>> prevMap) = this.AStar(startNode, endNode, heuristic);
            
            if (weightMap[endNode] == Int32.MaxValue)
            {
                throw new InvalidOperationException("There is no path from the start node to the end node.");
            }
            return weightMap[endNode];
        }


    public IEnumerable<GraphNode<T>> GetShortestPathAStar(GraphNode<T> startNode, GraphNode<T> endNode, Func<GraphNode<T>, GraphNode<T>, int> heuristic)
        {           
            (Dictionary<GraphNode<T>, int> weightMap, Dictionary<GraphNode<T>, GraphNode<T>> prevMap) = this.AStar(startNode, endNode, heuristic);

            if (weightMap[endNode] == Int32.MaxValue)
            {
                throw new InvalidOperationException("There is no path from the start node to the end node.");
            }

            Stack<GraphNode<T>> path = new DataStructures.Stack<GraphNode<T>>();
            GraphNode<T> currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Push(currentNode);
                currentNode = prevMap[currentNode];
            }

            path.Push(startNode);

            while (path.Size > 0)
            {
                yield return path.Pop();
            }
        }

    // We could similarly implement 
    // GetAllShortestDistancesAStar(GraphNode<T> startNode, heuristic),
    // and GetAllShortestPathsAStar(GraphNode<T> startNode, heuristic),
    // but this would involve
    // running AStar(GraphNode<T> startNode, GraphNode<T> n, heuristic) for 
    // every node n in the graph.
}

}




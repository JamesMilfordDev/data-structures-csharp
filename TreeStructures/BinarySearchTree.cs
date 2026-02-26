// ===== Role =====

// This is a consumer-facing class that manages T values in a BST system.
// A BST maintains sorted order and allows efficient insertion/lookup/deletion
// of T values.

// This is a high-level class layered on my BinaryTree<T>, a low-level,
// internal mechanism that manages collections of BinaryTreeNode<T> instances in
// a binary tree structure.

// With this implementation, BinaryTree<T> provides both
// the main mechanism *and* the structural policy that each node has
// a left child and right child position.

// BinarySearchTree<T> imposes an ordering by Value policy on top of this:
// left child < parent < right child.
// Ordering is based on the generic interface IComparable<T>.

// There is a notable analogy here with:

// SimpleNode<T> -> SinglyLinkedList<T> -> Queue<T> and Stack<T>.
// ComplexNode<T> -> DoublyLinkedList<T> -> Deque<T>.

// In each case,
// we have a dumb, low-level container that is wholly managed
// by another class. This other class is still a low-level
// internal mechanism to manage the structure of a collection
// of the dumb, low-level container nodes. The other class is not 
// intended for direct consumer use, but rather provides a mechanism
// on which higher-level, consumer-facing classes are built.

// However, there is an important disanalogy with the current case.
// Because BinarySearchTree<T> layers an additonal ordering policy
// on BinaryTree<T>, we actually have access to ordered traversals
// of the underlying BinaryTree<T>. These ordered traversals are more efficient
// than the unordered traversals that BinaryTree<T> methods execute.
// As such, by adding the additional ordering policy to BinaryTree<T>,
// in BinarySearchTree<T> we are also able to *expand the mechanism*.

// Queue<T>, Stack<T>, and Deque<T> are basically consumer-facing
// thin wrappers over the mechanism SinglyLinkedList<T> or DoublyLinkedList<T>.
// BinarySearchTree<T> is consumer-facing, but its method logic
// is not just offloaded to BinaryTree<T>.

// In order to generate this extra mechanism in BinarySearchTree<T>,
// but maintain the private setters of Count and Root in BinaryTree<T>,
// BinaryTree<T> actually includes tailored methods to make the 
// adjustments to a BinaryTree<T> that the specialised methods in 
// BinarySearchTree<T> require. In particular:
// DecrementCount() and ShiftRootDownToUniqueChild().



// ===== Invariants =====

// Structure:

// BinarySearchTree<T> inherits the structural policy
// of BinaryTree<T> that nodes have at most two children.
// BinarySearchTree<T> imposes no further structural policy.


// Order by Value:

// Left child < parent < right child.


// Duplication: 

// Recall that in BinaryTree<T>,
// a tree is allowed multiple nodes with the same Value.
// However, a tree cannot contain the same node in two separate locations.

// In BinarySearchTree<T>, we disallow
// duplicates on a broad definition of duplication:

// Two nodes A and B are duplicates iff 
// the Value of A is "equal in order" to the Value of B,
// where "equal in order" is defined in the
// relevant CompareTo() method for the type T of the Values.
// That is: A.Value.CompareTo(B.Value) == 0.



// ===== Methods ===== 

// Clear().
// Calls the BinaryTree<T> method Clear() on the BinaryTree<T> reference
// stored by the BinarySearchTree<T> instance.
// Time complexity O(1).

// FindMin().
// Traverses down the far left branch of tree.
// Time complexity O(log n) for balanced trees; O(n) for degenerate.
// Throws an exception if the BST is empty.

// FindMax().
// Traverses down the far right branch of tree.
// Time complexity O(log n) for balanced trees; O(n) for degenerate.
// Throws an exception if the BST is empty.

// Contains(T value).
// Performs an ordered traversal of the tree to check
// whether a T object of equal ordering to value is in the tree.
// Time complexity O(log n) for balanced trees; O(n) for degenerate.

// Insert(T value).
// Adds an element with the passed T value at the relevant position.
// This involves an ordered traversal to find the insertion point.
// Throws an exception if the BST already contain an element with a Value
// of equal ordering.
// Time complexity O(log n) for balanced trees; O(n) for degenerate.

// Remove(T value).
// Removes the element with a T value of equal order to the passed T value.
// Throws an exception if the BST is empty or it does not contain
// an element with a Value of equal ordering.
// This involves: (Part 1) an ordered traversal to find the relevant node 
// (if it exists); then (Part 2) an adjustment to the tree.
// The initial traversal is
// time complexity O(log n) for balanced trees; O(n) for degenerate.
// Some Part 2 adjustments involve a "bubbling up" procedure, which is 
// time complexity O(log n) for balanced trees; O(n) for degenerate.

// Remove(T value) appeals to two special methods in BinaryTree<T>:
// DecrementCount() and ShiftRootDownToUniqueChild().



// ===== Design Discussion ===== 

// Consequences of no Parent Reference:

// Remove(T Value) is more complex than it would be if BinaryTreeNode<T>
// stored a Parent reference. However, the increased complexity only amounts to
// manually tracking parent nodes and additional conditional logic.
// We strictly do not improve time complexity of the necessary traversals by including
// a Parent reference in a node.


// Properties and Access Modifiers:

// There is a Size property, but it does not have a 
// private backing field as this is unnecessary.
// Rather, Size only has a getter (which is public), 
// which directly gets the Count property of the 
// underlying BinaryTree<T>, because consumers may want to access the size of the BST.

// There is no Root property, as this is unnecessary: consumers do not need to access
// the Root of the underlying tree. I could have added a Root property with a private
// getter that immediately accesses the Root of the underlying BinaryTree<T>, but 
// this is unnecessary: it merely allows us to access the Root in methods here
// with this.Root rather than this.Tree.Root.

// If anything, I find the syntax of this.Tree.Size and this.Tree.Root more
// helpful to readers here, as it stresses that BinarySearchTree<T> does not have 
// backing fields for Root/Size.

// The class and all its API methods are public as they are consumer-facing.


// Duplication:

// The duplication policy (no broad duplicates)
// is required in order to straightforwardly satisfy
// the ordering policy left child < parent < right child.
// As an alternative, we could adopt a more complex policy.
// For example, we could store a Count property in a node,
// representing how many of the node's Value are present.




using System;


namespace DataStructures
{

// We use the where keyword in the class declaraction
// to impose a constraint on which types can be passed
// into the type parameter for our generic class.

// In particular, we say our type T must implement IComparable<T>,
// which importantly for us contains the CompareTo() method.
// More on the CompareTo() method shortly.
// Note: although a value type is not a class, it can still implement interfaces.

public class BinarySearchTree<T> where T : IComparable<T>
{
        
    private BinaryTree<T> Tree {get;}
    public int Size
        {
            get { return this.Tree.Count;}
        }

    public BinarySearchTree()
        {
            this.Tree = new BinaryTree<T>();
        }

    
    public void Clear()
        {
            this.Tree.Clear();
        }

    public T FindMin()
        {
            // We just traverse down the far left branch.

            // Time complexity:
            // O(log n) for balanced trees; O(n) for degenerate.
            // Space complexity:
            // O(1).
            
            if (this.Tree.Root == null)
            {
                    throw new InvalidOperationException("The tree is empty.");
            }

            BinaryTreeNode<T> currentNode = this.Tree.Root;

            while (currentNode.LeftChild != null)
            {
                currentNode = currentNode.LeftChild;
            }

            return currentNode.Value;
        }

    public T FindMax()
        {

            // We just traverse down the far right branch.

            // Time complexity:
            // O(log n) for balanced trees; O(n) for degenerate.
            // Space complexity:
            // O(1).
            
            if (this.Tree.Root == null)
            {
                    throw new InvalidOperationException("The tree is empty.");
            }

            BinaryTreeNode<T> currentNode = this.Tree.Root;

            while (currentNode.RightChild != null)
            {
                currentNode = currentNode.RightChild;
            }

            return currentNode.Value;
        }

    // We can implement a "contains value" method by
    // calling the BinaryTree ContainsValue() method on our Tree object here:

    private bool ContainsUnordered(T value)
        {
            return this.Tree.ContainsValue(value);
        }

    // However, as this employs BFS traversal (unordered), it has
    // time complexity O(n).
    // We can utilise our ordered tree to perform an ordered search, with
    // time complexity O(log n) for balanced trees, O(n) for degenerate trees:

    public bool Contains(T value)
        {
            // We traverse the tree by comparing value with currentNode.Value.

            // If value and currentNode.Value are "equal", we return true.
            // If value is "less than" currentNode.Value, we move to the left child
            // of currentNode. If it doesn't have one, we return false.
            // If value is "greater than" currentNode.Value, we move to the right child
            // of currentNode. If it doesn't have one, we return false.

            // This movement reflects the global ordering policy that
            // the left child < parent < right child.

            // This approach works as we disallow duplication on the broad definition.

            BinaryTreeNode<T>? currentNode = this.Tree.Root;

            while (currentNode != null)
            {

                // Our aim is to compare value with currentNode.Value.
                // These are T objects.
                // To do this, we call the CompareTo() method on the value T object,
                // passing in the currentNode.Value T object as argument into the 
                // method call.
                // This call returns 0 if value and currentNode.Value are "equal in ordering".
                // It returns less than 0 if value is "less than" currentNode.Value.
                // It returns greater than 0 if value is "greater than" currentNode.Value.

                int comparison = value.CompareTo(currentNode.Value);

                if (comparison == 0)
                {
                    return true;
                }

                else if (comparison < 0)
                {
                    if (this.Tree.HasLeft(currentNode)) // Check to see if left child exists.
                    {
                        currentNode = currentNode.LeftChild;
                    }
                    else
                    {
                        return false;
                    }
                }

                else
                {
                    if (this.Tree.HasRight(currentNode)) // Check to see if right child exists.
                    {
                        currentNode = currentNode.RightChild;
                    }
                    else
                    {
                        return false;
                    }
                }
                
            }

            return false;
        }

    public void Insert(T value)
        {
            // We want to insert a new node with a Value of value.
            // Its place in the tree is determined by the ordering of value against
            // the Values of other nodes in the tree.
            // Once we have found the insertion point and inserted a 
            // new node, we return.

            // Again, we perform an ordered traversal, comparing value against
            // the Value of currentNode, using CompareTo().

            // Recall that we disallow duplicates on the broad definition:
            // A and B are duplicates iff A.Value.CompareTo(B.Value) == 0.

            // For this method, I will throw an exception if
            // inserting the new value into the tree would create a duplicate.

            if (this.Tree.Root == null)
            {
                this.Tree.SetRoot(value);
                return;
            }

            BinaryTreeNode<T> currentNode = this.Tree.Root;

            while (true)
            {
                int comparison = value.CompareTo(currentNode.Value);

                if (comparison == 0)
                {
                    throw new InvalidOperationException("The tree already has a value of equal order.");
                }

                if (comparison < 0)
                {
                    if (!this.Tree.HasLeft(currentNode))
                    {
                        this.Tree.AddLeft(currentNode, value);
                        return;
                    }
                    else
                    {
                        currentNode = currentNode.LeftChild;
                    }
                }

                else
                {
                    if (!this.Tree.HasRight(currentNode))
                    {
                        this.Tree.AddRight(currentNode, value);
                        return;
                    }
                    else
                    {
                        currentNode = currentNode.RightChild;
                    }
                }
            }
        }

    public void Remove(T value)
        {
            // This method has two parts:
            
            // Part 1. Find the node with Value of value.
            // Part 2. Remove the value from the Tree, and 
            // adjust the Tree to preserve global order/Count.

            if (this.Tree.Root == null)
            {
                throw new InvalidOperationException("The tree is empty.");
            }

            // Part 1. Find the node x with a Value of value.
            
            // This involves an ordered traversal of the Tree.
            // This is time complexity O(log n) for balanced trees, and 
            // O(n) for degenerate trees.

            // As a node does not store a Parent reference, we manually track 
            // x's Parent to help adjusting the Tree after the value removal.

            // If we stored a Parent reference in a node, we would still need
            // to perform the traversal, which would have the same time
            // complexity. However, we would not need to 
            // manually track the parent node.
            
            BinaryTreeNode<T>? parentNode = null;
            BinaryTreeNode<T>? currentNode = this.Tree.Root;

            while (currentNode != null)
            {
                int comparison = value.CompareTo(currentNode.Value);

                if (comparison == 0)
                {
                    break;
                }

                if (comparison < 0)
                {
                    parentNode = currentNode;
                    currentNode = currentNode.LeftChild;
                }
                else
                {
                    parentNode = currentNode;
                    currentNode = currentNode.RightChild;
                }
            }
            // If there is no such node, we throw an exception:

            if (currentNode == null)
            {
                throw new InvalidOperationException("The tree does not contain that value.");
            }

            // Part 2. We remove the value from the Tree and adjust Count.

            // Three cases:
            // (A) currentNode has no children (it's a leaf).
            // (B) currentNode has exactly one child.
            // (C) currentNode has two children (complex case).

            // (A) currentNode has no children (it's a leaf).
            // We remove currentNode.

            if (!this.Tree.HasLeft(currentNode) && !this.Tree.HasRight(currentNode))
            {
                // Three sub-cases:
                // (A.i) currentNode == this.Tree.Root.
                // (A.ii) currentNode == parentNode.LeftChild.
                // (A.iii) currentNode == parentNode.RightChild.

                if (currentNode == this.Tree.Root) // (A.i)
                {
                    this.Tree.Clear();
                    // Decrementing Count to 0 is handled in Clear().

                    return;
                    // This return isn't necessary, as all conditional code
                    // blocks here are logical endpoints. However, I have included
                    // a return in each block for readability and safety.
                }

                // parentNode != null in (A.ii) and (A.iii.)

                else if (currentNode == parentNode.LeftChild) // (A.ii)
                {
                    parentNode.LeftChild = null;
                    this.Tree.DecrementCount();
                    return;

                    // We must manually decrement the Count property of
                    // this.Tree here and in certain other cases.
                    // We do this with the BinaryTree<T>
                    // method DecrementCount().
                }

                else    // (A.iii)
                {
                    parentNode.RightChild = null;
                    this.Tree.DecrementCount();
                    return;
                }
            }

            // (B) currentNode has exactly one child.
            // We remove currentNode.

            else if ((this.Tree.HasLeft(currentNode) && !this.Tree.HasRight(currentNode)) || (!this.Tree.HasLeft(currentNode) && this.Tree.HasRight(currentNode)))
            {
                // Save the child for simplicity:
                
                BinaryTreeNode<T> child;
                if (this.Tree.HasLeft(currentNode))
                {
                    child = currentNode.LeftChild;
                }
                else
                {
                    child = currentNode.RightChild;
                }
                
                // Three sub-cases:
                // (B.i) currentNode == this.Tree.Root (no parent).
                // (B.ii) currentNode == parentNode.LeftChild.
                // (B.iii) currentNode == parentNode.RightChild.

                if (currentNode == this.Tree.Root)
                {
                    this.Tree.ShiftRootDownToUniqueChild();
                    return;
                }

                // parentNode != null in (B.ii) and (B.iii.)

                else if (currentNode == parentNode.LeftChild)
                {
                    parentNode.LeftChild = child;
                    this.Tree.DecrementCount();
                    return;
                }

                else
                {
                    parentNode.RightChild = child;
                    this.Tree.DecrementCount();
                    return;
                }
            }

            // (C) currentNode has two children (complex case).

            else
            {
                // Rather than removing currentNode,
                // we are going to replace the Value of currentNode
                // with the Value of another node n in the tree, then
                // remove n from the tree.


                // n is the node with the Value that follows
                // currentNode.Value in the overall ordering of T objects.
                // This will be the node with the smallest Value in the right subtree.
                // That is, the node at the bottom of the far left branch in
                // the right subtree from currentNode.

                // Identifying n involves a traversal of the far left branch
                // of the right subtree from currentNode.

                // As a node does not store a Parent reference, we record 
                // n's Parent to help remove n.

                // If we stored a Parent reference in a node, we would still need
                // to perform the traversal, which would have the same time
                // complexity. However, we would not need to 
                // manually track the parent node.

                BinaryTreeNode<T> nParent = currentNode;
                BinaryTreeNode<T> n = currentNode.RightChild;
                
                while (this.Tree.HasLeft(n))
                {
                    nParent = n;
                    n = n.LeftChild;
                }

                // Replace currentNode's Value with n's Value.

                currentNode.Value = n.Value;

                // Now that we've harvested n's Value, we remove n from the tree.
                // How this is done depends on the position 
                // of n in the tree.

                // First, let's save the subtree beneath n.
                // By definition, n has no LeftChild.
                // It may or may not have a RightChild.

                BinaryTreeNode<T>? nChild = n.RightChild;

                // Now, two cases:

                // (i) n is at the top of the right subtree 
                // (nParent == currentNode).
                // In this case, n == nParent.RightChild.
                // So we replace nParent.RightChild with nChild.

                // (ii) n is deeper in the right subtree
                // In this case, n == nParent.LeftChild.
                // So we replace nParent.LeftChild with nChild.

                if (nParent == currentNode)
                {
                    nParent.RightChild = nChild;
                    this.Tree.DecrementCount();
                    return;
                }
                else
                {
                    nParent.LeftChild = nChild;
                    this.Tree.DecrementCount();
                    return;
                }
            }
        }

}

}
import java.util.ArrayList;
import java.util.List;

public class BacktrackingAVL extends AVLTree {
    // For clarity only, this is the default ctor created implicitly.
    public BacktrackingAVL() {
        super();
    }

	//You are to implement the function Backtrack.
    public void Backtrack() {
        if (root == null)
            return;

        ImbalanceCases imbalanceCases = (ImbalanceCases) history.pop();
        if(imbalanceCases.equals(ImbalanceCases.NO_IMBALANCE)){
            Node value = (Node) history.pop();
            if(value.parent == null)
                root = null;
            else if (value.parent.left == value){
                value.parent.left = null;
                value.parent = null;
            }
            else if (value.parent.right == value) {
                value.parent.right = null;
                value.parent = null;
            }
            if (root != null){
            updateHeight(root);
            updateSize(root);
            }
        }

        else if(imbalanceCases.equals(ImbalanceCases.LEFT_LEFT)){
            Node value = (Node) history.pop();
            if (value.parent != null){
                if (value.parent.left == value)
                    value.parent.left = rotateLeft(value);
                else
                    value.parent.right = rotateLeft(value);
            }
            else {
                root = value.right;
                if (value.right.left!= null){
                    value.right = value.right.left;
                    value.right.parent = value;
                }
                else
                    value.right = null;

                value.parent = root;
                root.left=value;
                root.parent = null;
            }
            Backtrack();
        }

        else if(imbalanceCases.equals(ImbalanceCases.LEFT_RIGHT)){
            Node value = (Node) history.pop();
            if (value.parent.left == value)
                value.parent.left = rotateRight(value);
            else
                value.parent.right = rotateRight(value);
            Backtrack();
        }

        else if(imbalanceCases.equals(ImbalanceCases.RIGHT_LEFT)){
            Node value = (Node) history.pop();
            if (value.parent.right == value)
                value.parent.right = rotateLeft(value);
            else
                value.parent.left = rotateLeft(value);
            Backtrack();
        }

        else if(imbalanceCases.equals(ImbalanceCases.RIGHT_RIGHT)){
            Node value = (Node) history.pop();
            if (value.parent != null){
                if (value.parent.left == value)
                    value.parent.left = rotateRight(value);
                else
                    value.parent.right = rotateRight(value);
            }
            else {
                root = value.left;
                if (value.left.right != null){
                    value.left = value.left.right;
                    value.left.parent = value;
                }
                else
                    value.left = null;

                value.parent = root;
                root.right=value;
                root.parent = null;
            }

            Backtrack();
        }
    }

    public int updateHeight(Node node){
        if (node.left == null & node.right==null){
            node.height = 0;
            return 0;
        }

        int height1 = (node.left != null) ? updateHeight(node.left) : 0;
        int height2 = (node.right != null) ? updateHeight(node.right) : 0;

        node.height = Math.max(height1,height2) +1 ;
        return node.height;
    }



    //Change the list returned to a list of integers answering the requirements
    public static List<Integer> AVLTreeBacktrackingCounterExample() {
        List ans = new ArrayList();
        ans.add(1);
        ans.add(2);
        ans.add(3);

        return ans;
    }
    
    public int Select(int index) {
        if(root == null)
            return 0;
        return Select(root,index);
    }

    public int Select(Node node,int index) {
        int curr_rank = 1;
        if (node.left != null)
        curr_rank =node.left.size+ curr_rank;

        if (curr_rank==index)
            return node.value;

        else if (index < curr_rank)
            return Select(node.left,index);
        else
            return Select(node.right,index-curr_rank);
    }
    
    public int Rank(int value) {
        return Rank(root,value,0);
    }

    public int Rank(Node node,int value,int counter) {
        if (node == null)
            return counter;

        else if (node.value == value)
            return (node.left == null ) ? counter : counter+node.left.size;
        else if (node.value > value)
            return Rank(node.left, value,counter);
        else {
            return (node.left == null) ? Rank(node.right,value,counter) + 1 : Rank(node.right,value,counter) +node.left.size + 1;
        }

    }

}

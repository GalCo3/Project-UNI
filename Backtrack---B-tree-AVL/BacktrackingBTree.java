import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;

public class BacktrackingBTree<T extends Comparable<T>> extends BTree<T> {
	// For clarity only, this is the default ctor created implicitly.
	public BacktrackingBTree() {
		super();
	}

	public BacktrackingBTree(int order) {
		super(order);
	}

	//You are to implement the function Backtrack.
	public void Backtrack() {
		if (actions.size()==0 || !actions.pop().equals("insertion"))
			return;

		BTreeActionType insertionCase = (BTreeActionType) actions.pop();
		if (insertionCase.equals(BTreeActionType.NONE)){
			T value = (T)actions.pop();
			Node<T> node = getNode(value);
			node.removeKey(value);
		}
		while(actions.size()!= 0 && actions.peek().equals(BTreeActionType.SPLIT)) {
			actions.pop();
			Unsplit();
		}
		--size;
		if(size == 0) root = null;
	}

	public void Unsplit(){

		T median = (T) actions.pop();// o(1)
		T rightSplit = (T) actions.pop();// o(1)
		T leftSplit =  (T) actions.pop();// o(1)
		Node<T> right = getNode(rightSplit);// o(
		Node<T> left= getNode(leftSplit);// o(
		Node<T> parent = getNode(median);// o(
		left.keys[left.numOfKeys] = median; //o(1)
		left.numOfKeys++;// o(1)
		for (int i =0; i < right.numOfKeys; i++) {// o(t-1)
			left.keys[left.numOfKeys] =right.keys[i]; //o(1)
			left.numOfKeys++;//o(1)
		}

		parent.removeKey(median);// o(2t-1)
		if(right.numOfChildren != 0){// o(
			for(int i = 0; i< right.numOfChildren;i++){
				left.addChild(right.getChild(i));
			}
		}
		parent.removeChild(right);
		if(parent.numOfKeys == 0){
			root = left;
			root.parent = null;
		}
	}
	
	//Change the list returned to a list of integers answering the requirements
	public static List<Integer> BTreeBacktrackingCounterExample(){

		List<Integer> ans = new LinkedList<>();
		ans.add(1);
		ans.add(30);
		ans.add(2);
		ans.add(29);
		ans.add(3);
		ans.add(28);
		ans.add(4);
		ans.add(27);
		ans.add(5);
		ans.add(26);
		ans.add(6);
		ans.add(25);
		ans.add(7);
		ans.add(24);
		ans.add(8);
		ans.add(23);
		ans.add(9);
		ans.add(22);
		ans.add(10);

		return ans;
	}
}

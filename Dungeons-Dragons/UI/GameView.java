package UI;
import BusinessLayer.Abstracts.Player;
import BusinessLayer.Abstracts.Tile;
import java.util.List;

public class GameView {
    private MessageCallback message;

    public GameView() {
        message = this::printMessage;
    }

    public MessageCallback getMessage() {
        return message;
    }

    public void setMessage(MessageCallback message) {
        this.message = message;
    }

    public void printMessage(String message) {
        System.out.println(message);
    }

    public void printBoard(GameBoard board){
        for(Tile t:board.getTiles()){
            System.out.print(t);
            if(board.getTiles().indexOf(t) + 1 < board.getTiles().size()) {
                if (board.getTiles().get(board.getTiles().indexOf(t) + 1).getPosition().getY() > t.getPosition().getY())
                    System.out.println();
            }
        }
        System.out.println();
    }

    public void printMenu(List<Player> playerList){
        System.out.println("Select a player:");
        for(Player p:playerList) {
            System.out.println(playerList.indexOf(p)+1 + ". " + p.getInfo());
        }
    }

}



package BusinessLayer.Abstracts;

import UI.*;

public abstract class Tile implements Comparable<Tile> {
    protected char tile;
    protected Position position;
    protected GameBoard board;
    protected MessageCallback message;

    protected Tile(char tile,MessageCallback message){
        this.tile = tile;
        this.message = message;
    }

    public char getTile() {
        return tile;
    }

    public MessageCallback getMessage() {
        return message;
    }
    public Position getPosition() {
        if(position == null) return new Position(-1,-1);
        else return position;
    }
    public void setMessage(MessageCallback message) {
        this.message=message;
    }
    public void setPosition(Position position) {
        this.position = position;
    }
    public void setBoard(GameBoard board) {
        this.board = board;
    }
    public void setTile(char c){
        tile = c;
    }

    public abstract boolean isEnemy();
    public abstract boolean isPlayer();
    public abstract void visit(Tile other);
    public abstract void accept(Unit other);
    public abstract void accept(Unit other,int attackPoints);
    public abstract void enemyTurn();
    public abstract void processStep();
    public abstract void specialAbility(Tile other, int attack);
    public abstract String getInfo();
    protected void initialize(Position position){
        this.position = position;
    }

    @Override
    public int compareTo(Tile tile) {
        return getPosition().compareTo(tile.getPosition());
    }

    @Override
    public String toString() {
        return String.valueOf(getTile());
    }

    public abstract void cast();
}

package UI;

import BusinessLayer.Abstracts.*;
import java.util.*;

public class GameBoard {
    List<Tile> tiles;
    List<Tile> enemies;

    public GameBoard(List<Tile> tiles){
        this.tiles = new ArrayList<>();
        enemies = new ArrayList<>();
        for (Tile tile:tiles)
        {
            tile.setBoard(this);
            this.tiles.add(tile);
            if (tile.isEnemy())
                enemies.add(tile);
        }
    }

    public List<Tile> getTiles() {
        return tiles;
    }

    public void setTiles(List<Tile> tiles) {
        this.tiles = tiles;
    }

    public Tile getTile(Position pos) {
        for (Tile t : tiles) {
            if (t.getPosition().compareTo(pos) == 0) return t;
        }
        throw new NoSuchElementException("No such tile in the game board");
    }

    public Tile getTile(int x, int y) {
        Position pos = new Position(x,y);
        for (Tile t : tiles) {
            if (t.getPosition().compareTo(pos) == 0) return t;
        }
        throw new NoSuchElementException("No such tile in the game board");
    }

    public List<Tile> getEnemies() {
        return enemies;
    }

    public Tile getPlayer() {
        for (Tile tile : tiles) {
            if (tile.isPlayer())
                return tile;
        }
        throw new NoSuchElementException("No player");
    }

    public List<Tile> getAllEnemiesInRange(int range)
    {
        List<Tile> inRange = new ArrayList<>();
        Tile player = getPlayer();
        for(int i =0; i < enemies.size(); i++){
            Tile t = enemies.get(i);
            if(Unit.range(t,player) <= range)
                inRange.add(t);
        }
        return inRange;
    }

    public void removeEnemy(Enemy e,boolean ability){
        Tile empty = new Empty(e.getPosition(), getPlayer().getMessage());
        Tile p = getPlayer();
        tiles.set(tiles.indexOf(e),empty);
        if(!ability) p.visit(empty);
        enemies.remove(e);
    }
}

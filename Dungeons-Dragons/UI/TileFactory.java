package UI;

import BusinessLayer.Abstracts.*;
import BusinessLayer.Enemies.Boss;
import BusinessLayer.Enemies.Monster;
import BusinessLayer.Enemies.Trap;
import BusinessLayer.Players.Hunter;
import BusinessLayer.Players.Mage;
import BusinessLayer.Players.Rogue;
import BusinessLayer.Players.Warrior;

import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.function.Function;
import java.util.function.Supplier;
import java.util.stream.Collectors;

public class TileFactory {
    private List<Supplier<Player>> playersList;
    private Map<Character, Supplier<Enemy>> enemiesList;
    private Player selected;
    private MessageCallback message;

    public TileFactory(MessageCallback message){
        this.message = message;
        playersList = initPlayers();
        enemiesList = initEnemies();
    }

    private Map<Character, Supplier<Enemy>> initEnemies() {
        List<Supplier<Enemy>> enemies = Arrays.asList(
                () -> new Monster('s', "Lannister Solider", 80, 8, 3,25, 3,message),
                () -> new Monster('k', "Lannister Knight", 200, 14, 8, 50, 4,message),
                () -> new Monster('q', "Queen's Guard", 400, 20, 15, 100, 5,message),
                () -> new Monster('z', "Wright", 600, 30, 15,100, 3,message),
                () -> new Monster('b', "Bear-Wright", 1000, 75, 30, 250, 4,message),
                () -> new Monster('g', "Giant-Wright",1500, 100, 40,500, 5,message),
                () -> new Monster('w', "White Walker", 2000, 150, 50, 1000, 6,message),
                () -> new Boss('M', "The Mountain", 1000, 60, 25,  500, 6,5,message),
                () -> new Boss('C', "Queen Cersei", 100, 10, 10,1000, 1,8,message),
                () -> new Boss('K', "Night's King", 5000, 300, 150, 5000, 8,3,message),
                () -> new Trap('B', "Bonus Trap", 1, 1, 1, 250,  1,5,message),
                () -> new Trap('Q', "Queen's Trap", 250, 50, 10, 100, 3, 7,message),
                () -> new Trap('D', "Death Trap", 500, 100, 20, 250, 1, 10, message)
        );

        return enemies.stream().collect(Collectors.toMap(s -> s.get().getTile(), Function.identity()));
    }

    private List<Supplier<Player>> initPlayers() {
        return Arrays.asList(
                () -> new Warrior("Jon Snow", 300, 30, 4, 3,message),
                () -> new Warrior("The Hound", 400, 20, 6, 5,message),
                () -> new Mage("Melisandre", 100, 5, 1, 300, 30, 15, 5, 6,message),
                () -> new Mage("Thoros of Myr", 250, 25, 4, 150, 20, 20, 3, 4,message),
                () -> new Rogue("Arya Stark", 150, 40, 2, 20,message),
                () -> new Rogue("Bronn", 250, 35, 3, 50,message),
                () -> new Hunter("Ygritte", 220, 30, 2, 6, message)
        );
    }

    public List<Supplier<Player>> getPlayersList() {
        return playersList;
    }

    public List<Player> listPlayers(){
        return playersList.stream().map(Supplier::get).collect(Collectors.toList());
    }

    public Tile produceTile(char i, Position p){
        if(i == '.') return produceEmpty(p);
        else if(i == '#') return produceWall(p);
        else if(i == '@') {
            selected.setPosition(p);
            return selected;
        }
        else return produceEnemy(i,p);
    }

    public Enemy produceEnemy(char i, Position p){
        Enemy e = initEnemies().get(i).get();
        e.setPosition(p);
        return e;
    }

    public Player producePlayer(int idx){
		this.selected = initPlayers().get(idx - 1).get();
        return selected;
    }

    public Empty produceEmpty(Position p){
        Empty e = new Empty(p,message);
        return e;
    }

    public Wall produceWall(Position p){
        Wall w = new Wall(p,message);
        return w;
    }
}

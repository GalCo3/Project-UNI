package Tests;

import BusinessLayer.Abstracts.*;
import BusinessLayer.Enemies.Monster;
import BusinessLayer.Players.Warrior;
import UI.GameBoard;
import UI.MessageCallback;
import org.junit.*;
import java.util.LinkedList;

public class MonsterTests {
    private Monster monster;
    private Monster monsterCopy;
    private Empty empty;
    private Wall wall;
    private GameBoard board;
    private Player player;
    private LinkedList<Tile> tiles;
    private MessageCallback message;

    //replace message printing
    public void printMessage(String message) {
    }

    @Before
    public void initTest(){
        message = this::printMessage;
        wall = new Wall(new Position(0,1),message);
        empty = new Empty(new Position(0,1),message);
        player = new Warrior("Player", 1, 1, 0, 1, message);
        monster = new Monster('m',"Monster",1,100000,1,1,3,message);
        monsterCopy = new Monster('m',"Monster",1,100000,1,1,3,message);
        tiles = new LinkedList<>();
        tiles.add(player);
        tiles.add(monster);
        tiles.add(monsterCopy);
        board = new GameBoard(tiles);
        monster.setPosition(new Position(0,0));
        monsterCopy.setPosition(new Position(0,0));
        player.setPosition(new Position(0,2));
    }

    @Test
    public void castTest(){
        monster.cast();
        Assert.assertEquals("Check for no changes after cast",monsterCopy,monster);
    }

    @Test
    public void turnAttackPlayerTest(){
        player.setPosition(new Position(0,1));
        monster.enemyTurn();
        Assert.assertTrue("Check if monster attacks player when near (1 in 100,000 chance to fail due to probability)",player.getHealthAmount()<1);
    }

    @Test
    public void moveToPlayerEmptyTest(){
        tiles.add(empty);
        board = new GameBoard(tiles);
        monster.enemyTurn();
        Assert.assertEquals("Check if monster moved towards the player with empty tile",new Position(0,1),monster.getPosition());
    }

    @Test
    public void moveToPlayerWallTest(){
        tiles.add(wall);
        board = new GameBoard(tiles);
        monster.enemyTurn();
        Assert.assertEquals("Check if monster failed to move towards the player with a wall",new Position(0,0),monster.getPosition());
    }

    @Test
    public void moveRandomlyTest(){
        player.setPosition(new Position(100,100));
        Empty empty1 = new Empty(new Position(0,1),message);
        Empty empty2 = new Empty(new Position(1,0),message);
        Empty empty3 = new Empty(new Position(-1,0),message);
        Empty empty4 = new Empty(new Position(0,-1),message);
        tiles.add(empty1);
        tiles.add(empty2);
        tiles.add(empty3);
        tiles.add(empty4);
        board = new GameBoard(tiles);
        monster.enemyTurn();
        Assert.assertNotEquals("Check if monster moved randomly (1 in 5 chance to fail due to probability)",new Position(0,0),monster.getPosition());
    }

    @Test
    public void stepTest(){
        monster.processStep();
        Assert.assertEquals("Check for no changes after process step",monsterCopy,monster);
    }

    @Test
    public void infoTest(){
        Assert.assertEquals("Check proper stats information",monster.getInfo(),"Name: Monster, Health: 1/1, Attack: 100000, Defense: 1, Experience value: 1, Vision range: 3");
    }
}

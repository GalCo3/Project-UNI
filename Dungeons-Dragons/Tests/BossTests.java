package Tests;

import BusinessLayer.Abstracts.*;
import BusinessLayer.Enemies.Boss;
import BusinessLayer.Players.Warrior;
import UI.GameBoard;
import UI.MessageCallback;
import org.junit.*;
import java.util.LinkedList;

public class BossTests {
    private Boss boss;
    private Boss bossCopy;
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
        boss = new Boss('b',"Boss",1,100000,1,1,3,1,message);
        bossCopy = new Boss('b',"Boss",1,100000,1,1,3,1,message);
        tiles = new LinkedList<>();
        tiles.add(player);
        tiles.add(boss);
        tiles.add(bossCopy);
        board = new GameBoard(tiles);
        boss.setPosition(new Position(0,0));
        bossCopy.setPosition(new Position(0,0));
        player.setPosition(new Position(0,2));
    }

    @Test
    public void castTest(){
        boss.castAbility();
        Assert.assertTrue("Check if monster kills player from afar",player.getHealthAmount() < 1);
    }

    @Test
    public void turnAttackPlayerTest(){
        player.setPosition(new Position(0,1));
        boss.enemyTurn();
        Assert.assertTrue("Check if monster attacks player when near (1 in 100,000 chance to fail due to probability)",player.getHealthAmount()<1);
    }

    @Test
    public void moveToPlayerEmptyTest(){
        tiles.add(empty);
        board = new GameBoard(tiles);
        boss.enemyTurn();
        Assert.assertEquals("Check if monster moved towards the player with empty tile",new Position(0,1),boss.getPosition());
    }

    @Test
    public void moveToPlayerWallTest(){
        tiles.add(wall);
        board = new GameBoard(tiles);
        boss.enemyTurn();
        Assert.assertEquals("Check if monster failed to move towards the player with a wall",new Position(0,0),boss.getPosition());
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
        boss.enemyTurn();
        Assert.assertNotEquals("Check if monster moved randomly (1 in 5 chance to fail due to probability)",new Position(0,0),boss.getPosition());
    }

    @Test
    public void infoTest(){
        Assert.assertEquals("Check proper stats information",boss.getInfo(),"Name: Boss, Health: 1/1, Attack: 100000, Defense: 1, Experience value: 1, Vision range: 3, Ability frequency: 1, Combat ticks: 0");
    }
}

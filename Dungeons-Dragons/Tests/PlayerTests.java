package Tests;

import BusinessLayer.Abstracts.Position;
import BusinessLayer.Abstracts.Tile;
import BusinessLayer.Enemies.Monster;
import BusinessLayer.Players.Warrior;
import UI.GameBoard;
import UI.MessageCallback;
import org.junit.Assert;
import org.junit.Before;
import org.junit.Test;

import java.util.LinkedList;

public class PlayerTests {
    private Warrior warrior;
    private Warrior warriorCopy;
    private Monster monster;
    private GameBoard board;
    private LinkedList<Tile> tiles;
    private MessageCallback message;

    //replace message printing
    public void printMessage(String message) {
    }

    @Before
    public void initTest(){
        message = this::printMessage;
        monster = new Monster('m',"Monster", 1, 1000000, 0, 1,1, message);
        warrior = new Warrior("Warrior",1,1000000,0,10,message);
        warriorCopy = new Warrior("Warrior",1,1000000,1,10,message);
        tiles = new LinkedList<>();
        tiles.add(monster);
        tiles.add(warrior);
        tiles.add(warriorCopy);
        board = new GameBoard(tiles);
        warrior.setPosition(new Position(0,0));
        warriorCopy.setPosition(new Position(0,0));
        monster.setPosition(new Position(0,1));
    }

    @Test
    public void deathTest(){
        warrior.onDeath();
        warriorCopy.setTile('X');
        warriorCopy.setAlive(false);
        Assert.assertTrue("Test if death works properly",warrior.getTile()==warriorCopy.getTile() && warrior.getAlive() == warriorCopy.getAlive());
    }

    @Test
    public void combatWithEnemyTest(){
        warrior.combat(monster);
        Assert.assertTrue("Test monster combat with player (1 in 1000000 chance to fail due to probability)", warrior.getHealthAmount()<1);
    }

    @Test
    public void combatWithPlayerTest(){
        monster.combat(warrior);
        Assert.assertTrue("Test player combat with monster (1 in 1000000 chance to fail due to probability)", monster.getHealthAmount()<1);
    }

    @Test
    public void getExperienceTest(){
        monster.onDeath(warrior,false);
        Assert.assertTrue("Test if player gets experience when killing a monster (1 in 1000000 chance to fail due to probability)", warrior.getExperience() == 1);
    }

    @Test
    public void switchPositionOnKillTest(){
        monster.onDeath(warrior,false);
        Assert.assertTrue("Test if player takes position of monster after killing it normally (1 in 1000000 chance to fail due to probability)", warrior.getPosition().equals(new Position(0,1)));
    }

    @Test
    public void stayPositionOnKillTest(){
        monster.onDeath(warrior,true);
        Assert.assertTrue("Test if player does not take position of monster after killing it with an ability (1 in 1000000 chance to fail due to probability)", !warrior.getPosition().equals(new Position(0,1)));
    }

    @Test
    public void removeEnemyTest(){
        monster.onDeath(warrior,false);
        Assert.assertTrue("Check if monster was removed from the board", board.getTile(new Position(0,1)).getTile() != 'm');
    }
}

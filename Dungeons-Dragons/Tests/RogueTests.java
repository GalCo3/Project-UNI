package Tests;

import BusinessLayer.Abstracts.Position;
import BusinessLayer.Abstracts.Tile;
import BusinessLayer.Enemies.Monster;
import BusinessLayer.Players.Rogue;
import UI.GameBoard;
import UI.MessageCallback;
import org.junit.*;
import java.util.LinkedList;

public class RogueTests {
    private Rogue rogue;
    private Rogue rogueCopy;
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
        monster = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        rogue = new Rogue("Rogue",1,1000,1,10,message);
        rogueCopy = new Rogue("Rogue",1,1000,1,10,message);
        tiles = new LinkedList<>();
        tiles.add(monster);
        tiles.add(rogue);
        tiles.add(rogueCopy);
        board = new GameBoard(tiles);
        rogue.setPosition(new Position(0,0));
        rogueCopy.setPosition(new Position(0,0));
        monster.setPosition(new Position(0,1));
    }

    @Test
    public void levelUpTest(){
        rogue.setExperience(50);
        rogue.levelUp();
        rogueCopy.setPlayerLevel(2);
        rogueCopy.setHealthAmount(21);
        rogueCopy.setHealthPool(21);
        rogueCopy.setAttackPoints(1014);
        rogueCopy.setDefensePoints(3);
        Assert.assertEquals("Test proper level up",rogueCopy,rogue);
    }

    @Test
    public void stepTickTest(){
        rogue.setCurrentEnergy(0);
        rogue.processStep();
        rogueCopy.setCurrentEnergy(10);
        Assert.assertEquals("Test process step",rogueCopy,rogue);
    }

    @Test
    public void abilityHitTest(){
        rogue.castAbility();
        Assert.assertTrue("Test ability killing a monster",monster.getHealthAmount()<1);
    }

    @Test
    public void abilityNoEnemyTest(){
        monster.setPosition(new Position(100,100));
        Monster monsterCopy = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        monsterCopy.setPosition(new Position(100,100));
        rogue.castAbility();
        Assert.assertEquals("Test casting ability with no enemy in range",monsterCopy,monster);
    }

    @Test
    public void abilityNoManaTest(){
        rogue.setCurrentEnergy(0);
        rogue.castAbility();
        Monster monsterCopy = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        monsterCopy.setPosition(new Position(0,1));
        Assert.assertEquals("Test casting ability with no energy",monsterCopy,monster);
    }

    @Test
    public void infoTest(){
        rogue.setExperience(0);
        Assert.assertEquals("Check proper stats information","Class: Rogue, Name: Rogue, Health: 1/1, Attack: 1000, Defense: 1, Level: 1, Experience: 0, Cost: 10, Energy: 100\n",rogue.getInfo());
    }

    @Test
    public void abilityTest(){
        Assert.assertEquals("Check for proper ability use text", "Rogue used Fan of Knives", rogue.printSpecialAbility());
    }
}

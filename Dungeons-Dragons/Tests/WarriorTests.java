package Tests;

import BusinessLayer.Abstracts.Position;
import BusinessLayer.Abstracts.Tile;
import BusinessLayer.Enemies.Monster;
import BusinessLayer.Players.Warrior;
import UI.GameBoard;
import UI.MessageCallback;
import org.junit.*;
import java.util.LinkedList;

public class WarriorTests {
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
        monster = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        warrior = new Warrior("Warrior",100,1000,1,10,message);
        warriorCopy = new Warrior("Warrior",100,1000,1,10,message);
        tiles = new LinkedList<>();
        tiles.add(monster);
        tiles.add(warrior);
        tiles.add(warriorCopy);
        board = new GameBoard(tiles);
        warrior.setPosition(new Position(0,0));
        warriorCopy.setPosition(new Position(0,0));
        monster.setPosition(new Position(0,2));
    }

    @Test
    public void levelUpTest(){
        warrior.setExperience(50);
        warrior.levelUp();
        warriorCopy.setPlayerLevel(2);
        warriorCopy.setHealthAmount(130);
        warriorCopy.setHealthPool(130);
        warriorCopy.setAttackPoints(1012);
        warriorCopy.setDefensePoints(5);
        Assert.assertEquals("Test proper level up",warriorCopy,warrior);
    }

    @Test
    public void stepTickTest(){
        warrior.setRemainingCooldown(10);
        warrior.processStep();
        warriorCopy.setRemainingCooldown(9);
        Assert.assertEquals("Test process step",warriorCopy,warrior);
    }

    @Test
    public void abilityHitTest(){
        warrior.castAbility();
        Assert.assertTrue("Test ability killing a monster",monster.getHealthAmount()<1);
    }

    @Test
    public void abilityNoEnemyTest(){
        monster.setPosition(new Position(100,100));
        Monster monsterCopy = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        monsterCopy.setPosition(new Position(100,100));
        warrior.castAbility();
        Assert.assertEquals("Test casting ability with no enemy in range",monsterCopy,monster);
    }

    @Test
    public void abilityCooldownTest(){
        warrior.setRemainingCooldown(0);
        warrior.castAbility();
        monster.setHealthAmount(1);
        Monster monsterCopy = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        monsterCopy.setPosition(new Position(0,2));
        Assert.assertEquals("Test casting ability when on cooldown",monsterCopy,monster);
    }

    @Test
    public void infoTest(){
        warrior.setExperience(0);
        Assert.assertEquals("Check proper stats information","Class: Warrior, Name: Warrior, Health: 100/100, Attack: 1000, Defense: 1, Level: 1, Experience: 0, Ability cooldown: 10, Remaining cooldown: 0\n",warrior.getInfo());
    }

    @Test
    public void abilityTest(){
        Assert.assertEquals("Check for proper ability use text", "Warrior used Avengers Shield", warrior.printSpecialAbility());
    }
}

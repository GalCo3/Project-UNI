package Tests;

import BusinessLayer.Abstracts.Position;
import BusinessLayer.Abstracts.Tile;
import BusinessLayer.Enemies.Monster;
import BusinessLayer.Players.Mage;
import UI.GameBoard;
import UI.MessageCallback;
import org.junit.*;
import java.util.LinkedList;

public class MageTests {
    private Mage mage;
    private Mage mageCopy;
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
        mage = new Mage("Mage",1,1000,1,10,1,1,1,3,message);
        mageCopy = new Mage("Mage",1,1000,1,10,1,1,1,3,message);
        tiles = new LinkedList<>();
        tiles.add(monster);
        tiles.add(mage);
        tiles.add(mageCopy);
        board = new GameBoard(tiles);
        mage.setPosition(new Position(0,0));
        mageCopy.setPosition(new Position(0,0));
        monster.setPosition(new Position(0,2));
    }

    @Test
    public void levelUpTest(){
        mage.setExperience(50);
        mage.levelUp();
        mageCopy.setPlayerLevel(2);
        mageCopy.setHealthAmount(21);
        mageCopy.setHealthPool(21);
        mageCopy.setAttackPoints(1008);
        mageCopy.setDefensePoints(3);
        mageCopy.setManaPool(60);
        mageCopy.setCurrentMana(17);
        mageCopy.setManaPool(60);
        mageCopy.setSpellPower(21);
        Assert.assertEquals("Test proper level up",mageCopy,mage);
    }

    @Test
    public void stepTickTest(){
        mageCopy.setCurrentMana(3);
        mage.processStep();
        Assert.assertEquals("Test process step",mageCopy,mage);
    }

    @Test
    public void abilityHitTest(){
        mage.castAbility();
        Assert.assertTrue("Test ability killing a monster",monster.getHealthAmount()<1);
    }

    @Test
    public void abilityNoEnemyTest(){
        monster.setPosition(new Position(100,100));
        Monster monsterCopy = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        monsterCopy.setPosition(new Position(100,100));
        mage.castAbility();
        Assert.assertEquals("Test casting ability with no enemy in range",monsterCopy,monster);
    }

    @Test
    public void abilityNoManaTest(){
        mage.setCurrentMana(0);
        mage.castAbility();
        Monster monsterCopy = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        monsterCopy.setPosition(new Position(0,2));
        Assert.assertEquals("Test casting ability with no mana",monsterCopy,monster);
    }

    @Test
    public void infoTest(){
        mage.setExperience(0);
        Assert.assertEquals("Check proper stats information","Class: Mage, Name: Mage, Health: 1/1, Attack: 1000, Defense: 1, Level: 1, Experience: 0, Mana: 2/10, Mana cost: 1, Spellpower: 1, Ability range: 3\n",mage.getInfo());
    }

    @Test
    public void abilityTest(){
        Assert.assertEquals("Check for proper ability use text", "Mage used Blizzard", mage.printSpecialAbility());
    }
}

package Tests;

import BusinessLayer.Abstracts.*;
import BusinessLayer.Enemies.Monster;
import BusinessLayer.Players.Hunter;
import UI.GameBoard;
import UI.MessageCallback;
import org.junit.*;
import java.util.LinkedList;

public class HunterTests {
    private Hunter hunter;
    private Hunter hunterCopy;
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
        hunter = new Hunter("Hunter",1,1000,1,3,message);
        hunterCopy = new Hunter("Hunter",1,1000,1,3,message);
        tiles = new LinkedList<>();
        tiles.add(monster);
        tiles.add(hunter);
        tiles.add(hunterCopy);
        board = new GameBoard(tiles);
        hunter.setPosition(new Position(0,0));
        hunterCopy.setPosition(new Position(0,0));
        monster.setPosition(new Position(0,2));
    }

    @Test
    public void levelUpTest(){
        hunter.setExperience(50);
        hunter.levelUp();
        hunterCopy.setPlayerLevel(2);
        hunterCopy.setHealthAmount(21);
        hunterCopy.setHealthPool(21);
        hunterCopy.setAttackPoints(1012);
        hunterCopy.setDefensePoints(5);
        hunterCopy.setArrowsCount(30);
        Assert.assertEquals("Test proper level up",hunterCopy,hunter);
    }

    @Test
    public void stepTickTest(){
        hunterCopy.setTicksCount(1);
        hunter.processStep();
        Assert.assertEquals("Test process step without adding arrows",hunterCopy,hunter);
    }

    @Test
    public void stepArrowsTest(){
        hunterCopy.setArrowsCount(11);
        for(int i = 0; i<11;i++)
            hunter.processStep();
        Assert.assertEquals("Test arrows added after 10 turns",hunterCopy,hunter);
    }

    @Test
    public void abilityHitTest(){
        hunter.castAbility();
        Assert.assertTrue("Test shooting and killing a monster",monster.getHealthAmount()<1);
    }

    @Test
    public void abilityNoEnemyTest(){
        monster.setPosition(new Position(100,100));
        Monster monsterCopy = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        monsterCopy.setPosition(new Position(100,100));
        hunter.castAbility();
        Assert.assertEquals("Test casting ability with no enemy in range",monsterCopy,monster);
    }

    @Test
    public void abilityNoArrowsTest(){
        hunter.setArrowsCount(0);
        hunter.castAbility();
        Monster monsterCopy = new Monster('m',"Monster", 1, 1, 0, 1,1, message);
        monsterCopy.setPosition(new Position(0,2));
        Assert.assertEquals("Test shooting without arrows",monsterCopy,monster);
    }

    @Test
    public void infoTest(){
        hunter.setExperience(0);
        Assert.assertEquals("Check proper stats information","Class: Hunter, Name: Hunter, Health: 1/1, Attack: 1000, Defense: 1, Level: 1, Experience: 0, Range: 3, Arrows count: 10\n",hunter.getInfo());
    }

    @Test
    public void abilityTest(){
        Assert.assertEquals("Check for proper ability use text", "Hunter used Shoot", hunter.printSpecialAbility());
    }
}

package Tests;

import BusinessLayer.Abstracts.Player;
import BusinessLayer.Abstracts.Position;
import BusinessLayer.Abstracts.Tile;
import BusinessLayer.Enemies.Trap;
import BusinessLayer.Players.Warrior;
import UI.GameBoard;
import UI.MessageCallback;
import org.junit.*;
import java.util.LinkedList;

public class TrapTests {
    private Trap t;
    private Trap tCopy;
    private GameBoard board;
    private Player player;
    private LinkedList<Tile> tiles;
    private MessageCallback message;

    //replace message printing
    public void printMessage(String message) {
    }

    @Before
    public void initTests(){
        message = this::printMessage;
        player = new Warrior("Player", 1, 1, 0, 1, message);
        t = new Trap('t',"Test",1,100000,1,1,1,2,message);
        tCopy = new Trap('t',"Test",1,100000,1,1,1,2,message);
        tiles = new LinkedList<>();
        tiles.add(player);
        tiles.add(t);
        tiles.add(tCopy);
        board = new GameBoard(tiles);
        t.setPosition(new Position(0,0));
        tCopy.setPosition(new Position(0,0));
        player.setPosition(new Position(1,1));
    }

    @Test
    public void CastTest(){
        t.cast();
        Assert.assertEquals("Check for no changes after cast",tCopy,t);
    }

    @Test
    public void TurnSetVisibleTest(){
        t.enemyTurn();
        t.enemyTurn();
        t.enemyTurn();
        t.enemyTurn();
        t.enemyTurn();
        Assert.assertTrue("Check if trap is invisible when invisibility timer passes",t.getVisible() & (t.getTile() == 't'));
    }

    @Test
    public void TurnSetInvisibleTest(){
        t.enemyTurn();
        t.enemyTurn();
        Assert.assertTrue("Check if trap is invisible when invisibility timer passes",!t.getVisible() & (t.getTile() != 't'));
    }

    @Test
    public void TurnAttackPlayerTest(){
        t.enemyTurn();
        Assert.assertTrue("Check if trap attacks player when near (1 in 100,000 chance to fail due to probability)",player.getHealthAmount()<1);
    }

    @Test
    public void TurnDoNothingTest(){
        player.setPosition(new Position(2,2));
        t.enemyTurn();
        tCopy.setTicksCount(tCopy.getTicksCount()+1);
        Assert.assertEquals("Check trap does nothing when no player near",tCopy,t);
    }

    @Test
    public void StepTest(){
        t.processStep();
        Assert.assertEquals("Check for no changes after process step",tCopy,t);
    }

    @Test
    public void InfoTest(){
        Assert.assertEquals("Check proper stats information",t.getInfo(),"Name: Test, Health: 1/1, Attack: 100000, Defense: 1, Experience value: 1, Visibility time: 1, Invisibility time: 2\n");
    }
}

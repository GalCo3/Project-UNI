package UI;

import BusinessLayer.Abstracts.Position;
import BusinessLayer.Abstracts.Tile;
import java.io.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Scanner;
import java.util.stream.Collectors;

public class GameManager {

    private GameBoard board;
    private final String levelPath;
    private final TileFactory tileFactory;
    private final GameView gameView;
    private boolean stop;

    public GameManager(String path){
        this.gameView = new GameView();
        tileFactory = new TileFactory(gameView.getMessage());
        levelPath = path;
        stop = false;
    }

    public void startGame(){
        selectPlayer();
        play();
    }

    public void play(){
        for (int i = 1; i < 5 & !stop; i = i + 1) {
            List<String> level = loadPath(levelPath + "\\level" + i + ".txt");
            if (level.isEmpty()) throw new IllegalArgumentException("This file is Illegal");
            boardInit(level);
            board.setTiles(board.getTiles().stream().sorted(Tile::compareTo).collect(Collectors.toCollection(ArrayList::new)));
            gameView.printBoard(board);
            gameView.printMessage(board.getPlayer().getInfo());
            while (levelCheck()) {
              tick();
                    if (!board.getEnemies().isEmpty()) {
                        board.setTiles(board.getTiles().stream().sorted(Tile::compareTo).collect(Collectors.toCollection(ArrayList::new)));
                        gameView.printBoard(board);
                        gameView.printMessage(board.getPlayer().getInfo());
                    }
                }
                if (board.getPlayer().getTile() == 'X') {
                    gameView.printMessage("Game Over.");
                    stop = true;
                    return;
                }
                if (i==5 && board.getEnemies().size() == 0 )
                {
                    gameView.printMessage("winner winner chicken dinner! "+"\n Game Over.");
                }
            }
    }

    public void selectPlayer() {
        Scanner scanner = new Scanner(System.in);
        gameView.printMenu(tileFactory.listPlayers());
        String selectedPlayer = scanner.nextLine();
        while (!playerSelectCheck(selectedPlayer)) {
            gameView.printMessage("Invalid input. Please select a number from the list");
            gameView.printMenu(tileFactory.listPlayers());
            selectedPlayer = scanner.nextLine();
        }
        int input = Integer.parseInt(selectedPlayer);
        tileFactory.producePlayer(input);
    }

    public void boardInit(List<String> chars){
        List<Tile> tiles = new ArrayList<>();
        for (int i = 0; i < chars.size(); i++) {
            for (int j = 0; j < chars.get(i).length(); j++) {
                Position p = new Position(j,i);
                Tile toAdd = tileFactory.produceTile(chars.get(i).charAt(j),p);
                tiles.add(toAdd);
            }
        }
        board = new GameBoard(tiles);
    }

    public void tick(){
        playerTick();
        enemyTick();
    }

    public boolean levelCheck(){
        return board.getEnemies().size() > 0 && board.getPlayer().getTile() != 'X';
    }

    private static List<String> loadPath(String path) {
        List<String> lines = new ArrayList<>();
        try {
            BufferedReader reader = new BufferedReader(new FileReader(path));
            String next;
            while ((next = reader.readLine()) != null) {
                lines.add(next);
            }
        } catch (FileNotFoundException e) {
            System.out.println("Levels not found");
        } catch (IOException e) {
            System.out.println(e.getMessage() + "\n" + e.getStackTrace());
        }
        return lines;
    }

    public void playerTick(){
        Scanner scanner = new Scanner(System.in);
        String input = scanner.next();
        while (!checkInput(input)){
            gameView.printMessage("Illegal input. Legal inputs are: 'w','a','s','d' to move, 'q' to skip turn and 'e' to use special ability");
            input = scanner.next();
        }
        Tile player = board.getPlayer();
        Position playerPos = new Position(player.getPosition().getX(),player.getPosition().getY());

        if(input.equals("w") | input.equals("W")){
            Tile t = board.getTile(new Position(playerPos.getX(), playerPos.getY() - 1));
            player.visit(t);
        }
        else if(input.equals("a") | input.equals("A")){
            Tile t = board.getTile(new Position(playerPos.getX() -1, playerPos.getY()));
            player.visit(t);
        }
        else if(input.equals("s") | input.equals("S")){
            Tile t = board.getTile(new Position(playerPos.getX(), playerPos.getY() + 1));
            player.visit(t);
        }
        else if(input.equals("d") | input.equals("D")){
            Tile t = board.getTile(new Position(playerPos.getX() + 1, playerPos.getY()));
            player.visit(t);
        }
        else if(input.equals("e") | input.equals("E")){
            player.cast();
        }
        else if(input.equals("q") | input.equals("Q")){
            return;
        }
        player.processStep();
    }

    public void enemyTick(){
        List<Tile> monsters = board.getEnemies();
        for (int i = 0; i<monsters.size();i++) {
            Tile monster = monsters.get(i);
            monster.enemyTurn();
            monster.processStep();
        }
    }

    public boolean playerSelectCheck(String input) {
        try {
            int inputVal = Integer.parseInt(input);
            return inputVal > 0 && inputVal <= tileFactory.getPlayersList().size();
        }
        catch (NumberFormatException e) {
            return false;
        }
    }

        private boolean checkInput(String input) {
        if (input.length() != 1)
            return false;
        else {
            String legalInputs = "wasdqeWASDQE";
            return legalInputs.indexOf(input.charAt(0)) != -1;
        }
    }

    public static void main(String []args ){
        String currentWorkingDir = args[0];
        //String currentWorkingDir = (System.getProperty("user.dir") ) + File.separator + "levels_dir";
        GameManager gameManager = new GameManager(currentWorkingDir);
        gameManager.startGame();
    }
}

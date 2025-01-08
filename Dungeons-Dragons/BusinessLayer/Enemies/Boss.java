package BusinessLayer.Enemies;

import BusinessLayer.Abstracts.*;
import UI.MessageCallback;
import java.util.Random;

public class Boss extends Monster implements HeroicUnit {
    private Integer abilityFreq;
    private Integer combatTicks;

    public Boss(char Character, String Name, Integer Health, Integer Attack, Integer Defense, Integer Experience, Integer visionRange, Integer abilityFreq, MessageCallback message) {
        super(Character, Name, Health, Attack, Defense, Experience, visionRange,message);
        this.abilityFreq = abilityFreq;
        this.combatTicks = 0;
    }

    public void setCombatTicks(Integer combatTicks) {
        this.combatTicks = combatTicks;
    }
    public Integer getCombatTicks() {
        return combatTicks;
    }
    public Integer getAbilityFreq() {
        return abilityFreq;
    }

    @Override
    public void castAbility() {
        Tile player = board.getPlayer();
        specialAbility(player,getAttackPoints());
    }

    @Override
    public void enemyTurn(){
        if(Unit.range(this, board.getPlayer()) < getVisionRange()){
            if(getCombatTicks() == getAbilityFreq()){
                setCombatTicks(0);
                castAbility();
            }
            else{
                Tile player = board.getPlayer();
                setCombatTicks(getCombatTicks() + 1);
                int dx = getPosition().getX() - player.getPosition().getX();
                int dy = getPosition().getY() - player.getPosition().getY();
                if (Math.abs(dx) > Math.abs(dy)) {
                    if (dx > 0) {
                        Tile tile1 = board.getTile(getPosition().getX() - 1, getPosition().getY());
                        visit(tile1);
                    } else {
                        Tile tile1 = board.getTile(getPosition().getX() + 1, getPosition().getY());
                        visit(tile1);
                    }
                }
                else {
                    if (dy > 0) {
                        Tile tile1 = board.getTile(getPosition().getX(), getPosition().getY() - 1);
                        visit(tile1);
                    } else {
                        Tile tile1 = board.getTile(getPosition().getX(), getPosition().getY() + 1);
                        visit(tile1);
                    }
                }
            }
        }
        else {
            setCombatTicks(0);
            Random random = new Random();
            int next = random.nextInt(4);
            if (next == 0) {
                Tile t = board.getTile(new Position(getPosition().getX(), getPosition().getY() - 1));
                visit(t);
            } else if (next == 1) {
                Tile t = board.getTile(new Position(getPosition().getX() - 1, getPosition().getY()));
                visit(t);
            } else if (next == 2) {
                Tile t = board.getTile(new Position(getPosition().getX(), getPosition().getY() + 1));
                visit(t);
            } else {
                Tile t = board.getTile(new Position(getPosition().getX() + 1, getPosition().getY()));
                visit(t);
            }
        }
    }

    @Override
    public String getInfo() {
        return super.getInfo() + ", Ability frequency: " + getAbilityFreq() + ", Combat ticks: " + getCombatTicks();
    }
}

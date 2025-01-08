package BusinessLayer.Abstracts;

import UI.MessageCallback;

import java.util.Random;

public abstract class Unit extends Tile {
    private String name;
    private Integer healthPool;
    private Integer healthAmount;
    private Integer attackPoints;
    private Integer defensePoints;

    protected Unit(char tile, String nameVal, int healthCapacity, int attack, int defense, MessageCallback messageCallback) {
        super(tile,messageCallback);
        name = nameVal;
        healthAmount = healthCapacity;
        healthPool = healthCapacity;
        attackPoints = attack;
        defensePoints = defense;
    }

    public String getName() {
        return name;
    }

    public Integer getHealthPool() {
        return healthPool;
    }

    public void setHealthPool(Integer newHealth) {
        healthPool = newHealth;
    }

    public Integer getHealthAmount() {
        return healthAmount;
    }

    public void setHealthAmount(Integer newHealth) {
        healthAmount = newHealth;
    }

    public Integer getAttackPoints() {
        return attackPoints;
    }

    public void setAttackPoints(Integer newAttack) {
        attackPoints = newAttack;
    }

    public Integer getDefensePoints() {
        return defensePoints;
    }

    public void setDefensePoints(Integer newDefenseVal) {
        defensePoints = newDefenseVal;
    }

    public abstract void combat(Unit other);
    public abstract void combat(Unit other,int attackPoints);
    public abstract void playerCombat(Enemy enemy);
    public abstract void playerCombat(Enemy enemy,int attackPoints);
    public abstract void enemyCombat(Player player,int attackPoints);
    public abstract void enemyCombat(Player player);

    @Override
    public void visit(Tile other) {
        other.accept(this);
    }

    @Override
    public void accept(Unit other) {
        combat(other);
    }

    public void accept(Unit other,int attackPoints) {
        this.combat(other,attackPoints);
    }

    @Override
    public void specialAbility(Tile other, int attack) {
        other.accept(this, attack);
    }

    protected int attack() {
        Random rand = new Random();
        return rand.nextInt(getAttackPoints() + 1);
    }

    public int defend() {
        Random rand = new Random();
        return rand.nextInt(getDefensePoints() + 1);
    }

    public static double range(Tile a, Tile b){
        Position aPos = a.getPosition();
        Position bPos=  b.getPosition();
        return Math.sqrt(((aPos.getX()-bPos.getX())*(aPos.getX()-bPos.getX()))+((aPos.getY()-bPos.getY())*(aPos.getY()-bPos.getY())));
    }
}




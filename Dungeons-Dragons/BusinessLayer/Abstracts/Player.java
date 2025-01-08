package BusinessLayer.Abstracts;

import UI.MessageCallback;

public abstract class Player extends Unit implements HeroicUnit {
    public static Integer experience = 0;
    public static Integer playerLevel = 1;
    public boolean isAlive;

    public Player(String name, int healthCapacity, int attack, int defense,MessageCallback message)
    {
        super('@',name,healthCapacity,attack,defense,message);
        isAlive=true;
    }

    public  Integer getExperience() {
        return experience;
    }

    public void setExperience(Integer experience) {
        Player.experience = experience;
    }

    public Integer getPlayerLevel(){
        return playerLevel;
    }

    public void setPlayerLevel(Integer level){
        playerLevel = level;
    }

    public boolean getAlive() {
        return isAlive;
    }

    public void setAlive(boolean status){
        isAlive = status;
    }

    public MessageCallback getMessage (){return message;}

    @Override
    public boolean isPlayer() {
        return true;
    }

    @Override
    public boolean isEnemy() {
        return false;
    }

    @Override
    public void combat(Unit other) {
        other.enemyCombat(this);
    }

    @Override
    public void enemyCombat(Player player) {}

    @Override
    public void enemyCombat(Player player, int attackPoints) {}

    @Override
    public void enemyTurn() {}

    @Override
    public void castAbility() {
        castAbility();
    }

    @Override
    public void cast() {
        castAbility();
    }

    public abstract String printSpecialAbility ();

    public void levelUp(){
        if(getExperience() >= 50 * getPlayerLevel()) {
            setExperience(getExperience() - (50 * getPlayerLevel()));
            setPlayerLevel(getPlayerLevel() + 1);
            setHealthPool(getHealthPool() + (10 * getPlayerLevel()));
            setHealthAmount(getHealthPool());
            setAttackPoints(getAttackPoints() + (4 * getPlayerLevel()));
            setDefensePoints(getDefensePoints() + (getPlayerLevel()));
            message.send(getName() + " leveled up to level " + getPlayerLevel());
        }
        else return;
    }

    @Override
    public void playerCombat(Enemy enemy) {
        int attack = attack();
        int defense = enemy.defend();
        int damageTaken = attack - defense;
        if(damageTaken < 0) damageTaken = 0;
        message.send(getInfo());
        message.send(enemy.getInfo());
        message.send(getName() + " rolled " + attack + " attack");
        message.send(enemy.getName() + " rolled " + defense + " defense");
        message.send(enemy.getName() + " took " + damageTaken + " damage"+"\n");
        enemy.setHealthAmount(enemy.getHealthAmount() - damageTaken);
        if (enemy.getHealthAmount()<=0)
            enemy.onDeath(this, true);
    }

    @Override
    public void playerCombat(Enemy enemy , int attackPoints) {
        int attack = attackPoints;
        int defense = enemy.defend();
        int damageTaken = attack - defense;
        if(damageTaken < 0) damageTaken = 0;
        message.send(getInfo());
        message.send(enemy.getInfo());
        message.send(printSpecialAbility());
        message.send(enemy.getName() + " rolled " + defense + " defense");
        message.send(enemy.getName() + " took " + damageTaken + " damage"+"\n");
        enemy.setHealthAmount(enemy.getHealthAmount() - damageTaken);
        if (enemy.getHealthAmount()<=0)
            enemy.onDeath(this, true);
    }

    @Override
    public void combat(Unit other, int attackPoints) {
        other.enemyCombat(this,attackPoints);
    }

    public void onDeath() {
        setTile('X');
        isAlive=false;
        message.send(getName() + " died");
    }
    @Override
    public String getInfo(){
        return ("Name: " + getName()+ ", Health: " + getHealthAmount() + "/" + getHealthPool() +", Attack: " + getAttackPoints() + ", Defense: " + getDefensePoints() + ", Level: " + getPlayerLevel() + ", Experience: " + getExperience());
    }
}

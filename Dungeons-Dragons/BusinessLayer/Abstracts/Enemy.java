package BusinessLayer.Abstracts;

import UI.MessageCallback;

public abstract class Enemy extends Unit {
    public Integer experienceValue;

    protected Enemy(char tile, String name, int healthCapacity, int attack, int defense, MessageCallback message) {
        super(tile, name, healthCapacity, attack, defense, message);
    }

    @Override
    public void combat(Unit other, int attackPoints) {
        other.playerCombat(this,attackPoints);
    }

    public Integer getExperienceValue() {
        return experienceValue;
    }

    @Override
    public boolean isEnemy() {
        return true;
    }

    @Override
    public boolean isPlayer() {
        return false;
    }

    @Override
    public void combat(Unit other) {
        other.playerCombat(this);
    }

    @Override
    public void playerCombat(Enemy enemy) {}

    @Override
    public void playerCombat(Enemy enemy, int attackPoints) {}

    @Override
    public void enemyCombat(Player player) {
        int attack = attack();
        int defense = player.defend();
        int damageTaken = attack - defense;
        if (damageTaken < 0) damageTaken = 0;
        message.send(getInfo());
        message.send(player.getInfo());
        message.send(getName() + " rolled " + attack + " attack");
        message.send(player.getName() + " rolled " + defense + " defense");
        message.send(player.getName() + " took " + damageTaken + " damage"+"\n");
        player.setHealthAmount(player.getHealthAmount() - damageTaken);
        if (player.getHealthAmount()<= 0 ) {
            player.onDeath();
        }
    }

    @Override
    public void enemyCombat(Player player, int attackPoints) {
        int attack = attackPoints;
        int defense = player.defend();
        int damageTaken = attack - defense;
        if (damageTaken < 0) damageTaken = 0;
        message.send(getInfo());
        message.send(player.getInfo());
        message.send(getName() + " used special ability");//only in boss
        message.send(player.getName() + " rolled " + defense + " defense");
        message.send(player.getName() + " took " + damageTaken + " damage"+"\n");
        player.setHealthAmount(player.getHealthAmount() - damageTaken);
        if (player.getHealthAmount()<= 0 )
            player.onDeath();
    }

    public void onDeath(Player p, boolean ability) {
        p.setExperience(p.getExperience()+ getExperienceValue());
        p.levelUp();
        board.removeEnemy(this,ability);
    }

    @Override
    public String getInfo() {
        return null;
    }

}

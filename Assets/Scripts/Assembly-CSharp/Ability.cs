public class Ability
{
	public int fame;

	public int loyalty;

	public float attack;

	public float defense;

	public float strength;

	public float intellectual;

	public float constitution;

	public float critical;

	public float speed;

	public float colltime;

	public float hp;

	public float mp;

	public Ability Clone()
	{
		Ability ability = new Ability();
		ability.fame = fame;
		ability.loyalty = loyalty;
		ability.attack = attack;
		ability.defense = defense;
		ability.strength = strength;
		ability.intellectual = intellectual;
		ability.constitution = constitution;
		ability.critical = critical;
		ability.speed = speed;
		ability.colltime = colltime;
		ability.hp = hp;
		ability.mp = mp;
		return ability;
	}
}

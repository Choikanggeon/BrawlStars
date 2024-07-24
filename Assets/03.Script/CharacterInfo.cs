using UnityEngine;

public enum FightType
{
    Shelly,
    Colt,
    Nita,
    Barley,
    EnergyBox
}

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class CharacterInfo : ScriptableObject {
    public FightType _fightType;

    public int _health;
    public int _damage;
    public float _bulletfastVelocity;
    public float _bulletRange;
    public int _ammo;
    public float _reloadTime;
    public int _energyNum;

    #region Skill variable
    public int _skillEnergyLimit;
    public float _skillRange;
    public float _skillDamage;
    #endregion

}

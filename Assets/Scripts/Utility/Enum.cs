public enum CardType
{
    AttackCard,//攻击卡
    AbilityCard,//效果卡
    PAffectCard,//单玩家效果卡
    SpecialCard,//特殊卡
    None
}

public enum TurnState
{
    PlayerTurnStart,
    PlayerTurn,
    PlayerTurnEnd,
    EnemyTurnStart,
    EnemyTurnEnd,
}

public enum EquipmentType
{
    /// <summary>
    /// 头盔
    /// </summary>
    Helmet,

    /// <summary>
    /// 胸甲
    /// </summary>
    Cuirass,

    /// <summary>
    /// 武器
    /// </summary>
    Weapon,

    /// <summary>
    /// 鞋子
    /// </summary>
    Shoe
}

public enum RoomType
{
    SmallEnemy,
    BigEnemy,
    Shop,
    Event,
    Treasure,
    Boss,
}

public enum RoomStage
{
    Visited,//已访问
    CanVisit,//未访问但是可以访问
    CantVisit//不可访问
}

/// <summary>
/// 游戏当前所在阶段或者说场景
/// </summary>
public enum GameState
{
    Map,
    Bar,
    Battle,
    Shop,
    Treasure,
    Delete,
    Event,
    CardBag,
}

public enum CardQuality
{
    White,
    Green,
    Blue,
    Purple,
    Gold,
}
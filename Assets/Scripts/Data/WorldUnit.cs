public class WorldUnit
{
    public int offsetX;
    public int offsetY;

    /// <summary>
    /// 1 是普通的墙
    /// </summary>
    public int state;

    public WorldUnit (int _ox, int _oy, int _state)
    {
        this.offsetX = _ox;
        this.offsetY = _oy;
        this.state = _state;
    }
}

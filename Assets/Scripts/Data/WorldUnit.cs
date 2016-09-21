//using TreaslandLib.Algorithms.AStar;
public class WorldUnit //: Point
{
    public int offsetX;
    public int offsetY;

    /// <summary>
    /// 1 是普通的墙
    /// </summary>
    public int state;

    public WorldUnit (int _ox, int _oy, int _state) //: base(_ox,_oy)
    {
        this.offsetX = _ox;
        this.offsetY = _oy;
        this.state = _state;
    }
}

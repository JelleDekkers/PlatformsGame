public class MovementInfo {

    public BlockMoveable block;
    public IntVector2 direction;
    public IntVector2 newDirection;
    public Block neighbourBlock;

    /// <summary>
    /// Simple class to help with moving neighbours and remember new directions in case of Portals. 
    /// </summary>
    /// <param name="block"></param>
    /// <param name="direction"></param>
    public MovementInfo(BlockMoveable block, IntVector2 direction, IntVector2 newDirection, Block neighbourBlock) {
        this.block = block;
        this.direction = direction;
        this.newDirection = newDirection;
        this.neighbourBlock = neighbourBlock;
    }
}

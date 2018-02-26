public class MovementInfo {

    public BlockMoveable block;
    public Tile newTile;
    public Portal portal;
    public IntVector2 direction, newDirection;
    public Block neighbourBlock;

    /// <summary>
    /// Simple class to help with moving neighbours and remember new directions in case of Portals. 
    /// </summary>
    public MovementInfo(BlockMoveable block, IntVector2 direction, IntVector2 newDirection, Portal portal, Tile newTile, Block neighbourBlock) {
        this.block = block;
        this.direction = direction;
        this.portal = portal;
        this.newDirection = newDirection;
        this.neighbourBlock = neighbourBlock;
        this.newTile = newTile;
    }
}

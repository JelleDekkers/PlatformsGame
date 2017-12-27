public class MovementInfo {

    public BlockMoveable block;
    public IntVector2 direction;

    public MovementInfo(BlockMoveable b, IntVector2 d) {
        block = b;
        direction = d;
    }
}

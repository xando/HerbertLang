using System.Text;

namespace HerbertLang;

public static class Tile {
    public const string PLAYER_UP = "⇧";
    public const string PLAYER_RIGHT = "⇨";
    public const string PLAYER_DOWN = "⇩";
    public const string PLAYER_LEFT = "⇦";
    public const string OBSTACLE = "#";
    public const string SPACE = "_";
    public const string ITEM = "★";

    public static readonly Dictionary<string, int> toDirection = new Dictionary<string, int> {
        {Tile.PLAYER_UP, 0},
        {Tile.PLAYER_RIGHT, 1},
        {Tile.PLAYER_DOWN, 2},
        {Tile.PLAYER_LEFT, 3},
    };

    public static readonly Dictionary<int, string> toTile = toDirection.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

}


public struct PlayerPosition {
    public int x;
    public int y;
}

public struct Player {
    public PlayerPosition position;
    public int direction;
}


public class World {
    public string[,] world;

    public Player player;

    public World(string[,] world) {
        this.world = world;
        this.player = getPlayerLocation(world);
    }

    static Player getPlayerLocation(string[,] world) {

        for (int y = 0; y < world.GetLength(0); y++) {
            for (int x = 0; x < world.GetLength(1); x++) {
                if (world[y, x] is
                    Tile.PLAYER_UP or
                    Tile.PLAYER_RIGHT or
                    Tile.PLAYER_DOWN or
                    Tile.PLAYER_LEFT) {

                    var direction = Tile.toDirection[world[y, x]];
                    var position = new PlayerPosition { x = x, y = y };

                    return new Player { position = position, direction = direction, };
                }
            }
        }

        throw new SolutionError("No player found in the world");
    }

    public bool movePlayerForward() {
        var nextPosition = new PlayerPosition {
            x = this.player.position.x,
            y = this.player.position.y
        };

        if (player.direction == 0) {
            nextPosition.y--;
        } else
        if (player.direction == 1) {
            nextPosition.x++;
        } else
        if (player.direction == 2) {
            nextPosition.y++;
        } else
        if (player.direction == 3) {
            nextPosition.x--;
        }

        if (nextPosition.y > this.world.GetUpperBound(0) || nextPosition.y < 0) {
            return false;
        }
        if (nextPosition.x > this.world.GetUpperBound(1) || nextPosition.x < 0) {
            return false;
        }
        if (this.world[nextPosition.y, nextPosition.x] == Tile.OBSTACLE) {
            return false;
        }

        this.world[player.position.y, player.position.x] = Tile.SPACE;

        this.player.position = nextPosition;

        if (this.world[player.position.y, player.position.x] == Tile.ITEM) {
            this.world[player.position.y, player.position.x] = Tile.SPACE;
        }

        return true;
    }

    public void turnPlayerLeft() {
        this.player.direction = (this.player.direction - 1) % 4;
    }

    public void turnPlayerRight() {
        this.player.direction = (this.player.direction + 1) % 4;
    }

    public bool isSolved() {
        for (int y = 0; y < this.world.GetLength(0); y++) {
            for (int x = 0; x < this.world.GetLength(1); x++) {
                if (this.world[y, x] == Tile.ITEM) {
                    return false;
                }
            }
        }
        return true;
    }

    public override string ToString() {
        var sb = new StringBuilder();
        
        for (int y = 0; y <= this.world.GetUpperBound(0); y++) {
            for (int x = 0; x <= this.world.GetUpperBound(1); x++) {
                if (player.position.x == x && player.position.y == y) {
                    sb.Append(Tile.toTile[this.player.direction]);
                } else {
                    sb.Append(this.world[y, x]);
                }
            }
            sb.Append("\n");
        }

        return sb.ToString();
    }

}

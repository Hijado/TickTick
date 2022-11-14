using Engine;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

/// <summary>
/// Represents a rocket enemy that flies horizontally through the screen.
/// </summary>
class Rocket : AnimatedGameObject
{
    Level level;
    Vector2 startPosition;
    const float speed = 500;
    bool notHit = true;

    public Rocket(Level level, Vector2 startPosition, bool facingLeft) 
        : base(TickTick.Depth_LevelObjects)
    {
        this.level = level;

        LoadAnimation("Sprites/LevelObjects/Rocket/spr_rocket@3", "rocket", true, 0.1f);
        LoadAnimation("Sprites/LevelObjects/Player/spr_explode@5x5", "Bomb", false, 0.04f);
        PlayAnimation("rocket");
        SetOriginToCenter();

        sprite.Mirror = facingLeft;
        if (sprite.Mirror)
        {
            velocity.X = -speed;
            this.startPosition = startPosition + new Vector2(2 * speed, 0);
        }
        else
        {
            velocity.X = speed;
            this.startPosition = startPosition - new Vector2(2 * speed, 0);
        }
        Reset();
    }

    public override void Reset()
    {
        // go back to the starting position
        LocalPosition = startPosition;
        notHit = true;
        PlayAnimation("rocket");
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // if the rocket has left the screen, reset it
        if (sprite.Mirror && BoundingBox.Right < level.BoundingBox.Left)
            LocalPosition = startPosition;
        else if (!sprite.Mirror && BoundingBox.Left > level.BoundingBox.Right)
            LocalPosition = startPosition;

        // if the rocket touches the player, the player dies, if the player jumps on the rocket, rocket dissappears
        // visible tracks if its visible
        if (level.Player.CanCollideWithObjects && HasPixelPreciseCollision(level.Player) && notHit)
        {
            // if player position is higher that this object position then it means collision with top
            // player needs to be falling to prevent jumping into rocket and then triggering this if statement
            if (level.Player.IsFalling && level.Player.GlobalPosition.Y < this.GlobalPosition.Y)
            {
                // sets visible to false to prevent player from dying to afterimage
                notHit = false;

                // plays the bomb animation and explode soundeffect, also lets player jump, mario effect
                PlayAnimation("Bomb");
                ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_player_explode");
                level.Player.Jump();

            }
            // all other collision player dies
            else
            {
                level.Player.Die();
                Reset();
            }
        }
    }
}

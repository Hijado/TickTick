using System;
using Microsoft.Xna.Framework;
using Engine;


class PowerUpOrDown : SpriteGameObject
{
    Level level;
    Vector2 startPosition;

    // for tracking power up/down time so that after a while it can be reverted
    int timer = 0;

    // for one of the two powers
    int power;

    // for the bounce
    protected float bounce;

    public PowerUpOrDown(Level level, Vector2 startPosition) : base("Sprites/LevelObjects/powerUpOrDown", TickTick.Depth_LevelObjects)
    {
        // gives this object acces to the level, for collision with player
        this.level = level;
        this.startPosition = startPosition;

        // random power
        power = TickTick.Random.Next(2);

        SetOriginToCenter();
        Reset();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        randomPower();

        // copied from the waterdrop class to add a bounce effect
        double t = gameTime.TotalGameTime.TotalSeconds * 3.0f + LocalPosition.X;
        bounce = (float)Math.Sin(t) * 0.2f;
        localPosition.Y += bounce;

        // mirrors the lucky block after 0.5 sec
        if (gameTime.TotalGameTime.Milliseconds > 500)
            sprite.Mirror = true;
        else
            sprite.Mirror = false;
    }

    /// <summary>
    /// gives lucky block a one of the two adjustments and after a few seconds restores the "original" value
    /// </summary>
    protected void randomPower()
    {
        // in the unfortunate cricumstance that the player gets 2 power downs in a row, idle animation will be played
        if (level.Player.walkingSpeedAdjustment == -400)
            level.Player.PlayAnimation("idle");

        // check if the player collets the power up/down, chosen at random
        if (Visible && level.Player.CanCollideWithObjects && HasPixelPreciseCollision(level.Player))
        {
            switch (power)
            {
                case 0:
                    level.Player.walkingSpeedAdjustment -= 200;
                    break;
                case 1:
                    level.Player.walkingSpeedAdjustment += 200;
                    break;
            }
            Visible = false;
            ExtendedGame.AssetManager.PlaySoundEffect("Sounds/snd_power");
        }

        // after 10 seconds, power up/down will fade
        if (!this.Visible)
        {
            if (timer == 500 && power == 0)
                level.Player.walkingSpeedAdjustment += 200;
            else if (timer == 500 && power == 1)
                level.Player.walkingSpeedAdjustment -= 200;

            if (timer <= 500)
                timer++;
            else
                return;
        }
    }

    /// <summary>
    /// Resets position of lucky block and makes it visible if collected
    /// </summary>
    public override void Reset()
    {
        localPosition = startPosition;
        Visible = true;
    }
}


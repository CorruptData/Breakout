﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

public class Ball
{
    //Starting position of the ball.
    double PositionX = 45;
    double PositionY = 190;
    //Size of the ball (diameter).
    static int Size = 7;

    //Speed in pixels/sec.
    double Speed = 2;
    //Angle of the ball.
    double Theta = -Math.PI/4;

    //The maximum angle a ball can riccochet off the paddle.
    //Unimplemented.
    double MaxReflect = Math.PI;

    //The total points.
    public static int Points = 0;

    //Pause the ball after death.
    bool Paused = true;
    //Time in seconds to hold the ball in place after death.
    double PauseTimer = 3;

    //The game over condition.
    public static bool Dead = false;
    //Number of balls remaining.
    public static int BallsLeft;
    public static readonly int BallsLeftStart = 2;

    //For those times when the ball is not doing much.
    double DontBeMadTimer;
    readonly double DontBeMadTimerDefault = 10;
    
    //The balls' texture.
    Texture2D BallTexture;

    public Ball()
    {
        DontBeMadTimer = DontBeMadTimerDefault;
        BallTexture = Utils.TextureLoader("ball.png");
    }

    public void Draw(AD2SpriteBatch sb)
    {
        sb.DrawTexture(BallTexture, (int)PositionX, (int)PositionY);

        if (DontBeMadTimer < 0)
            Utils.DefaultFont.Draw(sb, "ENJOY THE MUSIC", 40, 212, Color.White, 2, true);
    }

    public void Update(InGame world, int ms)
    {
        if (!Paused)
        {
            //Break movements into small steps for edge collision.
            int steps = (int)Speed + 1;
            DontBeMadTimer -= (double)ms / 1000;
            for (int step = 0; step != steps; step++)
            {
                
                //Move the ball based on the angle.
                PositionX += (Math.Cos(Theta) * Speed) / steps;
                PositionY += (Math.Sin(Theta) * Speed) / steps;

                //check for collision with the world.
                if (WorldCollide(world))
                    return;
                //check for collision with the paddle.
                PaddleCollide(world);
                //Check for collision with bricks.
                BounceBallOffBricks(world);
            }
        }else //If the ball is being held after death,
        {
            //roll the Pause Timer.
            PauseTimer -= (double)ms/1000;
            //Has the Pause Timer expired?
            if (PauseTimer <= 0)
                //The ball is released.
                Paused = false;
        }
    }

    bool WorldCollide(InGame world)
    {
        //check for vertical world collision
        if ((PositionX <= 0) || (PositionX + Size >= Breakout.StageWidth))
            //Hit a side! Reflect the x direction of the ball.
            FlipThetaX();
        //check for horizontal world collision
        if (PositionY <= 0)
            //Hit the top! Reflect the y direction of the ball.
            FlipThetaY();
        else if (PositionY > Breakout.BaseHeight)
            //if the ball is off the borrom, then it dies.
            BallOut(world);
        return PositionY > Breakout.BaseHeight;
    }

    void FlipThetaX()
    {
        //Reflect the X direction of the ball.
        Theta = -(Theta + -(Math.PI / 2)) + (Math.PI / 2);
    }

    void FlipThetaY()
    {
        //Reflect the Y direction of the ball.
        Theta = -(Theta);
    }

    void BounceBallOffBricks(InGame world)
    {
        foreach (Brick br in world.Bricks)
        {
            //See if the ball is touching any of the bricks.
            if ((br.BrickLive) && InGame.Collide((int)PositionX, (int)PositionY, Size, Size, br.PositionX, br.PositionY, br.Width, br.Height))
            {
                //Now check if the collision is on the top or bottom,
                if ((TopCollide(br)) || BottomCollide(br))
                    //If so, then reflect the Y direction of the ball.
                    FlipThetaY();
                //Otherwise, it can be assumed it hit on one of the sides,
                else
                    //and we can reflect the X direction.
                    FlipThetaX();

                //Get points
                switch (br.BrickType)
                {
                    case ("normal"):
                        Points++;
                        break;

                    case ("blue"):
                        Points += 2;
                        break;
                }
                //Kill the brick.
                br.BrickLive = false;
                //Play the hit sound.
                SoundManager.Play("hit.wav");
                //Reset the fun stuff.
                DontBeMadTimer = DontBeMadTimerDefault;
                //It is assumed you can only hit one brick at a time.
                //This SHOULD be looked into, but I probably won't.
                return;
            }
        }
    }
    
    void PaddleCollide(InGame world)
    {
        if (InGame.Collide((int)PositionX, (int)PositionY + Size - 1, Size, 1, (int)world.Player.PositionX, world.Player.PositionY, world.Player.Width, 1) && Math.Sin(Theta) > 0)
        {
            Theta = (((PositionX + (Size / 2) + -world.Player.PositionX) / (world.Player.Width)) + -1) * MaxReflect;

            DontBeMadTimer = DontBeMadTimerDefault;
        }
    }

    bool TopCollide(Brick br)
    {
        return Math.Sin(Theta) > 0 && InGame.Collide((int)PositionX, (int)PositionY + Size +- 1, Size, 1, br.PositionX, br.PositionY, br.Width, 1);
    }

    bool BottomCollide(Brick br)
    {
        return Math.Sin(Theta) < 0 && InGame.Collide((int)PositionX, (int)PositionY, Size, 1, br.PositionX, br.PositionY + br.Height +- 1, br.Width, 1);
    }

    void BallOut(InGame world)
    {
        if (BallsLeft > 0)
            //Number of balls avaliable is lessened
            BallsLeft--;
        else
            Dead = true;
            //Game Over!

        //Remove a ball from the list
        world.OutOfBounds.AddLast(this);
    }
}
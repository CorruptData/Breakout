﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Title
{
    //Title Screen's texture.
    Texture2D TitleSreen;

    public Title()
    {
        //Texture loaded in for the Title screen.
        TitleSreen = Utils.TextureLoader(@"title.png");
    }

    public void Draw(AD2SpriteBatch sb)
    {
        //Draw the title screen.
        sb.DrawTexture(TitleSreen, 0, 0, Breakout.BaseWidth, Breakout.BaseHeight);

        Utils.DefaultFont.Draw(sb, "PRESS ENTER TO BEGIN", 40, 140, Color.White, 2, true);
    }

    public Breakout.State Update(KeyboardState ks)
    {
        //Press Enter to go on to the game.
        if (ks.IsKeyDown(Keys.Enter))
        {
            if (!Breakout.EnterIsHeld)
            {
                Breakout.EnterIsHeld = true;
                return Breakout.State.InGame;
            }
        }
        else
            Breakout.EnterIsHeld = false;

        //If enter is not pressed, stay on the title screen.
        return Breakout.State.Title;
    }
}

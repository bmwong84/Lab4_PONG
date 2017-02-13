using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;    // for Vector2
using Microsoft.Xna.Framework.Graphics;    // for Texture2d

namespace IWKS_3400_Lab3
{
    class clsSprite
    {
        // Public types for class clsSprite
        public Texture2D texture { get; set; }    // sprite texture, read-only property
        public Vector2 position;    // sprite postion on the screen
        public Vector2 size { get; set; }  // sprite size in pixels
        public Vector2 velocity { get; set; } /* sprites moving velocity */
        public Vector2 screenSize { get; set; } /* screenSize for collision detection */
        public Vector2 center { get { return position + (size/2); } }    // sprite center
        public float halfSizeY { get { return size.Y / 2; } }
        public float radius { get { return size.X / 2; } }    // sprite radius
        public float cpuChaseSpeed = 5;

        // Constructor
        public clsSprite(Texture2D newTexture, Vector2 newPosition, Vector2 newSize, int screenWidth, int screenHeight)
        {
            texture = newTexture;
            position = newPosition;
            size = newSize;
            screenSize = new Vector2(screenWidth, screenHeight); /* screenSize is 2 dimensional */
        }

        // Class Methods

        //pre: clsSprite public types have been initialized.
        //post: Draw the sprite with spriteBatch Function. Returns void
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);    /* Creating sprite. White inidicates no tint */
        }

        //pre: Game initialized and non player sprite is in motion
        //post: Boundaries for screen are set. Function returns true only when a wall collision is detected
        public bool Move()
        {
            // if we'll move out of the screen, invert velocity checking right boundary
            if (position.X + size.X + velocity.X > screenSize.X)
            {
                velocity = new Vector2(-velocity.X, velocity.Y);
                return true;
            }

            // checking the bottom boundary
            if (position.Y + size.Y + velocity.Y > screenSize.Y)
            {
                velocity = new Vector2(velocity.X, -velocity.Y);
                return true;
            }

            // checking left boundary
            if( position.X + velocity.X < 0)
            {
                velocity = new Vector2(-velocity.X, velocity.Y);
                return true;
            }

            
            // checking top boundary
            if (position.Y + velocity.Y < 0)
            {
                velocity = new Vector2(velocity.X, -velocity.Y);
                return true;
            }

            // update the adjusted veloctiy
            position += velocity;
            return false;
        }

        //pre: none
        //post: resets the postion of the sprite to current postion, if sprite is directed to move off of screen by user.
        public void PlayerMoveRestrition()
        {
            // if we'll move out of the screen, invert velocity checking right boundary
            if (position.X + size.X + velocity.X >= screenSize.X)
            {
                position = new Vector2((screenSize.X - size.X), position.Y);    // Have to subtract size.X otherwise sprite would be placed on the outside of the border 
            }

            // checking the bottom boudary
            if (position.Y + size.Y + velocity.Y >= screenSize.Y)
            {
                position = new Vector2(position.X, (screenSize.Y - size.Y));    // Same as above. Calculating for size of sprite (removing it) allows it to spawn inside screen
            }

            // checking left boundary
            if (position.X + velocity.X <= 0)
            {
                position = new Vector2(0, position.Y); ;
            }

            // checking top boundary
            if (position.Y + velocity.Y <= 0)
            {
                position = new Vector2(position.X, 0);
            }
        }

        //pre: clsSprite object(s) initialized and in use
        //post: AI for npc sprite. Chase down the otherSprite
        public void ChaseBallSprite(clsSprite otherSprite)
        {
            if (this.position.Y < otherSprite.position.Y)
            {
                velocity = new Vector2(0, cpuChaseSpeed);
            }
            if(this.position.Y > otherSprite.position.Y)
            {
                velocity = new Vector2(0, -cpuChaseSpeed);
            }
            position += velocity;
        }

        //pre: clsSprite object initialized and in use 
        //post: returns true if a collision is detected
        public bool Collides(clsSprite otherSprite)
        {
            return (this.position.X + this.size.X >= otherSprite.position.X &&
                    this.position.X <= otherSprite.position.X + otherSprite.size.X &&
                    this.position.Y + this.size.Y >= otherSprite.position.Y &&
                    this.position.Y <= otherSprite.position.Y + otherSprite.size.Y);    /* These lines of code denote a collision detection using a cartesian coord. system. returns true if detected */
        }

        //pre: clsSprite object(s) initialized and in use
        //post: returns true if circular sprite collision is detected
        public bool CircleCollides(clsSprite otherSprite)
        {
            // Check if two circle sprites collided
            return (Vector2.Distance(this.center, otherSprite.center) <= this.radius + otherSprite.radius);
        }

    }
}


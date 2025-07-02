using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace JungleMonkey
{
    public class Monkey : ICharacter
    {
        private PictureBox monkeySprite;
        private Image spriteSheet;
        private int currentFrame = 0;
        private int totalFrames = 8;
        private Timer animationTimer;

        private int jumpSpeed = 0;
        private int force = 20;
        private int gravity = 2;
        private bool isJump = false;
        private bool isOnGround = true;
        private int groundTop;

        private int frameWidth = 32;
        private int frameHeight = 32;

        public Monkey(Image spriteSheet, int groundTop)
        {
            this.groundTop = groundTop;
            this.spriteSheet = spriteSheet;

            monkeySprite = new PictureBox
            {
                Size = new Size(56, 56),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Left = 100,
                Top = groundTop - 64 + 1,
                BackColor = Color.Transparent
            };

            StartAnimation();
        }

        public PictureBox Sprite => monkeySprite;
        public bool IsOnGround => isOnGround;

        public void Jump()
        {
            if (isOnGround)
            {
                isJump = true;
                isOnGround = false;
                jumpSpeed = force;
            }
        }

        public void ApplyGravity()
        {
            if (isJump)
            {
                monkeySprite.Top -= jumpSpeed;
                jumpSpeed -= gravity;

                if (jumpSpeed <= -force)
                {
                    isJump = false;
                    isOnGround = true;
                    jumpSpeed = 0;
                }
            }
            else
            {
                monkeySprite.Top = groundTop - monkeySprite.Height + 1;
            }
        }

        private void StartAnimation()
        {
            animationTimer = new Timer();
            animationTimer.Interval = 100;
            animationTimer.Tick += (s, e) => Animate();
            animationTimer.Start();
        }

        private void Animate()
        {
            int col = currentFrame % 3;
            int row = currentFrame / 3;

            Rectangle srcRect = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);
            Bitmap frame = new Bitmap(frameWidth, frameHeight);

            using (Graphics g = Graphics.FromImage(frame))
            {
                g.DrawImage(spriteSheet, new Rectangle(0, 0, frameWidth, frameHeight), srcRect, GraphicsUnit.Pixel);
            }

            frame.MakeTransparent();

            monkeySprite.Image = frame;

            currentFrame = (currentFrame + 1) % totalFrames;
        }

        public void Update() { }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Media;
using Timer = System.Windows.Forms.Timer;

namespace JungleMonkey
{
    public class Games : Form
    {
        private Timer timer;
        private Image bgImage, tileImage, monkeyImage, bananaImage, obstacleImage;
        private GroundManager groundManager;
        private ICharacter monkey;
        private Label labelScore;
        private SoundPlayer collisionSound;

        private List<IGameObject> obstacles = new List<IGameObject>();
        private List<IGameObject> bananas = new List<IGameObject>();
        private Random rand = new Random();
        private int score = 0;
        private int cooldownObstacle = 0;
        private int cooldownBanana = 0;

        public Games()
        {
            this.ClientSize = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            SoundPlay.Play("C:\\FP_PBO_FIX_BANGET\\JungleMonkey_Game\\Kvarmez - Jungle Journey (freetouse.com).mp3");
            collisionSound = new SoundPlayer("C:\\FP_PBO_FIX_BANGET\\JungleMonkey_Game\\Coin01.wav");

            bgImage = Image.FromFile("C:\\FP_PBO_FIX_BANGET\\JungleMonkey_Game\\Previewx3.png");
            tileImage = Image.FromFile("C:\\FP_PBO_FIX_BANGET\\JungleMonkey_Game\\tiles.png");
            monkeyImage = Image.FromFile("C:\\FP_PBO_FIX_BANGET\\JungleMonkey_Game\\Run2.png");
            bananaImage = Image.FromFile("C:\\FP_PBO_FIX_BANGET\\JungleMonkey_Game\\l0_sprite_07.png");
            obstacleImage = Image.FromFile("C:\\FP_PBO_FIX_BANGET\\JungleMonkey_Game\\Spike.png");

            this.BackgroundImage = bgImage;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            groundManager = new GroundManager(tileImage, this);
            int groundTop = groundManager.GroundTop;

            monkey = new Monkey(monkeyImage, groundTop);
            this.Controls.Add(monkey.Sprite);

            labelScore = new Label
            {
                Text = "Score: 0",
                Font = new Font("Times New Roman", 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(10, 10)
            };
            this.Controls.Add(labelScore);
            labelScore.BringToFront();

            timer = new Timer();
            timer.Interval = 20;
            timer.Tick += GameLoop;
            timer.Start();
        }

        private void GameLoop(object sender, EventArgs e)
        {
            groundManager.MoveGround();
            monkey.ApplyGravity();

            cooldownObstacle--;
            if (cooldownObstacle <= 0)
            {
                var obs = new Obstacle(obstacleImage, groundManager.GroundTop, this.ClientSize.Width);
                this.Controls.Add(obs.Sprite);
                obstacles.Add(obs);
                cooldownObstacle = rand.Next(40, 80);
            }

            cooldownBanana--;
            if (cooldownBanana <= 0)
            {
                var bana = new Banana(bananaImage, groundManager.GroundTop, this.ClientSize.Width);
                this.Controls.Add(bana.Sprite);
                bana.Sprite.BringToFront();
                bananas.Add(bana);
                cooldownBanana = rand.Next(60, 100);
            }

            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                obstacles[i].Move();
                var sprite = ((Obstacle)obstacles[i]).Sprite;

                if (sprite.Right < 0)
                {
                    this.Controls.Remove(sprite);
                    obstacles.RemoveAt(i);
                }
                else if (obstacles[i].CheckCollision(monkey.Sprite))
                {
                    GameOver();
                    return;
                }
            }

            for (int i = bananas.Count - 1; i >= 0; i--)
            {
                bananas[i].Move();
                var sprite = ((Banana)bananas[i]).Sprite;

                if (sprite.Right < 0)
                {
                    this.Controls.Remove(sprite);
                    bananas.RemoveAt(i);
                }
                else if (bananas[i].CheckCollision(monkey.Sprite))
                {
                    score++;
                    labelScore.Text = "Score: " + score.ToString();
                    collisionSound.Play();
                    this.Controls.Remove(sprite);
                    bananas.RemoveAt(i);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space && monkey.IsOnGround)
            {
                monkey.Jump();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void GameOver()
        {
            timer.Stop();
            SoundPlay.Stop();
            GameOver over = new GameOver(score);
            over.Show();
            this.Hide();
        }
    }
}
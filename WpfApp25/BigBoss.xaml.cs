﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp25
{
    /// <summary>
    /// Логика взаимодействия для BigBoss.xaml
    /// </summary>
    public partial class BigBoss : Window
    {
        bool goLeft, goRight;
        bool goLeftFrnd, goRightFrnd;
        List<Rectangle> itemsToRemove = new List<Rectangle>();
        int bulletTimer = 0;
        int bulletTimerLimit = 90;
        int bossSpeed;
        int totalBosses = 1;
        int totalShields = 2;
        int bossHealth = 2000;
        int shieldHealth = 200;
        bool gameOver = false;
        int shieldcount = 1;
        int side;
        Rect hitbox;
        DispatcherTimer gameTimer = new DispatcherTimer();
        DispatcherTimer bossTimer = new DispatcherTimer();
        ImageBrush myCanvasSkin = new ImageBrush();
        ImageBrush playerSkin = new ImageBrush();
        ImageBrush friendSkin = new ImageBrush();
        bool canRemove = false;
        bool canIntersect = true;
        Rectangle newShield;
        public BigBoss()
        {
            InitializeComponent();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(30);
            gameTimer.Start();
            Random random = new Random();
            bossTimer.Tick += BossTimer_Tick;
            bossTimer.Interval = TimeSpan.FromMilliseconds(random.Next(500, 6000));
            bossTimer.Start();
            //gameTimer.Stop();
            playerSkin.ImageSource = new BitmapImage(new Uri("Images/MyShip_-3000.png", UriKind.Relative));
            friendSkin.ImageSource = new BitmapImage(new Uri("Images/friend.png", UriKind.Relative));
            myCanvasSkin.ImageSource = new BitmapImage(new Uri("Images/hell3.png", UriKind.Relative));
            player.Fill = playerSkin;
            friend.Fill = friendSkin;
            myCanvas.Background = myCanvasSkin;
            myCanvas.Focus();
            progres.Maximum = bossHealth;
            progres.Value = bossHealth;
            MakeBoss(1);
            progres.Maximum = bossHealth;
            progres.Value = bossHealth;
            newShield = MakeShield(2);
            bossSpeed = 9;
            side = 1;
        }

        private void BossTimer_Tick(object sender, EventArgs e)
        {
            //Random random = new Random();
            //int value = random.Next(1,2);

            if (side == 1)
            {
                side = -1;
                return;
            }
            else
            {
                side = 1;
            }

            bossSpeed = bossSpeed * side;

        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (newShield is Rectangle && (string)newShield.Tag == "shield")
            {
                itemsToRemove.Add(newShield);
                Rect shieldHitBox = new Rect(Canvas.GetLeft(newShield), Canvas.GetTop(newShield), newShield.Width, newShield.Height);
                hitbox = shieldHitBox;
            }
            Canvas.SetLeft(newShield, Canvas.GetLeft(newShield) + 6);
            if (Canvas.GetLeft(newShield) > 820)
            {
                Canvas.SetLeft(newShield, -80);

            }
            Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            Rect friendHitBox = new Rect(Canvas.GetLeft(friend), Canvas.GetTop(friend), friend.Width, friend.Height);
            liveBoss.Content = "Осталось всего здоровья: " + bossHealth;

            if (goLeft == true && Canvas.GetLeft(player) > 0)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - 10);
            }

            if (goLeftFrnd == true && Canvas.GetLeft(friend) > 0)
            {
                Canvas.SetLeft(friend, Canvas.GetLeft(friend) - 10);
            }

            if (goRightFrnd == true && Canvas.GetLeft(friend) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(friend, Canvas.GetLeft(friend) + 10);
            }

            if (goRight == true && Canvas.GetLeft(player) + 80 < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + 10);
            }
            bulletTimer -= 3;
            if (bulletTimer < 0)
            {
                BossBulletMaker(Canvas.GetLeft(player) + 20, 10);
                BossBulletMaker(Canvas.GetLeft(friend) + 20, 10);
                Random random = new Random();
                BossBulletMaker(random.Next(100, 500), random.Next(3, 10));
                BossBulletMaker(random.Next(200, 600), random.Next(3, 10));
                BossBulletMaker(random.Next(250, 700), random.Next(3, 10));
                BossBulletMaker(random.Next(300, 800), random.Next(3, 10));
                bulletTimer = bulletTimerLimit;
            }
            foreach (var x in myCanvas.Children.OfType<Rectangle>())
            {
                if (x is Rectangle && (string)x.Tag == "bullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }
                    Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "boss")
                        {
                            Rect bossHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (bullet.IntersectsWith(bossHit))
                            {
                                if (bossHealth < 1)
                                {
                                    itemsToRemove.Add(y);
                                }
                                itemsToRemove.Add(x);
                                bossHealth -= 10;
                                progres.Value = bossHealth;
                            }
                        }
                    }
                }
                if (x is Rectangle && (string)x.Tag == "bulletFrnd")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) - 20);

                    if (Canvas.GetTop(x) < 10)
                    {
                        itemsToRemove.Add(x);
                    }
                    Rect friendbullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    foreach (var y in myCanvas.Children.OfType<Rectangle>())
                    {
                        if (y is Rectangle && (string)y.Tag == "boss")
                        {
                            Rect bossHit = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                            if (friendbullet.IntersectsWith(bossHit))
                            {
                                if (bossHealth < 1)
                                {
                                    itemsToRemove.Add(y);
                                }
                                itemsToRemove.Add(x);
                                bossHealth -= 10;
                                progres.Value = bossHealth;
                            }
                        }
                    }
                }
                if (x is Rectangle && (string)x.Tag == "boss")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + bossSpeed);
                    if (Canvas.GetLeft(x) > 820) // условие перемещения шлеппы
                    {
                        Canvas.SetLeft(x, -80);
                    }
                    if (Canvas.GetLeft(x) < -100) // условие перемещения шлеппы
                    {
                        Canvas.SetLeft(x, 700);
                    }    
                }
                if (x is Rectangle && (string)x.Tag == "bossBullet")
                {
                    Canvas.SetTop(x, Canvas.GetTop(x) + 10); //скорость пули босса
                    if (Canvas.GetTop(x) > 480)
                    {
                        itemsToRemove.Add(x);
                    }
                    Rect bossBulletHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (playerHitBox.IntersectsWith(bossBulletHitBox) || friendHitBox.IntersectsWith(bossBulletHitBox))
                    {
                        ShowGameOver("Босс испепелил вас!!");
                    }

                    if (hitbox.IntersectsWith(bossBulletHitBox) && canIntersect)
                    {
                        itemsToRemove.Add(x);
                        //myCanvas.Children.Remove(x);
                        if (shieldHealth < 1)
                        {
                            itemsToRemove.Add(newShield);
                            canRemove = true;
                            canIntersect = false;
                        }
                        shieldHealth -= 5;
                    }
                }
            }

            foreach (Rectangle x in itemsToRemove)
            {
                if (x.Tag.ToString() != "shield")
                    myCanvas.Children.Remove(x);
                if (canRemove)
                    myCanvas.Children.Remove(x);
            }

            if (bossHealth < 1500)
            {
                bossSpeed = 11 * side;
                bulletTimer -= 3;
                if (bulletTimer < 0)
                {
                    BossBulletMaker(Canvas.GetLeft(player) + 20, 10);
                    BossBulletMaker(Canvas.GetLeft(friend) + 20, 10);
                    bulletTimer = bulletTimerLimit;
                }
            }
            if (bossHealth < 900)
            {
                bossSpeed = 12 * side;
                bulletTimer -= 3;
                if (bulletTimer < 0)
                {
                    Random random = new Random();
                    BossBulletMaker(random.Next(0, 300), random.Next(3, 10));
                    BossBulletMaker(random.Next(500, 740), random.Next(3, 10));
                    bulletTimer = bulletTimerLimit;
                }
            }
            if (bossHealth < 600)
            {
                bossSpeed = 13 * side;
                bulletTimer -= 3;
                if (bulletTimer < 0)
                {
                    Random random = new Random();
                    BossBulletMaker(random.Next(0, 130), random.Next(3, 10));
                    BossBulletMaker(random.Next(0, 280), random.Next(3, 10));
                    BossBulletMaker(random.Next(0, 300), random.Next(3, 10));
                    BossBulletMaker(random.Next(400, 740), random.Next(3, 10));
                    bulletTimer = bulletTimerLimit;
                }
            }
            if (bossHealth < 300)
            {
                bulletTimer -= 3;
                if (bulletTimer < 0)
                {
                    Random random = new Random();
                    BossBulletMaker(random.Next(100, 400), random.Next(3, 10));
                    BossBulletMaker(random.Next(150, 450), random.Next(3, 10));
                    BossBulletMaker(random.Next(180, 480), random.Next(3, 10));
                    BossBulletMaker(random.Next(250, 500), random.Next(3, 10));
                    BossBulletMaker(random.Next(300, 530), random.Next(3, 10));
                    BossBulletMaker(random.Next(320, 540), random.Next(3, 10));
                    BossBulletMaker(random.Next(500, 700), random.Next(3, 10));
                    bulletTimer = bulletTimerLimit;
                }
            }
            if (bossHealth < 1)
            {
                ShowGameOver("Вы прошли игру");
                MessageBox.Show("Вы прошли игру (Не один большой Шлеппа русский кот не пострадал)");
            }

            foreach (Rectangle i in itemsToRemove) //удаление Большого Шлеппы
            {
                if (bossHealth < 1)
                {
                    myCanvas.Children.Remove(i);
                }
            }
        }
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }

            if (e.Key == Key.Right)
            {
                goRight = true;
            }
            if (e.Key == Key.A)
            {
                goLeftFrnd = true;
            }

            if (e.Key == Key.D)
            {
                goRightFrnd = true;
            }
        }
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }

            if (e.Key == Key.A)
            {
                goLeftFrnd = false;
            }

            if (e.Key == Key.D)
            {
                goRightFrnd = false;
            }

            if (e.Key == Key.Right)
            {
                goRight = false;
            }
            if (e.Key == Key.Space)
            {
                ImageBrush bulletSkin = new ImageBrush();
                ImageBrush bulletSkinfrnd = new ImageBrush();
                Rectangle newBullet = new Rectangle { Tag = "bullet", Height = 50, Width = 40, Fill = bulletSkin };
                Rectangle newBullet2 = new Rectangle { Tag = "bullet", Height = 50, Width = 40, Fill = bulletSkin };
                Canvas.SetTop(newBullet2, Canvas.GetTop(player) - newBullet2.Height);
                Canvas.SetLeft(newBullet2, Canvas.GetLeft(player) + player.Width / 2 + 10);
                Canvas.SetTop(newBullet, Canvas.GetTop(player) - newBullet.Height);
                Canvas.SetLeft(newBullet, Canvas.GetLeft(player) + player.Width / 2 - 10);
                myCanvas.Children.Add(newBullet);
                myCanvas.Children.Add(newBullet2);
                bulletSkin.ImageSource = new BitmapImage(new Uri("Images/Bullet.png", UriKind.Relative));
            }
            if (e.Key == Key.LeftCtrl)
            {
                ImageBrush bulletSkinfrnd = new ImageBrush();
                Rectangle newBulletFrnd = new Rectangle { Tag = "bulletFrnd", Height = 50, Width = 40, Fill = bulletSkinfrnd };
                Rectangle newBulletFrnd1 = new Rectangle { Tag = "bulletFrnd", Height = 50, Width = 40, Fill = bulletSkinfrnd };
                Canvas.SetTop(newBulletFrnd1, Canvas.GetTop(friend) - newBulletFrnd1.Height);
                Canvas.SetLeft(newBulletFrnd1, Canvas.GetLeft(friend) + friend.Width / 2 + 10);
                Canvas.SetTop(newBulletFrnd, Canvas.GetTop(friend) - newBulletFrnd.Height);
                Canvas.SetLeft(newBulletFrnd, Canvas.GetLeft(friend) + friend.Width / 2 - 10);
                myCanvas.Children.Add(newBulletFrnd);
                myCanvas.Children.Add(newBulletFrnd1);
                bulletSkinfrnd.ImageSource = new BitmapImage(new Uri("Images/Bullet.png", UriKind.Relative));
            }

            if (e.Key == Key.Enter && gameOver == true)
            {
                Menu menu = new Menu();
                menu.Show();
                Close();
            }
        }
        private void BossBulletMaker(double x, double y)
        {
            ImageBrush newBossBullet = new ImageBrush();
            Rectangle bossBullet = new Rectangle { Tag = "bossBullet", Height = 50, Width = 25, Fill = newBossBullet, StrokeThickness = 5, };
            Canvas.SetTop(bossBullet, y);
            Canvas.SetLeft(bossBullet, x);
            myCanvas.Children.Add(bossBullet);
            newBossBullet.ImageSource = new BitmapImage(new Uri("Images/BossBullet2.png", UriKind.Relative));
        }

        private void MakeBoss(int limit)
        {
            int left = 0;
            totalBosses = limit;
            for (int i = 0; i < limit; i++)
            {
                ImageBrush bossSkin = new ImageBrush();
                Rectangle newBoss = new Rectangle { Tag = "boss", Height = 100, Width = 200, Fill = bossSkin };
                Canvas.SetTop(newBoss, 50);
                Canvas.SetLeft(newBoss, left);
                myCanvas.Children.Add(newBoss);
                left -= 1;
                bossSkin.ImageSource = new BitmapImage(new Uri("Images/BossDemon.png", UriKind.Relative));
            }
        }
        public Rectangle MakeShield(int limitshield)
        {

            if (shieldcount < limitshield)
            {
                Random random = new Random();
                double height = random.Next(200, 300);
                double width = random.Next(0, 700);
                Rectangle newShield = new Rectangle { Tag = "shield", Height = 30, Width = 100, Fill = Brushes.Red, Stroke = Brushes.Black };
                Canvas.SetTop(newShield, height);
                Canvas.SetLeft(newShield, width);
                myCanvas.Children.Add(newShield);
                shieldcount++;
                return newShield;
            }
            Rectangle rectangle = new Rectangle { Tag = "shield1", Height = 0, Width = 0, Fill = Brushes.Transparent, Stroke = Brushes.Transparent };
            Canvas.SetTop(rectangle, 0);
            Canvas.SetLeft(rectangle, 0);
            myCanvas.Children.Add(rectangle);
            return rectangle;
        }
        private void ShowGameOver(string message)
        {
            gameOver = true;
            gameTimer.Stop();
            liveBoss.Content = " " + message + " Нажмите Enter чтобы снова играть";
        }
    }
}


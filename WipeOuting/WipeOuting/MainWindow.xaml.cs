using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Microsoft.Kinect;
using System.Windows.Input;
using Box2DX.Common;
using System.Windows.Media;
using Microsoft.Speech.Recognition;
using System.Media;

/* 結合作業時メモ
 * 
 *
 * 07/10につくったもの
 * ・Goaled()部
 * ・それを呼び出すためのHandleGameTimer()内if文
 * ・
 * 
 * ※KinectSensorの変数を残してあるためそのあたりの扱いを慎重にすること
 * ※描画処理部の変更について細心の注意を払うこと
 */

namespace WipeOuting
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        World world;
        Parts parts;
        
        #region サンプルコード部
        private const int TimerResolution = 2;  // ms
        private const int NumIntraFrames = 3;
        private const int MaxShapes = 80;
        private const double MaxFramerate = 70;
        private const double MinFramerate = 15;
        private const double MinShapeSize = 12;
        private const double MaxShapeSize = 90;
        private const double DefaultDropRate = 2.5;
        private const double DefaultDropSize = 32.0;
        private const double DefaultDropGravity = 1.0;

        private DateTime lastFrameDrawn = DateTime.MinValue;
        private DateTime predNextFrame = DateTime.MinValue;
        private double actualFrameTime;

        private double targetFramerate = MaxFramerate;
        private int frameCount;
        private bool runningGameThread;
        #endregion

        MyKinect kinect;
        PlayerSeesaw playerSeesaw;
        Image img;

        public MainWindow()
        {
            InitializeComponent();
            parts = new Parts();
            //startBgm();
            kinect = new MyKinect(this.SpeechRecognized);
            img = new Image();
            img.Source = this.kinect.imageSource;
            initelement();

            playerSeesaw = new PlayerSeesaw();

            playfield.Children.Add(img);
        }
        #region サンプルコード部
        [DllImport("Winmm.dll", EntryPoint = "timeBeginPeriod")]
        private static extern int TimeBeginPeriod(uint period);

        #endregion

        private void initelement()
        {
            UIElement[] elements = parts.elementSend();
            foreach(UIElement el in elements){
                playfield.Children.Add(el);
            }

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            #region サンプルコード部
            this.runningGameThread = false;
            Properties.Settings.Default.Save();
            #endregion
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region サンプルコード部
            playfield.ClipToBounds = true;

            TimeBeginPeriod(TimerResolution);
            var myGameThread = new Thread(this.GameThread);
            myGameThread.SetApartmentState(ApartmentState.STA);
            myGameThread.Start();
            #endregion
        }

        #region タイマー・スレッド
        private void GameThread()
        {
            #region サンプルコード部
            this.runningGameThread = true;
            this.predNextFrame = DateTime.Now;
            this.actualFrameTime = 1000.0 / this.targetFramerate;
            while (this.runningGameThread)
            {
                DateTime now = DateTime.Now;
                if (this.lastFrameDrawn == DateTime.MinValue)
                {
                    this.lastFrameDrawn = now;
                }

                double ms = now.Subtract(this.lastFrameDrawn).TotalMilliseconds;
                this.actualFrameTime = (this.actualFrameTime * 0.95) + (0.05 * ms);
                this.lastFrameDrawn = now;

                this.frameCount++;
                if ((this.frameCount % 100 == 0) && (1000.0 / this.actualFrameTime < this.targetFramerate * 0.92))
                {
                    this.targetFramerate = System.Math.Max(MinFramerate, (this.targetFramerate + (1000.0 / this.actualFrameTime)) / 2);
                }

                if (now > this.predNextFrame)
                {
                    this.predNextFrame = now;
                }
                else
                {
                    double milliseconds = this.predNextFrame.Subtract(now).TotalMilliseconds;
                    if (milliseconds >= TimerResolution)
                    {
                        Thread.Sleep((int)(milliseconds + 0.5));
                    }
                }

                this.predNextFrame += TimeSpan.FromMilliseconds(1000.0 / this.targetFramerate);

                this.Dispatcher.Invoke(DispatcherPriority.Send, new Action<int>(this.HandleGameTimer), 0);

            }
            #endregion
        }

        private void HandleGameTimer(int param)
        {
            parts.renewGallPos();//ゴールかごの位置更新
            if (parts.checkGoal()==true)
            {
                Goaled();
            }
            

            Canvas.SetTop(img, this.Height - 300);
            Canvas.SetLeft(img, this.Width / 2 - 320);
            Canvas.SetRight(img, this.Width / 2 + 320);
            Canvas.SetBottom(img, this.Height);
            
            draw();//描画
        }
        #endregion
        #region 描画部
        private void draw()
        {
            //フレームを進める
            world = parts.World;
            float timestep = 1f / 60f;
            world.Step(timestep, 8, 1);

            playerSeesaw.Step(kinect.pos, timestep, 8, 1);
            parts.setAngle(playerSeesaw.Angle());

            //構成部品を取得し描画
            Body drawparts = world.GetBodyList();
            while (drawparts != null)
            {
                drawtoCanvas(drawparts);
                drawparts=drawparts.GetNext();
            }
        }

        private void drawtoCanvas(Body b)
        {
            Box2DX.Collision.Shape sh = b.GetShapeList();
            int scount = sCount(sh);
            sh = b.GetShapeList();//カウントしたらもう一回戻す(たぶんこれ連結リストでできてる感じがする…)
            //下に下がるバーだったら(構成してるシェイプ数が3つ)
            if (scount == 3)
            {
                Vec2 sPos = avePosition((PolygonShape)sh);
                Canvas.SetTop((Image)b.GetUserData(), (b.GetPosition().Y - 0.7) * Parts.Zoom);//計算上は-0.4が正しくなるはずなんだけど…
                Canvas.SetLeft((Image)b.GetUserData(), (b.GetPosition().X - 7 + sPos.X) * Parts.Zoom);
                return;
            }
            //ギザギザバーだったら(構成しているシェイプ数が7つ)
            if (scount == 7)
            {
                Vec2 sPos = avePosition((PolygonShape)sh);
                Canvas.SetTop((Image)b.GetUserData(), (b.GetPosition().Y - 0.5) * Parts.Zoom);//計算上は-0.35が正しくなるはずなんだけど…
                Canvas.SetLeft((Image)b.GetUserData(), (b.GetPosition().X - 7 + sPos.X) * Parts.Zoom);
                return;
            }
            //ゴールだったら(構成しているシェイプ数が4つ)
            if (scount == 4)
            {
                Vec2 sPos = avePosition((PolygonShape)sh);
                Canvas.SetTop((Image)b.GetUserData(), (b.GetPosition().Y - 0.5) * Parts.Zoom);
                Canvas.SetLeft((Image)b.GetUserData(), (b.GetPosition().X - 0.5) * Parts.Zoom);
                return;
            }
            if (sh != null)
            {
                switch (sh.GetType())
                {
                    case ShapeType.PolygonShape:
                        Vec2 sPos = avePosition((PolygonShape)sh);
                        Canvas.SetTop((Image)b.GetUserData(), (b.GetPosition().Y - 0.2 + sPos.Y) * Parts.Zoom);
                        Canvas.SetLeft((Image)b.GetUserData(), (b.GetPosition().X - 7 + sPos.X) * Parts.Zoom);
                        break;
                    case ShapeType.CircleShape:
                        Canvas.SetTop((Image)b.GetUserData(), (b.GetPosition().Y - 0.4) * Parts.Zoom);
                        Canvas.SetLeft((Image)b.GetUserData(), (b.GetPosition().X - 0.4) * Parts.Zoom);
                        break;
                    default:
                        break;
                }
            }

        }

        //シェイプ数を調べる
        private int sCount(Box2DX.Collision.Shape s)
        {
            int i = 0;
            while (s != null)
            {
                i++;
                s = s.GetNext();
            }
            return i;
        }
        //各頂点の位置をもとに図形の中心を割り出す
        private Vec2 avePosition(PolygonShape ps)
        {
            float x=0,y=0;
            Vec2[] Pos = ps.GetCoreVertices();
            int count = ps.VertexCount;
            if(count>0){
                for(int i=0;i<count;i++){
                    x+=Pos[i].X;
                    y+=Pos[i].Y;
                }
                x/=count;
                y/=count;
            }
            return new Vec2(x,y);
        }
        //クリアしたらCLEAR！　と表示
        private void Goaled()
        {
            TextBlock txblc = new TextBlock();
            txblc.Width = 300;
            txblc.FontSize = 60;
            txblc.Foreground = Brushes.Red;
            txblc.Text = "CLEAR!!";
            Canvas.SetTop(txblc, 240);
            Canvas.SetLeft(txblc, 300);
            playfield.Children.Add(txblc);
        }
        #endregion

        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            const double ConfidenceThreshold = 0.3;

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                String command = e.Result.Semantics.Value.ToString();
                switch (command)
                {
                    case "start":
                        this.parts.startBall();
                        break;
                }
            }
        }

        private void startBgm()
        {
            string buf = System.IO.Directory.GetCurrentDirectory();
            string ui = buf.Replace("/", @"\") + @"\..\..\Sounds\bgm.wav";
            SoundPlayer sound = new SoundPlayer(ui);
            sound.PlayLooping();
        }

    }

}

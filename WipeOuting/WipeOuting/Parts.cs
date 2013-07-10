using System.Windows.Media;
using System.Windows.Shapes;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Media;

namespace WipeOuting
{
    class Parts
    {
        
        private World world;
        public World World{
            get { return world; }
        }
        const float DRParse = (float)System.Math.PI/180;
        UIElement[] elements =new UIElement[8];

        public const int Zoom = 40;
        Body floor1;
        Body floor2;
        Body floor3;
        Body floor4;
        Body floor5;
        Body unfloor;
        Body ball;
        Body gall;
        Image Flr1;
        Image Flr2;
        Image Flr3;
        Image Flr4;
        Image Flr5;
        Image ell;
        Image Kago;
        Image ufimg;
        Box2DX.Collision.Shape glshape,ufshape,blshape;
        Vec2 Recsize = new Vec2(14f,0.4f);

        float Radius = 0.4f;
        private float angle;
        private int pm;//かごの動き制御
        private int recount;
        private bool isGoal=false;
        private bool isGameover = false;
        public Parts()
        {
            angle = 0;
            recount=0;
            pm = -1;
            initShapes();
            initB2d();
        }

        #region 物体の初期化
        private void initB2d()
        {
            //有効範囲の設定
            AABB aabb = new AABB();
            aabb.LowerBound.Set(-100.0f, -100.0f);
            aabb.UpperBound.Set(100.0f, 100.0f);
            //Box2dの初期化
            world = new World(aabb, new Vec2(0f, 9.8f), false);

            makefloors();
            makegoal();

            //ボールの初期設定
            BodyDef bldf = new BodyDef();
            bldf.Position.Set(17, 0);
            CircleDef blci = new CircleDef();
            blci.Radius = this.Radius;
            blci.Density = 4;
            blci.Friction = 0.4f;
            //ボールのセット
            ball = world.CreateBody(bldf);
            blshape=ball.CreateShape(blci);
            ball.SetUserData(ell);
            world.SetContactListener(new GoalListener(this));
 
        }

        private void makefloors()
        {
            //床の初期設定
            BodyDef flrdf1 = new BodyDef();
            flrdf1.Position.Set(12, 3);
            flrdf1.Angle = angle * DRParse;

            BodyDef flrdf2 = new BodyDef();
            flrdf2.Position.Set(12, 4.5f);
            flrdf2.Angle = angle * DRParse;

            BodyDef flrdf3 = new BodyDef();
            flrdf3.Position.Set(12, 6.3f);
            flrdf3.Angle = angle * DRParse;

            BodyDef flrdf4 = new BodyDef();
            flrdf4.Position.Set(12, 8.2f);
            flrdf4.Angle = angle * DRParse;

            BodyDef flrdf5 = new BodyDef();
            flrdf5.Position.Set(12, 9.7f);
            flrdf5.Angle = angle * DRParse;

            //床のセット(床1)
            floor1 = world.CreateBody(flrdf1);

            PolygonDef flrpl1 = new PolygonDef();
            flrpl1.SetAsBox(Recsize.X / 2, Recsize.Y / 2);
            flrpl1.Density = 2;
            flrpl1.Friction = 0.4f;
            floor1.CreateShape(flrpl1);
            floor1.SetUserData(Flr1);
            //床のセット(床2)
            floor2 = world.CreateBody(flrdf2);

            PolygonDef flrpl2 = new PolygonDef();
            flrpl2.SetAsBox(Recsize.X / 2, Recsize.Y / 2, new Vec2(-3, 0), 0);
            flrpl2.Density = 2;
            flrpl2.Friction = 0.4f;
            floor2.CreateShape(flrpl2);
            floor2.SetUserData(Flr2);
            //床のセット(床3)
            floor3 = world.CreateBody(flrdf3);

            PolygonDef flrpl3_1 = new PolygonDef();
            flrpl3_1.SetAsBox(Recsize.X / 2, Recsize.Y / 2,new Vec2(0,0.15f),0);
            flrpl3_1.Density = 2;
            flrpl3_1.Friction = 0.4f;
            PolygonDef flrpl3_2= new PolygonDef();
            flrpl3_2.VertexCount=3;
            flrpl3_2.Vertices[0].Set(-7,-0.05f);
            flrpl3_2.Vertices[1].Set(-7f, -0.35f);
            flrpl3_2.Vertices[2].Set(-5.6f,-0.05f);
            flrpl3_2.Density = 2;
            flrpl3_2.Friction = 0.4f;
            PolygonDef flrpl3_3 = new PolygonDef();
            flrpl3_3.VertexCount = 3;
            flrpl3_3.Vertices[0].Set(-5.6f, -0.05f);
            flrpl3_3.Vertices[1].Set(-4.2f, -0.35f);
            flrpl3_3.Vertices[2].Set(-2.8f, -0.05f);
            flrpl3_3.Density = 2;
            flrpl3_3.Friction = 0.4f;
            PolygonDef flrpl3_4 = new PolygonDef();
            flrpl3_4.VertexCount = 3;
            flrpl3_4.Vertices[0].Set(-2.8f, -0.05f);
            flrpl3_4.Vertices[1].Set(-1.4f, -0.35f);
            flrpl3_4.Vertices[2].Set(0f, -0.05f);
            flrpl3_4.Density = 2;
            flrpl3_4.Friction = 0.4f;
            PolygonDef flrpl3_5 = new PolygonDef();
            flrpl3_5.VertexCount = 3;
            flrpl3_5.Vertices[0].Set(0f, -0.05f);
            flrpl3_5.Vertices[1].Set(1.4f, -0.35f);
            flrpl3_5.Vertices[2].Set(2.8f, -0.05f);
            flrpl3_5.Density = 2;
            flrpl3_5.Friction = 0.4f;
            PolygonDef flrpl3_6 = new PolygonDef();
            flrpl3_6.VertexCount = 3;
            flrpl3_6.Vertices[0].Set(2.8f, -0.05f);
            flrpl3_6.Vertices[1].Set(4.2f, -0.35f);
            flrpl3_6.Vertices[2].Set(5.6f, -0.05f);
            flrpl3_6.Density = 2;
            flrpl3_6.Friction = 0.4f;
            PolygonDef flrpl3_7 = new PolygonDef();
            flrpl3_7.VertexCount = 3;
            flrpl3_7.Vertices[0].Set(5.6f, -0.05f);
            flrpl3_7.Vertices[1].Set(7f, -0.35f);
            flrpl3_7.Vertices[2].Set(7f, -0.05f);
            flrpl3_7.Density = 2;
            flrpl3_7.Friction = 0.4f;
            floor3.CreateShape(flrpl3_2);
            floor3.CreateShape(flrpl3_3);
            floor3.CreateShape(flrpl3_4);
            floor3.CreateShape(flrpl3_5);
            floor3.CreateShape(flrpl3_6);
            floor3.CreateShape(flrpl3_7);
            floor3.CreateShape(flrpl3_1);
            floor3.SetUserData(Flr3);
            //床のセット(床4)
            floor4 = world.CreateBody(flrdf4);

            PolygonDef flrpl4_1 = new PolygonDef();
            flrpl4_1.SetAsBox(Recsize.X / 2, Recsize.Y / 2, new Vec2(-3, 0.2f), 0);
            flrpl4_1.Density = 2;
            flrpl4_1.Friction = 0.4f;
            PolygonDef flrpl4_2 = new PolygonDef();
            flrpl4_2.SetAsBox(2,0.2f, new Vec2(-8, -0.2f), 0);
            flrpl4_2.Density = 2;
            flrpl4_2.Friction = 0.4f;
            PolygonDef flrpl4_3 = new PolygonDef();
            flrpl4_3.VertexCount = 3;
            flrpl4_3.Vertices[0].Set(-6, 0f);
            flrpl4_3.Vertices[1].Set(-6f, -0.4f);
            flrpl4_3.Vertices[2].Set(-2f, 0f);
            flrpl4_3.Density = 2;
            flrpl4_3.Friction = 0.4f;
            floor4.CreateShape(flrpl4_3);
            floor4.CreateShape(flrpl4_2);
            floor4.CreateShape(flrpl4_1);
            floor4.SetUserData(Flr4);
            //床のセット(床5)
            floor5 = world.CreateBody(flrdf5);

            PolygonDef flrpl5 = new PolygonDef();
            flrpl5.SetAsBox(Recsize.X / 2, Recsize.Y / 2);
            flrpl5.Density = 2;
            flrpl5.Friction = 0.4f;
            floor5.CreateShape(flrpl5);
            floor5.SetUserData(Flr5);

            //本来はあまりよくないけど、ゲームオーバー用にも画像を用意します…
            BodyDef handef = new BodyDef();
            handef.Position.Set(12, 14);
            unfloor=world.CreateBody(handef);
            PolygonDef hanpol = new PolygonDef();
            hanpol.SetAsBox(30, 0.5f);
            hanpol.IsSensor = true;
            ufshape=unfloor.CreateShape(hanpol);
            unfloor.SetUserData(ufimg);

        }

        //ゴール用かごのセット
        private void makegoal()
        {
            BodyDef gldef = new BodyDef();
            gldef.Position.Set(4.5f, 11.7f);
            PolygonDef glpl_1 = new PolygonDef();
            glpl_1.SetAsBox(0.6f, 0.05f, new Vec2(0, 0.5f), 0);
            glpl_1.Density = 4;
            glpl_1.Friction = 0.4f;
            PolygonDef glpl_2 = new PolygonDef();
            glpl_2.SetAsBox(0.05f, 0.5f, new Vec2(-0.55f, 0), 0);
            glpl_2.Density = 4;
            glpl_2.Friction = 0.4f;
            PolygonDef glpl_3 = new PolygonDef();
            glpl_3.SetAsBox(0.05f, 0.5f, new Vec2(0.55f, 0), 0);
            glpl_3.Density = 4;
            glpl_3.Friction = 0.4f;
            PolygonDef glpl_9 = new PolygonDef();
            glpl_9.IsSensor = true;
            glpl_9.SetAsBox(0.1f, 0.1f);
            gall = world.CreateBody(gldef);
            gall.CreateShape(glpl_1);
            gall.CreateShape(glpl_2);
            gall.CreateShape(glpl_3);
            glshape=gall.CreateShape(glpl_9);//衝突判定用にshapeをもつ
            gall.SetUserData(Kago);

        }

        private void initShapes()
        {
            System.Uri curd = new System.Uri(System.IO.Directory.GetCurrentDirectory());
            //Floor1用
            Flr1 = new Image();
            Flr1.Source = new BitmapImage(new System.Uri(curd,@"..\Images\barleft.png"));
            Flr1.Width = Recsize.X * Zoom;
            Flr1.Height = Recsize.Y * Zoom;
            Flr1.RenderTransform = new RotateTransform(angle, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);


            //Floor2用
            Flr2 = new Image();
            Flr2.Source = new BitmapImage(new System.Uri(curd, @"..\Images\barright.png"));
            Flr2.Width = Recsize.X * Zoom;
            Flr2.Height = Recsize.Y * Zoom;
            Flr2.RenderTransform = new RotateTransform(angle, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);

            //Floor2用
            Flr3 = new Image();
            Flr3.Source = new BitmapImage(new System.Uri(curd, @"..\Images\bartry.png"));
            Flr3.Width = Recsize.X * Zoom;
            Flr3.Height = (Recsize.Y+0.6) * Zoom;
            Flr3.RenderTransform = new RotateTransform(angle, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);

            //Floor2用
            Flr4 = new Image();
            Flr4.Source = new BitmapImage(new System.Uri(curd, @"..\Images\bardown.png"));
            Flr4.Width = Recsize.X * Zoom;
            Flr4.Height = (Recsize.Y+1) * Zoom;
            Flr4.RenderTransform = new RotateTransform(angle, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);

            //Floor2用
            Flr5 = new Image();
            Flr5.Source = new BitmapImage(new System.Uri(curd, @"..\Images\barleft.png"));
            Flr5.Width = Recsize.X * Zoom;
            Flr5.Height = Recsize.Y * Zoom;
            Flr5.RenderTransform = new RotateTransform(angle, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);

            //ボール用
            ell = new Image();
            ell.Source = new BitmapImage(new System.Uri(curd, @"..\Images\ball.png"));
            ell.Width = Radius*2* Zoom;
            ell.Height = Radius * 2 * Zoom;

            //かご用
            Kago = new Image();
            Kago.Source = new BitmapImage(new System.Uri(curd, @"..\Images\gall.png"));
            Kago.Width =  Zoom;
            Kago.Height = Zoom;

            ufimg = new Image();
            ufimg.Source = new BitmapImage(new System.Uri(curd, @"..\Images\dummy.png"));
            ufimg.Width = Zoom;
            ufimg.Height = Zoom;
        }
        #endregion
        //シェイプの受け渡し
        public UIElement[] elementSend()
        {
            elements[0] = Flr1;
            elements[1] = Flr2;
            elements[2] = Flr3;
            elements[3] = Flr4;
            elements[4] = Flr5;
            elements[5] = ell;
            elements[6] = Kago;
            elements[7] = ufimg;
            return elements;
        }
        //角度の更新
        public void renewAngle(float d)
        {
            angle = d;
            if (angle > 12)
            {
                angle = 12;
            }
            if (angle < -12)
            {
                angle = -12;
            }
            floor1.SetXForm(floor1.GetPosition(), angle * DRParse);
            floor2.SetXForm(floor2.GetPosition(), angle * DRParse);
            floor3.SetXForm(floor3.GetPosition(), angle * DRParse);
            floor4.SetXForm(floor4.GetPosition(), angle * DRParse);
            floor5.SetXForm(floor5.GetPosition(), angle * DRParse);

            Flr1.RenderTransform = new RotateTransform(angle, Recsize.X / 2 * Zoom, Recsize.Y/2 * Zoom);
            Flr2.RenderTransform = new RotateTransform(angle, (Recsize.X / 2 + 3) * Zoom, Recsize.Y / 2 * Zoom);
            Flr3.RenderTransform = new RotateTransform(angle, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);
            Flr4.RenderTransform = new RotateTransform(angle, (Recsize.X / 2 + 3) * Zoom, Recsize.Y / 2 * Zoom);
            Flr5.RenderTransform = new RotateTransform(angle, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);
            ell.RenderTransform = new RotateTransform(ball.GetAngle() * (1 / DRParse), Radius * Zoom, Radius * Zoom);

        }

        public void setAngle(float p)
        {
            floor1.SetXForm(floor1.GetPosition(), p);
            floor2.SetXForm(floor2.GetPosition(), p);
            floor3.SetXForm(floor3.GetPosition(), p);
            floor4.SetXForm(floor4.GetPosition(), p);
            floor5.SetXForm(floor5.GetPosition(), p);

            Flr1.RenderTransform = new RotateTransform(p / DRParse, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);
            Flr2.RenderTransform = new RotateTransform(p / DRParse, (Recsize.X / 2 + 3) * Zoom, Recsize.Y / 2 * Zoom);
            Flr3.RenderTransform = new RotateTransform(p / DRParse, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);
            Flr4.RenderTransform = new RotateTransform(p / DRParse, (Recsize.X / 2 + 3) * Zoom, Recsize.Y / 2 * Zoom);
            Flr5.RenderTransform = new RotateTransform(p / DRParse, Recsize.X / 2 * Zoom, Recsize.Y / 2 * Zoom);
            ell.RenderTransform = new RotateTransform(ball.GetAngle() * (1 / DRParse), Radius * Zoom, Radius * Zoom);

        }

        //ボールを放つ
        public void startBall(){
            ball.SetMassFromShapes();

        }
        //ボールを止める
        public void stopBall()
        {
            MassData stop = new MassData();
            stop.Mass = 0;
            ball.SetMass(stop);
        }
        //ゴールしたか判断する
        public bool checkGoal(){
            bool res = isGoal;
            if (isGoal)
            {
                isGoal = false;
            }
            return res;
        }
        //リスタート処理
        public void Restart()
        {
            stopBall();
            ball.SetXForm(new Vec2(17, 0), 0);
            ball.SetAngularVelocity(0);
            ball.SetLinearVelocity(new Vec2(0, 0));
        }
        //ゴールのかごを動かす
        public void renewGallPos()
        {
            const float dx = 0.01f;
            gall.SetXForm(new Vec2(gall.GetPosition().X + (dx * pm), gall.GetPosition().Y), 0);
            recount++;
            if (recount > 240)
            {
                pm *= -1;
                recount = 0;
            }
            if(isGameover){
                Restart();
                isGameover = false;
            }
        }

        //衝突判定リスナ(インナークラス)
        private class GoalListener : ContactListener
        {
            Parts p;
            Body prevBody = null;
            string ui;
            public GoalListener(Parts p)
            {
                this.p = p;
                string buf = System.IO.Directory.GetCurrentDirectory();
                ui = buf.Replace("/", @"\") + @"\..\..\Sounds\poyo.wav";
            }
            public override void Add(ContactPoint point)
            {
                base.Add(point);
                if (point.Shape1 == p.glshape || point.Shape2 == p.glshape)
                {
                    p.isGoal = true;
                }
                if (point.Shape1 == p.ufshape || point.Shape2 == p.ufshape)
                {
                    p.isGameover = true;//直接メソッド呼び出しがなぜか怒られるので、フラグをセットしてreNewGallPosを踏み台にしてリスタート処理
                }
                if (point.Shape1 == p.blshape) 
                {
                    if (point.Shape2.GetBody() != prevBody)
                    {
                        prevBody = point.Shape2.GetBody();
                        SoundPlayer sp = new SoundPlayer(ui);
                        sp.Play();
                    }
                }
                if (point.Shape2 == p.blshape)
                {
                    if (point.Shape1.GetBody() != prevBody)
                    {
                        prevBody = point.Shape1.GetBody();
                        SoundPlayer sp = new SoundPlayer(ui);
                        sp.Play();
                    }
                }

            }
        }
    }
}

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

class Shogi : Form {

  public const int ChipX = 50;
  public const int ChipY = 55;
  private Piece[,] Pieces = new Piece[9,9];
  public Piece[] Ally;
  public Piece[] Enemy;
  public Piece[,] AllyStock = new Piece[9,5];
  public Piece[,] EnemyStock = new Piece[9,5];
  private Piece Have = null;

  private Image Field;

  static void Main(){
    Application.Run(new Shogi());
  }
  public Shogi(){
    this.Text = "Something like Shogi";
    this.MouseClick += new MouseEventHandler(MyMouseClick);
    this.KeyPress += new KeyPressEventHandler(MyKeyPress);
    ClientSize = new Size(577, 496);
    BuildField();
    Ally = new Piece[20];
    Enemy = new Piece[20];
    BuildPiece(Ally, true);
    BuildPiece(Enemy, false);
    Ally = null;
    Enemy = null;
    this.SetStyle(
      ControlStyles.Opaque |
      ControlStyles.DoubleBuffer |
      ControlStyles.UserPaint |
      ControlStyles.AllPaintingInWmPaint, true
    );
  }
  private void BuildField(){
    Pen pen = new Pen(Color.Black);
    Brush sb = new SolidBrush(Color.White);
    this.Field = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
    Graphics g = Graphics.FromImage(Field);
    g.FillRectangle(sb, 0, 0, this.ClientSize.Width, ChipY*9);
    g.DrawLine(pen, ChipX*9, this.ClientSize.Height/2, this.ClientSize.Width, this.ClientSize.Height/2);
    for(int i = 0; i < Pieces.GetLength(0); i++){
      for(int j = 0; j < Pieces.GetLength(1); j++){
        g.DrawRectangle(pen, ChipX*i, ChipY*j, ChipX, ChipY);
      }
    }
  }
  private void BuildPiece(Piece[] pieces, bool player){
    if(player){
      String[] table1 = new String[]{"香","桂","銀","金","王","金","銀","桂","香"};
      String[] table2 = new String[]{"香成","桂成","銀成"," "," "," ","銀成","桂成","香成"};
      int c = 0;
      for(int i = 0; i < 9; i++, c++){
        pieces[c] = new Piece("歩", "と金", i, 6, true, true);
        Pieces[i,6] = pieces[c];
      }
      pieces[c] = new Piece("角", "竜馬", 1, 7, true, true);
      Pieces[1,7] = pieces[c];
      c++;
      pieces[c] = new Piece("飛", "龍王", 7, 7, true, true);
      Pieces[7,7] = pieces[c];
      c++;
      for(int i = 0; i < 9; i++, c++){
        if(table2[i] == " "){
          pieces[c] = new Piece(table1[i], i, 8, true, true);
        }else{
          pieces[c] = new Piece(table1[i], table2[i], i, 8, true, true);
        }
        Pieces[i,8] = pieces[c];
      }
    }else{
      String[] table1 = new String[]{"香","桂","銀","金","玉","金","銀","桂","香"};
      String[] table2 = new String[]{"香成","桂成","銀成"," "," "," ","銀成","桂成","香成"};
      int c = 0;
      for(int i = 0; i < 9; i++, c++){
        pieces[c] = new Piece("歩", "と金", i, 2, false, true);
        Pieces[i,2] = pieces[c];
      }
      pieces[c] = new Piece("角", "竜馬", 7, 1, false, true);
      Pieces[7,1] = pieces[c];
      c++;
      pieces[c] = new Piece("飛", "龍王", 1, 1, false, true);
      Pieces[1,1] = pieces[c];
      c++;
      for(int i = 0; i < 9; i++, c++){
        if(table2[i] == " "){
          pieces[c] = new Piece(table1[i], i, 0, false, true);
        }else{
          pieces[c] = new Piece(table1[i], table2[i], i, 0, false, true);
        }
        Pieces[i,0] = pieces[c];
      }
    }
  }
  void MyMouseClick(object sender, MouseEventArgs e){
    int x = e.X/ChipX;
    int y = e.Y/ChipY;
    if(this.Have != null){
      if(e.X < ChipX*9){
        this.Have.Have();
        if(Pieces[x, y] == null){
          Pieces[x, y] = this.Have;
          Pieces[Have.GetX(), Have.GetY()] = null;
          this.Have.Worp(x, y);
          this.Have = null;
        }else{
          if(this.Have.GetX() == x && this.Have.GetY() == y){
            this.Have = null;
          }else{
            this.Have = Pieces[x, y];
            this.Have.Have();
          }
        }
      }else{
        this.Have.Mini = true;
        this.Have.Worp(9,9);
        this.Have = null;
      }
    }else{
      if(e.X < ChipX*9){
        if(Pieces[x, y] != null){
          this.Have = Pieces[x, y];
          this.Have.Have();
        }
      }else{

      }
    }
    // int x = e.X/ChipX;
    // int y = e.Y/ChipY;
    // if(Pieces[x, y] == null){
    //   if(this.Have != null){
    //     Pieces[x, y] = this.Have;
    //     this.Have.Have();
    //     Pieces[Have.GetX(), Have.GetY()] = null;
    //     this.Have.Worp(x, y);
    //     this.Have = null;
    //   }
    // }else{
    //   if(this.Have != null){
    //     this.Have.Have();
    //   this.Have = Pieces[x, y];
    //   this.Have.Have();
    //   }
    // }
    Refresh();
  }
  void MyKeyPress(object sender, KeyPressEventArgs e){
    if(e.KeyChar == 'n'){
      Console.Write(1);
      if(this.Have != null){
        Console.Write(2);
        this.Have.Turn();
        Refresh();
      }
    }
  }
  protected override void OnPaint(PaintEventArgs e){
    base.OnPaint(e);
    Graphics g = e.Graphics;
    g.DrawImage(Field, 0, 0);
    for(int i = 0; i < Pieces.GetLength(0); i++){
      for(int j = 0; j < Pieces.GetLength(1); j++){
        if(Pieces[i, j] != null){
          Pieces[i, j].Draw(g);
        }
      }
    }
  }
}


class Piece{
  private Image FrontImage;
  private Image BackImage;
  public int X;
  public int Y;
  const int Xdiff = 50;
  const int Ydiff = 55;
  private bool IsFront = true;
  public bool IsAlly;
  public bool Mini = false;
  public bool IsHave = false;
  private bool HaveBack = false;

  public Piece(Bitmap image, int x, int y, bool ally){
    this.FrontImage = image;
    this.BackImage = null;
    this.IsAlly = ally;
    Worp(x, y);
  }
  public Piece(Bitmap frontImage, Bitmap backImage, int x, int y, bool ally){
    this.FrontImage = frontImage;
    this.BackImage = backImage;
    this.HaveBack = true;
    this.IsAlly = ally;
    Worp(x, y);
  }
  public Piece(String fileName, int x, int y, bool ally){
    this.FrontImage = new Bitmap(fileName);
    this.BackImage = null;
    this.IsAlly = ally;
    Worp(x, y);
  }
  public Piece(String fileNameFront, String fileNameBack, int x, int y, bool ally){
    this.FrontImage = new Bitmap(fileNameFront);
    this.BackImage = new Bitmap(fileNameBack);
    this.HaveBack = true;
    this.IsAlly = ally;
    Worp(x, y);
  }
  public Piece(String pieceName, int x, int y, bool ally, bool no){
    this.FrontImage = BuildImage(pieceName, false);
    this.BackImage = null;
    this.IsAlly = ally;
    Worp(x, y);
  }
  public Piece(String pieceNameFront, String pieceNameBack, int x, int y, bool ally, bool no){
    this.FrontImage = BuildImage(pieceNameFront, false);
    this.BackImage = BuildImage(pieceNameBack, true);
    this.HaveBack = true;
    this.IsAlly = ally;
    Worp(x, y);
  }

  public Image BuildImage(String name, bool back){
    Image buffer = new Bitmap(Xdiff, Ydiff);
    Graphics g = Graphics.FromImage(buffer);
    Brush sb = new SolidBrush(Color.Brown);
    g.FillRectangle(sb, 2, 2, 46, 51);
    if(!back){
      sb = new SolidBrush(Color.Black);
    }else{
      sb = new SolidBrush(Color.Red);
    }
    if(name.Length == 1){
      g.DrawString(name, new Font("有澤行書", 28), sb, 0, 8);
    }else{
      g.DrawString(name.Substring(0,1), new Font("有澤行書", 24), sb, 4, 0);
      g.DrawString(name.Substring(1,1), new Font("有澤行書", 24), sb, 4, 25);
    }
    g.Dispose();
    return buffer;
  }
  public void Draw(Graphics g){
    if(Mini){
      g.DrawImage(GetImage(), X, Y, Xdiff/2, Ydiff/2);
    }else{
      if(IsHave){
        g.DrawImage(CreateNegativeImage(GetImage()), X, Y, Xdiff, Ydiff);
      }else{
        g.DrawImage(GetImage(), X, Y, Xdiff, Ydiff);
      }
    }
  }
  public void Worp(int x, int y){
    this.X = x*Xdiff;
    this.Y = y*Ydiff;
  }
  public void Move(int x, int y){
    this.X += x*Xdiff;
    this.Y += y*Ydiff;
  }
  public void Turn(){
    if(HaveBack){
      IsFront = !IsFront;
    }
  }
  public void Have(){
    this.IsHave = !this.IsHave;
  }
  public Image Rotate(Image img){
    Image img2 = (Image)img.Clone();
    img2.RotateFlip(RotateFlipType.Rotate180FlipX);
    img2.RotateFlip(RotateFlipType.Rotate180FlipY);
    return img2;
  }
  public Image GetImage(){
    if(IsFront){
      if(IsAlly){
        return FrontImage;
      }else{
        return Rotate(FrontImage);
      }
    }else{
      if(IsAlly){
        return FrontImage;
      }else{
        return Rotate(BackImage);
      }
    }
  }
  public int GetX(){
    return this.X/Xdiff;
  }
  public int GetY(){
    return this.Y/Ydiff;
  }
  //コピペ
  public static Image CreateNegativeImage(Image img){
    //ネガティブイメージの描画先となるImageオブジェクトを作成
    Bitmap negaImg = new Bitmap(img.Width, img.Height);
    //negaImgのGraphicsオブジェクトを取得
    Graphics g = Graphics.FromImage(negaImg);

    //ColorMatrixオブジェクトの作成
    System.Drawing.Imaging.ColorMatrix cm =
        new System.Drawing.Imaging.ColorMatrix();
    //ColorMatrixの行列の値を変更して、色が反転されるようにする
    cm.Matrix00 = -1;
    cm.Matrix11 = -1;
    cm.Matrix22 = -1;
    cm.Matrix33 = 1;
    cm.Matrix40 = cm.Matrix41 = cm.Matrix42 = cm.Matrix44 = 1;

    //ImageAttributesオブジェクトの作成
    System.Drawing.Imaging.ImageAttributes ia =
        new System.Drawing.Imaging.ImageAttributes();
    //ColorMatrixを設定する
    ia.SetColorMatrix(cm);

    //ImageAttributesを使用して色が反転した画像を描画
    g.DrawImage(img,
        new Rectangle(0, 0, img.Width, img.Height),
        0, 0, img.Width, img.Height, GraphicsUnit.Pixel, ia);

    //リソースを解放する
    g.Dispose();

    return negaImg;
  }

}

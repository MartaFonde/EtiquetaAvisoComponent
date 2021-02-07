using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EtiquetaAvisoComponent
{
    public enum eMarca
    {
        Nada,
        Cruz,
        Circulo, 
        ImagenDeForma
    }
    [
        DefaultProperty("Marca"),
        DefaultEvent("Click")
    ]
    public partial class EtiquetaAviso : Control
    {
        Rectangle rect = new Rectangle(0, 0, 0, 0);

        [Category("Design")]
        [Description("Marca a la izquierda de label")]
        private eMarca marca = eMarca.Nada;
        public eMarca Marca
        {
            set
            {
                if(Enum.IsDefined(typeof(eMarca), value))
                {
                    marca = value;                    
                    Refresh();
                    
                }
                else
                {
                    throw new InvalidEnumArgumentException();
                }
                
            }
            get { return marca; }
        }

        private Color colorInicial;

        [Category("Design")]
        [Description("Color inicial del gradiente de fondo")]
        public Color ColorInicial
        {
            set
            {
                //if(Enum.IsDefined(typeof(Color), value))      Non fai falta porque o VS xa nos pide escoller Color
                //{
                    colorInicial = value;
                    Refresh();
                //}
                //else
                //{
                   // throw new InvalidEnumArgumentException();
                //}
            }
            get
            {
                return colorInicial;
            }
        }

        private Color colorFinal;

        [Category("Design")]
        [Description("Color final del gradiente de fondo")]
        public Color ColorFinal
        {
            set
            {
                colorFinal = value;
                Refresh();                             
            }
            get
            {
                return colorFinal;
            }
        }
        private bool gradiente;

        [Category("Design")]
        [Description("Gradiente de fondo")]
        public bool Gradiente
        {
            set
            {
                gradiente = value;                
                Refresh();                
            }
            get
            {
                return gradiente;
            }
        }

        private Bitmap imagenMarca;
        [Category("Design")]
        [Description("Imagen de marca")]
        public Bitmap ImagenMarca
        {
            set
            {
                try
                {
                    Bitmap b = new Bitmap(value);
                    imagenMarca = value;
                    Refresh();
                }
                catch (Exception e) when (e is ArgumentException || e is FileNotFoundException)
                {
                    MessageBox.Show("There was an error.Check the path to the image file.");
                }                                                      
            }
            get
            {
                return imagenMarca;
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (gradiente)
            {
                LinearGradientBrush gb = new LinearGradientBrush(new PointF(0, 0), new PointF(this.Width, this.Height), 
                    ColorInicial, ColorFinal);
                e.Graphics.FillRectangle(gb, new RectangleF(0, 0, this.Width, this.Height));
                gb.Dispose();
            }

            int grosor = 0; //Grosor de las líneas de dibujo
            int offsetX = 0; //Desplazamiento a la derecha del texto
            int offsetY = 0; //Desplazamiento hacia abajo del texto
                             //Esta propiedad provoca mejoras en la apariencia o en la eficiencia
                             // a la hora de dibujar
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //Dependiendo del valor de la propiedad marca dibujamos una
            //Cruz o un Círculo
            switch (Marca)
            {
                case eMarca.Circulo:
                    grosor = 20;
                    g.DrawEllipse(new Pen(Color.Green, grosor), grosor, grosor, this.Font.Height, this.Font.Height);

                    rect = new Rectangle(grosor / 2, grosor / 2, this.Font.Height+grosor, this.Font.Height+grosor);

                    offsetX = this.Font.Height + grosor;
                    offsetY = grosor;
                    break;
                case eMarca.Cruz:
                    grosor = 5;
                    Pen lapiz = new Pen(Color.Red, grosor);
                    g.DrawLine(lapiz, grosor, grosor, this.Font.Height, this.Font.Height);
                    g.DrawLine(lapiz, this.Font.Height, grosor, grosor, this.Font.Height);

                    rect = new Rectangle(grosor / 2, grosor / 2, this.Font.Height + grosor/2, this.Font.Height + grosor/2);

                    offsetX = this.Font.Height + grosor;
                    offsetY = grosor / 2;
                    //Es recomendable liberar recursos de dibujo pues se
                    //pueden realizar muchos y cogen memoria
                    lapiz.Dispose();
                    break;
                case eMarca.ImagenDeForma:
                    if(ImagenMarca != null)
                    {
                        grosor = 5;                        
                        g.DrawImage(ImagenMarca, grosor, grosor, this.Font.Height, this.Font.Height);

                        rect = new Rectangle(grosor, grosor, this.Font.Height, this.Font.Height);

                        offsetX = this.Font.Height + grosor;
                        offsetY = grosor;
                    }
                    break;
                case eMarca.Nada:
                    rect = new Rectangle(0, 0, 0, 0);
                    break;
            }

            //g.DrawRectangle(new Pen(new SolidBrush(Color.Red)), rect);                
            

            //Finalmente pintamos el Texto; desplazado si fuera necesario
            SolidBrush b = new SolidBrush(this.ForeColor);
            g.DrawString(this.Text, this.Font, b, offsetX + grosor, offsetY);
            Size tam = g.MeasureString(this.Text, this.Font).ToSize();
            this.Size = new Size(tam.Width + offsetX + grosor, tam.Height + offsetY * 2);

            b.Dispose();

        }

        [Category("Click en marca")]
        [Description("Se lanza cuando se hace click en marca")]
        public event System.EventHandler ClickEnMarca;

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Point p = new Point(this.PointToClient(Cursor.Position).X, this.PointToClient(Cursor.Position).Y);
            if (/*Marca != eMarca.Nada && */rect.Contains(p))
            {
                //MessageBox.Show("aqui "+this.PointToClient(Cursor.Position).X+", "+ this.PointToClient(Cursor.Position).Y);
                //MessageBox.Show("click rect");
                ClickEnMarca?.Invoke(this, EventArgs.Empty);                
            }
        }
        

        public EtiquetaAviso()
        {
            InitializeComponent();
            this.ColorInicial = Color.White;
            this.ColorFinal = Color.White;
            this.Gradiente = false;
        }
    }
}

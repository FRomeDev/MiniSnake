using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace MiniSnake
{
    public class Game1 : Game
    { 
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Texture2D serpiente;
        Texture2D ladrillo;
        Texture2D manzana;
        List<Vector2> posicSegmentos = new List<Vector2>();
        int velocidad = 5;
        int velocidadX = 6, velocidadY = 0;
        int columnas = 1280 / 40;
        int filas = 720 / 40;
        int puntos = 0;
        int fotoGramasPorSegundo = 40;
        string[] nivel =
        {
            "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
            "X                       X   a  X",
            "X     a                 X      X",
            "X                              X",
            "X             a                X",
            "X                       X      X",
            "X                       X      X",
            "X                       X a    X",
            "X                       X      X",
            "XXXXXXXXXX              X      X",
            "X                              X",
            "X  a                           X",
            "X           XXXXXXXXX          X",
            "X               a              X",
            "X   XXXXX                      X",
            "X       X                   a  X",
            "X   a   X                      X",
            "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
        };
        List<Rectangle> obstaculos =  new List<Rectangle>();
        List<Rectangle> manzanas = new List<Rectangle>();
        SpriteFont fuente;
        Song musicaDeFondo;
        SoundEffect recogerManzanas;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            posicSegmentos.Add(new Vector2(300, 200));

            for (int fila = 0; fila < filas; fila++)
            {
                for (int columna = 0; columna < columnas; columna++)
                {
                    if (nivel[fila][columna] == 'X')
                    {
                        obstaculos.Add(
                            new Rectangle(columna * 40, fila * 40, 40, 40));
                    }
                    if (nivel[fila][columna] == 'a')
                    {
                        manzanas.Add(
                            new Rectangle(columna * 40, fila * 40, 40, 40));
                    }
                }
            }
             IsFixedTimeStep = true;
            TargetElapsedTime = System.TimeSpan.FromSeconds(
                1.0f / fotoGramasPorSegundo);

        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            serpiente = Content.Load<Texture2D>("ball");
            ladrillo = Content.Load<Texture2D>("brick");
            manzana = Content.Load<Texture2D>("apple");
            fuente = Content.Load<SpriteFont>("arial");
            musicaDeFondo = Content.Load<Song>("Sonido de fondo");
            recogerManzanas = Content.Load<SoundEffect>("Blob");
            MediaPlayer.Play(musicaDeFondo);
            MediaPlayer.IsRepeating = true;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Comprobacion de teclas
            var state = Keyboard.GetState(); 
            if (state.IsKeyDown(Keys.Right))
            {
                velocidadX = velocidad;
                velocidadY = 0;
            }

            if (state.IsKeyDown(Keys.Left))
            {
                velocidadX = -velocidad;
                velocidadY = 0;
            }
            if (state.IsKeyDown(Keys.Up))
            {
                velocidadX = 0;
                velocidadY = -velocidad;
            }
            if (state.IsKeyDown(Keys.Down))
            {
                //state.GetPressedKeys
                velocidadX = 0;
                velocidadY = velocidad;
            }
            for (int i = posicSegmentos.Count -1; i >= 1; i--)            
                posicSegmentos[i] = posicSegmentos[i -1];

                posicSegmentos[0] = new Vector2(
                (float)(posicSegmentos[0].X +
                velocidadX),
                (float)(posicSegmentos[0].Y +
                velocidadY));

            // Comprobacion de colisiones
            foreach (Rectangle o in obstaculos)
             {
                if (o.Intersects(
                    new Rectangle(
                        (int)posicSegmentos[0].X, 
                        (int)posicSegmentos[0].Y,
                        40, 40)))
                    Exit();
            }
            // Comprobar si colisiona contra si misma

            /*for (int p = 1; p < posicSegmentos.Count; p++)
            {
                if (posicSegmentos[0].X == posicSegmentos[p].X)
                { 
                  Exit();
                }
            } */ 
            
            for (int i = 0;i<manzanas.Count;i++)
            {
                if (manzanas[i].Intersects(
                    new Rectangle(
                        (int)posicSegmentos[0].X, 
                        (int)posicSegmentos[0].Y,
                         40, 40)))
                {

                    manzanas.RemoveAt(i);
                    recogerManzanas.CreateInstance().Play();
                    puntos += 5;
                    float xUltima = posicSegmentos[posicSegmentos.Count - 1].X;
                    float yUltima = posicSegmentos[posicSegmentos.Count - 1].Y;
                    posicSegmentos.Add(
                        new Vector2(
                            xUltima - System.Math.Sign(velocidadX) * 40,
                            yUltima - System.Math.Sign(velocidadY) * 40));
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(30,30,30));

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            for(int fila = 0;fila <filas;fila++)
            {
                for(int columna = 0; columna < columnas;columna++)
                {
                    if (nivel[fila][columna] == 'X')
                        spriteBatch.Draw(ladrillo,
                            new Rectangle(columna * 40, fila * 40, 40, 40),
                            Color.White);
                }
            }

            foreach (Rectangle o in manzanas)
            {
                spriteBatch.Draw(manzana,o,
                            Color.White);
            }

            spriteBatch.DrawString(fuente,
                "Puntos: " + puntos,
                new Vector2(50, 50),
                Color.Red); 

            foreach (Vector2 pos in posicSegmentos)
                spriteBatch.Draw(serpiente,
                new Rectangle((int)pos.X, (int)pos.Y, 40, 40),
                Color.White);



            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

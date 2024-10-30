using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TextRenderer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int _screenX, _screenY;

        private TextRenderer _textRenderer;
        private Texture2D _lettersSheet;

        private TextManager _textManager;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            _screenX = 1024;
            _screenY = 768;
            _graphics.PreferredBackBufferWidth = _screenX;
            _graphics.PreferredBackBufferHeight = _screenY;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _lettersSheet = Content.Load<Texture2D>("LetterAtlas");
            _textRenderer = new TextRenderer(_spriteBatch, _lettersSheet);
            _textRenderer.SetAsciiRange(32, 151);
            _textRenderer.SetFontWidthInPixels(8);
            _textRenderer.CondenseLetterSpacing(2);
            _textRenderer.SetFontScale(2);

            _textManager = new TextManager();
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            _textManager.TypeWriterEffect(_spriteBatch, _textRenderer, "This is a test string...", 0, new Vector2(100, 100));
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        public static void DrawLineBetween(
            SpriteBatch spriteBatch,
            Vector2 startPos,
            Vector2 endPos,
            int thickness,
            Color color
            )
        {
            // Create a texture as wide as the distance between two points and as high as
            // the desired thickness of the line.
            var distance = (int)Vector2.Distance(startPos, endPos);
            var texture = new Texture2D(spriteBatch.GraphicsDevice, distance, thickness);

            // Fill texture with given color.
            var data = new Color[distance * thickness];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = color;
            }
            texture.SetData(data);

            // Rotate about the beginning middle of the line.
            var rotation = (float)Math.Atan2(endPos.Y - startPos.Y, endPos.X - startPos.X);
            var origin = new Vector2(0, thickness / 2);

            spriteBatch.Draw(
                texture,
                startPos,
                null,
                Color.White,
                rotation,
                origin,
                1.0f,
                SpriteEffects.None,
                1.0f);
        }

    }
}

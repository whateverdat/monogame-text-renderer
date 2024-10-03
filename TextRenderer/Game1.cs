using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TextRenderer
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private int _screenX, _screenY;

        private TextRenderer _textRenderer;
        private Texture2D _lettersSheet;

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
            _lettersSheet = Content.Load<Texture2D>("LettersSheet");
            _textRenderer = new TextRenderer(_spriteBatch, _lettersSheet);
            _textRenderer.SetFontWidthInPixels(8);
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

            List<string> _texts = new List<string>();
            _texts = new List<string>
            {
                "This is you. Run around with WASD keys to get near and help the robots that are rebooting.",
                "Be aware of the healthbar above your head, if it depletes, the game is over.",
                "When bad robots are near, you take damage. When good robots are near, you heal up.",
                "Every 10 score point earns you an upgrade. Choose between an increase in speed, health, rebooting speed, or healing amount.",
            };

            _textRenderer.CondenseLetterSpacing(3);
            _textRenderer.SetFontScale(2);
            foreach (var text in _texts)
                _textRenderer.DrawString(text, new Vector2(_screenX / 2 - _textRenderer.MeasureString(text).Width / 2, _screenY / 2 - _textRenderer.MeasureString(text).Height / 2 + _texts.IndexOf(text) * 16));

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

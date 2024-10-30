using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace TextRenderer
{
    internal class TextManager
    {
        private string _typeWriterString;
        private int _typeWriterCounter, _typeWriterCursor;

        private float _fadeInTransparency;
        private int _fadeInCounter;

        private float _fadeOutTransparency = 1;
        private int _fadeOutCounter;
        public TextManager() { }

        /// <summary>
        /// A typewriter style effect for text rendering. Speed must be between 1 and 10.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="renderer"></param>
        /// <param name="text"></param>
        /// <param name="speed"></param>
        /// <param name="position"></param>
        public void TypeWriterEffect(SpriteBatch spriteBatch, TextRenderer renderer, string text, int speed, Vector2 position)
        {
            if (_typeWriterString == null)
            {
                _typeWriterCursor = 0;
                _typeWriterString += text[_typeWriterCursor];
                _typeWriterCursor++;
            }
            else if (_typeWriterString.Length == text.Length)
            {
                renderer.DrawString(spriteBatch, _typeWriterString, position);
                return;
            }

            _typeWriterCounter++;
            if (_typeWriterCounter == 10 - speed)
            {
                _typeWriterString += text[_typeWriterCursor];
                _typeWriterCursor++;
                _typeWriterCounter = 0;
            }
            renderer.DrawString(spriteBatch, _typeWriterString, position);
        }

        /// <summary>
        /// A fade in effect for text rendering. Speed must be between 1 and 10.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="renderer"></param>
        /// <param name="text"></param>
        /// <param name="speed"></param>
        /// <param name="position"></param>
        public void FadeInEffect(SpriteBatch spriteBatch, TextRenderer renderer, string text, int speed, Vector2 position)
        {
            if (_fadeInTransparency == 0)
            {
                _fadeInTransparency = .01f * speed;
            }
            else if (_fadeInTransparency == 1)
            {
                renderer.DrawString(spriteBatch, text, position);
                return;
            }

            _fadeInCounter++;
            if (_fadeInCounter == 5 && _fadeInTransparency < 1)
            {
                _fadeInTransparency += .01f * speed;
                if (_fadeInTransparency > 1) _fadeInTransparency = 1;
                _fadeInCounter = 0;
            }
            renderer.DrawString(spriteBatch, text, position, _fadeInTransparency);
        }

        /// <summary>
        /// A fade out effect for text rendering. Speed must be between 1 and 10.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="renderer"></param>
        /// <param name="text"></param>
        /// <param name="speed"></param>
        /// <param name="position"></param>
        public void FadeOutEffect(SpriteBatch spriteBatch, TextRenderer renderer, string text, int speed, Vector2 position)
        {
            if (_fadeOutTransparency == 0) return;

             _fadeOutCounter++;
            if (_fadeOutCounter == 5 && _fadeOutTransparency > 0)
            {
                _fadeOutTransparency -= .01f * speed;
                if (_fadeOutTransparency < 0) _fadeOutTransparency = 0;
                _fadeOutCounter = 0;
            }
            renderer.DrawString(spriteBatch, text, position, _fadeOutTransparency);
        }
    }
}

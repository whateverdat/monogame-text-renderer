using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TextRenderer
{
    internal class TextRenderer
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;

        private Queue<Rectangle> _drawQ;
        private Dictionary<char, Rectangle> _storedPositions;
        
        private int _fontWidth;
        private int _rowCount, _columnCount;
        private int _letterSpacing, _fontScale;
        private int _asciiStartAt, _asciiEndAt;
        public TextRenderer(SpriteBatch spriteBatch, Texture2D LettersSheet)
        {
            _spriteBatch = spriteBatch;
            _texture = LettersSheet;
            _drawQ = new Queue<Rectangle>();
            _asciiStartAt = 32;
            _asciiEndAt = 126;
            _storedPositions = new Dictionary<char, Rectangle>(_asciiEndAt - _asciiStartAt + 1);
        }

        /// <summary>
        /// Sets the width of each character in the provided character sheet.
        /// Width and height of the provided character sheet must be divisible without remainder by the provided width.
        /// </summary>
        /// <param name="width"></param>
        /// <exception cref="Exception"></exception>
        public void SetFontWidthInPixels(int width)
        {
            _fontWidth = width;
            if (_texture.Width % width != 0 || _texture.Height % width != 0) throw new Exception("Provided character width " + width + ", is not compatible with provided character sheet " + _texture.Width + "x" + _texture.Height + ".");
            else
            {
                _rowCount = _texture.Width / width;
                _columnCount = _texture.Height / width;
                _storedPositions.Clear();
            }
        }

        /// <summary>
        /// Set range for allowed ascii characters, default is 32 to 126, from space to '~'.
        /// </summary>
        /// <param name="startAt"></param>
        /// <param name="endAt"></param>
        /// <exception cref="Exception">Difference between each value determines the number of characters in the atlas.</exception>
        public void SetAsciiRange(int startAt, int endAt)
        {
            if (_asciiStartAt >= _asciiEndAt) throw new Exception("Range for allowed ascii range cannot be negative or 0.");
            else
            {
                _asciiStartAt = startAt;
                _asciiEndAt = endAt;
                _storedPositions.Clear();
            }
        }

        /// <summary>
        /// Condense the space between each letter, higher the value, closer the characters. To scramble text and make unreadable, use 'CondenseLetterSpacingUnsafe()'.
        /// </summary>
        public void CondenseLetterSpacing(int width)
        {
            if (width <= _fontWidth / 2)
                _letterSpacing = width;
            else throw new Exception("Letter spacing cannot exceed half of the letter width, if this is intended, use SetLetterSpacingUnsafe()");
        }

        /// <summary>
        /// Condense the space between each letter, higher the value, closer the characters. Use it to scramble texts, make them possibly unreadable.
        /// </summary>
        /// <param name="width"></param>
        public void CondenseLetterSpacingUnsafe(int width)
        {
            _letterSpacing = width;
        }

        /// <summary>
        /// Expand the space between each letter, higher the value, further away the characters from eachother.
        /// </summary>
        /// <param name="width"></param>
        public void ExpandLetterSpacing(int width)
        {
            _letterSpacing = 0 - width;
        }

        /// <summary>
        /// Returns a rectangle with the width and height of a given text, when rendered with current settings.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Rectangle MeasureString(string text)
        {
            return new Rectangle(0, 0, (text.Length * ((_fontWidth * _fontScale) - (_letterSpacing * _fontScale))) + _letterSpacing * _fontScale, _fontWidth * _fontScale);
        }

        /// <summary>
        /// Scale the rendered text size, enlarge the size of a character n times.
        /// </summary>
        /// <param name="scale"></param>
        /// <exception cref="Exception">Font scale is the multiplication factor of the original size.</exception>
        public void SetFontScale(int scale)
        {
            if (scale > 0)
                _fontScale = scale;
            else throw new Exception("Font scale cannot be lower than 1.");
        }

        /// <summary>
        /// Main method of rendering a string to the screen.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        public void DrawString(string text, Vector2 position)
        {
            ParseString(text);
            while (_drawQ.Count > 0)
            {
                _spriteBatch.Draw(_texture, new Rectangle((int)position.X, (int)position.Y, _fontWidth * _fontScale, _fontWidth * _fontScale), _drawQ.Peek(), Color.White);
                _drawQ.Dequeue();
                position.X += _fontWidth * _fontScale - _letterSpacing * _fontScale;
            }
        }

        /// <summary>
        /// Given a string and a max width, text will wrap around in case of going beyond that max width.
        /// Per default behaviour:
        ///     When hyphenation occurs, a hyphen will be placed at the end of the line.
        ///     When a new line's first character is a space, it will be ommited.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="maxWidth"></param>
        public void DrawStringWrapAround(string text, Vector2 position, int maxWidth)
        {
            if (MeasureString(text).Width > maxWidth)
            {
                StoreString(" ?.,-");

                ParseString(text);

                int currentLineWidth = 0;
                int initialX = (int)position.X;

                while (_drawQ.Count > 0)
                {
                    if (currentLineWidth == 0 && _drawQ.Peek() == _storedPositions[' '])
                    {
                        _drawQ.Dequeue();
                        continue;
                    }

                    int spaceUsed = _fontWidth * _fontScale - _letterSpacing * _fontScale;
                    currentLineWidth += spaceUsed;
                    if (currentLineWidth >= maxWidth - spaceUsed)
                    {
                        if (_drawQ.Peek() != _storedPositions[' '])
                            _spriteBatch.Draw(_texture, new Rectangle((int)position.X, (int)position.Y, _fontWidth * _fontScale, _fontWidth * _fontScale), _storedPositions['-'], Color.White);
                        else
                        {
                            position.Y += _fontWidth * _fontScale;
                            position.X = initialX;
                            currentLineWidth = 0;
                            _drawQ.Dequeue();
                            continue;
                        }

                        position.Y += _fontWidth * _fontScale;
                        position.X = initialX;
                        currentLineWidth = spaceUsed; ;
                    }
                    _spriteBatch.Draw(_texture, new Rectangle((int)position.X, (int)position.Y, _fontWidth * _fontScale, _fontWidth * _fontScale), _drawQ.Peek(), Color.White);
                    position.X += spaceUsed;
                    _drawQ.Dequeue();
                }

            } else DrawString(text, position);
        }

        /// <summary>
        /// Find and--if not already--store the corresponding target rectangle of the characters in a string within atlas.
        /// </summary>
        /// <param name="text"></param>
        private void ParseString(string text)
        {
            foreach (char c in text)
            {
                if (_storedPositions.TryGetValue(c, out Rectangle rect)) _drawQ.Enqueue(rect);
                else
                {
                    int charPosition = (int)c - _asciiStartAt;

                    try
                    {
                        if (charPosition < 0 || charPosition > 95) throw new Exception("Only ascii from" + _asciiStartAt + (char)_asciiStartAt + " to " + _asciiEndAt + (char)_asciiEndAt + " allowed.");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        charPosition = (int)'?' - _asciiStartAt; /// Convert to question mark to handle 
                    }

                    Rectangle targetRectangle = GetTargetRectangle(charPosition);

                    _storedPositions.Add(c, targetRectangle);
                    _drawQ.Enqueue(targetRectangle);
                }
            }
        }

        /// <summary>
        /// Given a string, creates position stores for each character in the string.
        /// </summary>
        /// <param name="text"></param>
        private void StoreString(string text)
        {
            foreach (char c in text)
                if (!_storedPositions.TryGetValue(c, out Rectangle rect))
                {
                    int charPosition = (int)c - _asciiStartAt;

                    try
                    {
                        if (charPosition < 0 || charPosition > 95) throw new Exception("Only ascii from" + _asciiStartAt + (char)_asciiStartAt + " to " + _asciiEndAt + (char)_asciiEndAt + " allowed.");
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e);
                        charPosition = (int)'?' - _asciiStartAt; /// Convert to question mark to handle 
                    }
                    Rectangle targetRectangle = GetTargetRectangle(charPosition);
                    _storedPositions.Add(c, targetRectangle);
                }
        }

        /// <summary>
        /// Given a character's position, get the rectangle corresponding to that character on the atlas.
        /// </summary>
        /// <param name="charPosition"></param>
        /// <returns></returns>
        private Rectangle GetTargetRectangle(int charPosition)
        {
            int charX = charPosition % _rowCount;
            int charY = charPosition / _rowCount;
            return new Rectangle(charX * _fontWidth, charY * _fontWidth, _fontWidth, _fontWidth);
        }
    }
}

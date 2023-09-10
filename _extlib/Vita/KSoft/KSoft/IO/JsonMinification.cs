﻿/* This is a .NET port of the Douglas Crockford's JSMin 'C' project.
 * The author's copyright message is reproduced below.
 */

/* jsmin.c
   2013-03-29
Copyright (c) 2002 Douglas Crockford  (www.crockford.com)
Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
The Software shall be used for Good, not Evil.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace DouglasCrockford.JsMin
{
	/// <summary>
	/// The exception that is thrown when a minification of asset code by JSMin is failed
	/// </summary>
	[SuppressMessage("Microsoft.Design", "CA1032")]
	[SuppressMessage("Microsoft.Design", "CA2237")]
	public sealed class JsMinificationException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the DouglasCrockford.JsMin.JsMinificationException
		/// class with a specified error message
		/// </summary>
		/// <param name="message">The message that describes the error</param>
		public JsMinificationException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Initializes a new instance of the DouglasCrockford.JsMin.JsMinificationException
		/// class with a specified error message and a reference to the inner exception that is the cause of
		/// this exception
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception</param>
		/// <param name="innerException">The exception that is the cause of the current exception</param>
		public JsMinificationException(string message, Exception innerException)
			: base(message, innerException)
		{ }
	};

	/// <summary>
	/// The JavaScript Minifier
	/// </summary>
	public sealed class JsMinifier
		: IDisposable
	{
		/// <summary>
		/// Average compression ratio
		/// </summary>
		const double AVERAGE_COMPRESSION_RATIO = 0.6;

		const int EOF = -1;

		private StringBuilder _sb;
		[SuppressMessage("Microsoft.Design", "CA2213:DisposableFieldsShouldBeDisposed")]
		private StringReader _reader;
		[SuppressMessage("Microsoft.Design", "CA2213:DisposableFieldsShouldBeDisposed")]
		private StringWriter _writer;

		private int _theA;
		private int _theB;
		private int _theLookahead = EOF;
		private int _theX = EOF;
		private int _theY = EOF;

		/// <summary>
		/// Synchronizer of minification
		/// </summary>
		private readonly object _minificationSynchronizer = new object();


		public void Dispose()
		{
			KSoft.Util.DisposeAndNull(ref _reader);
			KSoft.Util.DisposeAndNull(ref _writer);
		}

		/// <summary>
		/// Removes a comments and unnecessary whitespace from JavaScript code
		/// </summary>
		/// <param name="content">JavaScript content</param>
		/// <returns>Minified JavaScript content</returns>
		public String Minify(string content)
		{
			string minifiedContent;

			lock (_minificationSynchronizer)
			{
				_theA = 0;
				_theB = 0;
				_theLookahead = EOF;
				_theX = EOF;
				_theY = EOF;

				int estimatedCapacity = (int)Math.Floor(content.Length * AVERAGE_COMPRESSION_RATIO);
				if (_sb == null)
				{
					_sb = new StringBuilder(estimatedCapacity);
				}
				else
				{
					_sb.Capacity = estimatedCapacity;
				}

				_reader = new StringReader(content);
				_writer = new StringWriter(_sb);

				try
				{
					InnerMinify();
					_writer.Flush();

					minifiedContent = TrimStartAndToString(_sb);
				}
				catch (JsMinificationException)
				{
					throw;
				}
				finally
				{
					KSoft.Util.DisposeAndNull(ref _reader);
					KSoft.Util.DisposeAndNull(ref _writer);

					_sb.Clear();
				}
			}

			return minifiedContent;
		}

		/// <summary>
		/// Returns a true if the character is a letter, digit, underscore, dollar sign, or non-ASCII character
		/// </summary>
		/// <param name="c">The character</param>
		/// <returns>Result of check</returns>
		private static bool IsAlphanum(int c)
		{
			return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
				(c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
				c > 126);
		}

		/// <summary>
		/// Returns a next character from input stream. Watch out for lookahead.
		/// If the character is a control character, translate it to a space or linefeed.
		/// </summary>
		/// <returns>The character</returns>
		private int Get()
		{
			int c = _theLookahead;
			_theLookahead = EOF;

			if (c == EOF)
			{
				c = _reader.Read();
			}

			if (c >= ' ' || c == '\n' || c == EOF)
			{
				return c;
			}

			if (c == '\r')
			{
				return '\n';
			}

			return ' ';
		}

		/// <summary>
		/// Gets a next character without getting it
		/// </summary>
		/// <returns>The character</returns>
		private int Peek()
		{
			_theLookahead = Get();

			return _theLookahead;
		}

		/// <summary>
		/// Gets a next character, excluding comments.
		/// <code>Peek()</code> is used to see if a '/' is followed by a '/' or '*'.
		/// </summary>
		/// <returns>The character</returns>
		private int Next()
		{
			int c = Get();

			if (c == '/')
			{
				switch (Peek())
				{
					case '/':
						for (;;)
						{
							c = Get();

							if (c <= '\n')
							{
								break;
							}
						}

						break;
					case '*':
						Get();

						while (c != ' ')
						{
							switch (Get())
							{
								case '*':
									if (Peek() == '/')
									{
										Get();
										c = ' ';
									}

									break;
								case EOF:
									throw new JsMinificationException("Unterminated comment.");
							}
						}

						break;
				}
			}

			_theY = _theX;
			_theX = c;

			return c;
		}

		/// <summary>
		/// Do something! What you do is determined by the argument:
		///		1 - Output A. Copy B to A. Get the next B.
		///		2 - Copy B to A. Get the next B. (Delete A).
		///		3 - Get the next B. (Delete B).
		/// <code>Action</code> treats a string as a single character.
		/// Wow! <code>Action</code> recognizes a regular expression
		/// if it is preceded by <code>(</code> or , or <code>=</code>.
		/// </summary>
		/// <param name="d">Action type</param>
		private void Action(int d)
		{
			if (d == 1)
			{
				Put(_theA);

				if (
					(_theY == '\n' || _theY == ' ') &&
					(_theA == '+' || _theA == '-' || _theA == '*' || _theA == '/') &&
					(_theB == '+' || _theB == '-' || _theB == '*' || _theB == '/')
				)
				{
					Put(_theY);
				}
			}

			if (d <= 2)
			{
				_theA = _theB;

				if (_theA == '\'' || _theA == '"' || _theA == '`')
				{
					for (;;)
					{
						Put(_theA);
						_theA = Get();

						if (_theA == _theB)
						{
							break;
						}

						if (_theA == '\\')
						{
							Put(_theA);
							_theA = Get();
						}

						if (_theA == EOF)
						{
							throw new JsMinificationException("Unterminated string literal.");
						}
					}
				}
			}

			if (d <= 3)
			{
				_theB = Next();
				if (_theB == '/' && (
					_theA == '(' || _theA == ',' || _theA == '=' || _theA == ':' ||
					_theA == '[' || _theA == '!' || _theA == '&' || _theA == '|' ||
					_theA == '?' || _theA == '+' || _theA == '-' || _theA == '~' ||
					_theA == '*' || _theA == '/' || _theA == '{' || _theA == '\n'
				))
				{
					Put(_theA);

					if (_theA == '/' || _theA == '*')
					{
						Put(' ');
					}

					Put(_theB);

					for (;;)
					{
						_theA = Get();

						if (_theA == '[')
						{
							for (;;)
							{
								Put(_theA);
								_theA = Get();

								if (_theA == ']')
								{
									break;
								}

								if (_theA == '\\')
								{
									Put(_theA);
									_theA = Get();
								}

								if (_theA == EOF)
								{
									throw new JsMinificationException("Unterminated set in Regular Expression literal.");
								}
							}
						}
						else if (_theA == '/')
						{
							switch (Peek())
							{
								case '/':
								case '*':
									throw new JsMinificationException("Unterminated set in Regular Expression literal.");
							}

							break;
						}
						else if (_theA == '\\')
						{
							Put(_theA);
							_theA = Get();
						}

						if (_theA == EOF) {
							throw new JsMinificationException("Unterminated Regular Expression literal.");
						}

						Put(_theA);
					}

					_theB = Next();
				}
			}
		}

		/// <summary>
		/// Copies a input to the output, deleting the characters which are insignificant to JavaScript.
		/// Comments will be removed. Tabs will be replaced with spaces.
		/// Carriage returns will be replaced with linefeeds. Most spaces and linefeeds will be removed.
		/// </summary>
		private void InnerMinify()
		{
			if (Peek() == 0xEF)
			{
				Get();
				Get();
				Get();
			}

			_theA = '\n';
			Action(3);

			while (_theA != EOF)
			{
				switch (_theA)
				{
					case ' ':
						Action(IsAlphanum(_theB) ? 1 : 2);
						break;
					case '\n':
						switch (_theB)
						{
							case '{':
							case '[':
							case '(':
							case '+':
							case '-':
							case '!':
							case '~':
								Action(1);
								break;
							case ' ':
								Action(3);
								break;
							default:
								Action(IsAlphanum(_theB) ? 1 : 2);
								break;
						}

						break;
					default:
						switch (_theB)
						{
							case ' ':
								Action(IsAlphanum(_theA) ? 1 : 3);
								break;
							case '\n':
								switch (_theA)
								{
									case '}':
									case ']':
									case ')':
									case '+':
									case '-':
									case '"':
									case '\'':
									case '`':
										Action(1);
										break;
									default:
										Action(IsAlphanum(_theA) ? 1 : 3);
										break;
								}

								break;
							default:
								Action(1);
								break;
						}

						break;
				}
			}
		}

		#region Methods for substitution methods of the C language

		/// <summary>
		/// Puts a character to output stream
		/// </summary>
		/// <param name="c">The character</param>
		private void Put(int c)
		{
			_writer.Write((char)c);
		}

		#endregion

		/// <summary>
		/// Removes the all leading white-space characters from the current <see cref="StringBuilder"/> instance
		/// </summary>
		/// <param name="source">Instance of <see cref="StringBuilder"/></param>
		/// <returns>Instance of <see cref="StringBuilder"/> without leading white-space characters</returns>
		public static string TrimStartAndToString(StringBuilder source)
		{
			int charCount = source.Length;
			if (charCount == 0)
			{
				return source.ToString();
			}

			int charIndex = 0;

			while (charIndex < charCount)
			{
				char charValue = source[charIndex];
				if (!IsWhitespace(charValue))
				{
					break;
				}

				charIndex++;
			}

			if (charIndex > 0)
			{
				source.Remove(0, charIndex);
			}

			return source.ToString(charIndex, source.Length-charIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWhitespace(char source)
		{
			return source == ' ' || (source >= '\t' && source <= '\r');
		}
	};
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mocs.Utils
{
    public class ColorUtil
    {
        /// <summary>
        /// 色の名前からブラシを生成
        /// </summary>
        /// <param name="name">色の名前</param>
        /// <returns>ブラシ</returns>
        public static SolidColorBrush brushFromColorName(string name)
        {
            return new SolidColorBrush(colorFromName(name));
        }

        public static SolidColorBrush brushFromRgb(byte r, byte g, byte b)
        {
            byte a = 255;
            Color color = Color.FromArgb(a, r, g, b);

            return new SolidColorBrush(color);
        }


        /// <summary>
        /// 色の名前からカラーを生成
        /// </summary>
        /// <param name="name">色の名前</param>
        /// <returns>カラー</returns>
        public static Color colorFromName(string name)
        {
            return (Color)System.Windows.Media.ColorConverter.ConvertFromString(name);
        }
    }
}

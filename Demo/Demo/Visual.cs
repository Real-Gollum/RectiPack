using RectiPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Demo {
    
    internal static class Visual {

        public static void RenderImage(Package data, string filePath) {

            // Create the base canvas
            Bitmap img = new Bitmap((int)data.boundingRect.width, (int)data.boundingRect.height);

            Graphics g = Graphics.FromImage(img);

            g.Clear(Color.Black);

            for (int i = 0; i < data.placements.Length; i++) {
                DrawRect(i, data.placements[i], g);
            }

            g.Dispose();

            img.Save(filePath, ImageFormat.Png);

        }


        private static readonly Font font = new Font("Arial", 10);

        private static void DrawRect(int index, AreaRect rect, Graphics g) {

            g.FillRectangle(new SolidBrush(Color.FromArgb(Random.Shared.Next(0x10, 0xf0), Random.Shared.Next(0x10, 0xf0), Random.Shared.Next(0x10, 0xf0))), (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

            g.DrawString($"{index}\n{rect.width}x{rect.height}", font, Brushes.White, (int)rect.x, (int)rect.y);

        }

    }

}

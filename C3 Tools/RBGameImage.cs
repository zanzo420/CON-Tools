using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

/*
 * The concept is the following:
Harmonix always uses the same type of image files in their Xbox 360/PS3 games (observed from Guitar Hero 2 to Rock Band: Blitz), with only the headers varying from game to game and based on the proportions of the image. All the images are DDS textures, in a combination of DXT1, DXT5, and 3Dc Normal Map formats with and without mip-maps enabled. Even though no DXT3 textures have been observed, this method will also support them if they are found.

Xbox 360 images are byte swapped, PS3 images are not. Advanced Art Converter takes care of that for you, just make sure you got the right extension for the file (.png_xbox or .png_ps3).

As of this writing, over 100 HMX headers are included, but there are undoubtedly more to be found.
Run your .png_xbox or .png_ps3 image through Advanced Art Converter, anything that isn't converted properly means it's got a header it doesn't (yet) recognize.

The header is always the first 32 bytes of the .png_xbox / .png_ps3 file, however, the second 16 bytes (so far observed) have always been 0x00, so we can ignore those.

Copy the first 16 bytes of the header into a new binary file (I recommend using HxD, but any hex editor will work), and save it following the naming convention I came up with (more below). To speed up the process, the program only searches for .header files, so make sure to add that extension to it.

The file names are used by the code to efficiently determine the proportions and format of the image when it builds the DDS header we'll need.
Once Advanced Art Converter knows which file it is, it removes the 32-byte HMX header, does the byte-swapping if needed, and adds the appropriate DDS header.

For example: RB3_256x512_DXT5.header tells the code the image is 256 pixels wide, 512 pixels high, and uses DXT5 format. The 'RB3' at the beginning doesn't do anything, it's just there to distinguish the source of the header and because we usually have more than one header of any given proportion from different games. This naming convention is NOT case sensitive.

That's it. Once you got your 16 byte header file properly named and with the .header extension, drop it in the /bin/headers folder. Run Advanced Art Converter again and it will pick it up, and your file should be converted properly this time. Repeat for any other files that fail conversion.

---------------------------------------------------------------------------------

TIPS AND ADVANCED INFORMATION (I.E. OPTIONAL READING)

The current code will always generate mip-map-disabled DDS textures. If the original game texture contained mip-maps, the data is still contained in the file, but the header won't support it so if you open the file in Photoshop or Paint.NET you won't see the mip-maps. You can repair the header manually if you want to recover the mip-maps, for the purposes of this program the added complexity of getting this part of the header correctly is beyond my interest.

NORMAL FILES - Look at the name of the file. If the file name contains _norm or _normal, then make sure that header has NORMAL added to it for format. 3Dc Normal Map textures are used by Harmonix to create depth when combined with the corresponding texture. These are the minority of the textures you'll ever find, but just a heads up. If you incorrectly label it DXT5 or DXT1 the resulting image will be garbled. So getting the format correct is important.

DETERMINING THE FORMAT BASED ON THE HMX HEADER
This becomes easier based on how many files you've looked at. At this point I can pick it out instantly. This is a good way to start:
01 04 08, 02 04 08, or sometimes just 04 08 at the start of the header indicates DXT1
01 08 18, or sometimes just 08 18 at the start of the header indicates DXT5
01 08 20, or sometimes just 08 20 at the start of the header indicates 3Dc Normal Map (NORMAL)

DETERMINING THE DIMENSIONS BASED ON THE HMX HEADER
This changes location in the header based on the game, but it's always going to be width byte height byte or byte width byte height
Everything is in proportion to the number 256. So if the width byte is 0x01 = 256px, if it's 0x02 = 512px, 0x04 = 1024px, 0x08 = 2048px (the largest I've found)
Where the "byte" mentioned above before or after the width and height bytes come into play are if the width or height byte is 0x00.
Now we're working on the basis of 128. So if byte width is 0x80 0x00 = 128px, but if it's 0x40 0x00 = 64px, if it's 0x20 0x00 = 32px, 0x10 0x00 = 16px and 0x08 0x00 = 8px (the smallest I've found)
Knowing that, once you find where in the header the proportion bytes are, you should be able to quickly determine the proportions, if not...

DETERMINING THE DIMENSIONS BASED ON THE FILE SIZE
DDS textures are much like BITMAP images = file size is always the same for an image of the same proportions. Whether the image is transparent, pure white or full of varying colors, the resulting DDS file (and by extension, our .png_xbox or .png_ps3 file) will be a specific file size
Here are some file sizes to get you started:
256x256 DXT1 = 43KB
256x256 DXT5 = 86KB (always 2x the file size of the DXT1 counterpart)
512x512 DXT1 = 171KB
512x512 DXT5 = 342KB (see?)
1024x1024 DXT1 = 682KB
1024x1024 DXT5 = 1.33MB (see again - you get the point)

Using a combination of what you now know, figuring out the image format based on the HMX header suddenly becomes not so difficult :-)

ADVANCED TIP = Rock Band: Blitz and the Dance Central games were observed to vary the first 5 bytes of the header for otherwise identical files. You can still copy and use the 16 bytes as instructed above, but you will get greater coverage if you skip the first 5 bytes and only copy the other 11 to your header file. See the existing BLITZ_ and DC3_ headers for an example.

Hope this is helpful.
 */

namespace C3Tools
{
    public class RBGameImage
    {
        private readonly Stream bytes;
        private readonly bool byteSwapped;
        
        /// <summary>
        /// Instantiates an in-memory image using the given bytes. 
        /// </summary>
        /// <param name="bytes">The game image file bytes.</param>
        /// <param name="byteSwapped">True if the game image file is byte swapped.</param>
        public RBGameImage(Stream bytes, bool byteSwapped)
        {
            this.bytes = bytes;

            if (byteSwapped)
            {
                this.bytes = GetByteSwappedRepresentation();
            }
        }
        
        /// <summary>
        /// Generates a DDS image file header for the given format and size.
        /// </summary>
        /// <param name="format">The format to set.</param>
        /// <param name="width">The width to set.</param>
        /// <param name="height">The height to set.</param>
        /// <returns>The created header.</returns>
        private static byte[] BuildDDSHeader(string format, UInt16 width, UInt16 height)
        {
            // start with a header for a 512x512 DXT5 image
            var dds = new byte[]
                {
                    0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x07, 0x10, 0x0A, 0x00, 0x00, 0x02, 0x00, 0x00, 
                    0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x4E, 0x45, 0x4D, 0x4F, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00, 
                    0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };

            // update the version info
            switch (format.ToLowerInvariant())
            {
                case "dxt1":
                    dds[87] = 0x31;
                    break;
                case "dxt3":
                    dds[87] = 0x33;
                    break;
                case "dxt5":
                    dds[87] = 0x35;
                    break;
                case "normal":
                    dds[84] = 0x41;
                    dds[85] = 0x54;
                    dds[86] = 0x49;
                    dds[87] = 0x32;
                    break;
            }

            // update the size
            dds[12] = (byte) (height & 0xFF);
            dds[13] = (byte) (height >> 8);

            dds[16] = (byte) (width & 0xFF);
            dds[17] = (byte) (width >> 8);

            return dds;
        }

        /// <summary>
        /// Returns a new DDS header for the given game image file header.
        /// </summary>
        /// <param name="hmxHeader">The game image file header to read.</param>
        /// <returns>The appropriately sized DDS header based on the given file.</returns>
        /// <exception cref="Exception">Thrown if an unexpected header format is encountered.</exception>
        private static byte[] GetDDSHeader(IEnumerable<byte> hmxHeader)
        {
            // all samples so far have last 16 bytes of header zeroed out, so to compare we only use first 16
            byte[] initialHeader = hmxHeader.Take(16).ToArray();
            
            //some games have a bunch of headers for the same files, so let's skip the varying portion and just
            //grab the part that tells us the dimensions and image format
            byte[] shortHeader = hmxHeader.Skip(5).Take(11).ToArray();

            //official album art header is usually 256x256 dxt1

            // TODO: don't read headers from file; see how Wii does it
            string[] headers = Directory.GetFiles(Path.Combine(Application.StartupPath, "bin/headers/"), "*.header");

            string matchingHeaderName = null;

            foreach (string headerName in headers)
            {
                byte[] content = File.ReadAllBytes(headerName);
                if (initialHeader.SequenceEqual(content) || shortHeader.SequenceEqual(content))
                {
                    matchingHeaderName = Path.GetFileNameWithoutExtension(headerName).ToUpperInvariant();
                    break;
                }
            }

            if (matchingHeaderName is null)
            {
                throw new Exception("new header format encountered");
            }
            
            // parts is [game, size, format]
            string[] parts = matchingHeaderName.Split('_');

            string ddsFormat;

            if (parts[2].Contains("DXT1"))
            {
                ddsFormat = "DXT1";
            }
            else if (parts[2].Contains("DXT5"))
            {
                ddsFormat = "DXT5";
            }
            else if (parts[2].Contains("NORMAL"))
            {
                ddsFormat = "NORMAL";
            }
            else
            {
                throw new Exception("unhandled recognized header format encountered");
            }

            UInt16[] sizes = parts[1].Split('X').Select(x => Convert.ToUInt16(x)).ToArray();

            return BuildDDSHeader(ddsFormat.ToLowerInvariant(), sizes[0], sizes[1]);
        }

        /// <summary>
        /// Returns the data with the header in-tact and data byte-swapped.
        /// </summary>
        /// <returns>Swapped image file.</returns>
        private MemoryStream GetByteSwappedRepresentation()
        {
            byte[] header = new byte[32];
            bytes.Read(header, 0, 32);

            var output = new MemoryStream();
            output.Write(header, 0, 32);

            var buffer = new byte[4];

            while (bytes.Read(buffer, 0, 4) > 0)
            {
                var word = new byte[4];
                word[0] = buffer[1];
                word[1] = buffer[0];
                word[2] = buffer[3];
                word[3] = buffer[2];
                output.Write(word, 0, 4);
            }

            return output;
        }

        /// <summary>
        /// Returns a DDS file extracted from the given game image file.
        /// </summary>
        /// <returns>The extracted DDS file.</returns>
        public byte[] ToDDSBytes()
        {
            bytes.Seek(0, SeekOrigin.Begin);

            // grab HMX header to compare against known headers
            var originalHeader = new byte[32];
            bytes.Read(originalHeader, 0, 32);

            // create dds header
            var output = new MemoryStream();
            var newHeader = GetDDSHeader(originalHeader);
            output.Write(newHeader, 0, newHeader.Length);

            // copy remaining data
            bytes.CopyTo(output);

            return output.ToArray();
        }

        /// <summary>
        /// Returns an Xbox (byte-swapped) representation of the image file.
        /// </summary>
        /// <returns>The Xbox byte representation.</returns>
        public byte[] ToXboxBytes()
        {
            bytes.Seek(0, SeekOrigin.Begin);
            return GetByteSwappedRepresentation().ToArray();
        }

        /// <summary>
        /// Returns a PS3 representation of the image file.
        /// </summary>
        /// <returns>The PS3 byte representation.</returns>
        public byte[] ToPS3Bytes()
        {
            bytes.Seek(0, SeekOrigin.Begin);

            using (MemoryStream mem = new MemoryStream())
            {
                bytes.CopyTo(mem);
                return mem.ToArray();
            }
        }
    }
}
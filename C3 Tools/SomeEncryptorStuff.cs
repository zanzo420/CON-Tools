using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace C3Tools {
	public class ItemPair {
		private int dataA;
		private int dataB;

		public ItemPair(int A_0, int A_1) {
			this.b(A_0);
			this.a(A_1);
		}

		public int b() {
			return this.dataA;
		}

		public void b(int A_0) {
			this.dataA = A_0;
		}

		public int a() {
			return this.dataB;
		}

		public void a(int A_0) {
			this.dataB = A_0;
		}
	}

	public class EncryptorThing {
		private CryptVersion currentCryptVersion;

		private readonly NemoTools Tools;

		private readonly byte[] m_c = new byte[4] {
			79,
			103,
			103,
			83
		};

		private readonly byte[] m_d = new byte[4] {
			72,
			77,
			88,
			65
		};

		private readonly byte[] m_e = new byte[16] {
			55,
			178,
			226,
			185,
			28,
			116,
			250,
			158,
			56,
			129,
			8,
			234,
			54,
			35,
			219,
			228
		};

		private static byte[] m_f = new byte[16] {
			1,
			34,
			0,
			56,
			210,
			1,
			120,
			139,
			221,
			205,
			208,
			240,
			254,
			62,
			36,
			127
		};

		private static byte[] m_g = new byte[16] {
			81,
			115,
			173,
			229,
			179,
			153,
			184,
			97,
			88,
			26,
			249,
			184,
			30,
			167,
			190,
			191
		};

		private static byte[] h = new byte[16] {
			198,
			34,
			148,
			48,
			216,
			60,
			132,
			20,
			8,
			115,
			124,
			242,
			35,
			246,
			235,
			90
		};

		private static byte[] i = new byte[16] {
			2,
			26,
			131,
			243,
			151,
			233,
			212,
			184,
			6,
			116,
			20,
			107,
			48,
			76,
			0,
			145
		};

		private readonly byte[] j = new byte[16] {
			192,
			135,
			105,
			0,
			226,
			124,
			115,
			235,
			204,
			212,
			33,
			61,
			112,
			42,
			79,
			237
		};

		private readonly byte[] k = new byte[16] {
			49,
			152,
			224,
			109,
			22,
			180,
			128,
			13,
			177,
			202,
			249,
			44,
			216,
			213,
			22,
			130
		};

		[CompilerGenerated] private int l;

		[CompilerGenerated] private int m;

		[CompilerGenerated] private byte[] n;

		[CompilerGenerated] private List<ItemPair> o;

		[CompilerGenerated] private byte[] p;

		[CompilerGenerated]
		private int e() {
			return l;
		}

		[CompilerGenerated]
		private void b(int A_0) {
			l = A_0;
		}

		[CompilerGenerated]
		private int d() {
			return m;
		}

		[CompilerGenerated]
		private void a(int A_0) {
			m = A_0;
		}

		[CompilerGenerated]
		public byte[] getSomeArray() {
			return n;
		}

		[CompilerGenerated]
		public void c(byte[] A_0) {
			n = A_0;
		}

		[CompilerGenerated]
		private List<ItemPair> c() {
			return o;
		}

		[CompilerGenerated]
		private void a(List<ItemPair> A_0) {
			o = A_0;
		}

		[CompilerGenerated]
		private byte[] b() {
			return p;
		}

		[CompilerGenerated]
		private void b(byte[] A_0) {
			p = A_0;
		}

		public EncryptorThing() {
			g();
			this.Tools = new NemoTools();
		}

		public void g() {
			c(new byte[0]);
			b(new byte[0]);
			this.currentCryptVersion = CryptVersion.x0A;
			b(10);
			a(new List<ItemPair>());
		}

		private bool a(byte[] mData) {
			using (MemoryStream input = new MemoryStream(mData)) {
				using (BinaryReader binaryReader = new BinaryReader(input)) {
					this.currentCryptVersion = (CryptVersion) binaryReader.ReadInt32();
					binaryReader.ReadInt32();
					b(binaryReader.ReadInt32());
					a(binaryReader.ReadInt32());
					int num = binaryReader.ReadInt32();
					a(new List<ItemPair>());
					for (int i = 0; i < num; i++) {
						int a_ = binaryReader.ReadInt32();
						int a_2 = binaryReader.ReadInt32();
						c().Add(new ItemPair(a_, a_2));
					}

					if (this.currentCryptVersion != CryptVersion.x0A) {
						int count = (this.currentCryptVersion == CryptVersion.x0B) ? 16 : 72;
						b(binaryReader.ReadBytes(count));
					}

					int count2 = (int) (binaryReader.BaseStream.Length - binaryReader.BaseStream.Position);
					c(binaryReader.ReadBytes(count2));
				}
			}

			return true;
		}

		private void WriteOutResult(string filename, bool A_1, bool A_2, bool A_3 = false) {
			this.Tools.DeleteFile(filename);

			using (FileStream output = File.OpenWrite(filename)) {
				using (BinaryWriter binaryWriter = new BinaryWriter(output)) {
					if (A_1 || A_3) {
						binaryWriter.Write(A_3 ? 13 : 10);
						binaryWriter.Write((A_2 || A_3) ? a() : (a() - b().Length));
						binaryWriter.Write(e());
						binaryWriter.Write(d());
						binaryWriter.Write(c().Count);
						foreach (ItemPair item in c()) {
							binaryWriter.Write(item.b());
							binaryWriter.Write(item.a());
						}

						if (A_2 || A_3) {
							binaryWriter.Write(b());
						}
					}

					binaryWriter.Write(getSomeArray());
				}
			}
		}

		public bool DecryptThing(byte[] mData, bool bypass, bool keepHeader = true, bool A_3 = true,
			DecryptMode decryptMode = DecryptMode.ToFile, string outFilename = "") {
			CryptVersion cryptVersion;
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(mData))) {
				cryptVersion = (CryptVersion) binaryReader.ReadInt32();
			}

			if (!IsSupportedCryptVersion(cryptVersion)) {
				return false;
			}

			if (!a(mData)) {
				return false;
			}

			if (b().Length != 16 && b().Length != 72) {
				return false;
			}

			if (!bypass && !Tools.HasMasterPassword()) {
				return false;
			}

			byte[] a_2;
			switch (this.currentCryptVersion) {
			case CryptVersion.x0B:
				a_2 = this.m_e;
				break;
			case CryptVersion.x0C:
			case CryptVersion.x0D:
				a_2 = CryptoHelperGuy.a(ref EncryptorThing.m_f, ref mData, A_2: false);
				break;
			case CryptVersion.x0E:
				a_2 = CryptoHelperGuy.a(ref EncryptorThing.m_g, ref mData, A_2: true);
				break;
			case CryptVersion.x0F:
				a_2 = CryptoHelperGuy.a(ref h, ref mData, A_2: true);
				break;
			case CryptVersion.x10:
				a_2 = CryptoHelperGuy.a(ref i, ref mData, A_2: true);
				break;
			default:
				return false;
			}

			if (!CryptoHelperGuy.a(new MemoryStream(getSomeArray()), a_2, b())) {
				return false;
			}

			try {
				byte[] first = new BinaryReader(new MemoryStream(getSomeArray())).ReadBytes(4);
				if (first.SequenceEqual(this.m_d)) {
					a(A_0: false);
				} else if (!first.SequenceEqual(this.m_c)) {
					return false;
				}

				if (decryptMode == DecryptMode.ToMemory) {
					return true;
				}

				WriteOutResult(outFilename, keepHeader, A_3);
			} catch (Exception) {
				return false;
			}

			return File.Exists(outFilename);
		}

		private void a(bool A_0 = false) {
			using (MemoryStream memoryStream = new MemoryStream(getSomeArray())) {
				byte[] array = new byte[58];
				memoryStream.Read(array, 0, array.Length);
				uint num = BitConverter.ToUInt32(new byte[4] {
					array[15],
					array[14],
					array[13],
					array[12]
				}, 0);
				uint num2 = BitConverter.ToUInt32(new byte[4] {
					array[23],
					array[22],
					array[21],
					array[20]
				}, 0);
				uint num3 = BitConverter.ToUInt32(new byte[4] {
					b()[24],
					b()[25],
					b()[26],
					b()[27]
				}, 0);
				uint num4 = (num3 ^ 0x36363636) * 1664525 + 1013907456 - 3233;
				uint num5 = BitConverter.ToUInt32(new byte[4] {
					b()[16],
					b()[17],
					b()[18],
					b()[19]
				}, 0);
				uint num6 = (num5 ^ 0x5C5C5C5C) * 1664525 + 1013907456 - 3233;
				num6 = num6 * 1664525 + 1013907456 - 3233;
				num6 ^= num;
				num4 ^= num2;
				memoryStream.Seek(0L, SeekOrigin.Begin);
				memoryStream.Write(A_0 ? this.m_d : this.m_c, 0, this.m_c.Length);
				byte[] array2 = a((IList<byte>) BitConverter.GetBytes(num6));
				memoryStream.Seek(12L, SeekOrigin.Begin);
				memoryStream.Write(array2, 0, array2.Length);
				byte[] array3 = a((IList<byte>) BitConverter.GetBytes(num4));
				memoryStream.Seek(20L, SeekOrigin.Begin);
				memoryStream.Write(array3, 0, array3.Length);
			}
		}

		public bool EncryptThing(byte[] mData, string mOut, bool isPS3 = false) {
			CryptVersion v;
			using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(mData))) {
				v = (CryptVersion) binaryReader.ReadInt32();
			}

			// must be plain text
			if (v != CryptVersion.x0A) {
				return false;
			}

			if (!a(mData)) {
				return false;
			}

			b(isPS3 ? CryptoHelperGuy.h : CryptoHelperGuy.g);
			a(A_0: true);
			if (!CryptoHelperGuy.a(new MemoryStream(getSomeArray()), isPS3 ? k : j, isPS3 ? CryptoHelperGuy.h : CryptoHelperGuy.g)) {
				return false;
			}

			WriteOutResult(mOut, A_1: true, A_2: true, A_3: true);
			return File.Exists(mOut);
		}

		private static byte[] a(IList<byte> A_0) {
			byte[] array = new byte[A_0.Count];
			for (int i = 0; i < A_0.Count; i++) {
				array[i] = A_0[A_0.Count - 1 - i];
			}

			return array;
		}

		private static bool IsSupportedCryptVersion(CryptVersion A_0) {
			switch (A_0) {
			case CryptVersion.x0B:
			case CryptVersion.x0C:
			case CryptVersion.x0D:
			case CryptVersion.x0E:
			case CryptVersion.x0F:
			case CryptVersion.x10:
				return true;
			default:
				return false;
			}
		}

		private int a() {
			return 20 + c().Count * 8 + b().Length;
		}
	}

	public class CryptoHelperGuy {
		private readonly ICryptoTransform ecbEncryptor;

		private readonly byte[] hashBuffer;

		private byte[] c;

		private int d;

		public static readonly byte[] e = new byte[16] {
			0,
			0,
			0,
			0,
			99,
			51,
			45,
			99,
			117,
			115,
			116,
			111,
			109,
			115,
			49,
			52
		};

		public static readonly byte[] f = new byte[72] {
			0,
			0,
			0,
			0,
			99,
			117,
			115,
			116,
			111,
			109,
			115,
			45,
			98,
			121,
			45,
			99,
			117,
			115,
			116,
			111,
			109,
			115,
			99,
			114,
			101,
			97,
			116,
			111,
			114,
			115,
			99,
			111,
			108,
			108,
			101,
			99,
			116,
			105,
			118,
			101,
			45,
			116,
			111,
			111,
			108,
			115,
			45,
			98,
			121,
			45,
			116,
			114,
			111,
			106,
			97,
			110,
			110,
			101,
			109,
			111,
			45,
			50,
			48,
			49,
			53,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		public static readonly byte[] g = new byte[72] {
			0,
			0,
			0,
			0,
			99,
			117,
			115,
			116,
			111,
			109,
			115,
			45,
			99,
			114,
			101,
			97,
			116,
			111,
			114,
			115,
			0,
			0,
			0,
			0,
			195,
			195,
			195,
			195,
			0,
			0,
			0,
			0,
			195,
			195,
			195,
			195,
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			1,
			35,
			69,
			103,
			137,
			171,
			205,
			239,
			195,
			195,
			195,
			195,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		public static readonly byte[] h = new byte[72] {
			0,
			0,
			0,
			0,
			52,
			150,
			193,
			31,
			5,
			9,
			18,
			102,
			158,
			32,
			9,
			239,
			48,
			139,
			162,
			33,
			0,
			0,
			0,
			0,
			77,
			18,
			106,
			71,
			0,
			0,
			0,
			0,
			172,
			5,
			125,
			240,
			139,
			41,
			29,
			144,
			44,
			88,
			122,
			12,
			16,
			195,
			83,
			235,
			78,
			100,
			234,
			218,
			65,
			200,
			165,
			236,
			66,
			38,
			127,
			0,
			4,
			240,
			85,
			39,
			6,
			0,
			0,
			0,
			0,
			0,
			0,
			0
		};

		private static readonly byte[] i = new byte[384] {
			127,
			149,
			91,
			157,
			148,
			186,
			18,
			241,
			215,
			90,
			103,
			217,
			22,
			69,
			40,
			221,
			97,
			85,
			85,
			175,
			35,
			145,
			214,
			10,
			58,
			66,
			129,
			24,
			180,
			247,
			243,
			4,
			120,
			150,
			93,
			146,
			146,
			176,
			71,
			172,
			143,
			91,
			109,
			220,
			28,
			65,
			126,
			218,
			106,
			85,
			83,
			175,
			32,
			200,
			220,
			10,
			102,
			67,
			221,
			28,
			178,
			165,
			164,
			12,
			126,
			146,
			92,
			147,
			144,
			237,
			74,
			173,
			139,
			7,
			54,
			211,
			16,
			65,
			120,
			143,
			96,
			8,
			85,
			168,
			38,
			207,
			208,
			15,
			101,
			17,
			132,
			69,
			177,
			160,
			250,
			87,
			121,
			151,
			11,
			144,
			146,
			176,
			68,
			173,
			138,
			14,
			96,
			217,
			20,
			17,
			126,
			141,
			53,
			93,
			92,
			251,
			33,
			156,
			211,
			14,
			50,
			64,
			209,
			72,
			184,
			167,
			161,
			13,
			40,
			195,
			93,
			151,
			193,
			236,
			66,
			241,
			220,
			93,
			55,
			218,
			20,
			71,
			121,
			138,
			50,
			92,
			84,
			242,
			114,
			157,
			211,
			13,
			103,
			76,
			214,
			73,
			180,
			162,
			243,
			80,
			40,
			150,
			94,
			149,
			197,
			233,
			69,
			173,
			138,
			93,
			100,
			142,
			23,
			64,
			46,
			135,
			54,
			88,
			6,
			253,
			117,
			144,
			208,
			95,
			58,
			64,
			212,
			76,
			176,
			247,
			167,
			4,
			44,
			150,
			1,
			150,
			155,
			188,
			21,
			166,
			222,
			14,
			101,
			141,
			23,
			71,
			47,
			221,
			99,
			84,
			85,
			175,
			118,
			202,
			132,
			95,
			98,
			68,
			128,
			74,
			179,
			244,
			244,
			12,
			126,
			196,
			14,
			198,
			154,
			235,
			67,
			160,
			219,
			10,
			100,
			223,
			28,
			66,
			36,
			137,
			99,
			92,
			85,
			243,
			113,
			144,
			220,
			93,
			96,
			64,
			209,
			77,
			178,
			163,
			167,
			13,
			44,
			154,
			11,
			144,
			154,
			190,
			71,
			167,
			136,
			90,
			109,
			223,
			19,
			29,
			46,
			139,
			96,
			94,
			85,
			242,
			116,
			156,
			215,
			14,
			96,
			64,
			128,
			28,
			183,
			161,
			244,
			2,
			40,
			150,
			91,
			149,
			193,
			233,
			64,
			163,
			143,
			12,
			50,
			223,
			67,
			29,
			36,
			141,
			97,
			9,
			84,
			171,
			39,
			154,
			211,
			88,
			96,
			22,
			132,
			79,
			179,
			164,
			243,
			13,
			37,
			147,
			8,
			192,
			154,
			189,
			16,
			162,
			214,
			9,
			96,
			143,
			17,
			29,
			122,
			143,
			99,
			11,
			93,
			242,
			33,
			236,
			215,
			8,
			98,
			64,
			132,
			73,
			176,
			173,
			242,
			7,
			41,
			195,
			12,
			150,
			150,
			235,
			16,
			160,
			218,
			89,
			50,
			211,
			23,
			65,
			37,
			220,
			99,
			8,
			4,
			174,
			119,
			203,
			132,
			90,
			96,
			77,
			221,
			69,
			181,
			244,
			160,
			5
		};

		private static readonly byte[] j = new byte[32] {
			57,
			162,
			191,
			83,
			125,
			136,
			29,
			3,
			53,
			56,
			163,
			128,
			69,
			36,
			238,
			202,
			37,
			109,
			165,
			194,
			101,
			169,
			148,
			115,
			229,
			116,
			235,
			84,
			229,
			149,
			63,
			28
		};

		public CryptoHelperGuy(byte[] A_0, byte[] A_1) {
			d = 0;
			this.hashBuffer = new byte[16];
			Array.Copy(A_1, this.hashBuffer, 16);
			Rijndael rijndael = Rijndael.Create();
			rijndael.Key = A_0;
			rijndael.Mode = CipherMode.ECB;
			rijndael.Padding = PaddingMode.None;
			this.ecbEncryptor = rijndael.CreateEncryptor();
			a();
		}

		private void a() {
			c = this.ecbEncryptor.TransformFinalBlock(this.hashBuffer, 0, this.hashBuffer.Length);
		}

		private byte[] a(byte[] A_0) {
			if (d == this.hashBuffer.Length) {
				for (int i = 0; i < this.hashBuffer.Length; i++) {
					this.hashBuffer[i]++;
					if (this.hashBuffer[i] != 0) {
						break;
					}
				}

				a();
				d = 0;
			}

			byte[] array = new byte[A_0.Length];
			for (int j = 0; j < A_0.Length; j++) {
				array[j] = c[d + j];
			}

			d += A_0.Length;
			if (A_0.Length != 8) {
				return BitConverter.GetBytes(A_0[0] ^ array[0]);
			}

			long value = BitConverter.ToInt64(A_0, 0) ^ BitConverter.ToInt64(array, 0);
			return BitConverter.GetBytes(value);
		}

		public static bool a(Stream A_0, byte[] A_1, byte[] A_2) {
			byte[] array = new byte[16];
			using (MemoryStream memoryStream = new MemoryStream(A_2)) {
				memoryStream.Read(array, 0, array.Length);
			}

			CryptoHelperGuy guy = new CryptoHelperGuy(A_1, array);
			long num = 0L;
			byte[] array2 = new byte[8];
			for (int i = 0; i < A_0.Length / 8; i++) {
				A_0.Seek(num, SeekOrigin.Begin);
				A_0.Read(array2, 0, array2.Length);
				array2 = guy.a(array2);
				A_0.Seek(num, SeekOrigin.Begin);
				A_0.Write(array2, 0, array2.Length);
				num += array2.Length;
			}

			long num2 = A_0.Length % 8;
			if (num2 == 0) {
				return true;
			}

			array2 = new byte[1];
			for (int i = 0; i < num2; i++) {
				A_0.Seek(num, SeekOrigin.Begin);
				A_0.Read(array2, 0, array2.Length);
				array2[0] = guy.a(array2)[0];
				A_0.Seek(num, SeekOrigin.Begin);
				A_0.Write(array2, 0, array2.Length);
				num += array2.Length;
			}

			return true;
		}

		public static byte[] a(ref byte[] A_0, ref byte[] A_1, bool A_2) {
			byte[] array = new byte[16];
			byte[] array2 = new byte[32];
			int num = BitConverter.ToInt32(A_1, 16);
			Array.Copy(A_1, 20 + num * 8 + 16 + 32, array, 0, 16);
			using (AesManaged aesManaged = new AesManaged()) {
				aesManaged.Mode = CipherMode.ECB;
				aesManaged.BlockSize = 128;
				aesManaged.KeySize = 128;
				aesManaged.Padding = PaddingMode.None;
				aesManaged.Key = A_0;
				ICryptoTransform cryptoTransform = aesManaged.CreateDecryptor();
				cryptoTransform.TransformBlock(array, 0, array.Length, array, 0);
			}

			int num2 = BitConverter.ToInt32(A_1, 20 + num * 8 + 16 + 48);
			int a_ = BitConverter.ToInt32(A_1, 20 + num * 8 + 16);
			int a_2 = BitConverter.ToInt32(A_1, 20 + num * 8 + 16 + 8);
			num2 = num2 % 6 + 6;
			Array.Copy(CryptoHelperGuy.i, 32 * num2, array2, 0, 32);
			array2 = a(j, array2);
			byte[] array3 = a(a_, a_2, a((IList<byte>) array2), A_2);
			byte[] array4 = new byte[16];
			for (int i = 0; i < 16; i++) {
				array4[i] = (byte) (array3[i] ^ array[i]);
			}

			return array4;
		}

		private static byte[] a(int A_0, int A_1, byte[] A_2, bool A_3) {
			int[] array = new int[256];
			int[] array2 = new int[256];
			byte[] array3 = new byte[64];
			byte[] array4 = new byte[64];
			int num = A_0;
			int num2 = A_1;
			for (int i = 0; i < 256; i++) {
				array[i] = (byte) ((byte) A_0 >> 3);
				A_0 = 1664525 * A_0 + 1013904223;
			}

			A_0 = num2;
			for (int i = 0; i < 256; i++) {
				array2[i] = (byte) (((byte) A_0 >> 2) & 0x3F);
				A_0 = 1664525 * A_0 + 1013904223;
			}

			if (A_1 == 0) {
				A_1 = 12351;
			}

			for (int i = 0; i < 32; i++) {
				int num3;
				do {
					A_1 = 1664525 * A_1 + 1013904223;
					num3 = ((A_1 >> 2) & 0x1F);
				} while (array3[num3] != 0);

				array4[i] = (byte) num3;
				array3[num3] = 1;
			}

			int[] array5;
			if (A_3) {
				for (int i = 32; i < 64; i++) {
					int num3;
					do {
						num = 1664525 * num + 1013904223;
						num3 = ((num >> 2) & 0x1F) + 32;
					} while (array3[num3] != 0);

					array4[i] = (byte) num3;
					array3[num3] = 1;
				}

				array5 = array2;
			} else {
				array5 = array;
			}

			for (int j = 0; j < 16; j++) {
				int num4 = 0;
				byte b = A_2[j];
				for (int i = 0; i < 8; i++) {
					int num3 = array4[array5[A_2[num4]]];
					b = a(b, A_2[num4 + 1], num3);
					num4 += 2;
				}

				A_2[j] = b;
			}

			return A_2;
		}

		private static byte a(byte A_0, byte A_1, int A_2) {
			int num;
			switch (A_2) {
			case 0: {
				byte b = a(A_0, (A_1 == 0) ? 1 : 0);
				num = A_1 + b;
				break;
			}
			case 1: {
				byte b = a(A_0, 3);
				num = A_1 + b;
				break;
			}
			case 2: {
				byte b = CryptoHelperGuy.b(A_0, 1);
				num = A_1 + b;
				break;
			}
			case 3:
				num = (A_1 ^ ((A_0 >> (A_1 & 7)) | (byte) (A_0 << (-A_1 & 7))));
				break;
			case 4: {
				byte b = CryptoHelperGuy.b(A_0, 4);
				num = (A_1 ^ b);
				break;
			}
			case 5: {
				byte b = a(A_0, 3);
				num = A_1 + (A_1 ^ b);
				break;
			}
			case 6: {
				byte b = CryptoHelperGuy.b(A_0, 2);
				num = A_1 + b;
				break;
			}
			case 7:
				num = A_1 + ((A_0 == 0) ? 1 : 0);
				break;
			case 8: {
				byte b = a(A_0, (A_1 == 0) ? 1 : 0);
				num = (A_1 ^ b);
				break;
			}
			case 9: {
				byte b = CryptoHelperGuy.b(A_0, 3);
				num = (A_1 ^ (A_1 + b));
				break;
			}
			case 10: {
				byte b = CryptoHelperGuy.b(A_0, 3);
				num = A_1 + b;
				break;
			}
			case 11: {
				byte b = CryptoHelperGuy.b(A_0, 4);
				num = A_1 + b;
				break;
			}
			case 12:
				num = (A_0 ^ A_1);
				break;
			case 13:
				num = (A_1 ^ ((A_0 == 0) ? 1 : 0));
				break;
			case 14: {
				byte b = a(A_0, 3);
				num = (A_1 ^ (A_1 + b));
				break;
			}
			case 15: {
				byte b = CryptoHelperGuy.b(A_0, 3);
				num = (A_1 ^ b);
				break;
			}
			case 16: {
				byte b = CryptoHelperGuy.b(A_0, 2);
				num = (A_1 ^ b);
				break;
			}
			case 17: {
				byte b = CryptoHelperGuy.b(A_0, 3);
				num = A_1 + (A_1 ^ b);
				break;
			}
			case 18:
				num = A_1 + (A_0 ^ A_1);
				break;
			case 19:
				num = A_0 + A_1;
				break;
			case 20: {
				byte b = a(A_0, 3);
				num = (A_1 ^ b);
				break;
			}
			case 21:
				num = (A_1 ^ (A_0 + A_1));
				break;
			case 22:
				num = a(A_0, (A_1 == 0) ? 1 : 0);
				break;
			case 23: {
				byte b = a(A_0, 1);
				num = A_1 + b;
				break;
			}
			case 24:
				num = ((A_0 >> (A_1 & 7)) | (A_0 << (-A_1 & 7)));
				break;
			case 25: {
				byte b = (byte) ((A_0 == 0) ? ((A_1 != 0) ? 1 : 128) : 0);
				num = b;
				break;
			}
			case 26: {
				byte b = a(A_0, 2);
				num = A_1 + b;
				break;
			}
			case 27: {
				byte b = a(A_0, 1);
				num = (A_1 ^ b);
				break;
			}
			case 28:
				A_0 = (byte) (~A_0);
				goto case 24;
			case 29: {
				byte b = a(A_0, 2);
				num = (A_1 ^ b);
				break;
			}
			case 30:
				num = A_1 + ((A_0 >> (A_1 & 7)) | (byte) (A_0 << (-A_1 & 7)));
				break;
			case 31: {
				byte b = CryptoHelperGuy.b(A_0, 1);
				num = (A_1 ^ b);
				break;
			}
			case 32:
				num = ((byte) (((A_0 << 8) | 0xAA | (byte) (~A_0)) >> 4) ^ A_1);
				break;
			case 33:
				num = (byte) (((uint) ((byte) (~A_0) | (A_0 << 8)) >> 3) ^ A_1);
				break;
			case 34:
				num = (byte) (((((A_0 << 8) ^ 0xFF00) | A_0) >> 2) ^ A_1);
				break;
			case 35:
				num = (byte) ((((A_0 ^ 0x5C) | (A_0 << 8)) >> 5) ^ A_1);
				break;
			case 36:
				num = (byte) ((((A_0 << 8) | 0x65 | (A_0 ^ 0x3C)) >> 2) ^ A_1);
				break;
			case 37:
				num = (byte) ((((A_0 ^ 0x36) | (A_0 << 8)) >> 2) ^ A_1);
				break;
			case 38:
				num = (byte) ((((A_0 ^ 0x36) | (A_0 << 8)) >> 4) ^ A_1);
				break;
			case 39:
				num = (byte) ((((A_0 ^ 0x5C) | (A_0 << 8) | 0x36) >> 1) ^ A_1);
				break;
			case 40:
				num = (byte) ((((byte) (~A_0) | (A_0 << 8)) >> 5) ^ A_1);
				break;
			case 41:
				num = (byte) ((((~A_0 << 8) | A_0) >> 6) ^ A_1);
				break;
			case 42:
				num = (byte) ((((A_0 ^ 0x5C) | (A_0 << 8)) >> 3) ^ A_1);
				break;
			case 43:
				num = (byte) ((((A_0 ^ 0x3C) | 0x65 | (A_0 << 8)) >> 5) ^ A_1);
				break;
			case 44:
				num = (byte) ((((A_0 ^ 0x36) | (A_0 << 8)) >> 1) ^ A_1);
				break;
			case 45:
				num = (byte) ((((A_0 ^ 0x65) | (A_0 << 8) | 0x3C) >> 6) ^ A_1);
				break;
			case 46:
				num = (byte) ((((A_0 ^ 0x5C) | (A_0 << 8)) >> 2) ^ A_1);
				break;
			case 47:
				num = (byte) ((((A_1 ^ 0xAA) | (A_1 << 8) | 0xFF) >> 3) ^ A_0);
				break;
			case 48:
				num = (byte) ((((A_0 ^ 0x63) | (A_0 << 8) | 0x5C) >> 6) ^ A_1);
				break;
			case 49:
				num = (byte) ((((A_0 ^ 0x5C) | (A_0 << 8) | 0x36) >> 7) ^ A_1);
				break;
			case 50:
				num = (byte) ((((A_0 ^ 0x5C) | (A_0 << 8)) >> 6) ^ A_1);
				break;
			case 51:
				num = (byte) (((((A_0 << 8) ^ 0xFF00) | A_0) >> 3) ^ A_1);
				break;
			case 52:
				num = (byte) ((((byte) (~A_0) | (A_0 << 8)) >> 6) ^ A_1);
				break;
			case 53:
				num = (byte) (((((A_0 << 8) ^ 0xFF00) | A_0) >> 5) ^ A_1);
				break;
			case 54:
				num = (byte) ((((A_0 ^ 0x3C) | 0x65 | (A_0 << 8)) >> 4) ^ A_1);
				break;
			case 55:
				num = (byte) ((((A_0 ^ 0x63) | (A_0 << 8) | 0x5C) >> 3) ^ A_1);
				break;
			case 56:
				num = (byte) ((((A_0 ^ 0x63) | (A_0 << 8) | 0x5C) >> 5) ^ A_1);
				break;
			case 57:
				num = (byte) ((((A_0 ^ 0xAF) | (A_0 << 8) | 0xFA) >> 5) ^ A_1);
				break;
			case 58:
				num = (byte) ((((A_0 ^ 0x5C) | (A_0 << 8) | 0x36) >> 5) ^ A_1);
				break;
			case 59:
				num = (byte) ((((A_0 ^ 0x5C) | (A_0 << 8) | 0x36) >> 3) ^ A_1);
				break;
			case 60:
				num = (byte) ((((A_0 ^ 0x36) | (A_0 << 8)) >> 3) ^ A_1);
				break;
			case 61:
				num = (byte) ((((A_0 ^ 0x63) | (A_0 << 8) | 0x5C) >> 4) ^ A_1);
				break;
			case 62:
				num = (byte) ((((A_0 ^ 0xFF) | (A_0 << 8) | 0xAF) >> 6) ^ A_1);
				break;
			case 63:
				num = (byte) ((((byte) (~A_0) | (A_0 << 8)) >> 2) ^ A_1);
				break;
			default:
				return 0;
			}

			return (byte) (num & 0xFF);
		}

		private static byte[] a(IList<byte> A_0, byte[] A_1) {
			for (int i = 0; i < 14; i++) {
				a(ref A_1, 19, 2);
				a(ref A_1, 22, 1);
				a(ref A_1, 23, 6);
				a(ref A_1, 26, 5);
				a(ref A_1, 27, 10);
				a(ref A_1, 30, 9);
				a(ref A_1, 31, 14);
				a(ref A_1, 2, 13);
				a(ref A_1, 3, 18);
				a(ref A_1, 6, 17);
				a(ref A_1, 7, 22);
				a(ref A_1, 10, 21);
				a(ref A_1, 11, 26);
				a(ref A_1, 14, 25);
				a(ref A_1, 15, 30);
				a(ref A_1, 18, 29);
				a(ref A_1, 29, 2);
				a(ref A_1, 28, 3);
				a(ref A_1, 25, 6);
				a(ref A_1, 24, 7);
				a(ref A_1, 21, 10);
				a(ref A_1, 20, 11);
				a(ref A_1, 17, 14);
				a(ref A_1, 16, 15);
				a(ref A_1, 13, 18);
				a(ref A_1, 12, 19);
				a(ref A_1, 9, 22);
				a(ref A_1, 8, 23);
				a(ref A_1, 5, 26);
				a(ref A_1, 4, 27);
				a(ref A_1, 1, 30);
				a(ref A_1, 0, 31);
				a(ref A_1, 16, 2);
				a(ref A_1, 28, 3);
				a(ref A_1, 12, 6);
				a(ref A_1, 24, 7);
				a(ref A_1, 8, 10);
				a(ref A_1, 20, 11);
				a(ref A_1, 4, 14);
				a(ref A_1, 16, 15);
				a(ref A_1, 0, 18);
				a(ref A_1, 12, 19);
				a(ref A_1, 28, 22);
				a(ref A_1, 8, 23);
				a(ref A_1, 24, 26);
				a(ref A_1, 4, 27);
				a(ref A_1, 20, 30);
				a(ref A_1, 0, 31);
				a(ref A_1, 29, 2);
				a(ref A_1, 15, 3);
				a(ref A_1, 25, 6);
				a(ref A_1, 11, 7);
				a(ref A_1, 21, 10);
				a(ref A_1, 7, 11);
				a(ref A_1, 17, 14);
				a(ref A_1, 3, 15);
				a(ref A_1, 13, 18);
				a(ref A_1, 31, 19);
				a(ref A_1, 9, 22);
				a(ref A_1, 27, 23);
				a(ref A_1, 5, 26);
				a(ref A_1, 23, 27);
				a(ref A_1, 1, 30);
				a(ref A_1, 19, 31);
				a(ref A_1, 29, 21);
				a(ref A_1, 28, 3);
				a(ref A_1, 25, 25);
				a(ref A_1, 24, 7);
				a(ref A_1, 21, 29);
				a(ref A_1, 20, 11);
				a(ref A_1, 17, 1);
				a(ref A_1, 16, 15);
				a(ref A_1, 13, 5);
				a(ref A_1, 12, 19);
				a(ref A_1, 9, 9);
				a(ref A_1, 8, 23);
				a(ref A_1, 5, 13);
				a(ref A_1, 4, 27);
				a(ref A_1, 1, 17);
				a(ref A_1, 0, 31);
				a(ref A_1, 29, 2);
				a(ref A_1, 28, 22);
				a(ref A_1, 25, 6);
				a(ref A_1, 24, 26);
				a(ref A_1, 21, 10);
				a(ref A_1, 20, 30);
				a(ref A_1, 17, 14);
				a(ref A_1, 16, 2);
				a(ref A_1, 13, 18);
				a(ref A_1, 12, 6);
				a(ref A_1, 9, 22);
				a(ref A_1, 8, 10);
				a(ref A_1, 5, 26);
				a(ref A_1, 4, 14);
				a(ref A_1, 1, 30);
				a(ref A_1, 0, 18);
			}

			for (int i = 0; i < 32; i++) {
				A_1[i] ^= A_0[i];
			}

			return A_1;
		}

		private static byte b(byte A_0, int A_1) {
			return (byte) (((A_0 << A_1) | (A_0 >> 8 - A_1)) & 0xFF);
		}

		private static byte a(byte A_0, int A_1) {
			return (byte) (((A_0 >> A_1) | (A_0 << 8 - A_1)) & 0xFF);
		}

		private static byte a(byte A_0) {
			if (A_0 > 96) {
				return (byte) (A_0 - 87);
			}

			if (A_0 > 64) {
				return (byte) (A_0 - 55);
			}

			if (A_0 > 47) {
				return (byte) (A_0 - 48);
			}

			return 0;
		}

		private static byte[] a(IList<byte> A_0) {
			byte[] array = new byte[16];
			for (int i = 0; i < 16; i++) {
				array[i] = (byte) ((a(A_0[i * 2]) << 4) | a(A_0[i * 2 + 1]));
			}

			return array;
		}

		private static void a(ref byte[] A_0, int A_1, int A_2) {
			byte b = A_0[A_1];
			A_0[A_1] = A_0[A_2];
			A_0[A_2] = b;
		}
	}
}

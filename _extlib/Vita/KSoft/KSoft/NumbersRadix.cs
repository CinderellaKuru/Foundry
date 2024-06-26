﻿
namespace KSoft
{
	/// <summary>Valid numerical bases (radix) that can be used in this library suite</summary>
	[EnumBitEncoderDisable]
	public enum NumeralBase : byte
	{
		Binary	= 2,
		Octal	= 8,
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1720:IdentifiersShouldNotContainTypeNames")]
		Decimal = 10,
		Hex		= 16,
	};

	/// <summary>Valid numerical bases (radix) that can be used in <see cref="KSoft.Numbers"/></summary>
	[EnumBitEncoderDisable]
	public enum NumbersRadix : byte
	{
		Binary	= NumeralBase.Binary,
		Octal	= NumeralBase.Octal,
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1720:IdentifiersShouldNotContainTypeNames")]
		Decimal = NumeralBase.Decimal,
		Hex		= NumeralBase.Hex,
		Base36	= 36,
		Base64	= 64,
	};
}

using System.ComponentModel;
namespace LiveSplit.Nestopia {
	public enum SplitType {
		[Description("Equals Value")]
		Equals,
		[Description("Less Than Value")]
		LessThan,
		[Description("Greater Than Value")]
		GreaterThan,
		[Description("Changed Value")]
		Changed,
		[Description("Less Than Previous Value")]
		ChangedLessThan,
		[Description("Greater Than Previous Value")]
		ChangedGreaterThan,
	}
	public enum ValueSize {
		[Description("Unsigned 8 Bits")]
		UInt8,
		[Description("Unsigned 16 Bits")]
		UInt16,
		[Description("Unsigned 32 Bits")]
		UInt32,
		[Description("Signed 8 Bits")]
		Int8,
		[Description("Signed 16 Bits")]
		Int16,
		[Description("Signed 32 Bits")]
		Int32,
	}
}
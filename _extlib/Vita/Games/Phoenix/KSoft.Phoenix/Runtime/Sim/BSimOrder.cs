﻿
using BVector = System.Numerics.Vector4;
using BEntityID = System.Int32;

namespace KSoft.Phoenix.Runtime
{
	sealed class BSimOrder
		: IO.IEndianStreamSerializable
	{
		const int cMaximumWaypoints = 0xC8;

		internal static readonly FreeListInfo kFreeListInfo = new FreeListInfo(cSaveMarker.SimOrder)
		{
			MaxCount=0x4E20,
		};

		public BSimTarget Target { get; private set; }
		public BVector[] Waypoints;
		public BEntityID OwnerID;
		public uint ID, RefCount;
		public float Angle;
		public sbyte Mode;
		public byte Priority;
		public bool AttackMove, OverridePosition, OverrideRange,
			AutoGeneratedAttackMove;

		public BSimOrder()
		{
			Target = new BSimTarget();
		}

		#region IEndianStreamSerializable Members
		public void Serialize(IO.EndianStream s)
		{
			s.Stream(Target);
			BSaveGame.StreamVectorArray(s, ref Waypoints, cMaximumWaypoints);
			s.Stream(ref OwnerID);
			s.Stream(ref ID); s.Stream(ref RefCount);
			s.Stream(ref Angle);
			s.Stream(ref Mode);
			s.Stream(ref Priority);
			s.Stream(ref AttackMove); s.Stream(ref OverridePosition); s.Stream(ref OverrideRange);
			s.Stream(ref AutoGeneratedAttackMove);
		}
		#endregion
	};
}
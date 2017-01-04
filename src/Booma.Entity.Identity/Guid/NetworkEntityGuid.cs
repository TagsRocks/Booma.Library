﻿using GladNet.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Booma.Entity.Identity
{
	//Format: 0xSSEE XXXX IIII IIII
	//[IIII IIII] unsigned 32bit unique identifier.
	//[X] unsued
	//[SS] Server ID. Will probably remain unimplemented for awhile until cross-server is needed.
	//[EE] Entity type. Reserves 255 indicies for possible entity types.
	/// <summary>
	/// Unique GUID identifier for network entites. Based on Blizzard's 64bit GUID implementation
	/// which encodes flags and uses masking to dervive information about a GUID.
	/// See: http://wowwiki.wikia.com/wiki/API_UnitGUID
	/// </summary>
	[GladNetSerializationContract]
	public class NetworkEntityGuid : IEntityIdentifiable
	{
		/// <summary>
		/// Represents an Empty or uninitialized <see cref="NetworkEntityGuid"/>.
		/// </summary>
		public static NetworkEntityGuid Empty { get; } = new NetworkEntityGuid(0);

		/// <summary>
		/// Raw 64bit numerical representation of the GUID.
		/// </summary>
		private ulong rawGuidValue { get; }

		/// <summary>
		/// Indicates the <see cref="EntityType"/> that this <see cref="NetworkEntityGuid"/> is for.
		/// </summary>
		public EntityType EntityType { get { return (EntityType)(byte)((rawGuidValue & 0x00FF000000000000) << 48); } } //mask out to the EE (entity Type) and then shift it down to a byte

		/// <summary>
		/// Indiciates the current GUID of the entity. This is the last chunk represents the actual ID without any type or identifying information.
		/// </summary>
		public int EntityId { get { return (int)(rawGuidValue & 0x00000000FFFFFFFF); } } //FFFF FFFF masks out everything but an unsigned integer. Casts to int. We waste bits this way but we gain considerable perf.

		public NetworkEntityGuid(ulong guidValue)
		{
			rawGuidValue = guidValue;
		}
	}
}

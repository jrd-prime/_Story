// using System;
// using System.Collections.Generic;
// using Newtonsoft.Json;
// using UnityEngine;
//
// namespace _StoryGame.Core.Uids
// {
// 	[Serializable]
// 	public struct Uid : IEquatable<Uid>
// 	{
// 		public static readonly Uid Empty = new();
//
// 		[JsonProperty] [SerializeField] private uint value;
//
// 		private Uid(uint value)
// 		{
// 			this.value = value;
// 		}
//
// 		public bool Equals(Uid other) => value == other.value;
//
// 		public override bool Equals(object obj) => obj is Uid uid && Equals(uid);
//
// 		public override int GetHashCode() => (int)value;
//
// 		public static explicit operator Uid(uint value) => new(value);
//
// 		public static explicit operator uint(Uid uid) => uid.value;
//
// 		public static bool operator ==(Uid a, Uid b) => a.value == b.value;
//
// 		public static bool operator !=(Uid a, Uid b) => a.value != b.value;
//
// 		public override string ToString() => $"Uid #{value}";
//
// 		public static Uid Parse(string value)
// 		{
// 			var tmp = value.Remove(0, 5);
// 			return (Uid)uint.Parse(tmp);
// 		}
//
// 		public static readonly IEqualityComparer<Uid> Comparer = new ValueEqualityComparer();
//
// 		private sealed class ValueEqualityComparer : IEqualityComparer<Uid>
// 		{
// 			public bool Equals(Uid x, Uid y) => x.value == y.value;
//
// 			public int GetHashCode(Uid obj) => (int)obj.value;
// 		}
// 	}
// }
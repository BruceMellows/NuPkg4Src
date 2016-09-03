// NuPkg4Src-Authors: Bruce Mellows
// NuPkg4Src-Description: Provides a tuple a static type (that can be safely compared using reference equals)
// NuPkg4Src-Tags: CSharp Source Static Tuple
// NuPkg4Src-Id: ExampleLibrary.StaticTuple
// NuPkg4Src-ContentPath: ExampleLibrary
// NuPkg4Src-Hash: SHA512Managed:0204D3794253CDA2A87EC0A4BECB5EB38203DD091A844EC3D04D1DD9A3A243D5FF9C4D557732AB93C6C274067201EAC5E907CFF074D40264E21540CEA280D135
// NuPkg4Src-Version: 1.0.0
// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace ExampleLibrary
{
    using System;

    internal sealed class StaticTuple
    {
        public static StaticTuple<TItem1, TItem2> Create<TItem1, TItem2>(TItem1 item1, TItem2 item2)
            where TItem1 : class
            where TItem2 : class
        {
            return new StaticTuple<TItem1, TItem2>(item1, item2);
        }
    }

    public sealed class StaticTuple<TItem1, TItem2> : IEquatable<StaticTuple<TItem1, TItem2>>
        where TItem1 : class
        where TItem2 : class
    {
        private StaticTuple()
        {
        }

        internal StaticTuple(TItem1 item1, TItem2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        public TItem1 Item1 { get; private set; }

        public TItem2 Item2 { get; private set; }

        public static bool operator ==(StaticTuple<TItem1, TItem2> a, StaticTuple<TItem1, TItem2> b)
        {
            return ReferenceEquals(a, b) || a.Equals(b);
        }

        public static bool operator !=(StaticTuple<TItem1, TItem2> a, StaticTuple<TItem1, TItem2> b)
        {
            return !(ReferenceEquals(a, b) || a.Equals(b));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as StaticTuple<TItem1, TItem2>);
        }

        public bool Equals(StaticTuple<TItem1, TItem2> that)
        {
            return !ReferenceEquals(that, null) && ReferenceEquals(this.Item1, that.Item1) && ReferenceEquals(this.Item2, that.Item2);
        }

        public override int GetHashCode()
        {
            return this.Item1.GetHashCode() ^ this.Item2.GetHashCode();
        }
    }
}

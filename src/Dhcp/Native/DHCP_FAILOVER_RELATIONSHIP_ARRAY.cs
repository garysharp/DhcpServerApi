using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Dhcp.Native
{
    internal readonly struct DHCP_FAILOVER_RELATIONSHIP_ARRAY : IDisposable
    {
        /// <summary>
        /// This member is of type DWORD and contains the number of DHCP_FAILOVER_RELATIONSHIP elements specified in the subsequent pRelationships field.
        /// </summary>
        public readonly int NumElements;
        /// <summary>
        /// This member is a pointer to an array of DHCP_FAILOVER_RELATIONSHIP structures of length numElements and contains failover relationship information.
        /// </summary>
        private readonly IntPtr RelationshipsPointer;

        public IEnumerable<DHCP_FAILOVER_RELATIONSHIP> Relationships
        {
            get
            {
                if (NumElements == 0 || RelationshipsPointer == IntPtr.Zero)
                    yield break;

                var iter = RelationshipsPointer;
                var size = Marshal.SizeOf(typeof(DHCP_FAILOVER_RELATIONSHIP));
                for (var i = 0; i < NumElements; i++)
                {
                    yield return iter.MarshalToStructure<DHCP_FAILOVER_RELATIONSHIP>();
                    iter += size;
                }
            }
        }

        public void Dispose()
        {
            foreach (var relationship in Relationships)
                relationship.Dispose();

            Api.FreePointer(RelationshipsPointer);
        }
    }
}

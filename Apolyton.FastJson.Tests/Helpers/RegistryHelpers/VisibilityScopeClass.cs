using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;

namespace Apolyton.FastJson.Tests.Helpers.RegistryHelpers
{
    public class VisibilityScopeClass
    {
        [DataMember]
        public int PublicGetSet
        {
            get;
            set;
        }

        [DataMember]
        public int PublicGetInternalSet
        {
            get;
            internal set;
        }

        [DataMember]
        internal int InternalGetSet
        {
            get;
            set;
        }

        private int noGetPublicSet;
        
        [DataMember]
        public int NoGetPublicSet
        {
            set { noGetPublicSet = value; }
        }
    }
}

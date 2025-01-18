using System;
using System.Collections;
using System.Collections.Generic;

namespace Peer2Peer.Password
{
    abstract class Iterator : IEnumerator
    {
        object IEnumerator.Current => Current();
        public abstract int Key();       
        public abstract object Current();
        public abstract bool MoveNext();    
        public abstract void Reset();
    }
}
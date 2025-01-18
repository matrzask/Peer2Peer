using System;
using System.Collections;
using System.Collections.Generic;

namespace Peer2Peer.Password
{
    public abstract class IteratorAggregate : IEnumerable
    {
        public abstract IEnumerator GetEnumerator();
    }
}
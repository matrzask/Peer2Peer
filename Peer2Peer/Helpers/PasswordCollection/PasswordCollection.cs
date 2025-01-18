using System;
using System.Collections;
using System.Collections.Generic;

namespace Peer2Peer.Password
{
    public class PasswordCollection : IteratorAggregate
    {
        List<string> collection = new List<string>();
        bool direction = false;
        public void ReverseDirection(){direction = !direction;}
        public List<string> getItems(){
            return collection;
        }
        public void AddItem(string item){
            collection.Add(item);
        }
        public override IEnumerator GetEnumerator(){
            return new PasswordIterator(this, direction);
        }
    }
}
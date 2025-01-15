using Peer2Peer.Helpers;
using Peer2Peer.Messages;

namespace Peer2Peer.Nodes
{
    public abstract class Node
    {
        public string nodeId;
        private readonly HashSet<string> _completedChunks = new HashSet<string>();


        protected void SendMessage(Message message, Node node)
        {
            message.Execute(node);
        }

        public virtual void WorkCompleted(WorkChunk chunk, string workerId)
        {
            string chunkString = chunk.Length + ":" + chunk.Start;
            _completedChunks.Add(chunkString);

        }
    }
}
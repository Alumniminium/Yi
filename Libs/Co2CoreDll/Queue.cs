// * ************************************************************
// * * START:                                          queue.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * Queued list for the library.
// * queue.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by Saint (tao4229 @ e*pvp)
// * 
// * ************************************************************

using System;

namespace CO2_CORE_DLL
{
    //TODO: This is the original class. More work should be done.
    public class Queue<T>
    {
        private class LinkedListNode<T2>
        {
            public T2 Value;
            public LinkedListNode<T2> Next;

            public LinkedListNode(T2 value) { Value = value; }
        }

        private LinkedListNode<T> First = null;
        private LinkedListNode<T> Last = null;
        private Int32 CountValue;

        public Int32 Count { get { return this.CountValue; } }

        public void Enqueue(T obj)
        {
            lock (this)
            {
                if (First == null)
                {
                    First = new LinkedListNode<T>(obj);
                    Last = First;
                }
                else
                {
                    var Node = new LinkedListNode<T>(obj);
                    Last.Next = Node;
                    Last = Node;
                }
                this.CountValue++;
            }
        }

        public T Dequeue()
        {
            lock (this)
            {
                if (First == null)
                    throw new Exception("No elements left in the Queue");
                else
                {
                    var Node = First.Next;
                    var Return = First.Value;

                    First = Node;
                    this.CountValue--;
                    return Return;
                }
            }
        }

        public T Peek()
        {
            lock (this)
            {
                if (First == null)
                    throw new Exception("No elements left in the Queue");
                else
                    return First.Value;
            }
        }

        public bool Remove(T value)
        {
            lock (this)
            {
                if (First.Value.Equals(value))
                {
                    First = First.Next;
                    this.CountValue--;
                    return true;
                }

                var Node = First;
                while (Node != null && Node.Next != null)
                {
                    if (Node.Next.Value.Equals(value))
                    {
                        Node.Next = Node.Next.Next;
                        this.CountValue--;
                        return true;
                    }
                    Node = Node.Next;
                }
                return false;
            }
        }
    }
}

// * ************************************************************
// * * END:                                            queue.cs *
// * ************************************************************
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSystems
{
    internal class ForwardEnumerator : IEnumerator<object>
    {
        private int index;
        private int numMoves;
        private List<object> str;

        public ForwardEnumerator(List<object> str, int index)
        {
            this.str = str;
            this.index = index;
            this.numMoves = -1;
        }

        public object Current
        {
            get
            {
                return str[index + numMoves];
            }
        }

        public void Dispose()
        {
            this.str = null;
        }

        public bool MoveNext()
        {
            ++this.numMoves;
            return this.index + this.numMoves < this.str.Count;
        }

        public void Reset()
        {
            numMoves = -1;
        }

        public int NumMoves
        {
            get { return numMoves + 1; }
        }
    }    
}

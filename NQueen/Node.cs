using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NQueen
{
    public class Node
    {
        public class QueenPositions
        {
            #region Fields
            private int N;
            private int[] _positions;
            private int _attackValue = -1;
            int i, j, x, y;
            #endregion

            #region Constructor(s)
            public QueenPositions(int N)
            {
                _positions = new int[N];
                this.N = N;
            }
            #endregion

            public int this[int idx]
            {
                get
                {
                    return _positions[idx];
                }
                set
                {
                    _attackValue = -1;
                    _positions[idx] = value;
                }
            }

            public int AttackValue
            {
                get
                {
                    if (_attackValue < 0)
                    {

                        _attackValue = 0;
                        for (i = 0; i < N - 1; i++)
                        {
                            j = _positions[i];

                            //Check Right
                            for (x = i + 1; x < N; x++)
                                if (_positions[x] == j)
                                    _attackValue++;

                            //Check Diagonal Down-Right
                            for (y = j + 1, x = i + 1; y < N && x < N; y++, x++)
                                if (_positions[x] == y)
                                    _attackValue++;

                            //Check Diagonal Up-Right
                            for (y = j - 1, x = i + 1; y > -1 && x < N; y--, x++)
                                if (_positions[x] == y)
                                    _attackValue++;
                        }
                    }
                    return _attackValue;
                }
            }

            public int Length
            {
                get
                {
                    return N;
                }
            }
        }

        #region Fields
        int i, j, x, y;
        private int N;
        public QueenPositions Positions;
        #endregion

        public int Value
        {
            get
            {
                return Positions.AttackValue;
            }
        }

        public bool Equals(Node other)
        {
            for (i = 0; i < N; i++)
                if (this.Positions[i] != other.Positions[i])
                    return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            Node other = obj as Node;
            if (other == null)
                return false;

            return Equals(other);
        }

        public override string ToString()
        {
            string s = "{ ";
            for (int i = 0; i < N; i++)
                s += Positions[i].ToString() + " ";
            s += "} \n";

            for (int j = 0; j < N; j++)
            {
                for (int i = 0; i < N; i++)
                {
                    if (Positions[i] == j)
                        //if (grid[j, i] != 0)
                        s += "♥ ";
                    else s += "0" + " ";
                }
                s += "\n";
            }
            return s;
        }

        public Node Copy()
        {
            Node n = new Node(N);
            for (int i = 0; i < N; i++)
                n.Positions[i] = Positions[i];
            //n._attackValue = _attackValue;
            return n;
        }

        public Node(int N)
        {
            this.N = N;
            this.Positions = new QueenPositions(N);            
        }
    }
}

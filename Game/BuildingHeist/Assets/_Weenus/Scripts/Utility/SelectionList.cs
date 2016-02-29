using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GiveUp.Core
{
    public class SelectionList<T> : List<T>
    {

        public event EventHandler EndOfList;

        private void onEndOfList()
        {
            if (EndOfList != null)
                EndOfList(this, EventArgs.Empty);
        }

        public SelectionList() : base() { }

        public SelectionList(IEnumerable<T> coll) : base(coll) { }

        public bool AllowRollOver = false;

        int currIndex = 0;
        public int CurrentIndex
        {
            get { return currIndex; }
            set { currIndex = value; }
        }

        public bool IsNextAvailable()
        {
            return (this.CurrentIndex < (this.Count - 1));
        }

        public void Next()
        {
            if (Count < 1)
                return;

            currIndex++;
            EnsureSafeIndex();
        }

        public void Prev()
        {
            if (Count < 1)
                return;

            currIndex--;
            EnsureSafeIndex();
        }

        public T CurrentValue
        {
            get
            {
                if (Count < 1)
                    throw new Exception("Cannot get current value of an empty collection");

                EnsureSafeIndex();

                return this[currIndex];
            }
        }

        private void EnsureSafeIndex()
        {
            if (Count < 1)
                return;

            if (currIndex >= this.Count)
            {
                if (AllowRollOver)
                    currIndex = 0;
                else
                    currIndex = Count - 1;

                onEndOfList();
            }

            if (currIndex < 0)
                if (AllowRollOver)
                    currIndex = Count - 1;
                else
                    currIndex = 0;
        }


        public void ResetIndex()
        {
            currIndex = 0;
        }


    }

}

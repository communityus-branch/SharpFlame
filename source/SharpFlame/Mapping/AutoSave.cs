

using System;


namespace SharpFlame.Mapping
{
    public class AutoSave
    {
        public int ChangeCount;
        public DateTime SavedDate;

        public AutoSave()
        {
            SavedDate = DateTime.Now;
        }
    }
}
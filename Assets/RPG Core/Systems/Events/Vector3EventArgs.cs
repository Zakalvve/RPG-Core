using UnityEngine;

namespace RPG.Core.Events
{
    public class Vector3EventArgs : RPGEventArgs
    {
        public Vector3EventArgs(Vector3 vector, string id = null) : base(id)
        {
            Vector = vector;
        }
        public Vector3 Vector;
    }
}

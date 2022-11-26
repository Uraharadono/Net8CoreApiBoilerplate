using System;

namespace Net7CoreApiBoilerplate.Infrastructure.Events
{
    public class EntityEventArgs : EventArgs
    {
        public Type[] Types { get; }

        public EntityEventArgs(Type[] types)
        {
            Types = types;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class ObjectUtil
    {

        public static T Also<T>(this T self, Action<T> block)
        {
            block(self);
            return self;
        }

        public static R Let<T, R>(this T self, Func<T, R> func)
        {
            return func(self);
        }
    }
}

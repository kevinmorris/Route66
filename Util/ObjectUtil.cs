using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class ObjectUtil
    {
        /// <summary>
        /// Mimicking Kotlin's <code>also</code> function
        /// </summary>
        /// <see href="https://kotlinlang.org/docs/scope-functions.html"></see>
        /// <typeparam name="T">owner type</typeparam>
        /// <param name="self">owner object</param>
        /// <param name="block">the block</param>
        /// <returns>always returns <code>self</code></returns>
        public static T Also<T>(this T self, Action<T> block)
        {
            block(self);
            return self;
        }

        /// <summary>
        /// Mimicking Kotlin's <code>let</code> function
        /// </summary>
        /// <see href="https://kotlinlang.org/docs/scope-functions.html"></see>
        /// <typeparam name="T">owner type</typeparam>
        /// <typeparam name="R">return type</typeparam>
        /// <param name="self">owner object</param>
        /// <param name="func">the block</param>
        /// <returns>always returns <code></code></returns>
        public static R Let<T, R>(this T self, Func<T, R> func)
        {
            return func(self);
        }
    }
}

/*
 * Phosphorus Five, copyright 2014 - 2015, Thomas Hansen, isa.lightbringer@gmail.com
 * Phosphorus Five is licensed under the terms of the MIT license, see the enclosed LICENSE file for details
 */

using System.Collections.Generic;
using System.Linq;
using p5.core;

namespace p5.exp.iterators
{
    /// <summary>
    ///     Returns all nodes matching the given modulo.
    /// 
    ///     Will return all n'th node from previous Iterator result, where "n" is defined through a modulo iterator, for instance; /%2
    ///     to return all "even nodes" from previous iterator.
    /// 
    ///     Example;
    ///     <pre>/%2</pre>
    /// </summary>
    public class IteratorModulo : Iterator
    {
        private readonly int _modulo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="phosphorus.expressions.iterators.IteratorModulo" /> class.
        /// </summary>
        /// <param name="modulo">modulo</param>
        public IteratorModulo (int modulo)
        {
            _modulo = modulo;
        }

        public override IEnumerable<Node> Evaluate
        {
            get
            {
                var idxNo = 0;
                return Left.Evaluate.Where (idxCurrent => idxNo++%_modulo == 0);
            }
        }
    }
}
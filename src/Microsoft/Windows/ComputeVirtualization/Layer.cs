using System;

namespace Microsoft.Windows.ComputeVirtualization
{
    /// <summary>
    /// Represents a read-only storage layer that acts as the parent of a sandbox or other layer.
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// The ID of the layer. It does not matter what it is, but once a sandbox or layer references a parent
        /// with a given ID, the same ID must always be used.
        /// </summary>
        public Guid Id;

        /// <summary>
        /// The path to the layer.
        /// </summary>
        public string Path;
    }
}

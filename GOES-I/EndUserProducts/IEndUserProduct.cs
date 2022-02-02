using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GOES_I.EndUserProducts
{
    internal interface IEndUserProduct
    {
        /// <summary>
        /// The name of the end user product, must be valid to use in a path
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is an end user process complete
        /// </summary>
        /// <returns>True if the process has been completed</returns>
        public bool IsProcessComplete(string basePath);

        /// <summary>
        /// Get the end user product requirements
        /// </summary>
        /// <returns>An array containing every requirements</returns>
        public GoesProduct[] GetRequirements();

        /// <summary>
        /// Process the end user product
        /// </summary>
        /// <param name="rawProductsPath">Path to the folder containing the raw products</param>
        /// <returns></returns>
        public Task Process(string rawProductsPath);

        /// <summary>
        /// Has the end user product all raw requirements
        /// </summary>
        /// <param name="rawProductsPath"></param>
        /// <returns></returns>
        public bool HasRequirements(string rawProductsPath);
    }
}

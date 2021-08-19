// -----------------------------------------------------------------------
// <copyright file="BaseContext.cs" company="Îakaré Software'oka">
//     Copyright (c) Îakaré Software'oka. All rights reserved. Licensed under the MIT license. See
//     LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace CoreLibrary.Context
{

    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Base Context Class.
    /// </summary>
    public class BaseContext : DbContext
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="BaseContext" />.
        /// </summary>
        public BaseContext() => _ = (Database?.EnsureCreated());

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="BaseContext" />.
        /// </summary>
        /// <param name="options">
        /// Opções do DbContext.
        /// </param>
        public BaseContext(DbContextOptions options) : base(options)
        {
            _ = (Database?.EnsureCreated());
        }
    }
}
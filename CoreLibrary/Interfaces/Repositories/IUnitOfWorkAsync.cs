// <copyright file="IUnitOfWorkAsync.cs" company="Îakaré Software'oka">
//     Copyright (c) Îakaré Software'oka. All rights reserved. Licensed under the MIT license. See
//     LICENSE file in the project root for full license information.
// </copyright>

namespace CoreLibrary.Interfaces.Repositories
{
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// Classe para servir de interface no salvamento do banco de dados.
    /// </summary>
    public interface IUnitOfWorkAsync
    {
        /// <summary>
        /// Inicia transação com o banco de dados.
        /// </summary>
        /// <returns>
        /// Retorna a transação.
        /// </returns>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// Comita a transação do banco.
        /// </summary>
        /// <param name="transaction">
        /// Transação aberta.
        /// </param>
        /// <returns>
        /// Retorna a task.
        /// </returns>
        Task CommitTransactionAsync(IDbContextTransaction transaction);

        /// <summary>
        /// Reverte alterações.
        /// </summary>
        void RollbackTransaction();
    }
}

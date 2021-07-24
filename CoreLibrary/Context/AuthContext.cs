namespace CoreLibrary.Context
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using CoreLibrary.Interfaces;

    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// Base Auth Context.
    /// </summary>
    public class AuthContext : IdentityDbContext, IUnitOfWork
    {
        /// <summary>
        /// Inicia uma nova instância da classe <see cref="BaseContext" />.
        /// </summary>
        public AuthContext() => _ = (Database?.EnsureCreated());

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="BaseContext" />.
        /// </summary>
        /// <param name="options">
        /// Opções do DbContext.
        /// </param>
        public AuthContext(DbContextOptions options) : base(options)
        {
            _ = (Database?.EnsureCreated());
        }

        /// <summary>
        /// Obtém a transação atual.
        /// </summary>
        /// <returns>
        /// Transação atual.
        /// </returns>
        public IDbContextTransaction? CurrentTransaction { get; private set; }

        /// <summary>
        /// Indica se existe transação.
        /// </summary>
        public bool HasActiveTransaction => CurrentTransaction != null;

        /// <inheritdoc />
        public IDbContextTransaction BeginTransaction()
        {
            if (CurrentTransaction != null)
                return CurrentTransaction;

            CurrentTransaction = Database.BeginTransaction(IsolationLevel.ReadCommitted);

            return CurrentTransaction;
        }

        /// <inheritdoc />
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (CurrentTransaction != null)
                return CurrentTransaction;

            CurrentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted).ConfigureAwait(true);

            return CurrentTransaction;
        }

        /// <inheritdoc />
        public void CommitTransaction(IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if (transaction != CurrentTransaction)
                throw new InvalidOperationException($"Transação {transaction.TransactionId} não é a atual.");

            try
            {
                bool isObjectSaved = SaveEntities();

                if (isObjectSaved)
                    transaction.Commit();
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw new DbUpdateException(ex.Message);
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        /// <inheritdoc />
        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            if (transaction != CurrentTransaction)
            {
                throw new InvalidOperationException($"Transação {transaction.TransactionId} não é a atual.");
            }

            try
            {
                bool isObjectSavedAsync = await SaveEntitiesAsync().ConfigureAwait(true);

                if (isObjectSavedAsync)
                {
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw new DataException(ex.Message);
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        /// <inheritdoc />
        public void RollbackTransaction()
        {
            try
            {
                CurrentTransaction?.Rollback();
            }
            finally
            {
                if (CurrentTransaction != null)
                {
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                }
            }
        }

        /// <inheritdoc />
        public bool SaveEntities()
        {
            return SaveChanges() > 0;
        }

        /// <inheritdoc />
        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            return await SaveChangesAsync(cancellationToken).ConfigureAwait(true) > 0;
        }
    }
}

// <copyright file="ValueObject.cs" company="Îakaré Software'oka">
//     Copyright (c) Îakaré Software'oka. All rights reserved. Licensed under the MIT license. See
//     LICENSE file in the project root for full license information.
// </copyright>

namespace CoreLibrary.Models
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Classe base para objetos de valor.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Retorna se objeto a ser comparado com atual é igual.
        /// </summary>
        /// <param name="obj">
        /// Objeto a ser comparado.
        /// </param>
        /// <returns>
        /// Verdadeiro caso igual.
        /// Falso caso diferente.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            ValueObject? other = (ValueObject)obj;

            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Gera um hash baseado nos itens do objeto.
        /// </summary>
        /// <returns>
        /// Retorna o hash da operação.
        /// </returns>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// Verifica se um objeto é igual a outro.
        /// </summary>
        /// <param name="left">
        /// Objeto original.
        /// </param>
        /// <param name="right">
        /// Objeto a ser comparado.
        /// </param>
        /// <returns>
        /// Verdadeiro caso igual.
        /// Falso caso diferente.
        /// </returns>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            return !(left is null ^ right is null) && (left is null || left.Equals(right));
        }

        /// <summary>
        /// Verifica se um objeto é diferente do outro.
        /// </summary>
        /// <param name="left">
        /// Objeto original.
        /// </param>
        /// <param name="right">
        /// Objeto a ser comparado.
        /// </param>
        /// <returns>
        /// Verdadeiro caso diferente.
        /// Falso caso igual.
        /// </returns>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        /// <summary>
        /// Busca componentes dos objetos.
        /// </summary>
        /// <returns>
        /// Retorna lista dos componentes.
        /// </returns>
        protected abstract IEnumerable<object> GetEqualityComponents();
    }
}
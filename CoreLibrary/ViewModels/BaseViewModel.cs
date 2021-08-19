namespace CoreLibrary.ViewModels
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// ViewModel base.
    /// </summary>
    public class BaseViewModel
    {
        /// <summary>
        /// Identificador padrão de entidades.
        /// </summary>
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }
    }
}
namespace CoreLibrary.Enums
{
    using System.ComponentModel;

    /// <summary>
    /// Enumerador com papéis básicos de usuário.
    /// </summary>
    public enum EPapeis
    {
        /// <summary>
        /// Papel de administrador.
        /// </summary>
        [Description("Administrador")]
        Admim,

        /// <summary>
        /// Papel de desenvolvedor.
        /// </summary>
        [Description("Desenvolvedor")]
        Developer,
    }
}
